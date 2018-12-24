using METU.VRS.Controllers;
using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using METU.VRS.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace METU.VRS.Tests.Controllers
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class StickerControllerTest : ControllerTestBase
    {
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
        public void Detail()
        {
            var mockUser = University.GetUser("e100");

            StickerController controller = new StickerController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), controller);

            ViewResult result = controller.Detail(1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(StickerApplication));

            StickerApplication model = result.Model as StickerApplication;
            Assert.AreEqual(1, model.ID);
        }

        [TestMethod]
        public void DetailNotAllowed()
        {
            var mockUser = University.GetUser("e100");

            StickerController controller = new StickerController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), controller);

            ActionResult result = controller.Detail(2);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpUnauthorizedResult));
        }

        [TestMethod]
        public void DetailwithApprovalUser()
        {
            var mockUser = University.GetUser("o101");

            StickerController controller = new StickerController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), controller);

            ViewResult result = controller.Detail(1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(StickerApplication));

            StickerApplication model = result.Model as StickerApplication;
            Assert.AreEqual(1, model.ID);
        }
    }
}
