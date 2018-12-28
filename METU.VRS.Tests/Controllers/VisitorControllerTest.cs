using METU.VRS.Controllers;
using METU.VRS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;

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

    }
}
