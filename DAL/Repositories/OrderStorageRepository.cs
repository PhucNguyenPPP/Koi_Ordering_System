using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class OrderStorageRepository : GenericRepository<OrderStorage>, IOrderStorageRepository
    {
        public OrderStorageRepository(KoiDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<OrderShipperDTO>> GetAssignedOrdersByShipper(Guid shipperId)
        {
            var assignedOrders = await _dbSet
                .Where(os => os.ShipperId == shipperId)
                .Include(os => os.Order)
                .ThenInclude(o => o.Customer)
                .Include(os => os.Order)
                .ThenInclude(o => o.Kois) // Assuming you want the farm data through Kois
                .Select(os => new OrderShipperDTO
                {
                    OrderId = os.Order.OrderId,
                    OrderNumber = os.Order.OrderNumber,
                    FarmId = os.Order.Kois.FirstOrDefault().FarmId, // Assuming relation through Koi
                    FarmName = os.Order.Kois.FirstOrDefault().Farm.FarmName,
                    CreatedDate = os.Order.CreatedDate,
                    TotalPrice = os.Order.TotalPrice,
                    Status = os.Order.Status,
                    CustomerId = os.Order.CustomerId,
                    CustomerName = os.Order.Customer.FullName
                })
                .ToListAsync();

            return assignedOrders;
        }
    }
}
