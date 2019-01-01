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
            Trace.WriteLine("GET /Staff/ListVisitors");

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.PlateSortParm = sortOrder == "Plate" ? "plate_desc" : "Plate";

            searchString = searchString ?? currentFilter;

            ViewBag.CurrentFilter = searchString;

            IPagedList<StickerApplication> visitorApplications = ListApplications(sortOrder, currentFilter, searchString, page, StickerApplicationStatus.WaitingForApproval);

            return View(visitorApplications);
        }

        public ActionResult VisitorArrived(int id)
        {
            using (DatabaseContext db = GetNewDBContext())
            {
                Visitor visitor = db.Visitors
                    .Include("Vehicle")
                    .Include("User")
                    .FirstOrDefault(v => v.ID == id);
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
                        return RedirectToAction("ListVisitors", new { err = "Visitor not updated: [" + ex.Message + "]"});
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