using System.Collections.Generic;
using System.Linq;

using CMS.Activities;
using CMS.Base;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.Helpers;

namespace MedioClinic.Customizations.DataProtection
{
    public class ContactDataEraser : IPersonalDataEraser
    {
        public void Erase(IEnumerable<BaseInfo> identities, IDictionary<string, object> configuration)
        {
            // Gets all contact objects added by registered IIdentityCollector implementations
            var contacts = identities.OfType<ContactInfo>();

            // Does nothing if no contacts were collected
            if (!contacts.Any())
            {
                return;
            }

            // Gets a list of identifiers for the contacts
            List<int> contactIds = contacts.Select(c => c.ContactID).ToList();

            // The context ensures that objects are permanently deleted with all versions, without creating recycle bin records
            // Contact and activity objects do not support versioning,
            // but we recommend using this action context in general when deleting personal data
            using (new CMSActionContext() { CreateVersion = false })
            {
                // Deletes the activities of the given contacts (if enabled in the configuration)
                DeleteActivities(contactIds, configuration);

                // Deletes the given contacts (if enabled in the configuration)
                // Also automatically deletes activities of the given contacts (contacts are parent objects of activities)
                DeleteContacts(contacts, configuration);
            }
        }

        private void DeleteActivities(List<int> contactIds, IDictionary<string, object> configuration)
        {
            // Checks whether deletion of activities is enabled in the configuration options
            object deleteActivities;
            if (configuration.TryGetValue("deleteActivities", out deleteActivities)
                && ValidationHelper.GetBoolean(deleteActivities, false))
            {
                // Deletes the activities of the specified contacts
                // The system may contain a very large number of activity records, so the example uses the BulkDelete API
                // This is more efficient, but does not perform general actions, such as logging of synchronization tasks
                ActivityInfoProvider.ProviderObject.BulkDelete(
                    new WhereCondition().WhereIn("ActivityContactID", contactIds));
            }
        }

        private static void DeleteContacts(IEnumerable<ContactInfo> contacts, IDictionary<string, object> configuration)
        {
            // Checks whether deletion of contacts is enabled in the configuration options
            object deleteContacts;
            if (configuration.TryGetValue("deleteContacts", out deleteContacts)
                && ValidationHelper.GetBoolean(deleteContacts, false))
            {
                // Deletes the specified contacts
                foreach (ContactInfo contact in contacts)
                {
                    ContactInfo.Provider.Delete(contact);
                }
            }
        }
    }
}
