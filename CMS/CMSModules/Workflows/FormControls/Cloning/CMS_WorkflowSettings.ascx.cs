using System;

using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Workflows_FormControls_Cloning_CMS_WorkflowSettings : CloneSettingsControl
{
    #region "Properties"
    
    /// <summary>
    /// Excluded binding types.
    /// </summary>
    public override string ExcludedBindingTypes
    {
        get
        {
            return WorkflowTransitionInfo.OBJECT_TYPE;
        }
    }


    /// <summary>
    /// Excluded other binding types.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return WorkflowTransitionInfo.OBJECT_TYPE;
        }
    }


    /// <summary>
    /// Excluded other binding types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return chkWorkflowScope.Checked ? base.ExcludedChildTypes : WorkflowScopeInfo.OBJECT_TYPE;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblWorkflowScope.ToolTip = GetString("clonning.settings.workflow.workflowscope.tooltip");
    }

    #endregion
}