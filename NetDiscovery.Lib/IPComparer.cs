using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NetDiscovery.Lib
{
    public class IPComparer : IComparer<IPAddress>
    {
        public int Compare(IPAddress x, IPAddress y)
        {
            byte[] first = x.GetAddressBytes();
            byte[] second = y.GetAddressBytes();
            return first.Zip(second, (a, b) => a.CompareTo(b))
                        .FirstOrDefault(c => c != 0);
        }
    }
}
