using System;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.FormEngine.Web.UI;
using CMS.MacroEngine;


/// <summary>
/// Displays the description macro setting, and allows the user to edit it.
/// </summary>
public partial class CMSModules_ContactManagement_FormControls_SalesForce_Description : FormEngineUserControl
{

    #region "Public properties"

    /// <summary>
    /// Gets or sets the description macro.
    /// </summary>
    public override object Value
    {
        get
        {
            return DescriptionMacroEditor.Text;
        }
        set
        {
            DescriptionMacroEditor.Text = value as string;
        }
    }


    /// <summary>
    /// Gets or sets whether the control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            DescriptionMacroEditor.ReadOnly = !value;
            base.Enabled = value;
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ContactInfo contact = new ContactInfo();
        MacroResolver resolver = MacroContext.CurrentResolver.CreateChild();
        resolver.SetNamedSourceData("Contact", contact);
        DescriptionMacroEditor.Resolver = resolver;
        DescriptionMacroEditor.Editor.Language = LanguageEnum.Text;
    }

    #endregion

}