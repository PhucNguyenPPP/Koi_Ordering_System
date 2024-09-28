using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;

namespace DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly KoiDbContext _context;
        public UnitOfWork()
        {
            _context = new KoiDbContext();
            Breed = new BreedRepository(_context);
            Role = new RoleRepository(_context);
            User = new UserRepository(_context);
            RefreshToken = new RefreshTokenRepository(_context);
            Koi = new KoiRepository(_context);
            Order = new OrderRepository(_context);
            Policy = new PolicyRepository(_context);
        }


        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<bool> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public IBreedRepository Breed { get; private set; }
        public IRoleRepository Role { get; private set; }
        public IUserRepository User { get; private set; }
        public IRefreshTokenRepository RefreshToken { get; private set; }
        public IKoiRepository Koi { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IPolicyRepository Policy { get; private set; }
    }
}