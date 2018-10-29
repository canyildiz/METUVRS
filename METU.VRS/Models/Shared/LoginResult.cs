namespace METU.VRS.Models.Shared
{
    public class LoginResult
    {
        public string UserName { get; set; }

        public string[] Roles { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Branch { get; set; }

        public bool Result { get; set; }
    }
}