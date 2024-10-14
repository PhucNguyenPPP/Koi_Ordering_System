using DAL.UnitOfWork;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ShippingFeeService : IShippingFeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShippingFeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> GetShippingFeeByStorageProvinceId(Guid storageProvinceJapanId, Guid storageProvinceVietnamId)
        {
            var shippingFee = await _unitOfWork.ShippingFee.GetByCondition(c => c.StorageProvinceJpId == storageProvinceJapanId
            && c.StorageProvinceVnId == storageProvinceVietnamId);

            decimal price = 0;
            if(shippingFee == null )
            {
                return price;
            }

            return shippingFee.Price;
        }
    }
}
