
using Core.Entities.OrderAggregate;

namespace Core.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllOrders();

    Task<Order?> GetOrderByIdAsync(int id);
}
