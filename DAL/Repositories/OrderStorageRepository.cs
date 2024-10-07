using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class OrderStorageRepository : GenericRepository<OrderStorage>, IOrderStorageRepository
    {
        public OrderStorageRepository(KoiDbContext context) : base(context)
        {
        }
    }
}
