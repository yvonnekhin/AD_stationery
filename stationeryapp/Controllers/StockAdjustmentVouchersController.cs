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
        public ActionResult Index(string sessionId)
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
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;

                return View(db.StockAdjustmentVouchers.ToList());
            }
            else if (storeManager != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(db.StockAdjustmentVouchers.ToList());
            }
            else if (storeSupervisor != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                return View(db.StockAdjustmentVouchers.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

           
        }

        // GET: StockAdjustmentVouchers/Details/5
        public ActionResult Details(string id,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
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

            if (storeclerk != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;

                return View(stockAdjustmentVoucher);
            }
            else if (storeManager != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(stockAdjustmentVoucher);
            }
            else if (storeSupervisor != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                return View(stockAdjustmentVoucher);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(StockAdjustmentVoucher sav, string Command, string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
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
                return RedirectToAction("Index", "StockAdjustmentVouchers", new { sessionId = sessionId });

            }


            if (storeclerk != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;

                return View(sav);
            }
            else if (storeManager != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(sav);
            }
            else if (storeSupervisor != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                return View(sav);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }


            
        }


        // GET: StockAdjustmentVouchers/Create
        public ActionResult Create(string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();

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

            if (storeclerk != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                return View(savd);
            }
            else if (storeManager != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(savd);
            }
            else if (storeSupervisor != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                return View(savd);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

          
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
        public ActionResult Edit(string id,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(id);
            if (stockAdjustmentVoucher == null)
            {
                return HttpNotFound();
            }

            if (storeclerk != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                return View(stockAdjustmentVoucher);
            }
            else if (storeManager != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(stockAdjustmentVoucher);
            }
            else if (storeSupervisor != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                return View(stockAdjustmentVoucher);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // POST: StockAdjustmentVouchers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdjustmentVoucherNumber,Status,Remarks,Date")] StockAdjustmentVoucher stockAdjustmentVoucher, string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (ModelState.IsValid)
            {
                db.Entry(stockAdjustmentVoucher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","StockAdjustmentVouchers", new { sessionId = sessionId });
            }

            if (storeclerk != null)
            {
                return RedirectToAction("Index", "StockAdjustmentVouchers", new { sessionId = sessionId });
            }
            else if (storeManager != null)
            {
                return RedirectToAction("Index", "StockAdjustmentVouchers", new { sessionId = sessionId });
            }
            else if (storeSupervisor != null)
            {
                return RedirectToAction("Index", "StockAdjustmentVouchers", new { sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // GET: StockAdjustmentVouchers/Delete/5
        public ActionResult Delete(string id,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(id);        

            if (stockAdjustmentVoucher == null)
            {
                return HttpNotFound();
            }

            if (storeclerk != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                return View(stockAdjustmentVoucher);
            }
            else if (storeManager != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(stockAdjustmentVoucher);
            }
            else if (storeSupervisor != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                return View(stockAdjustmentVoucher);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // POST: StockAdjustmentVouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
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
            if (storeclerk != null)
            {
                return RedirectToAction("Index", "StockAdjustmentVouchers",new { sessionId=sessionId});
            }
            else if (storeManager != null)
            {

                return RedirectToAction("Index", "StockAdjustmentVouchers", new { sessionId = sessionId });
            }
            else if (storeSupervisor != null)
            {
                return RedirectToAction("Index", "StockAdjustmentVouchers", new { sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
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
