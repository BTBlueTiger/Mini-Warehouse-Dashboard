using Microsoft.AspNetCore.SignalR;

namespace MiniWarehouse.Api.Hubs;

// Der Hub erbt von der Basisklasse 'Hub'
public class WarehouseHub : Hub
{
    // Diese Methode kann vom Server oder von Clients aufgerufen werden
    // Wir definieren hier, welche Parameter wir an die UI schicken
    public async Task SendUpdate(string message, string type, string timestamp)
    {
        // 'ReceiveUpdate' ist der "Event-Name", auf den der Blazor-Client hört
        await Clients.All.SendAsync("ReceiveUpdate", message, type, timestamp);
    }
}