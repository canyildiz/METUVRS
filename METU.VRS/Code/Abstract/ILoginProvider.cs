using METU.VRS.Models.Shared;

namespace METU.VRS.Abstract
{
    public interface ILoginProvider
    {
     LoginResult Login(string username, string password);
    }

}
