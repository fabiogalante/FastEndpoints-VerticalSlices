using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ShippingService.Database;
using ShippingService.Domain.Customers;

namespace ShippingService.Features.Customers.CreateCustomer;

public sealed record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber);

public sealed record CustomerResponse(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber);

public class CreateCustomerEndpoint(
    ShippingDbContext dbContext,
    ILogger<CreateCustomerEndpoint> logger)
    : Endpoint<CreateCustomerRequest, Results<Ok<CustomerResponse>, ValidationProblem, Conflict<string>>>
{
    public override void Configure()
    {
        Post("/api/customers");
        AllowAnonymous();
        Validator<CreateCustomerRequestValidator>();
    }

    public override async Task<Results<Ok<CustomerResponse>, ValidationProblem, Conflict<string>>> ExecuteAsync(
        CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customerExists = await dbContext.Set<Customer>()
	        .AnyAsync(c => c.Email == request.Email, cancellationToken);

        if (customerExists)
        {
            logger.LogInformation("Customer with email '{Email}' already exists", request.Email);
            return TypedResults.Conflict($"Customer with email '{request.Email}' already exists");
        }

        var customer = request.MapToCustomer();

        await dbContext.Set<Customer>().AddAsync(customer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created customer: {@Customer}", customer);

        var response = customer.MapToResponse();
        return TypedResults.Ok(response);
    }
}

public class CreateCustomerRequestValidator : Validator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty();
    }
}

static file class MappingExtensions
{
    public static CustomerResponse MapToResponse(this Customer customer)
    {
	    return new CustomerResponse(
		    CustomerId: customer.Id,
		    FirstName: customer.FirstName,
		    LastName: customer.LastName,
		    Email: customer.Email,
		    PhoneNumber: customer.PhoneNumber
	    );
    }

    public static Customer MapToCustomer(this CreateCustomerRequest request)
    {
	    return Customer.Create(
		    request.FirstName,
		    request.LastName,
		    request.Email,
		    request.PhoneNumber);
    }
}


