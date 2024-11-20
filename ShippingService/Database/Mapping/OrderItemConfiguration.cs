using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Orders;

namespace ShippingService.Database.Mapping;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
	public void Configure(EntityTypeBuilder<OrderItem> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Product).IsRequired();
		builder.Property(x => x.Quantity).IsRequired();

		builder.HasOne(x => x.Order)
			.WithMany(x => x.Items)
			.HasForeignKey(x => x.OrderId)
			.IsRequired();
	}
}
