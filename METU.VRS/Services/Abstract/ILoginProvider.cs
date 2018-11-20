namespace METU.VRS.Services.Abstract
{
    public interface ILoginProvider
    {
        LDAPResult Login(string username, string password);
    }

}
