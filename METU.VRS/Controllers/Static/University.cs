using METU.VRS.Models;
using METU.VRS.Services;
using METU.VRS.Services.Abstract;
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

        public static Quota GetQuotaForTerm(StickerTerm term, BranchAffiliate division)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.Quotas
                         .Where(q => q.Term.ID == term.ID && (q.Division == null || q.Division.UID == division.UID))
                         .OrderByDescending(q => q.Division.ID)
                         .FirstOrDefault();
            }
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