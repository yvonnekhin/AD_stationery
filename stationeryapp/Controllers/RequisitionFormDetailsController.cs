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
    public class RequisitionFormDetailsController : Controller
    {
        private ModelDBContext db = new ModelDBContext();
        private RequisitionFormsDBContext db1 = new RequisitionFormsDBContext();
        private StoreClerkDBContext db2 = new StoreClerkDBContext();
        // GET: RequisitionFormDetails
        public ActionResult Index()
        {
            var requisitionFormDetails = db.RequisitionFormDetails.Include(r => r.RequisitionForm).Include(r => r.StationeryCatalog);
            return View(requisitionFormDetails.ToList());
        }
        // GET: RequisitionFormDetails/Details/5
        public ActionResult Details(string id, string sessionId)
        {
            StoreClerk storeclerk = db2.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            RequisitionForm request = db1.RequisitionForms.Where(p => p.FormNumber == id).FirstOrDefault();
            List<RequisitionFormDetail> detail = db.RequisitionFormDetails.Where(x => x.FormNumber == id).ToList();
            if (storeclerk != null && sessionId !=null)
            {
                if (request.Status == "Approved" && request.Status != "Completed")
                {
                    request.DateReceived = DateTime.Now;
                    request.ReceivedBy = storeclerk.Id;
                    request.Status = "Read";
                }
              
                int num = db1.RequisitionForms.Where(x => x.Status == "Approved").Count();
                db1.Entry(request).State = EntityState.Modified;
                db1.SaveChanges();
                ViewData["sessionId"] = storeclerk.SessionId;
                ViewData["username"] = storeclerk.UserName;
                ViewData["sumTotal"] = (num).ToString();
                return View(detail);
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
