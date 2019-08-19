using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using stationeryapp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace stationeryapp.Controllers
{
    public class TrendController : Controller
    {
        private ModelDBContext db = new ModelDBContext();
        // GET: Trend
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TrendAnalysis(string sessionId)
        {
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

                return View();
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
                return View();
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
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
           
        }
        [HttpPost]
        public ActionResult TrendSubmit(string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();

            string itemselected = Request["Items"].ToString();
            string monthselected = Request["Months"].ToString();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:5000/itemmonth/");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new
            StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    item = itemselected,
                    month = monthselected
                });

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            String result = null;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            //ViewBag.res = result;
            var JSONObj = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(result);
            String month1 = JSONObj["month1"];
            String month2 = JSONObj["month2"];
            String predict = JSONObj["predicted"];
            int mon1 = Int32.Parse(month1);
            int mon2 = Int32.Parse(month2);
            int pred = Int32.Parse(predict);

            String mont1var = null;
            String mont2var = null;
            switch (Int32.Parse(Request["Months"]))
            {
                case 3:
                    mont1var = "Jan";
                    mont2var = "Feb";
                    break;
                case 4:
                    mont1var = "Feb";
                    mont2var = "March";
                    break;
                case 5:
                    mont1var = "March";
                    mont2var = "April";
                    break;
                case 6:
                    mont1var = "April";
                    mont2var = "May";
                    break;
                case 7:
                    mont1var = "May";
                    mont2var = "June";
                    break;
                case 8:
                    mont1var = "June";
                    mont2var = "July";
                    break;
                case 9:
                    mont1var = "July";
                    mont2var = "August";
                    break;
                case 10:
                    mont1var = "August";
                    mont2var = "September";
                    break;
                case 11:
                    mont1var = "September";
                    mont2var = "October";
                    break;
                case 12:
                    mont1var = "October";
                    mont2var = "November";
                    break;
            }


            decimal mont1 = mon1;
            decimal mont2 = mon2;
            decimal prd1 = pred;
            new Chart(width: 800, height: 200).AddSeries(
              chartType: "column",
            xValue: new[] { mont1var, mont2var, "predicted" },
            yValues: new[] { mont1, mont2, prd1 }).Write("png");

           


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
                ViewBag.res = Request["Months"];
                return View();
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
                ViewBag.res = Request["Months"];
                return View();
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
                ViewBag.res = Request["Months"];
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            

        }

        [HttpPost]
        public ActionResult DeptSubmit(string sessionId)
        {
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            string itemselected = Request["Dept"].ToString();
            string monthselected = Request["Months"].ToString();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:5000/deptlist/");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new
            StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    Dept = itemselected,
                    month = monthselected
                });

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            String result = null;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            //ViewBag.res = result;
            var JSONObj = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(result);
            String month1 = JSONObj["month1"];
            String month2 = JSONObj["month2"];
            String predict = JSONObj["predicted"];
            int mon1 = Int32.Parse(month1);
            int mon2 = Int32.Parse(month2);
            int pred = Int32.Parse(predict);

            String mont1var = null;
            String mont2var = null;
            switch (Int32.Parse(Request["Months"]))
            {
                case 3:
                    mont1var = "Jan";
                    mont2var = "Feb";
                    break;
                case 4:
                    mont1var = "Feb";
                    mont2var = "March";
                    break;
                case 5:
                    mont1var = "March";
                    mont2var = "April";
                    break;
                case 6:
                    mont1var = "April";
                    mont2var = "May";
                    break;
                case 7:
                    mont1var = "May";
                    mont2var = "June";
                    break;
                case 8:
                    mont1var = "June";
                    mont2var = "July";
                    break;
                case 9:
                    mont1var = "July";
                    mont2var = "August";
                    break;
                case 10:
                    mont1var = "August";
                    mont2var = "September";
                    break;
                case 11:
                    mont1var = "September";
                    mont2var = "October";
                    break;
                case 12:
                    mont1var = "October";
                    mont2var = "November";
                    break;
            }


            decimal mont1 = mon1;
            decimal mont2 = mon2;
            decimal prd1 = pred;
            new Chart(width: 800, height: 200).AddSeries(
              chartType: "column",
            xValue: new[] { mont1var, mont2var, "predicted" },
            yValues: new[] { mont1, mont2, prd1 }).Write("png");

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
                ViewBag.res = Request["Months"];
                return View();
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
                ViewBag.res = Request["Months"];
                return View();
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
                ViewBag.res = Request["Months"];
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

            
        }



    }

}
