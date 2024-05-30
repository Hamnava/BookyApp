namespace SignalRServer.Models
{
    public class PresenseInfo
    {
        public bool IsWorkerService { get; set; }
        public string DeviceId { get; set; }
        public string ConnectionId { get; set; }
        public string UniqueClientId { get; set; }
        public bool IsOnline { get; set; }
    }
}
