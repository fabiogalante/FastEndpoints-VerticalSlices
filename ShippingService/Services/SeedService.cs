using Bogus;
using Microsoft.EntityFrameworkCore;
using ShippingService.Database;
using ShippingService.Domain.Customers;
using ShippingService.Domain.Orders;
using ShippingService.Domain.Shipments;

namespace ShippingService.Services;

public class SeedService(ShippingDbContext context)
{
	public async Task SeedDataAsync()
	{
		if (await context.Shipments.CountAsync(_ => true) > 0)
		{
			return;
		}

		var fakeCustomers = new Faker<Customer>()
			.CustomInstantiator(f => Customer.Create(
				f.Name.FirstName(),
				f.Name.LastName(),
				f.Internet.Email(),
				f.Phone.PhoneNumber()
			));

		var customers = fakeCustomers.Generate(5);
		context.Customers.AddRange(customers);
		await context.SaveChangesAsync();

		var fakeOrders = new Faker<Order>()
			.CustomInstantiator(f => Order.Create(
				f.Commerce.Ean8(),
				customers[f.Random.Int(0, customers.Count - 1)],
				Enumerable.Range(1, f.Random.Int(1, 5))
					.Select(_ => new OrderItem(f.Commerce.ProductName(), f.Random.Int(1, 5)))
					.ToList()
			));

		var orders = fakeOrders.Generate(10);
		context.Orders.AddRange(orders);
		await context.SaveChangesAsync();

		var fakeShipments = new Faker<Shipment>()
			.CustomInstantiator(f => Shipment.Create(
				f.Commerce.Ean8(),
				orders[f.Random.Int(0, orders.Count - 1)].Id,
				new Address
				{
					Street = f.Address.StreetAddress(),
					City = f.Address.City(),
					Zip = f.Address.ZipCode()
				},
				f.Commerce.Department(),
				f.Internet.Email(),
				orders[f.Random.Int(0, orders.Count - 1)].Items
					.Select(item => new ShipmentItem
					{
						Product = item.Product,
						Quantity = item.Quantity
					})
					.ToList()
			));

		var shipments = fakeShipments.Generate(10);
		context.Shipments.AddRange(shipments);
		await context.SaveChangesAsync();
	}
}
