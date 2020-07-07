using System;
using System.Collections.Generic;
using System.Text;

namespace NetDiscovery.Lib
{
    public enum TransportType
    {
        unknown = 0,
        udp = 1,
        tcp = 2,
        sctp,
        dccp
    }
}
