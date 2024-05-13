
using BookyApp.Helpers;
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


        public MainWindow()
        {
            InitializeComponent();
            DataContext = _statusManager;
        }

     

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new NamedPipeClientStream(".", "PipesOfPiece", PipeDirection.Out))
            {
                try
                {
                    await client.ConnectAsync(5000); // Timeout for connection attempt
                    _statusManager.Connection1Status = Brushes.Green;
                    using (var writer = new StreamWriter(client))
                    {
                        await writer.WriteAsync("Hello from WPF!");
                        await writer.FlushAsync();
                    }
                }
                catch (Exception)
                {
                    _statusManager.Connection1Status = Brushes.Red;
                }
            }
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
            try
            {
                using (var client = new NamedPipeClientStream(".", "SendReceive", PipeDirection.InOut, PipeOptions.Asynchronous))
                {
                    await client.ConnectAsync();
                    _statusManager.Connection2Status = Brushes.Green;
                    using (var writer = new StreamWriter(client, leaveOpen: true)) // Important to leave the underlying stream open
                    {
                        var data = new HelloWithObject
                        {
                            Name = "Nematulah",
                            Family = "Hussaini",
                            Age = 24
                        };

                        // Serialize the object to a JSON string
                        string jsonData = JsonConvert.SerializeObject(data);

                        await writer.WriteLineAsync(jsonData);
                        await writer.FlushAsync();
                    }

                    using (var reader = new StreamReader(client)) // Continue using the same stream for reading
                    {
                        string response = await reader.ReadLineAsync();
                        MessageBox.Show(response);
                    }
                }
            }
            catch (Exception ex)
            {
                _statusManager.Connection2Status = Brushes.Red;
                MessageBox.Show($"Error: {ex.Message}");
            }
           
        }

        private async void SendMessageToSignalR_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new NamedPipeClientStream(".", "SendMessageToSignalR", PipeDirection.Out))
            {
                await client.ConnectAsync();
                using (var writer = new StreamWriter(client))
                {
                    await writer.WriteAsync("This message received from WPF to worker service via NamedPipeClientStream and then from worker service sent to chatHub via SignalR!");
                    await writer.FlushAsync();
                }
            }
        }
    }


    public class HelloWithObject
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public int Age { get; set; }
    }
}