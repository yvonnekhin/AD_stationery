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
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.UserName == storeClerk.UserName).FirstOrDefault();
            string hashedPassword = CalculateMD5Hash(storeClerk.Password);
            if (ModelState.IsValid)
            {
                if (storeclerk.Password == hashedPassword)
                {
                    string sessionId = Guid.NewGuid().ToString();
                    storeclerk.Password = hashedPassword;
                    storeclerk.SessionId = sessionId;
                    db.Entry(storeclerk).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { sessionId=storeclerk.SessionId,username=storeclerk.UserName });
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
                StoreClerk storeclerk = db.StoreClerks.Where(p => p.UserName == storeClerk.UserName).FirstOrDefault();
                string hashedPassword = CalculateMD5Hash(storeClerk.Password);
                if (ModelState.IsValid)
                {
                    if (storeclerk.Password == hashedPassword)
                    {
                        string sessionId = Guid.NewGuid().ToString();
                        storeclerk.Password = hashedPassword;
                        storeclerk.SessionId = sessionId;
                        db.Entry(storeclerk).State = EntityState.Modified;
                        db.SaveChanges();
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
                if (hashedPassword.Equals(user.Password))
                {
                    if (user.Designation == "Head" || user.Designation == "Delegate")
                    {
                        Session.Add("user", user);
                        Session.Add("count", 0);
                        string sessionId = Guid.NewGuid().ToString();
                        user.SessionId = sessionId;
                        db1.Entry(user).State = EntityState.Modified;
                        db1.SaveChanges();

                        return RedirectToAction("Index", "Hod");
                    }
                    else if (user.Designation == "Employee" || user.Designation == "Rep")
                    {
                        Session.Add("user", user);
                        Session.Add("count", 0);
                        string sessionId = Guid.NewGuid().ToString();
                        user.SessionId = sessionId;
                        db1.Entry(user).State = EntityState.Modified;
                        db1.SaveChanges();
                        return RedirectToAction("Index", "Employee");
                    }
                }
                else
                {
                    return RedirectToAction("EmployeeIndex", new { msg = "invalid password" });
                }
            }
            return RedirectToAction("EmployeeIndex", new { msg = "invalid user" });
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
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
