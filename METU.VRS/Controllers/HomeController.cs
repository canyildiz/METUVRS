using METU.VRS.Abstract;
using METU.VRS.Concrete;
using METU.VRS.Models.Shared;
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
            ViewBag.UserName = "testuser";
            return View();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string Username, string Password, string returnUrl)
        {
            ILoginProvider loginProvider = new DummyLoginProvider();
            LoginResult result = loginProvider.Login(Username, Password);
            if (result.Result)
            {
                if (HttpContext != null)
                {
                    FormsAuthentication.SetAuthCookie(Username, false);
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