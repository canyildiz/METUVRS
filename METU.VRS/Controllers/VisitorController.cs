﻿using METU.VRS.Models;
using METU.VRS.Services;
using PagedList;
using System;
using System.Data.Entity;
using System.Diagnostics;
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
            Trace.WriteLine("GET /Visitor/Index");
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Apply()
        {
            Trace.WriteLine("GET /Visitor/Apply");
            return View(new Visitor());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(Visitor visitor)
        {
            Trace.WriteLine("POST /Visitor/Apply");
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
            Trace.WriteLine("GET /Visitor/Detail");
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

        [HttpGet]
        public ActionResult List(string sortOrder, string currentFilter, string searchString, int? page)
        {
            Trace.WriteLine("GET /Visitor/List");

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

            IQueryable<Visitor> visitors = null;

            using (DatabaseContext db = GetNewDBContext())
            {
                visitors = db.Visitors
                    .AsNoTracking()
                    .Include(a => a.Vehicle)
                    .Include(a => a.User)
                    .Where(a => a.User.UID == User.Identity.Name);


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
                return View(visitors.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        public ActionResult Approve(int Id)
        {
            Trace.WriteLine("GET /Visitor/Approve");
            return SetVisitorStatus(Id, true);
        }

        [HttpGet]
        public ActionResult Reject(int Id)
        {
            Trace.WriteLine("GET /Visitor/Reject");
            return SetVisitorStatus(Id, false);
        }


        private ActionResult SetVisitorStatus(int Id, bool approve)
        {
            using (DatabaseContext db = GetNewDBContext())
            {
                var visitor = db.Visitors
                    .Include(v => v.User)
                    .Include(v => v.Vehicle)
                    .FirstOrDefault(v => v.ID == Id);

                if (visitor == null)
                {
                    throw new HttpAntiForgeryException("Visitor not found");
                }

                if (visitor.User.UID != User.Identity.Name)
                {
                    throw new HttpAntiForgeryException();
                }

                visitor.Status = approve ? VisitorStatus.WaitingForArrival : VisitorStatus.Rejected;
                visitor.ApproveDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("List");
            }
        }
    }
}