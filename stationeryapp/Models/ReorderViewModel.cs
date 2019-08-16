using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class ReorderViewModel
    {
        public string ItemNumber { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public Nullable<int> Balance { get; set; }
        public Nullable<int> ReorderLevel { get; set; }
        public Nullable<int> ReorderQuantity { get; set; }
        public string PONumber { get; set; }
        public Nullable<System.DateTime> SupplyByDate { get; set; }
    }
}