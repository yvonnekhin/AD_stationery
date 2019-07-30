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
    public class StationeryCatalogReportController : Controller
    {
        private StationeryCatalogDB db = new StationeryCatalogDB();

        // GET: StationeryCatalogReport
        public ActionResult Index()
        {
            return View(db.StationeryCatalogs.ToList());
        }

        // GET: StationeryCatalogReport/Details/5
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

        // GET: StationeryCatalogReport/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StationeryCatalogReport/Create
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

            return View(stationeryCatalog);
        }

        // GET: StationeryCatalogReport/Edit/5
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
            return View(stationeryCatalog);
        }

        // POST: StationeryCatalogReport/Edit/5
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
            return View(stationeryCatalog);
        }

        // GET: StationeryCatalogReport/Delete/5
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

        // POST: StationeryCatalogReport/Delete/5
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
