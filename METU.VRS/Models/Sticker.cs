using System.ComponentModel.DataAnnotations.Schema;

namespace METU.VRS.Models
{
    public class Sticker
    {
        [ForeignKey("Application")]
        public int ID { get; set; }
        public int SerialNumber { get; set; }

        public virtual StickerApplication Application { get; set; }
    }
}