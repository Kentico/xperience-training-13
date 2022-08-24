using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using TinyCsvParser;

using CMS.ContactManagement;
using CMS.Globalization;
using CMS.OnlineForms;
using CMS.SiteProvider;
using CMS.DataEngine;
using CMS.Activities;
using CMS.Base;
using CMS.OnlineMarketing;
using CMS.LicenseProvider;
using CMS.WebAnalytics;
using System.Threading;
using CMS.DocumentEngine;

namespace XperienceAdapter.Generator
{
    public class Generator : IGenerator
    {
        private const string TestingCenterStartingDateDbName = "TestingCenterStartingDate";

        private const string NoPathMessage = "The path must be specified.";

        private const string NoCodenameMessage = "The codename must be specified";

        private const string UsThreeLetterCode = "USA";

        private static CsvParserOptions _csvParserOptions = new CsvParserOptions(true, ',');

        private readonly IActivityLogService _activityLogService;

        private readonly ISiteService _siteService;

        private readonly IAnalyticsLogger _analyticsLogger;

        private readonly IABTestManager _abTestManager;

        public Generator(IActivityLogService activityLogService,
                         ISiteService siteService,
                         IABTestConversionLogger abTestConversionLogger,
                         IAnalyticsLogger analyticsLogger,
                         IABTestManager abTestManager)
        {
            _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _analyticsLogger = analyticsLogger ?? throw new ArgumentNullException(nameof(analyticsLogger));
            _abTestManager = abTestManager ?? throw new ArgumentNullException(nameof(abTestManager));
        }

        public void GenerateContacts(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(NoPathMessage, nameof(path));
            }

            var mapping = new ContactMapping();
            var parser = new CsvParser<Contact>(_csvParserOptions, mapping);
            var contacts = parser?.ReadFromFile(path, Encoding.UTF8).ToList();

            var usCountryId = CountryInfo.Provider.Get()
                .WhereEquals(nameof(CountryInfo.CountryThreeLetterCode), UsThreeLetterCode)
                .TopN(1)
                .FirstOrDefault()?
                .CountryID;

            if (contacts?.Any() == true)
            {
                foreach (var contact in contacts)
                {
                    if (contact?.IsValid == true)
                    {
                        var result = contact.Result;

                        var stateId = StateInfo.Provider.Get()
                            .WhereEquals(nameof(StateInfo.StateCode), result.StateCode)
                            .TopN(1)
                            .FirstOrDefault()?
                            .StateID;

                        var contactInfo = new ContactInfo()
                        {
                            ContactFirstName = result.FirstName,
                            ContactLastName = result.LastName,
                            ContactEmail = result.EmailAddress,
                            ContactCity = result.City,
                            ContactMonitored = true
                        };

                        if (usCountryId.HasValue)
                        {
                            contactInfo.ContactCountryID = usCountryId.Value;
                        }

                        if (stateId.HasValue)
                        {
                            contactInfo.ContactStateID = stateId.Value;
                        }

                        contactInfo.SetValue(TestingCenterStartingDateDbName, result.TestingCenterStartingDate);
                        ContactInfo.Provider.Set(contactInfo);
                    }
                }
            }
        }

