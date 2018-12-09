using METU.VRS.Services;
using System.Collections.Generic;
using System.Linq;

namespace METU.VRS.Models
{
    public class UserCategory
    {
        public int ID { get; set; }
        public string UID { get; set; }
        public string Description { get; set; }

        public List<StickerType> EligibleStickerType()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.StickerTypes
                    .Where(s => s.UserCategory.UID == UID)
                    .OrderByDescending(s => s.ID)
                    .ToList();
            }
        }

        public bool CanApplyOnBehalfOf
        {
            get
            {
                return (UID == "affiliate");
            }
        }
    }
}