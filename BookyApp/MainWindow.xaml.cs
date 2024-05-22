
using BookyApp.Helpers;
using BookyApp.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media;

namespace BookyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private HubConnection _connection1;
        private HubConnection _connection2;
        private ConnectionStatusManager _statusManager = new ConnectionStatusManager();
        private NamedPipeClientService _pipeClientService;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSignalR();
            PresenceSignalR();
            DataContext = _statusManager;
            _pipeClientService = new NamedPipeClientService();
        }




        private void UpdateUI(bool isAlive)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _statusManager.ConnectionStatus = isAlive ? Brushes.Green : Brushes.Red;
            });
        }



        private async void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            await _pipeClientService.SendMessageAsync("PipesOfPiece", PipeDirection.Out, "Hello from WPF!");
        }



        private async void InitializeSignalR()
        {
            _connection1 = new HubConnectionBuilder()
                .WithUrl("https://localhost:7031/chatHub")
                .Build();

            _connection1.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    MessageBox.Show(newMessage);
                });
            });

            try
            {
                await _connection1.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void PresenceSignalR()
        {
            _connection2 = new HubConnectionBuilder()
                .WithUrl("https://localhost:7031/presenceHub")
                .Build();

            _connection2.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection2.StartAsync();
            };

            _connection2.On<bool>("WorkerServiceStatus", (isAlive) =>
            {
                UpdateUI(isAlive);
            });

            await _connection2.StartAsync();
            await _connection2.SendAsync("SendHelloMessage", "WPF service is running");

            // Request the current status of the worker service immediately after connecting
            await _connection2.InvokeAsync("GetWorkerServiceStatus");
        }

        private async void SignalRMessageButton_Click(object sender, RoutedEventArgs e)
        {
            await _connection1.InvokeAsync("SendMessage", "WPF User", "Hello from WPF");
        }



        private async void SendAndRecieveDataButton_Click(object sender, RoutedEventArgs e)
        {
            var data = new HelloWithObject { Name = "Nematulah", Family = "Hussaini", Age = 24 };
            await _pipeClientService.SendMessageAsync("SendReceive", PipeDirection.InOut, data);
        }

        private async void SendMessageToSignalR_Click(object sender, RoutedEventArgs e)
        {
            var data = "This message received from WPF to worker service via NamedPipeClientStream and then from worker service sent to chatHub via SignalR!";

            await _pipeClientService.SendMessageAsync("SendMessageToSignalR", PipeDirection.Out, data);

        }
    }



}