using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class OrderRepository(StoreContext context) : IOrderRepository
{
    public async Task<IReadOnlyList<Order>> GetAllOrders(string email)
    {
        return await context.Orders
            .Where(o => o.BuyerEmail == email)
            .Include(x => x.DeliveryMethod)
            .Include(x => x.OrderItems)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id, string email)
    {
        return await context.Orders
            .Include(x => x.DeliveryMethod)
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id && o.BuyerEmail == email);
    }

    public async Task<Order?> GetOrderByOrderIntentId(string paymentIntentId)
    {
        return await context.Orders
            .Include(x => x.DeliveryMethod)
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
