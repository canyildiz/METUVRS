using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace METU.VRS.Models
{
    public class Payment
    {
        [ForeignKey("Application")]
        public int ID { get; set; }
        public int Amount { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime TransactionDate { get; set; }

        public virtual StickerApplication Application { get; set; }
    }
}