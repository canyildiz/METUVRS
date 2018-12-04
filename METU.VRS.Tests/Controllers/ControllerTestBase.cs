using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;

namespace METU.VRS.Tests.Controllers
{
    public class ControllerTestBase
    {
        protected Mock<HttpContextBase> MockAuthContext(User mockUser)
        {
            var httpreq = new Mock<HttpRequestBase>();
            var httpctx = new Mock<HttpContextBase>();
            var authUser = new Mock<METU.VRS.UI.METUPrincipal>();
            authUser.Setup(u => u.User).Returns(mockUser);

            httpctx.Setup(ctx => ctx.Request).Returns(httpreq.Object);
            httpctx.Setup(ctx => ctx.User).Returns(authUser.Object);
            return httpctx;
        }
    }
}
