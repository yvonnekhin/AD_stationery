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
    public class DisbursementListsController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: DisbursementLists
        public ActionResult Index()

        {

            List<DisbursementList> disbursementLists = db.DisbursementLists.ToList();
            List<DepartmentList> departmentLists = db.DepartmentLists.ToList();
            List<CollectionPoint> collectionPoints = db.CollectionPoints.ToList();

            var disbursementRecord = from p in departmentLists
                                     join d in disbursementLists on p.DepartmentCode equals d.DepartmentCode into table1
                                     from d in table1.ToList()
                                     join c in collectionPoints on p.CollectionPoint equals c.CollectionPointCode into table2
                                     from c in table2.ToList()
                                     select new ViewModelD
                                     {
                                         collectionPoint = c,
                                         disbursementList = d,
                                         departmentList = p
                                     };


            return View(disbursementRecord);
        }


        // GET: DisbursementLists/Details/5
        public ActionResult DisbursementForm(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DisbursementList disbursementList = db.DisbursementLists.Find(id);
            if (disbursementList == null)
            {
                return HttpNotFound();
            }
            return View(disbursementList);
        }



        // GET: DisbursementLists/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentCode = new SelectList(db.DepartmentLists, "DepartmentCode", "DepartmentName");
            return View();
        }

        // POST: DisbursementLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ListNumber,DepartmentCode,Date,Status")] DisbursementList disbursementList)
        {
            if (ModelState.IsValid)
            {
                db.DisbursementLists.Add(disbursementList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentCode = new SelectList(db.DepartmentLists, "DepartmentCode", "DepartmentName", disbursementList.DepartmentCode);
            return View(disbursementList);
        }

        // GET: DisbursementLists/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisbursementList disbursementList = db.DisbursementLists.Find(id);
            if (disbursementList == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentCode = new SelectList(db.DepartmentLists, "DepartmentCode", "DepartmentName", disbursementList.DepartmentCode);
            return View(disbursementList);
        }

        // POST: DisbursementLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ListNumber,DepartmentCode,Date,Status")] DisbursementList disbursementList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(disbursementList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentCode = new SelectList(db.DepartmentLists, "DepartmentCode", "DepartmentName", disbursementList.DepartmentCode);
            return View(disbursementList);
        }

        // GET: DisbursementLists/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisbursementList disbursementList = db.DisbursementLists.Find(id);
            if (disbursementList == null)
            {
                return HttpNotFound();
            }
            return View(disbursementList);
        }

        // POST: DisbursementLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DisbursementList disbursementList = db.DisbursementLists.Find(id);
            db.DisbursementLists.Remove(disbursementList);
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
