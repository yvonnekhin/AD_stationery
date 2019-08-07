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

        private StationeryRetrievalFormDetailsDBContext adb = new StationeryRetrievalFormDetailsDBContext();

        // GET: StationeryRetrievalForms
        public ActionResult Index()
        {
            return View(db.StationeryRetrievalForms.ToList());
        }

        // GET: StationeryRetrievalForms/Details/5
        public ActionResult Details(string id)
        {
            List<StationeryRetrievalFormDetail> retrievalFormDetails = db.StationeryRetrievalFormDetails.ToList();
            List<StationeryCatalog> catalogs = db.StationeryCatalogs.ToList();
            List<DepartmentList> departmentLists = db.DepartmentLists.ToList();

            var retrievalFormRecord = (from r in retrievalFormDetails
                                       join c in catalogs on r.ItemNumber equals c.ItemNumber into table1
                                       from c in table1.ToList()
                                       join d in departmentLists on r.DepartmentCode equals d.DepartmentCode into table2
                                       from d in table2.ToList()
                                       select new ViewModelRetrieval
                                       {
                                           retrievalFormDetails = r,
                                           catalogs = c,
                                           departmentLists = d
                                       }).Where(x => x.retrievalFormDetails.FormNumber == id);

            //Display 'To Date'
            StationeryRetrievalForm retrievalForm1 = db.StationeryRetrievalForms.Where(r => r.FormNumber == id).Single();
            String ToDate = retrievalForm1.Date.Value.ToString("dd-MMM-yyyy");
            ViewData["ToDate"] = ToDate;

            //Display 'From Date'
            StationeryRetrievalForm retrievalForm2 = db.StationeryRetrievalForms.OrderByDescending(x => x.Date)
                                                       .Where(x => x.Status == "Submitted")
                                                       .Where(x => x.Date < retrievalForm1.Date)
                                                       .First();
            String FromDate = retrievalForm2.Date.Value.AddDays(1).ToString("dd-MMM-yyyy");
            ViewData["FromDate"] = FromDate;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StationeryRetrievalForm stationeryRetrievalForm = db.StationeryRetrievalForms.Find(id);
            if (stationeryRetrievalForm == null)
            {
                return HttpNotFound();
            }

            List<ConsolidatedRequest> clist = sortDetails(retrievalFormRecord);

            return View(clist);
        }

        // GET: StationeryRetrievalForms/Create
        public ActionResult Create()
        {
            //Display 'From Date'
            StationeryRetrievalForm retrievalForm1 = db.StationeryRetrievalForms.OrderByDescending(x => x.Date)
                                                       .Where(x => x.Status == "Submitted").First();
            String FromDate = retrievalForm1.Date.Value.ToString("dd-MMM-yyyy");
            ViewData["FromDate"] = FromDate;

            List<RequisitionForm> requisitionForms = db.RequisitionForms.ToList();
            List<StationeryCatalog> catalogs = db.StationeryCatalogs.ToList();
            List<Employee> employees = db.Employees.ToList();
            List<RequisitionFormDetail> requisitionFormDetails = db.RequisitionFormDetails.ToList();
            List<OutstandingList> outstandingLists = db.OutstandingLists.ToList();
            List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();
            List<StationeryRetrievalFormDetail> retrievalFormDetails = db.StationeryRetrievalFormDetails.ToList();

            var requisitionReceived = (from r in requisitionForms
                                       join e in employees on r.EmployeeId equals e.Id into table1
                                       from e in table1
                                       join d in requisitionFormDetails on r.FormNumber equals d.FormNumber into table2
                                       from d in table2
                                       join c in catalogs on d.ItemNumber equals c.ItemNumber into table3
                                       from c in table3.ToList()
                                       select new ViewModelRetrieval
                                       {
                                           requisitionForms = r,
                                           employees = e,
                                           requisitionFormDetails = d,
                                           catalogs = c
                                       }).Where(x => x.requisitionForms.Status == "Received");

            List<RetrievalRecord> result = sortCreate(requisitionReceived);
            return View(result);
        }

        [HttpPost]
        public ActionResult Update(FormCollection fc)
        {
            //Display 'From Date'
            StationeryRetrievalForm retrievalForm1 = db.StationeryRetrievalForms.OrderByDescending(x => x.Date)
                                                       .Where(x => x.Status == "Submitted").First();
            String FromDate = retrievalForm1.Date.Value.ToString("dd-MMM-yyyy");
            ViewData["FromDate"] = FromDate;

            //Using same query as for create, getting the first value in the list
            List<RequisitionForm> requisitionForms = db.RequisitionForms.ToList();
            List<StationeryCatalog> catalogs = db.StationeryCatalogs.ToList();
            List<Employee> employees = db.Employees.ToList();
            List<RequisitionFormDetail> requisitionFormDetails = db.RequisitionFormDetails.ToList();
            List<OutstandingList> outstandingLists = db.OutstandingLists.ToList();
            List<PurchaseOrder> purchaseOrders = db.PurchaseOrders.ToList();
            List<StationeryRetrievalFormDetail> retrievalFormDetails = db.StationeryRetrievalFormDetails.ToList();

            List<ViewModelRetrieval> requisitionReceived = (List<ViewModelRetrieval>)(from r in requisitionForms
                                                                                      join e in employees on r.EmployeeId equals e.Id into table1
                                                                                      from e in table1.ToList()
                                                                                      join d in requisitionFormDetails on r.FormNumber equals d.FormNumber into table2
                                                                                      from d in table2.ToList()
                                                                                      join c in catalogs on d.ItemNumber equals c.ItemNumber into table3
                                                                                      from c in table3.ToList()
                                                                                      select new ViewModelRetrieval
                                                                                      {
                                                                                          requisitionForms = r,
                                                                                          employees = e,
                                                                                          requisitionFormDetails = d,
                                                                                          catalogs = c
                                                                                      }).Where(x => x.requisitionForms.Status == "Received").ToList();

            //Generate new retrieval form number
            string newFormNumber = generateFormNumber(db.StationeryRetrievalForms.ToList().Count);
            //setting the date to FromDate and status to Submitted
            DateTime date = DateTime.Parse(FromDate);
            string status = "Submitted";
            StationeryRetrievalForm retrievalFormFinal = new StationeryRetrievalForm(newFormNumber, date, status);
            db.StationeryRetrievalForms.Add(retrievalFormFinal);
            db.SaveChanges();

            //getting the item numbers from the query           
            string[] itemnumber = requisitionReceived.ConvertAll<String>(p => p.catalogs.ItemNumber.ToString()).ToArray<String>();
            //getting the updated needed & actual values from the html view
            string[] departmentcodes = fc.GetValues("departmentcode");
            string[] neededvalues = fc.GetValues("needed");
            string[] actualvalues = fc.GetValues("actual");

            //creating the StationeryRetrievalFormDetails in db
            for (int i = 0; i < neededvalues.Length; i++)
            {
                StationeryRetrievalFormDetail record = new StationeryRetrievalFormDetail();
                //autogenerate random string for formdetailsnumber
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[8];
                var random = new Random();
                for (int j = 0; j < stringChars.Length; j++)
                {
                    stringChars[j] = chars[random.Next(chars.Length)];
                }
                var finalString = new String(stringChars);
                record.FormDetailsNumber = finalString;
                record.FormNumber = newFormNumber;
                record.ItemNumber = (itemnumber[i]);
                record.DepartmentCode = (departmentcodes[i]);
                record.Needed = Convert.ToInt32(neededvalues[i]);
                record.Actual = Convert.ToInt32(actualvalues[i]);
                //updating the DB
                db.StationeryRetrievalFormDetails.Add(record);
                db.SaveChanges();
            }

            //Update status in RequisitionForms to "Outstanding" upon submission of retrieval form


            //if item is in stock(needed == actual), update disbursement DB and balance in StationeryCatalog


            //else if insufficent quantity, update outstanding list


            return RedirectToAction("Index", "StationeryRetrievalForms");
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
        public ActionResult Edit(string id)
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

        // POST: StationeryRetrievalForms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FormNumber,Date,Status")] StationeryRetrievalForm stationeryRetrievalForm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stationeryRetrievalForm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stationeryRetrievalForm);
        }

        // GET: StationeryRetrievalForms/Delete/5
        public ActionResult Delete(string id)
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

        // POST: StationeryRetrievalForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            StationeryRetrievalForm stationeryRetrievalForm = db.StationeryRetrievalForms.Find(id);
            db.StationeryRetrievalForms.Remove(stationeryRetrievalForm);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public List<ConsolidatedRequest> sortDetails(IEnumerable<ViewModelRetrieval> retrievalFormRecord)
        {
            List<ConsolidatedRequest> clist = new List<ConsolidatedRequest>();
            foreach (var record in retrievalFormRecord)
            {
                ConsolidatedRequest.DepartmentRequest t = new ConsolidatedRequest.DepartmentRequest(record.departmentLists.DepartmentCode,
                    (int)record.retrievalFormDetails.Needed, (int)record.retrievalFormDetails.Actual);
                ConsolidatedRequest req = clist.Find(n => n.binNumber == record.catalogs.BinNumber);
                if (req != null)
                {
                    req.addNeeded((int)record.retrievalFormDetails.Needed);
                    req.addRetrieved((int)record.retrievalFormDetails.Actual);
                    req.requests.Add(t);
                }
                else
                {
                    ConsolidatedRequest cr = new ConsolidatedRequest(record.catalogs.BinNumber, record.catalogs.Description);
                    cr.addNeeded((int)record.retrievalFormDetails.Needed);
                    cr.addRetrieved((int)record.retrievalFormDetails.Actual);
                    cr.requests.Add(t);
                    clist.Add(cr);
                }
            }

            return clist;
        }

        public List<RetrievalRecord> sortCreate(IEnumerable<ViewModelRetrieval> retrievalFormRecord)
        {
            List<RetrievalRecord> result = new List<RetrievalRecord>();
            foreach (var record in retrievalFormRecord)
            {
                RetrievalRecord rr = new RetrievalRecord(record.catalogs.BinNumber, record.catalogs.Description, record.employees.DepartmentCode,
                    (int)record.requisitionFormDetails.Quantity, (int)record.requisitionFormDetails.Quantity);
                result.Add(rr);
            }

            return result;
        }

        public string generateFormNumber(int count)
        {
            count += 1;
            return count.ToString();
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
