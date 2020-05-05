using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Web.UI;


[EditedObject("cms.workflowaction", "actionId")]
[UIElement(ModuleName.CMS, "Workflow.Actions.Parameters")]
public partial class CMSModules_Workflows_Pages_WorkflowAction_Tab_Parameters : CMSWorkflowPage
{
    #region "Private properties"

    private WorkflowActionInfo ActionInfo
    {
        get
        {
            return (WorkflowActionInfo)EditedObject;
        }
    }

    #endregion


    #region "Event handlers"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";
        CurrentMaster.BodyClass += " FieldEditorBody";
        
        InitializeFieldEditor();
        
        ScriptHelper.HideVerticalTabs(this);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes field editor.
    /// </summary>
    private void InitializeFieldEditor()
    {
        if (ActionInfo == null)
        {
            return;
        }
        fieldEditor.FormDefinition = ActionInfo.ActionParameters;
        fieldEditor.OnAfterDefinitionUpdate += fieldEditor_OnAfterDefinitionUpdate;
    }


    /// <summary>
    /// Field editor updated event.
    /// </summary>
    private void fieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        if (ActionInfo != null)
        {
            ActionInfo.ActionParameters = fieldEditor.FormDefinition;
            WorkflowActionInfoProvider.SetWorkflowActionInfo(ActionInfo);
        }
    }

    #endregion
}
