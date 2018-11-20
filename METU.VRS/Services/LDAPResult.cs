namespace METU.VRS.Services
{
    public class LDAPResult
    {
        public string UID { get; set; }

        public string CN { get; set; }

        public string OU { get; set; }

        public string DC { get; set; }

        public bool Result { get; set; }
    }
}