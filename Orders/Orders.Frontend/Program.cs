using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Orders.Frontend;
using Orders.Frontend.Repositories;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Agrego la Url del Frontend  para asi conectarnos con el Backend, Aqui se configuran los servicios
// Estamos haciendo una inyecion AddScoped
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7096//") });
//Objeto buider para inyertarlo con inyecion AddScoped
builder.Services.AddScoped<IRepository, Repository>();

await builder.Build().RunAsync();
