using METU.VRS.Models;
using METU.VRS.Models.CT;
using METU.VRS.Services;
using METU.VRS.UI;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using PagedList;


namespace METU.VRS.Controllers
{
    public class StaffController : Base.ControllerBase
    {
        public StaffController() : base()
        {
        }

        public ActionResult Index()
        {
            Trace.WriteLine("GET /Staff/Index");
            return RedirectToAction("ListVisitors");
        }

        [HttpGet]
        public ActionResult ListVisitors(string sortOrder, string currentFilter, string searchString, int? page)
        {
            Trace.WriteLine("GET /Staff/Visitors/List");

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.PlateSortParm = sortOrder == "Plate" ? "plate_desc" : "Plate";

            searchString = searchString ?? currentFilter;

            ViewBag.CurrentFilter = searchString;

            IQueryable<Visitor> visitors = null;

            using (DatabaseContext db = GetNewDBContext())
            {
                visitors = db.Visitors
                    .AsNoTracking()
                    .Include(a => a.Vehicle)
                    .Include(a => a.User)
                    .Where(a => a.Status == VisitorStatus.WaitingForArrival);

                if (!string.IsNullOrEmpty(searchString))
                {
                    visitors = visitors.Where(a =>
                    a.Name.Equals(searchString)
                    || a.Email.Contains(searchString)
                    || a.Description.Contains(searchString)
                    || a.Vehicle.PlateNumber.Contains(searchString)
                    || a.Vehicle.RegistrationNumber.Contains(searchString)
                    || a.Vehicle.OwnerName.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "name_desc":
                        visitors = visitors.OrderByDescending(v => v.Name);
                        break;
                    case "Date":
                        visitors = visitors.OrderBy(v => v.VisitDate);
                        break;
                    case "date_desc":
                        visitors = visitors.OrderByDescending(v => v.VisitDate);
                        break;
                    case "Plate":
                        visitors = visitors.OrderBy(v => v.Vehicle.PlateNumber);
                        break;
                    case "plate_desc":
                        visitors = visitors.OrderByDescending(v => v.Vehicle.PlateNumber);
                        break;
                    default:
                        visitors = visitors.OrderBy(v => v.Status).OrderBy(v => v.LastModified);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                pageNumber = pageNumber > 0 ? pageNumber : 1;
                return View(visitors.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult VisitorArrived(int id)
        {
            using (DatabaseContext db = GetNewDBContext())
            {
                Visitor visitor = db.Visitors.Include("Vehicle").FirstOrDefault(v => v.ID == id);
                if(null != visitor)
                {
                    visitor.Status = VisitorStatus.Arrived;
                    visitor.LastModified = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("ListVisitors", new { err = "Visitor not updated! " + ex.Message });
                    }
                }
                else
                {
                    return RedirectToAction("ListVisitors", new { error = "Visitor not found" });
                }
            }
            return RedirectToAction("ListVisitors", new { success = 1 });
        }
    }
}