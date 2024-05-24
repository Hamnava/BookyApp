using System.ComponentModel;
using System.Windows.Media;

namespace BookyApp.Helpers
{
    public class ConnectionStatusManager : INotifyPropertyChanged
    {
        private Brush _connectionStatus = Brushes.Red;
        public Brush ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (_connectionStatus != value)
                {
                    _connectionStatus = value;
                    OnPropertyChanged(nameof(ConnectionStatus));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
