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

        // GET: Login/Edit/5
        public ActionResult Login()
        {
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
                StoreClerk storeclerk = db.StoreClerks.Where(p => p.UserName == storeClerk.UserName).First();
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

        public ActionResult Logout(string sessionId)
        {
            StoreClerk storeclerk = db.StoreClerks.Where(p => p.SessionId == sessionId).First();
            storeclerk.SessionId = null;
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
