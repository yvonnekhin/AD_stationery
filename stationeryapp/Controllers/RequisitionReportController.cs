using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using stationeryapp.Models;

namespace stationeryapp.Controllers
{
    public class RequisitionReportController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: RequisitionReport
        public ActionResult Index(string searchString, string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();


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
            }

            var item = from i in db.StationeryCatalogs
                       join i1 in db.StationeryRetrievalFormDetails on i.ItemNumber equals i1.ItemNumber                
                       where i.ItemNumber == i1.ItemNumber
                       select new RequisitionViewModel
                       {
                           ItemNumber = i.ItemNumber,
                           Category = i.Category,
                           Description = i.Description,
                           DepartmentCode = i1.DepartmentCode,
                           Needed = (int)i1.Needed
                       };

            if (!String.IsNullOrEmpty(searchString))
            {               
                item = item.Where(i => i.DepartmentCode.Contains(searchString));
                return View(item);
            }
            else
            {                
                return View(item);
            }
                  
        }

        public ActionResult exportRequisitionReport()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RequisitionReport.rpt"));
            rd.Database.Tables[0].SetDataSource(db.StationeryCatalogs.ToList());
            rd.Database.Tables[1].SetDataSource(db.StationeryRetrievalFormDetails.ToList());           
            Response.Buffer = false;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                System.IO.Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                return File(stream, "application/ pdf", "RequisitionReport.pdf");
            }
            catch
            {
                throw;
            }
        }

        // GET: RequisitionReport/Details/5
        public ActionResult Details(string id)
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
            return View(stationeryCatalog);
        }

        // GET: RequisitionReport/Create
        public ActionResult Create()
        {
            ViewBag.SupplierCode1 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName");
            ViewBag.SupplierCode2 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName");
            ViewBag.SupplierCode3 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName");
            return View();
        }

        // POST: RequisitionReport/Create
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

        // GET: RequisitionReport/Edit/5
        public ActionResult Edit(string id)
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
            return View(stationeryCatalog);
        }

        // POST: RequisitionReport/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemNumber,Category,Description,ReorderLevel,ReorderQuantity,UnitOfMeasure,BinNumber,Price,SupplierCode1,SupplierCode2,SupplierCode3,Balance")] StationeryCatalog stationeryCatalog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stationeryCatalog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SupplierCode1 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode1);
            ViewBag.SupplierCode2 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode2);
            ViewBag.SupplierCode3 = new SelectList(db.SupplierLists, "SupplierCode", "SupplierName", stationeryCatalog.SupplierCode3);
            return View(stationeryCatalog);
        }

        // GET: RequisitionReport/Delete/5
        public ActionResult Delete(string id)
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
            return View(stationeryCatalog);
        }

        // POST: RequisitionReport/Delete/5
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
