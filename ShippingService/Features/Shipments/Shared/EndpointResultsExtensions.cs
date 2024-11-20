using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ShippingService.Features.Shipments.Shared;

public static class EndpointResultsExtensions
{
	public static ProblemHttpResult ToProblem(this Error error)
	{
		return CreateProblem(error);
	}

	public static ProblemHttpResult ToProblem(this List<Error> errors)
	{
		return errors.Count is 0 ? TypedResults.Problem() : CreateProblem(errors);
	}

	private static ProblemHttpResult CreateProblem(Error error)
	{
		var statusCode = error.Type switch
		{
			ErrorType.Conflict => StatusCodes.Status409Conflict,
			ErrorType.Validation => StatusCodes.Status400BadRequest,
			ErrorType.NotFound => StatusCodes.Status404NotFound,
			ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
			_ => StatusCodes.Status500InternalServerError
		};

		var dictionary = new Dictionary<string, object?>
		{
			[error.Code] = error.Description
		};

		return TypedResults.Problem(statusCode: statusCode, extensions: dictionary);
	}

	private static ProblemHttpResult CreateProblem(List<Error> errors)
	{
		var statusCode = errors.First().Type switch
		{
			ErrorType.Conflict => StatusCodes.Status409Conflict,
			ErrorType.Validation => StatusCodes.Status400BadRequest,
			ErrorType.NotFound => StatusCodes.Status404NotFound,
			ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
			_ => StatusCodes.Status500InternalServerError
		};

		return TypedResults.Problem(statusCode: statusCode, extensions: errors.ToDictionary(k => k.Code, object? (v) => v.Description));
	}
}

