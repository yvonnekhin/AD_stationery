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
    
    public partial class SupplierList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SupplierList()
        {
            this.StationeryCatalogs = new HashSet<StationeryCatalog>();
            this.StationeryCatalogs1 = new HashSet<StationeryCatalog>();
            this.StationeryCatalogs2 = new HashSet<StationeryCatalog>();
            this.PurchaseOrders = new HashSet<PurchaseOrder>();
        }
    
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string GSTNo { get; set; }
        public string ContactName { get; set; }
        public Nullable<int> PhoneNo { get; set; }
        public Nullable<int> FaxNo { get; set; }
        public string Address { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StationeryCatalog> StationeryCatalogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StationeryCatalog> StationeryCatalogs1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StationeryCatalog> StationeryCatalogs2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
