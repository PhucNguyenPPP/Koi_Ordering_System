using Common.DTO.Cart;
using Common.DTO.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ICartService
    {
        Task<bool> AddKoiToCart(CartDTO cartDTO);
        Task<ResponseDTO> CheckValidationCart(CartDTO cartDTO);
        List<GetCartDTO>? GetCartByUser(Guid userId);
        Task<bool> DeleteCart(Guid cartId);
    }
}
