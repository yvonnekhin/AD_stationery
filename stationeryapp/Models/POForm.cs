using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class POForm
    {
        public PurchaseOrder purchaseOrder { get; set; }
        public PurchaseOrderDetail purchaseOrderDetail { get; set; }
        public SupplierList supplierList { get; set; }
        public StationeryCatalog stationeryCatalog { get; set; }
    }
}