using METU.VRS.Controllers;
using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using METU.VRS.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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
        [TestInitialize]
        public void Init()
        {
            University.Init();
        }

        [TestMethod]
        public void IndexWithDataTest()
        {
            var mockUser = new User() { UID = "e100" };
            var mockData = new List<StickerApplication> {
                new StickerApplication { LastModified = new DateTime(2018, 1, 1), User=mockUser }
            };
            var mockDbSet = new Mock<DbSet<StickerApplication>>().SetupData(mockData);
            var dbctx = new Mock<DatabaseContext>();
            dbctx.Setup(c => c.StickerApplications).Returns(mockDbSet.Object);

            StickerController controller = new StickerController(dbctx.Object);
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index() as ViewResult;
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(List<StickerApplication>));
            Assert.AreEqual(1, ((List<StickerApplication>)result.Model).Count);
            Assert.AreEqual(mockData.FirstOrDefault().LastModified, ((List<StickerApplication>)result.Model).FirstOrDefault().LastModified);
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
            var mockUser = new User() { UID = "e100", Division = new BranchAffiliate { UID = "II" } };
            var mockData = new StickerApplication
            {
                User = mockUser,
                Type = new StickerType { TermType = TermTypes.AcademicYear },
                Vehicle = new Vehicle { OwnerName = "Test Owner", PlateNumber = "06AA1234", RegistrationNumber = "AA123456" }
            };

            var mockDbSet = new Mock<DbSet<StickerApplication>>().SetupData(new List<StickerApplication>());

            var dbctx = new Mock<DatabaseContext>();
            dbctx.Setup(c => c.StickerApplications).Returns(mockDbSet.Object);
            dbctx.Setup(c => c.BranchsAffiliates).Returns(new Mock<DbSet<BranchAffiliate>>().Object);
            dbctx.Setup(c => c.Users).Returns(new Mock<DbSet<User>>().SetupData(new User[] { mockUser }).Object);

            StickerController controller = new StickerController(dbctx.Object);
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), controller);

            RedirectToRouteResult result = controller.Apply(mockData) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("action", result.RouteValues.Keys.FirstOrDefault());
            Assert.AreEqual("Index", result.RouteValues.Values.FirstOrDefault());
            Assert.AreEqual(1, mockDbSet.Object.ToList().Count());
            Assert.AreEqual("06AA1234", mockDbSet.Object.ToList().FirstOrDefault().Vehicle.PlateNumber);
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
