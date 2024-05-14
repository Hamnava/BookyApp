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

        //private Brush _connection1Status = Brushes.Red;
        //private Brush _connection2Status = Brushes.Red;
        //public Brush Connection1Status
        //{
        //    get => _connection1Status;
        //    set => SetProperty(ref _connection1Status, value, nameof(Connection1Status));
        //}

        //public Brush Connection2Status
        //{
        //    get => _connection2Status;
        //    set => SetProperty(ref _connection2Status, value, nameof(Connection2Status));
        //}

        //// Add more properties as needed
        //// Example: public Brush Connection2Status { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //private void SetProperty<T>(ref T storage, T value, string propertyName)
        //{
        //    if (EqualityComparer<T>.Default.Equals(storage, value)) return;
        //    storage = value;
        //    OnPropertyChanged(propertyName);
        //}

        //public void SetStatus(string key, Brush status)
        //{
        //    switch (key)
        //    {
        //        case "Connection1Status":
        //            Connection1Status = status;
        //            break;
        //        case "Connection2Status":
        //            Connection2Status = status;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(key), "Invalid connection key");
        //    }
        //}
    }


}
