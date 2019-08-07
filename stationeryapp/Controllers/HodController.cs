using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using stationeryapp.Models;
namespace stationeryapp.Controllers
{
    public class HodController : Controller
    {
        // GET: Hod
        public ActionResult Index()
        {
            Employee user = (Employee)Session["user"];
            int count;
            List<RequisitionForm> notify_form_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                notify_form_list = db.RequisitionForms.Where(f=>f.Notification == "sent_to_hod" ).ToList();
                count = db.RequisitionForms.Where(f => f.Notification == "sent_to_hod").Count();
            }
            Session["count"] = count;
            Session["notify_form_list"] = notify_form_list;
            return View();
        }

        public ActionResult req_forms(string form_id, string notify)
        {
            if (notify=="seen")
            {
                using (ModelDBContext db = new ModelDBContext())
                {
                    db.RequisitionForms.Where(f => f.FormNumber == form_id).FirstOrDefault().Notification = "seen_by_hod";
                    db.SaveChanges();
                }
            }
            int count;
            List<RequisitionForm> notify_form_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                notify_form_list = db.RequisitionForms.Where(f => f.Notification == "sent_to_hod").ToList();
                count = db.RequisitionForms.Where(f => f.Notification == "sent_to_hod").Count();
            }
            Session["count"] = count;
            Session["notify_form_list"] = notify_form_list;

            Employee user = (Employee)Session["user"];
            List<RequisitionFormDetail> form_cart;
            RequisitionForm form;
            List<StationeryCatalog> items;
            Employee emp;
            DepartmentList dept;
            using (ModelDBContext db = new ModelDBContext())
            {
                 form = db.RequisitionForms.Find(form_id);
                 form_cart  = db.RequisitionFormDetails.Where(f => f.FormNumber == form_id).ToList();
                 items = db.StationeryCatalogs.ToList();
                 emp = db.Employees.Where(e => e.Id == form.EmployeeId).FirstOrDefault();
                 dept = db.DepartmentLists.Where(e => e.DepartmentCode == emp.DepartmentCode).FirstOrDefault();
            }

            ViewData["form_cart"] = form_cart;
            ViewData["form_number"] = form_id;
            ViewData["emp"] = emp;
            ViewData["emp_dept"] = dept;
            ViewData["catalog_list"] = items;
            if (form.Status=="pending")
            {
                ViewData["req_pending_forms"] = true;
            }
            ViewData["req_form"] = form;

