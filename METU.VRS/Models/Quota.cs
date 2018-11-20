namespace METU.VRS.Models
{
    public class Quota
    {
        public int ID { get; set; }
        public string UID { get; set; }
        public int TotalQuota { get; set; }
        public decimal StickerFee { get; set; }

        public virtual BranchAffiliate Division { get; set; }
        public virtual StickerTerm Term { get; set; }
        public virtual StickerType Type { get; set; }

    }
}