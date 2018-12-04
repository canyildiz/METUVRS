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


        private string _selectedType = null;

        [NotMapped]
        [Display(Name = "Sticker Type")]
        public string SelectedType
        {
            get => Quota?.Type == null ? _selectedType : Type.ID.ToString();
            set => _selectedType = value;
        }

        [NotMapped]
        public StickerType Type
        {
            get
            {
                return Quota.Type;
            }
        }

        [NotMapped]
        public StickerTerm Term
        {
            get
            {
                return Quota.Term;
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