            return View();
        }

        public ActionResult approve_reject(string form_id, string form_status, string comment, string approved_by)
        {
            Debug.WriteLine(form_id+form_status+comment);
            string notify_status="";
            string email_status = "";
            if (form_status.Equals("APPROVE"))
            {
                form_status = "approved";
                notify_status = "approved_by_hod";
                email_status = "Approved By HOD";
            }
            else
            {
                form_status = "rejected";
                notify_status = "rejected_by_hod";
                email_status = "Rejected By HOD";
            }
            RequisitionForm form;
            using (ModelDBContext db = new ModelDBContext())
            {
                form = db.RequisitionForms.Find(form_id);
                form.Status = form_status;
                form.Comments = comment;
                form.DateApproved = DateTime.Now.Date;
                form.ApprovedBy = approved_by;
                form.Notification = notify_status;
                db.SaveChanges();
            }
            RequisitionForm form_;
            using (ModelDBContext db = new ModelDBContext())
            {
                form_ = db.RequisitionForms.Find(form_id);
                util.SendEmail(form_.Employee.EmailAddress, "From Head Of Dept", form_.Employee.DepartmentCode + "/" + (1000 + int.Parse(form_.FormNumber)).ToString() + " " + email_status);
            }
                

            return RedirectToAction("Index", "Hod");
        }

        public ActionResult newstationaryrequest()
        {
            Employee user = (Employee)Session["user"];
            List<RequisitionForm> requestlist;
            List<Employee> emp_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                emp_list = db.Employees.ToList();
                requestlist = db.RequisitionForms.Where(f => db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode).Select(e => e.Id).Contains(f.EmployeeId) && (f.Status == "pending")).OrderByDescending(f=>f.DateReceived).ToList();
            }
            ViewData["emp_list"] = emp_list;
            ViewData["userobj"] = user;
            ViewData["requestlist"] = requestlist;
            ViewData["from_pending_reqs"] = true;

            return View("stationaryrequesthistory");
        }
        public ActionResult stationaryrequesthistory()
        {
            Employee user = (Employee)Session["user"];
            List<RequisitionForm> requestlist;
            List<Employee> emp_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                emp_list = db.Employees.ToList();
                requestlist = db.RequisitionForms.Where( f => db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode).Select(e=>e.Id).Contains(f.EmployeeId) && (f.Status=="approved" || f.Status=="rejected")).OrderByDescending(f => f.DateReceived).ToList();
            }
            ViewData["emp_list"] = emp_list;
            ViewData["userobj"] = user;
            ViewData["requestlist"] = requestlist;
            return View();
        }


        public ActionResult editdeptinfo(string id)
        {
            Employee user = (Employee)Session["user"];
            Employee hod;
            Employee current_rep;
            DepartmentList dept;
            List<Employee> emp_list;
            List<CollectionPoint> coll_list;
            
            using (ModelDBContext db = new ModelDBContext())
            {
                hod = db.Employees.Find(user.Id);
                current_rep = db.Employees.Where(e => e.Designation == "Rep" && e.DepartmentCode==user.DepartmentCode).FirstOrDefault();
                
                if (current_rep == null)
                {
                    current_rep = hod;
                }
                dept = (DepartmentList)db.DepartmentLists.Where(d => d.DepartmentCode == hod.DepartmentCode).FirstOrDefault();
                emp_list = db.Employees.Where(e => e.DepartmentCode == hod.DepartmentCode).ToList();
                coll_list = db.CollectionPoints.ToList();
            }

            ViewData["current_rep"] = current_rep;
            ViewData["coll_list"] = coll_list;
            ViewData["hod"] = hod;
            ViewData["dept"] = dept;
            ViewData["emp_list"] = emp_list;

            return View();
        }

        [HttpPost]
        public ActionResult updatedeptinfo(string rep_id,string coll_point,string current_dept_code)
        {
            DepartmentList dept_to_update_coll_point;
            Employee rep;
            Employee current_rep;
            using (ModelDBContext db = new ModelDBContext())
            {
                rep = db.Employees.Find(rep_id);
                

                current_rep = db.Employees.Where(e => e.Designation == "Rep" && e.DepartmentCode == rep.DepartmentCode).FirstOrDefault();
                if (current_rep!=null)
                {
                    current_rep.Designation = "Employee";
                }

                dept_to_update_coll_point = db.DepartmentLists.Find(current_dept_code);

                if (rep.Designation != "Head")
                {
                    rep.Designation = "Rep";
                }
         
                dept_to_update_coll_point.CollectionPoint = coll_point;

                db.SaveChanges();
            }

            Debug.WriteLine(dept_to_update_coll_point.DepartmentName+ rep.FirstName+current_dept_code);
            return View("Index");
        }

        public ActionResult assigndelegate(string id)
        {
            Employee user = (Employee)Session["user"];
            Employee delegate_;
            List<Employee> dept_emp_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                delegate_ = db.Employees.Where(e => e.Designation == "Delegate" && e.DepartmentCode == user.DepartmentCode).FirstOrDefault();
                dept_emp_list = db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode).ToList();
            }

            ViewData["delegate_"] = (delegate_ != null ) ? delegate_ : user;
            ViewData["dept_emp_list"] = dept_emp_list;

            return View();
        }

        public ActionResult edit_delegate(string new_delegate_id, string from_date, string to_date)
        {
            Debug.WriteLine(new_delegate_id + from_date + to_date);
            Employee delegate_;
            Employee previous_delegate;
            using (ModelDBContext db = new ModelDBContext())
            {
                
                delegate_ = db.Employees.Where(e => e.Id == new_delegate_id).FirstOrDefault();
                previous_delegate = db.Employees.Where(e => e.DepartmentCode == delegate_.DepartmentCode && e.Designation == "Delegate").FirstOrDefault();
                if (previous_delegate != null)
                {
                    previous_delegate.Designation = "Employee";
                    previous_delegate.DelegateFrom = null;
                    previous_delegate.DelegateTo = null;
                }
                if (delegate_.Designation != "Head")
                {
                    if (Convert.ToDateTime(from_date) > Convert.ToDateTime(to_date))
                    {
                        return RedirectToAction("assigndelegate", new { msg = "invalid date" });
                    }

                    delegate_.Designation = "Delegate";
                    delegate_.DelegateFrom = Convert.ToDateTime(from_date);
                    delegate_.DelegateTo = Convert.ToDateTime(to_date);
                }
                else
                {
                    delegate_.DelegateFrom = DateTime.Now;
                    delegate_.DelegateTo = DateTime.Now;
                }
                db.SaveChanges();
            }
            return View("index");
        }
    }
}