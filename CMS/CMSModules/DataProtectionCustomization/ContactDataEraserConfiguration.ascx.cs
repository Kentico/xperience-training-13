using CMS.UIControls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CMSApp.CMSModules.DataProtectionCustomization
{
    public partial class ContactDataEraserConfiguration : ErasureConfigurationControl
    {
        public override IDictionary<string, object> GetConfiguration(IDictionary<string, object> configuration)
        {
            // The configuration keys must match the values checked in your IPersonalDataEraser implementations
            configuration.Add("deleteContacts", chbDeleteContacts.Checked);
            configuration.Add("deleteActivities", chbDeleteActivities.Checked);

            return configuration;
        }

        public override bool IsValid()
        {
            // Validates that at least one deletion checkbox is selected
            bool isValid = chbDeleteContacts.Checked || chbDeleteActivities.Checked;

            if (!isValid)
            {
                // Adds an error message that the control displays when the validation fails
                AddError("No data was selected for deletion.");
            }

            return isValid;
        }
    }
}