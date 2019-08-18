using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using stationeryapp.Models;

namespace stationeryapp.Controllers
{
    public class LoginController : Controller
    {
        private ModelDBContext db = new ModelDBContext();
        ModelDBContext db1 = new ModelDBContext();

        // GET: Login/Edit/5
        public ActionResult Login()
        {
            return View();
        }

  

        [HttpPost]
        public JsonResult SetPerson(StoreClerk storeClerk)
        {
            PurchaseOrdersController purchaseOrdersController = new PurchaseOrdersController();
            purchaseOrdersController.SystemGeneratePO();

            StationeryRetrievalFormsController stationeryRetrievalFormsController = new StationeryRetrievalFormsController();
            stationeryRetrievalFormsController.GenerateRetrievalForm();
            stationeryRetrievalFormsController.MergeRetrievalForms();
            StoreClerk storeclerk = db1.StoreClerks.Where(p => p.UserName == storeClerk.UserName).FirstOrDefault();
            string hashedPassword = CalculateMD5Hash(storeClerk.Password);
            if (ModelState.IsValid)
            {
                if (storeclerk!=null &&  storeclerk.Password == hashedPassword)
                {
                    string sessionId = Guid.NewGuid().ToString();
                    storeclerk.Password = hashedPassword;
                    storeclerk.SessionId = sessionId;
                    db1.Entry(storeclerk).State = EntityState.Modified;
                    db1.SaveChanges();
                    return Json(new { sessionId=storeclerk.SessionId,username=storeclerk.UserName,status="success" });
                }
                else
                {
                    return Json(new { status="fail"});
                }
            }
            else
            {
                return Json(new { status = "fail" });
            }
        }


        //for storeMangerLogin
        public ActionResult LoginStoreManager ()
        {
            return View();
        }
        //for storeSupervisor Login
        public ActionResult LoginStoreSupervisor()
        {
            return View();
        }


        public ActionResult EmployeeIndex(String msg)
        {
            Employee logged_user;

            if (Session["user"] != null)
            {
                logged_user = db1.Employees.Find(((Employee)Session["user"]).Id);
                logged_user.SessionId = null;
                db1.Entry(logged_user).State = EntityState.Modified;
                db1.SaveChanges();
                Session.Remove("user");
                Session.Remove("sid");
            }
            ViewData["msg"] = msg;
            return View();
        }

