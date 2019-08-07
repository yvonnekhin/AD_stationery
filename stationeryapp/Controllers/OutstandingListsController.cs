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
    public class OutstandingListsController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: OutstandingLists
        public ActionResult Index()
        {
            List<OutstandingList> outstandingLists = db.OutstandingLists.ToList();
            List<StationeryCatalog> catalogs = db.StationeryCatalogs.ToList();
            List<StationeryRetrievalFormDetail> retrievalFormDetails = db.StationeryRetrievalFormDetails.ToList();
            List<PurchaseOrderDetail> purchaseOrderDetails = db.PurchaseOrderDetails.ToList();
            List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();

            var outstandingListRecord = (from o in outstandingLists                                         
                                         //join r in retrievalFormDetails on o.ItemNumber equals r.ItemNumber into table1
                                         //from r in table1.ToList()
                                         //join c in catalogs on o.ItemNumber equals c.ItemNumber into table2
                                         //from c in table2.ToList()
                                         //join pd in purchaseOrderDetails on o.ItemNumber equals pd.ItemNumber into table3
                                         //from pd in table3.ToList()
                                         //join p in purchaseOrders on pd.PONumber equals p.PONumber into table4
                                         //from p in table4.ToList()
                                         select new ViewModelOutstanding
                                         {
                                             //outstandingLists = o,
                                             //retrievalFormDetails = r,
                                             //catalogs = c,
                                             //purchaseOrderDetails = pd,
                                             //purchaseOrders = p
                                         });

            return View(outstandingListRecord.ToList());
            
            //var outstandingLists = db.OutstandingLists.Include(o => o.DepartmentList).Include(o => o.RequisitionForm).Include(o => o.StationeryCatalog).Include(o => o.PurchaseOrder);
            //return View(outstandingLists.ToList());
        }

        // GET: OutstandingLists/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OutstandingList outstandingList = db.OutstandingLists.Find(id);
            if (outstandingList == null)
            {
                return HttpNotFound();
            }
            return View(outstandingList);
        }

        // GET: OutstandingLists/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentCode = new SelectList(db.DepartmentLists, "DepartmentCode", "DepartmentName");
            ViewBag.RequisitionFormNumber = new SelectList(db.RequisitionForms, "FormNumber", "EmployeeId");
            ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category");
            ViewBag.PONumber = new SelectList(db.PurchaseOrders, "PONumber", "SupplierCode");
            return View();
        }

        // POST: OutstandingLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OutstandingListNumber,RequisitionFormNumber,DateApproved,ItemNumber,Balance,DepartmentCode,Needed,PONumber,Status")] OutstandingList outstandingList)
        {
            if (ModelState.IsValid)
            {
                db.OutstandingLists.Add(outstandingList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.DepartmentCode = new SelectList(db.DepartmentLists, "DepartmentCode", "DepartmentName", outstandingList.DepartmentCode);
            //ViewBag.RequisitionFormNumber = new SelectList(db.RequisitionForms, "FormNumber", "EmployeeId", outstandingList.RequisitionFormNumber);
            //ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category", outstandingList.ItemNumber);
            ViewBag.PONumber = new SelectList(db.PurchaseOrders, "PONumber", "SupplierCode", outstandingList.PONumber);
            return View(outstandingList);
        }

        // GET: OutstandingLists/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OutstandingList outstandingList = db.OutstandingLists.Find(id);
            if (outstandingList == null)
            {
                return HttpNotFound();
            }
            //ViewBag.DepartmentCode = new SelectList(db.DepartmentLists, "DepartmentCode", "DepartmentName", outstandingList.DepartmentCode);
            //ViewBag.RequisitionFormNumber = new SelectList(db.RequisitionForms, "FormNumber", "EmployeeId", outstandingList.RequisitionFormNumber);
            //ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category", outstandingList.ItemNumber);
            ViewBag.PONumber = new SelectList(db.PurchaseOrders, "PONumber", "SupplierCode", outstandingList.PONumber);
            return View(outstandingList);
        }

        // POST: OutstandingLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OutstandingListNumber,RequisitionFormNumber,DateApproved,ItemNumber,Balance,DepartmentCode,Needed,PONumber,Status")] OutstandingList outstandingList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(outstandingList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.DepartmentCode = new SelectList(db.DepartmentLists, "DepartmentCode", "DepartmentName", outstandingList.DepartmentCode);
            //ViewBag.RequisitionFormNumber = new SelectList(db.RequisitionForms, "FormNumber", "EmployeeId", outstandingList.RequisitionFormNumber);
            //ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category", outstandingList.ItemNumber);
            ViewBag.PONumber = new SelectList(db.PurchaseOrders, "PONumber", "SupplierCode", outstandingList.PONumber);
            return View(outstandingList);
        }

        // GET: OutstandingLists/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OutstandingList outstandingList = db.OutstandingLists.Find(id);
            if (outstandingList == null)
            {
                return HttpNotFound();
            }
            return View(outstandingList);
        }

        // POST: OutstandingLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            OutstandingList outstandingList = db.OutstandingLists.Find(id);
            db.OutstandingLists.Remove(outstandingList);
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