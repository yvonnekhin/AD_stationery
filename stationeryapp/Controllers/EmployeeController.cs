using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using stationeryapp.Models;

namespace stationery.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index(string id)
        {
            Employee user = (Employee)Session["user"];
            Session["count"] = 0; 
            int count;
            List<RequisitionForm> notify_form_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                notify_form_list = db.RequisitionForms.Where(f => f.EmployeeId == user.Id && (f.Notification == "approved_by_hod" || f.Notification == "rejected_by_hod")).ToList();
                count = db.RequisitionForms.Where(f => f.EmployeeId == user.Id && (f.Notification == "approved_by_hod" || f.Notification == "rejected_by_hod")).Count();
            }
            Session["count"] = count;
            Session["notify_form_list"] = notify_form_list;
            ViewData["userobj"] = user;
            return View();
        }

        public ActionResult newrequest()
        {
            employee_utility eu = new employee_utility();
            Employee user = (Employee)Session["user"];
            Employee emp = eu.get_employee(user);
            
            eu.delete_temp_request_forms(user);
            string form_number = eu.get_form_no();
            eu.add_newrequestform(form_number, emp);

            ViewData["catalog_list"] = eu.get_catalog_items();
            ViewData["emp"] = emp;
            ViewData["emp_dept"] = eu.get_department(user);
            ViewData["form_number"] = form_number;
            return View();
        }

        public ActionResult viewrequest(string id)
        {
            employee_utility eu = new employee_utility();
            Employee user = (Employee)Session["user"];
            
            ViewData["userobj"] = user;
            ViewData["requestlist"] = eu.get_req_forms(user);
            return View();
        }

        public ActionResult addnew(RequisitionFormDetail form_detail_obj)
        {
            employee_utility eu = new employee_utility();
            eu.add_new_form_detail_row(form_detail_obj);
            
            return RedirectToAction("addcart");
        }

        public ActionResult addcart()
        {
            employee_utility eu = new employee_utility();
            Employee user = (Employee)Session["user"];
            
            string form_number = eu.get_form_no_();
            List<RequisitionFormDetail> form_cart_list = eu.get_RequisitionFormDetails(form_number);
            
            ViewData["catalog_list"] = eu.get_catalog_items();
            ViewData["emp"] = eu.get_employee(user);
            ViewData["emp_dept"] = eu.get_department(user);
            ViewData["form_cart"] = form_cart_list;
            ViewData["form_number"] = form_number;

            return View("newrequest");
        }

        public ActionResult submitrequest()
        {
            var emp_id = Request.Form["emp_id"];      
            var form_number = Request.Form["form_number"];

            employee_utility eu = new employee_utility();
            eu.submit_request(emp_id, form_number);
            
            return RedirectToAction("EmployeeIndex");
        }

        public ActionResult view_single_request(string form_id, string notify)
        {
            employee_utility eu = new employee_utility();
            Employee user = (Employee)Session["user"];
            List<RequisitionForm> notify_form_list;
            int count;
            if (notify == "seen")
            {
                using (ModelDBContext db = new ModelDBContext())
                { 
                    RequisitionForm old_rec_to_update = db.RequisitionForms.Where(e => e.FormNumber == form_id).FirstOrDefault();
                    if (old_rec_to_update != null)
                    {
                        old_rec_to_update.Notification = "seen_by_emp";
                    }
                    db.SaveChanges();
                }
                
            }
            using (ModelDBContext db = new ModelDBContext())
            {
                notify_form_list = db.RequisitionForms.Where(f => f.EmployeeId == user.Id && (f.Notification == "approved_by_hod" || f.Notification == "rejected_by_hod")).ToList();
                count = db.RequisitionForms.Where(f => f.EmployeeId == user.Id && (f.Notification == "approved_by_hod" || f.Notification == "rejected_by_hod")).Count();
            }

            Session["count"] = count;
            Session["notify_form_list"] = notify_form_list;

            ViewData["form_cart"] = eu.get_RequisitionFormDetails(form_id);
            ViewData["form_number"] = form_id;
            ViewData["emp"] = eu.get_employee(user);
            ViewData["emp_dept"] = eu.get_department(user);
            ViewData["catalog_list"] = eu.get_catalog_items();
            ViewData["single_form_view"] = true;
            ViewData["req_form"] = eu.get_single_form_details(form_id);

            return View("newrequest");
        }

        public ActionResult notify()
        {
            Employee user = (Employee)Session["user"];
            List<RequisitionForm> notify_form_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                notify_form_list = db.RequisitionForms.Where(f => f.EmployeeId == user.Id && (f.Notification == "approved_by_hod" || f.Notification == "rejected_by_hod")).ToList();
            }
            Session["count"] = null;
            return RedirectToAction("index");

        }
    }
}