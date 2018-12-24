using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using METU.VRS.Services;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Security;

namespace METU.VRS.UI
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class METUPrincipal : ClaimsPrincipal
    {
        private readonly IIdentity formsIdentity;

        public METUPrincipal()
        {

        }

        public METUPrincipal(FormsAuthenticationTicket ticket)
        {
            formsIdentity = new FormsIdentity(ticket);
            User = University.GetUser(formsIdentity.Name);
        }

        public virtual User User { get; }

        public override IIdentity Identity
        {
            get { return formsIdentity; }
        }

        public override bool IsInRole(string role)
        {
            return (User.Roles.FirstOrDefault(r => r.UID == role) != null);
        }
    }
}