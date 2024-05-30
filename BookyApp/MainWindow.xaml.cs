
using BookyApp.Helper;
using BookyApp.Helpers;
using BookyApp.Models;
using BookyApp.Services;
using Microsoft.Extensions.Configuration;
using System.IO.Pipes;
using System.Windows;

namespace BookyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SignalRService _signalRService;
        private readonly NamedPipeService _namedPipeService;
        private readonly ConnectionStatusManager _statusManager;

        public MainWindow()
        {
            InitializeComponent();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var settings = configuration.Get<SignalRConfiguration>().SignalR;

            _statusManager = new ConnectionStatusManager();
            DataContext = _statusManager;

            _signalRService = new SignalRService(settings, _statusManager);
            _namedPipeService = new NamedPipeService();

            InitializeSignalR();
        }

        private async void InitializeSignalR()
        {
            string deviceId = DeviceInfoHelper.GetDeviceId();
            string uniqueClientId = ClientIdHelper.GetOrCreateClientId();
            string clientType = "WPF";

            await _signalRService.InitializeChatConnection(uniqueClientId, deviceId, clientType);
            await _signalRService.InitializePresenceConnection(uniqueClientId, deviceId);
        }

        private async void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            await _namedPipeService.SendMessageAsync("PipesOfPiece", PipeDirection.Out, "Hello from WPF!");
        }

        private async void SignalRMessageButton_Click(object sender, RoutedEventArgs e)
        {
            await _signalRService.SendMessage("WPF User", "Hello from WPF");
        }

        private async void SendAndRecieveDataButton_Click(object sender, RoutedEventArgs e)
        {
            var data = new HelloWithObject { Name = "Nematulah", Family = "Hussaini", Age = 24 };
            await _namedPipeService.SendMessageAsync("SendReceive", PipeDirection.InOut, data);
        }

        private async void SendMessageToSignalR_Click(object sender, RoutedEventArgs e)
        {
            var data = "This message received from WPF to worker service via NamedPipeClientStream and then from worker service sent to chatHub via SignalR!";
            await _namedPipeService.SendMessageAsync("SendMessageToSignalR", PipeDirection.Out, data);
        }
    }




}