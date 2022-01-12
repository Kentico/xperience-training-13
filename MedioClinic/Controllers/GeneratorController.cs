using Common.Configuration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using XperienceAdapter.Generator;
using XperienceAdapter.Localization;

namespace MedioClinic.Controllers
{
    public class GeneratorController : BaseController
    {
        private const string ContactFilePath = "\\Generator\\Contacts.csv";

        private readonly IWebHostEnvironment _environment;

        private readonly IGenerator _generator;

        public GeneratorController(
            ILogger<GeneratorController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IWebHostEnvironment environment,
            IGenerator generator)
            : base(logger, optionsMonitor, stringLocalizer)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public IActionResult GenerateContacts()
        {
            var contactCompletePath = $"{_environment.ContentRootPath}{ContactFilePath}";

            try
            {
                _generator.GenerateContacts(contactCompletePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return Content($"Error: {ex.Message}");
            }

            return Content("Done");
        }
    }

}
