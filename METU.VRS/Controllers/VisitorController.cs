using METU.VRS.Models;
using METU.VRS.Services;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace METU.VRS.Controllers
{
    public class VisitorController : Base.ControllerBase
    {
        public VisitorController() : base()
        {
        }

        public VisitorController(DatabaseContext db) : base(db)
        {
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Apply()
        {
            return View(new Visitor());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(Visitor visitor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (DatabaseContext db = GetNewDBContext())
                    {
                        StickerApplication application = db.StickerApplications.
                            FirstOrDefault(s => s.Status == StickerApplicationStatus.Active && 
                            s.Vehicle.PlateNumber == visitor.Vehicle.PlateNumber);

                        if (application != null)
                        {
                            throw new ArgumentException("This vehicle has already an active sticker");
                        }


                        Visitor prevVisitor = db.Visitors.
                            FirstOrDefault(v =>
                            (v.Status == VisitorStatus.WaitingForApproval || v.Status == VisitorStatus.WaitingForArrival) &&
                            v.Vehicle.PlateNumber == visitor.Vehicle.PlateNumber &&
                            v.VisitDate == visitor.VisitDate &&
                            v.User.UID == visitor.User.UID);
                        if (prevVisitor != null)
                        {
                            throw new ArgumentException("This vehicle has already an active visit request on that day");
                        }


                        User user = db.Users.FirstOrDefault(u => u.UID == visitor.User.UID);
                        if (user == null)
                        {
                            throw new ArgumentException("Visitee's username is invalid");
                        }
                        else
                        {
                            visitor.User = user;
                        }

                        visitor.CreateDate = DateTime.Now;
                        visitor.LastModified = DateTime.Now;
                        visitor.EntryDate = null;
                        visitor.ApproveDate = null;
                        visitor.Status = VisitorStatus.WaitingForApproval;
                        visitor.UID = Guid.NewGuid().ToString().ToLower();
                        db.Visitors.Add(visitor);
                        db.SaveChanges();

                        return RedirectToAction("Detail", "Visitor", new { visitor.UID, success = 1 });
                    }
                }
                catch (System.Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View(visitor);
                }

            }
            else
            {
                return new EmptyResult();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Detail(string UID)
        {
            using (DatabaseContext db = GetNewDBContext())
            {
                Visitor visitor = db.Visitors.
                    Include(v => v.Vehicle).
                    Include(v => v.User).
                    FirstOrDefault(v => v.UID == UID);

                if (visitor == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(visitor);
                }
            }
        }
    }
}