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
        private ModelDBContext db1 = new ModelDBContext();
        private ModelDBContext db2 = new ModelDBContext();
        // GET: RequisitionFormDetails
        public ActionResult Index()
        {
            var requisitionFormDetails = db.RequisitionFormDetails.Include(r => r.RequisitionForm).Include(r => r.StationeryCatalog);
            return View(requisitionFormDetails.ToList());
        }
        // GET: RequisitionFormDetails/Details/5
        public ActionResult Details(string id, string sessionId)
        {
            if (sessionId == null||id==null)
            {
                return RedirectToAction("Login", "Login");
            }
            StoreClerk storeclerk = db2.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db2.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db2.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            RequisitionForm request = db1.RequisitionForms.Where(p => p.FormNumber == id).FirstOrDefault();
            List<RequisitionFormDetail> detail = db.RequisitionFormDetails.Where(x => x.FormNumber == id).ToList();
            string eId = request.EmployeeId;
            Employee emp = db.Employees.Where(y => y.Id == eId).FirstOrDefault();
            string emailAddress = emp.EmailAddress;
            string subject = "Your requisition has been received";
            string message = "<p>Dear "+emp.UserName +"."+"</p><br/><p>Your requisition has been received and will be pending</p><br/><p>Stationery Management Team</p>";

            if (storeclerk != null)
            {
                if (request.Status == "Approved" && request.Status != "Completed")
                {
                    util.SendEmail(emailAddress, subject, message);
                    request.DateReceived = DateTime.Now;
                    request.ReceivedBy = storeclerk.Id;
                    request.Status = "Read";
                }               
                db1.Entry(request).State = EntityState.Modified;
                db1.SaveChanges();
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
                return View(detail);
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
                return View(detail);
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
