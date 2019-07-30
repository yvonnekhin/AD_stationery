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
    public class RequisitionFormDetailsController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: RequisitionFormDetails
        public ActionResult Index()
        {
            var requisitionFormDetails = db.RequisitionFormDetails.Include(r => r.RequisitionForm).Include(r => r.StationeryCatalog);
            return View(requisitionFormDetails.ToList());
        }

        // GET: RequisitionFormDetails/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequisitionFormDetail requisitionFormDetail = db.RequisitionFormDetails.Find(id);
            if (requisitionFormDetail == null)
            {
                return HttpNotFound();
            }
            return View(requisitionFormDetail);
        }

        // GET: RequisitionFormDetails/Create
        public ActionResult Create()
        {
            ViewBag.FormNumber = new SelectList(db.RequisitionForms, "FormNumber", "EmployeeId");
            ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category");
            return View();
        }

        // POST: RequisitionFormDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FormDetailsNumber,FormNumber,ItemNumber,Quantity")] RequisitionFormDetail requisitionFormDetail)
        {
            if (ModelState.IsValid)
            {
                db.RequisitionFormDetails.Add(requisitionFormDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FormNumber = new SelectList(db.RequisitionForms, "FormNumber", "EmployeeId", requisitionFormDetail.FormNumber);
            ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category", requisitionFormDetail.ItemNumber);
            return View(requisitionFormDetail);
        }

        // GET: RequisitionFormDetails/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequisitionFormDetail requisitionFormDetail = db.RequisitionFormDetails.Find(id);
            if (requisitionFormDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.FormNumber = new SelectList(db.RequisitionForms, "FormNumber", "EmployeeId", requisitionFormDetail.FormNumber);
            ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category", requisitionFormDetail.ItemNumber);
            return View(requisitionFormDetail);
        }

        // POST: RequisitionFormDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FormDetailsNumber,FormNumber,ItemNumber,Quantity")] RequisitionFormDetail requisitionFormDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requisitionFormDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FormNumber = new SelectList(db.RequisitionForms, "FormNumber", "EmployeeId", requisitionFormDetail.FormNumber);
            ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category", requisitionFormDetail.ItemNumber);
            return View(requisitionFormDetail);
        }

        // GET: RequisitionFormDetails/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequisitionFormDetail requisitionFormDetail = db.RequisitionFormDetails.Find(id);
            if (requisitionFormDetail == null)
            {
                return HttpNotFound();
            }
            return View(requisitionFormDetail);
        }

        // POST: RequisitionFormDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            RequisitionFormDetail requisitionFormDetail = db.RequisitionFormDetails.Find(id);
            db.RequisitionFormDetails.Remove(requisitionFormDetail);
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
