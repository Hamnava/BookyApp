namespace SignalRServer.Models
{
    public class ClientInfo
    {
        public string ConnectionId { get; set; }
        public string UniqueClientId { get; set; } // Unique ID that persists across connections
        public string DeviceId { get; set; }
        public string ClientType { get; set; }
        public bool IsOnline { get; set; }
    }
}
