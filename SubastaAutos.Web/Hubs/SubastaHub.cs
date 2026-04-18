using Microsoft.AspNetCore.SignalR;

namespace SubastaAutos.Web.Hubs

{
    public class SubastaHub: Hub
    {
        // El cliente se une al grupo de una subasta específica
        public async Task UnirseASubasta(string idSubasta)
        {
            Console.WriteLine($"Cliente {Context.ConnectionId} uniéndose a subasta-{idSubasta}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"subasta-{idSubasta}");
            Console.WriteLine($"Cliente unido al grupo subasta-{idSubasta}"); await Groups.AddToGroupAsync(Context.ConnectionId, $"subasta-{idSubasta}");
        }

        // El cliente sale del grupo
        public async Task SalirDeSubasta(string idSubasta)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"subasta-{idSubasta}");
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
                Console.WriteLine($"SignalR desconectado: {exception.Message}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}

