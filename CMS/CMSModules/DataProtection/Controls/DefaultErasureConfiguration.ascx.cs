using System;
using System.Collections.Generic;

using CMS.Base.Web.UI;
using CMS.UIControls;

/// <summary>
/// Default configuration control for erasure process configuration input.
/// </summary>
public partial class CMSModules_DataProtection_Controls_DefaultErasureConfiguration : ErasureConfigurationControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);
    }


    /// <summary>
    /// Fills the <paramref name="configuration"/> with user provided configuration options.
    /// </summary>
    public override IDictionary<string, object> GetConfiguration(IDictionary<string, object> configuration)
    {
        configuration.Add("deleteContacts", chbDeleteContacts.Checked);
        configuration.Add("deleteContactFromAccounts", chbDeleteContactFromAccounts.Checked);
        configuration.Add("deleteSubscriptionFromNewsletters", chbDeleteSubscriptionFromNewsletters.Checked);
        configuration.Add("deleteActivities", chbDeleteActivities.Checked);
        configuration.Add("deleteSubmittedFormsActivities", chbDeleteSubmittedFormsActivities.Checked);
        configuration.Add("deleteSubmittedFormsData", chbDeleteSubmittedFormsData.Checked);

        configuration.Add("deleteCustomers", chbDeleteCustomers.Checked);
        configuration.Add("deleteShoppingCarts", chbDeleteShoppingCarts.Checked);
        configuration.Add("deleteWishlists", chbDeleteWishlists.Checked);

        if (chbDeleteOrdersOlderThanYears.Checked)
        {
            configuration.Add("deleteOrdersOlderThanYears", txtNumberOfYears.Text.Trim());
        }

        return configuration;
    }


    /// <summary>
    /// Returns <c>true</c> if user provided configuration is valid. Otherwise, returns <c>false</c> and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        var isValid = true;

        if (!IsAnyCheckboxChecked())
        {
            isValid = false;
            AddError(GetString("dataprotection.app.deletedata.nodataselected"));
        }

        if (chbDeleteOrdersOlderThanYears.Checked)
        {
            int years;
            if (!Int32.TryParse(txtNumberOfYears.Text, out years) || years < 0)
            {
                isValid = false;
                AddError(GetString("dataprotection.app.deletedata.inputisnotanumber"));
            }
        }

        return isValid;
    }


    private bool IsAnyCheckboxChecked()
    {
        return
            chbDeleteContacts.Checked ||
            chbDeleteContactFromAccounts.Checked ||
            chbDeleteSubscriptionFromNewsletters.Checked ||
            chbDeleteActivities.Checked ||
            chbDeleteSubmittedFormsActivities.Checked ||
            chbDeleteSubmittedFormsData.Checked ||
            chbDeleteCustomers.Checked ||
            chbDeleteShoppingCarts.Checked ||
            chbDeleteWishlists.Checked ||
            chbDeleteOrdersOlderThanYears.Checked;
    }
}