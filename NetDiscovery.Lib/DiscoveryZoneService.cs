using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class DiscoveryZoneService : DiscoveryZone
    {
        public string ServiceName { get; }

        internal DiscoveryZoneService(DomainName name, DiscoveryZone parent, string sName) : base(name, parent)
        {
            ServiceName = sName;
        }

        public Uri Url
        {
            get
            {
                return new Uri(string.Format(CultureInfo.InvariantCulture, TextResources.IANADatabaseSearchUrl, this.Name));
            }
        }


    }
}
