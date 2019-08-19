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
    public class StoreManagersController : Controller
    {
        private ModelDBContext db = new ModelDBContext();
        public ActionResult Index(string sessionId,string tag)
        {
            StoreManager storeManager=db.StoreManagers.Where(p=>p.SessionId== sessionId).FirstOrDefault();
            if (storeManager != null && sessionId != null)
            {
                int num = db.RequisitionForms.Where(x => x.Status == "Approved").Count();
                int numDisbuserment = db.DisbursementLists.Where(x => x.Status == "Pending").Count();
                int numOutS = db.OutstandingLists.Where(x => x.Status == "Awaiting Goods").Count();
                int numRetrive = db.StationeryRetrievalForms.Where(x => x.Status == "Pending").Count();
                int numPO = db.PurchaseOrders.Where(x => x.Status == "Not Submitted").Count();
                int numStock = db.StockAdjustmentVouchers.Where(x => x.Status == "Pending").Count();
                ViewData["num"] = num;
                ViewData["numDisbuserment"] = numDisbuserment;
                ViewData["numOutSt"] = numOutS;
                ViewData["numRetriF"] = numRetrive;
                ViewData["numPO"] = numPO;
                ViewData["numStockAj"] = numStock;
                ViewData["sumTotal"] = (num + numDisbuserment + numOutS + numRetrive + numPO + numStock).ToString();
                ViewData["sessionId"] = storeManager.SessionId;
                ViewData["username"] = storeManager.UserName;
                ViewData["tag"] = tag;
                return View();
            }
            else
            {
                return RedirectToAction("LoginStoreManager", "Login");
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
