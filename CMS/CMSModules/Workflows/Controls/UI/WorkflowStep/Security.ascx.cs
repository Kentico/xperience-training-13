using System;
using System.Data;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Definitions;


public partial class CMSModules_Workflows_Controls_UI_WorkflowStep_Security : CMSAdminEditControl
{
    #region "Variables"
        
    private WorkflowStepInfo mWorkflowStep;
    private WorkflowInfo mWorkflow;
    private SourcePoint mCurrentSourcePoint;
    protected string currentRoles = string.Empty;
    protected string currentUsers = string.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Workflow step ID
    /// </summary>
    public int WorkflowStepID
    {
        get;
        set;
    }


    /// <summary>
    /// With source point GUID set, this control manages security for source point. With null manages security for workflow step.
    /// </summary>
    public Guid? SourcePointGuid
    {
        get;
        set;
    }


    /// <summary>
    /// Workflow step
    /// </summary>
    public WorkflowStepInfo WorkflowStep => mWorkflowStep ?? (mWorkflowStep = WorkflowStepInfo.Provider.Get(WorkflowStepID));


    /// <summary>
    /// Workflow
    /// </summary>
    private WorkflowInfo Workflow
    {
        get
        {
            if (mWorkflow == null && WorkflowStep != null)
            {
                mWorkflow = WorkflowInfo.Provider.Get(WorkflowStep.StepWorkflowID);
            }

            return mWorkflow;
        }
    }


    /// <summary>
    /// Source point on workflow step
    /// </summary>
    public SourcePoint CurrentSourcePoint
    {
        get
        {
            if ((mCurrentSourcePoint == null) && (SourcePointGuid != null))
            {
                mCurrentSourcePoint = WorkflowStep.StepDefinition.SourcePoints.FirstOrDefault(i => i.Guid == SourcePointGuid);
            }
            return mCurrentSourcePoint;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            usRoles.StopProcessing = value;
            usUsers.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            usRoles.IsLiveSite = value;
            usUsers.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Roles security of workflow step or its source point
    /// </summary>
    private WorkflowStepSecurityEnum RolesSecurity
    {
        get
        {
            if (SourcePointGuid == null)
            {
                return WorkflowStep.StepRolesSecurity;
            }

            return CurrentSourcePoint?.StepRolesSecurity ?? WorkflowStepSecurityEnum.Default;
        }
        set
        {
            if (SourcePointGuid == null)
            {
                WorkflowStep.StepRolesSecurity = value;
            }
            else
            {
                if (CurrentSourcePoint != null)
                {
                    CurrentSourcePoint.StepRolesSecurity = value;
                }
            }
        }
    }


    /// <summary>
    /// Users security of workflow step or its source point
    /// </summary>
    private WorkflowStepSecurityEnum UsersSecurity
    {
        get
        {
            if (SourcePointGuid == null)
            {
                return WorkflowStep.StepUsersSecurity;
            }

            return CurrentSourcePoint?.StepUsersSecurity ?? WorkflowStepSecurityEnum.Default;
        }
        set
        {
            if (SourcePointGuid == null)
            {
                WorkflowStep.StepUsersSecurity = value;
            }
            else
            {
                if (CurrentSourcePoint != null)
                {
                    CurrentSourcePoint.StepUsersSecurity = value;
                }
            }
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (WorkflowStepID <= 0)
        {
            StopProcessing = true;
            return;
        }

        EditedObject = WorkflowStep;

        if (SourcePointGuid != null)
        {
            ShowInformation(GetString("workflowsteppoint.securityInfo"));
        }
        else if (WorkflowStep.StepAllowBranch)
        {
            ShowInformation(GetString("workflowstep.securityInfo"));
        }

        if (Workflow.IsAutomation)
        {
            headRoles.ResourceString = "processstep.rolessecurity";
            headUsers.ResourceString = "processstep.userssecurity";
        }
        else
        {
            headRoles.ResourceString = (SourcePointGuid == null) ? "workflowstep.rolessecurity" : "workflowsteppoint.rolessecurity";
            headUsers.ResourceString = (SourcePointGuid == null) ? "workflowstep.userssecurity" : "workflowsteppoint.userssecurity";
        }

        SetupRolesSelector();
        SetupUsersSelector();

        rbRoleType.SelectedIndexChanged += rbRoleType_SelectedIndexChanged;
        rbUserType.SelectedIndexChanged += rbUserType_SelectedIndexChanged;

        if (!RequestHelper.IsPostBack())
        {
            string resPrefix = (SourcePointGuid == null) ? "workflowstep" : "workflowsteppoint";
            ControlsHelper.FillListControlWithEnum<WorkflowStepSecurityEnum>(rbRoleType, resPrefix + ".security");
            ControlsHelper.FillListControlWithEnum<WorkflowStepSecurityEnum>(rbUserType, resPrefix + ".usersecurity");
            rbRoleType.SelectedValue = ((int)RolesSecurity).ToString();
            rbUserType.SelectedValue = ((int)UsersSecurity).ToString();
        }

        // Get the active roles for this site
        string where = $"[StepID] = {WorkflowStepID} AND [StepSourcePointGuid] {(SourcePointGuid == null ? "IS NULL" : $"= '{SourcePointGuid}'")}";

        var roles = WorkflowStepRoleInfo.Provider.Get()
            .Where(where)
            .Column("RoleID")
            .GetListResult<int>();
        currentRoles = string.Join(";", roles.ToArray());

        // Get the active users for this site
        var users = WorkflowStepUserInfo.Provider.Get()
            .Where(where)
            .Column("UserID")
            .GetListResult<int>();
        currentUsers = string.Join(";", users.ToArray());
                
        // Init lists when security type changes
        if (!RequestHelper.IsPostBack() || ControlsHelper.GetPostBackControlID(Page).StartsWith(rbRoleType.UniqueID, StringComparison.Ordinal) || ControlsHelper.GetPostBackControlID(Page).StartsWith(rbUserType.UniqueID, StringComparison.Ordinal))
        {
            usRoles.Value = currentRoles;
            usUsers.Value = currentUsers;
        }
    }
    

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (WorkflowStepID > 0)
        {
            usRoles.Visible = RolesSecurity != WorkflowStepSecurityEnum.Default;
            usUsers.Visible = UsersSecurity != WorkflowStepSecurityEnum.Default;
        }
    }


    protected void rbRoleType_SelectedIndexChanged(object sender, EventArgs e)
    {
        RolesSecurity = (WorkflowStepSecurityEnum)ValidationHelper.GetInteger(rbRoleType.SelectedValue, 0);
        WorkflowStep.Update();
        ShowChangesSaved();
    }


    protected void rbUserType_SelectedIndexChanged(object sender, EventArgs e)
    {
        UsersSecurity = (WorkflowStepSecurityEnum)ValidationHelper.GetInteger(rbUserType.SelectedValue, 0);
        WorkflowStep.Update();
        ShowChangesSaved();
    }


    protected void usRoles_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveRolesData();
        pnlUpdateRoles.Update();
    }


    protected void usUsers_OnSelectionChanged(object sender, EventArgs e)
    {

        SaveUsersData();
        pnlUpdateUsers.Update();
    }


    private void SetupRolesSelector()
    {
        usRoles.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
        usRoles.SetValue("DefaultFilterValue", SiteContext.CurrentSiteID);
        usRoles.AdditionalColumns = "SiteId";
        usRoles.OnSelectionChanged += usRoles_OnSelectionChanged;
        usRoles.ObjectType = RoleInfo.OBJECT_TYPE;
        usRoles.UniGrid.OnLoadColumns += PrepareColumns;
    }


    private void SetupUsersSelector()
    {
        usUsers.OnSelectionChanged += usUsers_OnSelectionChanged;
        usUsers.ObjectType = UserInfo.OBJECT_TYPE;
        usUsers.WhereCondition = "(UserIsHidden = 0 OR UserIsHidden IS NULL)";
    }

    #endregion


    #region "Control handling"

    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        pnlUpdateRoles.Update();
    }


