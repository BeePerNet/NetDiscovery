using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class DiscoveryZoneDomain : DiscoveryZone
    {
        internal DiscoveryZoneDomain(DomainName name, DiscoveryZone parent) : base(name, parent)
        {
        }

    }
}
