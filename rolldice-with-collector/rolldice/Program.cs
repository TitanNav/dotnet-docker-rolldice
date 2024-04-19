using System.Globalization;

using Microsoft.AspNetCore.Mvc;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

const string serviceName = "roll-dice3";

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName))
        .AddOtlpExporter(options =>
            {
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                //options.Endpoint = new Uri("https://otlp.nr-data.net/v1/logs");
                options.Endpoint = new Uri("http://otel-collector:4318");

            });
});
builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(serviceName))
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddOtlpExporter(options =>
            {
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                //options.Endpoint = new Uri("https://otlp.nr-data.net/v1/traces");
                options.Endpoint = new Uri("http://otel-collector:4318");
            }))
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddOtlpExporter(options =>
            {
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                //options.Endpoint = new Uri("https://otlp.nr-data.net/v1/metrics");
                options.Endpoint = new Uri("http://otel-collector:4318");
            }));

var app = builder.Build();

string HandleRollDice([FromServices]ILogger<Program> logger, string? player)
{
    var result = RollDice();

    if (string.IsNullOrEmpty(player))
    {
        logger.LogInformation("Anonymous player is rolling the dice: {result}", result);
    }
    else
    {
        logger.LogInformation("{player} is rolling the dice: {result}", player, result);
    }

    return result.ToString(CultureInfo.InvariantCulture);
}

int RollDice()
{
    return Random.Shared.Next(1, 7);
}

app.MapGet("/rolldice/{player?}", HandleRollDice);

app.Run();