    /// <summary>
    /// Saves roles data
    /// </summary>
    private void SaveRolesData()
    {
        Guid stepSourcePointGuid = SourcePointGuid ?? Guid.Empty;

        // Remove old items
        string newValues = ValidationHelper.GetString(usRoles.Value, null);
        string removedRoles = DataHelper.GetNewItemsInList(newValues, currentRoles);

        if (!String.IsNullOrEmpty(removedRoles))
        {
            string[] newItems = removedRoles.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Add all new items to site
            foreach (string item in newItems)
            {
                int roleId = ValidationHelper.GetInteger(item, 0);

                // If role is authorized, remove it
                WorkflowStepRoleInfo wsr = WorkflowStepRoleInfo.Provider.Get(WorkflowStepID, roleId, stepSourcePointGuid);
                wsr?.Delete();
            }
        }

        // Add new items
        string addedRoles = DataHelper.GetNewItemsInList(currentRoles, newValues);
        if (!String.IsNullOrEmpty(addedRoles))
        {
            string[] newItems = addedRoles.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (string item in newItems)
            {                
                int roleId = ValidationHelper.GetInteger(item, 0);

                // If role is not authorized, authorize it
                if (WorkflowStepRoleInfo.Provider.Get(WorkflowStepID, roleId, stepSourcePointGuid) == null)
                {
                    WorkflowStepRoleInfo.Provider.Add(WorkflowStepID, roleId, stepSourcePointGuid);
                }
            }
        }
        
        ShowChangesSaved();
    }


    /// <summary>
    /// Saves users data
    /// </summary>
    private void SaveUsersData()
    {
        Guid stepSourcePointGuid = SourcePointGuid ?? Guid.Empty;

        // Remove old items
        string newValues = ValidationHelper.GetString(usUsers.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentUsers);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Add all new items to site
            foreach (string item in newItems)
            {
                int userId = ValidationHelper.GetInteger(item, 0);
                // If user is authorized, remove it
                WorkflowStepUserInfo wsu = WorkflowStepUserInfo.Provider.Get(WorkflowStepID, userId, stepSourcePointGuid);
                wsu?.Delete();
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentUsers, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Add all new items to site
            foreach (string item in newItems)
            {
                int userId = ValidationHelper.GetInteger(item, 0);

                // If user is not authorized, authorize it
                if (WorkflowStepUserInfo.Provider.Get(WorkflowStepID, userId, stepSourcePointGuid) == null)
                {
                    WorkflowStepUserInfo.Provider.Add(WorkflowStepID, userId, stepSourcePointGuid);
                }
            }
        }

        ShowChangesSaved();
    }


    private void PrepareColumns()
    {
        if (usRoles.UniGrid.GridColumns == null)
        {
            return;
        }
        
        var siteColumn = new Column
        {
            Name = "sitename",
            Source = "SiteID",
            ExternalSourceName = "#sitenameorglobal",
            Wrap = false,
            Caption = "$general.sitename$"
        };

        var fillingColumn = new Column
        {
            CssClass = "filling-column"
        };

        usRoles.UniGrid.GridColumns.Columns.Add(siteColumn);
        usRoles.UniGrid.GridColumns.Columns.Add(fillingColumn);

        usRoles.UniGrid.GridColumns.Columns[1].CssClass = string.Empty;
    }

    #endregion
}

