using System.Text.Json.Serialization;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ShippingService.Database;
using ShippingService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddHostLogging();
builder.Services.AddWebHostInfrastructure(builder.Configuration);
builder.Services.AddFastEndpoints();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<ShippingDbContext>();
	await dbContext.Database.MigrateAsync();

	var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
	await seedService.SeedDataAsync();
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseFastEndpoints(c =>
{
	c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});

app.Run();
