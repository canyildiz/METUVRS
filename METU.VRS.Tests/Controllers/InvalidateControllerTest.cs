using METU.VRS.Controllers;
using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace METU.VRS.Tests.Controllers
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class InvalidateControllerTest : ControllerTestBase
    {
        [TestMethod]
        public void Index()
        {
            var mockDeliverUser = University.GetUser("o103");

            InvalidateController controller = new InvalidateController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockDeliverUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("", "", "", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreNotEqual(0, model.Count);
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.Active).Count());
        }

        [TestMethod]
        public void IndexFiltered()
        {
            var mockDeliverUser = University.GetUser("o103");

            InvalidateController controller = new InvalidateController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockDeliverUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("", "", "Test Student4", 1) as ViewResult; //e103
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.Active).Count());
            Assert.AreEqual(1, model.Count);
        }

        [TestMethod]
        public void IndexSorted()
        {
            var mockDeliverUser = University.GetUser("o103");

            InvalidateController controller = new InvalidateController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockDeliverUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("name_desc", "", "", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreNotEqual(0, model.Count);
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.Active).Count());
            Assert.AreEqual("Test Student4", model.FirstOrDefault().Owner.Name);
        }

        [TestMethod]
        public void IndexFilteredThenInvalidate()
        {
            var mockDeliverUser = University.GetUser("o103");

            InvalidateController controller = new InvalidateController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockDeliverUser).Object, new RouteData(), controller);

            //get list
            ViewResult result = controller.Index("", "", "Test Student4", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.Active).Count());
            Assert.AreEqual(1, model.Count);

            //invalidate
            StickerApplication stickerApplication = model.FirstOrDefault();
            controller.Invalidate(stickerApplication.ID);

            //get list
            result = controller.Index("", "", "Test Student4", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));
            model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Count);

            //applications index page for user
            StickerController scontroller = new StickerController
            {
                ControllerContext = new ControllerContext(MockAuthContext(University.GetUser(stickerApplication.User.UID)).Object, new RouteData(), controller)
            };
            ViewResult sresult = scontroller.Index() as ViewResult;

            Assert.IsNotNull(sresult);
            Assert.IsInstanceOfType(sresult.Model, typeof(List<StickerApplication>));

            List<StickerApplication> smodel = sresult.Model as List<StickerApplication>;
            Assert.AreNotEqual(0, smodel.Count);
            Assert.AreEqual(StickerApplicationStatus.Invalidated, smodel.FirstOrDefault().Status);
        }

        [TestMethod]
        public void InvalidateSticker()
        {
            var mockUser = University.GetUser("o103");

            InvalidateController controller = new InvalidateController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("", "", "", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));
            PagedList<StickerApplication> applications = result.Model as PagedList<StickerApplication>;
            Assert.IsTrue(applications.Count > 0, "No active stickers found");
            StickerApplication application = applications.FirstOrDefault();
            Assert.IsNotNull(application);
            Assert.AreEqual(StickerApplicationStatus.Active, application.Status);
            StickerApplication invalidatedApplication = application;

            RedirectToRouteResult invalidateResult = controller.Invalidate(application.ID) as RedirectToRouteResult;
            Assert.AreEqual(1, invalidateResult.RouteValues["success"]);
            Assert.AreEqual("Index", invalidateResult.RouteValues["action"]);

            result = controller.Index("", "", "", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));
            applications = result.Model as PagedList<StickerApplication>;
            StickerApplication testApplication = applications.FirstOrDefault(a => a.ID == invalidatedApplication.ID);
            Assert.IsNull(testApplication, "Invalidated sticker still listed as Active");

            //applications index page for user
            StickerController scontroller = new StickerController
            {
                ControllerContext = new ControllerContext(MockAuthContext(University.GetUser(invalidatedApplication.User.UID)).Object, new RouteData(), controller)
            };
            ViewResult sresult = scontroller.Index() as ViewResult;

            Assert.IsNotNull(sresult);
            Assert.IsInstanceOfType(sresult.Model, typeof(List<StickerApplication>));

            List<StickerApplication> smodel = sresult.Model as List<StickerApplication>;
            Assert.AreNotEqual(0, smodel.Count);
            Assert.AreEqual(StickerApplicationStatus.Invalidated, smodel.FirstOrDefault().Status);
        }
    }
}
