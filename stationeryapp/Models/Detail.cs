using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class Detail
    {
        public string[] Infos1 { get; set; }
    }
    public class Infos
    {
        public string DetailId { get; set; }
        public string ListId { get; set; }
        public string ItemNumber { get; set; }
        public string Category { get; set; }
        public string Desc { get; set; }
        public string Quantity { get; set; }
        public string Actual { get; set; }
        public string Remark { get; set; }
    }
}