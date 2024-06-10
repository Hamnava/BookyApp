using BookyApp.Helpers;
using BookyApp.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Windows;
using System.Windows.Media;

namespace BookyApp.Services
{
    public class SignalRService
    {
        private readonly HubConnection _chatConnection;
        private readonly HubConnection _presenceConnection;
        private readonly ConnectionStatusManager _statusManager;

        public SignalRService(SignalRSettings settings, ConnectionStatusManager statusManager)
        {
            _statusManager = statusManager;

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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";

                    // Show the custom message box in the middle of the main window
                    var mainWindow = Application.Current.MainWindow;
                    CustomMessageShow.Show(newMessage, mainWindow);
                });
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

            _presenceConnection.On<bool>("WorkerServiceStatus", (isAlive) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _statusManager.ConnectionStatus = isAlive ? Brushes.Green : Brushes.Red;
                });
            });

            await _presenceConnection.StartAsync();
            await _presenceConnection.SendAsync("SendHelloMessage", "WPF service is running");
            await _presenceConnection.InvokeAsync("RegisterClient", uniqueClientId, deviceId, false);
            await _presenceConnection.InvokeAsync("GetWorkerServiceStatus", deviceId);
        }

        public async Task SendMessage(string user, string message)
        {
            await _chatConnection.InvokeAsync("SendMessage", user, message);
        }
    }

}
