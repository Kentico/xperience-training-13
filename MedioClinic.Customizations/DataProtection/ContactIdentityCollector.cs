using System.Collections.Generic;
using System.Linq;

using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;

namespace MedioClinic.Customizations.DataProtection
{
    public class ContactIdentityCollector : IIdentityCollector
    {
        public void Collect(IDictionary<string, object> dataSubjectIdentifiersFilter, List<BaseInfo> identities)
        {
            // Does nothing if the identifier inputs do not contain the "email" key or if its value is empty
            if (!dataSubjectIdentifiersFilter.ContainsKey("email"))
            {
                return;
            }

            string email = dataSubjectIdentifiersFilter["email"] as string;
            
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            // Finds contacts with a matching email address
            List<ContactInfo> contacts = ContactInfo.Provider.Get()
                                                .WhereEquals(nameof(ContactInfo.ContactEmail), email)
                                                .ToList();

            // Adds the matching contact objects to the list of collected identities
            identities.AddRange(contacts);
        }
    }
}
