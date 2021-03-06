﻿using METU.VRS.Controllers.Static;
using METU.VRS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Web;

namespace METU.VRS.Tests.Controllers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ControllerTestBase
    {
        protected Mock<HttpContextBase> MockAuthContext(User mockUser)
        {
            var httpreq = new Mock<HttpRequestBase>();
            var httpctx = new Mock<HttpContextBase>();
            var authUser = new Mock<METU.VRS.UI.METUPrincipal>();
            authUser.Setup(u => u.User).Returns(mockUser);
            authUser.Setup(u => u.Identity.Name).Returns(mockUser.UID);
            authUser.Setup(u => u.Identity.IsAuthenticated).Returns(true);
            authUser.Setup(u => u.IsInRole(It.IsAny<string>()))
                .Returns((string role) => mockUser.Roles.Where(u => u.UID == role).Count() == 1);
            httpctx.Setup(ctx => ctx.Request).Returns(httpreq.Object);
            httpctx.Setup(ctx => ctx.User).Returns(authUser.Object);
            return httpctx;
        }

        [TestInitialize]
        public void Init()
        {
            Migrations.TestDatabase.WipeTestDatabase();
            University.Init();
        }
    }
}
