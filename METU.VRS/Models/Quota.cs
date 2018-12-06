using METU.VRS.Services;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace METU.VRS.Models
{
    public class Quota
    {
        public int ID { get; set; }
        public string UID { get; set; }
        public int TotalQuota { get; set; }
        public int StickerFee { get; set; }

        [NotMapped]
        public int RemainingQuota
        {
            get
            {
                if (TotalQuota == -1)
                {
                    return -1;
                }

                using (DatabaseContext db = new DatabaseContext())
                {
                    int usedQuota = db.StickerApplications.Where(a => a.Quota.ID == ID && 
                    (a.Status == StickerApplicationStatus.Active || 
                    a.Status == StickerApplicationStatus.WaitingForDelivery || 
                    a.Status == StickerApplicationStatus.WaitingForPayment
                    )).Count();
                    return TotalQuota - usedQuota;
                }
            }
        }

        public virtual BranchAffiliate Division { get; set; }
        public virtual StickerTerm Term { get; set; }
        public virtual StickerType Type { get; set; }

    }
}