using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ShippingService.Database;
using ShippingService.Features.Shipments.Shared;

namespace ShippingService.Features.Shipments.DispatchShipment;

public class DispatchShipmentEndpoint(ShippingDbContext dbContext,
	ILogger<DispatchShipmentEndpoint> logger)
	: EndpointWithoutRequest<Results<NoContent, NotFound<string>, ProblemHttpResult>>
{
	public override void Configure()
	{
		Post("/api/shipments/dispatch/{shipmentNumber}");
		AllowAnonymous();
	}

	public override async Task<Results<NoContent, NotFound<string>, ProblemHttpResult>> ExecuteAsync(CancellationToken cancellationToken)
	{
		var shipmentNumber = Route<string>("shipmentNumber");

		var shipment = await dbContext.Shipments.FirstOrDefaultAsync(x => x.Number == shipmentNumber, cancellationToken);
		if (shipment is null)
		{
			logger.LogDebug("Shipment with number {ShipmentNumber} not found", shipmentNumber);
			return TypedResults.NotFound($"Shipment with number '{shipmentNumber}' not found");
		}

		var response = shipment.Dispatch();
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Dispatched shipment with {ShipmentNumber}", shipmentNumber);
		return TypedResults.NoContent();
	}
}
