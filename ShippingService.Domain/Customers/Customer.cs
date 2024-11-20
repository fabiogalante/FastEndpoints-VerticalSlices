using ShippingService.Domain.Orders;

namespace ShippingService.Domain.Customers;

public class Customer
{
	public Guid Id { get; private set; }
	public string FirstName { get; private set; }
	public string LastName { get; private set; }
	public string Email { get; private set; }
	public string PhoneNumber { get; private set; }
	public IReadOnlyList<Order> Orders => _orders.AsReadOnly();

	private readonly List<Order> _orders = [];

	private Customer() { }

	public static Customer Create(string firstName, string lastName, string email, string phoneNumber)
	{
		return new Customer
		{
			Id = Guid.NewGuid(),
			FirstName = firstName,
			LastName = lastName,
			Email = email,
			PhoneNumber = phoneNumber
		};
	}

	public void AddOrder(Order order)
	{
		_orders.Add(order);
	}
}
