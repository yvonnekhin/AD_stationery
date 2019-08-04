using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class ViewModelRetrieval
    {
        public StationeryRetrievalFormDetail retrievalFormDetails { get; set; }
        public StationeryCatalog catalogs { get; set; }
        public DepartmentList departmentLists { get; set; }

        public RequisitionForm requisitionForms { get; set; }
        public Employee employees { get; set; }
        public RequisitionFormDetail requisitionFormDetails { get; set; }

        public PurchaseOrder purchaseOrders { get; set; }

        public OutstandingList outstandingLists { get; set; }

        public ConsolidatedRequest consolidatedRequest { get; set; }
        
        public RetrievalRecord retrievalRecord { get; set; }
    }
}