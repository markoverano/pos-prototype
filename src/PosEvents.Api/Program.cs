using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PosEvents.Api.Observability;
using PosEvents.Api.Processing;
using PosEvents.Api.Repository;
using PosEvents.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEventRepository, InMemoryEventRepository>();
builder.Services.AddSingleton<EventMetrics>();
builder.Services.AddTransient<IEventValidator, EventValidator>();
builder.Services.AddTransient<IEventProcessor, EventProcessingService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;

        var feature = context.Features.Get<IExceptionHandlerFeature>();
        if (feature is not null)
        {
            var logger = context.RequestServices
                .GetRequiredService<ILogger<Program>>();
            logger.LogError(feature.Error, "Unhandled exception");
        }

        var problem = new ProblemDetails
        {
            Status = 500,
            Title  = "An unexpected error occurred."
        };

        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }
