using METU.VRS.Models;
using METU.VRS.Services;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Security;

namespace METU.VRS.UI
{
    public class METUPrincipal : ClaimsPrincipal
    {
        private IIdentity formsIdentity;

        public METUPrincipal()
        {

        }

        public METUPrincipal(FormsAuthenticationTicket ticket)
        {
            formsIdentity = new FormsIdentity(ticket);
            using (DatabaseContext db = new DatabaseContext())
            {
                User = db.Users
                    .Include(u => u.Category)
                    .Include(u => u.Division)
                    .Include(u => u.Roles)
                    .Where(u => u.UID == formsIdentity.Name)
                    .AsNoTracking()
                    .FirstOrDefault();
            }

        }

        public virtual User User { get; }

        public override IIdentity Identity
        {
            get { return formsIdentity; }
        }

        public override bool IsInRole(string role)
        {
            return (User.Roles.Where(r => r.UID == role).FirstOrDefault() != null);
        }
    }
}