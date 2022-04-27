using System;
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

namespace XperienceAdapter.Generator
{
    public class Generator : IGenerator
    {
        private const string TestingCenterStartingDateDbName = "TestingCenterStartingDate";

        private const string NoPathMessage = "The path must be specified.";

        private const string NoCodenameMessage = "The codename must be specified";
        
        private static CsvParserOptions _csvParserOptions = new CsvParserOptions(true, ',');

        private readonly IActivityLogService _activityLogService;

        public Generator(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
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
            var countryIds = CountryInfo.Provider.Get().OrderBy("CountryID").Select(c => c.CountryID).ToList();
            var random = new Random();
            var max = countryIds?.Count();

            if (contacts?.Any() == true && countryIds?.Any() == true)
            {
                foreach (var contact in contacts)
                {
                    if (contact?.IsValid == true)
                    {
                        var result = contact.Result;
                        var countryIndex = random?.Next(0, max ?? 0) ?? 0;

                        var contactInfo = new ContactInfo()
                        {
                            ContactFirstName = result.FirstName,
                            ContactLastName = result.LastName,
                            ContactEmail = result.EmailAddress,
                            ContactCountryID = countryIds[countryIndex],
                            ContactMonitored = true
                        };

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

        public void GenerateConversions()
        {
            throw new NotImplementedException();
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
    }
}
