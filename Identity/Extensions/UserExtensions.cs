using System;
using System.Collections.Generic;

using Identity.Models;

namespace Identity.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Converts standard ASP.NET Identity <see cref="string"/> roles to <see cref="Roles"/>.
        /// </summary>
        /// <param name="roles">ASP.NET Identity roles.</param>
        /// <returns>Strongly-typed <see cref="Roles"/> roles.</returns>
        public static Roles ToMedioClinicRoles(this IEnumerable<string> roles)
        {
            Roles foundRoles = Roles.None;

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (Enum.TryParse(role, out Roles parsedRole))
                    {
                        foundRoles |= parsedRole;
                    }
                } 
            }

            return foundRoles;
        }
    }
}
