using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Utya.Client;
using Utya.Client.Services;
using Utya.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IShortLinkService, ShortLinkService>();
builder.Services.AddTransient<CookieHandler>();
builder.Services.AddHttpClient("Utya.ServerAPI",
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CookieHandler>();

await builder.Build().RunAsync();