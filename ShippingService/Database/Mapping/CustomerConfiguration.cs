using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Customers;

namespace ShippingService.Database.Mapping;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
	public void Configure(EntityTypeBuilder<Customer> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.FirstName).IsRequired();
		builder.Property(x => x.LastName).IsRequired();
		builder.Property(x => x.Email).IsRequired();
		builder.Property(x => x.PhoneNumber).IsRequired();

		builder.HasMany(x => x.Orders)
			.WithOne(x => x.Customer)
			.HasForeignKey(x => x.CustomerId)
			.IsRequired();

		builder.Navigation(x => x.Orders)
			.UsePropertyAccessMode(PropertyAccessMode.Field);
	}
}
