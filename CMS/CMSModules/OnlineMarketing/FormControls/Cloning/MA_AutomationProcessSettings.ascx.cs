using System;

using CMS.Automation;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_OnlineMarketing_FormControls_Cloning_MA_AutomationProcessSettings : CloneSettingsControl
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
            return chkWorkflowTrigger.Checked ? base.ExcludedChildTypes : ObjectWorkflowTriggerInfo.OBJECT_TYPE;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblWorkflowTrigger.ToolTip = GetString("clonning.settings.workflow.workflowtrigger.tooltip");
    }

    #endregion
}