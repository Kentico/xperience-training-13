using CMS.Activities;
using CMS.Base;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Globalization;
using CMS.LicenseProvider;
using CMS.Newsletters;
using CMS.OnlineForms;
using CMS.OnlineMarketing;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.WebAnalytics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using TinyCsvParser;

namespace XperienceAdapter.Generator
{
    public class Generator : IGenerator
    {
        private const string TestingCenterStartingDateDbName = "TestingCenterStartingDate";

        private const string NoPathMessage = "The path must be specified.";

        private const string NoCodenameMessage = "The codename must be specified";

        private const string UsThreeLetterCode = "USA";

        private const int LoggingHistoryDays = 30;

        private const string mailingTaskName = "NewsletterSender";

        private static CsvParserOptions _csvParserOptions = new CsvParserOptions(true, ',');

        private readonly IActivityLogService _activityLogService;

        private readonly ISiteService _siteService;

        private readonly IAnalyticsLogger _analyticsLogger;

        private readonly IABTestManager _abTestManager;

        private readonly IIssueScheduler _issueScheduler;

        public Generator(IActivityLogService activityLogService,
                         ISiteService siteService,
                         IAnalyticsLogger analyticsLogger,
                         IABTestManager abTestManager,
                         IIssueScheduler issueScheduler)
        {
            _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _analyticsLogger = analyticsLogger ?? throw new ArgumentNullException(nameof(analyticsLogger));
            _abTestManager = abTestManager ?? throw new ArgumentNullException(nameof(abTestManager));
            _issueScheduler = issueScheduler ?? throw new ArgumentNullException(nameof(issueScheduler));

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

        public List<string> GenerateNewsletterSubscribers(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(NoPathMessage, nameof(path));
            }

            var mapping = new NewsletterSubscriberMapping();
            var parser = new CsvParser<NewsletterSubscriber>(_csvParserOptions, mapping);
            var allSubscribers = parser?.ReadFromFile(path, Encoding.UTF8).ToList();
            var successfullSaves = new List<string>();

            if (allSubscribers?.Any() == true)
            {
                foreach (var subscriber in allSubscribers)
                {
                    if (subscriber.IsValid)
                    {
                        var relatedContact = ContactInfo.Provider.Get()
                            .WhereEquals(nameof(ContactInfo.ContactEmail), subscriber.Result.SubscriberEmail)
                            .TopN(1)
                            .FirstOrDefault();

                        var newsletter = NewsletterInfo.Provider.Get()
                            .WhereEquals(NewsletterInfo.TYPEINFO.CodeNameColumn, subscriber.Result.NewsletterName)
                            .TopN(1)
                            .FirstOrDefault();

                        if (relatedContact != null && newsletter != null)
                        {
                            try
                            {
                                SaveNewsletterSubscriber(relatedContact, newsletter.NewsletterID);

                                successfullSaves.Add(subscriber.Result.SubscriberEmail!);
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                }
            }

            return successfullSaves;
        }

        public void GenerateNewsletterOpensAndLinkClicks(string path, List<string> subscribers)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(NoPathMessage, nameof(path));
            }

            if (subscribers is null)
            {
                throw new ArgumentNullException(nameof(subscribers));
            }

            var randomizer = new Random();
            var mapping = new NewsletterLinkClickMapping();
            var parser = new CsvParser<NewsletterLinkClick>(_csvParserOptions, mapping);
            var allClicks = parser?.ReadFromFile(path, Encoding.UTF8).ToList();

            if (allClicks?.Any() == true)
            {
                foreach (var click in allClicks)
                {
                    if (click.IsValid && subscribers.Contains(click.Result.ClickedLinkEmail!))
                    {
                        var result = click.Result;
                        var newsletter = NewsletterInfo.Provider.Get().WithCodeName(result.NewsletterName).TopN(1).FirstOrDefault();

                        var issue = IssueInfo.Provider.Get()
                            .WhereEquals(nameof(IssueInfo.IssueDisplayName), result.IssueDisplayName)
                            .WhereEquals(nameof(IssueInfo.IssueNewsletterID), newsletter?.NewsletterID)
                            .TopN(1)
                            .FirstOrDefault();

                        if (newsletter != null && issue != null)
                        {
                            EnsureEmailSending(issue);
                            var randomDate = DateTime.UtcNow.AddDays(randomizer.Next(-30, -1));
                            LogEmailOpening(result.ClickedLinkEmail!, issue, randomDate);
                            LogLinkClick(result.ClickedLinkEmail!, randomDate, issue, randomizer);
                        }
                    }
                }
            }
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
                || !CMS.OnlineMarketing.ABTestInfoProvider.ABTestingEnabled(site.SiteName))
            {
                return;
            }

            var abTestInfo = CMS.OnlineMarketing.ABTestInfo.Provider.Get()
                .WhereEquals(nameof(CMS.OnlineMarketing.ABTestInfo.ABTestOriginalPage), page.NodeAliasPath)
                .TopN(1)
                .FirstOrDefault();

            if (abTestInfo == null || !ABTestStatusEvaluator.ABTestIsRunning(abTestInfo))
            {
                return;
            }

            abTestInfo.ABTestOpenFrom = DateTimeOffset.UtcNow.AddDays(0 - LoggingHistoryDays).Date;
            CMS.OnlineMarketing.ABTestInfo.Provider.Set(abTestInfo);

            var conversions = abTestInfo.ABTestConversionConfiguration.ABTestConversions;
            var variants = _abTestManager.GetVariants(page);
            var randomizer = new Random();

            for (int i = 0; i < LoggingHistoryDays; i++)
            {
                var day = DateTime.UtcNow.AddDays(i - LoggingHistoryDays);

                foreach (var variant in variants)
                {
                    foreach (var conversion in conversions)
                    {
                        var isConversionInAbTest = abTestInfo.ABTestConversionConfiguration.TryGetConversion(conversion.ConversionName, out _);

                        if (!isConversionInAbTest)
                        {
                            continue;
                        }

                        var visitHitName = $"abvisitfirst;{abTestInfo.ABTestName};{variant.Guid}";
                        var visitData = new AnalyticsData(site.SiteID, conversion.ConversionName);

                        _analyticsLogger.LogCustomAnalytics(visitHitName, visitData, day);

                        var conversionValue = GetConversionValue(abTestInfo, conversion.ConversionName, 1);
                        var codeNameSuffix = $"{abTestInfo.ABTestName};{variant.Guid}";
                        var randomLowHigh = randomizer.Next(0, 1);
                        var randomLowHits = randomizer.Next(1, 100);
                        var randomHighHits = randomizer.Next(2000, 3000);
                        var randomHits = randomLowHigh == 0 ? randomLowHits : randomHighHits;

                        var conversionData = new AnalyticsData(site.SiteID,
                                                     conversion.ConversionName,
                                                     hits: randomHits,
                                                     value: conversionValue,
                                                     culture: Thread.CurrentThread.CurrentCulture.Name);

                        var conversionRecurringHitName = $"absessionconversionrecurring;{codeNameSuffix}";

                        _analyticsLogger.LogCustomAnalytics(conversionRecurringHitName, conversionData, day);

                        var statisticsName = $"abconversion;{codeNameSuffix}";

                        _analyticsLogger.LogCustomAnalytics(statisticsName, conversionData, day);
                    }
                }
            }
        }


