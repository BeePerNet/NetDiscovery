using DynamicData;
using DynamicData.Binding;
using Makaretu.Dns;
using ReactiveUI;
using Splat;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace NetDiscovery.Lib
{
    public class DiscoveryClient : DiscoveryNode<DiscoveryZone>
    {







        public override string Name => null;

        internal const string SERVICESFQDN = "_services._dns-sd._udp.";

        MulticastService mdns;
        ServiceDiscovery sd;
        DnsClient dnsClient;


        private bool running;
        public bool Running
        {
            get => running;
            private set => this.RaiseAndSetIfChanged(ref running, value);
        }

        private DateTime lastReceived;
        public DateTime LastReceived
        {
            get => lastReceived;
            private set => this.RaiseAndSetIfChanged(ref lastReceived, value);
        }


        private bool enableLog;
        public bool EnableLog
        {
            get => enableLog;
            set => this.RaiseAndSetIfChanged(ref enableLog, value);
        }


        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
        public ReactiveCommand<Unit, Unit> StartQueriesCommand { get; }

        public ReactiveCommand<Unit, Unit> ClearLogsCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearAllCommand { get; }



        public ReadOnlyObservableCollection<DiscoveryZone> Zones => Nodes;




        private readonly SourceList<DiscoveryInterface> updatebleInterfaces = new SourceList<DiscoveryInterface>();
        private readonly ReadOnlyObservableCollection<DiscoveryInterface> interfaces;
        public ReadOnlyObservableCollection<DiscoveryInterface> Interfaces => interfaces;
        IDisposable interfacesDisposable;

        private readonly SourceList<DiscoveryIP> updatebleIPs = new SourceList<DiscoveryIP>();
        private readonly ReadOnlyObservableCollection<DiscoveryIP> ips;
        public ReadOnlyObservableCollection<DiscoveryIP> IPs => ips;
        IDisposable ipsDisposable;


        private readonly SourceList<DiscoveryZone> updatebleAllZones = new SourceList<DiscoveryZone>();
        private readonly ReadOnlyObservableCollection<DiscoveryZone> allZones;
        public ReadOnlyObservableCollection<DiscoveryZone> AllZones => allZones;
        IDisposable allZonesDisposable;


        private readonly SourceList<DiscoveryRecord> updatebleAllRecords = new SourceList<DiscoveryRecord>();
        private readonly ReadOnlyObservableCollection<DiscoveryRecord> allRecords;
        public ReadOnlyObservableCollection<DiscoveryRecord> AllRecords => allRecords;
        IDisposable allRecordsDisposable;



        /*private readonly SourceList<Message> updatebleMessageLog = new SourceList<Message>();
        private readonly ReadOnlyObservableCollection<Message> messageLog;
        public ReadOnlyObservableCollection<Message> MessageLog => messageLog;
        IDisposable messageLogDisposable;*/

        private readonly SourceList<DiscoveryRecord> updatebleRecordLog = new SourceList<DiscoveryRecord>();
        private readonly ReadOnlyObservableCollection<DiscoveryRecord> recordLog;
        public ReadOnlyObservableCollection<DiscoveryRecord> RecordLog => recordLog;
        IDisposable recordLogDisposable;


        private readonly SourceList<QuestionLog> updatebleQuestionLog = new SourceList<QuestionLog>();
        private readonly ReadOnlyObservableCollection<QuestionLog> questionLog;
        public ReadOnlyObservableCollection<QuestionLog> QuestionLog => questionLog;
        IDisposable questionLogDisposable;

        private readonly SourceList<QueryLog> updatebleQueryLog = new SourceList<QueryLog>();
        private readonly ReadOnlyObservableCollection<QueryLog> queryLog;
        public ReadOnlyObservableCollection<QueryLog> QueryLog => queryLog;
        IDisposable queryLogDisposable;

        public DiscoveryClient() : base(DefaultComparer)
        {
            interfacesDisposable = updatebleInterfaces.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out interfaces).Subscribe();
            ipsDisposable = updatebleIPs.Connect().ObserveOn(RxApp.MainThreadScheduler).Sort(SortExpressionComparer<DiscoveryIP>.Ascending(x => x)).Bind(out ips).Subscribe();
            allZonesDisposable = updatebleAllZones.Connect().ObserveOn(RxApp.MainThreadScheduler).Sort(SortExpressionComparer<DiscoveryZone>.Ascending(x => x.DomainName)).Bind(out allZones).Subscribe();
            allRecordsDisposable = updatebleAllRecords.Connect().ObserveOn(RxApp.MainThreadScheduler).Sort(SortExpressionComparer<DiscoveryRecord>.Ascending(x => x.Name)).Bind(out allRecords).Subscribe();
            //messageLogDisposable = updatebleMessageLog.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out messageLog).Subscribe();
            recordLogDisposable = updatebleRecordLog.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out recordLog).Subscribe();
            questionLogDisposable = updatebleQuestionLog.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out questionLog).Subscribe();
            queryLogDisposable = updatebleQueryLog.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out queryLog).Subscribe();

            //AllRecords.ObserveCollectionChanges().ObserveOn(RxApp.TaskpoolScheduler).Do(x => ConnectResourceRecords(x.EventArgs.NewItems)).Subscribe();

            LastReceived = DateTime.MinValue;

            updatebleInterfaces.Edit(i =>
                i.Add(NetworkInterface.GetAllNetworkInterfaces().Select(x => new DiscoveryInterface(x))));

            StartCommand = ReactiveCommand.Create(Start, this.WhenAnyValue(x => x.Running).Select(x => !x));
            StopCommand = ReactiveCommand.Create(Stop, this.WhenAnyValue(x => x.Running));
            StartQueriesCommand = ReactiveCommand.Create(StartQueries);
            ClearLogsCommand = ReactiveCommand.Create(ClearLogs);
            ClearAllCommand = ReactiveCommand.Create(ClearAll);
        }

        internal DiscoveryClient(bool _) : base(DefaultComparer)
        {
            ObservableCollection<DiscoveryInterface> di = new ObservableCollection<DiscoveryInterface>();
            ObservableCollection<DiscoveryIP> dips = new ObservableCollection<DiscoveryIP>();
            //i.Add(new DiscoveryIP(IPAddress.Parse("127.0.0.1")));
            //i.Add()
            di.Add(NetworkInterface.GetAllNetworkInterfaces().Select(x => new DiscoveryInterface(x)));
            dips.Add(di.SelectMany(x => x.Interface.GetIPProperties().UnicastAddresses).Select(x => new DiscoveryIP(x.Address)).OrderBy(x => x));

            interfaces = new ReadOnlyObservableCollection<DiscoveryInterface>(di);
            ips = new ReadOnlyObservableCollection<DiscoveryIP>(dips);

            nodes = new ReadOnlyObservableCollection<DiscoveryZone>(new ObservableCollection<DiscoveryZone>(
                new DiscoveryZone[]
                {
                    DummyDiscoveryClient.Zone,
                    DummyDiscoveryClient.Zone,
                    DummyDiscoveryClient.Zone
                } ));

            allZones = nodes;

            allRecords = new ReadOnlyObservableCollection<DiscoveryRecord>(new ObservableCollection<DiscoveryRecord>(allZones.SelectMany(x => x.Records)));
        }

        private void InitializeServices()
        {
            if (mdns == null)
            {
                mdns = new MulticastService();
                mdns.IgnoreDuplicateMessages = false;
                mdns.MalformedMessage += Mdns_MalformedMessage;
                mdns.NetworkInterfaceDiscovered += Mdns_NetworkInterfaceDiscovered;
                mdns.AnswerReceived += Mdns_AnswerReceived;
                mdns.QueryReceived += Mdns_QueryReceived;

                sd = new ServiceDiscovery(mdns);
                sd.AnswersContainsAdditionalRecords = true;
                sd.ServiceDiscovered += Sd_ServiceDiscovered;
                sd.ServiceInstanceDiscovered += Sd_ServiceInstanceDiscovered;

                dnsClient = new DnsClient();
            }
        }

        private void Mdns_MalformedMessage(object sender, byte[] e)
        {
            this.Log().Error("Malformed message", e);
        }




        public void Start()
        {
            try
            {
                Running = true;
                if (mdns == null)
                    InitializeServices();
                mdns.Start();
            }
            catch (Exception ex)
            {
                this.Log().Fatal(ex);
                Running = false;
            }
        }

        public void StartQueries()
        {
            if (!Running)
                Start();

            if (Running)
                Observable.Start(() =>
                {
                    sd.QueryAllServices();

                    foreach (string dnssuffix in updatebleInterfaces.Items.Where(x => !string.IsNullOrWhiteSpace(x.DnsSuffix)).Select(x => x.DnsSuffix).Distinct())
                    {
                        CheckSendQuery(dnssuffix, true);
                        CheckSendQuery(SERVICESFQDN + dnssuffix, true);

                        ExecuteDomainQueries(dnssuffix, true);
                    }

                    ExecuteDomainQueries("local", false);

                }, RxApp.TaskpoolScheduler);

        }




        private void ExecuteDomainQueries(DomainName domain, bool unicast)
        {
            CheckSendQuery("_dns-update._udp." + domain, unicast);
            CheckSendQuery("_domain._udp." + domain, unicast);

            CheckSendQuery("b._dns-sd._udp." + domain, unicast);
            CheckSendQuery("lb._dns-sd._udp." + domain, unicast);
            CheckSendQuery("db._dns-sd._udp." + domain, unicast);  //db (default browse)
            CheckSendQuery("r._dns-sd._udp." + domain, unicast);   //r   (register)
            CheckSendQuery("dr._dns-sd._udp." + domain, unicast);  //dr (default register)
            CheckSendQuery("cf._dns-sd._udp." + domain, unicast);  //cf(configuration options-- not used)

            CheckSendQuery("_universal._sub._ipp._tcp." + domain, unicast);
            CheckSendQuery("_universal._sub._ipps._tcp." + domain, unicast);

            CheckSendQuery("_cups._sub._ipp._tcp." + domain, unicast);
            CheckSendQuery("_cups._sub._ipps._tcp." + domain, unicast);
        }



        private void Mdns_NetworkInterfaceDiscovered(object sender, NetworkInterfaceEventArgs e)
        {
            updatebleInterfaces.Edit(i =>
                i.Add(e.NetworkInterfaces.Where(x => !i.Any(y => x.Id == y.Id)).Select(x => new DiscoveryInterface(x))));
        }

        private void Mdns_AnswerReceived(object sender, MessageEventArgs e)
        {
            ParseMessage(e.Message, e.RemoteEndPoint);
        }

        private void Sd_ServiceInstanceDiscovered(object sender, ServiceInstanceDiscoveryEventArgs e)
        {
            ParseMessage(e.Message, e.RemoteEndPoint);
            //DiscoveryZone zone = GetZone(e.ServiceInstanceName);
            //Update zone
            //throw new NotImplementedException();

            //TODO: à vérifier
            //SendMulticastQuery(e.ServiceInstanceName);
        }

        private void Sd_ServiceDiscovered(object sender, DomainName e)
        {
            LastReceived = DateTime.Now;

            DiscoveryZone zone = GetZone(e);

            //Marche pas: 
            //sd.QueryServiceInstances(e);
            CheckSendQuery(e, zone.Unicast);
        }

        private void Mdns_QueryReceived(object sender, MessageEventArgs e)
        {
            e.Message.Questions.ForEach(x => ParseQuestionRecord(e.RemoteEndPoint, x));
        }

        public void ParseQuestionRecord(IPEndPoint from, Question res)
        {
            if (EnableLog)
                updatebleQuestionLog.Edit(x => x.Add(new QuestionLog(from, res)));
        }

        private void ParseMessage(Message message, IPEndPoint from)
        {
            LastReceived = DateTime.Now;

            message.AuthorityRecords.ForEach(x => ParseResourceRecord(x, from));
            message.Answers.ForEach(x => ParseResourceRecord(x, from));
            message.AdditionalRecords.ForEach(x => ParseResourceRecord(x, from));
        }



        public void ParseResourceRecord(ResourceRecord res, IPEndPoint from)
        {
            if (res == null)
                throw new ArgumentNullException(nameof(res), TextResources.RessourceRecordNull);

            DiscoveryRecord rec = new DiscoveryRecord(res, from);

            if (EnableLog)
                updatebleRecordLog.Edit(x => x.Add(rec));

            DiscoveryZone zone = GetZone(res.Name);

            if (from != null && rec.Name.ToString().StartsWith(SERVICESFQDN, StringComparison.InvariantCultureIgnoreCase))
            {

                lock (ipupdate)
                {
                    DiscoveryIP ip = GetIP(from.Address);
                    DiscoveryPort port = ip.updatebleNodes.Items.SingleOrDefault(x => x.Port == from.Port);
                    if (port == null)
                    {
                        port = new DiscoveryPort(TransportType.udp, from.Port);
                        port.AddChild(zone); ;
                        ip.updatebleNodes.Add(port);
                    }
                }
            }

            rec = UpdateRecord(zone, rec, from);

            if (zone.Unicast)
            {
                if (rec.Record is NSRecord || rec.Record is SOARecord)
                {
                    lock (ipupdate)
                    {
                        DiscoveryIP[] ips = Resolve(rec.Target).Select(x => GetIP(x)).ToArray();
                        DiscoveryPort port = ips.SelectMany(x => x.Ports).SingleOrDefault(y => y.Transport == TransportType.udp && y.Port == 53);
                        if (port == null)
                            port = new DiscoveryPort(TransportType.udp, 53);
                        if (!port.updatebleNodes.Items.Any(x => x.RecordDomainName == zone.RecordDomainName))
                            port.AddChild(zone);
                        foreach (DiscoveryIP ip in ips)
                            if (!ip.updatebleNodes.Items.Any(y => y.Transport == TransportType.udp && y.Port == 53))
                                ip.AddChild(port);
                    }
                }
            }
            if (rec.Record is AddressRecord addr)
                ParseIP(GetIP(addr.Address));
            else
            {
                string target = rec.Target;
                if (!string.IsNullOrWhiteSpace(target))
                {
                    zone = GetZone(target);

                    CheckSendQuery(target, zone.Unicast);
                }
            }
        }

        private readonly object ipupdate = new object();


        private IPAddress[] Resolve(DomainName name)
        {
            return updatebleAllRecords.Items.Where(x => x.Record is AddressRecord rec && rec.Name == name).Select(x => (x.Record as AddressRecord).Address).Distinct().ToArray();
            /*try
            {
                return dnsClient.ResolveAsync(name).Result;
            }
            catch (AggregateException ex)
            {
                this.Log().Error(ex, "Erreur requête");
            }
            return Array.Empty<IPAddress>();*/
        }


        private void ParseIP(DiscoveryIP ip)
        {
            lock (ipupdate)
            {
                List<DiscoveryRecord> finalList = new List<DiscoveryRecord>();

                DiscoveryRecord[] onePass = updatebleAllRecords.Items.Where(x => x.Target == ip.IP.ToString()).ToArray();
                while (onePass.Length > 0)
                {
                    finalList.Add(onePass);
                    string[] targets = onePass.Select(x => x.Name).ToArray();
                    onePass = updatebleAllRecords.Items.Where(x => x.Record is CNAMERecord && targets.Contains(x.Target)).ToArray();
                }

                DomainName[] domains = finalList.Select(x => x.DomainName).Distinct().ToArray();
                foreach (DomainName domain in domains)
                    if (!ip.updatebleDns.Items.Contains(domain))
                        ip.updatebleDns.Add(domain);

                DiscoveryRecord[] finalTargets = updatebleAllRecords.Items.Where(x => !string.IsNullOrWhiteSpace(x.Target) && domains.Contains(x.Target)).ToArray();

                foreach (DiscoveryRecord target in finalTargets)
                {
                    if (target.Record is SRVRecord srv)
                    {
                        DiscoveryZone zone = GetZone(target.DomainName);
                        int port = srv.Port;
                        TransportType transport = GetTransport(zone);
                        DiscoveryPort discoveryPort = ip.updatebleNodes.Items.SingleOrDefault(x => x.Transport == transport && x.Port == port);
                        if (discoveryPort == null)
                            discoveryPort = new DiscoveryPort(transport, port);
                        if (!discoveryPort.updatebleNodes.Items.Any(x => x.DomainName == zone.DomainName))
                            discoveryPort.AddChild(zone);

                        ip.AddChild(discoveryPort);
                    }
                }
            }
        }


        private static TransportType GetTransport(DiscoveryZone zone)
        {
            while (zone != null && !(zone is DiscoveryZoneProtocol))
                zone = zone.Parent;
            if (zone != null && zone is DiscoveryZoneProtocol pro)
                return pro.TransportType;
            return TransportType.unknown;
        }


        private DiscoveryZone GetZone(DomainName name)
        {
            DiscoveryZone zone = updatebleAllZones.Items.SingleOrDefault(x => x.RecordDomainName == name);
            if (zone == null)
            {
                updatebleAllZones.Edit(l =>
                {
                    zone = l.SingleOrDefault(x => x.RecordDomainName == name);
                    if (zone == null)
                        zone = CreateZone(l, name);
                });
            }
            return zone;
        }



        private DiscoveryZone CreateZone(IExtendedList<DiscoveryZone> l, DomainName name)
        {
            DiscoveryZone zone = l.SingleOrDefault(x => x.RecordDomainName == name);
            if (zone == null)
            {
                DomainName parentName = new DomainName(name.Labels.Skip(1).ToArray());
                DiscoveryZone parentZone = null;
                if (parentName.Labels.Count > 0)
                {
                    parentZone = l.SingleOrDefault(x => x.RecordDomainName == parentName);
                    if (parentZone == null)
                        parentZone = CreateZone(l, parentName);
                }
                zone = CreateZoneType(name, parentZone);
                if (parentZone == null)
                    this.AddChild(zone);
                else
                    parentZone.AddChild(zone);
                l.Add(zone);
                //ExecuteDomainQueries(zone); Non car ne repassera pas sur l'update
            }
            return zone;
        }

        private static DiscoveryZone CreateZoneType(DomainName domainName, DiscoveryZone parentZone)
        {
            if (parentZone == null)
                return new DiscoveryZoneDomain(domainName, parentZone);

            string label = domainName.Labels[0];
            bool dnsUnderline = label.StartsWith("_", StringComparison.Ordinal);
            if (dnsUnderline)
                label = label.Substring(1);

            if (parentZone is DiscoveryZoneDomain && dnsUnderline && Enum.TryParse<TransportType>(label, out TransportType tType))
                return new DiscoveryZoneProtocol(domainName, parentZone, tType);
            else if (parentZone is DiscoveryZoneProtocol && dnsUnderline)
            {
                /*if (IANAregistry.Instance.Records.Any(x => !string.IsNullOrWhiteSpace(x.ServiceName) && (x.TransportProtocol == TransportType.unknown || x.TransportProtocol == pZone.TransportType) && x.ServiceName.Equals(label, StringComparison.InvariantCultureIgnoreCase)))
                    return new DiscoveryZoneService(domainName, parentZone, label);
                else*/
                return new DiscoveryZoneService(domainName, parentZone, label);
            }
            else if (!dnsUnderline && parentZone is DiscoveryZoneDomain)
                return new DiscoveryZoneDomain(domainName, parentZone);
            else
                return new DiscoveryZone(domainName, parentZone);
        }









        private static DiscoveryRecord GetRecord(IEnumerable<DiscoveryRecord> l, DiscoveryRecord rec)
        {
            return l.SingleOrDefault(x => x.DomainName == rec.DomainName && x.Record.GetType() == rec.Record.GetType() && x.Value == rec.Value);
        }

        private DiscoveryRecord UpdateRecord(DiscoveryZone zone, DiscoveryRecord rec, IPEndPoint from)
        {
            DiscoveryRecord record = GetRecord(updatebleAllRecords.Items, rec);
            if (record == null)
            {
                updatebleAllRecords.Edit(l =>
                {
                    record = GetRecord(l, rec);
                    if (record == null)
                    {
                        record = rec;
                        l.Add(record);
                        zone.AddRecord(record);
                    }
                });
            }
            else
            {
                record.Update(rec.Record, from);
            }
            zone.Updated = DateTime.Now;
            return record;
        }









        private DiscoveryIP GetIP(IPAddress ip)
        {
            DiscoveryIP result = updatebleIPs.Items.SingleOrDefault(x => x.IP.Equals(ip));
            if (result == null)
            {
                updatebleIPs.Edit(l =>
                {
                    result = l.SingleOrDefault(x => x.IP.Equals(ip));
                    if (result == null)
                    {
                        result = new DiscoveryIP(ip);
                        l.Add(result);
                    }
                });
            }
            return result;
        }



        private void CheckSendQuery(DomainName domainName, bool unicast = false, DnsType type = DnsType.ANY)
        {
            if (Running)
            {
                Observable.Start(() =>
                {
                    bool send = type != DnsType.ANY;
                    if (!send)
                    {
                        DiscoveryRecord record = updatebleAllRecords.Items.Where(x => x.DomainName == domainName).OrderBy(x => x.Record.TTL).ThenBy(x => x.Updated).FirstOrDefault();
                        if (record == null || DateTime.Now > record.Updated + record.Record.TTL)
                            send = true;
                    }
                    if (send)
                        SendQuery(domainName, unicast, type);
                    else
                    {
                        IPAddress[] ips = updatebleAllRecords.Items.Where(x => x.DomainName == domainName && x.Record is AddressRecord).Select(x => (x.Record as AddressRecord).Address).Distinct().ToArray();
                        foreach (IPAddress ip in ips)
                            ParseIP(GetIP(ip));
                    }
                }, RxApp.TaskpoolScheduler);
            }
        }


        private void SendQuery(DomainName domainName, bool unicast = false, DnsType type = DnsType.ANY)
        {
            if (EnableLog)
                updatebleQueryLog.Edit(l => l.Add(new QueryLog(unicast, type, domainName)));

            Message message = new Message();
            Question question = new Question();
            question.Name = domainName;
            question.Type = type;
            message.Questions.Add(question);

            message.RA = true;
            message.RD = true;

            try
            {
                if (unicast)
                {
                    message.Id = dnsClient.NextQueryId();
                    Message msg = dnsClient.QueryAsync(message).Result;
                    ParseMessage(msg, null);
                }
                else
                {
                    mdns.SendQuery(message);
                }

            }
            catch (AggregateException ex)
            {
                this.Log().Error(ex, "Erreur requête", message);
            }
        }








        public void ClearLogs()
        {
            updatebleRecordLog.Edit(l => l.Clear());
            updatebleQuestionLog.Edit(l => l.Clear());
            updatebleQueryLog.Edit(l => l.Clear());
        }

        public void ClearAll()
        {
            ClearLogs();

            updatebleAllRecords.Edit(l => l.Clear());
            updatebleAllZones.Edit(l => l.Clear());
            updatebleIPs.Edit(l => l.Clear());
            updatebleNodes.Edit(l => l.Clear());
        }













        public void Stop()
        {
            Running = false;
            Observable.Start(() => DisposeServices(), RxApp.TaskpoolScheduler);
        }

        private void DisposeServices()
        {
            mdns?.Stop();
            sd?.Dispose();
            mdns?.Dispose();
            dnsClient?.Dispose();

            mdns = null;
            sd = null;
            dnsClient = null;
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
                return;

            if (disposing)
            {
                // Dispose all managed resources here.
                DisposeServices();

                ipsDisposable?.Dispose();
                updatebleIPs?.Dispose();
                ipsDisposable = null;

                interfacesDisposable?.Dispose();
                updatebleInterfaces?.Dispose();
                interfacesDisposable = null;

                allZonesDisposable?.Dispose();
                updatebleAllZones?.Dispose();
                allZonesDisposable = null;

                allRecordsDisposable?.Dispose();
                updatebleAllRecords?.Dispose();
                allRecordsDisposable = null;

                /*messageLogDisposable?.Dispose();
                updatebleMessageLog?.Dispose();
                messageLogDisposable = null;*/

                recordLogDisposable?.Dispose();
                updatebleRecordLog?.Dispose();
                recordLogDisposable = null;

                questionLogDisposable?.Dispose();
                updatebleQuestionLog?.Dispose();
                questionLogDisposable = null;

                queryLogDisposable?.Dispose();
                updatebleQueryLog?.Dispose();
                queryLogDisposable = null;
            }

            // Dispose of any unmanaged resources not wrapped in safe handles.

            _disposed = true;

            base.Dispose(disposing);
        }


    }
}
