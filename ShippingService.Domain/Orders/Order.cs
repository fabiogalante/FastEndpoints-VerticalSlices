using ShippingService.Domain.Customers;

namespace ShippingService.Domain.Orders;

public class Order
{
	public Guid Id { get; private set; }

	public string OrderNumber { get; private set; }

	public Guid CustomerId { get; private set; }

	public Customer Customer { get; private set; }

	public DateTime Date { get; private set; }

	public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

	private readonly List<OrderItem> _items = new();

	private Order() { }

	public static Order Create(string orderNumber, Customer customer, List<OrderItem> items)
	{
		return new Order
		{
			Id = Guid.NewGuid(),
			OrderNumber = orderNumber,
			Customer = customer,
			CustomerId = customer.Id,
			Date = DateTime.UtcNow
		}.AddItems(items);
	}

	private Order AddItems(List<OrderItem> items)
	{
		_items.AddRange(items);
		return this;
	}
}
