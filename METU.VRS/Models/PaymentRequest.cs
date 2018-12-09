using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace METU.VRS.Models
{
    public class PaymentRequest
    {
        [ForeignKey("Application")]
        public int ID { get; set; }
        public int Amount { get; set; }
        [Display(Name = "Card Holder")]
        public string CardHolder { get; set; }

        //TODO Continue here, then update view

        public virtual StickerApplication Application { get; set; }
    }
}