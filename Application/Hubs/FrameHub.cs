using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs;

public class FrameHub : Hub
{
    public async Task SendFrame(string frameBase64)
    {
        await Clients.All.SendAsync("ReceiveFrame", frameBase64);
    }
}