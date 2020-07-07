using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class DiscoveryInterface
    {
        //
        // Résumé :
        //     Gets the current operational state of the network connection.
        //
        // Retourne :
        //     One of the System.Net.NetworkInformation.OperationalStatus values.
        public OperationalStatus OperationalStatus => Interface.OperationalStatus;
        //
        // Résumé :
        //     Gets the interface type.
        //
        // Retourne :
        //     An System.Net.NetworkInformation.NetworkInterfaceType value that specifies the
        //     network interface type.
        public NetworkInterfaceType NetworkInterfaceType => Interface.NetworkInterfaceType;
        //
        // Résumé :
        //     Gets the name of the network adapter.
        //
        // Retourne :
        //     A System.String that contains the adapter name.
        public string Name => Interface.Name;
        //
        // Résumé :
        //     Gets a System.Boolean value that indicates whether the network interface is set
        //     to only receive data packets.
        //
        // Retourne :
        //     true if the interface only receives network traffic; otherwise, false.
        //
        // Exceptions :
        //   T:System.PlatformNotSupportedException:
        //     This property is not valid on computers running operating systems earlier than
        //     Windows XP.
        public bool IsReceiveOnly => Interface.IsReceiveOnly;
        //
        // Résumé :
        //     Gets the identifier of the network adapter.
        //
        // Retourne :
        //     A System.String that contains the identifier.
        public string Id => Interface.Id;
        //
        // Résumé :
        //     Gets the description of the interface.
        //
        // Retourne :
        //     A System.String that describes this interface.
        public string Description => Interface.Description;
        //
        // Résumé :
        //     Gets the speed of the network interface.
        //
        // Retourne :
        //     A System.Int64 value that specifies the speed in bits per second.
        public long Speed => Interface.Speed;
        //
        // Résumé :
        //     Gets a System.Boolean value that indicates whether the network interface is enabled
        //     to receive multicast packets.
        //
        // Retourne :
        //     true if the interface receives multicast packets; otherwise, false.
        //
        // Exceptions :
        //   T:System.PlatformNotSupportedException:
        //     This property is not valid on computers running operating systems earlier than
        //     Windows XP.
        public bool SupportsMulticast => Interface.SupportsMulticast;




        //
        // Résumé :
        //     Gets the anycast IP addresses assigned to this interface.
        //
        // Retourne :
        //     An System.Net.NetworkInformation.IPAddressInformationCollection that contains
        //     the anycast addresses for this interface.
        public string AnycastAddresses => string.Join(", ", Properties.AnycastAddresses.Select(x => x.Address));
        //
        // Résumé :
        //     Gets the addresses of Dynamic Host Configuration Protocol (DHCP) servers for
        //     this interface.
        //
        // Retourne :
        //     An System.Net.NetworkInformation.IPAddressCollection that contains the address
        //     information for DHCP servers, or an empty array if no servers are found.
        public string DhcpServerAddresses => string.Join(", ", Properties.DhcpServerAddresses.Select(x => x));
        //
        // Résumé :
        //     Gets the addresses of Domain Name System (DNS) servers for this interface.
        //
        // Retourne :
        //     A System.Net.NetworkInformation.IPAddressCollection that contains the DNS server
        //     addresses.
        public string DnsAddresses => string.Join(", ", Properties.DnsAddresses.Select(x => x));
        //
        // Résumé :
        //     Gets the Domain Name System (DNS) suffix associated with this interface.
        //
        // Retourne :
        //     A System.String that contains the DNS suffix for this interface, or System.String.Empty
        //     if there is no DNS suffix for the interface.
        //
        // Exceptions :
        //   T:System.PlatformNotSupportedException:
        //     This property is not valid on computers running operating systems earlier than
        //     Windows 2000.
        public string DnsSuffix => Properties.DnsSuffix;
        //
        // Résumé :
        //     Gets the IPv4 network gateway addresses for this interface.
        //
        // Retourne :
        //     An System.Net.NetworkInformation.GatewayIPAddressInformationCollection that contains
        //     the address information for network gateways, or an empty array if no gateways
        //     are found.
        public string GatewayAddresses => string.Join(", ", Properties.GatewayAddresses.Select(x => x.Address));
        //
        // Résumé :
        //     Gets a System.Boolean value that indicates whether NetBt is configured to use
        //     DNS name resolution on this interface.
        //
        // Retourne :
        //     true if NetBt is configured to use DNS name resolution on this interface; otherwise,
        //     false.
        public bool IsDnsEnabled => Properties.IsDnsEnabled;
        //
        // Résumé :
        //     Gets a System.Boolean value that indicates whether this interface is configured
        //     to automatically register its IP address information with the Domain Name System
        //     (DNS).
        //
        // Retourne :
        //     true if this interface is configured to automatically register a mapping between
        //     its dynamic IP address and static domain names; otherwise, false.
        public bool IsDynamicDnsEnabled => Properties.IsDynamicDnsEnabled;
        //
        // Résumé :
        //     Gets the multicast addresses assigned to this interface.
        //
        // Retourne :
        //     An System.Net.NetworkInformation.MulticastIPAddressInformationCollection that
        //     contains the multicast addresses for this interface.
        public string MulticastAddresses => string.Join(", ", Properties.MulticastAddresses.Select(x => x.Address));
        //
        // Résumé :
        //     Gets the unicast addresses assigned to this interface.
        //
        // Retourne :
        //     An System.Net.NetworkInformation.UnicastIPAddressInformationCollection that contains
        //     the unicast addresses for this interface.
        public string UnicastAddresses => string.Join(", ", Properties.UnicastAddresses.Select(x => x.Address));
        //
        // Résumé :
        //     Gets the addresses of Windows Internet Name Service (WINS) servers.
        //
        // Retourne :
        //     An System.Net.NetworkInformation.IPAddressCollection that contains the address
        //     information for WINS servers, or an empty array if no servers are found.
        public string WinsServersAddresses => string.Join(", ", Properties.WinsServersAddresses.Select(x => x));













        public PhysicalAddress PhysicalAddress => Interface.GetPhysicalAddress();

        public IPv4InterfaceProperties IPv4Properties { get; }
        public IPv6InterfaceProperties IPv6Properties { get; }



        public IPInterfaceProperties Properties { get; }

        public NetworkInterface Interface { get; }



        internal DiscoveryInterface(NetworkInterface net)
        {
            Interface = net;
            Properties = Interface.GetIPProperties();
            try
            {
                IPv4Properties = Properties.GetIPv4Properties();
            }
            catch (Exception) { }
            try
            {
                IPv6Properties = Properties.GetIPv6Properties();
            }
            catch (Exception) { }
        }
    }
}
