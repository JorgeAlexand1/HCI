using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace FISEI.Incidentes.Presentation.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var usuarioId = Context.User?.FindFirst("id")?.Value;
        
        if (!string.IsNullOrEmpty(usuarioId))
        {
            // Agregar el usuario a un grupo con su ID para notificaciones personalizadas
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{usuarioId}");
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var usuarioId = Context.User?.FindFirst("id")?.Value;
        
        if (!string.IsNullOrEmpty(usuarioId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{usuarioId}");
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
