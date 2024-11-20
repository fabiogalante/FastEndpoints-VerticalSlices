namespace ShippingService.Domain.Shipments;

public enum ShipmentStatus
{
	Created,
	Processing,
	Dispatched,
	InTransit,
	Delivered,
	Received,
	Cancelled
}
