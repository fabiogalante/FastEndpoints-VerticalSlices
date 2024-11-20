using ShippingService.Domain.Shipments;

namespace ShippingService.Features.Shipments.Shared.Responses;

public sealed record ShipmentResponse(
    string Number,
    Guid OrderId,
    Address Address,
    string Carrier,
    string ReceiverEmail,
    ShipmentStatus Status,
    List<ShipmentItemResponse> Items);
