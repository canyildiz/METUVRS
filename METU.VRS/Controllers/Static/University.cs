using METU.VRS.Models;
using METU.VRS.Services;
using METU.VRS.Services.Abstract;
using System;
using System.Collections.Generic;
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
                stickerTypes = db.StickerTypes.AsNoTracking().ToList();
                stickerTerms = db.StickerTerms.AsNoTracking().Where(s => DateTime.Now >= s.StartDate && DateTime.Now <= s.EndDate).ToList();

                loginProvider = new DummyLDAPProvider();
            }
            init = true;
        }


        public static UserRole GetRoleByUID(string UID)
        {
            return userRoles.Where(u => u.UID == UID).FirstOrDefault();
        }

        public static UserCategory GetCategoryByUID(string UID)
        {
            return userCategories.Where(u => u.UID == UID).FirstOrDefault();
        }

        public static BranchAffiliate GetBranchAffiliateByUID(string UID)
        {
            return branchsAffiliates.Where(b => b.UID == UID).FirstOrDefault();
        }

        public static StickerType GetStickerType(string ID)
        {
            return stickerTypes.Where(s => s.ID == Convert.ToInt32(ID)).FirstOrDefault();
        }

        public static StickerTerm GetOngoingTerm(TermTypes termType)
        {
            return stickerTerms.Where(s => s.Type == termType).FirstOrDefault();
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