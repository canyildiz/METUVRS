using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using METU.VRS.Services;
using METU.VRS.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace METU.VRS.Controllers
{
    public class StickerController : Base.ControllerBase
    {
        public StickerController() : base()
        {
        }

        public StickerController(DatabaseContext db) : base(db)
        {
        }


        public ActionResult Index()
        {
            Trace.WriteLine("GET /Sticker/Index");
            using (DatabaseContext db = GetNewDBContext())
            {
                List<StickerApplication> applications = db.StickerApplications
                    .Include(s => s.Vehicle)
                    .Include(s => s.Quota.Type)
                    .Include(s => s.Quota.Term)
                    .Include(s => s.Owner)
                    .Include(s => s.Sticker)
                    .Where(s => s.User.UID == ((METUPrincipal)User).User.UID)
                    .OrderByDescending(s => s.LastModified)
                    .ToList();

                if (applications.Count == 0)
                {
                    return RedirectToAction("Apply");
                }
                else
                {
                    return View(applications);
                }
            }
        }

        [HttpGet]
        public ActionResult Apply()
        {
            Trace.WriteLine("GET /Sticker/Apply");
            if (((METUPrincipal)User).User.CanApplyForMore)
            {
                return View(new StickerApplication());
            }
            else
            {
                return RedirectToAction("Index", "Sticker", new { nomoresticker = "1" });
            }

        }

        [HttpGet]
        public ActionResult Renew(int Id)
        {
            Trace.WriteLine("GET /Sticker/Renew");
            ActionResult detail = Detail(Id);
            if (!(detail is ViewResult))
            {
                return detail;
            }

            StickerApplication application = (detail as ViewResult).Model as StickerApplication;
            ActionResult newApplication = Apply(application.Clone());

            if (!(newApplication is EmptyResult))
            {
                using (DatabaseContext db = GetNewDBContext())
                {
                    db.StickerApplications.Attach(application);
                    application.Status = StickerApplicationStatus.Expired;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(StickerApplication application)
        {
            Trace.WriteLine("POST /Sticker/Apply");

            if (!((METUPrincipal)User).User.CanApplyForMore)
            {
                return RedirectToAction("Index", "Sticker", new { nomoresticker = "1" });
            }

            if (ModelState.IsValid)
            {
                using (DatabaseContext db = GetNewDBContext())
                {

                    int count = db.StickerApplications.Where(a => a.Vehicle.PlateNumber == application.Vehicle.PlateNumber &&
                          (a.User.UID != ((METUPrincipal)User).User.UID ||
                              (a.User.UID == ((METUPrincipal)User).User.UID &&
                                  (a.Status == StickerApplicationStatus.Active ||
                                  a.Status == StickerApplicationStatus.WaitingForDelivery ||
                                  a.Status == StickerApplicationStatus.WaitingForPayment ||
                                  a.Status == StickerApplicationStatus.WaitingForApproval)))).Count();

                    if (count > 0)
                    {
                        return RedirectToAction("Index", new { vehicleAlreadyActive = "1" });
                    }

                    application.ApproveDate = null;
                    application.CreateDate = DateTime.Now;
                    application.DeliveryDate = null;
                    application.LastModified = DateTime.Now;

                    if (((METUPrincipal)User).User.Category.CanApplyOnBehalfOf)
                    {
                        application.Status = StickerApplicationStatus.WaitingForPayment;
                    }
                    else
                    {
                        application.Status = StickerApplicationStatus.WaitingForApproval;
                    }

                    application.Payment = null;

                    User user = University.GetUser(((METUPrincipal)User).User.UID);
                    Quota quota = University.GetQuotaForUser(user, University.GetStickerType(application.SelectedType));

                    application.User = user;
                    application.Quota = quota;

                    if (!user.Category.CanApplyOnBehalfOf)
                    {
                        application.Owner = new ApplicationOwner() { Name = user.Name };
                    }

                    db.Users.Attach(application.User);
                    db.Quotas.Attach(application.Quota);
                    db.StickerApplications.Add(application);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", new { ok = "1" });
            }
            else
            {
                return new EmptyResult();
            }
        }


        [HttpGet]
        public ActionResult Detail(int Id)
        {
            Trace.WriteLine("GET /Sticker/Detail");
            using (DatabaseContext db = GetNewDBContext())
            {
                var application = db.StickerApplications
                    .Include(a => a.Owner)
                    .Include(a => a.Payment)
                    .Include(a => a.Quota.Term)
                    .Include(a => a.Quota.Type)
                    .Include(a => a.Sticker)
                    .Include(a => a.User.Division)
                    .Include(a => a.User.Category)
                    .Include(a => a.Vehicle)
                    .Where(a => a.ID == Id)
                    .FirstOrDefault();

                if (application == null)
                {
                    return new HttpNotFoundResult();
                }

                if (!(User.IsInRole("delivery_user") || User.IsInRole("approval_user")) &&
                    application.User.ID != ((METUPrincipal)User).User.ID)
                {
                    return new HttpUnauthorizedResult();
                }
                else if (User.IsInRole("approval_user") && application.User.Division.ID != ((METUPrincipal)User).User.Division.ID)
                {
                    return new HttpUnauthorizedResult();
                }

                return View(application);
            }

        }
    }
}