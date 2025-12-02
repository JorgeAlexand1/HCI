using Microsoft.JSInterop;

namespace FISEI.Incidentes.Infrastructure.Services;

public class TokenProvider
{
    private readonly IJSRuntime _jsRuntime;
    private string? _cachedToken;

    public TokenProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> GetTokenAsync()
    {
        if (_cachedToken != null) return _cachedToken;
        
        try
        {
            _cachedToken = await _jsRuntime.InvokeAsync<string?>("authStore.getToken");
            return _cachedToken;
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokenAsync(string token)
    {
        _cachedToken = token;
        await _jsRuntime.InvokeVoidAsync("authStore.setToken", token);
    }

    public async Task RemoveTokenAsync()
    {
        _cachedToken = null;
        await _jsRuntime.InvokeVoidAsync("authStore.removeToken");
    }
}
