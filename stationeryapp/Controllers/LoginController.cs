using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
        private StoreClerkDBContext db = new StoreClerkDBContext();
        ModelDBContext db1 = new ModelDBContext();

        // GET: Login/Edit/5
        public ActionResult Login()
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
                        return RedirectToAction("Index", "Home", new { storeclerk.SessionId });
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
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).FirstOrDefault();
            string newsessionId = Guid.NewGuid().ToString();
            storeclerk.SessionId = newsessionId;
            db.Entry(storeclerk).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Login");
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
