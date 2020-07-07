using DynamicData;
using DynamicData.Binding;
using Makaretu.Dns;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reactive.Linq;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class DiscoveryIP : DiscoveryNode<DiscoveryPort>, IComparable<DiscoveryIP>, IComparable
    {
        public IPAddress IP { get; }

        public override string Name => IP.ToString();

        internal readonly SourceList<DomainName> updatebleDns = new SourceList<DomainName>();
        private readonly ReadOnlyObservableCollection<DomainName> dns;
        public ReadOnlyObservableCollection<DomainName> Dns => dns;
        IDisposable dnsDisposable;

        public ReadOnlyObservableCollection<DiscoveryPort> Ports => Nodes;

        internal DiscoveryIP(IPAddress ip) : base(SortExpressionComparer<DiscoveryPort>.Ascending(x => x.Port).ThenByAscending(x => x.Transport))
        {
            IP = ip;
            dnsDisposable = updatebleDns.Connect().ObserveOn(RxApp.MainThreadScheduler).Sort(SortExpressionComparer<DomainName>.Ascending(x => x.ToString())).Bind(out dns).Subscribe();
        }


        /// <summary>
        /// Dummy one
        /// </summary>
        /// <param name="domains"></param>
        /// <param name="ports"></param>
        internal DiscoveryIP(IPAddress ip, string[] domains, DiscoveryPort[] ports) : base(SortExpressionComparer<DiscoveryPort>.Ascending(x => x.Port).ThenByAscending(x => x.Transport))
        {
            IP = ip;

            dns = new ReadOnlyObservableCollection<DomainName>(new ObservableCollection<DomainName>(domains.OrderBy(x => x).Select(x => new DomainName(x))));

            nodes = new ReadOnlyObservableCollection<DiscoveryPort>(new ObservableCollection<DiscoveryPort>(ports.OrderBy(x => x.Port).ThenBy(x => x.Transport)));
        }

        internal override void AddChild(DiscoveryPort srv)
        {
            updatebleNodes.Edit(l =>
            {
                if (!l.Any(y => y == srv))
                    l.Add(srv);
            });
        }

        /// <summary>
        /// Flag stating if the current instance is already disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Dispose method override, releasing all managed resources of the sub-class.
        /// </summary>
        /// <param name="disposing">States 
        /// if the resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                dnsDisposable?.Dispose();
                updatebleDns?.Dispose();
                dnsDisposable = null;
            }

            // Dispose of any unmanaged resources not wrapped in safe handles.

            _disposed = true;

            base.Dispose(disposing);
        }


        public int CompareTo(DiscoveryIP other)
        {
            if (other == null)
                return -1;
            byte[] first = this.IP.GetAddressBytes();
            byte[] second = other.IP.GetAddressBytes();
            return first.Zip(second, (a, b) => a.CompareTo(b))
                        .FirstOrDefault(c => c != 0);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as DiscoveryIP);
        }
    }
}
