using Abstractions;

namespace Identity.Models.Profile
{
    /// <summary>
    /// Common interface of view models of users of various roles.
    /// </summary>
    public interface IUserViewModel : IViewModel
    {
        /// <summary>
        /// Data that's common to users of all roles.
        /// </summary>
        CommonUserViewModel CommonUserViewModel { get; set; }
    }
}
