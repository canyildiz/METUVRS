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
        Staff = 10,
        Foundation = 20,
        Visitor = 30,
        Alumni = 40,
        Technopolis = 50,
        Student = 60,
        StudentParent = 70
    }
}