using System.Net.NetworkInformation;

public static class DeviceInfoHelper
{
    public static string GetDeviceId()
    {
        var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback && ni.OperationalStatus == OperationalStatus.Up);

        return networkInterface?.GetPhysicalAddress().ToString();
    }
}
