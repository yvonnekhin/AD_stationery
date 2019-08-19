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
using stationeryapp.Util;

namespace stationeryapp.Controllers
{
    public class StationeryRetrievalFormsController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        public JsonResult GetRetrieval()
        {
            StationeryRetrievalForm stationeryRetrievalForm = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").FirstOrDefault();
            if (stationeryRetrievalForm == null)
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string id = stationeryRetrievalForm.FormNumber;
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
                                                    }).OrderBy(x => x.Description).ToList();

                return Json(new { data = new { FormNumber = retrievalFormDetailsSelected.Select(x => x.stationeryRetrievalFormDetail.FormNumber), FormDetailsnumber = retrievalFormDetailsSelected.Select(x => x.stationeryRetrievalFormDetail.FormDetailsNumber), ItemNumber = retrievalFormDetailsSelected.Select(x => x.stationeryRetrievalFormDetail.ItemNumber), BinNumber = retrievalFormDetailsSelected.Select(x => x.BinNumber), Description = retrievalFormDetailsSelected.Select(x => x.Description), Dept = retrievalFormDetailsSelected.Select(x => x.stationeryRetrievalFormDetail.DepartmentCode), Needed = retrievalFormDetailsSelected.Select(x => x.stationeryRetrievalFormDetail.Needed), Actual = retrievalFormDetailsSelected.Select(x => x.stationeryRetrievalFormDetail.Actual) }, status = "success" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult PostRetrieval(Retrieval Details)
        {
            List<RetrievalItem> cc = JsonConvert.DeserializeObject<List<RetrievalItem>>(Details.RetData[0]);
            if (ModelState.IsValid)
            {
                List<string> departmentList = new List<string>();
                int outstandingListCount = db.OutstandingLists.Count();

                //update retrieval form status to "Submitted" 
                StationeryRetrievalForm existingStationeryRetrievalForm = db.StationeryRetrievalForms.Find(cc[0].FormNumber);
                existingStationeryRetrievalForm.Status = "Submitted";

                foreach (RetrievalItem commitedFormDetail in cc)
                {
                    //Update actual values into retrieval form details
                    StationeryRetrievalFormDetail existingStationeryRetrievalFormDetail = db.StationeryRetrievalFormDetails.Find(commitedFormDetail.FormDetailsnumber);
                    existingStationeryRetrievalFormDetail.Actual = Convert.ToInt32(commitedFormDetail.Actual);

                    //update stationery catalog
                    StationeryCatalog existingCatalog = db.StationeryCatalogs.Find(commitedFormDetail.ItemNumber);
                    existingCatalog.Balance -= Convert.ToInt32(commitedFormDetail.Actual);

                    //add the department code to our department list created above, to generate disbursement list by department below
                    if (!departmentList.Contains(commitedFormDetail.Dept))
                    {
                        departmentList.Add(commitedFormDetail.Dept);
                    }

                    //If insufficient inventory, add the item code to our item code list created above, so that we can generate a Outstanding List by item code 
                    if (Convert.ToInt32(commitedFormDetail.Needed) > Convert.ToInt32(commitedFormDetail.Actual))
                    {
                        outstandingListCount++;

                        OutstandingList outstandingItem = new OutstandingList
                        {
                            OutstandingListNumber = outstandingListCount.ToString(),
                            RetrievalFormDetailsNumber = commitedFormDetail.FormDetailsnumber,
                            Status = "Outstanding"
                        };
                        db.OutstandingLists.Add(outstandingItem);
                    }

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
                        Status = "Pending"
                    };
                    db.DisbursementLists.Add(dl);
                    db.SaveChanges();

                    DepartmentList dept = db.DepartmentLists.Where(x => x.DepartmentCode == deptCode).FirstOrDefault();
                    string Eid = dept.RepresentativeId;
                    Employee repo = db.Employees.Find(Eid);
                    string emailAddress = repo.EmailAddress;
                    string pointId = dept.CollectionPoint;
                    CollectionPoint point = db.CollectionPoints.Find(pointId);
                    string subject = "Your items are ready for collection";
                    string message = "<p>Dear " + repo.UserName + "." + "</p><br/><p>Your items are ready for collection</p><br/><p>Collection point and time: " + point.CollectionPointName + "---" + point.CollectionTime + "</p><br/><p>Stationery Management Team</p>";
                    util.SendEmail(emailAddress, subject, message);

                    int disbursementListDetailsCount = db.DisbursementListDetails.Count();

                    foreach (RetrievalItem commitedFormDetail in cc)
                    {
                        if (commitedFormDetail.Dept.Equals(deptCode))
                        {
                            disbursementListDetailsCount++;

                            DisbursementListDetail dld = new DisbursementListDetail
                            {
                                ListDetailsNumber = disbursementListDetailsCount.ToString(),
                                ListNumber = dl.ListNumber,
                                ItemNumber = commitedFormDetail.ItemNumber,
                                Quantity = Convert.ToInt32(commitedFormDetail.Actual)
                            };

                            db.DisbursementListDetails.Add(dld);
                            db.SaveChanges();
                        }

                    }
                }
            }
            return Json(new { status = "success" });
        }

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
            //GenerateRetrievalForm();

            List<StationeryRetrievalForm> stationeryRetrievalForms = db.StationeryRetrievalForms.ToList();

            var srfList = (from srf in stationeryRetrievalForms
                          orderby srf.Date descending
                          where srf.Status!="Merged"
                           select srf).ToList();

            if (storeclerk != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                return View(srfList);
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
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                return View(db.StationeryRetrievalForms.ToList());
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
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
                return View(db.StationeryRetrievalForms.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // GET: StationeryRetrievalForms/Details/5
        public ActionResult Details(string id, string sessionId)
        {
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
                                                }).OrderBy(x => x.Description).ToList();


            if (storeclerk != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
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
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
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
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
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

        // GET: StationeryRetrievalForms/Create
        public ActionResult Create()
        {
            return View();
        }

        public void MergeRetrievalForms()
        {
            List<StationeryRetrievalForm> stationeryRetrievalForms = db.StationeryRetrievalForms.ToList();
            List<StationeryRetrievalFormDetail> stationeryRetrievalFormDetails = db.StationeryRetrievalFormDetails.ToList();

            var srfList = (from srf in stationeryRetrievalForms
                          where srf.Status == "Pending"
                          select srf).ToList();

            if (srfList.Count > 1)
            {
                StationeryRetrievalForm createdStationeryRetrievalForm = new StationeryRetrievalForm
                {
                    FormNumber = (db.StationeryRetrievalForms.Count() + 1).ToString(),
                    Date = DateTime.Now,
                    Status = "Pending"
                };

                db.StationeryRetrievalForms.Add(createdStationeryRetrievalForm);
                db.SaveChanges();

                foreach (StationeryRetrievalForm sRF in srfList)
                {
                    StationeryRetrievalForm existingStationeryRetrievalForm = db.StationeryRetrievalForms.Find(sRF.FormNumber);
                    existingStationeryRetrievalForm.Status = "Merged";

                    var srfdList = (from srfd in stationeryRetrievalFormDetails
                                    where srfd.FormNumber == sRF.FormNumber
                                    select srfd).ToList();

                    foreach (StationeryRetrievalFormDetail srfd in srfdList)
                    {
                        StationeryRetrievalFormDetail existingStationeryRetrievalFormDetail = db.StationeryRetrievalFormDetails.Find(srfd.FormDetailsNumber);
                        existingStationeryRetrievalFormDetail.FormNumber = createdStationeryRetrievalForm.FormNumber;
                        db.SaveChanges();
                    }
                }

                List<StationeryRetrievalFormDetail> srfdListForGrouping = db.StationeryRetrievalFormDetails.ToList();

                var retrievalFormDetailsMerged = (from stationeryRetrievalFormDetail in srfdListForGrouping
                                                 where stationeryRetrievalFormDetail.FormNumber == createdStationeryRetrievalForm.FormNumber
                                                 group stationeryRetrievalFormDetail by new { stationeryRetrievalFormDetail.ItemNumber, stationeryRetrievalFormDetail.DepartmentCode } into group1
                                                 select new StationeryRetrievalFormDetail
                                                 {
                                                     FormNumber = createdStationeryRetrievalForm.FormNumber,
                                                     ItemNumber = group1.Key.ItemNumber,
                                                     DepartmentCode = group1.Key.DepartmentCode,
                                                     Needed = group1.Sum(x => x.Needed),
                                                     Actual = 0
                                                 }).ToList();

                var oldFormsToDelete = (from stationeryRetrievalFormDetail in srfdListForGrouping
                                       where stationeryRetrievalFormDetail.FormNumber == createdStationeryRetrievalForm.FormNumber
                                       select stationeryRetrievalFormDetail).ToList();

                foreach(StationeryRetrievalFormDetail formToDelete in oldFormsToDelete)
                {
                    db.StationeryRetrievalFormDetails.Remove(formToDelete);
                    db.SaveChanges();
                }


                foreach(StationeryRetrievalFormDetail formdetail in retrievalFormDetailsMerged)
                {
                    List<StationeryRetrievalFormDetail> listForId = db.StationeryRetrievalFormDetails.ToList();

                    var queryId = (from lfd in listForId
                                   select lfd.FormDetailsNumber).ToList();

                    formdetail.FormDetailsNumber = GenerateID.CreateNewId(queryId);
                    db.StationeryRetrievalFormDetails.Add(formdetail);
                    db.SaveChanges();
                }

            }
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
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                return View(retrievalFormDetailsSelected);
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
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                return View(retrievalFormDetailsSelected);
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
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
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

                    //If insufficient inventory, generate a outstanding list object for each retrieval form detail 
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
                    Status = "Pending"
                };
                db.DisbursementLists.Add(dl);
                db.SaveChanges();

                    DepartmentList dept = db.DepartmentLists.Where(x => x.DepartmentCode == deptCode).FirstOrDefault();
                    string Eid = dept.RepresentativeId;
                    Employee repo = db.Employees.Find(Eid);
                    string emailAddress = repo.EmailAddress;
                    string pointId = dept.CollectionPoint;
                    CollectionPoint point = db.CollectionPoints.Find(pointId);
                    string subject = "Your items are ready for collection";
                    string message = "<p>Dear " + repo.UserName + "." + "</p><br/><p>Your items are ready for collection</p><br/><p>Collection point and time: "+point.CollectionPointName+"---"+point.CollectionTime +"</p><br/><p>Stationery Management Team</p>";
                    util.SendEmail(emailAddress, subject, message);

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
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
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
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
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
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
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
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
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
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
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
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
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
