using METU.VRS.Controllers;
using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace METU.VRS.Tests.Controllers
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class VisitorControllerTest : ControllerTestBase
    {

        [TestMethod]
        public void Index()
        {
            VisitorController vc = new VisitorController();
            ViewResult actionResult = vc.Index() as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsNull(actionResult.Model);
        }

        [TestMethod]
        public void ApplyIndex()
        {
            VisitorController vc = new VisitorController();
            ViewResult actionResult = vc.Apply() as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.Model);
            Assert.IsInstanceOfType(actionResult.Model, typeof(Visitor));
        }

        [TestMethod]
        public void ApplyThenDetail()
        {
            var visitor = new Visitor
            {
                Description = "For testing purpose",
                Email = "test@test.com",
                Name = "Test Person",
                VisitDate = DateTime.Today,
                User = new User() { UID = "e101" },
                Vehicle = new Vehicle { OwnerName = "Test Owner", PlateNumber = "06AB123", RegistrationNumber = "AB12313", Type = VehicleType.Car }
            };

            VisitorController vc = new VisitorController();
            RedirectToRouteResult actionResult = vc.Apply(visitor) as RedirectToRouteResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Detail", actionResult.RouteValues["action"]);
            Assert.AreEqual("Visitor", actionResult.RouteValues["controller"]);
            Assert.IsNotNull(actionResult.RouteValues["UID"]);
            Assert.AreEqual(1, actionResult.RouteValues["success"]);

            ViewResult viewResult = vc.Detail(actionResult.RouteValues["UID"].ToString()) as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(Visitor));
            Visitor model = viewResult.Model as Visitor;
            Assert.AreEqual(visitor.Name, model.Name);
            Assert.AreEqual(visitor.Email, model.Email);
            Assert.AreEqual(VisitorStatus.WaitingForApproval, model.Status);
        }

        [TestMethod]
        public void ApplyWithAuth()
        {
            var visitor = new Visitor
            {
                Description = "For testing purpose",
                Email = "test@test.com",
                Name = "Test Person",
                VisitDate = DateTime.Today,
                User = null,
                Vehicle = new Vehicle { OwnerName = "Test Owner", PlateNumber = "06AB123", RegistrationNumber = "AB12313", Type = VehicleType.Car }
            };

            var mockUser = University.GetUser("e101");
            VisitorController vc = new VisitorController();
            vc.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), vc);
            RedirectToRouteResult actionResult = vc.Apply(visitor) as RedirectToRouteResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual("List", actionResult.RouteValues["action"]);
            Assert.AreEqual("Visitor", actionResult.RouteValues["controller"]);
            Assert.AreEqual(1, actionResult.RouteValues["success"]);
        }



        [TestMethod]
        public void ApplyWithInvalidModel()
        {
            VisitorController vc = new VisitorController();
            vc.ModelState.AddModelError("test", "error");
            EmptyResult actionResult = vc.Apply(new Visitor()) as EmptyResult;
            Assert.IsNotNull(actionResult);
        }

        [TestMethod]
        public void ApplyWithActiveSticker()
        {
            var visitor = new Visitor
            {
                Description = "For testing purpose",
                Email = "test@example.com",
                Name = "Test Person",
                VisitDate = DateTime.Today,
                User = new User() { UID = "e101" },
                Vehicle = new Vehicle { OwnerName = "Test Owner", PlateNumber = "06AA103", RegistrationNumber = "AB12313", Type = VehicleType.Car }
            };

            VisitorController vc = new VisitorController();
            ViewResult actionResult = vc.Apply(visitor) as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.ViewBag.ErrorMessage);
            Console.Out.WriteLine(actionResult.ViewBag.ErrorMessage);
        }

        [TestMethod]
        public void ApplyWithInvalidVisitee()
        {
            var visitor = new Visitor
            {
                Description = "For testing purpose",
                Email = "test@example.com",
                Name = "Test Person",
                VisitDate = DateTime.Today,
                User = new User() { UID = "invaliduserid" },
                Vehicle = new Vehicle { OwnerName = "Test Owner", PlateNumber = "06AB103", RegistrationNumber = "AB12313", Type = VehicleType.Car }
            };

            VisitorController vc = new VisitorController();
            ViewResult actionResult = vc.Apply(visitor) as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.ViewBag.ErrorMessage);
            Console.Out.WriteLine(actionResult.ViewBag.ErrorMessage);
        }

        [TestMethod]
        public void ApplyWithAlreadyRequestedVehicle()
        {
            var visitor = new Visitor
            {
                Description = "For testing purpose",
                Email = "test@example.com",
                Name = "Test Person",
                VisitDate = DateTime.Today,
                User = new User() { UID = "e101" },
                Vehicle = new Vehicle { OwnerName = "Test Owner", PlateNumber = "06BB100", RegistrationNumber = "AB12313", Type = VehicleType.Car }
            };

            VisitorController vc = new VisitorController();
            ViewResult actionResult = vc.Apply(visitor) as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.ViewBag.ErrorMessage);
            Console.Out.WriteLine(actionResult.ViewBag.ErrorMessage);
        }

        [TestMethod]
        public void DetailNotFound()
        {
            VisitorController vc = new VisitorController();
            HttpStatusCodeResult actionResult = vc.Detail(Guid.NewGuid().ToString()) as HttpStatusCodeResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(404, actionResult.StatusCode);
        }

        [TestMethod]
        public void ApproveThenDetail()
        {
            var mockUser = University.GetUser("e101");

            VisitorController vc = new VisitorController();
            vc.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), vc);

            ViewResult actionResult = vc.Detail("TEST1") as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Model, typeof(Visitor));
            Visitor model = actionResult.Model as Visitor;
            Assert.AreEqual(VisitorStatus.WaitingForApproval, model.Status);

            RedirectToRouteResult actionResult2 = vc.Approve(model.ID) as RedirectToRouteResult;
            Assert.AreEqual("List", actionResult2.RouteValues["action"]);

            ViewResult actionResult3 = vc.Detail("TEST1") as ViewResult;
            Assert.IsNotNull(actionResult3);
            Assert.IsInstanceOfType(actionResult3.Model, typeof(Visitor));
            Visitor model2 = actionResult3.Model as Visitor;
            Assert.AreEqual(VisitorStatus.WaitingForArrival, model2.Status);
            Assert.IsNotNull(model2.ApproveDate);
        }

        [TestMethod]
        public void RejectThenDetail()
        {
            var mockUser = University.GetUser("e101");

            VisitorController vc = new VisitorController();
            vc.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), vc);

            ViewResult actionResult = vc.Detail("TEST1") as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Model, typeof(Visitor));
            Visitor model = actionResult.Model as Visitor;
            Assert.AreEqual(VisitorStatus.WaitingForApproval, model.Status);

            RedirectToRouteResult actionResult2 = vc.Reject(model.ID) as RedirectToRouteResult;
            Assert.AreEqual("List", actionResult2.RouteValues["action"]);

            ViewResult actionResult3 = vc.Detail("TEST1") as ViewResult;
            Assert.IsNotNull(actionResult3);
            Assert.IsInstanceOfType(actionResult3.Model, typeof(Visitor));
            Visitor model2 = actionResult3.Model as Visitor;
            Assert.AreEqual(VisitorStatus.Rejected, model2.Status);
            Assert.IsNotNull(model2.ApproveDate);
        }

        [TestMethod]
        public void ApproveWithAnotherUserAuth()
        {
            var mockUser = University.GetUser("e100");

            VisitorController vc = new VisitorController();
            vc.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), vc);

            ViewResult actionResult = vc.Detail("TEST1") as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Model, typeof(Visitor));
            Visitor model = actionResult.Model as Visitor;
            Assert.AreEqual(VisitorStatus.WaitingForApproval, model.Status);
            Assert.ThrowsException<HttpAntiForgeryException>(() => vc.Approve(model.ID));
        }

        [TestMethod]
        public void ApproveMissingVisitor()
        {
            var mockUser = University.GetUser("e100");

            VisitorController vc = new VisitorController();
            vc.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), vc);
            Assert.ThrowsException<HttpAntiForgeryException>(() => vc.Approve(999));
        }

        [TestMethod]
        public void List()
        {
            var mockUser = University.GetUser("e101");

            VisitorController vc = new VisitorController();
            vc.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), vc);

            ViewResult actionResult = vc.List("", "", "", 1) as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Model, typeof(PagedList<Visitor>));
            PagedList<Visitor> visitors = actionResult.Model as PagedList<Visitor>;
            Assert.AreEqual(2, visitors.Count);

            actionResult = vc.List("name_desc", "", "John Doe", 1) as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Model, typeof(PagedList<Visitor>));
            visitors = actionResult.Model as PagedList<Visitor>;
            Assert.AreEqual(1, visitors.Count);
            Visitor visitor = visitors.FirstOrDefault();

            Assert.AreEqual(VisitorStatus.WaitingForApproval, visitor.Status);

            actionResult = vc.List("date_desc", "", "Jane Doe", 2) as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Model, typeof(PagedList<Visitor>));
            visitors = actionResult.Model as PagedList<Visitor>;
            Assert.AreEqual(1, visitors.Count);
            visitor = visitors.FirstOrDefault();

            Assert.AreEqual(VisitorStatus.WaitingForArrival, visitor.Status);
        }
    }
}
