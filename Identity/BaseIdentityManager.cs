using System;
using Microsoft.Extensions.Logging;

using Identity.Models;
using XperienceAdapter.Logging;
using Microsoft.Extensions.Localization;
using XperienceAdapter.Localization;

namespace Identity
{
    /// <summary>
    /// Base class for identity-related managers.
    /// </summary>
    public abstract class BaseIdentityManager
    {
        protected readonly ILogger _logger;

        protected readonly IStringLocalizer<SharedResource> _stringLocalizer;

        protected readonly IMedioClinicUserManager<MedioClinicUser> _userManager;

        public BaseIdentityManager(ILogger<BaseIdentityManager> logger,
                                   IStringLocalizer<SharedResource> stringLocalizer,
                                   IMedioClinicUserManager<MedioClinicUser> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Logs exceptions and gets a result object.
        /// </summary>
        /// <typeparam name="TResultState">Result states of the client code.</typeparam>
        /// <param name="methodName">Method name to log.</param>
        /// <param name="exception">An exception to log.</param>
        /// <param name="result">An operation result.</param>
        protected void HandleException<TResultState>(string methodName, Exception exception, ref IdentityManagerResult<TResultState> result)
            where TResultState : Enum
        {
            _logger.LogEvent(LogLevel.Error, methodName, exception: exception);
            result.Success = false;
            result.Errors.Add(exception.Message);
        }

        /// <summary>
        /// Localizes a string resource.
        /// </summary>
        /// <param name="resourceKey">Resource key.</param>
        /// <returns></returns>
        protected string Localize(string resourceKey) => _stringLocalizer[resourceKey];

        /// <summary>
        /// Localizes a string resource using a pattern.
        /// </summary>
        /// <param name="resourceKey">Resource key.</param>
        /// <param name="args">The values to format the string with.</param>
        /// <returns></returns>
        protected string Localize(string resourceKey, params object[] args) => _stringLocalizer[resourceKey, args];
    }
}