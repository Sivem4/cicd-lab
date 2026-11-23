using System.Net;
using System.Net.Http.Json;
using Api.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests;

[Trait("Category", "Database")]
public class NotesIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task PostAndGetNotes_PersistsNoteInSqlServer()
    {
        var client = factory.CreateClient();
        var payload = new { Content = "CI/CD with MS SQL works!" };

        var postResponse = await client.PostAsJsonAsync("/api/notes", payload);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var notes = await client.GetFromJsonAsync<List<Note>>("/api/notes");
        Assert.NotNull(notes);
        Assert.Contains(notes, n => n.Content == "CI/CD with MS SQL works!");
    }
}