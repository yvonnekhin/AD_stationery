using System;
using System.Collections;
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
    public class RequisitionFormsController : Controller
    {

        private ModelDBContext db = new ModelDBContext();
        private ModelDBContext db2 = new ModelDBContext();
        private ModelDBContext db1 = new ModelDBContext();



        // GET: RequisitionForms
        public ActionResult Index(string sessionId)
        {
            StoreClerk storeclerk = db2.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db2.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db2.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (storeclerk != null && sessionId !=null)
            {
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = "storeclerk";
                int num = db2.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db2.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db2.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db2.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db2.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db2.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                List<RequisitionForm> rev = db1.RequisitionForms.Where(x =>x.Status!="Rejected").Where(y=>y.Status!="Pending").ToList();
                return View(rev);
            }
            else if (storeManager != null && sessionId != null)
            {
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = "storeManager";
                int num = db2.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db2.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db2.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db2.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db2.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db2.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                List<RequisitionForm> rev = db1.RequisitionForms.ToList();
                return View(rev);
            }
            else if (storeSupervisor != null && sessionId != null)
            {
                ViewData["sessionId"] = storeSupervisor.SessionId;
                ViewData["username"] = storeSupervisor.UserName;
                ViewData["tag"] = "storeSupervisor";
                int num = db2.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db2.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db2.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db2.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db2.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db2.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                List<RequisitionForm> rev = db1.RequisitionForms.ToList();
                return View(rev);
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
