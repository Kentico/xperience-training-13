using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;


[ValidationProperty("Value")]
public partial class CMSFormControls_System_LocalizableTextBox : LocalizableFormEngineUserControl, ICallbackEventHandler
{
    #region "Constants"

    /// <summary>
    /// Localization macro starts with '{$' characters.
    /// </summary>
    public const string MACRO_START = "{$";

    /// <summary>
    /// Localization macro ends with '$}' characters.
    /// </summary>
    public const string MACRO_END = "$}";

    /// <summary>
    /// In-place localization macro starts with '{$=' characters and should not be localized in localizable textbox!
    /// </summary>
    protected const string INPLACE_MACRO_START = "{$=";

    /// <summary>
    /// URL of field localization modal dialog.
    /// </summary>
    public const string LOCALIZE_FIELD = "~/CMSFormControls/Selectors/LocalizableTextBox/LocalizeField.aspx";

    /// <summary>
    /// URL of string localization modal dialog.
    /// </summary>
    public const string LOCALIZE_STRING = "~/CMSFormControls/Selectors/LocalizableTextBox/LocalizeString.aspx";

    /// <summary>
    /// Default prefix for keys created in development mode.
    /// </summary>
    private const string DEV_PREFIX = "test.";

    private const string ADD_ARGUMENT = "add|";
    private const string TEXT_BOX_LOCALIZED_CSS = "input-localized";

    #endregion


    #region "Private variables"

    private string mResourceKeyPrefix;
    private bool mSaveAutomatically;
    private readonly bool mUserHasPermissionForLocalizations = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Localization", "LocalizeStrings");

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets value indicating if localization of key exists.
    /// </summary>
    private bool LocalizationExists
    {
        get
        {
            return ResHelper.TranslationFoundForLocalizedString(OriginalValue);
        }
    }


    /// <summary>
    /// Indicates if text contained in textbox is resolved resource string or if it is just plain text.
    /// </summary>
    private bool IsLocalizationMacro
    {
        get
        {
            return MacroProcessor.IsLocalizationMacro(OriginalValue);
        }
    }


    /// <summary>
    /// Contains unresolved resource string.
    /// </summary>
    private string OriginalValue
    {
        get
        {
            if (!string.IsNullOrEmpty(hdnValue.Value))
            {
                return hdnValue.Value;
            }
            return ValidationHelper.GetString(ViewState["OriginalValue"], string.Empty);
        }
        set
        {
            hdnValue.Value = value;
            ViewState["OriginalValue"] = value;
        }
    }


