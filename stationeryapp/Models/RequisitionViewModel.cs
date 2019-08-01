using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class RequisitionViewModel
    {
        public string ItemNumber { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string DepartmentCode { get; set; }
        public int Needed { get; set; }
    }
}