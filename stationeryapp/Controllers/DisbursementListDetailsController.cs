using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using stationeryapp.Models;

namespace stationeryapp.Controllers
{
    public class DisbursementListDetailsController : Controller
    {
        private ModelDBContext db = new ModelDBContext();
        private ModelDBContext adb = new ModelDBContext();
        public JsonResult GetDetails(string id)
        {
            var disbursementListDetail = db.DisbursementListDetails.Include(d => d.DisbursementList).Include(d => d.StationeryCatalog)
                                        .Where(DisbursementListDetails => DisbursementListDetails.ListNumber == id)
                                        .ToList();
            DisbursementList disbursementList = db.DisbursementLists.Where(d => d.ListNumber == id).Single();
            DepartmentList departmentList = db.DepartmentLists.Where(d => d.DepartmentCode == disbursementList.DepartmentCode).Single();
            CollectionPoint collectionPoint = db.CollectionPoints.Where(c => c.CollectionPointCode == departmentList.CollectionPoint).Single();
            Employee employee = db.Employees.Where(e => e.Id == departmentList.RepresentativeId).Single();

            return Json(new { data = new { detailId = disbursementListDetail.Select(x => x.ListDetailsNumber), listId = disbursementListDetail.Select(x => x.DisbursementList.ListNumber), ItemNumber = disbursementListDetail.Select(x => x.ItemNumber), Category = disbursementListDetail.Select(x => x.StationeryCatalog.Category), desc = disbursementListDetail.Select(x => x.StationeryCatalog.Description), quantity = disbursementListDetail.Select(x => x.Quantity), received = disbursementListDetail.Select(x => x.QuantityReceived), remark = disbursementListDetail.Select(x => x.Remarks) } }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult PostDetails(Detail Details)
        {
            List<Infos> bb = JsonConvert.DeserializeObject<List<Infos>>(Details.Infos1[0]);
            DisbursementList disbursementList = db.DisbursementLists.Find(bb[0].ListId);
            int newNumer = db.StockAdjustmentVouchers.Count();
            int length = bb.Count() + newNumer + 1;
            int newNumer2 = db.StockAdjustmentVoucherDetails.Count() + 1;
            bool flag = false;
            int countnotshow = 0;
            int recivedX;

            foreach (Infos item in bb)
            {
                DisbursementListDetail existing = db.DisbursementListDetails.Find(item.DetailId);
                existing.QuantityReceived = Convert.ToInt32(item.Actual);
                existing.Remarks = item.Remark;
                recivedX = Convert.ToInt32(existing.Quantity) - Convert.ToInt32(item.Actual);
                if (recivedX > 0)
                {
                    flag = true;

                }
                if (Convert.ToInt32(item.Actual) == 0)
                {
                    countnotshow++;
                }
                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (flag)
            {
                StockAdjustmentVoucher stockAdjustment = new StockAdjustmentVoucher();
                stockAdjustment.AdjustmentVoucherNumber = Convert.ToString(newNumer + 1);
                stockAdjustment.Status = "Pending";
                stockAdjustment.Date = DateTime.Now;
                stockAdjustment.Remarks = "System Genarate";
                adb.StockAdjustmentVouchers.Add(stockAdjustment);
                adb.SaveChanges();
            }


            foreach (Infos item in bb)
            {
                DisbursementListDetail existing = db.DisbursementListDetails.Find(item.DetailId);
                existing.QuantityReceived = Convert.ToInt32(item.Actual);
                existing.Remarks = item.Remark;
                recivedX = Convert.ToInt32(existing.Quantity) - Convert.ToInt32(item.Actual);
                if (recivedX > 0)
                {
                    StockAdjustmentVoucherDetail adjustmentVoucherDetail = new StockAdjustmentVoucherDetail();
                    adjustmentVoucherDetail.AdjustmentVoucherNumber = (newNumer + 1).ToString();
                    adjustmentVoucherDetail.AdjustmentDetailsNumber = newNumer2;
                    newNumer2++;
                    adjustmentVoucherDetail.QuantityAdjusted = recivedX;
                    adjustmentVoucherDetail.ItemNumber = existing.ItemNumber;
                    // remarks from where?
                    adjustmentVoucherDetail.Reason = existing.Remarks;
                    db.StockAdjustmentVoucherDetails.Add(adjustmentVoucherDetail);
                    db.SaveChanges();
                }
            }

            if (countnotshow == bb.Count())
            {
                disbursementList.Status = "Cancelled";
                db.Entry(disbursementList).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                disbursementList.Status = "Collected";
                db.Entry(disbursementList).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { status = "Submitted Successfully" });
        }


        public ActionResult Details(string id, string sessionId, string errorMeg)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();

            List<DisbursementList> disbursementLists = db.DisbursementLists.ToList();
            List<DisbursementListDetail> disbursementListDetails = db.DisbursementListDetails.ToList();
            List<StationeryCatalog> stationeryCatalogs = db.StationeryCatalogs.ToList();
            var disbursementDetailRecord = (from d in disbursementListDetails
                                            join l in disbursementLists on d.ListNumber equals l.ListNumber into table1
                                            from l in table1.ToList()
                                            join s in stationeryCatalogs on d.ItemNumber equals s.ItemNumber into table2
                                            from s in table2.ToList()
                                            select new ViewModelDDetails
                                            {
                                                disbursementList = l,
                                                disbursementListDetail = d,
                                                stationeryCatalog = s,
                                            }).Where(x => x.disbursementList.ListNumber == id).ToList();


            if (disbursementDetailRecord.Count() == 0)
            {
                return RedirectToAction("Index", "DisbursementLists", new { @sessionId = sessionId });
            }
            DisbursementList disbursementList = db.DisbursementLists.Where(d => d.ListNumber == id).Single();
            DepartmentList departmentList = db.DepartmentLists.Where(d => d.DepartmentCode == disbursementList.DepartmentCode).Single();
            CollectionPoint collectionPoint = db.CollectionPoints.Where(c => c.CollectionPointCode == departmentList.CollectionPoint).Single();
            Employee employee = db.Employees.Where(e => e.Id == departmentList.RepresentativeId).Single();


            if (storeclerk != null)
            {

                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["collection"] = collectionPoint.CollectionPointName;
                ViewData["disbursementList"] = disbursementList.Date;
                ViewData["deparment"] = departmentList.DepartmentName;
                ViewData["employeeF"] = employee.FirstName;
                ViewData["employeeL"] = employee.LastName;
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                ViewData["errorMeg"] = errorMeg;
                return View(disbursementDetailRecord);
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
                ViewData["collection"] = collectionPoint.CollectionPointName;
                ViewData["disbursementList"] = disbursementList.Date;
                ViewData["deparment"] = departmentList.DepartmentName;
                ViewData["employeeF"] = employee.FirstName;
                ViewData["employeeL"] = employee.LastName;
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                ViewData["errorMeg"] = errorMeg;
                return View(disbursementDetailRecord);
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
                ViewData["collection"] = collectionPoint.CollectionPointName;
                ViewData["disbursementList"] = disbursementList.Date;
                ViewData["deparment"] = departmentList.DepartmentName;
                ViewData["employeeF"] = employee.FirstName;
                ViewData["employeeL"] = employee.LastName;
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
                ViewData["errorMeg"] = errorMeg;
                return View(disbursementDetailRecord);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult Update(List<ViewModelDDetails> Details,string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            DisbursementList disbursementList = db.DisbursementLists.Find(Details[0].disbursementList.ListNumber);
            int newNumer = db.StockAdjustmentVouchers.Count();
            int length = Details.Count() + newNumer + 1;
            int newNumer2 = db.StockAdjustmentVoucherDetails.Count() + 1;
            bool flag = false;
            int countnotshow = 0;
            int recivedX;

            foreach (ViewModelDDetails item in Details)
            {
                DisbursementListDetail existing = db.DisbursementListDetails.Find(item.disbursementListDetail.ListDetailsNumber);
                if (item.disbursementListDetail.QuantityReceived < 0 || item.disbursementListDetail.QuantityReceived > existing.Quantity)
                {
                    string errorMeg = "Error: " + "QuantityReceived cannot be LESS THAN 0 or GREATER THAN Quantity";
                    return RedirectToAction("Details", "DisbursementListDetails", new { @sessionId = sessionId, @id = Details[0].disbursementList.ListNumber, @errorMeg = errorMeg });
                }
            }

            foreach (ViewModelDDetails item in Details)
            {
                DisbursementListDetail existing = db.DisbursementListDetails.Find(item.disbursementListDetail.ListDetailsNumber);
                existing.QuantityReceived = item.disbursementListDetail.QuantityReceived;
                existing.Remarks = item.disbursementListDetail.Remarks;
                recivedX = Convert.ToInt32(existing.Quantity) - Convert.ToInt32(item.disbursementListDetail.QuantityReceived);
                if (recivedX > 0)
                {
                    flag = true;

                }
                if (Convert.ToInt32(item.disbursementListDetail.QuantityReceived) == 0)
                {
                    countnotshow++;
                }
                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (flag)
            {
                StockAdjustmentVoucher stockAdjustment = new StockAdjustmentVoucher();
                stockAdjustment.AdjustmentVoucherNumber = Convert.ToString(newNumer + 1);
                stockAdjustment.Status = "Pending";
                stockAdjustment.Date = DateTime.Now;
                stockAdjustment.Remarks = "System Generate";
                adb.StockAdjustmentVouchers.Add(stockAdjustment);
                adb.SaveChanges();
            }


            foreach (ViewModelDDetails item in Details)
            {
                DisbursementListDetail existing = db.DisbursementListDetails.Find(item.disbursementListDetail.ListDetailsNumber);
                existing.QuantityReceived = item.disbursementListDetail.QuantityReceived;
                existing.Remarks = item.disbursementListDetail.Remarks;
                recivedX = Convert.ToInt32(existing.Quantity) - Convert.ToInt32(item.disbursementListDetail.QuantityReceived);
                if (recivedX > 0)
                {
                    StockAdjustmentVoucherDetail adjustmentVoucherDetail = new StockAdjustmentVoucherDetail();
                    adjustmentVoucherDetail.AdjustmentVoucherNumber = (newNumer + 1).ToString();
                    adjustmentVoucherDetail.AdjustmentDetailsNumber = newNumer2;
                    newNumer2++;
                    adjustmentVoucherDetail.QuantityAdjusted = recivedX;
                    adjustmentVoucherDetail.ItemNumber = existing.ItemNumber;
                    // remarks from where?
                    adjustmentVoucherDetail.Reason = existing.Remarks;
                    db.StockAdjustmentVoucherDetails.Add(adjustmentVoucherDetail);
                    db.SaveChanges();
                }
            }

            if (countnotshow == Details.Count())
            {
                disbursementList.Status = "Cancelled";
                db.Entry(disbursementList).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                disbursementList.Status = "Collected";
                db.Entry(disbursementList).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (storeclerk != null)
            {
                return RedirectToAction("Index", "Home", new { @sessionId = sessionId, @tag = "storeclerk" });
            }
            else if (storeManager != null)
            {
                return RedirectToAction("Index", "StoreManagers", new { @sessionId = sessionId, @tag = "storeManager" });
            }
            else if (storeSupervisor != null)
            {
                return RedirectToAction("Index", "StoreSupervisors", new { @sessionId = sessionId, @tag = "storeSupervisor" });
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
