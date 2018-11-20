using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace METU.VRS.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult FAQ()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.UserName = "e100";
            return View();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string Username, string Password, string returnUrl)
        {
            User user = University.TryLogin(Username, Password);
            if (user != null)
            {
                if (HttpContext != null)
                {
                    FormsAuthentication.SetAuthCookie(user.UID, false);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Result = true;
                    return View();
                }
            }
            else
            {
                ViewBag.Result = false;
                ViewBag.UserName = Username;
                return View();
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}