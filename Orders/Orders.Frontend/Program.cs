using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Orders.Frontend;
using Orders.Frontend.AuthenticationProviders;
using Orders.Frontend.Repositories;
using Orders.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Agrego la Url del Backend  para asi conectarnos con el frontend, Aqui se configuran los servicios
// Estamos haciendo una inyecion AddScoped
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7112/") });
builder.Services.AddScoped<IRepository, Repository>();//Para configurar la inyeccion de Repository
builder.Services.AddSweetAlert2();//Inyectamos libreria AddSweetAlert2
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredModal();
builder.Services.AddMudServices();//Inyectamos libreria MudBlazor

builder.Services.AddScoped<AuthenticationProviderJWT>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
builder.Services.AddScoped<ILoginService, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());

await builder.Build().RunAsync();
