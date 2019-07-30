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
    public class ReorderReportController : Controller
    {
        private StationeryCatalogDB db = new StationeryCatalogDB();
        private ModelDBContext modeldb = new ModelDBContext();
   

        // GET: ReorderReport
        public ActionResult Index(string sortOrder, string searchString)
        {
            var item = from i in modeldb.StationeryCatalogs
                       join i1 in modeldb.PurchaseOrderDetails on i.ItemNumber equals i1.ItemNumber
                       join i2 in modeldb.PurchaseOrders on i1.PONumber equals i2.PONumber
                       where i.ItemNumber == i1.ItemNumber
                       select new ReorderViewModel
                       {
                           ItemNumber = i.ItemNumber,
                           Category = i.Category,
                           Description = i.Description,
                           Balance = i.Balance,
                           ReorderLevel = i.ReorderLevel,
                           ReorderQuantity = i.ReorderQuantity,
                           PONumber = i1.PONumber,
                           SupplyByDate = i2.SupplyByDate.Value,
                       };
                      
            if (!String.IsNullOrEmpty(searchString))
            {
                item = item.Where(i => i.Category.Contains(searchString));
                return View(item);
            }
            else
                return View(item);

            //non-working code
            //var item = from i1 in modeldb.StationeryCatalogs
            //           join i2 in modeldb.PurchaseOrderDetails on i1.ItemNumber equals i2.ItemNumber
            //           join i3 in modeldb.PurchaseOrders on i2.PONumber equals i3.PONumber
            //           where i1.ItemNumber == i2.ItemNumber
            //           select new 
            //                 {
            //                     col1 = i1.ItemNumber,
            //                     col2 = i1.Category,
            //                     col3 = i1.Description,
            //                     col4 = i1.Balance,
            //                     col5 = i1.ReorderLevel,
            //                     col6 = i1.ReorderQuantity,
            //                     col7 = i2.PONumber,
            //                     col8 = i3.SupplyByDate
            //                 };
            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    item = item.Where(i1 => i1.col2.Contains(searchString));
            //    //ViewData["list"] = item.ToList();
            //    //return View();
            //    //return View(stationery);
            //    return View(item.ToList());
            //}

            //else
            //    return View(modeldb.StationeryCatalogs.ToList());

            //Working code
            //var item = from i in db.StationeryCatalogs
            //           select i;
            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    item = item.Where(i => i.Category.Contains(searchString));
            //    return View(item.ToList());
            //}
            //else
            //    return View(db.StationeryCatalogs.ToList());
        }

        // GET: ReorderReport/Details/5
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

        // GET: ReorderReport/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReorderReport/Create
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

        // GET: ReorderReport/Edit/5
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

        // POST: ReorderReport/Edit/5
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

        // GET: ReorderReport/Delete/5
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

        // POST: ReorderReport/Delete/5
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
