using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace METU.VRS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication context = (HttpApplication)sender;
            HttpContext _httpContext = context.Context;

            if (_httpContext.User == null)
            {
                var cookie = HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName);
                if (cookie != null)
                {
                    var decodedTicket = FormsAuthentication.Decrypt(cookie.Value);
                    if (decodedTicket != null && !decodedTicket.Expired)
                    {
                        var principal = new System.Security.Principal.GenericPrincipal(new FormsIdentity(decodedTicket), new string[] { "user" });
                        HttpContext.Current.User = principal;
                    }
                }
            }
        }

    }
}
