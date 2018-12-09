using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace METU.VRS.Models.CT
{
    public class PaymentRequest
    {
        public const string OID_DELIMITER = "-";

        [Required]
        [Display(Name = "Card Holder")]
        public string CardHolder { get; set; }

        public int QuotaID { get; set; }
        public string oid { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "Card Number must be exactly 16 digits")]
        public string pan { get; set; }

        [Required]
        public string Ecom_Payment_Card_ExpDate_Month { get; set; }

        [Required]
        public string Ecom_Payment_Card_ExpDate_Year { get; set; }

        [Required]
        [Display(Name = "CVV")]
        [RegularExpression("^[0-99]{3}$", ErrorMessage ="CVV must be 3 digit number, written on back side of your card")]
        public string cv2 { get; set; }

        private StickerApplication application;
        public StickerApplication Application {
            get { return application; }
            set {
                application = value;
                oid = application.ID + OID_DELIMITER + System.DateTime.Now.ToString();
            }
        }

        public List<ExpiryMonthOption> ExpiryMonths() => new List<ExpiryMonthOption>
            {
                new ExpiryMonthOption() { Key="01", Value="January" },
                new ExpiryMonthOption() { Key="02", Value="February" },
                new ExpiryMonthOption() { Key="03", Value="March" },
                new ExpiryMonthOption() { Key="04", Value="April" },
                new ExpiryMonthOption() { Key="05", Value="May" },
                new ExpiryMonthOption() { Key="06", Value="June" },
                new ExpiryMonthOption() { Key="07", Value="July" },
                new ExpiryMonthOption() { Key="08", Value="August" },
                new ExpiryMonthOption() { Key="09", Value="September" },
                new ExpiryMonthOption() { Key="10", Value="October" },
                new ExpiryMonthOption() { Key="11", Value="November" },
                new ExpiryMonthOption() { Key="12", Value="December" }
            };
        public List<ExpiryYearOption> ExpiryYears() {
            List<ExpiryYearOption> retVal = new List<ExpiryYearOption>();
            int y = System.DateTime.Now.Year;
            for (int i = 0; i<=20; i++)
            {
                string yearVal = (y + i).ToString();
                retVal.Add(new ExpiryYearOption() { Value = yearVal, Key = yearVal.Substring((yearVal.Length -2)) });
            }
            return retVal;
        }
        public class ExpiryMonthOption
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class ExpiryYearOption
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}