using METU.VRS.Controllers.Static;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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


        [NotMapped]
        [Display(Name = "Sticker Type")]
        public string SelectedType
        {
            get
            {
                return Type?.ID.ToString();
            }
            set
            {
                Type = University.GetStickerType(value);
            }
        }


        public virtual StickerType Type { get; set; }
        public virtual Vehicle Vehicle { get; set; } = new Vehicle();
        public virtual StickerTerm Term { get; set; }
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
        WaitingForDelivery = 20,
        Active = 30,
        Expired = 40,
        NotApproved = 110,
        NotDelivered = 120,
        Invalidated = 130,
        Aborted = 140
    }
}