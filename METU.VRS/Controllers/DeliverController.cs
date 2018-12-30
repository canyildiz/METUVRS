using METU.VRS.Models;
using METU.VRS.Services;
using PagedList;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace METU.VRS.Controllers
{
    public class DeliverController : Base.ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "delivery_user")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            Trace.WriteLine("GET /Deliver/Index");

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.PlateSortParm = sortOrder == "Plate" ? "plate_desc" : "Plate";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            IQueryable<StickerApplication> applications = null;

            using (DatabaseContext db = GetNewDBContext())
            {
                applications = db.StickerApplications
                    .AsNoTracking()
                    .Include(a => a.Vehicle)
                    .Include(a => a.Owner)
                    .Include(a => a.User.Category)
                    .Include(a => a.User.Division)
                    .Include(a => a.Quota.Type)
                    .Where(a => a.Status == StickerApplicationStatus.WaitingForDelivery);


                if (!string.IsNullOrEmpty(searchString))
                {
                    applications = applications.Where(a =>
                    a.User.Name.Contains(searchString)
                    || a.User.UID.Equals(searchString)
                    || a.Owner.Name.Contains(searchString)
                    || a.Vehicle.PlateNumber.Contains(searchString)
                    || a.Vehicle.RegistrationNumber.Contains(searchString)
                    || a.Vehicle.OwnerName.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "name_desc":
                        applications = applications.OrderByDescending(s => s.Owner.Name);
                        break;
                    case "Date":
                        applications = applications.OrderBy(s => s.LastModified);
                        break;
                    case "date_desc":
                        applications = applications.OrderByDescending(s => s.LastModified);
                        break;
                    case "Plate":
                        applications = applications.OrderBy(s => s.Vehicle.PlateNumber);
                        break;
                    case "plate_desc":
                        applications = applications.OrderByDescending(s => s.Vehicle.PlateNumber);
                        break;
                    default:
                        applications = applications.OrderBy(s => s.Owner.Name);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(applications.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        [Authorize(Roles = "delivery_user")]
        public ActionResult Deliver(int Id)
        {
            Trace.WriteLine("GET /Deliver/Deliver");
            using (DatabaseContext db = GetNewDBContext())
            {
                Sticker sticker = db.Stickers.Where(s => s.FID == Id).FirstOrDefault();
                return View(sticker);
            }
        }

        [HttpPost]
        [Authorize(Roles = "delivery_user")]
        [ValidateAntiForgeryToken]
        public ActionResult Deliver(int Id, Sticker sticker)
        {
            Trace.WriteLine("POST /Deliver/Deliver");
            using (DatabaseContext db = GetNewDBContext())
            {
                var application = db.StickerApplications
                    .Include(a => a.User.Division)
                    .Include(a => a.User.Category)
                    .Include(a => a.Vehicle)
                    .Where(a => a.ID == Id)
                    .FirstOrDefault();

                if (application == null)
                {
                    throw new HttpAntiForgeryException("Application not found");
                }
                int userId = sticker.FID == 0 ? 0 : Convert.ToInt32(db.StickerApplications.Where(a => a.ID == sticker.FID).Select(a => a.User.ID).FirstOrDefault());
                var count = db.Stickers.Where(s => s.SerialNumber == sticker.SerialNumber && s.Application.User.ID != userId).Count();
                if (count > 0 || (sticker.SerialNumber == 0 && sticker.FID == 0))
                {
                    return RedirectToAction("Index", new { stickerInUse = "1" });
                }
                else
                {


                    if (sticker.FID == 0)
                    {
                        application.Sticker = new Sticker { SerialNumber = sticker.SerialNumber };
                    }

                    application.Status = StickerApplicationStatus.Active;
                    application.DeliveryDate = DateTime.Now;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "delivery_user")]
        public ActionResult Reject(int Id)
        {
            Trace.WriteLine("GET /Deliver/Reject");

            using (DatabaseContext db = GetNewDBContext())
            {
                var application = db.StickerApplications
                    .Include(a => a.User.Division)
                    .Include(a => a.Vehicle)
                    .Where(a => a.ID == Id)
                    .FirstOrDefault();

                if (application == null)
                {
                    return new HttpNotFoundResult();
                }

                application.Status = StickerApplicationStatus.NotDelivered;
                application.DeliveryDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}