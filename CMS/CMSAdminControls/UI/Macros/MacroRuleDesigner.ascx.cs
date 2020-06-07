using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.DragAndDrop;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.SiteProvider;

public partial class CMSAdminControls_UI_Macros_MacroRuleDesigner : FormEngineUserControl
{
    #region "Variables"

    private DragAndDropExtender extDragDrop;
    private int counter;
    private static StringSafeDictionary<string> mRulesTooltips;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns table of rules tooltips (indexed by rule id).
    /// This needs to be preserved for correct view after postback.
    /// </summary>
    public static StringSafeDictionary<string> RulesTooltips
    {
        get
        {
            return mRulesTooltips ?? (mRulesTooltips = new StringSafeDictionary<string>());
        }
    }


    /// <summary>
    /// Returns the representation of the designer tree. It returns the root of the whole tree.
    /// </summary>
    public MacroRuleTree RuleTree
    {
        get
        {
            if (ViewState["RuleTree"] == null)
            {
                ViewState["RuleTree"] = new MacroRuleTree();
            }
            return (MacroRuleTree)ViewState["RuleTree"];
        }
        private set
        {
            ViewState["RuleTree"] = value;
        }
    }


    /// <summary>
    /// Gets or sets name(s) of the Macro rule category(ies) which should be displayed in Rule designer. Items should be separated by semicolon.
    /// </summary>
    public string RuleCategoryNames
    {
        get;
        set;
    }


    /// <summary>
    /// Determines which rules to display. 0 means all rules, 1 means only rules which does not require context, 2 only rules which require context.
    /// </summary>
    public int DisplayRuleType
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether the global rules are shown among with the specific rules defined in the RuleCategoryNames property.
    /// </summary>
    public bool ShowGlobalRules
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates which macro rules will be shown in the listing along with the <see cref="MacroRuleAvailabilityEnum.Both"/>.
    /// </summary>
    /// <remarks>Value in each macro rule indicates in which application can the macro rule be evaluated (i.e. the implementation of underlying macros is available).</remarks>
    public MacroRuleAvailabilityEnum MacroRuleAvailability
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the text which is displayed by default when there is no rule defined.
    /// </summary>
    public string DefaultConditionText
    {
        get;
        set;
    }


    /// <summary>
    /// Returns the resulting condition
    /// </summary>
    public override object Value
    {
        get
        {
            string error = RuleTree.ValidateParameters();
            if (!string.IsNullOrEmpty(error))
            {
                pnlMessagePlaceholder.ShowError(GetString("macros.macrorule.requiredparamsmissing"));
                throw new Exception(error);
            }

            string condition = GetCondition();
            return "Rule(\"" + MacroElement.EscapeSpecialChars(condition) + "\", \"" + MacroElement.EscapeSpecialChars(GetXML()) + "\")";
        }
        set
        {
            ParseFromExpression(ValidationHelper.GetString(value, ""));
        }
    }


