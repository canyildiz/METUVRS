using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using METU.VRS.Models.CT;
using METU.VRS.Services;
using METU.VRS.UI;
using PagedList;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace METU.VRS.Controllers
{
    public class ApproveController : Base.ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "approval_user")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            Trace.WriteLine("GET /Approve/Index");

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
                    .Where(a => a.Status == StickerApplicationStatus.WaitingForApproval
                    && a.User.Division.ID == ((METUPrincipal)User).User.Division.ID);


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
        [Authorize(Roles = "approval_user")]
        public ActionResult Approve(int Id, int QuotaID)
        {
            Trace.WriteLine("GET /Approve/Approve");
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

                if (application.User.Division.ID != ((METUPrincipal)User).User.Division.ID)
                {
                    throw new HttpAntiForgeryException();
                }

                ApprovementOption approvementOption = application.GetApprovementOptions().FirstOrDefault(a => a.QuotaID == QuotaID);
                if (approvementOption == null)
                {
                    throw new HttpAntiForgeryException();
                }

                Quota quota = University.GetQuota(approvementOption.QuotaID);
                if (quota.RemainingQuota == 0)
                {
                    return RedirectToAction("Index");
                }
                db.Quotas.Attach(quota);
                application.Status = StickerApplicationStatus.WaitingForPayment;
                application.ApproveDate = DateTime.Now;
                application.Quota = quota;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize(Roles = "approval_user")]
        public ActionResult Reject(int Id)
        {
            Trace.WriteLine("GET /Approve/Reject");

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

                if (application.User.Division.ID != ((METUPrincipal)User).User.Division.ID)
                {
                    return new HttpUnauthorizedResult();
                }

                application.Status = StickerApplicationStatus.NotApproved;
                application.ApproveDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}