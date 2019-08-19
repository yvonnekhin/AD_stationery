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
        private ModelDBContext db = new ModelDBContext();
        private ModelDBContext db1 = new ModelDBContext();
        public ActionResult Index(string sessionId)
        {
            PurchaseOrdersController purchaseOrdersController = new PurchaseOrdersController();
            purchaseOrdersController.SystemGeneratePO();
            purchaseOrdersController.MergePurchaseOrders();

            StationeryRetrievalFormsController stationeryRetrievalFormsController = new StationeryRetrievalFormsController();
            stationeryRetrievalFormsController.GenerateRetrievalForm();
            stationeryRetrievalFormsController.MergeRetrievalForms();

            StoreClerk storeclerk = dbM.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();    
            if (storeclerk != null && sessionId != null)
            {
                int num= dbM.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment= dbM.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = dbM.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
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
                ViewData["tag"] = "storeclerk";
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