        private static decimal GetConversionValue(CMS.OnlineMarketing.ABTestInfo abTest, string conversionName, decimal value)
        {
            if (abTest.ABTestConversionConfiguration.TryGetConversion(conversionName, out var testConversion)
                && !testConversion.Value.Equals(0.0m))
            {
                return testConversion.Value;
            }

            return value;
        }

        private static SubscriberInfo? GetExistingSubscriber(ContactInfo contact) => SubscriberInfo.Provider.Get()
            .WhereEquals(nameof(SubscriberInfo.SubscriberEmail), contact.ContactEmail)
            .TopN(1)
            .FirstOrDefault();

        private void SaveNewsletterSubscriber(ContactInfo contact, int newsletterId)
        {
            var existingSubscriber = GetExistingSubscriber(contact);

            if (existingSubscriber is null)
            {
                var subscriberInfo = new SubscriberInfo
                {
                    SubscriberEmail = contact.ContactEmail,
                    SubscriberFirstName = contact.ContactFirstName,
                    SubscriberLastName = contact.ContactLastName,
                    SubscriberSiteID = _siteService.CurrentSite.SiteID,
                    SubscriberType = "om.contact",
                    SubscriberRelatedID = contact.ContactID,
                    SubscriberFullName = $"Contact '{contact.ContactFirstName} {contact.ContactLastName}'",
                };

                SubscriberInfo.Provider.Set(subscriberInfo);
            }

            existingSubscriber = GetExistingSubscriber(contact);

            var existingAssignmentCount = SubscriberNewsletterInfo.Provider.Get()
                .WhereEquals(nameof(SubscriberNewsletterInfo.NewsletterID), newsletterId)
                .WhereEquals(nameof(SubscriberNewsletterInfo.SubscriberID), existingSubscriber!.SubscriberID)
                .Count;

            if (existingAssignmentCount == 0)
            {
                var monthBefore = DateTime.UtcNow.AddDays(-30);

                var subscriberNewsletterInfo = new SubscriberNewsletterInfo
                {
                    NewsletterID = newsletterId,
                    SubscriberID = existingSubscriber.SubscriberID,
                    SubscribedWhen = monthBefore,
                    SubscriptionApprovedWhen = monthBefore,
                    SubscriptionApproved = true
                };

                SubscriberNewsletterInfo.Provider.Set(subscriberNewsletterInfo);
            }
        }

        private void EnsureEmailSending(IssueInfo issue)
        {
            if (issue.IssueStatus != IssueStatusEnum.Finished)
            {
                NewsletterTasksManager.DeleteMailoutTask(issue.IssueGUID, issue.IssueSiteID);
                _issueScheduler.ScheduleIssue(issue, DateTime.Now);
                var scheduledTask = TaskInfo.Provider.Get().WithCodeName(mailingTaskName).TopN(1).FirstOrDefault();

                if (scheduledTask != null)
                {
                    SchedulingExecutor.ExecuteTask(scheduledTask);
                }

                issue.IssueMailoutTime = DateTime.UtcNow.AddDays(-30);
                IssueInfo.Provider.Set(issue);
            }
        }

        private static void LogEmailOpening(string email, IssueInfo issue, DateTime date)
        {
            var openedEmailInfo = new OpenedEmailInfo
            {
                OpenedEmailEmail = email,
                OpenedEmailTime = date,
                OpenedEmailIssueID = issue.IssueID,
            };

            OpenedEmailInfo.Provider.Set(openedEmailInfo);
        }

        private static void LogLinkClick(string email, DateTime date, IssueInfo issue, Random randomizer)
        {
            var links = LinkInfo.Provider.Get()
                .WhereEquals(nameof(LinkInfo.LinkIssueID), issue.IssueID)
                .ToList();

            if (links.Count > 0)
            {
                var randomLink = links[randomizer.Next(0, links.Count - 1)];

                var clickedLinkInfo = new ClickedLinkInfo
                {
                    ClickedLinkEmail = email,
                    ClickedLinkTime = date,
                    ClickedLinkNewsletterLinkID = randomLink.LinkID
                };

                ClickedLinkInfo.Provider.Set(clickedLinkInfo);
            }
        }
    }
}
