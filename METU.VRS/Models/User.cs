using METU.VRS.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace METU.VRS.Models
{
    public class User
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "User Name")]
        [MaxLength(30)]
        public string UID { get; set; }

        public string Name { get; set; }

        public bool CanApplyForMore
        {
            get
            {
                if (Category != null && Category.CanApplyOnBehalfOf)
                {
                    return true;
                }
                else
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        return 0 == db.StickerApplications.Where(a => a.User.UID == UID && 
                        ((a.Quota.Term.EndDate>=DateTime.Now && a.Status == StickerApplicationStatus.Active) ||
                        a.Status == StickerApplicationStatus.WaitingForApproval ||
                        a.Status == StickerApplicationStatus.WaitingForPayment ||
                        a.Status == StickerApplicationStatus.WaitingForDelivery)).Count();
                    }
                }
            }
        }


        public virtual ICollection<UserRole> Roles { get; set; }

        public virtual UserCategory Category { get; set; }

        public virtual BranchAffiliate Division { get; set; }
    }
}