    /// <summary>
    /// Returns whether the parameter should be shown.
    /// </summary>
    private bool ShowParameterEdit
    {
        get
        {
            return ValidationHelper.GetBoolean(hdnParamEditShown.Value, false);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Attach events
        btnAutoIndent.Click += btnAutoIndent_Click;
        btnDelete.Click += btnDelete_Click;
        btnIndent.Click += btnIndent_Click;
        btnUnindent.Click += btnUnindent_Click;
        btnChangeOperator.Click += btnChangeOperator_Click;
        btnChangeParameter.Click += btnChangeParameter_Click;
        btnMove.Click += btnMove_Click;
        btnCancel.Click += btnCancel_Click;
        btnSetParameter.Click += btnSetParameter_Click;
        btnViewCode.Click += btnViewCode_Click;
        btnAddClause.Click += btnAddClause_Click;
        btnClearAll.Click += btnClearAll_Click;
        txtFilter.TextChanged += btnFilter_Click;

        btnFilter.Text = GetString("general.filter");
        btnSetParameter.Text = GetString("macros.macrorule.setparameter");
        btnCodeOK.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.cancel");
        btnIndent.ScreenReaderDescription = btnIndent.ToolTip = GetString("macros.macrorule.indent");
        btnUnindent.ScreenReaderDescription = btnUnindent.ToolTip = GetString("macros.macrorule.unindent");
        btnAutoIndent.ScreenReaderDescription = btnAutoIndent.ToolTip = GetString("macros.macrorule.autoindent");
        btnDelete.ScreenReaderDescription = btnDelete.ToolTip = GetString("general.delete");
        btnClearAll.ScreenReaderDescription = btnClearAll.ToolTip = GetString("macro.macrorule.clearall");
        btnViewCode.ScreenReaderDescription = btnViewCode.ToolTip = GetString("macros.macrorule.viewcode");

        btnIndent.OnClientClick = "if (isNothingSelected()) { alert(" + ScriptHelper.GetString(GetString("macros.macrorule.nothingselected")) + "); return false; }";
        btnUnindent.OnClientClick = "if (isNothingSelected()) { alert(" + ScriptHelper.GetString(GetString("macros.macrorule.nothingselected")) + "); return false; }";
        btnDelete.OnClientClick = "if (isNothingSelected()) { alert(" + ScriptHelper.GetString(GetString("macros.macrorule.nothingselected")) + "); return false; } else { if (!confirm('" + GetString("macros.macrorule.deleteconfirmation") + "')) { return false; }}";
        btnAutoIndent.OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("macros.macrorule.deleteautoindent")) + ")) { return false; }";
        btnClearAll.OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("macros.macrorule.clearall.confirmation")) + ")) { return false; }";

        lstRules.Attributes.Add("ondblclick", ControlsHelper.GetPostBackEventReference(btnAddClause));

        pnlViewCode.Visible = false;

        // Basic form
        formElem.SubmitButton.Visible = false;
        formElem.SiteName = SiteContext.CurrentSiteName;

        titleElem.TitleText = GetString("macros.macrorule.changeparameter");
        btnAddaClause.ToolTip = GetString("macros.macrorule.addclause");
        btnAddaClause.Click += btnAddClause_Click;

        // Drop cue
        Panel pnlCue = new Panel();
        pnlCue.ID = "pnlCue";
        pnlCue.CssClass = "MacroRuleCue";
        pnlCondtion.Controls.Add(pnlCue);

        pnlCue.Controls.Add(new LiteralControl("&nbsp;"));
        pnlCue.Style.Add("display", "none");

        // Create drag and drop extender
        extDragDrop = new DragAndDropExtender();
        extDragDrop.ID = "extDragDrop";
        extDragDrop.TargetControlID = pnlCondtion.ID;
        extDragDrop.DragItemClass = "MacroRule";
        extDragDrop.DragItemHandleClass = "MacroRuleHandle";
        extDragDrop.DropCueID = pnlCue.ID;
        extDragDrop.OnClientDrop = "OnDropRule";
        pnlCondtion.Controls.Add(extDragDrop);

