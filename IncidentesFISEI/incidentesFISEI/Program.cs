using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using incidentesFISEI;
using incidentesFISEI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient para comunicarse con la API
builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = new Uri("http://localhost:7001/")
});

// Registrar servicios personalizados
builder.Services.AddScoped<UserSessionService>();

await builder.Build().RunAsync();
