using System;
using Core.Entities.OrderAggregate;

namespace Core.Specifications;

public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(string email) : base(x => x.BuyerEmail == email)
    {

    }

    public OrderSpecification(string paymentIntentId, bool isPaymentIntent) : base(x => x.PaymentIntentId == paymentIntentId)
    {
        // AddInclude("OrderItems");
        // AddInclude("DeliveryMethod");
    }
}
