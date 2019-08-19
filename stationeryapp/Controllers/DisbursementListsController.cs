using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
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

            return Json(new { data = new { id = disbursementRecord.Select(x => x.disbursementList.ListNumber), DepartmentCode = disbursementRecord.Select(x => x.departmentList.DepartmentCode), CollectionPointname = disbursementRecord.Select(x => x.collectionPoint.CollectionPointName), Date = disbursementRecord.Select(x => x.disbursementList.Date), Status = disbursementRecord.Select(x => x.disbursementList.Status) } }, JsonRequestBehavior.AllowGet);
        }

        // GET: DisbursementLists
        public ActionResult Index(string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();

            List<DisbursementList> disbursementLists = db.DisbursementLists.ToList();
            List<DepartmentList> departmentLists = db.DepartmentLists.ToList();
            List<CollectionPoint> collectionPoints = db.CollectionPoints.ToList();

            var disbursementRecord = from p in departmentLists
                                     join d in disbursementLists on p.DepartmentCode equals d.DepartmentCode into table1
                                     from d in table1.ToList()
                                     join c in collectionPoints on p.CollectionPoint equals c.CollectionPointCode into table2
                                     from c in table2.ToList()
                                     orderby d.Status descending
                                     select new ViewModelD
                                     {
                                         collectionPoint = c,
                                         disbursementList = d,
                                         departmentList = p
                                     };

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
                return View(disbursementRecord);
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
                return View(disbursementRecord);
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
                return View(disbursementRecord);
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
