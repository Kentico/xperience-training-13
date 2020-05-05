using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;


public partial class CMSFormControls_Macros_ConditionBuilder : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets ors sets value if the displayed text is User-Friendly representation of a macro.
    /// </summary>
    private string EditorValue
    {
        get
        {
            if (string.IsNullOrEmpty(hdnValue.Value))
            {
                return txtMacro.Text;
            }
            return hdnValue.Value;
        }
    }


    /// <summary>
    /// Indicates whether builder has more lines.
    /// </summary>
    public bool SingleLineMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SingleLineMode"), true);
        }
        set
        {
            SetValue("SingleLineMode", value);
            txtMacro.SingleLineMode = value;
        }
    }


    /// <summary>
    /// Determines whether the global rules are shown among with the specific rules defined in the RuleCategoryNames property.
    /// </summary>
    public bool ShowGlobalRules
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowGlobalRules"), true);
        }
        set
        {
            SetValue("ShowGlobalRules", value);
        }
    }


    /// <summary>
    /// Gets or sets macro resolver name which should be used for macro editor (if no name is given, MacroResolver.Current is used).
    /// </summary>
    public override string ResolverName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ResolverName"), "");
        }
        set
        {
            SetValue("ResolverName", value);
        }
    }


    /// <summary>
    /// Gets or sets name(s) of the Macro rule category(ies) which should be displayed in Rule designer. Items should be separated by semicolon.
    /// </summary>
    public string RuleCategoryNames
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RuleCategoryNames"), "");
        }
        set
        {
            SetValue("RuleCategoryNames", value);
        }
    }


    /// <summary>
    /// Determines which rules to display. 0 means all rules, 1 means only rules which does not require context, 2 only rules which require context.
    /// </summary>
    public int DisplayRuleType
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DisplayRuleType"), 0);
        }
        set
        {
            SetValue("DisplayRuleType", value);
        }
    }


    /// <summary>
    /// Gets or sets he maximum width of conditional builder in pixels.
    /// 0 mean unlimited and default value is 600px.
    /// </summary>
    public int MaxWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxWidth"), 600);
        }
        set
        {
            SetValue("MaxWidth", value);
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return txtMacro.Editor.Enabled;
        }
        set
        {
            txtMacro.Editor.Enabled = value;
            btnEdit.Enabled = value;
            btnClear.Enabled = value;
        }
    }


    /// <summary>
    /// Determines if value will be signed and wrapped in macro brackets.
    /// </summary>
    public bool AddDataMacroBrackets
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddDataMacroBrackets"), true);
        }
        set
        {
            SetValue("AddDataMacroBrackets", value);
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed by default when there is no rule defined.
    /// </summary>
    public string DefaultConditionText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultConditionText"), "");
        }
        set
        {
            SetValue("DefaultConditionText", value);
        }
    }


    /// <summary>
    /// Indicates which macro rules will be shown in the listing along with the <see cref="MacroRuleAvailabilityEnum.Both"/>.
    /// </summary>
    /// <remarks>Value in each macro rule indicates in which application can the macro rule be evaluated (i.e. the implementation of underlying macros is available).</remarks>
    public MacroRuleAvailabilityEnum MacroRuleAvailability
    {
        get
        {
            return (MacroRuleAvailabilityEnum)GetValue("MacroRuleAvailability", (int)MacroRuleAvailabilityEnum.MainApplication);
        }
        set
        {
            SetValue("MacroRuleAvailability", (int)value);
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            string val = MacroProcessor.RemoveDataMacroBrackets(EditorValue.Trim());
            if (!string.IsNullOrEmpty(val))
            {
                if (AddDataMacroBrackets)
                {
                    if (!(MacroStaticSettings.AllowOnlySimpleMacros || MacroSecurityProcessor.IsSimpleMacro(val)))
                    {
                        val = MacroSecurityProcessor.AddMacroSecurityParams(val, MacroIdentityOption.FromUserInfo(MembershipContext.AuthenticatedUser));
                    }

                    val = "{%" + val + "%}";
                }
                return val;
            }
            return string.Empty;
        }
        set
        {
            string val = MacroProcessor.RemoveDataMacroBrackets(ValidationHelper.GetString(value, ""));
            val = MacroSecurityProcessor.RemoveMacroSecurityParams(val, out MacroIdentityOption identityOption);
            hdnValue.Value = MacroProcessor.RemoveDataMacroBrackets(val.Trim());
            RefreshText();
        }
    }


    /// <summary>
    /// If true, auto completion is shown above the editor, otherwise it is below (default position is below).
    /// </summary>
    public bool ShowAutoCompletionAbove
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAutoCompletionAbove"), false);
        }
        set
        {
            SetValue("ShowAutoCompletionAbove", value);
            txtMacro.ShowAutoCompletionAbove = value;
        }
    }


    /// <summary>
    /// Gets ClientID of the textbox with emailinput.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtMacro.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the left offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int LeftOffset
    {
        get
        {
            return txtMacro.LeftOffset;
        }
        set
        {
            txtMacro.LeftOffset = value;
        }
    }


    /// <summary>
    ///  Gets or sets the top offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int TopOffset
    {
        get
        {
            return txtMacro.TopOffset;
        }
        set
        {
            txtMacro.TopOffset = value;
        }
    }


    /// <summary>
    /// Client ID of primary input control.
    /// </summary>
    public override string InputClientID
    {
        get
        {
            if (pnlRule.Visible)
            {
                return pnlRule.ClientID;
            }
            else
            {
                return txtMacro.Editor.ClientID;
            }
        }
    }

    #endregion


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set control style and css class
        if (!string.IsNullOrEmpty(ControlStyle))
        {
            txtMacro.Editor.Attributes.Add("style", ControlStyle);
        }
        if (!string.IsNullOrEmpty(CssClass))
        {
            txtMacro.Editor.CssClass = CssClass;
        }

        txtMacro.ShowAutoCompletionAbove = ShowAutoCompletionAbove;
        txtMacro.Editor.UseSmallFonts = true;
        txtMacro.MixedMode = false;

        MacroResolver resolver = MacroResolverStorage.GetRegisteredResolver(ResolverName);
        if (resolver != null)
        {
            resolver.Settings.VirtualMode = true;
            txtMacro.Resolver = resolver;
        }

        var ed = txtMacro.Editor;
        ed.ShowToolbar = false;
        ed.ShowLineNumbers = false;
        ed.DynamicHeight = false;

        string script = @"
