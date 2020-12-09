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

        public int AvatarId { get; set; }

        /// <summary>
        /// Explicit default constructor to satisfy requirements of <see cref="ApplicationUserStore{TUser}"/>.
        /// </summary>
        public MedioClinicUser()
        {
        }

        /// <summary>
        /// Creates a <see cref="MedioClinicUser"/> object out of a <see cref="UserInfo"/> one.
        /// </summary>
        /// <param name="userInfo">The input object.</param>
        public MedioClinicUser(UserInfo userInfo) : base(userInfo)
        {
        }

        public override void MapFromUserInfo(UserInfo source)
        {
            base.MapFromUserInfo(source);

            DateOfBirth = source.GetDateTimeValue("UserDateOfBirth", DateTime.MinValue);
            Gender = (Gender)source.UserSettings.UserGender;
            City = source.GetStringValue("City", string.Empty);
            Street = source.GetStringValue("Street", string.Empty);
            Phone = source.UserSettings.UserPhone;
            Nationality = source.GetStringValue("Nationality", string.Empty);
            //AvatarId = source.UserAvatarID;
            //commented out?
        }

        public override void MapToUserInfo(UserInfo target)
        {
            base.MapToUserInfo(target);

            target.UserSettings.UserDateOfBirth = DateOfBirth;
            target.UserSettings.UserGender = (int)Gender;
            target.UserSettings.UserPhone = Phone;
            target.SetValue("City", City);
            target.SetValue("Street", Street);
            target.SetValue("Nationality", Nationality);
        }
    }
}
