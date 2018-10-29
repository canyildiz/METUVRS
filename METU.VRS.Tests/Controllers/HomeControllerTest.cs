using METU.VRS.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace METU.VRS.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            HomeController controller = new HomeController();
            ViewResult result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            HomeController controller = new HomeController();
            ViewResult result = controller.About() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Contact()
        {
            HomeController controller = new HomeController();
            ViewResult result = controller.Contact() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoginSuccess()
        {
            HomeController controller = new HomeController();
            ViewResult result = controller.Login("testuser", "password", "") as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewBag.Result);
        }

        [TestMethod]
        public void LoginFail()
        {
            HomeController controller = new HomeController();
            ViewResult result = controller.Login("testuser", "fail", "") as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.ViewBag.Result);
        }
    }
}
