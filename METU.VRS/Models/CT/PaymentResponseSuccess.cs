using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace METU.VRS.Models.CT
{
    public class PaymentResponseSuccess
    {
        public string Response { get; set; }

        public int amount { get; set; }

        public string TransId { get; set; }

        public int mdStatus { get; set; }

        private string returnOid;
        public string ReturnOid {
            get { return returnOid; }
            set
            {
                returnOid = value;
                ApplicationId = value.Substring(0, value.IndexOf(PaymentRequest.OID_DELIMITER));
            }
        }

        public string ApplicationId { get; set; }
    }
}