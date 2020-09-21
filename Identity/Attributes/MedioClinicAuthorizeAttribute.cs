using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using EnumsNET;

using CMS.Membership;
using CMS.SiteProvider;

using Identity.Extensions;
using Identity.Models;

namespace Identity.Attributes
{
    public class MedioClinicAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public new Roles Roles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var user = context.HttpContext?.User;

            if (user != null)
            {
                var userRoles = UserInfoProvider.GetRolesForUser(user.Identity?.Name, SiteContext.CurrentSiteName).ToMedioClinicRoles();

                if (user.Identity?.IsAuthenticated == false || !FlagEnums.HasAnyFlags(Roles, userRoles))
                {
                    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);

                    return;
                }
            }
        }
    }
}
