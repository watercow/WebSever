using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace WebServer.HttpServer
{
    public class interfaceDescriptor
    {
        public string interfaceIP;
        public string interfaceMAC;
        public string interfaceName;
    }

    public class IPConfig
    {
        public static List<interfaceDescriptor> networkCardIPs;
       
        public static IList<interfaceDescriptor> GetPhysicsNetworkCardIP()
        {
            networkCardIPs = new List<interfaceDescriptor>();

            NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in fNetworkInterfaces)
            {
                string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                if (rk != null)
                {
                    // 区分 PnpInstanceID  
                    // 如果前面有 PCI 就是本机的真实网卡 
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    {
                        string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                        int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                        if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI" || true)
                        {
                            IPInterfaceProperties fIPInterfaceProperties = adapter.GetIPProperties();
                            UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = fIPInterfaceProperties.UnicastAddresses;
                            foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                            {
                                if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    networkCardIPs.Add(
                                        new interfaceDescriptor
                                        {
                                            interfaceIP = UnicastIPAddressInformation.Address.ToString(),
                                            interfaceMAC = adapter.GetPhysicalAddress().ToString(),
                                            interfaceName = adapter.Name
                                        }); //Ip 地址
                                }
                            }
                        }
                    }
                }
            }
            return networkCardIPs;
        }
    }
}
