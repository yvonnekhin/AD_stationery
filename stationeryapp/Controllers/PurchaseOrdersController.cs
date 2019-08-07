using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using stationeryapp.Models;

namespace stationeryapp.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: PurchaseOrders
        public ActionResult Index()
        {
            var purchaseOrders = db.PurchaseOrders.Include(p => p.StoreClerk).Include(p => p.StoreClerk1).Include(p => p.StoreSupervisor).Include(p => p.SupplierList);

            SystemGeneratePO();

            return View(purchaseOrders.ToList());
        }

        // GET: PurchaseOrders/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder purchaseOrder = db.PurchaseOrders.Find(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound();
            }

            List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();
            List<PurchaseOrderDetail> purchaseOrderDetails = db.PurchaseOrderDetails.ToList();
            List<SupplierList> suppliers = db.SupplierLists.ToList();
            List<StationeryCatalog> stationeryCatalogs = db.StationeryCatalogs.ToList();

            var purchaseJoinResult = (from sc in stationeryCatalogs
                                      join pod in purchaseOrderDetails on sc.ItemNumber equals pod.ItemNumber into table1
                                      from pod in table1.ToList()
                                      join p in purchaseOrders on pod.PONumber equals p.PONumber into table2
                                      from p in table2.ToList()
                                      join s in suppliers on p.SupplierCode equals s.SupplierCode into table3
                                      from s in table3.ToList()
                                      select new POForm
                                      { // result selector 
                                          purchaseOrder = p,
                                          purchaseOrderDetail = pod,
                                          supplierList = s,
                                          stationeryCatalog = sc
                                      }).Where(x => x.purchaseOrder.PONumber == id);

            SupplierList approvedSupplier1 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode1);
            SupplierList approvedSupplier2 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode2);
            SupplierList approvedSupplier3 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode3);

            List<SupplierList> approvedSupplierList = new List<SupplierList> { approvedSupplier1, approvedSupplier2, approvedSupplier3 };

            ViewBag.SupplierCode1 = new SelectList(approvedSupplierList, "SupplierCode", "SupplierName");

            return View(purchaseJoinResult.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(List<POForm> poForms)
        {
            if (ModelState.IsValid)
            {
                PurchaseOrder existingPo = db.PurchaseOrders.Find(poForms[0].purchaseOrder.PONumber);
                existingPo.SupplierCode = poForms[0].purchaseOrder.SupplierCode;
                existingPo.SupplyByDate = poForms[0].purchaseOrder.SupplyByDate;
                existingPo.PurchaseValue = poForms[0].purchaseOrder.PurchaseValue;

                foreach (POForm po in poForms)
                {
                    PurchaseOrderDetail existingPod = db.PurchaseOrderDetails.Find(po.purchaseOrderDetail.PODetailsNumber);
                    existingPod.Quantity = po.purchaseOrderDetail.Quantity;

                    PurchaseOrder existingPO = db.PurchaseOrders.Find(po.purchaseOrder.PONumber);
                    existingPO.Status = "Submitted";
                    existingPO.DateOrdered = DateTime.Now;
                    existingPO.DateApproved = DateTime.Now;
                    existingPO.ApprovedBy = "2";

                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();
            List<PurchaseOrderDetail> purchaseOrderDetails = db.PurchaseOrderDetails.ToList();
            List<SupplierList> suppliers = db.SupplierLists.ToList();
            List<StationeryCatalog> stationeryCatalogs = db.StationeryCatalogs.ToList();

            var purchaseJoinResult = (from sc in stationeryCatalogs
                                      join pod in purchaseOrderDetails on sc.ItemNumber equals pod.ItemNumber into table1
                                      from pod in table1.ToList()
                                      join p in purchaseOrders on pod.PONumber equals p.PONumber into table2
                                      from p in table2.ToList()
                                      join s in suppliers on p.SupplierCode equals s.SupplierCode into table3
                                      from s in table3.ToList()
                                      select new POForm
                                      { // result selector 
                                          purchaseOrder = p,
                                          purchaseOrderDetail = pod,
                                          supplierList = s,
                                          stationeryCatalog = sc
                                      }).Where(x => x.purchaseOrder.PONumber == poForms.ToList()[0].purchaseOrder.PONumber);

            SupplierList approvedSupplier1 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode1);
            SupplierList approvedSupplier2 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode2);
            SupplierList approvedSupplier3 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode3);

            List<SupplierList> approvedSupplierList = new List<SupplierList> { approvedSupplier1, approvedSupplier2, approvedSupplier3 };

            ViewBag.SupplierCode1 = new SelectList(approvedSupplierList, "SupplierCode", "SupplierName");

            return View(purchaseJoinResult.ToList());
        }

        // GET: PurchaseOrders/Details/5
        public ActionResult Receive(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder purchaseOrder = db.PurchaseOrders.Find(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound();
            }

            List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();
            List<PurchaseOrderDetail> purchaseOrderDetails = db.PurchaseOrderDetails.ToList();
            List<SupplierList> suppliers = db.SupplierLists.ToList();
            List<StationeryCatalog> stationeryCatalogs = db.StationeryCatalogs.ToList();

            var purchaseJoinResult = (from sc in stationeryCatalogs
                                      join pod in purchaseOrderDetails on sc.ItemNumber equals pod.ItemNumber into table1
                                      from pod in table1.ToList()
                                      join p in purchaseOrders on pod.PONumber equals p.PONumber into table2
                                      from p in table2.ToList()
                                      join s in suppliers on p.SupplierCode equals s.SupplierCode into table3
                                      from s in table3.ToList()
                                      select new POForm
                                      { // result selector 
                                          purchaseOrder = p,
                                          purchaseOrderDetail = pod,
                                          supplierList = s,
                                          stationeryCatalog = sc
                                      }).Where(x => x.purchaseOrder.PONumber == id);

            SupplierList approvedSupplier1 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode1);
            SupplierList approvedSupplier2 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode2);
            SupplierList approvedSupplier3 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode3);

            List<SupplierList> approvedSupplierList = new List<SupplierList> { approvedSupplier1, approvedSupplier2, approvedSupplier3 };

            ViewBag.SupplierCode1 = new SelectList(approvedSupplierList, "SupplierCode", "SupplierName");

            return View(purchaseJoinResult.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Receive(List<POForm> poForms)
        {
            if (ModelState.IsValid)
            {
                PurchaseOrder existingPo = db.PurchaseOrders.Find(poForms[0].purchaseOrder.PONumber);
                existingPo.SupplierCode = poForms[0].purchaseOrder.SupplierCode;
                existingPo.ReceivedDate = poForms[0].purchaseOrder.ReceivedDate;
                existingPo.ReceivedValue = poForms[0].purchaseOrder.ReceivedValue;

                foreach (POForm po in poForms)
                {
                    PurchaseOrderDetail existingPod = db.PurchaseOrderDetails.Find(po.purchaseOrderDetail.PODetailsNumber);
                    existingPod.ReceivedQuantity = po.purchaseOrderDetail.ReceivedQuantity;

                    //update stationery catalog balance
                    StationeryCatalog existingSC = db.StationeryCatalogs.Find(po.purchaseOrderDetail.ItemNumber);
                    existingSC.Balance = po.purchaseOrderDetail.Quantity;

                    //update purchase order status
                    PurchaseOrder existingPO = db.PurchaseOrders.Find(po.purchaseOrder.PONumber);
                    existingPO.Status = "Received";

                    //update outstanding list status

                    var findOutstandingList = (from o in db.OutstandingLists
                                               join fd in db.StationeryRetrievalFormDetails on o.RetrievalFormDetailsNumber equals fd.FormDetailsNumber into table1
                                               from fd in table1
                                               where fd.ItemNumber == po.purchaseOrderDetail.ItemNumber
                                               select o.OutstandingListNumber).ToList();

                    OutstandingList existingOutstandingList = db.OutstandingLists.Find(findOutstandingList[0]);
                    existingOutstandingList.Status = "Completed";

                    //update requisition form details status for item received

                    var findRequisitionDetail = (from rf in db.RequisitionForms
                                                 join rfd in db.RequisitionFormDetails on rf.FormNumber equals rfd.FormNumber into table1
                                                 from rfd in table1
                                                 orderby rf.DateApproved descending
                                                 where rfd.ItemNumber == po.purchaseOrderDetail.ItemNumber
                                                 select rfd.FormDetailsNumber).ToList();

                    RequisitionFormDetail requisitionFormDetail = db.RequisitionFormDetails.Find(findRequisitionDetail[0]);
                    requisitionFormDetail.Status = "Received";

                    //create stock adjustment voucher if quantity received!=quantity ordered
                    if (existingPod.Quantity != po.purchaseOrderDetail.ReceivedQuantity)
                    {
                        int difference = (int)po.purchaseOrderDetail.ReceivedQuantity - (int)existingPod.Quantity;

                        StockAdjustmentVoucher sav = new StockAdjustmentVoucher
                        {
                            AdjustmentVoucherNumber = (db.StockAdjustmentVouchers.Count() + 1).ToString(),
                            Status = "Pending",
                            Remarks = "System generated voucher"
                        };

                        StockAdjustmentVoucherDetail savd = new StockAdjustmentVoucherDetail
                        {
                            AdjustmentDetailsNumber = db.StockAdjustmentVoucherDetails.Count() + 1,
                            AdjustmentVoucherNumber = sav.AdjustmentVoucherNumber,
                            ItemNumber = po.purchaseOrderDetail.ItemNumber,
                            QuantityAdjusted = difference,
                            Reason = "Refer to Purchase Order Number: " + po.purchaseOrder.PONumber
                        };

                        db.StockAdjustmentVouchers.Add(sav);
                        db.StockAdjustmentVoucherDetails.Add(savd);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();
            List<PurchaseOrderDetail> purchaseOrderDetails = db.PurchaseOrderDetails.ToList();
            List<SupplierList> suppliers = db.SupplierLists.ToList();
            List<StationeryCatalog> stationeryCatalogs = db.StationeryCatalogs.ToList();

            var purchaseJoinResult = (from sc in stationeryCatalogs
                                      join pod in purchaseOrderDetails on sc.ItemNumber equals pod.ItemNumber into table1
                                      from pod in table1.ToList()
                                      join p in purchaseOrders on pod.PONumber equals p.PONumber into table2
                                      from p in table2.ToList()
                                      join s in suppliers on p.SupplierCode equals s.SupplierCode into table3
                                      from s in table3.ToList()
                                      select new POForm
                                      { // result selector 
                                          purchaseOrder = p,
                                          purchaseOrderDetail = pod,
                                          supplierList = s,
                                          stationeryCatalog = sc
                                      }).Where(x => x.purchaseOrder.PONumber == poForms.ToList()[0].purchaseOrder.PONumber);

            SupplierList approvedSupplier1 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode1);
            SupplierList approvedSupplier2 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode2);
            SupplierList approvedSupplier3 = db.SupplierLists.Find(purchaseJoinResult.ToList()[0].stationeryCatalog.SupplierCode3);

            List<SupplierList> approvedSupplierList = new List<SupplierList> { approvedSupplier1, approvedSupplier2, approvedSupplier3 };

            ViewBag.SupplierCode1 = new SelectList(approvedSupplierList, "SupplierCode", "SupplierName");

            return View(purchaseJoinResult.ToList());
        }

        public void SystemGeneratePO()
        {
            List<StationeryCatalog> stationeryCatalogs = db.StationeryCatalogs.ToList();
            List<PurchaseOrder> purchaseOrders1 = db.PurchaseOrders.ToList();
            List<PurchaseOrderDetail> purchaseOrderDetails1 = db.PurchaseOrderDetails.ToList();
            List<SupplierList> suppliers = db.SupplierLists.ToList();
            List<OutstandingList> outstandingLists = db.OutstandingLists.ToList();
            List<StationeryRetrievalFormDetail> stationeryRetrievalFormDetails = db.StationeryRetrievalFormDetails.ToList();

            //1. Order due to outstanding requests (backward looking)
            var outstandingToOrder = (from o in outstandingLists
                                      join fd in stationeryRetrievalFormDetails on o.RetrievalFormDetailsNumber equals fd.FormDetailsNumber into table1
                                      from fd in table1
                                      where o.Status == "Outstanding"
                                      select fd.ItemNumber).ToList();

            var alreadyOrderedForOutstanding = (from po in purchaseOrders1
                                                join pod in purchaseOrderDetails1 on po.PONumber equals pod.PONumber into table1
                                                from pod in table1
                                                where po.Status != "Received" && po.Reason == "Outstanding"
                                                select pod.ItemNumber).ToList();

            var toOrderForOutstanding = outstandingToOrder.Except(alreadyOrderedForOutstanding);

            foreach (string itemNumber in toOrderForOutstanding)
            {
                List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();
                List<PurchaseOrderDetail> purchaseOrderDetails = db.PurchaseOrderDetails.ToList();
                int poCount = purchaseOrders.Count() + 1;
                int podCount = purchaseOrderDetails.Count() + 1;

                var retrievalFormsShortage = (from o in outstandingLists
                                              join fd in stationeryRetrievalFormDetails on o.RetrievalFormDetailsNumber equals fd.FormDetailsNumber into table1
                                              from fd in table1
                                              where o.Status == "Outstanding" && fd.ItemNumber == itemNumber
                                              select fd.FormDetailsNumber).ToList();

                PurchaseOrder newPO = new PurchaseOrder
                {
                    PONumber = poCount.ToString(),
                    SupplierCode = db.StationeryCatalogs.Find(itemNumber).SupplierCode1,
                    DeliverTo = "Logic University",
                    Attention = "1",
                    OrderedBy = "1",
                    Status = "Not Submitted",
                    Reason = "Outstanding"
                };

                PurchaseOrderDetail newPOD = new PurchaseOrderDetail
                {
                    PODetailsNumber = podCount.ToString(),
                    PONumber = newPO.PONumber,
                    ItemNumber = itemNumber,
                    Quantity = (int)db.StationeryRetrievalFormDetails.Find(retrievalFormsShortage[0]).Needed - (int)db.StationeryRetrievalFormDetails.Find(retrievalFormsShortage[0]).Actual,
                    Remarks = "Oustanding List"

                };

                //update outstanding list PO Number
                var findOutstandingList = (from o in outstandingLists
                                           join fd in stationeryRetrievalFormDetails on o.RetrievalFormDetailsNumber equals fd.FormDetailsNumber into table1
                                           from fd in table1
                                           where fd.ItemNumber == itemNumber
                                           select o.OutstandingListNumber).ToList();

                OutstandingList existingOutstandingList = db.OutstandingLists.Find(findOutstandingList[0]);
                existingOutstandingList.PONumber = newPO.PONumber;

                db.PurchaseOrders.Add(newPO);
                db.PurchaseOrderDetails.Add(newPOD);
                db.SaveChanges();
            }

            //2.Order to stock up to predicted level (forward looking)
            var lowStockToOrder = (from sc in stationeryCatalogs
                                   where sc.Balance < sc.ReorderLevel
                                   select sc.ItemNumber).ToList();

            var alreadyOrdered = (from po in purchaseOrders1
                                  join pod in purchaseOrderDetails1 on po.PONumber equals pod.PONumber into table1
                                  from pod in table1
                                  where po.Status != "Received" && po.Reason == "Restock"
                                  select pod.ItemNumber).ToList();

            var toOrderForLowStock = lowStockToOrder.Except(alreadyOrdered);

            foreach (string itemNumber in toOrderForLowStock)
            {
                List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();
                List<PurchaseOrderDetail> purchaseOrderDetails = db.PurchaseOrderDetails.ToList();
                int poCount = purchaseOrders.Count() + 1;
                int podCount = purchaseOrderDetails.Count() + 1;

                PurchaseOrder newPO = new PurchaseOrder
                {
                    PONumber = poCount.ToString(),
                    SupplierCode = db.StationeryCatalogs.Find(itemNumber).SupplierCode1,
                    DeliverTo = "Logic University",
                    Attention = "1",
                    OrderedBy = "1",
                    Status = "Not Submitted",
                    Reason = "Restock"
                };

                PurchaseOrderDetail newPOD = new PurchaseOrderDetail
                {
                    PODetailsNumber = podCount.ToString(),
                    PONumber = newPO.PONumber,
                    ItemNumber = itemNumber,
                    Quantity = (int)db.StationeryCatalogs.Find(itemNumber).ReorderQuantity,
                    Remarks = "Re-order level"

                };

                db.PurchaseOrders.Add(newPO);
                db.PurchaseOrderDetails.Add(newPOD);
                db.SaveChanges();
            }

        }

        // GET: PurchaseOrders/Create
        public ActionResult Create()
        {
            ViewBag.Attention = new SelectList(db.StoreClerks, "Id", "FirstName");
            ViewBag.OrderedBy = new SelectList(db.StoreClerks, "Id", "FirstName");
            ViewBag.ApprovedBy = new SelectList(db.StoreSupervisors, "Id", "FirstName");
            ViewBag.SupplierCode = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName");
            return View();
        }

        // POST: PurchaseOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PONumber,SupplierCode,DeliverTo,Attention,SupplyByDate,OrderedBy,DateOrdered,ApprovedBy,DateApproved,ReceivedGoodsFormNo,ReceivedDate,ReceivedValue,Status")] PurchaseOrder purchaseOrder)
        {
            if (ModelState.IsValid)
            {
                db.PurchaseOrders.Add(purchaseOrder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Attention = new SelectList(db.StoreClerks, "Id", "FirstName", purchaseOrder.Attention);
            ViewBag.OrderedBy = new SelectList(db.StoreClerks, "Id", "FirstName", purchaseOrder.OrderedBy);
            ViewBag.ApprovedBy = new SelectList(db.StoreSupervisors, "Id", "FirstName", purchaseOrder.ApprovedBy);
            ViewBag.SupplierCode = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", purchaseOrder.SupplierCode);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder purchaseOrder = db.PurchaseOrders.Find(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound();
            }
            ViewBag.Attention = new SelectList(db.StoreClerks, "Id", "FirstName", purchaseOrder.Attention);
            ViewBag.OrderedBy = new SelectList(db.StoreClerks, "Id", "FirstName", purchaseOrder.OrderedBy);
            ViewBag.ApprovedBy = new SelectList(db.StoreSupervisors, "Id", "FirstName", purchaseOrder.ApprovedBy);
            ViewBag.SupplierCode = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", purchaseOrder.SupplierCode);
            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PONumber,SupplierCode,DeliverTo,Attention,SupplyByDate,OrderedBy,DateOrdered,ApprovedBy,DateApproved,ReceivedGoodsFormNo,ReceivedDate,ReceivedValue,Status")] PurchaseOrder purchaseOrder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseOrder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Attention = new SelectList(db.StoreClerks, "Id", "FirstName", purchaseOrder.Attention);
            ViewBag.OrderedBy = new SelectList(db.StoreClerks, "Id", "FirstName", purchaseOrder.OrderedBy);
            ViewBag.ApprovedBy = new SelectList(db.StoreSupervisors, "Id", "FirstName", purchaseOrder.ApprovedBy);
            ViewBag.SupplierCode = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", purchaseOrder.SupplierCode);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder purchaseOrder = db.PurchaseOrders.Find(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound();
            }
            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PurchaseOrder purchaseOrder = db.PurchaseOrders.Find(id);

            List<PurchaseOrderDetail> pod = db.PurchaseOrderDetails.ToList();

            var podToDelete = (from purchaseOrderDetail in pod
                               where purchaseOrderDetail.PONumber == id
                               select purchaseOrderDetail).ToList();

            foreach(PurchaseOrderDetail purchaseOrderDetail in podToDelete)
            {
                db.PurchaseOrderDetails.Remove(purchaseOrderDetail);
                db.SaveChanges();
            }

            List<OutstandingList> osList = db.OutstandingLists.ToList();

            var osToUpdate = (from outstandingList in osList
                              where outstandingList.PONumber == id
                              select outstandingList).ToList();

            foreach(OutstandingList outstandingList in osToUpdate)
            {
                OutstandingList existingOs = db.OutstandingLists.Find(outstandingList.OutstandingListNumber);
                existingOs.PONumber = "null";
                db.SaveChanges();
            }

            db.PurchaseOrders.Remove(purchaseOrder);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
