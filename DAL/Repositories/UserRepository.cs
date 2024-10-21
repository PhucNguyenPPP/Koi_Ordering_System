using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(KoiDbContext context) : base(context)
        {
        }
        
        public async Task<ShipperDto[]> GetAllShipperInStorageProvinceAsync(Guid storageProvinceId, Guid roleId)
        {
            // Retrieve users who have the Shipper role and belong to the specified storage province
            var shippers = await _dbSet
                .Where(u => u.StorageProvinceId == storageProvinceId && u.RoleId == roleId)
                .Include(u => u.StorageProvince) // Include StorageProvince to access StorageName
                .Select(u => new ShipperDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    AvatarLink = u.AvatarLink,
                    Phone = u.Phone,
                    Address = u.Address,
                    Email = u.Email,
                    Gender = u.Gender,
                    StorageName = u.StorageProvince.ProvinceName
                })
                .ToArrayAsync();

            return shippers;
        }
    }
}
