using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class Retrieval
    {
        public string[] RetData { get; set; }
    }
    public class RetrievalItem
    {
        public string FormNumber { get; set; }
        public string FormDetailsnumber { get; set; }
        public string ItemNumber { get; set; }
        public string BinNumber { get; set; }
        public string Description { get; set; }
        public string Dept { get; set; }
        public string Needed { get; set; }
        public string Actual { get; set; }
    }
}