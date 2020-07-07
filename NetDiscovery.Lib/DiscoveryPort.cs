using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class DiscoveryPort : DiscoveryNode<DiscoveryZone>
    {
        public int Port { get; }
        public TransportType Transport { get; }
        //public int Priority { get; }
        public IANARegistryRecord IANARecord { get; }

        internal DiscoveryPort(TransportType transport, int port, DiscoveryZone[] childZones) : base(DefaultComparer)
        {
            Transport = transport;
            Port = port;

            nodes = new ReadOnlyObservableCollection<DiscoveryZone>(new ObservableCollection<DiscoveryZone>(childZones));
        }

        internal DiscoveryPort(TransportType transport, int port) : base(DefaultComparer)
        {
            //Record = record;
            Transport = transport;
            Port = port;
            //Priority = priority;

            /*if (port.HasValue)
            {
                Port = port.Value;
            }
            else
            {
                Tuple<int, int> portPriority = GetPortPriority(record.Record);
                Port = portPriority.Item1;
                Priority = portPriority.Item2;
            }*/
            //TODO: Ajouter le IANA
            //if (Port > 0)
            //IANARegistryRecord
        }


        public override string Name => Port.ToString(CultureInfo.InvariantCulture);

        //public DiscoveryRecord Record { get; }

        public ReadOnlyObservableCollection<DiscoveryZone> Instances => Nodes;

        /*internal override void AddChild(DiscoveryZone zone)
        {
            updatebleNodes.Edit(l =>
            {
                l.Add(zone);
            });
        }*/


        //TODO: À supprimer
        private static TransportType GetTransport(DomainName domainName)
        {
            if (domainName.Labels.Contains("_tcp"))
                return TransportType.tcp;
            else if (domainName.Labels.Contains("_udp"))
                return TransportType.udp;
            else
                return TransportType.unknown;
        }

        //TODO: à vérifier
        internal static Tuple<int, int, TransportType> GetPortTransportPriority(ResourceRecord record)
        {
            if (record is SRVRecord srv)
            {
                return new Tuple<int, int, TransportType>(srv.Port, srv.Priority, GetTransport(record.Name));
            }
            else if (record is MXRecord mx)
            {
                return new Tuple<int, int, TransportType>(25, mx.Preference, TransportType.tcp);
            }
            else if (record is NSRecord)
            {
                return new Tuple<int, int, TransportType>(53, 0, TransportType.udp);
            }
            else if (record is SOARecord)
            {
                return new Tuple<int, int, TransportType>(53, 0, TransportType.udp);
            }
            else
                throw new NotImplementedException();
        }


    }
}
