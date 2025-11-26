using System;
using System.Collections.Concurrent;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class NotificationHub : Hub 
{
    private static readonly ConcurrentDictionary<string, string> _userConnections = new();

    public override Task OnConnectedAsync()
    {
        var email = Context.User?.GetUserEmail();
        if (!string.IsNullOrEmpty(email))        
        {
            _userConnections[email] = Context.ConnectionId;
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var email = Context.User?.GetUserEmail();
        if (!string.IsNullOrEmpty(email))
        {
            _userConnections.TryRemove(email, out _);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public static string? GetConnectionIdByEmail(string email)
    {
        _userConnections.TryGetValue(email, out var connectionId);
        return connectionId;
    }
}
