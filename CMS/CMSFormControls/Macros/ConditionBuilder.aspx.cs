using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.UIControls;


public partial class CMSFormControls_Macros_ConditionBuilderDialog : DesignerPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "macro_rules_conditions";

    #endregion


    private string controlHash;
    private string clientId;


    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterESCScript = false;

        controlHash = QueryHelper.GetString("controlHash", "");
        clientId = QueryHelper.GetString("clientid", "");

        SetTitle(GetString("conditionbuilder.title"));
        PageTitle.HelpTopicName = HELP_TOPIC_LINK;

        Save += btnSave_Click;

        designerElem.RuleCategoryNames = QueryHelper.GetString("module", "");
        designerElem.DisplayRuleType = QueryHelper.GetInteger("ruletype", 0);
        designerElem.ShowGlobalRules = QueryHelper.GetBoolean("showglobal", true);
        designerElem.MacroRuleAvailability = (MacroRuleAvailabilityEnum)QueryHelper.GetInteger("macroruleavailability", 0);

        // Set correct resolver to the control
        string resolverName = ValidationHelper.GetString(SessionHelper.GetValue("ConditionBuilderResolver_" + controlHash), "");
        if (!string.IsNullOrEmpty(resolverName))
        {
            designerElem.ResolverName = resolverName;
        }

        // Set correct default condition text
        string defaultText = ValidationHelper.GetString(SessionHelper.GetValue("ConditionBuilderDefaultText_" + controlHash), "");
        if (!string.IsNullOrEmpty(defaultText))
        {
            designerElem.DefaultConditionText = defaultText;
        }

        if (!RequestHelper.IsPostBack())
        {
            string condition = MacroProcessor.RemoveDataMacroBrackets(ValidationHelper.GetString(SessionHelper.GetValue("ConditionBuilderCondition_" + controlHash), ""));
            designerElem.Value = condition;
        }

        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        // Clean-up the session
        SessionHelper.Remove("ConditionBuilderCondition_" + controlHash);
        SessionHelper.Remove("ConditionBuilderResolver_" + controlHash);
        SessionHelper.Remove("ConditionBuilderDefaultText_" + controlHash);

        try
        {
            string text = ValidationHelper.GetString(designerElem.Value, "");
            ltlScript.Text = ScriptHelper.GetScript("wopener.InsertMacroCondition" + ScriptHelper.GetString(clientId, false) + "(" + ScriptHelper.GetString(text) + "); CloseDialog();");
        }
        catch { }
    }
}
