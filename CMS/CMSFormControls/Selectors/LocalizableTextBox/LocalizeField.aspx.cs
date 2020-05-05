using System;
using System.Collections;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.UIControls;

[Title("localizable.localizefield")]
public partial class CMSFormControls_Selectors_LocalizableTextBox_LocalizeField : CMSModalPage
{
    #region "Variables"

    private string hdnValue;
    private string textbox;
    private string btnLocalizeField;
    private string btnLocalizeString;
    private string btnRemoveLocalization;
    private string plainText;
    private string resourceKeyPrefix;
    private string localizedInputContainer;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckPermissions("CMS.Localization", "LocalizeStrings");

        if (!QueryHelper.ValidateHash("hash"))
        {
            RedirectToInformation(GetString("general.badhashtitle"));
        }

        Save += btnOk_Click;

        string identifier = QueryHelper.GetString("params", String.Empty);
        Hashtable parameters = (Hashtable)WindowHelper.GetItem(identifier);

        if (parameters != null)
        {
            hdnValue = ValidationHelper.GetString(parameters["HiddenValue"], String.Empty);
            textbox = ValidationHelper.GetString(parameters["TextBoxID"], String.Empty);
            plainText = ValidationHelper.GetString(parameters["TextBoxValue"], String.Empty);
            btnLocalizeField = ValidationHelper.GetString(parameters["ButtonLocalizeField"], String.Empty);
            btnLocalizeString = ValidationHelper.GetString(parameters["ButtonLocalizeString"], String.Empty);
            btnRemoveLocalization = ValidationHelper.GetString(parameters["ButtonRemoveLocalization"], String.Empty);
            resourceKeyPrefix = ValidationHelper.GetString(parameters["ResourceKeyPrefix"], String.Empty);
            localizedInputContainer = ValidationHelper.GetString(parameters["LocalizedContainer"], String.Empty);
        }

        // Disable option to use existing resource string for user who is not admin
        lstExistingOrNew.Items[1].Enabled = CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);

        if (lstExistingOrNew.SelectedValue == "new")
        {
            pnlExistingKey.Visible = false;
            pnlNewKey.Visible = true;

            if (!SystemContext.DevelopmentMode)
            {
                lblPrefix.Text = resourceKeyPrefix;
                lblPrefix.RemoveCssClass("hidden");
            }

            if (!RequestHelper.IsPostBack())
            {
                var newResKey = LocalizationHelper.GetUniqueResStringKey(plainText, resourceKeyPrefix);
                if (!SystemContext.DevelopmentMode && newResKey.StartsWith(resourceKeyPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    newResKey = newResKey.Substring(resourceKeyPrefix.Length);
                }
                txtNewResource.Text = newResKey;
            }
        }
        else
        {
            pnlExistingKey.Visible = true;
            pnlNewKey.Visible = false;
        }
    }


    /// <summary>
    /// Button OK clicked.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Check if current user's culture exists
        string cultureCode = CultureHelper.PreferredUICultureCode;
        if (string.IsNullOrEmpty(cultureCode))
        {
            cultureCode = CultureHelper.DefaultUICultureCode;
        }

        ResourceStringInfo ri;
        string key;

        // Creating new resource string
        if (lstExistingOrNew.SelectedValue == "new")
        {
            key = SystemContext.DevelopmentMode ? txtNewResource.Text.Trim() : resourceKeyPrefix + txtNewResource.Text.Trim();
            ri = ResourceStringInfoProvider.GetResourceStringInfo(key);

            // Resource string doesn't exists yet
            if (ri == null)
            {
                if (!ValidationHelper.IsCodeName(key))
                {
                    ShowError(GetString("culture.invalidresstringkey"));
                }
                else
                {
                    // Save ResourceString
                    ri = new ResourceStringInfo();
                    ri.StringKey = key;
                    ri.CultureCode = cultureCode;
                    ri.TranslationText = plainText;
                    ri.StringIsCustom = !SystemContext.DevelopmentMode;
                    ResourceStringInfoProvider.SetResourceStringInfo(ri);
                }
            }

            string script = ScriptHelper.GetScript("CloseDialog(); wopener.SetResourceAndOpen('" + hdnValue + "', '" + ScriptHelper.GetString(key, false) + "', '" + textbox + "', " + ScriptHelper.GetString(plainText) + ", '" + btnLocalizeField + "', '" + btnLocalizeString + "', '" + btnRemoveLocalization + "', '" + localizedInputContainer + "');");
            ScriptHelper.RegisterStartupScript(this, typeof(string), "localizeField", script);
        }
        // Using existing resource string
        else
        {
            key = ValidationHelper.GetString(resourceSelector.Value, String.Empty);
            ri = ResourceStringInfoProvider.GetResourceStringInfo(key);

            // Key not found in DB
            if (ri == null)
            {
                // Try to find it in .resx file
                FileResourceManager resourceManager = LocalizationHelper.GetFileManager(cultureCode);
                if (resourceManager != null)
                {
                    string translation = resourceManager.GetString(key);
                    if (!string.IsNullOrEmpty(translation))
                    {
                        if (!SystemContext.DevelopmentMode)
                        {
                            // Save the key in DB
                            ri = new ResourceStringInfo
                            {
                                StringKey = key,
                                StringIsCustom = true,
                                CultureCode = cultureCode,
                                TranslationText = translation
                            };
                            ResourceStringInfoProvider.SetResourceStringInfo(ri);
                        }

                        string script = ScriptHelper.GetScript("CloseDialog(); wopener.SetResource('" + hdnValue + "', '" + key + "', '" + textbox + "', " + ScriptHelper.GetString(translation) + ", '" + btnLocalizeField + "', '" + btnLocalizeString + "', '" + btnRemoveLocalization + "', '" + localizedInputContainer + "');");
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "localizeField", script);
                    }
                    else
                    {
                        ShowError(GetString("localize.doesntexist"));
                    }
                }
                else
                {
                    ShowError(GetString("localize.doesntexist"));
                }
            }
            // Send to parent window selected resource key
            else
            {
                using (LocalizationActionContext context = new LocalizationActionContext())
                {
                    context.ResolveSubstitutionMacros = false;
                    var localizedText = ScriptHelper.GetString(GetString(key), true, true);
                    string script = ScriptHelper.GetScript($"wopener.SetResource('{hdnValue}', '{key}', '{textbox}', {localizedText}, '{btnLocalizeField}', '{btnLocalizeString}', '{btnRemoveLocalization}', '{localizedInputContainer}'); CloseDialog();");
                    ScriptHelper.RegisterStartupScript(this, typeof(string), "localizeField", script);
                }
                
            }
        }
    }

    #endregion
}