    /// <summary>
    /// Modal dialog identifier.
    /// </summary>
    private Guid Identifier
    {
        get
        {
            // Try to load data from control ViewState
            Guid identifier = ValidationHelper.GetGuid(Request.Params[hdnIdentifier.UniqueID], Guid.Empty);

            // Create new GUID for identifier
            if (identifier == Guid.Empty)
            {
                identifier = Guid.NewGuid();
            }

            // Assign identifier to hidden field
            hdnIdentifier.Value = identifier.ToString();

            return identifier;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets whether the control is read only
    /// </summary>
    public bool ReadOnly
    {
        get
        {
            return TextBox.ReadOnly;
        }
        set
        {
            TextBox.ReadOnly = value;
        }
    }


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
            if (TextBox != null)
            {
                TextBox.Enabled = value;
                btnLocalize.Enabled = value;
                btnOtherLanguages.Enabled = value;
                btnRemoveLocalization.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (MacroProcessor.IsLocalizationMacro(TextBox.Text) || !IsLocalizationMacro || !LocalizationExists)
            {
                // Return plain text contained in textbox - new localization macro is inserted or field is not localized
                return TextBox.Text;
            }
            else
            {
                // Return macro stored in hidden field - localization macro is already set and textbox contains translation
                return OriginalValue;
            }
        }
        set
        {
            if (mSaveAutomatically)
            {
                // Save if configured to save automatically
                Save();

                mSaveAutomatically = false;
            }

            string valueStr = OriginalValue = ValidationHelper.GetString(value, null);

            if (MacroProcessor.IsLocalizationMacro(valueStr) && !IsInplaceMacro(valueStr) && LocalizationExists)
            {
                using (LocalizationActionContext context = new LocalizationActionContext())
                {
                    context.ResolveSubstitutionMacros = false;
                    TextBox.Text = ResHelper.LocalizeString(valueStr);
                }
            }
            else
            {
                TextBox.Text = valueStr;
            }
        }
    }


    /// <summary>
    /// Returns client ID of the textbox.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return TextBox.ClientID;
        }
    }


    /// <summary>
    /// Publicly visible textbox which contains translated string or plain text.
    /// </summary>
    public CMSTextBox TextBox
    {
        get
        {
            cntrlContainer.LoadContainer();
            return textbox;
        }
    }


    /// <summary>
    /// TextMode of textbox.
    /// </summary>
    public TextBoxMode TextMode
    {
        get
        {
            return TextBox.TextMode;
        }
        set
        {
            TextBox.TextMode = value;
        }
    }


    /// <summary>
    /// Number of columns of textbox in multi-line mode.
    /// </summary>
    public int Columns
    {
        get
        {
            return TextBox.Columns;
        }
        set
        {
            TextBox.Columns = value;
        }
    }


    /// <summary>
    /// Number of rows of textbox in multi-line mode.
    /// </summary>
    public int Rows
    {
        get
        {
            return TextBox.Rows;
        }
        set
        {
            TextBox.Rows = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if control should save changes to resource string immediately after each PostBack. Default true.
    /// </summary>
    public bool AutoSave
    {
        get;
        set;
    } = true;


    /// <summary>
    /// Maximum length of plain text or resource string key. Validates in IsValid() method.
    /// </summary>
    public int MaxLength
    {
        get
        {
            return TextBox.MaxLength;
        }
        set
        {
            TextBox.MaxLength = value;
        }
    }


    /// <summary>
    /// Resource key prefix.
    /// </summary>
    public string ResourceKeyPrefix
    {
        get
        {
            // If user set prefix
            if (!String.IsNullOrEmpty(mResourceKeyPrefix))
            {
                return mResourceKeyPrefix;
            }
            // If in DevelopmentMode
            else if (SystemContext.DevelopmentMode)
            {
                return DEV_PREFIX;
            }
            // Otherwise return "custom."
            else
            {
                return "custom.";
            }
        }
        set
        {
            mResourceKeyPrefix = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site. Default value is FALSE for localizable text box.
    /// </summary>
    public override bool IsLiveSite
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if textbox value is used in content.
    /// Default value is FALSE for localizable text box. 
    /// </summary>
    public bool ValueIsContent
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ValueIsContent"), false);
        }
        set
        {
            SetValue("ValueIsContent", value);
        }
    }


    /// <summary>
    /// Text which will be used as watermark.
    /// </summary>
    public string WatermarkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WatermarkText"), string.Empty);
        }
        set
        {
            SetValue("WatermarkText", value);
            TextBox.WatermarkText = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnOtherLanguages.ToolTip = GetString("localizable.otherlanguages");
        btnLocalize.ToolTip = GetString("localizable.localize");
        btnRemoveLocalization.ToolTip = GetString("localizable.remove");

        btnOtherLanguages.OnClientClick = "LocalizeString('" + hdnValue.ClientID + "', '" + TextBox.ClientID + "'); return false;";
        btnRemoveLocalization.OnClientClick = String.Format("RemoveLocalization('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'); return false;",
            hdnValue.ClientID, TextBox.ClientID, btnLocalize.ClientID, btnOtherLanguages.ClientID, btnRemoveLocalization.ClientID, cntrlContainer.ClientID);

        btnLocalize.OnClientClick = "LocalizationDialog" + ClientID + "('" + ADD_ARGUMENT + "' + Get('" + TextBox.ClientID + "').value); return false;";

        // Apply CSS style
        if (!String.IsNullOrEmpty(CssClass))
        {
            TextBox.CssClass = CssClass;
            CssClass = null;
        }

        // Register event handler for saving data in BasicForm
        if (Form != null)
        {
            Form.OnAfterSave += Form_OnAfterSave;
        }

        // Save changes after each PostBack if set so
        else if (RequestHelper.IsPostBack() && Visible && AutoSave && IsLocalizationMacro && !String.IsNullOrEmpty(TextBox.Text.Trim()))
        {
            mSaveAutomatically = true;
        }

        SetClientSideMaxLength();
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (mSaveAutomatically)
        {
            Save();
        }

        // Ensure the text in textbox
        if (IsLocalizationMacro && LocalizationExists && !IsInplaceMacro(TextBox.Text) && RequestHelper.IsPostBack())
        {
            using (LocalizationActionContext context = new LocalizationActionContext())
            {
                context.ResolveSubstitutionMacros = false;
                TextBox.Text = ResHelper.LocalizeString(OriginalValue);
            }
        }

        // Set watermark text
        TextBox.WatermarkText = WatermarkText;

        SetVisibility();

        Reload();

        // Register the scripts
        if (cntrlContainer.DisplayActions)
        {
            RegisterScripts();
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Custom methods"

    private bool IsInplaceMacro(string text)
    {
        if (text == null)
        {
            return false;
        }

        return text.Trim().StartsWith(INPLACE_MACRO_START, StringComparison.Ordinal);
    }


    private int GetNumberOfCurrentSiteCultures()
    {
        return CultureSiteInfoProvider.GetSiteCultures(SiteContext.CurrentSiteName).Tables[0].Rows.Count;
    }


    /// <summary>
    /// Sets client-side max length of the textbox control.
    /// </summary>
    private void SetClientSideMaxLength()
    {
        if (FieldInfo != null)
        {
            var maxLength = FieldInfo.GetMaxInputLength();
            if (maxLength > 0)
            {
                TextBox.MaxLength = maxLength;
            }
        }
    }


    /// <summary>
    /// Sets the dialog parameters to the context.
    /// </summary>
    protected void SetFieldDialogParameters(string textboxValue)
    {
        Hashtable parameters = new Hashtable();
        parameters["TextBoxValue"] = textboxValue;
        parameters["HiddenValue"] = hdnValue.ClientID;
        parameters["TextBoxID"] = TextBox.ClientID;
        parameters["ButtonLocalizeField"] = btnLocalize.ClientID;
        parameters["ButtonLocalizeString"] = btnOtherLanguages.ClientID;
        parameters["ButtonRemoveLocalization"] = btnRemoveLocalization.ClientID;
        parameters["ResourceKeyPrefix"] = ResourceKeyPrefix;
        parameters["LocalizedContainer"] = cntrlContainer.ClientID;

        WindowHelper.Add(Identifier.ToString(), parameters);
    }


    /// <summary>
    /// Sets visibility according to user permissions.
    /// </summary>
    private void SetVisibility()
    {
        bool hasMoreCultures = ValueIsContent ? CultureInfoProvider.NumberOfUICultures > 1 : GetNumberOfCurrentSiteCultures() > 1;

        btnLocalize.Visible = btnOtherLanguages.Visible = mUserHasPermissionForLocalizations && (SystemContext.DevelopmentMode || hasMoreCultures);

        cntrlContainer.DisplayActions =
            (SystemContext.DevelopmentMode || hasMoreCultures || LocalizationExists)
            && !IsLiveSite
            && !IsInplaceMacro(TextBox.Text)
            && (LocalizationExists || mUserHasPermissionForLocalizations);

        // User without permissions can't edit localization, only remove localized value
        // But ReadOnly can't be used because of possible change of text right after the removing localization
        if (!mUserHasPermissionForLocalizations && LocalizationExists)
        {
            TextBox.Attributes.Add("readOnly", "true");
        }
        else
        {
            TextBox.Attributes.Remove("readOnly");
        }
    }


    /// <summary>
    /// Validates control.
    /// </summary>
    public override bool IsValid()
    {
        // Check for maximum length
        if (TextBox.MaxLength > 0)
        {
            return (OriginalValue.Length <= MaxLength) && (TextBox.Text.Length <= MaxLength);
        }
        else
        {
            return true;
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void Reload()
    {
        // Textbox contains translated macro
        if (IsLocalizationMacro && LocalizationExists)
        {
            btnLocalize.Style.Add(HtmlTextWriterStyle.Display, "none");
            btnOtherLanguages.Style.Add(HtmlTextWriterStyle.Display, "inline");
            btnRemoveLocalization.Style.Add(HtmlTextWriterStyle.Display, "inline");

            // Add localized CSS class
            cntrlContainer.AddCssClass(TEXT_BOX_LOCALIZED_CSS);

            TextBox.ToolTip =
                mUserHasPermissionForLocalizations ?
                String.Format(GetString("localizable.localized"), GetResourceKeyFromString(OriginalValue)) :
                GetString("localizable.localizedwithoutpermissions");
        }
        // Textbox contains only plain text
        else
        {
            btnLocalize.Style.Add(HtmlTextWriterStyle.Display, "inline");
            btnOtherLanguages.Style.Add(HtmlTextWriterStyle.Display, "none");
            btnRemoveLocalization.Style.Add(HtmlTextWriterStyle.Display, "none");

            cntrlContainer.RemoveCssClass(TEXT_BOX_LOCALIZED_CSS);
            TextBox.ToolTip = null;
        }
    }


    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        // Register function to set translation string key from dialog window
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterJQuery(Page);
        StringBuilder script = new StringBuilder();
        script.Append(
            @"
function Get(id) {
    return document.getElementById(id);
}

function SetResourceAndOpen(hdnValId, resKey, textBoxId, textBoxValue, btnLocalizeFieldId, btnLocalizeStringId, btnRemoveLocalizationId, localizedContainerId) {
    SetResource(hdnValId, resKey, textBoxId, textBoxValue, btnLocalizeFieldId, btnLocalizeStringId, btnRemoveLocalizationId, localizedContainerId);
    LocalizeString(hdnValId, textBoxId);
    return false;
}

function SetResource(hdnValId, resKey, textBoxId, textBoxValue, btnLocalizeFieldId, btnLocalizeStringId, btnRemoveLocalizationId, localizedContainerId) {
    Get(hdnValId).value = '", MACRO_START, @"' + resKey + '", MACRO_END, @"';

    var $textBox = $cmsj('#' + textBoxId);
    $textBox.val(textBoxValue);
    $textBox.trigger('change');

    $cmsj('#' + localizedContainerId).addClass('", TEXT_BOX_LOCALIZED_CSS, @"').trigger('checkScrollbar');
    
    $cmsj('#' + btnLocalizeFieldId).hide();
    $cmsj('#' + btnLocalizeStringId).css('display', 'inline');
    $cmsj('#' + btnRemoveLocalizationId).css('display', 'inline');
    return false;
}

function SetTranslation(textBoxId, textBoxValue, hdnValId, resKey) {
    var $textBox = $cmsj('#' + textBoxId);
    $textBox.val(textBoxValue);
    $textBox.trigger('change');

    $cmsj('#' + hdnValId).val('", MACRO_START, @"' + resKey + '", MACRO_END, @"');
}

function LocalizeFieldReady(rvalue, context) {
    modalDialog(context, 'localizableField', 720, 250, null, null, true);
    return false;
}

function LocalizeString(hdnValId, textBoxId) {
    var stringKey = Get(hdnValId).value;
    stringKey = stringKey.substring(", MACRO_START.Length, @", stringKey.length - ", MACRO_END.Length, @");
    modalDialog('", ResolveUrl(LOCALIZE_STRING), @"?hiddenValueControl=' + hdnValId + '&stringKey=' + escape(stringKey) + '&parentTextbox=' + textBoxId, 'localizableString', 900, 635, null, null, true);
    return false;
}

function RemoveLocalization(hdnValId, textBoxId, btnLocalizeFieldId, btnLocalizeStringId, btnRemoveLocalizationId, localizedContainerId) {
    if (confirm(" + ScriptHelper.GetLocalizedString("localizable.removelocalization") + @")) {
        var $textBox = $cmsj('#' + textBoxId);
        $textBox.prop('readonly', false);
        var hdnVal = Get(hdnValId);
        hdnVal.value = $textBox.val();
        $textBox.trigger('change');

        $cmsj('#' + localizedContainerId).removeClass('", TEXT_BOX_LOCALIZED_CSS, @"');
        $cmsj('#' + btnLocalizeStringId).hide();
        $cmsj('#' + btnRemoveLocalizationId).hide();
        $cmsj('#' + btnLocalizeFieldId).css('display', 'inline');
    }
    return false;
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "LocalizationDialogFunction", ScriptHelper.GetScript(script.ToString()));

        // Register callback to send current plain text to modal window for localization
        string url = LOCALIZE_FIELD + "?params=" + Identifier;
        url += "&hash=" + QueryHelper.GetHash(url, false);

        script = new StringBuilder();
        script.Append(@"
function LocalizationDialog", ClientID, @"(value) {
    ", Page.ClientScript.GetCallbackEventReference(this, "value", "LocalizeFieldReady", "'" + ResolveUrl(url) + "'"), @"
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "LocalizationDialog" + ClientID, ScriptHelper.GetScript(script.ToString()));
    }


    /// <summary>
    /// Saves translation for given resource string.
    /// </summary>
    /// <returns>Returns TRUE if resource string was successfully modified</returns>
    public override bool Save()
    {
        // Save changes only when translation is edited
        if (IsLocalizationMacro && mUserHasPermissionForLocalizations && LocalizationExists && !IsInplaceMacro(OriginalValue))
        {
            string resKey = GetResourceKeyFromString(OriginalValue);

            // Prevent from creating translation containing macro
            if (!MacroProcessor.IsLocalizationMacro(TextBox.Text))
            {
                resKey = resKey.Trim();

                var translationText = TextBox.Text.Trim();
                if (string.IsNullOrEmpty(translationText))
                {
                    lblError.Visible = true;
                    lblError.ResourceString = "localize.entertext";
                    return false;
                }

                var cultureCode = CultureInfoProvider.GetCultureID(CultureHelper.PreferredUICultureCode) != 0
                    ? CultureHelper.PreferredUICultureCode
                    : CultureHelper.DefaultUICultureCode;

                if (IsTranslationChanged(resKey, cultureCode, translationText))
                {
                    // Update / insert key
                    var ri = ResourceStringInfoProvider.GetResourceStringInfo(resKey) ?? new ResourceStringInfo
                    {
                        StringKey = resKey,
                        StringIsCustom = !SystemContext.DevelopmentMode
                    };

                    ri.TranslationText = translationText;
                    ri.CultureCode = cultureCode;

                    ResourceStringInfoProvider.SetResourceStringInfo(ri);
                    return true;
                }

                return false;
            }
        }

        return false;
    }


    private static bool IsTranslationChanged(string resourceKey, string cultureCode, string translation)
    {
        var existingTranslation = ResHelper.GetString(resourceKey, cultureCode);
        return !string.Equals(existingTranslation, translation, StringComparison.Ordinal);
    }


    /// <summary>
    /// Gets the resource string name from the text
    /// </summary>
    /// <param name="text">Text</param>
    private static string GetResourceKeyFromString(string text)
    {
        return text.Substring(MACRO_START.Length, text.Length - (MACRO_END.Length + MACRO_START.Length));
    }


    public override bool SetValue(string propertyName, object value)
    {
        switch (propertyName.ToLowerInvariant())
        {
            case "autosave":
                AutoSave = ValidationHelper.GetBoolean(value, AutoSave);
                return true;

            case "textmode":
                string textMode = ValidationHelper.GetString(value, TextBoxMode.SingleLine.ToString());
                TextBoxMode textBoxMode;
                Enum.TryParse(textMode, true, out textBoxMode);
                TextMode = textBoxMode;
                return true;
        }

        return base.SetValue(propertyName, value);
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// BasicForm saved event handler.
    /// </summary>
    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        Save();
    }


    /// <summary>
    /// Gets callback result.
    /// </summary>
    string ICallbackEventHandler.GetCallbackResult()
    {
        return String.Empty;
    }


    /// <summary>
    /// Raise callback event.
    /// </summary>
    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        if (eventArgument.StartsWith(ADD_ARGUMENT, StringComparison.Ordinal))
        {
            SetFieldDialogParameters(eventArgument.Substring(ADD_ARGUMENT.Length));
        }
    }

    #endregion
}