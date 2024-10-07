using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class ShippingFeeRepository : GenericRepository<ShippingFee>, IShippingFeeRepository
    {
        public ShippingFeeRepository(KoiDbContext context) : base(context)
        {
        }
    }
}
