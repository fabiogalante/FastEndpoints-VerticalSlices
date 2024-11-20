using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ShippingService.Database;
using ShippingService.Features.Shipments.CreateShipment;
using ShippingService.Features.Shipments.Shared.Responses;

namespace ShippingService.Features.Shipments.GetShipmentByNumber;

public record GetShipmentByNumberRequest(string ShipmentNumber);

public class GetShipmentByNumberEndpoint(ShippingDbContext dbContext,
	ILogger<GetShipmentByNumberEndpoint> logger)
	: Endpoint<GetShipmentByNumberRequest, Results<Ok<ShipmentResponse>, NotFound<string>>>
{
	public override void Configure()
	{
		Get("/api/shipments/{ShipmentNumber}");
		AllowAnonymous();
	}

	public override async Task<Results<Ok<ShipmentResponse>, NotFound<string>>> ExecuteAsync(GetShipmentByNumberRequest request, CancellationToken cancellationToken)
	{
		var shipment = await dbContext.Shipments.Include(x => x.Items)
			.FirstOrDefaultAsync(x => x.Number == request.ShipmentNumber, cancellationToken);
		if (shipment is null)
		{
			logger.LogDebug("Shipment with number {ShipmentNumber} not found", request.ShipmentNumber);
			return TypedResults.NotFound($"Shipment with number '{request.ShipmentNumber}' not found");
		}

		var response = shipment.MapToResponse();
		return TypedResults.Ok(response);
	}
}
