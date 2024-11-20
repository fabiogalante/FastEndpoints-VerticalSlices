using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Orders;

namespace ShippingService.Database.Mapping;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder.HasKey(x => x.Id);
		builder.HasIndex(x => x.OrderNumber);

		builder.Property(x => x.OrderNumber).IsRequired();
		builder.Property(x => x.Date).IsRequired();

		builder.HasMany(x => x.Items)
			.WithOne(x => x.Order)
			.HasForeignKey(x => x.OrderId)
			.IsRequired();

		builder.Navigation(x => x.Items)
			.UsePropertyAccessMode(PropertyAccessMode.Field);

		builder.HasOne(x => x.Customer)
			.WithMany(x => x.Orders)
			.HasForeignKey(x => x.CustomerId)
			.IsRequired();
	}
}
