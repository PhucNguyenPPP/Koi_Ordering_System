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
                .GroupBy(os => os.Order.OrderId) // Group by OrderId
                .Select(group => new OrderShipperDTO
                {
                    OrderId = group.FirstOrDefault().Order.OrderId,
                    OrderNumber = group.FirstOrDefault().Order.OrderNumber,
                    FarmId = group.FirstOrDefault().Order.Kois.FirstOrDefault().FarmId, // Assuming relation through Koi
                    FarmName = group.FirstOrDefault().Order.Kois.FirstOrDefault().Farm.FarmName,
                    CreatedDate = group.FirstOrDefault().Order.CreatedDate,
                    TotalPrice = group.FirstOrDefault().Order.TotalPrice, // Sum total price if needed
                    Status = group.FirstOrDefault().Order.Status,
                    CustomerId = group.FirstOrDefault().Order.CustomerId,
                    CustomerName = group.FirstOrDefault().Order.Customer.FullName
                })
                .ToListAsync();

            return assignedOrders;
        }
    }
}
