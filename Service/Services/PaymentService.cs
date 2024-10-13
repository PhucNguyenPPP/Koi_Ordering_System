using Common.Constant;
using Common.DTO.Payment;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Service.Interface;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVnPayService _vnPayService;
        public PaymentService(IUnitOfWork unitOfWork, IVnPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _vnPayService = vnPayService;
        }
        public async Task<string> CreatePaymentVNPayRequest(Guid orderId, HttpContext context)
        {
            var order = await _unitOfWork.Order.GetByCondition(c => c.OrderId == orderId);
            if (order == null)
            {
                return string.Empty;
            }

            var unpaidTransList = _unitOfWork.Transaction
                .GetAllByCondition(c => c.OrderId == orderId
                && c.Status == PaymentConstant.Unpaid);

            var unpaidTrans = (unpaidTransList != null && unpaidTransList.Any()) ? unpaidTransList.First() : null;

            if (unpaidTrans != null)
            {
                unpaidTrans.Status = PaymentConstant.CancelStatus;
                _unitOfWork.Transaction.Update(unpaidTrans);
            }

            await _unitOfWork.Transaction.AddAsync(new Transaction()
            {
                TransactionId = Guid.NewGuid(),
                PaymentMethodTransaction = PaymentConstant.VnPay,
                TransactionInfo = PaymentConstant.UnSet,
                TransactionNumber = PaymentConstant.UnSet,
                CreatedDate = DateTime.Now,
                Status = PaymentConstant.Unpaid,
                OrderId = orderId
            });

            await _unitOfWork.SaveChangeAsync();
            return await _vnPayService.CreatePaymentUrl(orderId, context);
        }

        public async Task<bool> HandlePaymentResponse(VnPayResponseDTO responseDTO)
        {
            var order = await _unitOfWork.Order.GetByCondition(c => c.OrderNumber == responseDTO.OrderNumber);
            if (order == null)
            {
                return false;
            }

            var unpaidTransList = _unitOfWork.Transaction
                .GetAllByCondition(c => c.OrderId == order.OrderId
                && c.Status == PaymentConstant.Unpaid);

            var unpaidTrans = (unpaidTransList != null && unpaidTransList.Any()) ? unpaidTransList.First() : null;

            if (unpaidTrans != null && unpaidTrans.Status == PaymentConstant.Unpaid)
            {
                if (responseDTO.IsSuccess)
                {
                    // update trans
                    unpaidTrans.Status = PaymentConstant.PaidStatus;
                    unpaidTrans.TransactionInfo = responseDTO.TransactionInfo;
                    unpaidTrans.TransactionNumber = responseDTO.TransactionNumber;
                }
                else
                {
                    // update trans
                    unpaidTrans.TransactionInfo = responseDTO.TransactionInfo;
                    unpaidTrans.TransactionNumber = responseDTO.TransactionNumber;
                    unpaidTrans.Status = PaymentConstant.CancelStatus;
                }

                _unitOfWork.Transaction.Update(unpaidTrans);

                order.Status = OrderStatusConstant.Processing;
                _unitOfWork.Order.Update(order);
                await _unitOfWork.SaveChangeAsync();
                return true;
            }

            return false;
        }
    }
}
