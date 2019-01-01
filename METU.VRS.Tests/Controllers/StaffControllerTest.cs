using METU.VRS.Controllers;
using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using METU.VRS.Services;
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
    public class StaffControllerTest : ControllerTestBase
    {

        [TestMethod]
        public void Index()
        {
            StaffController sc = new StaffController();
            RedirectToRouteResult result = sc.Index() as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues["controller"]);
            Assert.AreEqual("ListVisitors", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ListVisitors()
        {
            var mockUser = University.GetUser("o103");

            StaffController sc = new StaffController();
            sc.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), sc);

            ViewResult result = sc.ListVisitors("", "", null, 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<Visitor>));
            PagedList<Visitor> visitors = result.Model as PagedList<Visitor>;
            Assert.AreEqual(1, visitors.Count);

            Visitor visitor = visitors.FirstOrDefault();
            Assert.AreEqual(VisitorStatus.WaitingForArrival, visitor.Status);

            //sort by Date
            result = sc.ListVisitors("Date", "", "John Doe", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<Visitor>));
            visitors = result.Model as PagedList<Visitor>;
            Assert.AreEqual(0, visitors.Count);

            //sort by Plate without page number
            result = sc.ListVisitors("Plate", "", "Jane Doe", 0) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<Visitor>));
            visitors = result.Model as PagedList<Visitor>;
            Assert.AreEqual(1, visitors.Count);

            //sort by plate_desc
            result = sc.ListVisitors("plate_desc", "", "Jane Doe", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<Visitor>));
            visitors = result.Model as PagedList<Visitor>;
            Assert.AreEqual(1, visitors.Count);

            result = sc.ListVisitors("name_desc", "", "Jane Doe", 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<Visitor>));
            visitors = result.Model as PagedList<Visitor>;
            Assert.AreEqual(1, visitors.Count);

            result = sc.ListVisitors("date_desc", "", "Jane Doe", 2) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<Visitor>));
            visitors = result.Model as PagedList<Visitor>;
            Assert.AreEqual(0, visitors.Count);
        }

        [TestMethod]
        public void MarkArrivedSuccess()
        {
            var mockUser = University.GetUser("o103");

            StaffController sc = new StaffController();
            sc.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), sc);

            //list visitors
            ViewResult result = sc.ListVisitors("", "", null, 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<Visitor>));
            PagedList<Visitor> visitors = result.Model as PagedList<Visitor>;
            Assert.AreEqual(1, visitors.Count);

            Visitor visitor = visitors.FirstOrDefault();
            Assert.AreEqual(VisitorStatus.WaitingForArrival, visitor.Status);

            RedirectToRouteResult resultArrive = sc.VisitorArrived(visitor.ID) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(null, resultArrive.RouteValues["controller"]);
            Assert.AreEqual("ListVisitors", resultArrive.RouteValues["action"]);
            Assert.AreEqual(1, resultArrive.RouteValues["success"]);

            //list visitors again
            result = sc.ListVisitors("", "", null, 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<Visitor>));
            visitors = result.Model as PagedList<Visitor>;
            Assert.AreEqual(0, visitors.Count);

        }

        [TestMethod]
        public void MarkArrivedFail()
        {
            var mockUser = University.GetUser("o103");

            StaffController sc = new StaffController();
            sc.ControllerContext = new ControllerContext(MockAuthContext(mockUser).Object, new RouteData(), sc);

            //list visitors
            ViewResult result = sc.ListVisitors("", "", null, 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<Visitor>));
            PagedList<Visitor> visitors = result.Model as PagedList<Visitor>;
            Assert.AreEqual(1, visitors.Count);

            Visitor visitor = visitors.FirstOrDefault();
            Assert.AreEqual(VisitorStatus.WaitingForArrival, visitor.Status);

            RedirectToRouteResult resultArrive = sc.VisitorArrived(visitor.ID + 1000) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(null, resultArrive.RouteValues["controller"]);
            Assert.AreEqual("ListVisitors", resultArrive.RouteValues["action"]);
            Assert.IsNotNull(resultArrive.RouteValues["error"]);
            Assert.IsNull(resultArrive.RouteValues["success"]);

            //list visitors again
            result = sc.ListVisitors("", "", null, 1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PagedList<Visitor>));
            visitors = result.Model as PagedList<Visitor>;
            Assert.AreEqual(1, visitors.Count);
        }
    }
}
