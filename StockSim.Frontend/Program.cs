using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StockSim.Frontend;
using StockSim.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
// The splash markup inside #app is cleared automatically when the app first renders.
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
// Registered as the concrete type too so pages can raise sign-in/sign-out notifications directly.
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ISearchService, SearchService>();

await builder.Build().RunAsync();
