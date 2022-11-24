using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.OnlineMarketing;

using Common.Configuration;
using XperienceAdapter.Localization;

namespace MedioClinic.Controllers
{
    public class AbTestConversionController : BaseController
    {
        private readonly IABTestConversionLogger _aBTestConversionLogger;

        private readonly ISiteService _siteService;

        public AbTestConversionController(
            ILogger<AbTestConversionController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IABTestConversionLogger aBTestConversionLogger,
            ISiteService siteService
            )
            : base(logger, optionsMonitor, stringLocalizer)
        {
            _aBTestConversionLogger = aBTestConversionLogger ?? throw new ArgumentNullException(nameof(aBTestConversionLogger));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }

        public IActionResult LogConversion(string conversionName, string parameter)
        {
            if (string.IsNullOrEmpty(conversionName) || string.IsNullOrEmpty(parameter))
            {
                _logger.LogError($"An A/B test conversion could not be logged due to missing data. Conversion name: {conversionName}, parameter: {parameter}");

                return BadRequest();
            }

            _aBTestConversionLogger.LogConversion(conversionName, _siteService.CurrentSite.SiteName, parameter);

            return Ok();
        }
    }
}
