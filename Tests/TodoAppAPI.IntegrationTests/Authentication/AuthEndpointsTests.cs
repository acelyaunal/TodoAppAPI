using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoAppAPI.Application.Auth.Models;
using TodoAppAPI.WebAPI.Contracts.Requests;
using TodoAppAPI.IntegrationTests.Infrastructure;
using Xunit;

namespace TodoAppAPI.IntegrationTests.Authentication;

public class AuthEndpointsTests : IClassFixture<TodoAppApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(TodoAppApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_And_Login_Should_ReturnJwtToken()
    {
        var email = $"user-{Guid.NewGuid():N}@example.com";

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = email,
            Password = "Password123"
        });

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = email,
            Password = "Password123"
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResponse);
        Assert.False(string.IsNullOrWhiteSpace(authResponse!.Token));
        Assert.True(authResponse.ExpiresAtUtc > DateTime.UtcNow);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(authResponse.Token);
        Assert.Contains(token.Claims, claim => claim.Type == JwtRegisteredClaimNames.Sub);
        Assert.Contains(token.Claims, claim => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == email.ToLowerInvariant());
    }

    [Fact]
    public async Task Protected_Todo_Endpoint_Without_Token_Should_ReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/todos");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
