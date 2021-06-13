using BrowserTextRPG.Model;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BrowserTextRPG.Services.NotificationService
{
    public interface INotificationService
    {
        Task<GatewayResponse<bool>> InitiateWebSocketConnection(HttpContext context);
        Task PushMessage(string message, int connectionId);
    }
}
