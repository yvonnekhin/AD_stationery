using stationeryapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace stationeryapp.Filters
{
    public class empfilter : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext context)
        {
            Employee emp = (Employee)HttpContext.Current.Session["user"];
            string sid = (string)HttpContext.Current.Session["sid"];
            //bool result = IsActiveuser(emp);
            if (!IsActiveuser_session(sid, emp))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Login" },
                        { "action", "EmployeeIndex" }
                    }
                );
            }

        }

        private bool IsActiveuser(Employee emp)
        {
            return emp == null ? false : true;
        }

        private bool IsActiveuser_session(string sid, Employee user)
        {
            //string ssid = (sid == null) ? "null" : sid;
            //string sessionid = "0";
            //string id = "0";
            //id = (user == null) ? "0" : user.Id;
          
            //ModelDBContext db = new ModelDBContext();
            //Employee emp = db.Employees.Find(id);

            //sessionid = (emp == null ) ? "0" : emp.SessionId ;

            //return ssid.Equals(sessionid) ? true : false;
            if(sid == null || user == null)
            {
                return false;
            }
            else
            {
                if(user == null)
                {
                    return false;
                }
                string sessionid = new ModelDBContext().Employees.Find(user.Id).SessionId;
                return sid.Equals(sessionid) ? true : false;
            }
        }
    }

   
}
