using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using stationeryapp.Models;

namespace stationeryapp.Controllers
{
    public class HomeController : Controller
    {
        private ModelDBContext dbM = new ModelDBContext();
        private StoreClerkDBContext db = new StoreClerkDBContext();
        private RequisitionFormsDBContext db1 = new RequisitionFormsDBContext();
        public ActionResult Index(string sessionId,string tag)
        { 
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();    
            if (storeclerk != null && sessionId != null)
            {
                int num= db1.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment= dbM.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = dbM.OutstandingLists.Where(x => x.Status == "Outstanding").Count();
                int numRetrive = dbM.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = dbM.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = dbM.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["num"] = num;
                ViewData["numDisbuserment"] = numDisbuserment;
                ViewData["numOutSt"] = numOutS;
                ViewData["numRetriF"] = numRetrive;
                ViewData["numPO"] = numPO;
                ViewData["numStockAj"] = numStock;
                ViewData["sumTotal"] = (num + numDisbuserment+ numOutS + numRetrive + numPO+ numStock).ToString();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["tag"] = tag;
                return View();
            }     
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult NotFound(string aspxerrorpath)
        {
            Response.Status = "404 Not Found";
            Response.StatusCode = 404;
            return View("../Shared/Error");
        }
    }
}