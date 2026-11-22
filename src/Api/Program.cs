var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapGet("/api/info", () => Results.Ok(new
{
    Service = "CiCdLab API",
    Environment = app.Environment.EnvironmentName,
    TimestampUtc = DateTime.UtcNow
}));

app.Run();

public partial class Program;