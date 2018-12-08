using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace METU.VRS.Models.CT
{
    public class PaymentResponseFail
    {
        public string Response { get; set; }

        public string mdErrorMsg { get; set; }

        public int mdStatus { get; set; }

        public string ErrMsg  { get; set; }

        public string TransId { get; set; }

        private string returnOid;
        public string ReturnOid
        {
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