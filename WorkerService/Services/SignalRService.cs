using Microsoft.AspNetCore.SignalR.Client;
using WorkerService.Models;

namespace WorkerService.Services
{
    public class SignalRService
    {
        private readonly HubConnection _chatConnection;
        private readonly HubConnection _presenceConnection;

        public SignalRService(SignalRSettings settings)
        {
            _chatConnection = new HubConnectionBuilder()
                .WithUrl(settings.ChatHubUrl)
                .Build();

            _presenceConnection = new HubConnectionBuilder()
                .WithUrl(settings.PresenceHubUrl)
                .Build();
        }

        public async Task InitializeChatConnection(string uniqueClientId, string deviceId, string clientType)
        {
            _chatConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"{user}: {message}");
            });

            await _chatConnection.StartAsync();
            await _chatConnection.InvokeAsync("RegisterClient", uniqueClientId, deviceId, clientType);
        }

        public async Task InitializePresenceConnection(string uniqueClientId, string deviceId)
        {
            _presenceConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _presenceConnection.StartAsync();
            };

            await _presenceConnection.StartAsync();
            await _presenceConnection.SendAsync("SendHelloMessage", "Worker service is running");

            // Send registration information
            await _presenceConnection.InvokeAsync("RegisterClient", uniqueClientId, deviceId, true);
        }


        public async Task SendMessage(string user, string message)
        {
            await _chatConnection.InvokeAsync("SendMessage", user, message);
        }

    }

}
