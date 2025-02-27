using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(x => x.ShippingAdderss, o => o.WithOwner());
        builder.OwnsOne(x => x.PaymentSummary, o => o.WithOwner());
        builder.Property(x => x.Status).HasConversion(
            o => o.ToString(),
            o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
        );
        builder.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
        builder.HasMany(x => x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.OrderDate).HasConversion(
            d => d.ToUniversalTime(),
            d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
        );
    }
}

// Configuration for the Order entity:
// - `ShippingAddress` and `PaymentSummary` are owned types, meaning their properties are stored 
//   in the `Order` table instead of separate tables. We use `OwnsOne` to explicitly define this.
// - `Status` (an enum) is stored as a string instead of an integer to avoid issues if enum values change.
// - `Subtotal` is configured as `decimal(18,2)` to ensure proper precision for currency values.
// - `OrderItems` has a one-to-many relationship with `Order`, and cascading delete is enforced 
//   so that deleting an order removes its associated items.
// - `OrderDate` is stored in UTC format to maintain consistency across different time zones.
// DeliveryMethod : BaseEntity => Reference Navigation Property (Entity)