using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public void Dispose();
        public Task<bool> SaveChangeAsync();
        IBreedRepository Breed { get; }
        IRoleRepository Role { get; }
        IUserRepository User { get; }
        IRefreshTokenRepository RefreshToken { get; }
        IKoiRepository Koi { get; }
        IOrderRepository Order { get; }
        IPolicyRepository Policy { get; }
		IFarmImageRepository FarmImage { get; }
        IStorageProvinceRepository StorageProvince { get; }
        ICartRepository Cart { get; }
	}
}