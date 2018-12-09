using METU.VRS.Controllers;
using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using METU.VRS.Models.CT;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace METU.VRS.Tests.Controllers
{
    [TestClass]
    public class PayControllerTest : ControllerTestBase
    {

        [TestMethod]
        public void IndexWithoutApplicationId()
        {
            var mockApproveUser = University.GetUser("e101");

            PayController controller = new PayController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);

            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Sticker",result.RouteValues["controller"]);
            Assert.AreEqual("Index",result.RouteValues["action"]);
        }

        [TestMethod]
        public void Index()
        {
            var mockApproveUser = University.GetUser("e201");

            //get Applications
            StickerController sc = new StickerController();
            sc.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), sc);
            ViewResult indexResult = sc.Index() as ViewResult;
            Assert.IsNotNull(indexResult);
            Assert.IsInstanceOfType(indexResult.Model, typeof(List<StickerApplication>));

            //find application waiting for payment
            List<StickerApplication> listModel = indexResult.Model as List<StickerApplication>;
            Assert.AreNotEqual(0, listModel.Count);
            StickerApplication application = listModel.Find(m => m.Status == StickerApplicationStatus.WaitingForPayment);
            Assert.IsNotNull(application);

            PayController controller = new PayController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);
                       
            ViewResult result = controller.Index(application.ID) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PaymentRequest));

            PaymentRequest model = result.Model as PaymentRequest;
            Assert.IsNotNull(model.Application);
            Assert.IsNotNull(model.Application.Quota);
            Assert.IsNotNull(model.Application.Quota.StickerFee);
            Assert.AreEqual(model.Application.Quota.StickerFee, application.Quota.StickerFee);
        }

        [TestMethod]
        public void ResponseOk()
        {
            var mockApproveUser = University.GetUser("e101");

            //get Applications
            StickerController sc = new StickerController();
            sc.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), sc);
            ViewResult indexResult = sc.Index() as ViewResult;
            Assert.IsNotNull(indexResult);
            Assert.IsInstanceOfType(indexResult.Model, typeof(List<StickerApplication>));

            //find application waiting for payment
            List<StickerApplication> listModel = indexResult.Model as List<StickerApplication>;
            Assert.AreNotEqual(0, listModel.Count);
            StickerApplication application = listModel.Find(m => m.Status == StickerApplicationStatus.WaitingForPayment);
            Assert.IsNotNull(application);
            Assert.AreEqual(StickerApplicationStatus.WaitingForPayment, application.Status);

            //pay
            PayController controller = new PayController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);

            PaymentResponseSuccess pr = new PaymentResponseSuccess()
            {
                amount = application.Quota.StickerFee,
                TransId = "TEST-Transaction-001",
                ReturnOid = application.ID.ToString() + "-TestOrder",
                mdStatus=4,
                Response="success"
            };

            RedirectToRouteResult result = controller.Ok(pr) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Sticker", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.IsNotNull(result.RouteValues["ok_msg"]);

            //get updated list to be sure application status is not updated
            ViewResult indexResultR = sc.Index() as ViewResult;
            Assert.IsNotNull(indexResultR);
            Assert.IsInstanceOfType(indexResultR.Model, typeof(List<StickerApplication>));
            List<StickerApplication> listModelR = indexResultR.Model as List<StickerApplication>;
            Assert.AreNotEqual(0, listModelR.Count);
            StickerApplication applicationR = listModelR.Find(m => m.ID == application.ID);
            Assert.IsNotNull(applicationR);
            Assert.AreEqual(StickerApplicationStatus.WaitingForDelivery, applicationR.Status);
        }

        [TestMethod]
        public void ResponseFail()
        {
            var mockApproveUser = University.GetUser("e201");

            //get Applications
            StickerController sc = new StickerController();
            sc.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), sc);
            ViewResult indexResult = sc.Index() as ViewResult;
            Assert.IsNotNull(indexResult);
            Assert.IsInstanceOfType(indexResult.Model, typeof(List<StickerApplication>));

            //find application waiting for payment
            List<StickerApplication> listModel = indexResult.Model as List<StickerApplication>;
            Assert.AreNotEqual(0, listModel.Count);
            StickerApplication application = listModel.Find(m => m.Status == StickerApplicationStatus.WaitingForPayment);
            Assert.IsNotNull(application);
            Assert.AreEqual(StickerApplicationStatus.WaitingForPayment, application.Status);

            //pay
            PayController controller = new PayController();
            controller.ControllerContext = new ControllerContext(MockAuthContext(mockApproveUser).Object, new RouteData(), controller);

            PaymentResponseFail pr = new PaymentResponseFail()
            {
                TransId = "TEST-Transaction-002",
                ReturnOid = application.ID.ToString() + "-TestOrder",
                mdStatus = 4,
                Response = "error",
                ErrMsg = "Test Error Message"
            };

            RedirectToRouteResult result = controller.Fail(pr) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Pay", result.RouteValues["controller"]);
            Assert.AreEqual("", result.RouteValues["action"]);
            Assert.AreEqual(application.ID.ToString(), result.RouteValues["Id"]);
            Assert.IsNotNull(result.RouteValues["err_msg"]);
            Assert.AreEqual("Test Error Message", result.RouteValues["err_msg"]);

            //get updated list
            ViewResult indexResultR = sc.Index() as ViewResult;
            Assert.IsNotNull(indexResultR);
            Assert.IsInstanceOfType(indexResultR.Model, typeof(List<StickerApplication>));
            List<StickerApplication> listModelR = indexResultR.Model as List<StickerApplication>;
            Assert.AreNotEqual(0, listModelR.Count);
            StickerApplication applicationR = listModelR.Find(m => m.ID == application.ID);
            Assert.IsNotNull(applicationR);
            Assert.AreEqual(StickerApplicationStatus.WaitingForPayment, applicationR.Status);
        }
    }
}
