using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class IANARegistryRecord
    {
        [CsvColumn(Name = "Service Name", FieldIndex = 1)]
        public string ServiceName { get; set; }

        [CsvColumn(Name = "Port Number", FieldIndex = 2)]
        public string PortNumber { get; set; }

        [CsvColumn(Name = "Transport Protocol", FieldIndex = 3)]
        public TransportType TransportProtocol { get; set; }

        [CsvColumn(Name = "Description", FieldIndex = 4)]
        public string Description { get; set; }

        [CsvColumn(Name = "Assignee", FieldIndex = 5)]
        public string Assignee { get; set; }

        [CsvColumn(Name = "Contact", FieldIndex = 6)]
        public string Contact { get; set; }

        [CsvColumn(Name = "Registration Date", FieldIndex = 7)]
        public DateTime? RegistrationDate { get; set; }

        [CsvColumn(Name = "Modification Date", FieldIndex = 8)]
        public DateTime? ModificationDate { get; set; }

        [CsvColumn(Name = "Reference", FieldIndex = 9)]
        public string Reference { get; set; }

        [CsvColumn(Name = "Service Code", FieldIndex = 10)]
        public string ServiceCode { get; set; }

        [CsvColumn(Name = "Unauthorized Use Reported", FieldIndex = 11)]
        public string UnauthorizedUseReported { get; set; }

        [CsvColumn(Name = "Assignment Notes", FieldIndex = 12)]
        public string AssignmentNotes { get; set; }

        public int PortNumberMax { get; set; }

        public int PortNumberMin { get; set; }
    }
}
