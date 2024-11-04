using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class RefundRequestMediumRepository : GenericRepository<RefundRequestMedium>, IRefundRequestMediumRepository
    {
        public RefundRequestMediumRepository(KoiDbContext context) : base(context)
        {
        }
    }
}