using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using stationeryapp.Models;

namespace stationeryapp.Controllers
{
    public class StockAdjustmentVouchersController : Controller
    {
        private ModelDBContext db = new ModelDBContext();
        
        // GET: StockAdjustmentVouchers
        public ActionResult Index()
        {
            return View(db.StockAdjustmentVouchers.ToList());
        }

        // GET: StockAdjustmentVouchers/Details/5
        public ActionResult Details(string id)
        {
            List<StockAdjustmentVoucherDetail> savdList = db.StockAdjustmentVoucherDetails.ToList();

            var stockAdjustmentDetails = (from savd in savdList
                                          where savd.AdjustmentVoucherNumber == id
                                          select savd).ToList();

            ViewBag.savd = stockAdjustmentDetails;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(id);
            if (stockAdjustmentVoucher == null)
            {
                return HttpNotFound();
            }

            return View(stockAdjustmentVoucher);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(StockAdjustmentVoucher sav, string Command)
        {
            if (ModelState.IsValid)
            {
                if (Command == "Reject")
                {
                    sav.Status = "Rejected";
                }

                else if (Command == "Approve")
                {
                    sav.Status = "Approved";
                }
                StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(sav.AdjustmentVoucherNumber);
                stockAdjustmentVoucher.Status = sav.Status;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sav);
        }


        // GET: StockAdjustmentVouchers/Create
        public ActionResult Create()
        {
            StockAdjustmentVoucher stockAdjustmentVoucher = new StockAdjustmentVoucher();
            stockAdjustmentVoucher.AdjustmentVoucherNumber = (db.StockAdjustmentVouchers.Count() + 1).ToString();
            stockAdjustmentVoucher.Status = "Pending";
            stockAdjustmentVoucher.Date = DateTime.Today;
            stockAdjustmentVoucher.Remarks = "Store clerk";

            //db.StockAdjustmentVouchers.Add(stockAdjustmentVoucher);
            //db.SaveChanges();

            ViewBag.voucher = stockAdjustmentVoucher;

            StockAdjustmentVoucherDetail savd = new StockAdjustmentVoucherDetail();
            savd.AdjustmentVoucherNumber = stockAdjustmentVoucher.AdjustmentVoucherNumber;
            savd.AdjustmentDetailsNumber = (db.StockAdjustmentVoucherDetails.Count() + 1);

            return View(savd);
        }


        public JsonResult SaveData(string passdata, string vdata)
        {
            try
            {
                var serializeData = JsonConvert.DeserializeObject<List<StockAdjustmentVoucherDetail>>(passdata);
                var serializeVoucherData = JsonConvert.DeserializeObject<List<StockAdjustmentVoucher>>(vdata);

                foreach (var data in serializeVoucherData)
                {
                    db.StockAdjustmentVouchers.Add(data);
                }

                foreach (var data in serializeData)
                {
                    db.StockAdjustmentVoucherDetails.Add(data);
                }

                db.SaveChanges();
            }

            catch (Exception)
            {
                return Json("fail");
            }

            return Json("success");
        }

        // GET: StockAdjustmentVouchers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(id);
            if (stockAdjustmentVoucher == null)
            {
                return HttpNotFound();
            }
            return View(stockAdjustmentVoucher);
        }

        // POST: StockAdjustmentVouchers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdjustmentVoucherNumber,Status,Remarks,Date")] StockAdjustmentVoucher stockAdjustmentVoucher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockAdjustmentVoucher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockAdjustmentVoucher);
        }

        // GET: StockAdjustmentVouchers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(id);        

            if (stockAdjustmentVoucher == null)
            {
                return HttpNotFound();
            }
            return View(stockAdjustmentVoucher);
        }

        // POST: StockAdjustmentVouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(id);

            List<StockAdjustmentVoucherDetail> savd = db.StockAdjustmentVoucherDetails.ToList();

            var toDelete = (from stockAdjustmentVoucherDetail in savd
                               where stockAdjustmentVoucher.AdjustmentVoucherNumber == id
                               select stockAdjustmentVoucherDetail).ToList();

            foreach(StockAdjustmentVoucherDetail stockAdjustmentVoucherDetail in toDelete)
            {
                db.StockAdjustmentVoucherDetails.Remove(stockAdjustmentVoucherDetail);
            }

            db.StockAdjustmentVouchers.Remove(stockAdjustmentVoucher);
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
