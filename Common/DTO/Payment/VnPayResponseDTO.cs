using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Payment
{
    public class VnPayResponseDTO
    {
        public string OrderNumber { get; set; } = null!;
        public string TransactionInfo { get; set; } = null!;
        public string TransactionNumber { get; set; } = null!;
        public bool IsSuccess { get; set; }
    }
}
