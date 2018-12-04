using METU.VRS.Controllers;
using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using METU.VRS.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace METU.VRS.Tests.Controllers
{
    [TestClass]
    public class StickerControllerTest
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

        }

        [TestMethod]
        public void IndexWithDataTest()
        {
            var mockUser = University.GetUser("e100");

            StickerController controller = new StickerController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<StickerApplication>));

            List<StickerApplication> model = result.Model as List<StickerApplication>;
            Assert.AreNotEqual(0, model.Count);
            Assert.AreEqual(1, model.FirstOrDefault().ID);
        }

        [TestMethod]
        public void IndexWithoutData()
        {
            var mockUser = new User() { UID = "e100" };
            var mockData = new List<StickerApplication>();
            var mockDbSet = new Mock<DbSet<StickerApplication>>().SetupData(mockData);
            var dbctx = new Mock<DatabaseContext>();
            dbctx.Setup(c => c.StickerApplications).Returns(mockDbSet.Object);

            StickerController controller = new StickerController(dbctx.Object);
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), controller);

            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("action", result.RouteValues.Keys.FirstOrDefault());
            Assert.AreEqual("Apply", result.RouteValues.Values.FirstOrDefault());
        }

        [TestMethod]
        public void ApplyGetMethod()
        {
            StickerController controller = new StickerController();
            ViewResult result = controller.Apply() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(StickerApplication));
        }

        [TestMethod]
        public void ApplyPost()
        {
            var mockUser = University.GetUser("e102");
            var stickerType = University.GetStickerTypes(mockUser.Category).FirstOrDefault();
            var mockData = new StickerApplication
            {
                SelectedType = stickerType.ID.ToString(),
                Vehicle = new Vehicle { OwnerName = "Test Owner", PlateNumber = "06ZZ1234", RegistrationNumber = "ZZ123456" }
            };

            StickerController controller = new StickerController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), controller);

            RedirectToRouteResult result = controller.Apply(mockData) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("action", result.RouteValues.Keys.FirstOrDefault());
            Assert.AreEqual("Index", result.RouteValues.Values.FirstOrDefault());

            ViewResult indexResult = controller.Index() as ViewResult;

            Assert.IsNotNull(indexResult);
            Assert.IsInstanceOfType(indexResult.Model, typeof(List<StickerApplication>));

            List<StickerApplication> model = indexResult.Model as List<StickerApplication>;
            Assert.AreNotEqual(0, model.Count);
            Assert.AreEqual("06ZZ1234", model.FirstOrDefault().Vehicle.PlateNumber);
        }

        [TestMethod]
        public void ApproveGetList()
        {
            var mockApproveUser = University.GetUser("o101");

            StickerController controller = new StickerController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);

            ViewResult result = controller.Approve() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, ((List<StickerApplication>)result.Model).Count);
            Assert.AreEqual(0, ((List<StickerApplication>)result.Model).Where(a => a.Status != StickerApplicationStatus.WaitingForApproval).Count());
        }

        [TestMethod]
        public void TestDBConnection()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                List<StickerApplication> applications = db.StickerApplications
                    .OrderByDescending(a => a.LastModified)
                    .ToList();
                Assert.AreNotEqual(0, applications.Count);
            }
        }
        private Mock<HttpContextBase> MockAuthContext(User mockUser)
        {
            var httpreq = new Mock<HttpRequestBase>();
            var httpctx = new Mock<HttpContextBase>();
            var authUser = new Mock<METU.VRS.UI.METUPrincipal>();
            authUser.Setup(u => u.User).Returns(mockUser);

            httpctx.Setup(ctx => ctx.Request).Returns(httpreq.Object);
            httpctx.Setup(ctx => ctx.User).Returns(authUser.Object);
            return httpctx;
        }
    }
}
