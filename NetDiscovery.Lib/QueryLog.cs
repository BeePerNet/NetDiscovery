using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class QueryLog
    {
        public DateTime Timestamp { get; } = DateTime.Now;

        public bool Unicast { get; }

        public DnsType DnsType { get; }

        public DomainName Name { get; }

        internal QueryLog(bool unicast, DnsType dnsType, DomainName name)
        {
            Unicast = unicast;
            DnsType = dnsType;
            Name = name;
        }
    }
}
