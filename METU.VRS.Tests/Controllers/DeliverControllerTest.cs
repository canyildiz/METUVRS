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
    public class DeliverControllerTest : ControllerTestBase
    {
        [TestMethod]
        public void Index()
        {
            var mockDeliverUser = University.GetUser("o102");

            DeliverController controller = new DeliverController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockDeliverUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("", "", "", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreNotEqual(0, model.Count);
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForDelivery).Count());
        }

        [TestMethod]
        public void IndexFiltered()
        {
            var mockDeliverUser = University.GetUser("o102");

            DeliverController controller = new DeliverController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockDeliverUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("", "", "Test Student13", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForDelivery).Count());
            Assert.AreEqual(1, model.Count);
        }

        [TestMethod]
        public void IndexSorted()
        {
            var mockDeliverUser = University.GetUser("o102");

            DeliverController controller = new DeliverController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockDeliverUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("name_desc", "", "", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreNotEqual(0, model.Count);
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForDelivery).Count());
            Assert.AreEqual("Test Student13", model.FirstOrDefault().Owner.Name);
        }

        [TestMethod]
        public void IndexFilteredThenReject()
        {
            var mockDeliverUser = University.GetUser("o102");

            DeliverController controller = new DeliverController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockDeliverUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("", "", "Test Student13", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForDelivery).Count());
            Assert.AreEqual(1, model.Count);

            controller.Reject(model.FirstOrDefault().ID);
            result = controller.Index("", "", "Test Student13", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Count);
        }

        [TestMethod]
        public void IndexFilteredThenApprove()
        {
            var mockDeliverUser = University.GetUser("o102");

            DeliverController controller = new DeliverController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockDeliverUser).Object, new RouteData(), controller);

            //get list
            ViewResult result = controller.Index("", "", "Test Student13", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForDelivery).Count());
            Assert.AreEqual(1, model.Count);

            //deliver
            StickerApplication stickerApplication = model.FirstOrDefault();
            controller.Deliver(stickerApplication.ID);

            //get list
            result = controller.Index("", "", "Test Student13", 1) as ViewResult;
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
            Assert.AreEqual(StickerApplicationStatus.Active, smodel.FirstOrDefault().Status);
            Assert.AreEqual(123456, smodel.FirstOrDefault().Sticker.SerialNumber);

        }
    }
}