        // Load the rule set
        if (!RequestHelper.IsPostBack())
        {
            if (ShowGlobalRules || !string.IsNullOrEmpty(RuleCategoryNames))
            {
                var where = GetRulesWhereCondition();

                var ds = MacroRuleInfo.Provider.Get().Where(where).OrderBy("MacroRuleDisplayName")
                                              .Columns("MacroRuleID, MacroRuleDisplayName, MacroRuleDescription, MacroRuleRequiredData, MacroRuleAvailability").TypedResult;
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    AddRules(ds);
                }

                if (lstRules.Items.Count > 0)
                {
                    lstRules.SelectedIndex = 0;
                }
            }
        }

        AddTooltips();

        // Make sure that one user click somewhere else than to any rule, selection will disappear
        pnlCondtion.Attributes["onclick"] = "if (!doNotDeselect && !isCTRL) { $cmsj('.RuleSelected').removeClass('RuleSelected'); document.getElementById('" + hdnSelected.ClientID + "').value = ';'; }; doNotDeselect = false;";

        LoadFormDefinition(false);

        // Set the default button for parameter edit dialog so that ENTER key works to submit the parameter value
        pnlParameterPopup.DefaultButton = btnSetParameter.ID;

        // Ensure correct edit dialog show/hide (because of form controls which cause postback)
        btnSetParameter.OnClientClick = "HideParamEdit();";
        btnCancel.OnClientClick = "HideParamEdit();";
        if (ShowParameterEdit)
        {
            mdlDialog.Show();
        }

        if (!string.IsNullOrEmpty(hdnScroll.Value))
        {
            // Preserve scroll position
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "MacroRulesScroll", "setTimeout('setScrollPosition()', 100);", true);
        }
    }


    private void AddRules(InfoDataSet<MacroRuleInfo> ds)
    {
        var resolver = MacroResolverStorage.GetRegisteredResolver(ResolverName);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            bool add = true;
            if (resolver != null)
            {
                // Check the required data, all specified data have to be present in the resolver
                string requiredData = ValidationHelper.GetString(dr["MacroRuleRequiredData"], "");
                if (!string.IsNullOrEmpty(requiredData))
                {
                    var required = requiredData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var req in required)
                    {
                        if (!resolver.IsDataItemAvailable(req))
                        {
                            add = false;
                            break;
                        }
                    }
                }
            }

            if (add)
            {
                var ruleId = dr["MacroRuleID"].ToString();
                ListItem item = new ListItem(dr["MacroRuleDisplayName"].ToString(), ruleId);
                lstRules.Items.Add(item);

                // Save the tooltip
                RulesTooltips[ruleId] = ResHelper.LocalizeString(ValidationHelper.GetString(dr["MacroRuleDescription"], ""));
            }
        }
    }


    private void AddTooltips()
    {
        // Add tooltips to the rules in the list
        foreach (ListItem item in lstRules.Items)
        {
            if (RulesTooltips.ContainsKey(item.Value))
            {
                item.Attributes.Add("title", RulesTooltips[item.Value]);
            }
        }
    }


    private string GetRulesWhereCondition()
    {
        string where = (ShowGlobalRules ? "MacroRuleResourceName IS NULL OR MacroRuleResourceName = ''" : "");

        // Append rules module name condition
        if (!string.IsNullOrEmpty(RuleCategoryNames))
        {
            bool appendComma = false;
            StringBuilder sb = new StringBuilder();
            string[] names = RuleCategoryNames.Split(new[]
            {
                ';'
            }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string n in names)
            {
                string name = "'" + SqlHelper.GetSafeQueryString(n.Trim(), false) + "'";
                if (appendComma)
                {
                    sb.Append(",");
                }
                sb.Append(name);
                appendComma = true;
            }

            where = SqlHelper.AddWhereCondition(where, "MacroRuleResourceName IN (" + sb + ")", "OR");
        }

        // Append require context condition
        switch (DisplayRuleType)
        {
            case 1:
                where = SqlHelper.AddWhereCondition(where, "MacroRuleRequiresContext = 0", "AND");
                break;

            case 2:
                where = SqlHelper.AddWhereCondition(where, "MacroRuleRequiresContext = 1", "AND");
                break;
        }

        // Append macro rule availbility condition
        string macroRule = $"MacroRuleAvailability = {(int)MacroRuleAvailabilityEnum.Both}";
        if(MacroRuleAvailability != MacroRuleAvailabilityEnum.Both)
        {
            macroRule += $" OR MacroRuleAvailability = {(int)MacroRuleAvailability}";
        }
        where = SqlHelper.AddWhereCondition(where, macroRule);

        // Select only enabled rules
        where = SqlHelper.AddWhereCondition(where, "MacroRuleEnabled = 1");

        return where;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        RegisterScriptMethods();

        if (RuleTree.Children.Count > 0)
        {
            ltlText.Text = GetRuleHTML(RuleTree);
        }
        else
        {
            ltlText.Text = String.IsNullOrEmpty(DefaultConditionText) ? "<span class=\"MacroRuleInfo\">" + GetString("macros.macrorule.emptycondition") + "</span>" : DefaultConditionText;
        }
    }


    /// <summary>
    /// Registers needed JS methods for operating the designer.
    /// </summary>
    private void RegisterScriptMethods()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(@"
var doNotDeselect = false;
function SelectRule(path, currentElem) {

    doNotDeselect = true;

    if (currentElem == null) {
        return;
    }

    var hidden = document.getElementById('", hdnSelected.ClientID, @"');
    if (hidden != null) {
        if (!isCTRL) {
            // Deselect all rules when CTRL is not pressed
            $cmsj('.RuleSelected').removeClass('RuleSelected');
            hidden.value = '';
        }

        var orig = hidden.value;
        var newText = hidden.value.replace(';' + path + ';', ';');
        if (orig.length != newText.length) {
            // If the rule was present it means it was selected, so deselect the item
            currentElem.removeClass('RuleSelected');
        } else {
            // If the rule was not selected before, select it and add to the list of selected
            currentElem.addClass('RuleSelected');
            if (newText == '') {
                newText = ';' + path + ';';
            } else {
                newText += path + ';';
            }
        }
        hidden.value = newText;
    }
}

function setScrollPosition() {
    var hdnScroll = document.getElementById('", hdnScroll.ClientID, @"');
    var scrollDiv = document.getElementById('scrollDiv');
    if ((hdnScroll != null) && (scrollDiv != null)) {
        if (hdnScroll.value != '') {
            scrollDiv.scrollTop = hdnScroll.value;
        }
    }
}

function isNothingSelected() {
    // If nothing is selected, do not allow to use buttons such as delete, indent, unindent
    var newText = document.getElementById('", hdnSelected.ClientID, @"').value;
    return (newText == '') || (newText == ';') || (newText == ';;');
}

");

        sb.Append(
@"
var isCTRL = false;
$cmsj(document).keyup(function(event) {
    if (event.which == 17) {
        isCTRL = false;
    }  
}).keydown(function(event) {
    if (event.which == 17) {
       isCTRL = true;
    }  
});
");

        sb.Append(string.Format(
@"var targetPosition = new Array();
function OnDropRule(source, target) {{
    var item = target.get_droppedItem();
    var targetPos = target.get_position(); 

    var hidden = document.getElementById('{0}')
    if (hidden != null) {{
        hidden.value = item.id + ';' + targetPosition[targetPos];
        {1}; 
    }}
}}", hdnParam.ClientID, ControlsHelper.GetPostBackEventReference(btnMove)));

        sb.Append(
@"
if (window.recursiveDragAndDrop) {
    window.recursiveDragAndDrop = true;
}
if (window.lastDragAndDropBehavior) {
    lastDragAndDropBehavior._initializeDraggableItems();
}");

        sb.Append(
            @"
function ActivateBorder(elementId, className) {
  var e = document.getElementById(elementId);
  if (e != null) {
    e.className = e.className.replace(className, className + 'Active');
  }
}

function DeactivateBorder(elementId, className) {
  var e = document.getElementById(elementId);
  if (e != null) {
    e.className = e.className.replace(className + 'Active', className);
  }
}
");

        sb.Append(
@"function ChangeOperator(path, operator) {
    document.getElementById('", hdnOpSelected.ClientID, @"').value = path;
    document.getElementById('", hdnParam.ClientID, @"').value = operator;
    ", ControlsHelper.GetPostBackEventReference(btnChangeOperator), @"
}");

        sb.Append(
@"function ChangeParamValue(path, parameter) {
    document.getElementById('", hdnParamSelected.ClientID, @"').value = path;
    document.getElementById('", hdnParam.ClientID, @"').value = parameter;
    ", ControlsHelper.GetPostBackEventReference(btnChangeParameter), @"
}");

        sb.Append(
@"function InitDesignerAreaSize() {
    $cmsj('#", pnlCondtion.ClientID, @"').height(document.body.clientHeight - 295);
    $cmsj('#", lstRules.ClientID, @"').height(document.body.clientHeight - 287);
    $cmsj('.add-clause button').css('margin-top', (document.body.clientHeight - 164) / 2);
}

$cmsj(window).resize(InitDesignerAreaSize);
$cmsj(document).ready(InitDesignerAreaSize);
");

        sb.Append(
@"function HideParamEdit() {
    document.getElementById('" + hdnParamEditShown.ClientID + @"').value = '0';
}
");

        sb.Append(
@"$cmsj('#scrollDiv').scroll(function() {
  document.getElementById('" + hdnScroll.ClientID + @"').value = document.getElementById('scrollDiv').scrollTop;
});");
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "MacroRuleDesigner", ScriptHelper.GetScript(sb.ToString()));
    }

    #endregion


    #region "Sentences building"

    /// <summary>
    /// Renders complete rule.
    /// </summary>
    /// <param name="rule">Rule to render</param>
    private string GetRuleHTML(MacroRuleTree rule)
    {
        StringBuilder sb = new StringBuilder();

        // Append operator
        if (rule.Position > 0)
        {
            bool isAnd = (rule.Operator == "&&");
            sb.Append("<div class=\"MacroRuleOperator\" style=\"padding-left: ", 15 * (rule.Level - 1), "px\" onclick=\"ChangeOperator('", rule.IDPath, "', '", (isAnd ? "||" : "&&"), "');\">", (isAnd ? "and" : "or"), "</div>");
        }

        if (rule.IsLeaf)
        {
            sb.Append("<div id=\"", rule.IDPath, "\" class=\"MacroRule\" style=\"padding-left: ", 15 * (rule.Level - 1), "px\">");

            // Register position to a JS hashtable (for drag and drop purposes)
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "targetPosition" + counter, "targetPosition[" + counter++ + "] = '" + rule.Parent.IDPath + ";" + rule.Position + "';", true);

            sb.Append("<span id=\"ruleHandle" + rule.IDPath + "\"  class=\"MacroRuleHandle\">");
            string handleParams = "<span" + (rule.IsLeaf ? " onclick=\"SelectRule('" + rule.IDPath + "', $cmsj(this).parent()); return false;\"" : "") + "onmousedown=\"return false;\" onmouseover=\"ActivateBorder('ruleText" + rule.IDPath + "', 'MacroRuleText');\" onmouseout=\"DeactivateBorder('ruleText" + rule.IDPath + "', 'MacroRuleText');\">";
            string text = handleParams.Replace("##ID##", "0") + HTMLHelper.HTMLEncode(rule.RuleText) + "</span>";
            if (rule.Parameters != null)
            {
                foreach (string key in rule.Parameters.Keys)
                {
                    MacroRuleParameter p = rule.Parameters[key];

                    string paramText = (string.IsNullOrEmpty(p.Text) ? p.DefaultText : p.Text.TrimStart('#'));
                    paramText = MacroRuleTree.GetParameterText(paramText, true, null, p.ApplyValueTypeConversion ? p.ValueType : "text");

                    var parameterText = "</span><span class=\"MacroRuleParameter\" onclick=\"ChangeParamValue('" + rule.IDPath + "', " + ScriptHelper.GetString(key) + ");\">" + paramText + "</span>" + handleParams;

                    text = Regex.Replace(text, "\\{" + key + "\\}", TextHelper.EncodeRegexSubstitutes(parameterText), CMSRegex.IgnoreCase);
                }
            }
            bool isSelected = hdnSelected.Value.Contains(";" + rule.IDPath + ";");
            sb.Append("<div id=\"ruleText", rule.IDPath, "\" class=\"MacroRuleText", (isSelected ? " RuleSelected" : ""), "\">", text, "</div>");
            sb.Append("</span>");
            sb.Append("</div>");
        }
        else
        {
            foreach (MacroRuleTree child in rule.Children)
            {
                sb.Append(GetRuleHTML(child));
            }
        }

        return sb.ToString();
    }

    #endregion


    #region "Button operations

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        var textToFind = txtFilter.Text?.ToLowerInvariant() ?? string.Empty;

        foreach (ListItem item in lstRules.Items)
        {
            var text = item.Text?.ToLowerInvariant() ?? string.Empty;
            item.Enabled = text.Contains(textToFind);
        }
    }


    protected void btnClearAll_Click(object sender, EventArgs e)
    {
        RuleTree = new MacroRuleTree();
        hdnSelected.Value = "";
    }


    protected void btnViewCode_Click(object sender, EventArgs e)
    {
        viewCodeElem.Text = RuleTree.GetCondition();
        titleElem.TitleText = GetString("macros.macrorule.viewcodeheader");
        pnlViewCode.Visible = true;
        mdlDialog.Visible = true;
        mdlDialog.Show();
    }


    protected void btnMove_Click(object sender, EventArgs e)
    {
        string[] parts = hdnParam.Value.Split(';');
        if (parts.Length == 3)
        {
            var sourcePath = parts[0];

            var combinedParts = (string.IsNullOrEmpty(parts[1]) ? "" : parts[1] + ".") + parts[2];
            int plusOne = string.Compare(sourcePath, combinedParts, StringComparison.Ordinal);
            plusOne = (plusOne < 0 ? 1 : 0);

            var targetPath = (parts[1] == pnlCondtion.ClientID) ? "" : parts[1];

            RuleTree.MoveNode(sourcePath, targetPath, ValidationHelper.GetInteger(parts[2], 0) + plusOne);

            // Clear selection
            hdnSelected.Value = ";";
        }
    }


    protected void btnChangeParameter_Click(object sender, EventArgs e)
    {
        LoadFormDefinition(true);

        hdnLastSelected.Value = hdnParamSelected.Value;
        hdnLastParam.Value = hdnParam.Value;
        titleElem.TitleText = GetString("macros.macrorule.changeparameter");

        ShowParameterDialog();
    }


    protected void btnSetParameter_Click(object sender, EventArgs e)
    {
        var selected = GetSelected(hdnParamSelected.Value);
        if (selected != null)
        {
            string paramName = hdnParam.Value.ToLowerInvariant();

            var param = selected.Parameters[paramName];
            if (param != null)
            {
                if (formElem.ValidateData())
                {
                    // Load value from the form control
                    var ctrl = formElem.FieldControls[paramName];
                    if (ctrl != null)
                    {
                        var dataType = ctrl.FieldInfo.DataType;
                        var useNullInsteadOfDefaultValue = UseNullInsteadOfDefaultValue(dataType);

                        object convertedValue = DataTypeManager.ConvertToSystemType(TypeEnum.Field, dataType, ctrl.Value, null, useNullInsteadOfDefaultValue);

                        // Convert values to EN culture
                        string value = ValidationHelper.GetString(convertedValue, String.Empty, CultureHelper.EnglishCulture);
                        string displayName = ctrl.ValueDisplayName;

                        if (String.IsNullOrEmpty(displayName) && !String.IsNullOrEmpty(value))
                        {
                            displayName = value;
                            param.ApplyValueTypeConversion = true;
                        }

                        param.Value = value;
                        param.Text = displayName;
                        param.ValueType = dataType;
                    }

                    pnlModalProperty.Visible = false;
                    pnlFooter.Visible = false;
                }
                else
                {
                    ShowParameterDialog();
                }
            }
        }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        pnlModalProperty.Visible = false;
        pnlFooter.Visible = false;
    }


    protected void btnChangeOperator_Click(object sender, EventArgs e)
    {
        MacroRuleTree selected = GetSelected(hdnOpSelected.Value);
        if (selected != null)
        {
            selected.Operator = hdnParam.Value;
            if ((selected.Position == 1) && (selected.Parent != null))
            {
                // Change operator to previous sibling if we are changing the first operator in the group
                // It's because if we switch those two it should have same opearators
                selected.Parent.Children[0].Operator = selected.Operator;
            }
        }
    }


    protected void btnUnindent_Click(object sender, EventArgs e)
    {
        List<MacroRuleTree> selected = GetSelected();
        hdnSelected.Value = ";";
        foreach (MacroRuleTree item in selected)
        {
            item.Unindent();
            hdnSelected.Value += item.IDPath + ";";
        }
    }


    protected void btnIndent_Click(object sender, EventArgs e)
    {
        List<MacroRuleTree> selected = GetSelected();
        hdnSelected.Value = ";";
        foreach (MacroRuleTree item in selected)
        {
            item.Indent();
            hdnSelected.Value += item.IDPath + ";";
        }
    }


    protected void btnDelete_Click(object sender, EventArgs e)
    {
        List<MacroRuleTree> selected = GetSelected();
        foreach (MacroRuleTree item in selected)
        {
            item.Parent?.RemoveNode(item.Position);
        }
        hdnSelected.Value = "";
    }


    protected void btnAutoIndent_Click(object sender, EventArgs e)
    {
        MacroRuleTree.RemoveBrackets(RuleTree);
        RuleTree.AutoIndent();
    }


    protected void btnAddClause_Click(object sender, EventArgs e)
    {
        AddClause();
    }


    /// <summary>
    /// Adds a clause according to selected item.
    /// </summary>
    private void AddClause()
    {
        MacroRuleInfo rule = MacroRuleInfo.Provider.Get(ValidationHelper.GetInteger(lstRules.SelectedValue, 0));
        if (rule != null)
        {
            List<MacroRuleTree> selected = GetSelected();
            if (selected.Count == 1)
            {
                MacroRuleTree item = selected[0];
                if (item?.Parent != null)
                {
                    item.Parent.AddRule(rule, item.Position + 1);
                    return;
                }
            }

            // Add the rule at the root level, when no selected item
            RuleTree.AddRule(rule, RuleTree.Children.Count);
        }
    }


    /// <summary>
    /// Shows the parameter select dialog.
    /// </summary>
    private void ShowParameterDialog()
    {
        hdnParamEditShown.Value = "1";
        pnlModalProperty.Visible = true;
        pnlFooter.Visible = true;
        mdlDialog.Visible = true;
        mdlDialog.Show();
    }


    private static bool UseNullInsteadOfDefaultValue(string dataType)
    {
        // For data types except numeric and boolean ones we need to use null value instead of default one
        // To avoid usage of Guid.Empty for Guid data type etc.
        return !DataTypeManager.IsNumber(TypeEnum.Field, dataType) && !DataTypeManager.IsType<bool>(TypeEnum.Field, dataType);
    }

    #endregion


    #region "General methods"

    /// <summary>
    /// Gets the object from its IDPath.
    /// </summary>
    /// <param name="idPath">IDPath of the rule</param>
    private MacroRuleTree GetSelected(string idPath)
    {
        if (!string.IsNullOrEmpty(idPath))
        {
            string[] parts = idPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            MacroRuleTree srcGroup = RuleTree;
            foreach (string posStr in parts)
            {
                int pos = ValidationHelper.GetInteger(posStr, 0);
                if (srcGroup.Children.Count > pos)
                {
                    srcGroup = srcGroup.Children[pos];
                }
            }

            return srcGroup;
        }
        return null;
    }


    /// <summary>
    /// Returns list of selected objects (gets IDPaths from hidden field).
    /// </summary>
    private List<MacroRuleTree> GetSelected()
    {
        List<MacroRuleTree> selected = new List<MacroRuleTree>();
        if (!string.IsNullOrEmpty(hdnSelected.Value))
        {
            string[] ids = hdnSelected.Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // We need to sort the items, so as the items upper go sooner than items more down
            Array.Sort(ids);

            foreach (string id in ids)
            {
                selected.Add(GetSelected(id));
            }
        }
        return selected;
    }


    /// <summary>
    /// Loads the from definition from selected parameter into a BasicForm control.
    /// </summary>
    /// <param name="actual">If true, data from actual hiddens are loaded</param>
    private void LoadFormDefinition(bool actual)
    {
        MacroRuleTree selected = GetSelected((actual ? hdnParamSelected.Value : hdnLastSelected.Value));
        if (selected != null)
        {
            string paramName = (actual ? hdnParam.Value.ToLowerInvariant() : hdnLastParam.Value.ToLowerInvariant());
            MacroRuleParameter param = selected.Parameters[paramName];
            if (param != null)
            {
                FormInfo fi = new FormInfo(selected.RuleParameters);
                FormFieldInfo ffi = fi.GetFormField(paramName);
                if (ffi != null)
                {
                    fi = new FormInfo();
                    fi.AddFormItem(ffi);

                    DataRow row = fi.GetDataRow().Table.NewRow();

                    if (ffi.AllowEmpty && String.IsNullOrEmpty(param.Value))
                    {
                        if (!DataTypeManager.IsString(TypeEnum.Field, ffi.DataType))
                        {
                            row[paramName] = DBNull.Value;
                        }
                    }
                    else
                    {
                        // Convert to a proper type
                        var val = DataTypeManager.ConvertToSystemType(TypeEnum.Field, ffi.DataType, param.Value, CultureHelper.EnglishCulture);
                        if (val != null)
                        {
                            row[paramName] = val;
                        }
                    }

                    formElem.DataRow = row;
                    formElem.FormInformation = fi;
                    formElem.ReloadData();
                }
            }
        }
    }

    #endregion


    #region "Data methods"

    /// <summary>
    /// Returns the condition of the whole rule.
    /// </summary>
    public string GetCondition()
    {
        return RuleTree.GetCondition();
    }


    /// <summary>
    /// Returns the XML of the designer.
    /// </summary>
    public string GetXML()
    {
        return RuleTree.GetXML();
    }


    /// <summary>
    /// Loads the designer from xml.
    /// </summary>
    public void LoadFromXML(string xml)
    {
        try
        {
            MacroRuleTree ruleTree = new MacroRuleTree();

            ruleTree.LoadFromXml(xml);
            ViewState["RuleTree"] = ruleTree;
        }
        catch
        {
        }
    }


    /// <summary>
    /// Extracts the condition from Rule method.
    /// </summary>
    public string ConditionFromExpression(string expression)
    {
        MacroExpression xml = null;
        try
        {
            xml = MacroExpression.ExtractParameter(expression, "rule", 1);
        }
        catch
        {
        }

        MacroIdentityOption identityOption;
        if (xml == null)
        {
            return MacroSecurityProcessor.RemoveMacroSecurityParams(expression, out identityOption);
        }

        // Returns first parameter of the expression
        return MacroSecurityProcessor.RemoveMacroSecurityParams(ValidationHelper.GetString(xml.Value, ""), out identityOption);
    }


    /// <summary>
    /// Parses the rule tree from Rule expression.
    /// </summary>
    public void ParseFromExpression(string expression)
    {
        MacroExpression xml = MacroExpression.ExtractParameter(expression, "rule", 1);
        if (xml?.Type == ExpressionType.Value)
        {
            // Load from the XML
            LoadFromXML(xml.Value.ToString());
            return;
        }

        // If something went wrong, assign null to the state variable
        ViewState["RuleTree"] = null;
    }

    #endregion
}
