using METU.VRS.Controllers.Static;
using METU.VRS.UI;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace METU.VRS
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            University.Init();
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
                        var principal = new METUPrincipal(decodedTicket);
                        HttpContext.Current.User = principal;
                    }
                }
            }
        }

    }
}
