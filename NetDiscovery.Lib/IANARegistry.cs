using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Text;
using LINQtoCSV;
using System.Net;
using System.Globalization;
using System.ComponentModel;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public sealed partial class IANAregistry
    {
        public ReadOnlyCollection<IANARegistryRecord> Records { get; private set; }

        private IANAregistry() { }

        private static readonly object _lock = new object();

        private static IANAregistry instance;
        public static IANAregistry Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            IANAregistry newInstance = new IANAregistry();

                            CsvFileDescription inputFileDescription = new CsvFileDescription
                            {
                                SeparatorChar = ',',
                                FirstLineHasColumnNames = true,
                            };
                            ReadOnlyCollection<IANARegistryRecord> i;
                            Assembly assembly = typeof(IANAregistry).Assembly;
                            using (Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".service-names-port-numbers.csv"))
                            {
                                using (StreamReader reader = new StreamReader(stream))
                                {
                                    CsvContext context = new CsvContext();

                                    i = context.Read<IANARegistryRecord>(reader, inputFileDescription).ToList().AsReadOnly();
                                }
                            }
                            i.AsParallel().ForAll(x =>
                            {
                                if (!string.IsNullOrWhiteSpace(x.PortNumber))
                                {
                                    string[] values = x.PortNumber.Split('-');
                                    if (values.Length > 1)
                                    {
                                        x.PortNumberMin = int.Parse(values[0], CultureInfo.InvariantCulture);
                                        x.PortNumberMax = int.Parse(values[1], CultureInfo.InvariantCulture);
                                    }
                                    else
                                        x.PortNumberMin = x.PortNumberMax = int.Parse(x.PortNumber, CultureInfo.InvariantCulture);
                                }
                            });
                            newInstance.Records = i;
                            instance = newInstance;
                        }
                    }
                }
                return instance;
            }
        }
    }
}
