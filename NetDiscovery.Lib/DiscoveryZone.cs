using DynamicData;
using Makaretu.Dns;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class DiscoveryZone : DiscoveryNode<DiscoveryZone>
    {

        public string DomainName => string.Join(".", RecordDomainName.Labels);

        public bool Unicast => ReverseDomainName.Labels[0] != "local";

        public ReadOnlyObservableCollection<DiscoveryZone> Zones => Nodes;

        internal readonly SourceList<DiscoveryRecord> updatebleRecords = new SourceList<DiscoveryRecord>();
        private readonly ReadOnlyObservableCollection<DiscoveryRecord> records;
        public ReadOnlyObservableCollection<DiscoveryRecord> Records => records;
        IDisposable recordsDisposable;

        public DiscoveryZone Parent { get; }

        public DomainName RecordDomainName { get; }

        public string ReverseName => string.Join(".", ReverseDomainName.Labels);

        public DomainName ReverseDomainName { get; }
        public override string Name => RecordDomainName.Labels[0];

        internal DiscoveryZone(DomainName name, DiscoveryZone parent) : base(DefaultComparer)
        {
            RecordDomainName = name;
            ReverseDomainName = new DomainName(name.Labels.Reverse().ToArray());
            Parent = parent;
            recordsDisposable = updatebleRecords.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out records).Subscribe();
        }

        /// <summary>
        /// Dummy one
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="childs"></param>
        /// <param name="records"></param>
        internal DiscoveryZone(DomainName name, DiscoveryZone parent, DiscoveryZone[] childZones, DiscoveryRecord[] childRecords) : base(DefaultComparer)
        {
            RecordDomainName = name;
            ReverseDomainName = new DomainName(name.Labels.Reverse().ToArray());
            Parent = parent;

            nodes = new ReadOnlyObservableCollection<DiscoveryZone>(new ObservableCollection<DiscoveryZone>(childZones));

            records = new ReadOnlyObservableCollection<DiscoveryRecord>(new ObservableCollection<DiscoveryRecord>(childRecords));
        }

        internal void AddRecord(DiscoveryRecord child)
        {
            updatebleRecords.Edit(l => l.Add(child));
        }





        public override string ToString()
        {
            return DomainName.ToString();
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
                recordsDisposable?.Dispose();
                recordsDisposable = null;
                updatebleRecords?.Dispose();
            }

            // Dispose of any unmanaged resources not wrapped in safe handles.

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}
