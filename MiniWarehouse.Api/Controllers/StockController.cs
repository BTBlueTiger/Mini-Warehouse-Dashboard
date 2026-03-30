using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MiniWarehouse.Api.Hubs;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IHubContext<WarehouseHub> _hubContext;

    public StockController(IHubContext<WarehouseHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost("move/{itemId}")]
    public async Task<IActionResult> MoveItem(int itemId)
    {
        // 1. Hier käme deine DB-Logik (z.B. Status der Kassette auf 'Moving' setzen)
        // var item = await _repository.GetById(itemId);
        
        // 2. SignalR Event feuern
        // Wir senden: Nachricht, Typ, Zeitstempel
        await _hubContext.Clients.All.SendAsync("ReceiveUpdate", 
            $"Kassette {itemId} wird vom RBG 1 angefordert.", 
            "Movement", 
            DateTime.Now.ToString("HH:mm:ss"));

        return Ok(new { Message = $"Bewegung für Item {itemId} gestartet" });
    }
}