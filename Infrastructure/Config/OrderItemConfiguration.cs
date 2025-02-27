using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(x => x.ItemOrdered, o => o.WithOwner());
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
    }
}

// Configuration for the OrderItem entity:
// - `ItemOrdered` is an owned type, meaning its properties are stored within the `OrderItem` table.
// - `Price` is explicitly set as `decimal(18,2)` to ensure proper precision for monetary values.