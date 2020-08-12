using Core;

namespace Identity.Models.Profile
{
    /// <summary>
    /// Common interface of view models of users of various roles.
    /// </summary>
    public interface IUserViewModel
    {
        /// <summary>
        /// Data that's common to users of all roles.
        /// </summary>
        CommonUserViewModel CommonUserViewModel { get; set; }
    }
}
