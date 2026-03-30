using Microsoft.AspNetCore.SignalR;
using MiniWarehouse.Api.Hubs;

public class WarehouseSimulator(IHubContext<WarehouseHub> hubContext) : BackgroundService
{
    private readonly string[] _actions = ["Kassette eingelagert", "RBG fährt zu Position", "Lagerplatz optimiert", "Material entnommen"];
    private readonly Random _random = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Alle 10 bis 20 Sekunden ein Event simulieren
            await Task.Delay(_random.Next(10000, 20000), stoppingToken);

            var action = _actions[_random.Next(_actions.Length)];
            var id = _random.Next(100, 999);

            // Den Push an alle Clients senden
            await hubContext.Clients.All.SendAsync("ReceiveUpdate", 
                $"{action} (ID: {id})", "Simulated", DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}