using System;
using System.Collections.Generic;

using CMS.DataEngine;
using CMS.UIControls;
using CMS.UIControls.UniMenuConfig;
using CMS.WorkflowEngine.Factories;
using CMS.WorkflowEngine.Web.UI;

public partial class CMSModules_Workflows_Controls_WorkflowDesignerToolbar : GraphDesignerToolbar
{
    private const string CONTROL_PATH = "~/CMSAdminControls/UI/UniMenu/UniGraphToolbar/NodesGroup.ascx";
    private StepTypeDependencyInjector<Item> mInjector;


    /// <summary>
    /// Factory for uniMenu items.
    /// </summary>
    protected override StepTypeDependencyInjector<Item> Injector
    {
        get
        {
            return mInjector ?? (mInjector = new StepTypeUniMenuItems(JsGraphObject));
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        if (!Visible)
        {
            return;
        }

        RegisterInitScript(pnlAjax.ClientID);

        editToolbar.JsGraphObject = JsGraphObject;
        toolbar.Groups = GetGroups();

        base.OnLoad(e);
    }


    protected override WhereCondition GetWhereCondition()
    {
        var condition = base.GetWhereCondition();

        // Workflow type
        condition.And(new WhereCondition().WhereEquals("ActionWorkflowType", (int)Workflow.WorkflowType).Or().WhereNull("ActionWorkflowType"));

        return condition;
    }


    private List<Group> GetGroups()
    {
        var settingsGroup = new Group
        {
            Items = GetGroupItemsFromEnum(GetBasicItems(), editToolbar.Search)
        };
        EnsureControl(settingsGroup);

        var workflowGroup = new Group
        {
            Items = GetGroupItemsFromDB(editToolbar.Search)
        };
        EnsureControl(workflowGroup);

        return new List<Group>
        {
            settingsGroup,
            workflowGroup
        };
    }


    private List<WorkflowStepTypeEnum> GetBasicItems()
    {
        return new List<WorkflowStepTypeEnum>
        {
            WorkflowStepTypeEnum.Standard,
            WorkflowStepTypeEnum.Condition,
            WorkflowStepTypeEnum.Multichoice,
            WorkflowStepTypeEnum.MultichoiceFirstWin,
            WorkflowStepTypeEnum.Userchoice,
            WorkflowStepTypeEnum.Wait,
            WorkflowStepTypeEnum.DocumentPublished,
            WorkflowStepTypeEnum.DocumentArchived
        };
    }


    private void EnsureControl(Group group)
    {
        group.ControlPath = CONTROL_PATH;

        var guid = Guid.NewGuid().ToString();

        var nodesControl = (CMSUserControl)LoadUserControl(group.ControlPath);
        nodesControl.ID = "groupControl" + guid;
        nodesControl.ShortID = "gc" + guid;
        nodesControl.SetValue("NodesMenuItems", group.Items);

        group.Control = nodesControl;
    }
}
