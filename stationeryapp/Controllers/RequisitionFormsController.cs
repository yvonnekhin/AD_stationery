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

        private StoreClerkDBContext db = new StoreClerkDBContext();
        private RequisitionFormsDBContext db1 = new RequisitionFormsDBContext();


        // GET: RequisitionForms
        public ActionResult Index(string sessionId)
        {
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            if (storeclerk != null && sessionId !=null)
            {
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
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
