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
            return View(retrievalFormRecord);
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

            var requisitionFormRecord = (from r in requisitionForms
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
                                         }).Where(x => x.requisitionForms.Status == "Received");

            //for (int i = 0; i < requisitionFormRecord.Count; i++)
            //{ 
            //    if(requisi)
            //}

            return View(requisitionFormRecord);
        }

        [HttpPost]
        public ActionResult Update(ICollection<int> neededQuantity)
        {
            //int length = Details.Count() + 1;
            foreach (var i in neededQuantity)
            {
                Console.WriteLine(i);
            }

            //Find total quantity needed by each department
            //for (i = 0; i < ; i++)
            //{
            //    if (retrievalFormRecord.)
            //}

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
