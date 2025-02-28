using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class OrdersController(ICartService cartService, IUnitOfWork unit, IProductRepository productRepo, IOrderRepository orderRepo) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto createOrderDto)
    {
        var email = User.GetEmail();

        var cart = await cartService.GetCartAsync(createOrderDto.CartId);

        if (cart == null) return BadRequest("Cart not found");

        if (cart.PaymentIntentId == null) return BadRequest("No payment intent for this order");

        var items = new List<OrderItem>();

        foreach (var item in cart.Items)
        {
            var productItem = await productRepo.GetProductByIdAsync(item.ProductId);

            if (productItem == null) return BadRequest("Problem with the order");

            var itemOrdered = new ProductItemOrdered
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                PictureUrl = item.PictureUrl
            };

            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quantity = item.Quantity
            };
            items.Add(orderItem);
        }

        var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(createOrderDto.DeliveryMethodId);

        if (deliveryMethod == null) return BadRequest("No delivery method selectec");

        var order = new Order
        {
            OrderItems = items,
            DeliveryMethod = deliveryMethod,
            ShippingAdderss = createOrderDto.ShippingAddress,
            Subtotal = items.Sum(x => x.Price * x.Quantity),
            PaymentSummary = createOrderDto.PaymentSummary,
            PaymentIntentId = cart.PaymentIntentId,
            BuyerEmail = email
        };

        unit.Repository<Order>().Add(order);

        if (await unit.Complete())
        {
            return order;
        }

        return BadRequest("Problem creating order");
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
    {
        var email = User.GetEmail();

        var orders = await orderRepo.GetAllOrders(email);

        var ordersToReturn = orders.Select(x => x.ToDto()).ToList();

        return Ok(ordersToReturn);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        var email = User.GetEmail();

        var order = await orderRepo.GetOrderByIdAsync(id, email);

        if(order == null) return NotFound();

        return order.ToDto();
    }
}
