using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DAL.UnitOfWork;
using DAL.Entities;
using System.Security.Cryptography;
using Common.DTO.Payment;
using Service.Interfaces;

namespace Service.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VNPayCompare());
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public VnPayService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public async Task<string> CreatePaymentUrl(Guid orderId, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]!);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var order = await _unitOfWork.Order.GetByCondition(c => c.OrderId == orderId);
            if (order == null)
            {
                return string.Empty;
            }
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            AddRequestData("vnp_Version", _configuration["Vnpay:Version"]!);
            AddRequestData("vnp_Command", _configuration["Vnpay:Command"]!);
            AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]!);
            AddRequestData("vnp_Amount", (order.TotalPrice * 100).ToString());
            AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]!);
            AddRequestData("vnp_IpAddr", GetIpAddress(context));
            AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]!);
            AddRequestData("vnp_OrderInfo", $"Pay {order.TotalPrice * 100} for {order.OrderNumber} of Koi Shop");
            AddRequestData("vnp_OrderType", $"Pay {order.TotalPrice * 100} for {order.OrderNumber} of Koi Shop");
            AddRequestData("vnp_ReturnUrl", urlCallBack!);
            AddRequestData("vnp_TxnRef", $"{order.OrderNumber}");

            var paymentUrl = CreateRequestUrl(_configuration["Vnpay:BaseUrl"]!, _configuration["Vnpay:HashSecret"]!);

            return paymentUrl;
        }

        private string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "127.0.0.1";
        }
        private void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        private string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();

            foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            var querystring = data.ToString();

            baseUrl += "?" + querystring;
            var signData = querystring;
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }

            var vnpSecureHash = HmacSha512VnPay(vnpHashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnpSecureHash;

            return baseUrl;
        }

        private string HmacSha512VnPay(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
    }
}
