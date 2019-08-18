using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class AdjList
    {
        public string[] Infos { get; set; }
    }
    public class Info
    {
        public string ItemNumber { get; set; }
        public string QuantityAdjusted { get; set; }
        public string Reason { get; set; }
    }
}