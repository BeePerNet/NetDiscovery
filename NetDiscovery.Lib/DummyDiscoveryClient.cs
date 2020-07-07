using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace NetDiscovery.Lib
{
    public static class DummyDiscoveryClient
    {
        public static DiscoveryClient Client { get; } = new DiscoveryClient(true);

        public static DiscoveryInterface Interface => Client.Interfaces.OrderBy(x => x.Interface.GetIPProperties().UnicastAddresses.Count).Last();

        public static DiscoveryIP IPs
        {
            get
            {
                return new DiscoveryIP(
                    NetworkInterface.GetAllNetworkInterfaces().OrderBy(x => x.GetIPProperties().UnicastAddresses.Count).Last().GetIPProperties().UnicastAddresses.First().Address,
                    new string[] { "flagada.org", "spacex.mars", "nothing.gov.us", "nasa.gov", "abigdomainenametotest.domain.qc.ca", "little.org", "jvendrien.com" },
                        new DiscoveryPort[]
                        {
                        new DiscoveryPort(TransportType.udp, 5353),
                        new DiscoveryPort(TransportType.udp, 53),
                        new DiscoveryPort(TransportType.udp, 123),
                        new DiscoveryPort(TransportType.tcp, 443),
                        new DiscoveryPort(TransportType.tcp, 25),
                        new DiscoveryPort(TransportType.tcp, 80),
                        new DiscoveryPort(TransportType.tcp, 21),
                        new DiscoveryPort(TransportType.tcp, 22),
                        new DiscoveryPort(TransportType.tcp, 445)
                        }
                );
            }
        }
        public static DiscoveryIP IP
        {
            get
            {
                return new DiscoveryIP(
                    NetworkInterface.GetAllNetworkInterfaces().OrderBy(x => x.GetIPProperties().UnicastAddresses.Count).Last().GetIPProperties().UnicastAddresses.First().Address,
                    new string[] { "flagada.org", "spacex.mars", "nothing.gov.us", "nasa.gov", "abigdomainenametotest.domain.qc.ca", "little.org", "jvendrien.com" },
                        new DiscoveryPort[]
                        {
                        new DiscoveryPort(TransportType.udp, 5353),
                        new DiscoveryPort(TransportType.udp, 53),
                        new DiscoveryPort(TransportType.udp, 123)
                        }
                );
            }
        }

        public static DiscoveryRecord Record
        {
            get
            {
                return new DiscoveryRecord(new ARecord() { Name = "flagada.lan", TTL = TimeSpan.FromMinutes(2), Type = DnsType.A, Address = IPAddress.Parse("192.168.0.1") }, new IPEndPoint(IPAddress.Parse("192.168.0.1"), 53));
            }
        }

        public static DiscoveryPort Port
        {
            get
            {
                return new DiscoveryPort(TransportType.tcp, 5353, new DiscoveryZone[] {
                    new DiscoveryZone(new DomainName("www.flagada.lan"), null, Array.Empty<DiscoveryZone>(), Array.Empty<DiscoveryRecord>()) }
                );
            }
        }

        public static DiscoveryZone Zone
        {
            get
            {
                return new DiscoveryZone(new DomainName("flagada.lan"), null,
                    new DiscoveryZone[]
                    {
                        new DiscoveryZone(new DomainName("www.flagada.lan"), null, Array.Empty<DiscoveryZone>(), Array.Empty<DiscoveryRecord>()),
                        new DiscoveryZone(new DomainName("ftp.flagada.lan"), null, Array.Empty<DiscoveryZone>(), Array.Empty<DiscoveryRecord>())
                    },
                    new DiscoveryRecord[]
                    {
                        new DiscoveryRecord(new ARecord() { Name="flagada.flagada.flagada.flagada.flagada.lan", TTL=TimeSpan.FromMinutes(2), Type = DnsType.A, Address=IPAddress.Parse("192.168.0.1") }, new IPEndPoint(IPAddress.Parse("192.168.0.1"), 53)),
                        new DiscoveryRecord(new SOARecord() { Name="flagada.lan", TTL=TimeSpan.FromMinutes(2), Type = DnsType.SOA, PrimaryName="r2d2.glagada.flagada.flagada.flagada.flagada.lan" }, new IPEndPoint(IPAddress.Parse("192.168.0.1"), 53)),
                        new DiscoveryRecord(new ARecord() { Name="flagada.lan", TTL=TimeSpan.FromMinutes(2), Type = DnsType.A, Address=IPAddress.Parse("192.168.0.1") }, new IPEndPoint(IPAddress.Parse("192.168.0.1"), 53))
                    });
            }
        }

    }
}
