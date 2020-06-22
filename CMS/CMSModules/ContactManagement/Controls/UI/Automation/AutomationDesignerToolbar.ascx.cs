using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls.UniMenuConfig;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Factories;
using CMS.WorkflowEngine.Web.UI;

public partial class CMSModules_ContactManagement_Controls_UI_Automation_AutomationDesignerToolbar : GraphDesignerToolbar
{
    private readonly StringBuilder mStartupScriptBuilder = new StringBuilder();
    private StepTypeDependencyInjector<Item> mInjector;


    /// <summary>
    /// Factory for uniMenu items.
    /// </summary>
    protected override StepTypeDependencyInjector<Item> Injector
    {
        get
        {
            return mInjector ?? (mInjector = new StepTypeAutomationUniMenuItems(JsGraphObject));
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        if (!Visible)
        {
            return;
        }

        txtSearch.WatermarkText = GetString("unigraphToolbar.searchTooltip");

        InitializeAutomationSteps();

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsAsyncPostback())
        {
            RegisterDynamicSearchScript();
            RegisterToggleCategoryScript();
            RegisterInitScript(pnlAjax.ClientID);
        }
    }


    protected override WhereCondition GetWhereCondition()
    {
        var condition = base.GetWhereCondition();

        // Automation type
        condition.And(new WhereCondition().WhereEquals("ActionWorkflowType", (int)WorkflowTypeEnum.Automation));

        return condition;
    }


    private void RegisterToggleCategoryScript()
    {
        ScriptHelper.RegisterScriptFile(pnlAjax, "~/CMSScripts/Controls/CollapsibleCategory.js");
    }


    private void RegisterDynamicSearchScript()
    {
        var dynamicSearchScript = $@"
var filterStepsTimer;

function proceedSpecialKeys(el, e) {{
    // CR
    if (e.keyCode == 13) {{
        return false;
    }}
    // ESC
    else if (e.keyCode == 27) {{
        $cmsj(el).val('');
        e.preventDefault();
        e.stopPropagation();
        return false
    }}

    return true;
}}

function filterSteps() {{
    window.clearTimeout(filterStepsTimer);
    filterStepsTimer = window.setTimeout(function () {{
        {ControlsHelper.GetPostBackEventReference(pnlAjax)}
    }}, 200);
}}

$cmsj('#{txtSearch.ClientID}')
    .keypress(function (e) {{
        window.clearTimeout(filterStepsTimer);
        return proceedSpecialKeys(this, e);
    }})
    .keyup(function (e) {{
        var ret = proceedSpecialKeys(this, e);
        filterSteps();
        return ret; 
    }});";

        ScriptHelper.RegisterStartupScript(txtSearch, typeof(string), "AutomationToolbarDynamicSearchScript", dynamicSearchScript, true);
    }


    private void InitializeAutomationSteps()
    {
        var groups = GetGroups().Where(g => g.Items.Any()).ToList();

        if (groups.Count == 0)
        {
            lblNoResults.Visible = true;
        }
        else
        {
            groupRepeater.DataSource = groups;
            groupRepeater.DataBind();

            ScriptHelper.RegisterStartupScript(pnlAjax, typeof(string), "UniMenuStartupScript_" + pnlAjax.ClientID, ScriptHelper.GetScript(mStartupScriptBuilder.ToString()));
            mStartupScriptBuilder.Clear();
        }

        pnlAjax.Update();
    }


    private IEnumerable<Group> GetGroups()
    {
        var filterText = txtSearch.Text.Trim();

        yield return new Group
        {
            Caption = GetString("ma.designertoolbar.flowstepsgroup"),
            Items = GetGroupItemsFromEnum(GetBasicItems(), filterText)
        };

        yield return new Group
        {
            Caption = GetString("ma.designertoolbar.contactactionsgroup"),
            Items = GetGroupItemsFromDB(filterText)
        };

        yield return new Group
        {
            Caption = GetString("general.other"),
            Items = GetGroupItemsFromEnum(GetOtherItems(), filterText)
        };
    }


    private List<WorkflowStepTypeEnum> GetBasicItems()
    {
        return new List<WorkflowStepTypeEnum>
        {
            WorkflowStepTypeEnum.Standard,
            WorkflowStepTypeEnum.MultichoiceFirstWin,
            WorkflowStepTypeEnum.Userchoice,
            WorkflowStepTypeEnum.Wait,
            WorkflowStepTypeEnum.Finished
        };
    }


    private List<WorkflowStepTypeEnum> GetOtherItems()
    {
        return new List<WorkflowStepTypeEnum>
        {
            WorkflowStepTypeEnum.Note
        };
    }


    /// <summary>
    /// Appends draggable startup script of each automation step to the <see cref="mStartupScriptBuilder"/>.
    /// </summary>
    protected void stepRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var stepItem = e.Item.DataItem as Item;

        var stepControl = ControlsHelper.GetChildControl<CMSModules_ContactManagement_Controls_UI_Automation_AutomationDesignerToolbarStep>(e.Item);
        if (stepItem == null || stepControl == null)
        {
            return;
        }

        var stepPanel = ControlsHelper.GetChildControl<Panel>(stepControl);
        if (stepPanel == null)
        {
            return;
        }

        var stepDraggableScript = $"$cmsj( '#{stepPanel.ClientID}' ).draggable({{ helper:{GetDraggableHandler(stepItem.DraggableTemplateHandler)}, scope:'{stepItem.DraggableScope}', containment:'body' }});";
        mStartupScriptBuilder.AppendLine(stepDraggableScript);
    }


    /// <summary>
    /// Method for formatting draggable handler definition.
    /// </summary>
    /// <returns>clone if default</returns>
    private string GetDraggableHandler(string draggableTemplateHandler)
    {
        if (string.IsNullOrEmpty(draggableTemplateHandler))
        {
            return "clone";
        }
        return $"function(){{ return $cmsj(\"{draggableTemplateHandler}\"); }}";
    }
}
