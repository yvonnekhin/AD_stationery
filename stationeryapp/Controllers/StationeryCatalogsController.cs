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
    public class StationeryCatalogsController : Controller
    {
        private ModelDBContext db = new ModelDBContext();
        // GET: StationeryCatalogs
        public ActionResult Index(string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            var stationeryCatalogs = db.StationeryCatalogs.Include(s => s.SupplierList).Include(s => s.SupplierList1).Include(s => s.SupplierList2);
            //lxl-25-48
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
                return View(stationeryCatalogs.ToList());
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
                return View(stationeryCatalogs.ToList());

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
                return View(stationeryCatalogs.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
           
        }

        // GET: StationeryCatalogs/Details/5
        public ActionResult Details(string id, string sessionId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            List<StockAdjustmentVoucher> stockAdjustmentVouchers = db.StockAdjustmentVouchers.ToList();
            List<StockAdjustmentVoucherDetail> stockAdjustmentVoucherDetails = db.StockAdjustmentVoucherDetails.ToList();

            var adjustJoinResult = (from savd in stockAdjustmentVoucherDetails
                                    join sav in stockAdjustmentVouchers on savd.AdjustmentVoucherNumber equals sav.AdjustmentVoucherNumber into table1
                                    from sav in table1.ToList()
                                    select new RetrievalHistory
                                    {
                                        date = sav.Date,
                                        name = ("Stock Adjustment" + " " + sav.AdjustmentVoucherNumber),
                                        quantity = (int)savd.QuantityAdjusted,
                                        stockAdjustmentVoucherDetail = savd
                                    }).Where(x => x.stockAdjustmentVoucherDetail.ItemNumber == id && x.stockAdjustmentVoucherDetail.StockAdjustmentVoucher.Status == "Approved");

            List<StationeryRetrievalFormDetail> stationeryRetrievalFormDetails = db.StationeryRetrievalFormDetails.ToList();
            List<StationeryRetrievalForm> stationeryRetrievalForms = db.StationeryRetrievalForms.ToList();
            List<DepartmentList> departmentLists = db.DepartmentLists.ToList();

            var retrievalJoinResult = (from srf in stationeryRetrievalForms
                                       join srfd in stationeryRetrievalFormDetails on srf.FormNumber equals srfd.FormNumber into table1
                                       from srfd in table1.ToList()
                                       join d in departmentLists on srfd.DepartmentCode equals d.DepartmentCode into table2
                                       from d in table2.ToList()
                                       select new RetrievalHistory
                                       {
                                           date = srf.Date,
                                           name = d.DepartmentName,
                                           quantity = (int)srfd.Actual * -1,
                                           stationeryRetrievalFormDetail = srfd

                                       }).Where(x => x.stationeryRetrievalFormDetail.ItemNumber == id);

            List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();
            List<PurchaseOrderDetail> purchaseOrderDetails = db.PurchaseOrderDetails.ToList();
            List<SupplierList> suppliers = db.SupplierLists.ToList();

            var purchaseJoinResult = (from pod in purchaseOrderDetails
                                      join p in purchaseOrders on pod.PONumber equals p.PONumber into table1
                                      from p in table1.ToList()
                                      join s in suppliers on p.SupplierCode equals s.SupplierCode into table2
                                      from s in table2.ToList()
                                      select new RetrievalHistory
                                      { // result selector 
                                          date = p.ReceivedDate,
                                          name = s.SupplierName,
                                          quantity = (int)pod.Quantity,
                                          purchaseOrderDetail = pod
                                      }).Where(x => x.purchaseOrderDetail.ItemNumber == id);

            var unionRetrievalPurchase = retrievalJoinResult.Concat(purchaseJoinResult);

            var unionRetrievalPurchaseAdjust = unionRetrievalPurchase.Concat(adjustJoinResult);

            var sortedResult = from u in unionRetrievalPurchaseAdjust
                               orderby u.date
                               select u;

            ViewBag.history = sortedResult;

            StationeryCatalog stationeryCatalog = db.StationeryCatalogs.Find(id);
            if (stationeryCatalog == null && sessionId != null)
            {
                return HttpNotFound();
            }
            //lxl-126-149
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
                return View(stationeryCatalog);
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
                return View(stationeryCatalog);

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
                return View(stationeryCatalog);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // GET: StationeryCatalogs/Create
        public ActionResult Create(string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            ViewBag.SupplierCode1 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName");
            ViewBag.SupplierCode2 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName");
            ViewBag.SupplierCode3 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName");
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
                return View();
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
                return View();
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
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }         
        }

        // POST: StationeryCatalogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemNumber,Category,Description,ReorderLevel,ReorderQuantity,UnitOfMeasure,BinNumber,Price,SupplierCode1,SupplierCode2,SupplierCode3,Balance")] StationeryCatalog stationeryCatalog,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();    
            ViewBag.SupplierCode1 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode1);
            ViewBag.SupplierCode2 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode2);
            ViewBag.SupplierCode3 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode3);
            if (storeclerk != null)
            {
                if (ModelState.IsValid)
                {
                    db.StationeryCatalogs.Add(stationeryCatalog);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "StationeryCatalogs", new { sessionId = sessionId });
            }
            else if (storeManager != null)
            {
                if (ModelState.IsValid)
                {
                    db.StationeryCatalogs.Add(stationeryCatalog);
                    db.SaveChanges();                   
                }
                return RedirectToAction("Index", "StationeryCatalogs", new { sessionId=sessionId});
            }
            else if (storeSupervisor != null)
            {
                if (ModelState.IsValid)
                {
                    db.StationeryCatalogs.Add(stationeryCatalog);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "StationeryCatalogs", new { sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }          
        }

        // GET: StationeryCatalogs/Edit/5
        public ActionResult Edit(string id, string sessionId)
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
            StationeryCatalog stationeryCatalog = db.StationeryCatalogs.Find(id);
            if (stationeryCatalog == null)
            {
                return HttpNotFound();
            }
            ViewBag.SupplierCode1 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode1);
            ViewBag.SupplierCode2 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode2);
            ViewBag.SupplierCode3 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode3);     

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
                return View(stationeryCatalog);
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
                return View(stationeryCatalog);

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
                return View(stationeryCatalog);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: StationeryCatalogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemNumber,Category,Description,ReorderLevel,ReorderQuantity,UnitOfMeasure,BinNumber,Price,SupplierCode1,SupplierCode2,SupplierCode3,Balance")] StationeryCatalog stationeryCatalog, string sessionId)
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
                db.Entry(stationeryCatalog).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
            }
            ViewBag.SupplierCode1 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode1);
            ViewBag.SupplierCode2 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode2);
            ViewBag.SupplierCode3 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode3);

            if (storeclerk != null)
            {
                return RedirectToAction("Index", "StationeryCatalogs",new { @sessionId = sessionId });
            }
            else if (storeManager != null)
            {
                return RedirectToAction("Index", "StationeryCatalogs",new { @sessionId = sessionId });

            }
            else if (storeSupervisor != null)
            {
                return RedirectToAction("Index", "StationeryCatalogs", new { @sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // GET: StationeryCatalogs/Delete/5
        public ActionResult Delete(string id, string sessionId)
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
            StationeryCatalog stationeryCatalog = db.StationeryCatalogs.Find(id);
            if (stationeryCatalog == null)
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
                return View(stationeryCatalog);
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
                return View(stationeryCatalog);

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
                return View(stationeryCatalog);
            }
            else
            {
  
                return RedirectToAction("Login", "Login");
            }

            
        }

        // POST: StationeryCatalogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StationeryCatalog stationeryCatalog = db.StationeryCatalogs.Find(id);
            db.StationeryCatalogs.Remove(stationeryCatalog);
            db.SaveChanges();
            if (storeclerk != null)
            {
                return RedirectToAction("Index", "StationeryCatalogs", new { sessionId = sessionId });
            }
            else if (storeManager != null)
            {
                return RedirectToAction("Index", "StationeryCatalogs",new { sessionId=sessionId});

            }
            else if (storeSupervisor != null)
            {

                return RedirectToAction("Index", "StationeryCatalogs", new { sessionId = sessionId });
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
