using Bogus;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ShippingService.Database;
using ShippingService.Domain.Shipments;
using ShippingService.Features.Shipments.Shared.Responses;

namespace ShippingService.Features.Shipments.CreateShipment;

public sealed record CreateShipmentRequest(
    Guid OrderId,
    Address Address,
    string Carrier,
    string ReceiverEmail,
    List<ShipmentItem> Items);

public class CreateShipmentEndpoint(ShippingDbContext dbContext,
    ILogger<CreateShipmentEndpoint> logger)
    : Endpoint<CreateShipmentRequest, Results<Ok<ShipmentResponse>, Conflict<string>>>
{
    public override void Configure()
    {
        Post("/api/shipments");
        AllowAnonymous();
        Validator<CreateShipmentRequestValidator>();
    }

    public override async Task<Results<Ok<ShipmentResponse>, Conflict<string>>> ExecuteAsync(
        CreateShipmentRequest request, CancellationToken cancellationToken)
    {
        var shipmentAlreadyExists = await dbContext
	        .Shipments.AnyAsync(x => x.OrderId == request.OrderId, cancellationToken);

        if (shipmentAlreadyExists)
        {
            logger.LogInformation("Shipment for order '{OrderId}' is already created", request.OrderId);
            return TypedResults.Conflict($"Shipment for order '{request.OrderId}' is already created");
        }

        var shipmentNumber = new Faker().Commerce.Ean8();
        var shipment = request.MapToShipment(shipmentNumber);

        await dbContext.AddAsync(shipment, cancellationToken);

        logger.LogInformation("Created shipment: {@Shipment}", shipment);

        var response = shipment.MapToResponse();
        return TypedResults.Ok(response);
    }
}

public class CreateShipmentRequestValidator : Validator<CreateShipmentRequest>
{
    public CreateShipmentRequestValidator()
    {
        RuleFor(shipment => shipment.OrderId).NotEmpty();
        RuleFor(shipment => shipment.Carrier).NotEmpty();
        RuleFor(shipment => shipment.ReceiverEmail).NotEmpty();
        RuleFor(shipment => shipment.Items).NotEmpty();

        RuleFor(shipment => shipment.Address)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Address must not be null")
            .SetValidator(new AddressValidator());
    }
}

public class AddressValidator : Validator<Address>
{
    public AddressValidator()
    {
        RuleFor(address => address.Street).NotEmpty();
        RuleFor(address => address.City).NotEmpty();
        RuleFor(address => address.Zip).NotEmpty();
    }
}

internal static class MappingExtensions
{
	public static Shipment MapToShipment(this CreateShipmentRequest request, string shipmentNumber)
		=> Shipment.Create(
			shipmentNumber,
			request.OrderId,
			request.Address,
			request.Carrier,
			request.ReceiverEmail,
			request.Items.Select(x => new ShipmentItem
			{
				Product = x.Product,
				Quantity = x.Quantity
			}).ToList()
		);

    public static ShipmentResponse MapToResponse(this Shipment shipment)
        => new(
            shipment.Number,
            shipment.OrderId,
            shipment.Address,
            shipment.Carrier,
            shipment.ReceiverEmail,
            shipment.Status,
            shipment.Items
                .Select(x => new ShipmentItemResponse(x.Product, x.Quantity))
                .ToList()
        );
}

