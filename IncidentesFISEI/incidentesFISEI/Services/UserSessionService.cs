using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace incidentesFISEI.Services
{
    public class Usuario
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public TipoUsuario TipoUsuario { get; set; }
        
        public string FullName => $"{FirstName} {LastName}".Trim();
    }

    public enum TipoUsuario
    {
        UsuarioFinal = 1,
        Tecnico = 2,
        Supervisor = 3,
        Administrador = 4,
        Docente = 5
    }

    public class UserSessionService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigation;
        
        public bool IsAuthenticated { get; private set; } = false;
        public Usuario? CurrentUser { get; private set; }
        
        public event Action? OnAuthenticationStateChanged;
        
        public UserSessionService(IJSRuntime jsRuntime, NavigationManager navigation)
        {
            _jsRuntime = jsRuntime;
            _navigation = navigation;
        }

        public bool IsAdmin => CurrentUser?.TipoUsuario == TipoUsuario.Administrador;
        public bool IsTechnician => CurrentUser?.TipoUsuario == TipoUsuario.Tecnico;
        public bool IsSupervisor => CurrentUser?.TipoUsuario == TipoUsuario.Supervisor;
        public bool IsTeacher => CurrentUser?.TipoUsuario == TipoUsuario.Docente;
        public bool IsEndUser => CurrentUser?.TipoUsuario == TipoUsuario.UsuarioFinal;
        public bool IsLoggedIn => IsAuthenticated && CurrentUser != null;
        
        // Propiedades para obtener información del usuario actual
        public string FullName => CurrentUser?.FullName ?? "";
        public int UserId => CurrentUser?.Id ?? 0;

        public async Task InitializeAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                var userJson = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "currentUser");
                
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
                {
                    // TODO: Verificar que el token siga siendo válido
                    var user = System.Text.Json.JsonSerializer.Deserialize<Usuario>(userJson);
                    if (user != null)
                    {
                        CurrentUser = user;
                        IsAuthenticated = true;
                        OnAuthenticationStateChanged?.Invoke();
                    }
                }
            }
            catch (Exception)
            {
                // Si hay error, limpiar sesión
                await ClearSessionAsync();
            }
        }

        public async Task SetUserSessionAsync(Usuario user, string token)
        {
            CurrentUser = user;
            IsAuthenticated = true;
            
            // Guardar en localStorage
            var userJson = System.Text.Json.JsonSerializer.Serialize(user);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", userJson);
            
            OnAuthenticationStateChanged?.Invoke();
        }

        public async Task ClearSessionAsync()
        {
            CurrentUser = null;
            IsAuthenticated = false;
            
            // Limpiar localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser");
            
            OnAuthenticationStateChanged?.Invoke();
        }
        
        public async Task LogoutAsync()
        {
            await ClearSessionAsync();
            _navigation.NavigateTo("/login", forceLoad: true);
        }

        public string GetUserRole()
        {
            if (!IsAuthenticated || CurrentUser == null)
                return "Invitado";

            return CurrentUser.TipoUsuario switch
            {
                TipoUsuario.Administrador => "Administrador del Sistema",
                TipoUsuario.Tecnico => "Técnico Especializado",
                TipoUsuario.UsuarioFinal => "Usuario Final",
                _ => "Usuario"
            };
        }
    }
}