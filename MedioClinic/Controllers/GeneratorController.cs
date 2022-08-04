﻿using CMS.DocumentEngine;

using Common.Configuration;

using Kentico.Content.Web.Mvc;

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
        private const string GeneratorDataPath = "\\Generator\\";

        private const string ContactFilePath = GeneratorDataPath + "Contacts.csv";

        private const string FormDataFilePath = GeneratorDataPath + "FormData.csv";

        private const string AllergyTestCenterPagePath = "/Landing-pages/Allergy-test-center-partner-program";

        private const string FormCodename = "AllergyTestCenterApplication";

        private const string NoCsvFile = "The .csv file name must be specified.";

        private const string ContactsGenerated = "The contacts have been generated.";

        private const string FormDataGenerated = "The form data has been generated.";

        private readonly IWebHostEnvironment _environment;

        private readonly IPageRetriever _pageRetriever;

        private readonly IGenerator _generator;

        private IPageMetadata PageMetadata => new Models.PageMetadata { Title = "Data generator" };

        public GeneratorController(
            ILogger<GeneratorController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IWebHostEnvironment environment,
            IPageRetriever pageRetriever,
            IGenerator generator)
            : base(logger, optionsMonitor, stringLocalizer)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _pageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        [HttpGet]
        public IActionResult Index() => DefaultView();

        // POST: Generator/GenerateContacts
        [HttpPost]
        public IActionResult GenerateContacts() =>
            GenerateData(_generator.GenerateContacts, ContactFilePath, ContactsGenerated);

        // POST: Generator/GenerateFormData
        [HttpPost]
        public IActionResult GenerateFormData()
        {
            var landingPage = _pageRetriever.Retrieve<TreeNode>(query =>
                {
                    query.Path(AllergyTestCenterPagePath, PathTypeEnum.Single);
                })
                .FirstOrDefault();

            var completePath = $"{_environment.ContentRootPath}{FormDataFilePath}";

            try
            {
                _generator.GenerateFormData(completePath, FormCodename, landingPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return ErrorMessage(ex);
            }

            return DefaultView(FormDataGenerated);
        }

        private IActionResult GenerateData(Action<string> generatorAction, string csvFileName, string successMessage)
        {
            if (string.IsNullOrEmpty(csvFileName))
            {
                throw new ArgumentException(NoCsvFile, nameof(csvFileName));
            }

            if (generatorAction is null)
            {
                throw new ArgumentNullException(nameof(generatorAction));
            }

            var completePath = $"{_environment.ContentRootPath}{csvFileName}";

            try
            {
                generatorAction(completePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return ErrorMessage(ex);
            }

            return DefaultView(successMessage);
        }

        private IActionResult DefaultView(string? message = default)
        {
            var viewModel = GetPageViewModel(
                PageMetadata,
                message,
                true,
                false,
                Models.MessageType.Info);

            return View(nameof(this.Index), viewModel);
        }

        private IActionResult ErrorMessage(Exception ex)
        {
            var viewModel = GetPageViewModel(
                PageMetadata,
                ex.Message,
                true,
                false,
                Models.MessageType.Error);

            return View(nameof(this.Index), viewModel);
        }
    }
}