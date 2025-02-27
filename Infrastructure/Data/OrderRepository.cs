using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class OrderRepository(StoreContext context) : IOrderRepository
{
    public async Task<IReadOnlyList<Order>> GetAllOrders()
    {
        return await context.Orders
            .Include(x => x.DeliveryMethod)
            .Include(x => x.OrderItems)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await context.Orders
            .Include(x => x.DeliveryMethod)
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
