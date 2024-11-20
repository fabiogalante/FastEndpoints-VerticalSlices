namespace ShippingService.Domain.Orders;

public class OrderItem
{
	public Guid Id { get; private set; }

	public string Product { get; private set; } = null!;

	public int Quantity { get; private set; }

	public Guid OrderId { get; private set; }

	public Order Order { get; private set; } = null!;

	private OrderItem() { }

	public OrderItem(string productName, int quantity)
	{
		Id = Guid.NewGuid();
		Product = productName;
		Quantity = quantity;
	}
}
