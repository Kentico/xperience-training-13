using System;
using System.Linq;

using CMS.ContactManagement;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

/// <summary>
/// Control used in the settings of deleting inactive contacts. 
/// Loads all the options registered via <see cref="RegisterDeleteContactsImplementationAttribute"/>
/// </summary>
public partial class CMSModules_OnlineMarketing_Controls_Settings_SelectContactDeletionImplementation : FormEngineUserControl
{
    /// <summary>
    /// Value of the control. Reflects the underlying RadioButtonListControl.
    /// </summary>
    public override object Value
    {
        get
        {
            return rbList.Value;
        }

        set
        {
            rbList.Value = value;
        }
    }


    /// <summary>
    /// Enables or disables the control. Is added because this control is used in the settings and is automatically disabled
    /// when 'Inherit from global settings' checkbox is on.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return rbList.Enabled;
        }
        set
        {
            rbList.Enabled = value;
        }
    }


    /// <summary>
    /// Loads the ListItems to the underlying RadioButtonListControl.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        // First value is empty
        string options = ";{$settingskey.om.deleteinactivecontacts.method.donotdelete$}\r\n";

        // Load all the other settings for contact deletion from container
        // Items are registered through RegisterDeleteContactsImplementationAttribute
        foreach (var settingsItem in DeleteContactsSettingsContainer.SettingsItems.OrderBy(x => x.Name))
        {
            options += settingsItem.Name + ";" + GetString(settingsItem.DisplayNameResourceString) + "\r\n";
        }
        rbList.SetValue("options", options);

        SetExplanationText();

        base.OnLoad(e);
    }


    private void SetExplanationText()
    {
        litExplanationText.Text = String.Format(ResHelper.GetString("deleteinactivecontacts.settings.description"), DocumentationHelper.GetDocumentationTopicUrl("contacts_automatic_deletion"));
    }
}