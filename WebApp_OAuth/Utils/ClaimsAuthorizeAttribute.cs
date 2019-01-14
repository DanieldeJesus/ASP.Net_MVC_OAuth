using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace WebApp_OAuth.Utils
{
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = filterContext.HttpContext.User as ClaimsPrincipal;

            if (user != null /*&& user.HasClaim(claimType, claimValue)*/)
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}