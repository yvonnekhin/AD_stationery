using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using stationeryapp.Models;
using stationeryapp.Util;

namespace stationeryapp.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: PurchaseOrders
        public ActionResult Index(string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            var purchaseOrders = db.PurchaseOrders.Include(p => p.StoreClerk).Include(p => p.StoreClerk1).Include(p => p.StoreSupervisor).Include(p => p.SupplierList).OrderBy(p=>p.DateOrdered).Where(p=>p.Status!="Merged");


            if (storeclerk != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                return View(purchaseOrders.ToList());
            }
            else if (storeManager != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                return View(purchaseOrders.ToList());
            }
            else if (storeSupervisor != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
                return View(purchaseOrders.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // GET: PurchaseOrders/Details/5
        public ActionResult Details(string id, string sessionId)
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
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
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

            if (storeclerk != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                return View(purchaseJoinResult.ToList());
            }
            else if (storeManager != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                return View(purchaseJoinResult.ToList());

            }
            else if (storeSupervisor != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
                return View(purchaseJoinResult.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(List<POForm> poForms, string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
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
                return RedirectToAction("Index","PurchaseOrders", new { sessionId = sessionId });
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

            if (storeclerk != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                return View(purchaseJoinResult.ToList());
            }
            else if (storeManager != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                return View(purchaseJoinResult.ToList());

            }
            else if (storeSupervisor != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
                return View(purchaseJoinResult.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }      
        }

        // GET: PurchaseOrders/Receive/5
        public ActionResult Receive(string id, string sessionId)
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
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
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

            if (storeclerk != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                return View(purchaseJoinResult.ToList());
            }
            else if (storeManager != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                return View(purchaseJoinResult.ToList());

            }
            else if (storeSupervisor != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
                return View(purchaseJoinResult.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Receive(List<POForm> poForms, string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
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
                    existingSC.Balance += po.purchaseOrderDetail.ReceivedQuantity;

                    //update purchase order status
                    PurchaseOrder existingPO = db.PurchaseOrders.Find(po.purchaseOrder.PONumber);
                    existingPO.Status = "Received";

                    //update outstanding list status

                    if (po.purchaseOrderDetail.Remarks == "Outstanding")
                    {
                        var findOutstandingList = (from o in db.OutstandingLists
                                                   join fd in db.StationeryRetrievalFormDetails on o.RetrievalFormDetailsNumber equals fd.FormDetailsNumber into table1
                                                   from fd in table1
                                                   join rf in db.StationeryRetrievalForms on fd.FormNumber equals rf.FormNumber into table2
                                                   from rf in table2
                                                   orderby rf.Date
                                                   where fd.ItemNumber == po.purchaseOrderDetail.ItemNumber && o.Status=="Awaiting Goods"
                                                   select o).ToList();

                        foreach(OutstandingList ol in findOutstandingList) { 
                        OutstandingList existingOutstandingList = db.OutstandingLists.Find(ol.OutstandingListNumber);
                        existingOutstandingList.Status = "Completed";
                        }


                        //update requisition form details status for item received

                        var findRequisitionDetail = (from rf in db.RequisitionForms
                                                 join rfd in db.RequisitionFormDetails on rf.FormNumber equals rfd.FormNumber into table1
                                                 from rfd in table1
                                                 orderby rf.DateApproved descending
                                                 where rfd.ItemNumber == po.purchaseOrderDetail.ItemNumber
                                                 select rfd.FormDetailsNumber).ToList();

                    RequisitionFormDetail requisitionFormDetail = db.RequisitionFormDetails.Find(findRequisitionDetail[0]);
                    requisitionFormDetail.Status = "Received";

                    }

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
                return RedirectToAction("Index", "PurchaseOrders",new { sessionId=sessionId});
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
            if (storeclerk != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                return View(purchaseJoinResult.ToList());
            }
            else if (storeManager != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                return View(purchaseJoinResult.ToList());

            }
            else if (storeSupervisor != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
                return View(purchaseJoinResult.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

            
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
            List<PurchaseOrderDetail> orderForOutstandingList = (from o in outstandingLists
                                      join fd in stationeryRetrievalFormDetails on o.RetrievalFormDetailsNumber equals fd.FormDetailsNumber into table1
                                      from fd in table1
                                      where o.Status == "Outstanding"
                                      group fd by new {fd.ItemNumber} into group1
                                      select new PurchaseOrderDetail //we will only use itemcode, quantity to order here -> not saving to db, so IDs not required
                                      {
                                          ItemNumber = group1.Key.ItemNumber,
                                          Quantity = group1.Sum(x => x.Needed)-group1.Sum(x => x.Actual),
                                          Remarks = "Outstanding"

                                      }).ToList();

            //2.Order to stock up to predicted level (forward looking)

            var lowStockToOrder = (from sc in stationeryCatalogs
                                   where sc.Balance <= sc.ReorderLevel
                                   select sc.ItemNumber).ToList();

            var alreadyOrdered = (from po in purchaseOrders1
                                  join pod in purchaseOrderDetails1 on po.PONumber equals pod.PONumber into table1
                                  from pod in table1
                                  where po.Status == "Not Submitted" ||po.Status=="Submitted" && pod.Remarks == "Re-order level"
                                  select pod.ItemNumber).ToList();

            var toOrderForLowStock = lowStockToOrder.Except(alreadyOrdered);

            List<PurchaseOrderDetail> lowStockPODList = new List<PurchaseOrderDetail>();

            foreach (string itemNumber in toOrderForLowStock)
            {
                PurchaseOrderDetail newPOD = new PurchaseOrderDetail
                {
                    ItemNumber = itemNumber,
                    Quantity = (int)db.StationeryCatalogs.Find(itemNumber).ReorderQuantity,
                    Remarks = "Re-order level"
                };

                lowStockPODList.Add(newPOD);
            }

            var everythingToOrder = orderForOutstandingList.Concat(lowStockPODList);

            //generate a list of suppliers from whom we can order the items in the combined list

            List<string> supplierCodeList = new List<string>();

            if (everythingToOrder.Count() > 0)
            {
                foreach (PurchaseOrderDetail pod in everythingToOrder)
                {
                    string supplier = db.StationeryCatalogs.Find(pod.ItemNumber).SupplierCode1;

                    if (supplierCodeList.Count() == 0)
                    {
                        supplierCodeList.Add(supplier);
                    }

                    else if (!supplierCodeList.Contains(supplier))
                    {
                        supplierCodeList.Add(supplier);
                    }
                }

                //then for each supplier, generate a PO

                foreach (string supplier in supplierCodeList)
                {
                    var poIdList = (from po in db.PurchaseOrders.ToList()
                                    select po.PONumber).ToList();

                    PurchaseOrder newPO = new PurchaseOrder
                    {
                        PONumber = GenerateID.CreateNewId(poIdList),
                        SupplierCode = supplier,
                        DeliverTo = "Logic University",
                        Attention = "1",
                        OrderedBy = "1",
                        Status = "Not Submitted",
                        Reason = "System Generated"
                    };

                    db.PurchaseOrders.Add(newPO);
                    db.SaveChanges();

                    //insert PO Detail if the item's supplier is the current loop supplier
                    foreach (PurchaseOrderDetail pod in everythingToOrder)
                    {
                        var podIdList = (from purchaseOrderDetail in db.PurchaseOrderDetails
                                         select purchaseOrderDetail.PODetailsNumber).ToList();

                        if (db.StationeryCatalogs.Find(pod.ItemNumber).SupplierCode1 == supplier)
                        {
                            PurchaseOrderDetail newPOD = new PurchaseOrderDetail
                            {
                                PODetailsNumber = GenerateID.CreateNewId(podIdList),
                                PONumber = newPO.PONumber,
                                ItemNumber = pod.ItemNumber,
                                Quantity = pod.Quantity,
                                Remarks = pod.Remarks
                            };

                            db.PurchaseOrderDetails.Add(newPOD);
                            db.SaveChanges();
                        }

                        var findOutstandingList = (from o in outstandingLists
                                                   join fd in stationeryRetrievalFormDetails on o.RetrievalFormDetailsNumber equals fd.FormDetailsNumber into table1
                                                   from fd in table1
                                                   where fd.ItemNumber == pod.ItemNumber && o.Status == "Outstanding"
                                                   select o).ToList();

                        //Update the status of the outstanding list to "Awaiting goods" -> then no need to check for alreadyOrdered

                        foreach (OutstandingList ol in findOutstandingList)
                        {
                            OutstandingList existingOL = db.OutstandingLists.Find(ol.OutstandingListNumber);
                            existingOL.PONumber = newPO.PONumber;
                            existingOL.Status = "Awaiting Goods";

                            db.SaveChanges();
                        }
                       
                    }
                }
            }
        }

        public void MergePurchaseOrders()
        {
            List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();
            List<PurchaseOrderDetail> purchaseOrderDetails = db.PurchaseOrderDetails.ToList();

            //check if the existing unsubmitted POs have same suppliers
            var supplierList = (from po in purchaseOrders
                               where po.Status == "Not Submitted"
                               select po.SupplierCode).ToList();

            if (supplierList.GroupBy(x => x).Any(c => c.Count() > 1))
            {
                var distinctSuppliers = supplierList.GroupBy(x => x)
                    .Select(grp => grp.First())
                    .ToList();

                //create a new PO for each distinct supplier
                foreach(string supplierCode in distinctSuppliers)
                {
                    PurchaseOrder createdPurchaseOrder = new PurchaseOrder
                    {
                        PONumber = (db.PurchaseOrders.Count() + 1).ToString(),
                        SupplierCode = supplierCode,
                        DeliverTo = "Logic University",
                        Attention = "1",
                        OrderedBy = "1",
                        ApprovedBy = "1",
                        DateApproved = DateTime.Now,
                        Status = "Not Submitted"

                    };
                    db.PurchaseOrders.Add(createdPurchaseOrder);
                    db.SaveChanges();

                    //go thru list of existing unsubmitted PO and if supplier match with current loop supplier, change PO status to merged, and shift underlying PO details to the new PO
                    var poList = (from po in purchaseOrders
                                  where po.Status == "Not Submitted"
                                  select po).ToList();

                    foreach(PurchaseOrder po in poList)
                    {
                        if (po.SupplierCode.Equals(supplierCode))
                        {
                            PurchaseOrder existingPO = db.PurchaseOrders.Find(po.PONumber);
                            existingPO.Status = "Merged";

                            var podList = (from pod in purchaseOrderDetails
                                           where pod.PONumber == po.PONumber
                                           select pod).ToList();

                            foreach (PurchaseOrderDetail pod in podList)
                            {
                                PurchaseOrderDetail existingPurchaseOrderDetail = db.PurchaseOrderDetails.Find(pod.PODetailsNumber);
                                existingPurchaseOrderDetail.PONumber = createdPurchaseOrder.PONumber;
                                db.SaveChanges();
                            }

                        }
                    }
                }
            }  
        }

        // GET: PurchaseOrders/Edit/5
        public ActionResult Edit(string id, string sessionId)
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
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
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

            if (storeclerk != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                return View(purchaseJoinResult.ToList());
            }
            else if (storeManager != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                return View(purchaseJoinResult.ToList());

            }
            else if (storeSupervisor != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
                return View(purchaseJoinResult.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }

        // POST: PurchaseOrders/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "PONumber,SupplierCode,DeliverTo,Attention,SupplyByDate,OrderedBy,DateOrdered,ApprovedBy,DateApproved,ReceivedGoodsFormNo,ReceivedDate,ReceivedValue,Status")] PurchaseOrder purchaseOrder)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(purchaseOrder).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.Attention = new SelectList(db.StoreClerks, "Id", "FirstName", purchaseOrder.Attention);
        //    ViewBag.OrderedBy = new SelectList(db.StoreClerks, "Id", "FirstName", purchaseOrder.OrderedBy);
        //    ViewBag.ApprovedBy = new SelectList(db.StoreSupervisors, "Id", "FirstName", purchaseOrder.ApprovedBy);
        //    ViewBag.SupplierCode = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", purchaseOrder.SupplierCode);
        //    return View(purchaseOrder);
        //}

        // GET: PurchaseOrders/Delete/5
        public ActionResult Delete(string id,string sessionId)
        {

            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder purchaseOrder = db.PurchaseOrders.Find(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound();
            }

            if (storeclerk != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                return View(purchaseOrder);
            }
            else if (storeManager != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                return View(purchaseOrder);

            }
            else if (storeSupervisor != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
                return View(purchaseOrder);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
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

            if (storeclerk != null)
            {

                return RedirectToAction("Index", "PurchaseOrders", new { sessionId = sessionId });
            }
            else if (storeManager != null)
            {

                return RedirectToAction("Index", "PurchaseOrders",new { sessionId=sessionId});

            }
            else if (storeSupervisor != null)
            {
                return RedirectToAction("Index", "PurchaseOrders", new { sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
           
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
