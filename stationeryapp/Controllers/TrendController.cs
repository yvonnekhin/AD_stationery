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
        // GET: Trend
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TrendAnalysis()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TrendSubmit()
        {
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

            ViewBag.res = Request["Months"];
            return View();

        }

        [HttpPost]
        public ActionResult DeptSubmit()
        {
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

            ViewBag.res = Request["Months"];
            return View();

        }



    }

}
