//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace stationeryapp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseOrder()
        {
            this.PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
            this.OutstandingLists = new HashSet<OutstandingList>();
        }
    
        public string PONumber { get; set; }
        public string SupplierCode { get; set; }
        public string DeliverTo { get; set; }
        public string Attention { get; set; }
        //public Nullable<System.DateTime> SupplyByDate { get; set; }
        public System.DateTime SupplyByDate { get; set; }
        public string OrderedBy { get; set; }
        //public Nullable<System.DateTime> DateOrdered { get; set; }
        public System.DateTime DateOrdered { get; set; }
        public string ApprovedBy { get; set; }
        //public Nullable<System.DateTime> DateApproved { get; set; }
        public System.DateTime DateApproved { get; set; }
        public string ReceivedGoodsFormNo { get; set; }
        //public Nullable<System.DateTime> ReceivedDate { get; set; }
        public System.DateTime ReceivedDate { get; set; }
        //public Nullable<double> ReceivedValue { get; set; }
        public double ReceivedValue { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual StoreClerk StoreClerk { get; set; }
        public virtual StoreClerk StoreClerk1 { get; set; }
        public virtual StoreSupervisor StoreSupervisor { get; set; }
        public virtual SupplierList SupplierList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OutstandingList> OutstandingLists { get; set; }
    }
}
