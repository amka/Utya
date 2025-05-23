using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Utya.Shared.Models;
using Utya.Shared.Services;

namespace Utya.Client.Services;

public class UserService(IHttpClientFactory clientFactory, AuthenticationStateProvider authenticationStateAsync)
    : IUserService
{
    public async Task<Profile?> GetProfileAsync(string userId)
    {
        try
        {
            var client = clientFactory.CreateClient("Utya.ServerAPI");
            return await client.GetFromJsonAsync<Profile>("api/v1/Account/profile");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }


    public async Task<string?> GetCurrentUserId()
    {
        var authState = await authenticationStateAsync.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
    }
}