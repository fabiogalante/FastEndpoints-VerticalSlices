using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Orders;
using ShippingService.Domain.Shipments;

namespace ShippingService.Database.Mapping;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
	public void Configure(EntityTypeBuilder<Shipment> builder)
	{
		builder.HasKey(x => x.Id);
		builder.HasIndex(x => x.Number);

		builder.Property(x => x.Number).IsRequired();
		builder.Property(x => x.OrderId).IsRequired();
		builder.Property(x => x.Carrier).IsRequired();
		builder.Property(x => x.ReceiverEmail).IsRequired();

		builder.Property(x => x.Status)
			.HasConversion<string>()
			.IsRequired();

		builder.OwnsOne(x => x.Address, ownsBuilder =>
		{
			ownsBuilder.Property(x => x.Street).IsRequired();
			ownsBuilder.Property(x => x.City).IsRequired();
			ownsBuilder.Property(x => x.Zip).IsRequired();
		});

		builder.HasMany(x => x.Items)
			.WithOne(x => x.Shipment)
			.HasForeignKey(x => x.ShipmentId);

		builder.Navigation(x => x.Items)
			.UsePropertyAccessMode(PropertyAccessMode.Field);

		builder.HasOne<Order>()
			.WithMany()
			.HasForeignKey(x => x.OrderId)
			.IsRequired();
	}
}
