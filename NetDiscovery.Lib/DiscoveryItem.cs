using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public abstract class DiscoveryItem: ReactiveObject
    {
        public abstract string Name { get; }

    }
}