function InsertMacroCondition" + ClientID + @"(text) {
    var hidden = document.getElementById('" + hdnValue.ClientID + @"');
    hidden.value = text;
    " + ControlsHelper.GetPostBackEventReference(btnRefresh) + @";
}";
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "InsertMacroCondition" + ClientID, script, true);
        ScriptHelper.RegisterDialogScript(Page);

        btnEdit.Click += btnEdit_Click;
        btnRefresh.Click += btnRefresh_Click;

        if (txtMacro.ReadOnly)
        {
            txtMacro.Editor.Language = LanguageEnum.Text;
        }

        if (MaxWidth > 0)
        {
            pnlConditionBuilder.Attributes["style"] += " max-width: " + MaxWidth + "px;";
        }

        txtMacro.DataBind();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        btnEdit.ToolTip = GetString("macrodesigner.edit");
        if (!btnEdit.Enabled)
        {
            btnEdit.AddCssClass("ButtonDisabled");
        }

        btnClear.ToolTip = GetString("macrodesigner.clearcondition");
        if (btnClear.Enabled)
        {
            btnClear.Attributes.Add("onclick", "if (confirm(" + ScriptHelper.GetString(GetString("macrodesigner.clearconditionconfirm")) + ")) { InsertMacroCondition" + ClientID + "(''); } return false;");
        }
        else
        {
            btnClear.AddCssClass("ButtonDisabled");
        }

        if (!Enabled)
        {
            // Disable the textbox
            pnlRule.Attributes["disabled"] = "disabled";
        }
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        RefreshText();
    }


    private void RefreshText()
    {
        string hdnValueTrim = hdnValue.Value.Trim();
        if (hdnValueTrim.StartsWithCSafe("rule(", true))
        {
            // Empty rule designer condition is not considered as rule conditions
            if (hdnValueTrim.EqualsCSafe("Rule(\"\", \"<rules></rules>\")", true))
            {
                hdnValue.Value = "";
            }
            else
            {
                try
                {
                    string ruleText = MacroRuleTree.GetRuleText(hdnValueTrim, true, true, TimeZoneTransformation);

                    // Display rule text
                    ltlMacro.Text = ruleText;
                    txtMacro.Visible = false;
                    pnlRule.Visible = true;

                    pnlUpdate.Update();

                    return;
                }
                catch
                {
                    // If failed to parse the rule, extract the condition
                    MacroExpression xml = MacroExpression.ExtractParameter(hdnValueTrim, "rule", 0);
                    if (xml != null)
                    {
                        hdnValue.Value = MacroSecurityProcessor.RemoveMacroSecurityParams(ValidationHelper.GetString(xml.Value, ""), out MacroIdentityOption identityOption);
                    }
                }
            }
        }


        if (string.IsNullOrEmpty(hdnValue.Value) && !string.IsNullOrEmpty(DefaultConditionText))
        {
            ltlMacro.Text = DefaultConditionText;
            txtMacro.Text = "";
            txtMacro.Visible = false;
            pnlRule.Visible = true;
        }
        else
        {
            txtMacro.Text = hdnValue.Value;
            hdnValue.Value = null;
            txtMacro.Visible = true;
            pnlRule.Visible = false;
        }

        pnlUpdate.Update();
    }


    /// <summary>
    /// Displays correct timezone for the rule parameter values.
    /// </summary>
    /// <param name="o">Parameter values</param>
    private object TimeZoneTransformation(object o)
    {
        if (o is DateTime dt)
        {
            return TimeZoneHelper.GetCurrentTimeZoneDateTimeString(dt, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, out TimeZoneInfo usedTimeZone);
        }

        return o;
    }


    protected void btnEdit_Click(object sender, EventArgs e)
    {
        var controlHash = GetHashCode();
        SessionHelper.SetValue("ConditionBuilderCondition_" + controlHash, EditorValue);

        string dialogUrl = String.Format("{0}?clientid={1}&controlHash={2}&module={3}&ruletype={4}&showglobal={5}&macroruleavailability={6}",
            ApplicationUrlHelper.ResolveDialogUrl("~/CMSFormControls/Macros/ConditionBuilder.aspx"),
            ClientID,
            controlHash,
            RuleCategoryNames,
            DisplayRuleType,
            ShowGlobalRules ? "1" : "0",
            (int)MacroRuleAvailability);
        if (!string.IsNullOrEmpty(ResolverName))
        {
            SessionHelper.SetValue("ConditionBuilderResolver_" + controlHash, ResolverName);
        }
        if (!string.IsNullOrEmpty(DefaultConditionText))
        {
            SessionHelper.SetValue("ConditionBuilderDefaultText_" + controlHash, DefaultConditionText);
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ConditionBuilderDialog", "modalDialog('" + dialogUrl + "', 'editmacrocondition', '95%', 700);", true);
    }
}