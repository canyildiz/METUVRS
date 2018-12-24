using METU.VRS.Controllers;
using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using METU.VRS.Models.CT;
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
    public class ApproveControllerTest : ControllerTestBase
    {
        [TestMethod]
        public void Index()
        {
            var mockApproveUser = University.GetUser("o101");

            ApproveController controller = new ApproveController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("", "", "", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreNotEqual(0, model.Count);
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForApproval).Count());
        }

        [TestMethod]
        public void IndexFiltered()
        {
            var mockApproveUser = University.GetUser("o101");

            ApproveController controller = new ApproveController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("", "", "Test Student11", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForApproval).Count());
            Assert.AreEqual(1, model.Count);
        }

        [TestMethod]
        public void IndexFilteredWithActions()
        {
            var mockApproveUser = University.GetUser("o101");

            ApproveController controller = new ApproveController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("", "", "Test Student11", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForApproval).Count());
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual(2, model.FirstOrDefault().GetApprovementOptions().Count);
        }

        [TestMethod]
        public void IndexSorted()
        {
            var mockApproveUser = University.GetUser("o101");

            ApproveController controller = new ApproveController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("name_desc", "", "", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreNotEqual(0, model.Count);
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForApproval).Count());
            Assert.AreEqual("Test Student11", model.FirstOrDefault().Owner.Name);
        }

        [TestMethod]
        public void IndexFilteredThenReject()
        {
            var mockApproveUser = University.GetUser("o101");

            ApproveController controller = new ApproveController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);

            ViewResult result = controller.Index("", "", "Test Student11", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForApproval).Count());
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual(2, model.FirstOrDefault().GetApprovementOptions().Count);

            controller.Reject(model.FirstOrDefault().ID);
            result = controller.Index("", "", "Test Student11", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Count);
        }

        [TestMethod]
        public void IndexFilteredThenApprove()
        {
            var mockApproveUser = University.GetUser("o101");

            ApproveController controller = new ApproveController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);

            //get list
            ViewResult result = controller.Index("", "", "Test Student11", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<StickerApplication>));

            PagedList<StickerApplication> model = result.Model as PagedList<StickerApplication>;
            Assert.AreEqual(0, model.Where(a => a.Status != StickerApplicationStatus.WaitingForApproval).Count());
            Assert.AreEqual(1, model.Count);

            //approve
            StickerApplication stickerApplication = model.FirstOrDefault();
            List<ApprovementOption> approvementOptions = stickerApplication.GetApprovementOptions();
            Assert.AreEqual(2, approvementOptions.Count);
            ApprovementOption otherOption = approvementOptions.Where(a => a.QuotaID != stickerApplication.Quota.ID).FirstOrDefault();
            controller.Approve(stickerApplication.ID, otherOption.QuotaID);

            //get list
            result = controller.Index("", "", "Test Student11", 1) as ViewResult;
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
            Assert.AreEqual(StickerApplicationStatus.WaitingForPayment, smodel.FirstOrDefault().Status);

        }
    }
}
