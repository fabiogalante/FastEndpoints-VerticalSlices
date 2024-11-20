using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Shipments;

namespace ShippingService.Database.Mapping;

public class ShipmentItemConfiguration : IEntityTypeConfiguration<ShipmentItem>
{
	public void Configure(EntityTypeBuilder<ShipmentItem> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id).ValueGeneratedOnAdd();

		builder.Property(x => x.Product).IsRequired();
		builder.Property(x => x.Quantity).IsRequired();
	}
}
