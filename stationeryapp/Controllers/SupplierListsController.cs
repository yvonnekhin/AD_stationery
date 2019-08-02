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
        public ActionResult Index()
        {
            return View(db.SupplierLists.ToList());
        }

        // GET: SupplierLists/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupplierList supplierList = db.SupplierLists.Find(id);
            if (supplierList == null)
            {
                return HttpNotFound();
            }
            return View(supplierList);
        }

        // GET: SupplierLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SupplierLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SupplierCode,SupplierName,GSTNo,ContactName,PhoneNo,FaxNo,Address")] SupplierList supplierList)
        {
            if (ModelState.IsValid)
            {
                db.SupplierLists.Add(supplierList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(supplierList);
        }

        // GET: SupplierLists/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupplierList supplierList = db.SupplierLists.Find(id);
            if (supplierList == null)
            {
                return HttpNotFound();
            }
            return View(supplierList);
        }

        // POST: SupplierLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SupplierCode,SupplierName,GSTNo,ContactName,PhoneNo,FaxNo,Address")] SupplierList supplierList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplierList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplierList);
        }

        // GET: SupplierLists/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupplierList supplierList = db.SupplierLists.Find(id);
            if (supplierList == null)
            {
                return HttpNotFound();
            }
            return View(supplierList);
        }

        // POST: SupplierLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            SupplierList supplierList = db.SupplierLists.Find(id);
            db.SupplierLists.Remove(supplierList);
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
