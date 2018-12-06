using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace METU.VRS.Models
{
    public class Sticker
    {
        [Key]
        public int FID { get; set; }

        public int SerialNumber { get; set; }

        [Required]
        public virtual StickerApplication Application { get; set; }
    }
}