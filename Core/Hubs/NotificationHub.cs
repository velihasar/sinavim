using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = GetUserIdFromToken();
            if (userId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId.Value}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserIdFromToken();
            if (userId.HasValue)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId.Value}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        private int? GetUserIdFromToken()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                if (httpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        return userId;
                    }
                }
            }
            catch
            {
                // Token parsing failed, return null
            }

            return null;
        }
    }
} 