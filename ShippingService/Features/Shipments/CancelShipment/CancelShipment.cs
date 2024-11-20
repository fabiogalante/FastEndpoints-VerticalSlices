using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ShippingService.Database;
using ShippingService.Features.Shipments.Shared;

namespace ShippingService.Features.Shipments.CancelShipment;

public class CancelShipmentEndpoint(ShippingDbContext dbContext,
	ILogger<CancelShipmentEndpoint> logger)
	: EndpointWithoutRequest<Results<NoContent, NotFound<string>, ProblemHttpResult>>
{
	public override void Configure()
	{
		Post("/api/shipments/cancel/{shipmentNumber}");
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

		var response = shipment.Cancel();
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Shipment with {ShipmentNumber} was cancelled", shipmentNumber);
		return TypedResults.NoContent();
	}
}

