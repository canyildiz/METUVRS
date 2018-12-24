using METU.VRS.Controllers;
using METU.VRS.Controllers.Static;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace METU.VRS.Tests.Controllers
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class HomeControllerTest
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Migrations.TestDatabase.InitTestDatase();
            University.Init();
        }

        [TestInitialize]
        public void Init()
        {
            University.Init();
        }

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
            ViewResult result = controller.Login("e100", "e100", "") as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewBag.Result);
        }

        [TestMethod]
        public void LoginFail()
        {
            HomeController controller = new HomeController();
            ViewResult result = controller.Login("e100", "fail", "") as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.ViewBag.Result);
        }
    }
}
