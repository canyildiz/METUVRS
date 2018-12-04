using METU.VRS.Controllers.Static;
using METU.VRS.Models.CT;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace METU.VRS.Models
{
    public class StickerApplication
    {
        public int ID { get; set; }

        public StickerApplicationStatus Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ApproveDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; set; }


        private string _selectedType = null;
        [NotMapped]
        [Display(Name = "Sticker Type")]
        public string SelectedType
        {
            get => Quota?.Type == null ? _selectedType : Type.ID.ToString();
            set => _selectedType = value;
        }

        [NotMapped]
        public StickerType Type => Quota.Type;

        [NotMapped]
        public StickerTerm Term => Quota.Term;

        public List<ApprovementOption> GetApprovementOptions()
        {
            if (Status != StickerApplicationStatus.WaitingForApproval)
            {
                return null;
            }
            else
            {
                var quotas = University.GetQuotasForUser(User);
                return quotas.Select(q => new ApprovementOption {
                    Description = q.Type.Description,
                    QuotaID = q.ID,
                    Remaining = q.RemainingQuota
                }).ToList();
            }
        }

        public virtual Vehicle Vehicle { get; set; } = new Vehicle();
        public virtual Quota Quota { get; set; }
        public virtual User User { get; set; }
        public virtual ApplicationOwner Owner { get; set; }
        public virtual Sticker Sticker { get; set; }
        public virtual Payment Payment { get; set; }
    }

    public enum StickerApplicationStatus
    {
        NotSet = 0,
        WaitingForApproval = 10,
        WaitingForPayment = 20,
        WaitingForDelivery = 30,
        Active = 40,
        Expired = 50,
        NotApproved = 110,
        NotDelivered = 120,
        Invalidated = 130,
        Aborted = 140
    }
}