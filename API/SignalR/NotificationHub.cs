using System.Collections.Concurrent;
using API.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class NotificationHub: Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();

        public override Task OnConnectedAsync()
        {
            var email = Context.User?.GetEmail();

            if(!string.IsNullOrEmpty(email)) UserConnections[email] = Context.ConnectionId;

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var email = Context.User?.GetEmail();

            if(!string.IsNullOrEmpty(email)) UserConnections.TryRemove(email, out _);

            return base.OnConnectedAsync();
        }

        public static string? GetConnectionIdByEmail(string email)
        {
            UserConnections.TryGetValue(email, out var connectionId);

            return connectionId;
        }
    }
}

// This application uses SignalR to send real-time notifications to users when their payment is confirmed. The main focus is:

// Keeping track of active user connections.
// Sending notifications when an order’s payment status updates.
// Key Aspects of SignalR in the Online Store App:
// User Connection Management

// A static dictionary (UserConnections) is used to store user email and connection ID.
// On connection (OnConnectedAsync), the user’s email is mapped to their SignalR connection ID.
// On disconnection (OnDisconnectedAsync), the connection is removed from the dictionary.
// Real-time Notification on Payment Success

// When Stripe confirms a payment, the app updates the order’s status.
// It retrieves the buyer’s connection ID using NotificationHub.GetConnectionIdByEmail(order.BuyerEmail).
// If the connection exists, a notification (OrderCompleteNotification) is sent to the user via SignalR.
// Summary
// Uses direct messaging by tracking users' connection IDs.
// The notification hub is static, meaning it does not depend on SignalR groups but instead tracks users individually.
// Only sends one-way notifications (from server to client) after an event (payment success).