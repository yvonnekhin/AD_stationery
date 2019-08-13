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
    public class StationeryRetrievalFormsController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: StationeryRetrievalForms
        public ActionResult Index(string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            GenerateRetrievalForm();
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

                return View(db.StationeryRetrievalForms.ToList());
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
                return View(db.StationeryRetrievalForms.ToList());
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
                return View(db.StationeryRetrievalForms.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // GET: StationeryRetrievalForms/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StationeryRetrievalForm stationeryRetrievalForm = db.StationeryRetrievalForms.Find(id);
            if (stationeryRetrievalForm == null)
            {
                return HttpNotFound();
            }
            return View(stationeryRetrievalForm);
        }

        // GET: StationeryRetrievalForms/Create
        public ActionResult Create()
        {
            return View();
        }

        public void GenerateRetrievalForm()
        {
            List<RequisitionForm> rf = db.RequisitionForms.ToList();
            List<RequisitionFormDetail> rfd = db.RequisitionFormDetails.ToList();
            List<Employee> emp = db.Employees.ToList();

            StationeryRetrievalForm newStationeryRetrievalForm = new StationeryRetrievalForm
            {
                FormNumber = (db.StationeryRetrievalForms.Count() + 1).ToString(),
                Date = DateTime.Today,
                Status = "Pending"
            };

            var srfdList = (from requisitionForm in rf
                                  join requisitionFormDetail in rfd on requisitionForm.FormNumber equals requisitionFormDetail.FormNumber into table1
                                  from requisitionFormDetail in table1
                                  join employees in emp on requisitionForm.EmployeeId equals employees.Id into table2
                                  from employees in table2
                                  where (requisitionForm.Status == "Read" && requisitionFormDetail.Status == "Pending") || (requisitionForm.Status == "Read" && requisitionFormDetail.Status == "Received")
                                  group requisitionFormDetail by new { requisitionFormDetail.ItemNumber, employees.DepartmentCode } into group1
                                  select new StationeryRetrievalFormDetail
                                  {                                     
                                      FormNumber = newStationeryRetrievalForm.FormNumber,
                                      ItemNumber = group1.Key.ItemNumber,
                                      DepartmentCode = group1.Key.DepartmentCode,
                                      Needed = group1.Sum(x=>x.Quantity),
                                      Actual = 0
                                  }).ToList();

            var requisitionFormDetailList = (from requisitionForm in rf
                                            join requisitionFormDetail in rfd on requisitionForm.FormNumber equals requisitionFormDetail.FormNumber into table1
                                            from requisitionFormDetail in table1
                                            where (requisitionForm.Status == "Read" && requisitionFormDetail.Status == "Pending") || (requisitionForm.Status == "Read" && requisitionFormDetail.Status == "Received")
                                            select requisitionFormDetail).ToList();

            if (srfdList.Count>0)
            {
                db.StationeryRetrievalForms.Add(newStationeryRetrievalForm);
                db.SaveChanges();

                int count = (db.StationeryRetrievalFormDetails.Count() + 1);

                foreach (StationeryRetrievalFormDetail srfd in srfdList)
                {
                    srfd.FormDetailsNumber = count.ToString();
                    db.StationeryRetrievalFormDetails.Add(srfd);
                    count++;
                }

                foreach(RequisitionFormDetail requisition in requisitionFormDetailList)
                {
                    RequisitionFormDetail existingRequisitionFormDetail = db.RequisitionFormDetails.Find(requisition.FormDetailsNumber);
                    existingRequisitionFormDetail.Status = "Awaiting Retrieval";
                }

                db.SaveChanges();
            }
        }

        // POST: StationeryRetrievalForms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FormNumber,Date,Status")] StationeryRetrievalForm stationeryRetrievalForm)
        {
            if (ModelState.IsValid)
            {
                db.StationeryRetrievalForms.Add(stationeryRetrievalForm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stationeryRetrievalForm);
        }

        // GET: StationeryRetrievalForms/Edit/5
        public ActionResult Edit(string id, string sessionId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StationeryRetrievalForm stationeryRetrievalForm = db.StationeryRetrievalForms.Find(id);
            if (stationeryRetrievalForm == null)
            {
                return HttpNotFound();
            }

            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            List<StationeryRetrievalFormDetail> srfd = db.StationeryRetrievalFormDetails.ToList();
            List<StationeryCatalog> sc = db.StationeryCatalogs.ToList();

            var retrievalFormDetailsSelected = (from stationeryRetrievalFormDetail in srfd
                                               join stationeryCatalog in sc on stationeryRetrievalFormDetail.ItemNumber equals stationeryCatalog.ItemNumber into table1
                                               from stationeryCatalog in table1
                                               where stationeryRetrievalFormDetail.FormNumber == id
                                               select new RForm
                                               {
                                                   stationeryRetrievalFormDetail = stationeryRetrievalFormDetail,
                                                   Description = stationeryCatalog.Description,
                                                   BinNumber = stationeryCatalog.BinNumber
                                               }).OrderBy(x=>x.Description).ToList();


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

                return View(retrievalFormDetailsSelected);
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
                return View(retrievalFormDetailsSelected);
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
                return View(retrievalFormDetailsSelected);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
           
        }

        // POST: StationeryRetrievalForms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(List<RForm> commitedRetrievalForm, string sessionId)
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
                List<string> departmentList = new List<string>();
                int outstandingListCount = db.OutstandingLists.Count();

                //update retrieval form status to "Submitted" 
                StationeryRetrievalForm existingStationeryRetrievalForm = db.StationeryRetrievalForms.Find(commitedRetrievalForm[0].stationeryRetrievalFormDetail.FormNumber);
                existingStationeryRetrievalForm.Status = "Submitted";

                foreach (RForm commitedFormDetail in commitedRetrievalForm)
                {
                    //Update actual values into retrieval form details
                    StationeryRetrievalFormDetail existingStationeryRetrievalFormDetail = db.StationeryRetrievalFormDetails.Find(commitedFormDetail.stationeryRetrievalFormDetail.FormDetailsNumber);
                    existingStationeryRetrievalFormDetail.Actual = commitedFormDetail.stationeryRetrievalFormDetail.Actual;

                    //update stationery catalog
                    StationeryCatalog existingCatalog = db.StationeryCatalogs.Find(commitedFormDetail.stationeryRetrievalFormDetail.ItemNumber);
                    existingCatalog.Balance -= commitedFormDetail.stationeryRetrievalFormDetail.Actual;

                    //add the department code to our department list created above, to generate disbursement list by department below
                    if(!departmentList.Contains(commitedFormDetail.stationeryRetrievalFormDetail.DepartmentCode))
                    {
                        departmentList.Add(commitedFormDetail.stationeryRetrievalFormDetail.DepartmentCode);
                    }

                    //If insufficient inventory, add the item code to our item code list created above, so that we can generate a Outstanding List by item code 
                    if(commitedFormDetail.stationeryRetrievalFormDetail.Needed>commitedFormDetail.stationeryRetrievalFormDetail.Actual)
                    { 
                    outstandingListCount++;

                    OutstandingList outstandingItem = new OutstandingList
                    {
                        OutstandingListNumber = outstandingListCount.ToString(),
                        RetrievalFormDetailsNumber = commitedFormDetail.stationeryRetrievalFormDetail.FormDetailsNumber,
                        Status = "Outstanding"
                    };
                        db.OutstandingLists.Add(outstandingItem);
                    }
                    //send disbursement notification (!OUTSTANDING!)

                    db.SaveChanges();                    
                }
                
                //create a single disbursement list for each department
                foreach (string deptCode in departmentList)
                { 

                DisbursementList dl = new DisbursementList
                {
                    ListNumber = (db.DisbursementLists.Count() + 1).ToString(),
                    DepartmentCode = deptCode,
                    Date = DateTime.Today,
                    Status = "Open"
                };
                db.DisbursementLists.Add(dl);
                db.SaveChanges();

                    int disbursementListDetailsCount = db.DisbursementListDetails.Count();

                    foreach (RForm commitedFormDetail in commitedRetrievalForm)
                    {
                        if (commitedFormDetail.stationeryRetrievalFormDetail.DepartmentCode.Equals(deptCode))
                        {
                            disbursementListDetailsCount++;

                            DisbursementListDetail dld = new DisbursementListDetail
                            {
                                ListDetailsNumber = disbursementListDetailsCount.ToString(),
                                ListNumber = dl.ListNumber,
                                ItemNumber = commitedFormDetail.stationeryRetrievalFormDetail.ItemNumber,
                                Quantity = commitedFormDetail.stationeryRetrievalFormDetail.Actual
                            };

                            db.DisbursementListDetails.Add(dld);
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index", "StationeryRetrievalForms", new { sessionId=sessionId});
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

                return View(commitedRetrievalForm);
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
                return View(commitedRetrievalForm);
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
                return View(commitedRetrievalForm);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
          
        }

        // GET: StationeryRetrievalForms/Delete/5
        public ActionResult Delete(string id,string sessionId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StationeryRetrievalForm stationeryRetrievalForm = db.StationeryRetrievalForms.Find(id);
            if (stationeryRetrievalForm == null)
            {
                return HttpNotFound();
            }
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

                return View(stationeryRetrievalForm);
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
                return View(stationeryRetrievalForm);
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
                return View(stationeryRetrievalForm);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }

        // POST: StationeryRetrievalForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StationeryRetrievalForm stationeryRetrievalForm = db.StationeryRetrievalForms.Find(id);
            db.StationeryRetrievalForms.Remove(stationeryRetrievalForm);
            db.SaveChanges();
            if (storeclerk != null)
            {
                return RedirectToAction("Index", "StationeryRetrievalForms", new { sessionId = sessionId });
            }
            else if (storeManager != null)
            {
                return RedirectToAction("Index", "StationeryRetrievalForms", new { sessionId = sessionId });
            }
            else if (storeSupervisor != null)
            {
                return RedirectToAction("Index", "StationeryRetrievalForms",new { sessionId=sessionId});
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
