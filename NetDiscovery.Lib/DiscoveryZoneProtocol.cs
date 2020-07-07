using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class DiscoveryZoneProtocol : DiscoveryZone
    {
        public TransportType TransportType { get;}
        internal DiscoveryZoneProtocol(DomainName name, DiscoveryZone parent, TransportType tType) : base(name, parent)
        {
            TransportType = tType;
        }


    }
}
