using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public partial class RetrievalRecord
    {
        public string FormDetailsNumber { get; set; }
        public string binNumber { get; set; }
        public string description { get; set; }

        public string departmentCode { get; set; }
        public int needed { get; set; }
        public int actual { get; set; }

        public RetrievalRecord()
        {

        }

        public RetrievalRecord(string FormDetailsNumber, string binNumber, string description, string departmentCode, int needed, int actual)
        {
            this.FormDetailsNumber = FormDetailsNumber;
            this.binNumber = binNumber;
            this.description = description;
            this.departmentCode = departmentCode;
            this.needed = needed;
            this.actual = actual;
        }
    }
}