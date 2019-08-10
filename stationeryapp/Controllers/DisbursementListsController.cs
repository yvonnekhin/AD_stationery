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


        public JsonResult GetDisbursementList()
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

            return Json(disbursementRecord, JsonRequestBehavior.AllowGet);
        }

        // GET: DisbursementLists
        public ActionResult Index(string sessionId)

        {
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();

            List <DisbursementList> disbursementLists = db.DisbursementLists.ToList();
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

            if (storeclerk != null && sessionId != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;

                return View(disbursementRecord);
            }
            else if (storeManager != null && sessionId != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                return View(disbursementRecord);

            }
            else if (storeSupervisor != null && sessionId != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment).ToString();
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                return View(disbursementRecord);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        // GET: DisbursementLists/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentCode = new SelectList(db.DepartmentLists, "DepartmentCode", "DepartmentName");
            return View();
        }

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
