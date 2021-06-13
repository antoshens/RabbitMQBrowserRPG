using BrowserTextRPG.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserTextRPG.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly Dictionary<int, WebSocket> _webSockets;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            this._logger = logger;

            this._webSockets = new Dictionary<int, WebSocket>();
        }

        public async Task<GatewayResponse<bool>> InitiateWebSocketConnection(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var connectionId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var isWebSocketAdded = this._webSockets.TryAdd(connectionId, webSocket);

                if (!isWebSocketAdded) return new GatewayResponse<bool> { Data = true };

                this._logger.LogInformation($"WebSocket connection established for the userID: {connectionId}");

                await Echo(webSocket, connectionId);

                return new GatewayResponse<bool>
                {
                    Data = true
                };
            }
            else
            {
                this._logger.LogWarning("Could not establish websocket connection");

                return new GatewayResponse<bool>
                {
                    Fault = new Fault
                    {
                        ErrorCode = 400,
                        ErrorMessage = "Could not establish websocket connetcion"
                    }
                };
            }
        }

        public async Task PushMessage(string message, int connectionId)
        {
            if (!this._webSockets.ContainsKey(connectionId))
            {
                this._logger.LogWarning("Could not find a WebSocket connection for this user.");
                return;
            }    

            var webSocket = this._webSockets[connectionId];

            var serverMsg = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), WebSocketMessageType.Text, true, CancellationToken.None);

            this._logger.LogInformation("Push-notification sent to Client");
        }

        private async Task Echo(WebSocket webSocket, int connectionId)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            this._logger.LogInformation("Message received from Client");

            if (result.CloseStatus.HasValue)
            {
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                this._webSockets.Remove(connectionId);

                this._logger.LogInformation($"WebSocket connection closed for the userID: {connectionId}");
            }
        }
    }
}
