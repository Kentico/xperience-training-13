using System;
using System.Web.UI.WebControls;

using CMS.Automation;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Automation_Controls_Process_Edit : CMSUserControl
{
    private CMSAutomationManager mAutomationManager;


    /// <summary>
    /// Object instance
    /// </summary>
    public BaseInfo InfoObject => AutomationManager.InfoObject;


    /// <summary>
    /// State object
    /// </summary>
    public AutomationStateInfo StateObject => AutomationManager.StateObject;


    /// <summary>
    /// Automation manager control
    /// </summary>
    public CMSAutomationManager AutomationManager
    {
        get
        {
            if (mAutomationManager == null)
            {
                mAutomationManager = ControlsHelper.GetChildControl(Page, typeof(CMSAutomationManager)) as CMSAutomationManager;
                if (mAutomationManager == null)
                {
                    throw new Exception("[AutomationMenu.AutomationManager]: Missing automation manager.");
                }
            }

            return mAutomationManager;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ReloadData();
    }


    /// <summary>
    /// Reloads the page data.
    /// </summary>
    protected void ReloadData()
    {
        if (InfoObject != null)
        {
            var workflow = AutomationManager.Process;
            if (workflow != null)
            {
                ucDesigner.WorkflowID = workflow.WorkflowID;
                ucDesigner.SelectedStepID = StateObject.StateStepID;
                ucDesigner.Height = new Unit("88%");
                ucDesigner.ReadOnly = true;
            }
        }
        else
        {
            pnlWorkflow.Visible = false;
        }
    }
}
