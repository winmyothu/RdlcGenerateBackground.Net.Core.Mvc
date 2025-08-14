using Microsoft.AspNetCore.SignalR;

namespace RdlcSignalRDemo.Hubs;

public class ReportHub : Hub
{
    public async Task JoinJobGroup(string jobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
    }
}
