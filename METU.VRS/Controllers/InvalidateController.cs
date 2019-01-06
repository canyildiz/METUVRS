using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using METU.VRS.Services;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace METU.VRS.Controllers
{
    public class InvalidateController : Base.ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "delivery_user")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            Trace.WriteLine("GET /Invalidate/Index");

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.PlateSortParm = sortOrder == "Plate" ? "plate_desc" : "Plate";

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var res = University.GetStickerApplicationsByKeyword(sortOrder, currentFilter, searchString, page, StickerApplicationStatus.Active);
            return View(res);
        }

        [HttpGet]
        [Authorize(Roles = "delivery_user")]
        public ActionResult Invalidate(int Id)
        {
            Trace.WriteLine("GET /Invalidate/Invalidate");
            using (DatabaseContext db = GetNewDBContext())
            {
                var application = db.StickerApplications
                    .Include(a => a.User.Division)
                    .Include(a => a.User.Category)
                    .Include(a => a.Vehicle)
                    .Include(a => a.Sticker)
                    .Where(a => a.ID == Id)
                    .FirstOrDefault();

                if (application == null)
                {
                    return RedirectToAction("Index", new { error = "Sticker not found" });
                }


                try
                {
                    application.Status = StickerApplicationStatus.Invalidated;
                    application.LastModified = DateTime.Now;
                    db.SaveChanges();
                } catch(Exception ex)
                {
                    return RedirectToAction("Index", new { error = "Sticker could not be invaliated. Error: [" + ex.Message + "]"});
                }
                return RedirectToAction("Index", new { success=1 });
            }
        }
    }
}