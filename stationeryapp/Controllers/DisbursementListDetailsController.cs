﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using stationeryapp.Models;

namespace stationeryapp.Controllers
{
    public class DisbursementListDetailsController : Controller
    {
        private ModelDBContext db = new ModelDBContext();
        private StockAdjustmentVouchersDBContext adb = new StockAdjustmentVouchersDBContext();
        public ActionResult Index()
        {

            var disbursementListDetails = db.DisbursementListDetails.Include(d => d.DisbursementList).Include(d => d.StationeryCatalog);
            return View(disbursementListDetails.ToList());
        }


        public ActionResult Details(string id)
        {

            var disbursementListDetail = db.DisbursementListDetails.Include(d => d.DisbursementList).Include(d => d.StationeryCatalog)
                                         .Where(DisbursementListDetails => DisbursementListDetails.ListNumber == id)
                                         .ToList();
            //List<List<T>>
            DisbursementList disbursementList = db.DisbursementLists.Where(d => d.ListNumber == id).Single();
            DepartmentList departmentList = db.DepartmentLists.Where(d => d.DepartmentCode == disbursementList.DepartmentCode).Single();
            CollectionPoint collectionPoint = db.CollectionPoints.Where(c => c.CollectionPointCode == departmentList.CollectionPoint).Single();
            Employee employee = db.Employees.Where(e => e.Id == departmentList.RepresentativeId).Single();

            ViewData["collection"] = collectionPoint.CollectionPointName;
            ViewData["disbursementList"] = disbursementList.Date;
            ViewData["deparment"] = departmentList.DepartmentName;
            ViewData["employeeF"] = employee.FirstName;
            ViewData["employeeL"] = employee.LastName;
            return View(disbursementListDetail);
        }

        [HttpPost]
        public ActionResult Update(List<DisbursementListDetail> Details)
        {
            int newNumer = db.StockAdjustmentVouchers.Count();
            int length = Details.Count() + newNumer+1;
            int newNumer2 = db.StockAdjustmentVoucherDetails.Count()+1;
            bool flag = false;
            int countnotshow = 0;
            int recivedX;

            for (int i=newNumer+1; i< length; i++ )
            {
                DisbursementListDetail existing = db.DisbursementListDetails.Find(Details[i - newNumer-1].ListDetailsNumber);
                existing.QuantityReceived = Details[i - newNumer-1].QuantityReceived;
                existing.Remarks = Details[i - newNumer-1].Remarks;
                recivedX = Convert.ToInt32(existing.Quantity)- Convert.ToInt32(Details[i - newNumer - 1].QuantityReceived);
                if (recivedX > 0)
                {
                    flag = true;

                }
                if (Convert.ToInt32(Details[i - newNumer - 1].QuantityReceived)==0)
                {
                    countnotshow++;
                }
                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();

            }
            if (flag)
            {
                StockAdjustmentVoucher stockAdjustment = new StockAdjustmentVoucher();
                stockAdjustment.AdjustmentVoucherNumber = Convert.ToString(newNumer+1);
                stockAdjustment.Remarks ="should not be mine";
                stockAdjustment.Status = "pending";            
                adb.StockAdjustmentVouchers.Add(stockAdjustment);
                adb.SaveChanges();

            }
            for (int i = newNumer + 1; i < length; i++)
            {

                DisbursementListDetail existing = db.DisbursementListDetails.Find(Details[i - newNumer - 1].ListDetailsNumber);
                existing.QuantityReceived = Details[i - newNumer - 1].QuantityReceived;
                existing.Remarks = Details[i - newNumer - 1].Remarks;
                recivedX = Convert.ToInt32(existing.Quantity) - Convert.ToInt32(Details[i - newNumer - 1].QuantityReceived);
                if (recivedX > 0)
                {                
                    StockAdjustmentVoucherDetail adjustmentVoucherDetail = new StockAdjustmentVoucherDetail();
                    adjustmentVoucherDetail.AdjustmentVoucherNumber = (newNumer + 1).ToString();
                    adjustmentVoucherDetail.AdjustmentDetailsNumber= Convert.ToString(newNumer2);
                    newNumer2++;
                    adjustmentVoucherDetail.QuantityAdjusted = recivedX;
                    adjustmentVoucherDetail.Reason = Details[i - newNumer - 1].Remarks;

                    db.StockAdjustmentVoucherDetails.Add(adjustmentVoucherDetail);
                    db.SaveChanges();
                }
            }
            //update DisbursementList status
            if (countnotshow== Details.Count())
            {
                string id = db.DisbursementListDetails.Find(Details[0].ListDetailsNumber).ListNumber;
                DisbursementList disbursementList = db.DisbursementLists.Find(id);
                disbursementList.Status = "Cancelled";
                db.Entry(disbursementList).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                string id = db.DisbursementListDetails.Find(Details[0].ListDetailsNumber).ListNumber;
                DisbursementList disbursementList = db.DisbursementLists.Find(id);
                disbursementList.Status = "Collected";
                db.Entry(disbursementList).State = EntityState.Modified;
                db.SaveChanges();
            }
           
            return RedirectToAction("Index", "DisbursementListDetails");
        }






        // GET: DisbursementListDetails/Create
        public ActionResult Create()
        {
            ViewBag.ListNumber = new SelectList(db.DisbursementLists, "ListNumber", "DepartmentCode");
            ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category");
            return View();
        }

        // POST: DisbursementListDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ListDetailsNumber,ListNumber,ItemNumber,Quantity,QuantityReceived,Remarks")] DisbursementListDetail disbursementListDetail)
        {
            if (ModelState.IsValid)
            {
                db.DisbursementListDetails.Add(disbursementListDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ListNumber = new SelectList(db.DisbursementLists, "ListNumber", "DepartmentCode", disbursementListDetail.ListNumber);
            ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category", disbursementListDetail.ItemNumber);
            return View(disbursementListDetail);
        }

        // GET: DisbursementListDetails/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisbursementListDetail disbursementListDetail = db.DisbursementListDetails.Find(id);
            if (disbursementListDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ListNumber = new SelectList(db.DisbursementLists, "ListNumber", "DepartmentCode", disbursementListDetail.ListNumber);
            ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category", disbursementListDetail.ItemNumber);
            return View(disbursementListDetail);
        }

        // POST: DisbursementListDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ListDetailsNumber,ListNumber,ItemNumber,Quantity,QuantityReceived,Remarks")] DisbursementListDetail disbursementListDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(disbursementListDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ListNumber = new SelectList(db.DisbursementLists, "ListNumber", "DepartmentCode", disbursementListDetail.ListNumber);
            ViewBag.ItemNumber = new SelectList(db.StationeryCatalogs, "ItemNumber", "Category", disbursementListDetail.ItemNumber);
            return View(disbursementListDetail);
        }

        // GET: DisbursementListDetails/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisbursementListDetail disbursementListDetail = db.DisbursementListDetails.Find(id);
            if (disbursementListDetail == null)
            {
                return HttpNotFound();
            }
            return View(disbursementListDetail);
        }

        // POST: DisbursementListDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DisbursementListDetail disbursementListDetail = db.DisbursementListDetails.Find(id);
            db.DisbursementListDetails.Remove(disbursementListDetail);
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