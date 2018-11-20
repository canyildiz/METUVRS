using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace METU.VRS.Models
{
    public class ApplicationOwner
    {
        [ForeignKey("Application")]
        public int ID { get; set; }

        [Display(Name = "Application Owner")]
        public string Name { get; set; }

        public virtual StickerApplication Application { get; set; }
    }
}