using Makaretu.Dns;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class DiscoveryRecord : ReactiveObject
    {
        public string Name => string.Join(".", DomainName.Labels);
        public DnsType Type => Record.Type;
        public string Target => GetTarget();

        private DateTime updated;
        public DateTime Updated
        {
            get => updated;
            private set => this.RaiseAndSetIfChanged(ref updated, value);
        }

        public TimeSpan TTL => Record.TTL;
        public DateTime Created { get; } = DateTime.Now;
        public DateTime RecordCreated => Record.CreationTime;

        public DomainName DomainName => Record.Name;
        public string ReverseName => ReverseDomainName.ToString();
        public DomainName ReverseDomainName { get; }

        private IPEndPoint from;
        public IPEndPoint From
        {
            get => from;
            private set => this.RaiseAndSetIfChanged(ref from, value);
        }

        public string Value => GetValue();

        internal DiscoveryRecord(ResourceRecord rec, IPEndPoint fromIPEndPoint)
        {
            from = fromIPEndPoint;
            Updated = DateTime.Now;
            Record = rec;
            ReverseDomainName = new DomainName(rec.Name.Labels.Reverse().ToArray());
        }


        private ResourceRecord record;
        public ResourceRecord Record
        {
            get => record;
            private set => this.RaiseAndSetIfChanged(ref record, value);
        }

        internal void Update(ResourceRecord rec, IPEndPoint fromIPEndPoint)
        {
            if (fromIPEndPoint != null)
                from = fromIPEndPoint;
            Updated = DateTime.Now;
            Record = rec;

            this.RaisePropertyChanged(nameof(Target));
            this.RaisePropertyChanged(nameof(Value));
            this.RaisePropertyChanged(nameof(RecordCreated));
        }

        private string GetTarget()
        {
            if (Record is AddressRecord address)
                return address.Address.ToString();

            else if (Record is SRVRecord srv)
                return srv.Target.ToString();
            else if (Record is SOARecord soa)
                return soa.PrimaryName.ToString();
            else if (Record is NSRecord ns)
                return ns.Authority.ToString();
            else if (Record is MXRecord mx)
                return mx.Exchange.ToString();
            else if (Record is PTRRecord ptr)
                return ptr.DomainName.ToString();
            else if (Record is CNAMERecord cname)
                return cname.Target.ToString();
            else
                return string.Empty;
        }



        private string GetValue()
        {
            if (Record is AddressRecord address)
                return address.Address.ToString();
            else if (Record is SRVRecord srv)
                return string.Concat(srv.Target, " ", srv.Priority);
            else if (Record is SOARecord soa)
                return soa.PrimaryName.ToString();
            else if (Record is NSRecord ns)
                return ns.Authority.ToString();
            else if (Record is TXTRecord txt)
                return string.Join(", ", txt.Strings);
            else if (Record is MXRecord mx)
                return string.Concat(mx.Exchange, " ", mx.Preference);
            else if (Record is PTRRecord ptr)
                return ptr.DomainName.ToString();
            else if (Record is CNAMERecord cname)
                return cname.Target.ToString();
            else
                return Record.ToString();
        }

        public override string ToString()
        {
            return Record.ToString();
        }

    }

}
