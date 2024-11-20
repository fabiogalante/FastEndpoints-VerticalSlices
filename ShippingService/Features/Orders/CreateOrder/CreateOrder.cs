using Bogus;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ShippingService.Database;
using ShippingService.Domain.Customers;
using ShippingService.Domain.Orders;
using ShippingService.Domain.Shipments;
using ShippingService.Features.Shipments.CreateShipment;
using Order = ShippingService.Domain.Orders.Order;

namespace ShippingService.Features.Orders.CreateOrder;

public sealed record CreateOrderRequest(
    string CustomerId,
    List<OrderItemRequest> Items,
    Address ShippingAddress,
    string Carrier,
    string ReceiverEmail);

public sealed record OrderItemRequest(
    string ProductName,
    int Quantity);

public sealed record OrderResponse(
    Guid OrderId,
    string OrderNumber,
    DateTime OrderDate,
    List<OrderItemResponse> Items);

public sealed record OrderItemResponse(
    string ProductName,
    int Quantity);

public class CreateOrderEndpoint(
    ShippingDbContext dbContext,
    ILogger<CreateOrderEndpoint> logger)
    : Endpoint<CreateOrderRequest, Results<Ok<OrderResponse>, ValidationProblem, Conflict<string>, NotFound<string>>>
{
    public override void Configure()
    {
        Post("/api/orders");
        AllowAnonymous();
        Validator<CreateOrderRequestValidator>();
    }

    public override async Task<Results<Ok<OrderResponse>, ValidationProblem, Conflict<string>, NotFound<string>>> ExecuteAsync(
        CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Set<Customer>()
	        .FirstOrDefaultAsync(c => c.Id == Guid.Parse(request.CustomerId), cancellationToken);

        if (customer is null)
        {
            logger.LogWarning("Customer with ID '{CustomerId}' does not exist", request.CustomerId);
            return TypedResults.NotFound($"Customer with ID '{request.CustomerId}' does not exist");
        }

        var order = Order.Create(
            orderNumber: GenerateNumber(),
            customer,
            request.Items.Select(x => new OrderItem(x.ProductName, x.Quantity)).ToList()
        );

        var shipment = Shipment.Create(
            number: GenerateNumber(),
            orderId: order.Id,
            address: request.ShippingAddress,
            carrier: request.Carrier,
            receiverEmail: request.ReceiverEmail,
            items: []
        );

        var shipmentItems = CreateShipmentItems(order.Items, shipment.Id);
        shipment.AddItems(shipmentItems);

        await dbContext.Set<Order>().AddAsync(order, cancellationToken);
        await dbContext.Set<Shipment>().AddAsync(shipment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created order: {@Order} with shipment: {@Shipment}", order, shipment);

        var response = order.MapToResponse();
        return TypedResults.Ok(response);
    }

    private static string GenerateNumber() => new Faker().Commerce.Ean8();

    private static List<ShipmentItem> CreateShipmentItems(IReadOnlyList<OrderItem> orderItems, Guid shipmentId)
    {
	    return orderItems.Select(orderItem => new ShipmentItem
	    {
		    Product = orderItem.Product,
		    Quantity = orderItem.Quantity,
		    ShipmentId = shipmentId
	    }).ToList();
    }
}

public class CreateOrderRequestValidator : Validator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .Must(id => Guid.TryParse(id, out _));

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must have at least one item.")
            .ForEach(item =>
            {
                item.SetValidator(new OrderItemRequestValidator());
            });

        RuleFor(x => x.ShippingAddress)
            .NotNull()
            .SetValidator(new AddressValidator());

        RuleFor(x => x.Carrier)
            .NotEmpty();

        RuleFor(x => x.ReceiverEmail)
            .NotEmpty()
            .EmailAddress();
    }
}

public class OrderItemRequestValidator : Validator<OrderItemRequest>
{
    public OrderItemRequestValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}

static file class MappingExtensions
{
    public static OrderResponse MapToResponse(this Order order)
    {
        return new OrderResponse(
            OrderId: order.Id,
            OrderNumber: order.OrderNumber,
            OrderDate: order.Date,
            Items: order.Items.Select(x => new OrderItemResponse(
                ProductName: x.Product,
                Quantity: x.Quantity)).ToList()
        );
    }
}