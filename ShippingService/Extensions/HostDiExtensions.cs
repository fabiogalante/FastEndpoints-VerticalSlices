using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ShippingService.Database;
using ShippingService.Services;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class HostDiExtensions
{
	public static IServiceCollection AddWebHostInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<SeedService>();

		services.AddEfCore(configuration);

		services
			.AddEndpointsApiExplorer()
			.AddSwaggerGen();

		services.Configure<JsonOptions>(opt =>
		{
			opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});

		return services;
	}

	public static void AddHostLogging(this WebApplicationBuilder builder)
	{
		builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));
	}

	private static IServiceCollection AddEfCore(this IServiceCollection services, IConfiguration configuration)
	{
		var postgresConnectionString = configuration.GetConnectionString("Postgres");

		services.AddDbContext<ShippingDbContext>(x => x
			.EnableSensitiveDataLogging()
			.UseNpgsql(postgresConnectionString, npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__MyMigrationsHistory", "shipping-fast-endpoints"))
			.UseSnakeCaseNamingConvention()
		);

		return services;
	}
}
