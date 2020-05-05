using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.WorkflowEngine;


public partial class CMSModules_Automation_FormControls_ProcessStatusSelector : CMSAbstractBaseFilterControl
{

    #region "Properties"

    public override string WhereCondition
    {
        get
        {
            if (!String.IsNullOrEmpty(drpStatuses.SelectedValue))
            {
                return "StateStatus = " + drpStatuses.SelectedValue;
            }
            else
            {
                return String.Empty;
            }
        }
        set
        {
            drpStatuses.SelectedValue = value;
        }
    }

    #endregion


    #region "Control events"


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        InitDropDownList();
    }


    /// <summary>
    /// Fills drop down control with data.
    /// </summary>
    private void InitDropDownList()
    {
        drpStatuses.Items.Add(new ListItem(GetString("general.selectall"), String.Empty));
        drpStatuses.Items.Add(new ListItem(WorkflowHelper.GetProcessStatusString(ProcessStatusEnum.Pending), ((int)ProcessStatusEnum.Pending).ToString()));
        drpStatuses.Items.Add(new ListItem(WorkflowHelper.GetProcessStatusString(ProcessStatusEnum.Processing), ((int)ProcessStatusEnum.Processing).ToString()));
        drpStatuses.Items.Add(new ListItem(WorkflowHelper.GetProcessStatusString(ProcessStatusEnum.Finished), ((int)ProcessStatusEnum.Finished).ToString()));

        drpStatuses.SelectedIndex = 0;
    }

    
    /// <summary>
    /// Handles reset filter.
    /// </summary>
    public override void ResetFilter()
    {
        drpStatuses.SelectedIndex = 0;
    }

    #endregion
}
