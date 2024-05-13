
using BookyApp.Helpers;
using BookyApp.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Windows;
using System.Windows.Media;

namespace BookyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HubConnection _connection;

        private ConnectionStatusManager _statusManager = new ConnectionStatusManager();
        private NamedPipeClientService _pipeClientService;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSignalR();
            DataContext = _statusManager;
            _pipeClientService = new NamedPipeClientService(_statusManager);
        }

     

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            await _pipeClientService.SendMessageAsync("PipesOfPiece", PipeDirection.Out, "Hello from WPF!", "Connection1Status");
        }

     

        private async void InitializeSignalR()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7031/chatHub")
                .Build();

            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    MessageBox.Show(newMessage);
                });
            });

            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void SignalRMessageButton_Click(object sender, RoutedEventArgs e)
        {
            await _connection.InvokeAsync("SendMessage", "WPF User", "Hello from WPF");
        }



        private async void SendAndRecieveDataButton_Click(object sender, RoutedEventArgs e)
        {
            var data = new HelloWithObject { Name = "Nematulah", Family = "Hussaini", Age = 24 };
            await _pipeClientService.SendMessageAsync("SendReceive", PipeDirection.InOut, data, "Connection2Status");
        }

        private async void SendMessageToSignalR_Click(object sender, RoutedEventArgs e)
        {
            var data = "This message received from WPF to worker service via NamedPipeClientStream and then from worker service sent to chatHub via SignalR!";

            await _pipeClientService.SendMessageAsync("SendMessageToSignalR", PipeDirection.Out, data);

        }
    }


  
}