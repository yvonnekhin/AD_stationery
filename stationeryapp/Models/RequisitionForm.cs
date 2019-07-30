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
    
    public partial class RequisitionForm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RequisitionForm()
        {
            this.OutstandingLists = new HashSet<OutstandingList>();
            this.RequisitionFormDetails = new HashSet<RequisitionFormDetail>();
        }
    
        public string FormNumber { get; set; }
        public string EmployeeId { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> DateApproved { get; set; }
        public string ReceivedBy { get; set; }
        public Nullable<System.DateTime> DateReceived { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public string Notification { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OutstandingList> OutstandingLists { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequisitionFormDetail> RequisitionFormDetails { get; set; }
        public virtual StoreClerk StoreClerk { get; set; }
    }
}
