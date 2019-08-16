using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class ViewModelDDetails
    {
        public DisbursementList disbursementList { get; set; }
        public DisbursementListDetail disbursementListDetail { get; set; }
        public StationeryCatalog stationeryCatalog { get; set; }
    }
}