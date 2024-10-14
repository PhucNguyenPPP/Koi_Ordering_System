using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Repositories;

namespace DAL.Interfaces
{
    public interface IOrderStorageRepository : IGenericRepository <OrderStorage>
    {
    }
}
