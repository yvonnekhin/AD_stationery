using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using stationeryapp.Filters;
using stationeryapp.Models;
namespace stationeryapp.Controllers
{
    public class HodController : Controller
    {
        // GET: Hod
        [empfilter]
        public ActionResult Index(string sid)
        {
            Employee user = (Employee)Session["user"];
         
            int count;
            List<RequisitionForm> notify_form_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                notify_form_list = db.RequisitionForms.Where(row => (db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode).Select(f => f.Id).ToList().Contains(row.EmployeeId) && row.Notification == "sent_to_hod")).ToList();
                count = db.RequisitionForms.Where(row => (db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode).Select(f => f.Id).ToList().Contains(row.EmployeeId) && row.Notification == "sent_to_hod")).Count();
            }
            Session["count"] = count;
            Session["notify_form_list"] = notify_form_list;
            ViewData["sid"] = sid;
            return View();
        }

        [empfilter]
        public ActionResult req_forms(string form_id, string notify, string sid)
        {
            if (notify=="seen")
            {
                using (ModelDBContext db = new ModelDBContext())
                {
                    db.RequisitionForms.Where(f => f.FormNumber == form_id).FirstOrDefault().Notification = "seen_by_hod";
                    db.SaveChanges();
                }
            }
            Employee user = (Employee)Session["user"];
            int count;
            List<RequisitionForm> notify_form_list;
           
            using (ModelDBContext db = new ModelDBContext())
            {
                notify_form_list = db.RequisitionForms.Where(row => (db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode).Select(f => f.Id).ToList().Contains(row.EmployeeId) && row.Notification == "sent_to_hod")).ToList();
                count = db.RequisitionForms.Where(row => (db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode).Select(f => f.Id).ToList().Contains(row.EmployeeId) && row.Notification == "sent_to_hod")).Count();
            }
            Session["count"] = count;
            Session["notify_form_list"] = notify_form_list;

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
            ViewData["sid"] = sid;
            if (form.Status=="Pending")
            {
                ViewData["req_pending_forms"] = true;
            }
            ViewData["req_form"] = form;

            return View();
        }

        [empfilter]
        public ActionResult approve_reject(string form_id, string form_status, string comment, string approved_by, string sid)
        {
            Debug.WriteLine(form_id+form_status+comment);
            string notify_status="";
            string email_status = "";
            if (form_status.Equals("APPROVE"))
            {
                form_status = "Approved";
                notify_status = "approved_by_hod";
                email_status = "Approved By HOD";
            }
            else
            {
                form_status = "Rejected";
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
                util.SendEmail(db.Employees.Find(form_.EmployeeId).EmailAddress, "Your request status has been updated", form_.FormNumber + " " + email_status);
            }
                

            return RedirectToAction("Index", "Hod", new { sid = sid});
        }

        [empfilter]
        public ActionResult newstationaryrequest(string sid)
        {
            Employee user = (Employee)Session["user"];
            List<RequisitionForm> requestlist;
            List<Employee> emp_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                emp_list = db.Employees.ToList();
                requestlist = db.RequisitionForms.Where(f => db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode).Select(e => e.Id).Contains(f.EmployeeId) && (f.Status == "Pending")).OrderByDescending(f=>f.DateReceived).ToList();
            }
            ViewData["emp_list"] = emp_list;
            ViewData["userobj"] = user;
            ViewData["requestlist"] = requestlist;
            ViewData["from_pending_reqs"] = true;
            ViewData["sid"] = sid;

            return View("stationaryrequesthistory");
        }

        [empfilter]
        public ActionResult stationaryrequesthistory(string sid)
        {
            Employee user = (Employee)Session["user"];
            List<RequisitionForm> requestlist;
            List<Employee> emp_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                emp_list = db.Employees.ToList();
                requestlist = db.RequisitionForms.Where( f => db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode).Select(e=>e.Id).Contains(f.EmployeeId) && (f.Status=="Approved" || f.Status=="Rejected" || f.Status == "Read")).OrderByDescending(f => f.DateReceived).ToList();
            }
            ViewData["emp_list"] = emp_list;
            ViewData["userobj"] = user;
            ViewData["requestlist"] = requestlist;
            ViewData["sid"] = sid;
            return View();
        }

        [empfilter]
        public ActionResult editdeptinfo(string id, string sid)
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
                //current_rep = db.Employees.Where(e => e.Designation == "Rep" && e.DepartmentCode==user.DepartmentCode).FirstOrDefault();

                //if (current_rep == null)
                //{
                //    current_rep = hod;
                //}
                
                dept = (DepartmentList)db.DepartmentLists.Where(d => d.DepartmentCode == hod.DepartmentCode).FirstOrDefault();
                current_rep = db.Employees.Where(e => e.Id == db.DepartmentLists.Where(d => d.DepartmentCode == hod.DepartmentCode).FirstOrDefault().RepresentativeId).First();
                emp_list = db.Employees.Where(e => e.DepartmentCode == hod.DepartmentCode && e.Designation!="Delegate").ToList();
                coll_list = db.CollectionPoints.ToList();
            }

            ViewData["current_rep"] = current_rep;
            ViewData["coll_list"] = coll_list;
            ViewData["hod"] = hod;
            ViewData["dept"] = dept;
            ViewData["emp_list"] = emp_list;
            ViewData["sid"] = sid;

            return View();
        }

        [HttpPost]
        [empfilter]
        public ActionResult updatedeptinfo(string rep_id,string coll_point,string current_dept_code, string sid)
        {
            DepartmentList dept_to_update_coll_point;
            Employee rep;
            Employee current_rep;
            using (ModelDBContext db = new ModelDBContext())
            {
                rep = db.Employees.Find(rep_id);


                //current_rep = db.Employees.Where(e => e.Designation == "Rep" && e.DepartmentCode == rep.DepartmentCode).FirstOrDefault();
                //current_rep = db.Employees.Where(e => e.Id == db.DepartmentLists.Where(d => d.DepartmentCode == current_dept_code).FirstOrDefault().RepresentativeId).First();
                //if (current_rep!=null)
                //{
                //    current_rep.Designation = "Employee";
                //}

                dept_to_update_coll_point = db.DepartmentLists.Find(current_dept_code);

                //if (rep.Designation != "Head")
                //{
                //    rep.Designation = "Rep";
                //}
         
                dept_to_update_coll_point.CollectionPoint = coll_point;
                dept_to_update_coll_point.RepresentativeId = rep_id;

                db.SaveChanges();
            }

            Debug.WriteLine(dept_to_update_coll_point.DepartmentName+ rep.FirstName+current_dept_code);
            //return View("Index", new { sid = sid});
            return RedirectToAction("editdeptinfo", new { sid = sid });
        }

        [empfilter]
        public ActionResult assigndelegate(string id, string sid)
        {
            Employee user = (Employee)Session["user"];
            Employee delegate_;
            List<Employee> dept_emp_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                delegate_ = db.Employees.Where(e => e.Designation == "Delegate" && e.DepartmentCode == user.DepartmentCode).FirstOrDefault();
                dept_emp_list = db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode  && e.Id != db.DepartmentLists.Where(d=>d.DepartmentCode==user.DepartmentCode).FirstOrDefault().RepresentativeId).ToList();
            }

            ViewData["delegate_"] = (delegate_ != null ) ? delegate_ : user;
            ViewData["dept_emp_list"] = dept_emp_list;
            ViewData["sid"] = sid;

            return View();
        }

        [empfilter]
        [HttpPost]
        public ActionResult edit_delegate(string new_delegate_id, string from_date, string to_date, string sid)
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
                    //if (Convert.ToDateTime(from_date) > Convert.ToDateTime(to_date))
                    //{
                    //    return RedirectToAction("assigndelegate", new { msg = "invalid date" });
                    //}

                    delegate_.Designation = "Delegate";
                    delegate_.DelegateFrom = Convert.ToDateTime(from_date);
                    delegate_.DelegateTo = Convert.ToDateTime(to_date);
                }
                //else
                //{
                //    delegate_.DelegateFrom = DateTime.Now;
                //    delegate_.DelegateTo = DateTime.Now;
                //}
                db.SaveChanges();
            }
            return View("index", new { sid=sid});
        }

        [empfilter]
        public ActionResult remove_delegate(string hod_id, string sid)
        {
            Employee hod;
            Employee previous_delegate;
            using (ModelDBContext db = new ModelDBContext())
            {
                hod = db.Employees.Find(hod_id);
                previous_delegate = db.Employees.Where(e => e.DepartmentCode == hod.DepartmentCode && e.Designation == "Delegate").FirstOrDefault();

                Debug.WriteLine("prevoius delegate " + previous_delegate.ToString());

                if (previous_delegate != null)
                {
                    previous_delegate.Designation = "Employee";
                    previous_delegate.DelegateFrom = null;
                    previous_delegate.DelegateTo = null;
                }
                db.SaveChanges();
            }
            return RedirectToAction("assigndelegate", new { sid=sid});
        }

        public ActionResult viewhistorybyitem( string sid)
        {
            Employee user = (Employee)Session["user"];

            Dictionary<string, int> res_ = new Dictionary<string, int>();

            using (ModelDBContext db = new ModelDBContext())
            {
                //select ItemNumber, sum(Quantity) from RequisitionFormDetails where FormNumber in (select r.FormNumber from RequisitionForms r where r.EmployeeId in (select Id from Employee where DepartmentCode = 'CPSC') and r.Status = 'approved') group by  RequisitionFormDetails.ItemNumber

                List<RequisitionFormDetail> lists = db.RequisitionFormDetails.Where(formd => (db.RequisitionForms.Where(rf => db.Employees.Where(emp => emp.DepartmentCode == user.DepartmentCode).Select(e => e.Id).ToList().Contains(rf.EmployeeId) && (rf.Status == "Approved" || rf.Status=="Read")).Select(fff => fff.FormNumber).ToList()).Contains(formd.FormNumber)).ToList();


                foreach (var item in lists)
                {
                    if (res_.ContainsKey(item.ItemNumber))
                    {
                        res_[item.ItemNumber] = Convert.ToInt32(res_[item.ItemNumber]) + Convert.ToInt32(item.Quantity);
                    }
                    else
                    {
                        res_[db.StationeryCatalogs.Where(c => c.ItemNumber == item.ItemNumber).Select(e => e.Description).First()] = Convert.ToInt32(item.Quantity);
                    }

                }
            };
            ViewData["res_"] = res_;
            ViewData["sid"] = sid;
            return View();
        }

        public JsonResult GetPendingReq(string user_id)
        {
            List<RequisitionForm> requestlist;
            List<Employee> emp_list;
            Employee hod;
            using (ModelDBContext db = new ModelDBContext())
            {
                hod = db.Employees.Find(user_id);
                emp_list = db.Employees.ToList();
                requestlist = db.RequisitionForms.Where(f => db.Employees.Where(e => e.DepartmentCode == hod.DepartmentCode).Select(e => e.Id).Contains(f.EmployeeId) && (f.Status == "Pending")).OrderByDescending(f => f.DateReceived).ToList();
            }
            return Json(new { data = requestlist.Select(item => new { Status = item.Status, EmployeeId = item.EmployeeId, EmployeeName = emp_list.Where(e => e.Id == item.EmployeeId).Select(e => e.FirstName + " " + e.LastName).First().ToString(), FormNumber = item.FormNumber }) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReqformDetails(string form_id)
        {
            List<RequisitionFormDetail> form_list;
            List<Employee> emp_list;
            Employee emp;
            using (ModelDBContext db = new ModelDBContext())
            {
                emp_list = db.Employees.ToList();
                emp = emp_list.Where(e => e.Id == (db.RequisitionForms.Find(form_id).EmployeeId)).First();
                form_list = db.RequisitionFormDetails.Where(f => f.FormNumber == form_id).ToList();
            }
            return Json(new { data = form_list.Select(item => new { FormDetailsNumber = item.FormDetailsNumber, FormNumber = item.FormNumber, ItemNumber = item.ItemNumber, ItemDesc = new ModelDBContext().StationeryCatalogs.Where(s => s.ItemNumber == item.ItemNumber).Select(ss => ss.Description).First(), Quantity = item.Quantity, EmployeeName = emp.FirstName + " " + emp.LastName }) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDeptEmployees(string user_id)
        {
            List<Employee> emp_list;
            Employee hod;
            using (ModelDBContext db = new ModelDBContext())
            {
                hod = db.Employees.Find(user_id);
                emp_list = db.Employees.Where(e => e.DepartmentCode == hod.DepartmentCode).ToList();
            }
            return Json(new { data = emp_list.Select(item => new { EmployeeId = item.Id, EmployeeName = item.FirstName + " " + item.LastName }) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeptInfo(string user_id)
        {
            Employee hod;
            List<Employee> emp_list;
            DepartmentList dept;
            Employee rep;
            List<CollectionPoint> cp_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                cp_list = db.CollectionPoints.ToList();
                hod = db.Employees.Find(user_id);
                emp_list = db.Employees.Where(e => e.DepartmentCode == hod.DepartmentCode && e.Designation!="Delegate").ToList();
                dept = db.DepartmentLists.Where(d => d.DepartmentCode == hod.DepartmentCode).First();
                rep = db.Employees.Where(e => e.Id == dept.RepresentativeId).First();
            }
            return Json(new { data = new { collectionptname = cp_list.Where(c => c.CollectionPointCode == dept.CollectionPoint).Select(c => c.CollectionPointName).First(), collectionpt = dept.CollectionPoint, collectionrep = dept.RepresentativeId, collectionrepname = rep.FirstName + " " + rep.LastName }, data1 = emp_list.Select(item => new { EmployeeId = item.Id, EmployeeName = item.FirstName + " " + item.LastName }) }, JsonRequestBehavior.AllowGet);
        }

        public void UpdateFormDetails(string form_id, string comments, string status, string approved_by)
        {
            string notify_status;
            string email_status;
            Debug.WriteLine(form_id + comments + status + approved_by);
            RequisitionForm form;
            if (status.Equals("approved"))
            {
                status = "Approved";
                notify_status = "approved_by_hod";
                email_status = "Approved By HOD";
            }
            else
            {
                status = "Rejected";
                notify_status = "rejected_by_hod";
                email_status = "Rejected By HOD";
            }
            using (ModelDBContext db = new ModelDBContext())
            {
                form = db.RequisitionForms.Find(form_id);
                form.Comments = comments;
                form.Status = status;
                form.Notification = notify_status;
                form.ApprovedBy = approved_by;
                form.DateApproved = DateTime.Now.Date;
                db.SaveChanges();
            }
            RequisitionForm form_;
            using (ModelDBContext db = new ModelDBContext())
            {
                form_ = db.RequisitionForms.Find(form_id);
                util.SendEmail(db.Employees.Find(form_.EmployeeId).EmailAddress, "From Head Of Dept", form_.FormNumber + " " + email_status);
            }
        }

        public void DeptUpdate(string rep, string collectionpt, string hod_id)
        {
            Debug.WriteLine(rep + "   " + collectionpt + "   " + hod_id);
            Employee hod;
            DepartmentList dept;
            using (ModelDBContext db = new ModelDBContext())
            {
                hod = db.Employees.Find(hod_id);
                dept = db.DepartmentLists.Where(d => d.DepartmentCode == hod.DepartmentCode).First();
                dept.CollectionPoint = collectionpt;
                dept.RepresentativeId = rep;
                db.SaveChanges();
            }
        }

        public JsonResult GetDelegate(string user_id)
        {
            Employee delegate_;
            Employee hod;
            List<Employee> dept_emp_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                hod = db.Employees.Find(user_id);
                delegate_ = db.Employees.Where(e => e.Designation == "Delegate" && e.DepartmentCode == hod.DepartmentCode).FirstOrDefault();
                //dept_emp_list = db.Employees.Where(e => e.DepartmentCode == hod.DepartmentCode && e.Designation!="Rep").ToList();
                dept_emp_list = db.Employees.Where(e => e.DepartmentCode == hod.DepartmentCode && e.Id != db.DepartmentLists.Where(d => d.DepartmentCode == hod.DepartmentCode).FirstOrDefault().RepresentativeId).ToList();
            }

            Employee emp = (delegate_ != null) ? delegate_ : hod;
            return Json(new { data = new { current_delegateid = emp.Id, current_delegate = emp.FirstName + " " + emp.LastName, c_from = (emp.Designation == "Head") ? "" : Convert.ToDateTime(emp.DelegateFrom).Date.ToString("dd/MM/yyyy"), c_to = (emp.Designation == "Head") ? "" : Convert.ToDateTime(emp.DelegateTo).Date.ToString("dd/MM/yyyy") }, emp_list = dept_emp_list.Select(item => new { EmployeeId = item.Id, EmployeeName = item.FirstName + " " + item.LastName }) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult stationaryrequesthistory_android(string hod_id)
        {
            Employee user;
            List<RequisitionForm> requestlist;
            List<Employee> emp_list;
            using (ModelDBContext db = new ModelDBContext())
            {
                user = db.Employees.Find(hod_id);
                emp_list = db.Employees.ToList();
                requestlist = db.RequisitionForms.Where(f => db.Employees.Where(e => e.DepartmentCode == user.DepartmentCode).Select(e => e.Id).Contains(f.EmployeeId) && (f.Status == "Approved" || f.Status == "Rejected" || f.Status=="Read")).OrderByDescending(f => f.DateReceived).ToList();
            }
            return Json(new { data = requestlist.Select(l => new { formid = l.FormNumber, status = l.Status, employee = new ModelDBContext().Employees.Where(e => e.Id == l.EmployeeId).Select(e => e.FirstName + " " + e.LastName).First(), date = Convert.ToDateTime(l.DateReceived).Date.ToString("dd/MM/yyyy") }) }, JsonRequestBehavior.AllowGet);
        }

        public void DelegateUpdate(string emp_id, string hod_id, string from = "0001-01-01", string to = "0001-01-01")
        {
            Employee hod;
            Employee delegate_;
            Employee previous_delegate;
            using (ModelDBContext db = new ModelDBContext())
            {
                hod = db.Employees.Find(hod_id);
                delegate_ = db.Employees.Where(e => e.Id == emp_id).FirstOrDefault();
                previous_delegate = db.Employees.Where(e => e.DepartmentCode == hod.DepartmentCode && e.Designation == "Delegate").FirstOrDefault();

                if (previous_delegate != null)
                {
                    previous_delegate.Designation = "Employee";
                    previous_delegate.DelegateFrom = null;
                    previous_delegate.DelegateTo = null;
                }
                if (delegate_.Designation != "Head")
                {
                    delegate_.Designation = "Delegate";
                    delegate_.DelegateFrom = Convert.ToDateTime(from);
                    delegate_.DelegateTo = Convert.ToDateTime(to);
                }
                db.SaveChanges();
            }
        }

        public void Resetdelegate(string hod_id)
        {
            Employee hod;
            Employee previous_delegate;
            using (ModelDBContext db = new ModelDBContext())
            {
                hod = db.Employees.Find(hod_id);
                previous_delegate = db.Employees.Where(e => e.DepartmentCode == hod.DepartmentCode && e.Designation == "Delegate").FirstOrDefault();

                if (previous_delegate != null)
                {
                    previous_delegate.Designation = "Employee";
                    previous_delegate.DelegateFrom = null;
                    previous_delegate.DelegateTo = null;
                }
                db.SaveChanges();
            }
        }

        

    }
}