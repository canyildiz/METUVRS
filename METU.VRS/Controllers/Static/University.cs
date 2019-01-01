using METU.VRS.Models;
using METU.VRS.Services;
using METU.VRS.Services.Abstract;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace METU.VRS.Controllers.Static
{
    public static class University
    {
        private static bool init = false;
        private static List<UserCategory> userCategories;
        private static List<UserRole> userRoles;
        private static List<BranchAffiliate> branchsAffiliates;
        private static List<StickerType> stickerTypes;
        private static List<StickerTerm> stickerTerms;
        private static ILoginProvider loginProvider;
        private static Dictionary<string, User> userCache = new Dictionary<string, User>();

        private static readonly string DEFAULT_ROLE = "authenticated_user";

        public static void Init()
        {
            if (init)
            {
                return;
            }

            using (DatabaseContext db = new DatabaseContext())
            {
                userCategories = db.UserCategories.ToList();
                userRoles = db.UserRoles.AsNoTracking().ToList();
                branchsAffiliates = db.BranchsAffiliates.AsNoTracking().ToList();
                stickerTypes = db.StickerTypes.Include(s => s.UserCategory).AsNoTracking().ToList();
                stickerTerms = db.StickerTerms.AsNoTracking().Where(s => DateTime.Now >= s.StartDate && DateTime.Now <= s.EndDate).ToList();

                loginProvider = new DummyLDAPProvider();
            }
            init = true;
        }


        public static UserRole GetRoleByUID(string UID)
        {
            return userRoles.FirstOrDefault(u => u.UID == UID);
        }

        public static UserCategory GetCategoryByUID(string UID)
        {
            return userCategories.FirstOrDefault(u => u.UID == UID);
        }

        public static BranchAffiliate GetBranchAffiliateByUID(string UID)
        {
            return branchsAffiliates.FirstOrDefault(b => b.UID == UID);
        }

        public static StickerType GetStickerType(string ID)
        {
            return stickerTypes.FirstOrDefault(s => s.ID == Convert.ToInt32(ID));
        }

        public static StickerType[] GetStickerTypes(UserCategory userCategory)
        {
            return stickerTypes.Where(s => s.UserCategory.ID == userCategory.ID).ToArray();
        }

        public static StickerTerm GetOngoingTerm(TermTypes termType)
        {
            return stickerTerms.FirstOrDefault(s => s.Type == termType);
        }

        public static Quota GetQuotaForUser(User user, StickerType stickerType)
        {
            BranchAffiliate division = user.Division;
            StickerTerm term = GetOngoingTerm(stickerType.TermType);
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.Quotas.AsNoTracking()
                    .Include(q => q.Term)
                    .Include(q => q.Type)
                    .Where(q => q.Term.ID == term.ID && q.Type.ID == stickerType.ID && (q.Division == null || q.Division.UID == division.UID))
                    .OrderByDescending(q => q.Division.ID)
                    .FirstOrDefault();
            }
        }

        public static Quota GetQuota(int Id)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.Quotas.AsNoTracking()
                    .Include(q => q.Term)
                    .Include(q => q.Type)
                    .Where(q => q.ID == Id)
                    .FirstOrDefault();
            }
        }

        public static List<Quota> GetQuotasForUser(User user)
        {
            BranchAffiliate division = user.Division;

            using (DatabaseContext db = new DatabaseContext())
            {
                List<Quota> ret = new List<Quota>();
                foreach (var type in GetStickerTypes(user.Category))
                {
                    StickerTerm term = GetOngoingTerm(type.TermType);
                    ret.AddRange(db.Quotas.AsNoTracking()
                        .Include(q => q.Term)
                        .Include(q => q.Type)
                        .Where(q => q.Term.ID == term.ID && q.Type.ID == type.ID && (q.Division == null || q.Division.UID == division.UID))
                        .ToList());
                }
                return ret.ToList();
            }
        }

        public static User GetUser(string UID)
        {
            if (userCache.ContainsKey(UID))
            {
                return userCache[UID];
            }

            using (DatabaseContext db = new DatabaseContext())
            {
                userCache.Add(UID, db.Users
                    .Include(u => u.Roles)
                    .Include(u => u.Division)
                    .Include(u => u.Category)
                    .Where(u => u.UID == UID).AsNoTracking()
                    .FirstOrDefault());
            }

            return userCache[UID];
        }

        public static IPagedList<Visitor> GetVisitorsByKeyword(string sortOrder, string currentFilter, string searchString, int? page, VisitorStatus? visitorStatus, User user = null)
        {
            IQueryable<Visitor> visitors = null;

            using (DatabaseContext db = new DatabaseContext())
            {
                visitors = db.Visitors
                    .AsNoTracking()
                    .Include(a => a.Vehicle)
                    .Include(a => a.User);

                if (null != user)
                {
                    visitors = visitors.Where(a => a.User.UID == user.UID);
                }

                if (visitorStatus.HasValue)
                {
                    visitors = visitors.Where(a => a.Status == visitorStatus);
                }

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
                pageNumber = pageNumber > 0 ? pageNumber : 1;
                return visitors.ToPagedList(pageNumber, pageSize);
            }
        }
        public static IPagedList<StickerApplication> GetStickerApplicationsByKeyword(string sortOrder, string currentFilter, string searchString, int? page, StickerApplicationStatus? applicationStatus, User user = null, BranchAffiliate division = null)
        {
            IQueryable<StickerApplication> applications = null;

            using (DatabaseContext db = new DatabaseContext())
            {
                applications = db.StickerApplications
                    .AsNoTracking()
                    .Include(a => a.Vehicle)
                    .Include(a => a.Owner)
                    .Include(a => a.User.Category)
                    .Include(a => a.User.Division)
                    .Include(a => a.Quota.Type);

                if (null != user)
                {
                    applications = applications.Where(a => a.User == user);
                }

                if (null != applicationStatus)
                {
                    applications = applications.Where(a => a.Status == applicationStatus);
                }

                if (null != division)
                {
                    applications = applications.Where(a => a.User.Division.ID == division.ID);
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
                pageNumber = pageNumber > 0 ? pageNumber : 1;
                return applications.ToPagedList(pageNumber, pageSize);
            }
        }


        public static User TryLogin(string username, string password)
        {
            LDAPResult result = loginProvider.Login(username, password);
            if (!result.Result)
            {
                return null;
            }
            else
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    User user = db.Users.Where(u => u.UID == result.UID).FirstOrDefault();
                    if (user != null)
                    {
                        user.Name = result.CN;
                        user.Category = GetCategoryByUID(result.DC);
                        user.Division = GetBranchAffiliateByUID(result.OU);
                    }
                    else
                    {
                        user = new User()
                        {
                            UID = result.UID,
                            Name = result.CN,
                            Roles = { GetRoleByUID(DEFAULT_ROLE) },
                            Category = GetCategoryByUID(result.DC),
                            Division = GetBranchAffiliateByUID(result.OU)
                        };

                        db.Users.Add(user);
                    }

                    db.SaveChanges();
                    return user;
                }
            }
        }
    }
}