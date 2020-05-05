using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;

public partial class CMSFormControls_Classes_SelectQuery : FormEngineUserControl
{
    private bool? mHasUserAdministrationRights;


    /// <summary>
    /// Returns ClientID of the textbox with query.
    /// </summary>
    public override string ValueElementID => uniSelector.TextBoxSelect.ClientID;


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            SetSelectorEditability();
            uniSelector.Value = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            InitializeSelector();
        }
    }


    private void InitializeSelector()
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.AllowEmpty = false;
        uniSelector.SetValue("FilterMode", QueryInfo.OBJECT_TYPE);
        uniSelector.AdditionalSearchColumns = "QueryFullName";

        var editAuthorized = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Design", "EditSQLCode");
        if (editAuthorized)
        {
            const string BASE_URL = "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Query_Edit.aspx?editonlycode=true";

            uniSelector.NewItemPageUrl = URLHelper.AddParameterToUrl(BASE_URL, "selectedvalue", "##ITEMID##");
            uniSelector.EditItemPageUrl = URLHelper.AddParameterToUrl(BASE_URL, "name", "##ITEMID##");
            uniSelector.EditDialogWindowHeight = 780;
        }
    }


    /// <summary>
    /// Sets the selector control editability.
    /// </summary>
    /// <remarks>
    /// Only global administrator is allowed to edit the query value directly, everyone else can do it using selection dialog.
    /// <see cref="UniSelector.AllowEditTextBox"/> value has to be set before the <see cref="UniSelector.Value"/> to get value displayed and hash validated correctly.
    /// </remarks>
    private void SetSelectorEditability()
    {
        if (!mHasUserAdministrationRights.HasValue)
        {
            mHasUserAdministrationRights = MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
            uniSelector.AllowEditTextBox = mHasUserAdministrationRights.Value;
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        var value = Value?.ToString();

        // If macro or special value, do not validate
        if (MacroProcessor.ContainsMacro(value) || String.IsNullOrEmpty(value))
        {
            return true;
        }

        try
        {
            QueryInfoProvider.GetQueryInfo(value, true);
        }
        catch
        {
            ValidationError = GetString("query.queryorclassnotexist").Replace("%%code%%", value);
            return false;
        }

        return true;
    }
}