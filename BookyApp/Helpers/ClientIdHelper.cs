using System.IO;

namespace BookyApp.Helper
{
    public static class ClientIdHelper
    {
        private static readonly string ConfigFilePath = "clientId.config";

        public static string GetOrCreateClientId()
        {
            if (File.Exists(ConfigFilePath))
            {
                return File.ReadAllText(ConfigFilePath);
            }
            else
            {
                string clientId = Guid.NewGuid().ToString();
                File.WriteAllText(ConfigFilePath, clientId);
                return clientId;
            }
        }
    }

}
