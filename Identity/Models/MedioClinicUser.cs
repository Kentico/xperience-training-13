using System;

using CMS.Membership;
using Kentico.Membership;

namespace Identity.Models
{
    /// <summary>
    /// A derived <see cref="Kentico.Membership.ApplicationUser"/> class created for the purpose of the Medio Clinic website.
    /// </summary>
    /// <remarks>Designed to contain all role-specific properties of all users.</remarks>
    public class MedioClinicUser : ApplicationUser
    {
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public string? City { get; set; }

        public string? Street { get; set; }

        public string? Phone { get; set; }

        public string? Nationality { get; set; }
    }
}
