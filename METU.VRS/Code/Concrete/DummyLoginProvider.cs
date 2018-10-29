using METU.VRS.Abstract;
using METU.VRS.Models.Shared;

namespace METU.VRS.Concrete
{
    public class DummyLoginProvider : ILoginProvider
    {
        public LoginResult Login(string username, string password)
        {
            if (password == "fail")
            {
                return new LoginResult
                {
                    UserName = username,
                    Result = false
                };
            }
            else
            {
                return new LoginResult
                {
                    UserName = username,
                    Branch = "Test Graduation School",
                    FirstName = "John",
                    LastName = "Doe",
                    Result = true
                };
            }
        }
    }
}