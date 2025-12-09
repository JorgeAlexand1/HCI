using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace incidentesFISEI.Services
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;

        public AuthMessageHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener el token del localStorage
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

                // Si existe el token, agregarlo al header
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener token en AuthMessageHandler: {ex.Message}");
            }

            // Continuar con la cadena de handlers
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
