using DAL.Entities;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class BreedRepository : GenericRepository<Breed>, IBreedRepository
    {
        public BreedRepository(KoiDbContext context) : base(context)
        {
        }
    }
}
