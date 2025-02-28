
using Core.Entities.OrderAggregate;

namespace Core.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllOrders(string email);
    Task<Order?> GetOrderByIdAsync(int id, string email);
    Task<Order?> GetOrderByOrderIntentId(string paymentIntentId);
    Task<bool> SaveChangesAsync();
}
