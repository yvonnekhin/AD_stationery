﻿using System;
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
            //lxl-20-22
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            var stationeryCatalogs = db.StationeryCatalogs.Include(s => s.SupplierList).Include(s => s.SupplierList1).Include(s => s.SupplierList2);
            //lxl-25-48
            if (storeclerk != null && sessionId != null)
            {
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                return View(stationeryCatalogs.ToList());
            }
            else if (storeManager != null && sessionId != null)
            {
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(stationeryCatalogs.ToList());

            }
            else if (storeSupervisor != null && sessionId != null)
            {
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
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
            //lxl-57-60
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
            if (storeclerk != null && sessionId != null)
            {
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                return View(stationeryCatalog);
            }
            else if (storeManager != null && sessionId != null)
            {
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(stationeryCatalog);

            }
            else if (storeSupervisor != null && sessionId != null)
            {
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                return View(stationeryCatalog);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // GET: StationeryCatalogs/Create
        public ActionResult Create()
        {
            ViewBag.SupplierCode1 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName");
            ViewBag.SupplierCode2 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName");
            ViewBag.SupplierCode3 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName");
            return View();
        }

        // POST: StationeryCatalogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemNumber,Category,Description,ReorderLevel,ReorderQuantity,UnitOfMeasure,BinNumber,Price,SupplierCode1,SupplierCode2,SupplierCode3,Balance")] StationeryCatalog stationeryCatalog)
        {
            if (ModelState.IsValid)
            {
                db.StationeryCatalogs.Add(stationeryCatalog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SupplierCode1 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode1);
            ViewBag.SupplierCode2 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode2);
            ViewBag.SupplierCode3 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode3);
            return View(stationeryCatalog);
        }

        // GET: StationeryCatalogs/Edit/5
        public ActionResult Edit(string id, string sessionId)
        {
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
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();

            if (storeclerk != null && sessionId != null)
            {
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                return View(stationeryCatalog);
            }
            else if (storeManager != null && sessionId != null)
            {
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(stationeryCatalog);

            }
            else if (storeSupervisor != null && sessionId != null)
            {
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
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
            if (ModelState.IsValid)
            {
                db.Entry(stationeryCatalog).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
            }
            ViewBag.SupplierCode1 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode1);
            ViewBag.SupplierCode2 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode2);
            ViewBag.SupplierCode3 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode3);

            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();

            if (storeclerk != null && sessionId != null)
            {
                return RedirectToAction("Index", "StationeryCatalogs",new { @sessionId = sessionId });
            }
            else if (storeManager != null && sessionId != null)
            {
                return RedirectToAction("Index", "StationeryCatalogs",new { @sessionId = sessionId });

            }
            else if (storeSupervisor != null && sessionId != null)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StationeryCatalog stationeryCatalog = db.StationeryCatalogs.Find(id);
            if (stationeryCatalog == null)
            {
                return HttpNotFound();
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();

            if (storeclerk != null && sessionId != null)
            {
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                return View(stationeryCatalog);
            }
            else if (storeManager != null && sessionId != null)
            {
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(stationeryCatalog);

            }
            else if (storeSupervisor != null && sessionId != null)
            {
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
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
        public ActionResult DeleteConfirmed(string id)
        {
            StationeryCatalog stationeryCatalog = db.StationeryCatalogs.Find(id);
            db.StationeryCatalogs.Remove(stationeryCatalog);
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
