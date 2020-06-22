using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Definitions;


public partial class CMSModules_Workflows_FormControls_SourcePointSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Selected source point GUID.
    /// </summary>
    public override object Value
    {
        get
        {
            return SourcePointGuid;
        }
        set
        {
            SourcePointGuid = ValidationHelper.GetGuid(value, Guid.Empty);
        }
    }


    /// <summary>
    /// Selected source point GUID.
    /// </summary>
    public Guid SourcePointGuid
    {
        get
        {
            return ValidationHelper.GetGuid(ddlSourcePoints.SelectedValue, Guid.Empty);
        }
        set
        {
            ddlSourcePoints.SelectedValue = value.ToString();
        }
    }


    /// <summary>
    /// Gets or sets state enable.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            ddlSourcePoints.Enabled = value;
        }
    }


    /// <summary>
    /// ID of workflow step containing listed source points.
    /// </summary>
    public int WorkflowStepID
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing!
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        if (ddlSourcePoints.Items.Count == 0)
        {
            WorkflowStepInfo step = WorkflowStepInfo.Provider.Get(WorkflowStepID);
            if (step != null)
            {
                Step definition = step.StepDefinition;
                if ((definition == null) || (definition.SourcePoints == null))
                {
                    Visible = false;
                }
                else
                {
                    List<SourcePoint> sourcePoints = definition.SourcePoints;
                    Guid? defaultCase = null;
                    List<WorkflowTransitionInfo> connections = WorkflowTransitionInfoProvider.GetStepTransitions(step);

                    foreach (SourcePoint sp in sourcePoints)
                    {
                        AddItem(sp, connections);

                        if (sp.Type == SourcePointTypeEnum.SwitchDefault)
                        {
                            defaultCase = sp.Guid;
                        }
                    }

                    // Ensure default source point for timeout
                    if (step.StepAllowDefaultTimeoutTarget && !sourcePoints.Exists(s => (s is TimeoutSourcePoint)))
                    {
                        AddItem(new TimeoutSourcePoint(), connections);
                    }

                    if (definition.TimeoutTarget != Guid.Empty)
                    {
                        ddlSourcePoints.SelectedValue = definition.TimeoutTarget.ToString();
                    }
                    else if (defaultCase.HasValue)
                    {
                        ddlSourcePoints.SelectedValue = defaultCase.Value.ToString();
                    }
                }
            }
        }

        // Set visibility
        Visible = IsVisible();
    }


    /// <summary>
    /// Indicates if the control should be visible.
    /// </summary>
    /// <returns>TRUE if the control should be visible.</returns>
    public bool IsVisible()
    {
        return (ddlSourcePoints.Items.Count >= 2);
    }


    private void AddItem(SourcePoint sp, List<WorkflowTransitionInfo> connections)
    {
        string label = String.Format(GetString("wf.sourcepoint.label"), sp.Label);
        WorkflowTransitionInfo conn = connections.FirstOrDefault(i => i.TransitionSourcePointGUID == sp.Guid);
        if (conn != null)
        {
            WorkflowStepInfo target = WorkflowStepInfo.Provider.Get(conn.TransitionEndStepID);
            if (target != null)
            {
                label = String.Format("{0} {1}", label, string.Format(GetString("wf.sourcepoint.step"), target.StepDisplayName));
            }
        }

        ddlSourcePoints.Items.Add(new ListItem(ResHelper.LocalizeString(label), sp.Guid.ToString()));
    }

    #endregion
}