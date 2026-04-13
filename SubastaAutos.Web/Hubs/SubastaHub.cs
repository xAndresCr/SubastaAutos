using Microsoft.AspNetCore.SignalR;

namespace SubastaAutos.Web.Hubs

{
    public class SubastaHub: Hub
    {
        // El cliente se une al grupo de una subasta específica
        public async Task UnirseASubasta(string idSubasta)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"subasta-{idSubasta}");
        }

        // El cliente sale del grupo
        public async Task SalirDeSubasta(string idSubasta)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"subasta-{idSubasta}");
        }
    }
}

