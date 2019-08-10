using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class ViewModelOutstanding
    {
        //public OutstandingList outstandingLists { get; set; }
        //public StationeryCatalog catalogs { get; set; }
        //public StationeryRetrievalFormDetail retrievalFormDetails { get; set; }
        //public PurchaseOrderDetail purchaseOrderDetails { get; set; }
        //public PurchaseOrder purchaseOrders { get; set; }

        public string ItemCode { get; set; }
        public string Description { get; set; }
        public int ShortageQuantity { get; set; }
        public int InventoryBalance { get; set; }
        public string Status { get; set; }
    }
}