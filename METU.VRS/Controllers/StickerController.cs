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
                    .Include(s => s.Quota)
                    .Include(s => s.Owner)
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
        [Authorize(Roles = "approval_user")]
        public ActionResult Approve()
        {
            Trace.WriteLine("GET /Sticker/Approve");
            using (DatabaseContext db = GetNewDBContext())
            {
                List<StickerApplication> applications = db.StickerApplications
                    .Where(a => a.Status == StickerApplicationStatus.WaitingForApproval)
                    .OrderByDescending(a => a.LastModified)
                    .ToList();

                return View(applications);
            }
        }
    }
}