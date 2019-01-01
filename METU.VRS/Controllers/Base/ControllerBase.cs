using METU.VRS.Models;
using METU.VRS.Services;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace METU.VRS.Controllers.Base
{
    public class ControllerBase : Controller
    {
        private readonly DatabaseContext db = null;
        public ControllerBase()
        {

        }

        public ControllerBase(DatabaseContext db)
        {
            //for mock/stub
            this.db = db;
        }

        protected DatabaseContext GetNewDBContext()
        {
            if (db == null)
            {
                return new DatabaseContext();
            }
            else
            {
                return db;
            }
        }

        protected IPagedList<StickerApplication> ListApplications(string sortOrder, string currentFilter, string searchString, int? page, StickerApplicationStatus? applicationStatus, User user = null)
        {
            IQueryable<StickerApplication> applications = null;

            using (DatabaseContext db = GetNewDBContext())
            {
                applications = db.StickerApplications
                    .AsNoTracking()
                    .Include("Vehicle")
                    .Include("Owner")
                    .Include("User.Category")
                    .Include("User.Division")
                    .Include("Quota.Type");
                if(null != user)
                {
                    applications = applications.Where(a => a.User == user);
                }

                if (null != applicationStatus)
                {
                    applications = applications.Where(a => a.Status == applicationStatus);
                }

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
                pageNumber = pageNumber >= 0 ? pageNumber : 1;
                return applications.ToPagedList(pageNumber, pageSize);
            }
        }
    }
}