        public void GenerateFormData(string path, string formCodename, ITreeNode treeNode)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(NoPathMessage, nameof(path));
            }

            var mapping = new FormDataMapping();
            var parser = new CsvParser<FormData>(_csvParserOptions, mapping);
            var allFormData = parser?.ReadFromFile(path, Encoding.UTF8).ToList();

            if (allFormData?.Any() == true)
            {
                foreach (var formData in allFormData)
                {
                    if (formData.IsValid)
                    {
                        var formItem = SaveFormData(formCodename, formData.Result);

                        var contact = ContactInfo.Provider.Get()
                            .WhereEquals(nameof(ContactInfo.ContactEmail), formData.Result.EmailInput)
                            .TopN(1)
                            .FirstOrDefault();

                        if (contact != null)
                        {
                            LogFormSubmissionActivity(contact, treeNode, formItem);
                        }
                    }
                }
            }
        }

        public void GenerateActivities()
        {
            throw new NotImplementedException();
        }

        public void GenerateContactGroup()
        {
            throw new NotImplementedException();
        }

        public void GenerateAbTestConversions(TreeNode page, string requestDomain)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (string.IsNullOrEmpty(requestDomain))
            {
                throw new ArgumentException($"'{nameof(requestDomain)}' cannot be null or empty.", nameof(requestDomain));
            }

            LogRandomAbTestConversions(page, requestDomain);
        }

        private BizFormItem SaveFormData(string formCodename, FormData formData)
        {
            if (string.IsNullOrEmpty(formCodename))
            {
                throw new ArgumentException(NoCodenameMessage, nameof(formCodename));
            }

            var formObject = BizFormInfo.Provider.Get(formCodename, SiteContext.CurrentSiteID);

            if (formObject != null)
            {
                var formClass = DataClassInfoProvider.GetDataClassInfo(formObject.FormClassID);
                var formClassName = formClass.ClassName;
                var newFormItem = BizFormItem.New(formClassName);
                newFormItem.SetValue(nameof(FormData.CompanyName), formData.CompanyName);
                newFormItem.SetValue(nameof(FormData.Type), formData.Type);
                newFormItem.SetValue(nameof(FormData.ReasonsToJoin), formData.ReasonsToJoin);
                newFormItem.SetValue(nameof(FormData.FirstName), formData.FirstName);
                newFormItem.SetValue(nameof(FormData.LastName), formData.LastName);
                newFormItem.SetValue(nameof(FormData.EmailInput), formData.EmailInput);
                newFormItem.SetValue(nameof(FormData.StartingDate), formData.StartingDate);
                newFormItem.SetValue(nameof(FormData.PhotoOrMap), formData.PhotoOrMap);
                newFormItem.SetValue(nameof(FormData.ConsentAgreementForms), formData.ConsentAgreementForms);
                newFormItem.SetValue(nameof(FormData.ConsentAgreementFiles), formData.ConsentAgreementFiles);
                newFormItem.Insert();

                return newFormItem;
            }

            return null!;
        }

        private void LogFormSubmissionActivity(ContactInfo contactInfo, ITreeNode treeNode, BizFormItem formItem)
        {
            var activityInitializer = new FormSubmissionActivityInitializer(contactInfo, treeNode, formItem);
            _activityLogService.Log(activityInitializer);
        }

        private void LogRandomAbTestConversions(TreeNode page, string requestDomain)
        {
            var site = _siteService.CurrentSite;

            if (site == null 
                || !LicenseHelper.CheckFeature(requestDomain, FeatureEnum.ABTesting) 
                || !ABTestInfoProvider.ABTestingEnabled(site.SiteName))
            {
                return;
            }

            var abTestInfo = ABTestInfo.Provider.Get()
                .WhereEquals(nameof(ABTestInfo.ABTestOriginalPage), page.NodeAliasPath)
                .TopN(1)
                .FirstOrDefault();

            if (abTestInfo == null || !ABTestStatusEvaluator.ABTestIsRunning(abTestInfo))
            {
                return;
            }

            var conversions = abTestInfo.ABTestConversionConfiguration.ABTestConversions;
            var variants = _abTestManager.GetVariants(page);
            var randomizer = new Random();

            foreach (var variant in variants)
            {
                foreach (var conversion in conversions)
                {
                    var isConversionInAbTest = abTestInfo.ABTestConversionConfiguration.TryGetConversion(conversion.ConversionName, out _);

                    if (!isConversionInAbTest)
                    {
                        continue;
                    }

                    var conversionValue = GetConversionValue(abTestInfo, conversion.ConversionName, 1);
                    var codeNameSuffix = $"{abTestInfo.ABTestName};{variant.Guid}";
                    var randomHits = randomizer.Next(1, 100);

                    var data = new AnalyticsData(site.SiteID,
                                                 conversion.ConversionName,
                                                 hits: randomHits,
                                                 value: conversionValue,
                                                 culture: Thread.CurrentThread.CurrentCulture.Name);
                    
                    var name = $"absessionconversionrecurring;{codeNameSuffix}";
                    var randomPastDayNumber = randomizer.Next(-30, 0);
                    var randomPastDay = DateTimeOffset.UtcNow.AddDays(randomPastDayNumber).Date;

                    _analyticsLogger.LogCustomAnalytics(name, data, randomPastDay);

                    var statisticsName = $"abconversion;{codeNameSuffix}";

                    _analyticsLogger.LogCustomAnalytics(statisticsName, data, randomPastDay);
                }
            }
        }

        private static decimal GetConversionValue(ABTestInfo abTest, string conversionName, decimal value)
        {
            if (abTest.ABTestConversionConfiguration.TryGetConversion(conversionName, out var testConversion)
                && !testConversion.Value.Equals(0.0m))
            {
                return testConversion.Value;
            }

            return value;
        }
    }
}
