using Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapGet("/api/info", () => Results.Ok(new
{
    Service = "CiCdLab API",
    Environment = app.Environment.EnvironmentName,
    TimestampUtc = DateTime.UtcNow
}));

app.MapGet("/api/notes", async (AppDbContext db) =>
    Results.Ok(await db.Notes.OrderByDescending(n => n.Id).ToListAsync()));

app.MapPost("/api/notes", async (CreateNoteRequest request, AppDbContext db) =>
{
    var note = new Note { Content = request.Content };
    db.Notes.Add(note);
    await db.SaveChangesAsync();
    return Results.Created($"/api/notes/{note.Id}", note);
});

app.Run();

public record CreateNoteRequest(string Content);

public partial class Program;