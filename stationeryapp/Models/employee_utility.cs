using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class employee_utility
    {
        public void delete_temp_request_forms(Employee user)
        {
            using (ModelDBContext db = new ModelDBContext())
            {
                db.RequisitionForms.RemoveRange(db.RequisitionForms.Where(x => x.Status == "temp_pending"+user.Id).ToList());
                db.SaveChanges();
            }
        }
        public Employee get_employee(Employee user)
        {
            Employee emp;
            using (ModelDBContext db = new ModelDBContext())
            {
                emp = db.Employees.Find(user.Id);
            }
            return emp;
        }
        public DepartmentList get_department(Employee user)
        {
            DepartmentList dept;
            using (ModelDBContext db = new ModelDBContext())
            {
                dept = db.DepartmentLists.Find(user.DepartmentCode);
            }
            return dept;
        }
        public List<StationeryCatalog> get_catalog_items()
        {
            List<StationeryCatalog> items; 
            using (ModelDBContext db = new ModelDBContext())
            {
                items = db.StationeryCatalogs.ToList();
            }
            return items;
        }
        public string get_form_no()
        {
            string form_number;
            using (ModelDBContext db = new ModelDBContext())
            {
                if (db.RequisitionForms.AsEnumerable().ToList().Count != 0)
                {
                    form_number = db.RequisitionForms.AsEnumerable().Max(p => long.Parse(p.FormNumber)).ToString();
                    form_number = (Convert.ToInt32(form_number) + 1).ToString();
                }
                else
                {
                    form_number = "1";
                }
            }
            return form_number;
        }
        public string get_form_no_()
        {
            string form_number;
            using (ModelDBContext db = new ModelDBContext())
            {
                form_number = db.RequisitionForms.AsEnumerable().Max(p => long.Parse(p.FormNumber)).ToString();
            }
            return form_number;
        }
        public void add_newrequestform(string form_number, Employee emp)
        {
            using (ModelDBContext db = new ModelDBContext())
            {
                db.RequisitionForms.Add(new RequisitionForm { FormNumber = form_number, EmployeeId = emp.Id, Status = "temp_pending"+ emp.Id });
                db.SaveChanges();
            }
        }
        public List<RequisitionForm> get_req_forms(Employee user)
        {
            List<RequisitionForm> reqfrorms;
            using (ModelDBContext db = new ModelDBContext())
            {
                reqfrorms = db.RequisitionForms.Where(x => x.EmployeeId == user.Id && x.Status != "temp_pending"+user.Id).OrderByDescending(f=>f.DateReceived).ToList();
            }
            return reqfrorms;
        }
        public List<RequisitionFormDetail> get_RequisitionFormDetails(string form_number)
        {
            List<RequisitionFormDetail> temp;
            using (ModelDBContext db = new ModelDBContext())
            {
                temp = db.RequisitionFormDetails.Where(x => x.FormNumber == form_number).ToList();

            }
            return temp;
        }
        public void add_new_form_detail_row(RequisitionFormDetail form_detail_obj)
        {
            using (ModelDBContext db = new ModelDBContext())
            {
                List<RequisitionFormDetail> temp = db.RequisitionFormDetails.ToList();
                
                string form_details_num = (db.RequisitionFormDetails.AsEnumerable().ToList().Count == 0) ? "1" : (db.RequisitionFormDetails.AsEnumerable().Max(p => long.Parse(p.FormDetailsNumber)) + 1).ToString();

                RequisitionFormDetail old_rec_to_update = (from f in db.RequisitionFormDetails
                                                           where f.ItemNumber == form_detail_obj.ItemNumber && f.FormNumber == form_detail_obj.FormNumber
                                                           select f).FirstOrDefault();
                if (old_rec_to_update != null)
                {
                    old_rec_to_update.Quantity += form_detail_obj.Quantity;
                }
                else
                {
                    
                    db.RequisitionFormDetails.Add(new RequisitionFormDetail { FormDetailsNumber = form_details_num, FormNumber = form_detail_obj.FormNumber, ItemNumber = form_detail_obj.ItemNumber, Quantity = Convert.ToInt32(form_detail_obj.Quantity), Status="Pending" });
                }

                db.SaveChanges();
            }
        }

        public void submit_request(string emp_id, string form_number)
        {
            List<Employee> emp_list;
            Employee hod;
            RequisitionForm old_rec_to_update;
            using (ModelDBContext db = new ModelDBContext())
            {
                old_rec_to_update = (from f in db.RequisitionForms
                                                     where f.EmployeeId == emp_id && f.FormNumber == form_number && f.Status == "temp_pending" + emp_id
                                                     select f).FirstOrDefault();
                if (old_rec_to_update != null)
                {
                    old_rec_to_update.Notification = "sent_to_hod";
                    old_rec_to_update.DateReceived = DateTime.Now.Date;
                    old_rec_to_update.Status = "Pending";
                }
                db.SaveChanges();
            }
            RequisitionForm form_;
            using (ModelDBContext db = new ModelDBContext())
            {
                emp_list = db.Employees.ToList();
                form_ = db.RequisitionForms.Find(form_number);
            }
            //Debug.WriteLine("*************************" + form_.EmployeeId+ emp_list.Where(e => e.Id == form_.EmployeeId).Select(e => e.DepartmentCode).FirstOrDefault());
            hod = emp_list.Where(e => e.Designation == "Head" && e.DepartmentCode == emp_list.Where(emp => emp.Id == form_.EmployeeId).Select(emp => emp.DepartmentCode).FirstOrDefault()).FirstOrDefault();
            util.SendEmail(hod.EmailAddress, "New request has been received", "Request FORM : "+form_number+ " has been requested");
        }

        public RequisitionForm get_single_form_details(string form_id)
        {
            RequisitionForm form;
            using (ModelDBContext db = new ModelDBContext())
            {

                form = db.RequisitionForms.Find(form_id);
            }
            return form;
        }
    }
}