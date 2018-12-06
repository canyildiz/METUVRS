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
            return View(new StickerApplication());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(StickerApplication application)
        {
            Trace.WriteLine("POST /Sticker/Apply");

            if (ModelState.IsValid)
            {
                using (DatabaseContext db = GetNewDBContext())
                {
                    application.ApproveDate = null;
                    application.CreateDate = DateTime.Now;
                    application.DeliveryDate = null;
                    application.LastModified = DateTime.Now;

                    application.Status = StickerApplicationStatus.WaitingForApproval;
                    application.Payment = null;
                    application.Sticker = null;

                    User user = University.GetUser(((METUPrincipal)User).User.UID);
                    Quota quota = University.GetQuotaForUser(user, University.GetStickerType(application.SelectedType));

                    application.User = user;
                    application.Quota = quota;

                    application.Owner = new ApplicationOwner() { Name = user.Name };
                    db.Users.Attach(user);
                    db.Quotas.Attach(quota);
                    db.StickerApplications.Add(application);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View();
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