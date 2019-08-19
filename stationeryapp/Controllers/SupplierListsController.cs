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
    public class SupplierListsController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: SupplierLists
        public ActionResult Index(string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();        
            if (storeManager != null)
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
                return View(db.SupplierLists.ToList());
            }           
            else
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }

            
        }

        // GET: SupplierLists/Details/5
        public ActionResult Details(string id,string sessionId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (sessionId == null)
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            SupplierList supplierList = db.SupplierLists.Find(id);
            if (supplierList == null)
            {
                return HttpNotFound();
            }

           if (storeManager != null)
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
                return View(supplierList);
            }          
            else
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }
            
        }

        // GET: SupplierLists/Create
        public ActionResult Create(string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }        
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (storeManager != null)
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
            else
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }
           
        }

        // POST: SupplierLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SupplierCode,SupplierName,GSTNo,ContactName,PhoneNo,FaxNo,Address")] SupplierList supplierList, string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (storeManager != null&& ModelState.IsValid)
            {
                    db.SupplierLists.Add(supplierList);
                    db.SaveChanges();
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
                return RedirectToAction("Index", "SupplierLists", new { sessionId=sessionId});           
            }
            else
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }
     
        }

        // GET: SupplierLists/Edit/5
        public ActionResult Edit(string id,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupplierList supplierList = db.SupplierLists.Find(id);
            if (supplierList == null)
            {
                return HttpNotFound();
            }

            if (storeManager != null)
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
                return View(supplierList);
            }
            else
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }
           
        }

        // POST: SupplierLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SupplierCode,SupplierName,GSTNo,ContactName,PhoneNo,FaxNo,Address")] SupplierList supplierList,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (storeManager != null && ModelState.IsValid)
            {
                db.Entry(supplierList).State = EntityState.Modified;
                db.SaveChanges();
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
                return RedirectToAction("Index", "SupplierLists", new { sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }

        }

        // GET: SupplierLists/Delete/5
        public ActionResult Delete(string id,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupplierList supplierList = db.SupplierLists.Find(id);
            if (supplierList == null)
            {
                return HttpNotFound();
            }
            if (storeManager != null)
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
                return View(supplierList);
            }
            else
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }

            
        }

        // POST: SupplierLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("LoginStoreManager", "Login");
            }
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            SupplierList supplierList = db.SupplierLists.Find(id);

            if (storeManager != null && ModelState.IsValid)
            {
                db.SupplierLists.Remove(supplierList);
                db.SaveChanges();
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
                return RedirectToAction("Index", "SupplierLists", new { sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("LoginStoreManager", "Login");
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
