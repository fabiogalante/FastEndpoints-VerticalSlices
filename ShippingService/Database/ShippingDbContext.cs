using Microsoft.EntityFrameworkCore;
using ShippingService.Domain.Customers;
using ShippingService.Domain.Orders;
using ShippingService.Domain.Shipments;

namespace ShippingService.Database;

public class ShippingDbContext(DbContextOptions<ShippingDbContext> options)
	: DbContext(options)
{
	public DbSet<Shipment> Shipments { get; set; }

	public DbSet<ShipmentItem> ShipmentItems { get; set; }

	public DbSet<Order> Orders { get; set; }

	public DbSet<Customer> Customers { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.HasDefaultSchema("shipping-fast-endpoints");

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShippingDbContext).Assembly);
	}
}
