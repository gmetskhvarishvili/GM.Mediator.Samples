using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GM.Mediator.Sample.Tests.Api;

/// <summary>
/// End-to-end tests that boot the real API in-memory and exercise the endpoints, verifying the
/// full request pipeline: model binding -> mediator -> validation behaviour -> handler.
/// </summary>
public class SamplesEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_with_a_valid_command_returns_200_and_the_handler_result()
    {
        var response = await _client.PostAsJsonAsync("/samples", new { Test = "hello" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("passed successfully", body);
    }

    [Fact]
    public async Task Post_with_an_invalid_command_returns_400_problem_details()
    {
        var response = await _client.PostAsJsonAsync("/samples", new { Test = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("validation", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Put_with_a_valid_command_returns_200()
    {
        var response = await _client.PutAsJsonAsync("/samples", new { Test = "hello" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