        private string CalculateMD5Hash(string Password)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(Password);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        // POST: Login/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName,Password")] StoreClerk storeClerk)
        {
            if (storeClerk.UserName != null && storeClerk.Password!=null)
            {
                StoreClerk storeclerk = db1.StoreClerks.Where(p => p.UserName == storeClerk.UserName).FirstOrDefault();
                string hashedPassword = CalculateMD5Hash(storeClerk.Password);
                if (ModelState.IsValid)
                {
                    if (storeclerk.Password == hashedPassword)
                    {
                        string sessionId = Guid.NewGuid().ToString();
                        storeclerk.Password = hashedPassword;
                        storeclerk.SessionId = sessionId;
                        db1.Entry(storeclerk).State = EntityState.Modified;
                        db1.SaveChanges();
                        ViewData["tag"] = "storeClerk";
                        return RedirectToAction("Index", "Home", new { storeclerk.SessionId, tag = "storeClerk"});
                    }
                    else
                    {
                        return View();
                    }        
                }
                else
                {
                    return View();
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreManagerLogin([Bind(Include = "UserName,Password")] StoreManager storeManager)
        {
            if (storeManager.UserName != null && storeManager.Password != null)
            {
                StoreManager storeManager1 = db1.StoreManagers.Where(p => p.UserName == storeManager.UserName).FirstOrDefault();
                string hashedPassword = CalculateMD5Hash(storeManager.Password);
                if (ModelState.IsValid)
                {
                    if (storeManager1.Password == hashedPassword)
                    {
                        string sessionId = Guid.NewGuid().ToString();
                        storeManager1.Password = hashedPassword;
                        storeManager1.SessionId = sessionId;
                        db1.Entry(storeManager1).State = EntityState.Modified;
                        db1.SaveChanges();
                        return RedirectToAction("Index", "StoreManagers", new { storeManager1.SessionId, tag= "storeManager" });
                    }
                    else
                    {
                        return RedirectToAction("LoginStoreManager", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("LoginStoreManager", "Login");
                }
            }
            return RedirectToAction("LoginStoreManager", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreSupervisorLogin([Bind(Include = "UserName,Password")] StoreSupervisor storeSupervisor)
        {
            if (storeSupervisor.UserName != null && storeSupervisor.Password != null)
            {
                StoreSupervisor storeSupervisor1 = db1.StoreSupervisors.Where(p => p.UserName == storeSupervisor.UserName).FirstOrDefault();
                string hashedPassword = CalculateMD5Hash(storeSupervisor.Password);
                if (ModelState.IsValid)
                {
                    if (storeSupervisor1.Password == hashedPassword)
                    {
                        string sessionId = Guid.NewGuid().ToString();
                        storeSupervisor1.Password = hashedPassword;
                        storeSupervisor1.SessionId = sessionId;
                        db1.Entry(storeSupervisor1).State = EntityState.Modified;
                        db1.SaveChanges();
                        return RedirectToAction("Index", "StoreSupervisors", new { storeSupervisor1.SessionId, tag = "storeSupervisor" });
                    }
                    else
                    {
                        return RedirectToAction("LoginStoreSupervisor", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("LoginStoreSupervisor", "Login");
                }
            }
            return RedirectToAction("LoginStoreSupervisor", "Login");
        }




        [HttpPost]
        public ActionResult EmployeeLogin([Bind(Include = "UserName,Password")] Employee emp)
        {
            Employee user;
            string hashedPassword = CalculateMD5Hash(emp.Password);

            user = (from e in db1.Employees
                    where e.UserName == emp.UserName
                    select e).FirstOrDefault();

            if (user != null)
            {
                Debug.WriteLine(user.SessionId == null);

                if (user.SessionId != null)
                {
                    Debug.WriteLine(user.SessionId.Split('@')[1]);
                    DateTime date1 = Convert.ToDateTime(Convert.ToString(user.SessionId.Split('@')[1]));
                    DateTime date2 = DateTime.Now;
                    TimeSpan ts = date2 - date1;
                    Debug.WriteLine("No. of Minutes (Difference) = {0}", ts.TotalMinutes);

                    if(ts.TotalMinutes > 2)
                    {
                        using (ModelDBContext db1 = new ModelDBContext())
                        {
                            Employee e = db1.Employees.Where(ee => ee.UserName == emp.UserName).First();
                            e.SessionId = null;
                            db1.Entry(e).State = EntityState.Modified;
                            db1.SaveChanges();
                        }
                    }

                    return RedirectToAction("EmployeeIndex", new { msg = "Another session in progress. Try to logout First." });
                }
                if (hashedPassword.Equals(user.Password))
                {
                    if (user.Designation == "Head")
                    {
                        Session.Add("user", user);
                        Session.Add("count", 0);
                        string sessionId = Guid.NewGuid().ToString()+"@"+ DateTime.Now.ToString();
                        Session.Add("sid", sessionId);
                        user.SessionId = sessionId;
                        db1.Entry(user).State = EntityState.Modified;
                        db1.SaveChanges();

                        return RedirectToAction("Index", "Hod", new { sid  = sessionId});
                    }
                    else if (user.Designation == "Delegate")
                    {
                        int result = DateTime.Compare( Convert.ToDateTime(user.DelegateFrom).Date, DateTime.Now.Date);
                        Debug.WriteLine(DateTime.Now.Date+"todays date"+ Convert.ToDateTime(user.DelegateFrom).Date + " vs delegate from :" + result);                        
                        int result_ = DateTime.Compare(Convert.ToDateTime(user.DelegateTo).Date, DateTime.Now.Date);
                        Debug.WriteLine(DateTime.Now.Date + "todays date"+ Convert.ToDateTime(user.DelegateTo).Date + " vs delegate to :" + result_);
                        if ( ( result<0 || result == 0)  && (result_==0 || result_>0) )
                        {
                            Session.Add("user", user);
                            Session.Add("count", 0);
                            string sessionId = Guid.NewGuid().ToString() + "@" + DateTime.Now.ToString();
                            Session.Add("sid", sessionId);
                            user.SessionId = sessionId;
                            db1.Entry(user).State = EntityState.Modified;
                            db1.SaveChanges();

                            return RedirectToAction("Index", "Hod", new { sid = sessionId });
                        }
                        else
                        {
                            return RedirectToAction("EmployeeIndex", new { msg = "Delegate period over. Contact Head to Reset" });
                        }
                    }
                    else if (user.Designation == "Employee" || user.Designation == "Rep")
                    {
                        Session.Add("user", user);
                        Session.Add("count", 0);
                        string sessionId = Guid.NewGuid().ToString() + "@" + DateTime.Now.ToString();
                        Session.Add("sid", sessionId);
                        user.SessionId = sessionId;
                        db1.Entry(user).State = EntityState.Modified;
                        db1.SaveChanges();
                        return RedirectToAction("Index", "Employee", new { sid = sessionId });
                    }
                }
                else
                {
                    return RedirectToAction("EmployeeIndex", new { msg = "Invalid Password" });
                }
            }
            return RedirectToAction("EmployeeIndex", new { msg = "UserName not found. Try again" });
        }

        public ActionResult Logout(string sessionId)
        {

            StoreClerk storeclerk = db1.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreManager storeManager = db1.StoreManagers.Where(p => p.SessionId == sessionId).FirstOrDefault();
            StoreSupervisor storeSupervisor = db1.StoreSupervisors.Where(p => p.SessionId == sessionId).FirstOrDefault();
            string newsessionId = Guid.NewGuid().ToString();

            if (storeclerk != null && sessionId != null)
            {
                storeclerk.SessionId = newsessionId;
                db1.Entry(storeclerk).State = EntityState.Modified;
                db1.SaveChanges();
                return RedirectToAction("Login");
            }else if (storeManager!=null&& sessionId != null)
            {
                storeManager.SessionId = newsessionId;
                db1.Entry(storeManager).State = EntityState.Modified;
                db1.SaveChanges();
                return RedirectToAction("LoginStoreManager","Login");
            }
            else
            {
                storeSupervisor.SessionId = newsessionId;
                db1.Entry(storeSupervisor).State = EntityState.Modified;
                db1.SaveChanges();
                return RedirectToAction("LoginStoreSupervisor", "Login");
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db1.Dispose();
            }
            base.Dispose(disposing);
        }


        public JsonResult login_android(string Username, string Password)
        {
            Employee user;
            string hashedPassword = CalculateMD5Hash(Password).ToUpper();

            using (ModelDBContext db = new ModelDBContext())
            {
                user = (from e in db.Employees
                        where (e.UserName == Username && e.Designation == "Head") || (e.UserName == Username && e.Designation == "Delegate")
                        select e).FirstOrDefault();
            }
            if (user != null)
            {
                Debug.WriteLine("User is found..........");
                Debug.WriteLine(user.FirstName + user.Designation);

                if (user.Designation == "Delegate")
                {
                    int result = DateTime.Compare(Convert.ToDateTime(user.DelegateFrom).Date, DateTime.Now.Date);
                    Debug.WriteLine(DateTime.Now.Date + "todays date" + Convert.ToDateTime(user.DelegateFrom).Date + " vs delegate from :" + result);
                    int result_ = DateTime.Compare(Convert.ToDateTime(user.DelegateTo).Date, DateTime.Now.Date);
                    Debug.WriteLine(DateTime.Now.Date + "todays date" + Convert.ToDateTime(user.DelegateTo).Date + " vs delegate to :" + result_);
                    if ((result < 0 || result == 0) && (result_ == 0 || result_ > 0))
                    {
                        if (hashedPassword.Equals(user.Password))
                        {
                            Debug.WriteLine(Password);

                            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(emp), JsonRequestBehavior.AllowGet);
                            //return Json(user, JsonRequestBehavior.AllowGet);
                            //return Json(new { data= "ok"+"/"+user.Id+"/"+user.FirstName+"/"+user.LastName+"/"+user.DepartmentCode+"/"+user.Designation+"/"+user.EmailAddress }, JsonRequestBehavior.AllowGet);
                            return Json(new { data = new { status = "ok", user_id = user.Id, user_name = user.FirstName + " " + user.LastName } }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { data = new { status = "Invalid Password" } }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { data = new { status = "Invalid delegate" } }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (hashedPassword.Equals(user.Password))
                    {
                        Debug.WriteLine(Password);

                        //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(emp), JsonRequestBehavior.AllowGet);
                        //return Json(user, JsonRequestBehavior.AllowGet);
                        //return Json(new { data= "ok"+"/"+user.Id+"/"+user.FirstName+"/"+user.LastName+"/"+user.DepartmentCode+"/"+user.Designation+"/"+user.EmailAddress }, JsonRequestBehavior.AllowGet);
                        return Json(new { data = new { status = "ok", user_id = user.Id, user_name = user.FirstName + " " + user.LastName } }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = new { status = "Invalid Password" } }, JsonRequestBehavior.AllowGet);
                    }
                }      
            }
            return Json(new { data = new { status = "Invalid User" } }, JsonRequestBehavior.AllowGet);

        }
    }
}
