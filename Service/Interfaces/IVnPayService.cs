using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IVnPayService
    {
        Task<string> CreatePaymentUrl(Guid OrderId, HttpContext context);
    }
}
