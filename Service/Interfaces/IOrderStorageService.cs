using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.General;
using Common.DTO.OrderStorage;

namespace Service.Interfaces
{
    public interface IOrderStorageService
    {
        Task<ResponseDTO> AssignShipperJapan(AssignShipperDTO assignShipperDTO);
        Task<ResponseDTO> AssignShipperVietnam(AssignShipperDTO assignShipperDTO);

        Task<ResponseDTO> ConfirmDelivery(ConfirmDeliveryDTO confirmDeliveryDTO);
        Task<ResponseDTO> GetDeliveryOfOrder(Guid orderId);
        public Task<IEnumerable<OrderShipperDTO>> GetOrdersForShipper(Guid shipperId);
    }
}
