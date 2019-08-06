using stationeryapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace stationeryapp.Controllers
{
    public class ReportHomeController : Controller
    {
        // GET: ReportHome
        private ModelDBContext db = new ModelDBContext();
        public ActionResult Index(String sessionId)
        {
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (storeclerk != null && sessionId != null)
            {
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
            }
                return View();
        }
      
    }
}