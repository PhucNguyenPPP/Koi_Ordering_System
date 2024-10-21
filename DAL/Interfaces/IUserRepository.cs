using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<ShipperDto[]>  GetAllShipperInStorageProvinceAsync(Guid storageProvinceId, Guid roleId);
    }
}
