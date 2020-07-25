using System;
using Microsoft.Extensions.Logging;

using Identity.Models;
using XperienceAdapter.Logging;

namespace Identity
{
    /// <summary>
    /// Base class for identity-related managers.
    /// </summary>
    public abstract class BaseIdentityManager
    {
        protected readonly ILogger _logger;

        protected readonly IMedioClinicUserManager<MedioClinicUser> _userManager;

        public BaseIdentityManager(ILogger<BaseIdentityManager> logger, IMedioClinicUserManager<MedioClinicUser> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
    }
}