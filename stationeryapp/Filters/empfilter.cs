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
            bool result = IsActiveuser(emp);
            if (!IsActiveuser(emp))
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
    }

   
}
