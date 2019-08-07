using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class RetrievalHistory
    {
        public Nullable<System.DateTime> date { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public StationeryRetrievalFormDetail stationeryRetrievalFormDetail { get; set; }
        public PurchaseOrderDetail purchaseOrderDetail { get; set; }
        public StockAdjustmentVoucherDetail stockAdjustmentVoucherDetail { get; set; }
    }
}