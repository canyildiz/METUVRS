using System.ComponentModel.DataAnnotations;

namespace METU.VRS.Models
{
    public class Sticker
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [RegularExpression(@"\d{1,16}", ErrorMessage = "Serial number must be a number")]
        public int SerialNumber { get; set; }

        [Required]
        public virtual StickerApplication Application { get; set; }
    }
}