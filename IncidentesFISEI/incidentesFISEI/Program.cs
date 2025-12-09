using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Net.Http;
using incidentesFISEI;
using incidentesFISEI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registrar servicios personalizados PRIMERO para que estén disponibles
builder.Services.AddScoped<UserSessionService>();

// Configurar HttpClient con AuthMessageHandler para agregar token automáticamente
builder.Services.AddScoped(sp =>
{
    var jsRuntime = sp.GetRequiredService<IJSRuntime>();
    var handler = new AuthMessageHandler(jsRuntime)
    {
        InnerHandler = new HttpClientHandler()
    };

    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:7001/")
    };
    return client;
});

await builder.Build().RunAsync();
