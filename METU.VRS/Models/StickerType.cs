using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace METU.VRS.Models
{
    public class StickerType
    {
        public int ID { get; set; }

        [Display(Name = "Sticker Type")]
        public string Description { get; set; }
        public string Color { get; set; }
        public StickerClasses Class { get; set; }
        public TermTypes TermType { get; set; }

        public virtual UserCategory UserCategory { get; set; }
    }

    public enum StickerClasses
    {
        [Description("Staff Sticker")] Staff = 10,
        [Description("Foundation Sticker")] Foundation = 20,
        [Description("Visitor Sticker")] Visitor = 30,
        [Description("Alumni Sticker")] Alumni = 40,
        [Description("Technopolis Sticker")]Technopolis = 50,
        [Description("Student Sticker")] Student = 60,
        [Description("Student Parent Sticker")] StudentParent = 70
    }
}