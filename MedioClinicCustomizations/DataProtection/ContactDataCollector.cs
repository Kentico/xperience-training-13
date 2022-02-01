using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;

using MedioClinicCustomizations.DataProtection.Writers;

namespace MedioClinicCustomizations.DataProtection
{
    public class ContactDataCollector : IPersonalDataCollector
    {
        // Prepares a list of contact columns to be included in the personal data
        // Every Tuple contains a column name, and user-friendly description of its content
        private readonly List<Tuple<string, string>> contactColumns = new List<Tuple<string, string>> {
        Tuple.Create("ContactFirstName", "First name"),
        Tuple.Create("ContactMiddleName", "Middle name"),
        Tuple.Create("ContactLastName", "Last name"),
        Tuple.Create("ContactJobTitle", "Job title"),
        Tuple.Create("ContactAddress1", "Address"),
        Tuple.Create("ContactCity", "City"),
        Tuple.Create("ContactZIP", "ZIP"),
        Tuple.Create("ContactMobilePhone", "Mobile phone"),
        Tuple.Create("ContactBusinessPhone", "Business phone"),
        Tuple.Create("ContactEmail", "Email"),
        Tuple.Create("ContactBirthday", "Birthday"),
        Tuple.Create("ContactGender", "Gender"),
        Tuple.Create("ContactNotes", "Notes"),
        Tuple.Create("ContactGUID", "GUID"),
        Tuple.Create("ContactLastModified", "Last modified"),
        Tuple.Create("ContactCreated", "Created"),
        Tuple.Create("ContactCampaign", "Campaign"),
        Tuple.Create("ContactCompanyName", "Company name"),
        Tuple.Create("TestingCenterStartingDate", "Testing center starting date")
    };

        public PersonalDataCollectorResult Collect(IEnumerable<BaseInfo> identities, string outputFormat)
        {
            // Gets a list of all contact objects added by registered IIdentityCollector implementations
            List<ContactInfo> contacts = identities.OfType<ContactInfo>().ToList();

            // Uses a writer class to create the personal data, in either XML format or as human-readable text
            string contactData = null;
            if (contacts.Any())
            {
                switch (outputFormat.ToLowerInvariant())
                {
                    case PersonalDataFormat.MACHINE_READABLE:
                        contactData = GetXmlContactData(contacts);
                        break;
                    case PersonalDataFormat.HUMAN_READABLE:
                    default:
                        contactData = GetTextContactData(contacts);
                        break;
                }
            }

            return new PersonalDataCollectorResult
            {
                Text = contactData
            };
        }

        private string GetXmlContactData(List<ContactInfo> contacts)
        {
            using (var writer = new XmlPersonalDataWriter())
            {
                // Wraps the contact data into a <OnlineMarketingData> tag
                writer.WriteStartSection("OnlineMarketingData");

                foreach (ContactInfo contact in contacts)
                {
                    // Writes a tag representing a contact object
                    writer.WriteStartSection(ContactInfo.OBJECT_TYPE);
                    // Writes tags for the contact's personal data columns and their values
                    writer.WriteObject(contact, contactColumns.Select(t => t.Item1).ToList());
                    // Closes the contact object tag
                    writer.WriteEndSection();
                }

                // Closes the <OnlineMarketingData> tag
                writer.WriteEndSection();

                return writer.GetResult();
            }
        }

        private string GetTextContactData(List<ContactInfo> contacts)
        {
            var writer = new TextPersonalDataWriter();

            writer.WriteStartSection("On-line marketing data");

            foreach (ContactInfo contact in contacts)
            {
                writer.WriteStartSection("Contact");
                // Writes user-friendly descriptions of the contact's personal data columns and their values
                writer.WriteObject(contact, contactColumns);
                writer.WriteEndSection();
            }

            return writer.GetResult();
        }
    }
}