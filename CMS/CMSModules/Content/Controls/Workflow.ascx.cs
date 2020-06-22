using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Content_Controls_Workflow : CMSUserControl
{
    #region "Private variables"

    // Current Node
    private TreeNode mNode;
    private TreeProvider mTree;
    private WorkflowManager mWorkflowManager;
    private int mNodeId = -1;
    private int currentStepId;
    private WorkflowInfo mWorkflowInfo;

    private UserInfo currentUserInfo;
    private SiteInfo currentSiteInfo;

    private const string CONTENT_FOLDER = "~/CMSModules/Content/";
    private const string CONTENT_PROPERTIES_FOLDER = CONTENT_FOLDER + "CMSDesk/Properties/";

    #endregion


    #region "Properties"

    /// <summary>
    /// Tree provider
    /// </summary>
    public TreeProvider Tree
    {
        get
        {
            return mTree ?? (mTree = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
        set
        {
            mTree = value;
        }
    }


    /// <summary>
    /// Workflow manager
    /// </summary>
    private WorkflowManager WorkflowManager
    {
        get
        {
            return mWorkflowManager ?? (mWorkflowManager = WorkflowManager.GetInstance(Tree));
        }
    }


    /// <summary>
    /// Tree node.
    /// </summary>
    public TreeNode Node
    {
        get
        {
            return mNode ?? (mNode = DocumentHelper.GetDocument(NodeID, LocalizationContext.PreferredCultureCode, Tree));
        }
        set
        {
            mNode = value;

            mNodeId = -1;
            if (value != null)
            {
                mNodeId = value.NodeID;
            }
        }
    }

    
    /// <summary>
    /// Identifier of current node
    /// </summary>
    private int NodeID
    {
        get
        {
            return mNodeId;
        }
    }


    /// <summary>
    /// Workflow info object
    /// </summary>
    private WorkflowInfo WorkflowInfo
    {
        get
        {
            if (mWorkflowInfo == null)
            {
                if (Node != null)
                {
                    mWorkflowInfo = Node.GetWorkflow();
                }
            }

            return mWorkflowInfo;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        gridHistory.GridName = CONTENT_PROPERTIES_FOLDER + "WorkflowHistory.xml";
        gridSteps.GridName = CONTENT_PROPERTIES_FOLDER + "WorkflowSteps.xml";

        gridSteps.WhereCondition = "StepWorkflowID = @StepWorkflowID";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Turn sorting off
        gridSteps.GridView.AllowSorting = false;
        ReloadData();

        string viewVersionUrl = IsLiveSite ? ApplicationUrlHelper.ResolveDialogUrl(CONTENT_FOLDER + "CMSPages/Versions/ViewVersion.aspx") : ResolveUrl(CONTENT_PROPERTIES_FOLDER + "ViewVersion.aspx");

        string viewVersionScript = ScriptHelper.GetScript("function ViewVersion(versionHistoryId) {window.open('" + viewVersionUrl + "?versionHistoryId=' + versionHistoryId)}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "viewVersionScript", viewVersionScript);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }

    #endregion


    #region "Grid events"

    /// <summary>
    /// External step binding.
    /// </summary>
    protected object gridSteps_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "currentstepinfo":
                DataRowView data = (DataRowView)parameter;
                if (currentStepId <= 0)
                {
                    WorkflowStepTypeEnum stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(data["StepType"], 1);
                    if (stepType == WorkflowStepTypeEnum.DocumentEdit)
                    {
                        return UIHelper.GetAccessibleIconTag("icon-arrow-right");
                    }
                }
                else
                {
                    // Check if version history exists and node is published
                    if (Node.IsPublished && (Node.DocumentCheckedOutVersionHistoryID <= 0))
                    {
                        WorkflowStepTypeEnum stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(data["StepType"], 1);
                        if (stepType == WorkflowStepTypeEnum.DocumentPublished)
                        {
                            return UIHelper.GetAccessibleIconTag("icon-arrow-right");
                        }
                    }
                    else
                    {
                        int stepId = ValidationHelper.GetInteger(data["StepID"], 0);
                        if (stepId == currentStepId)
                        {
                            return UIHelper.GetAccessibleIconTag("icon-arrow-right");
                        }
                    }
                }
                return string.Empty;

            case "steporder":
                if (sender != null)
                {
                    // Get grid row
                    GridViewRow row = (GridViewRow)((DataControlFieldCell)sender).Parent;
                    int pageOffset = (gridSteps.Pager.CurrentPage - 1) * gridSteps.Pager.CurrentPageSize;
                    // Return row index
                    return (pageOffset + row.RowIndex + 1).ToString();
                }
                return string.Empty;
        }
        return parameter;
    }


    /// <summary>
    /// External history binding.
    /// </summary>
    protected object gridHistory_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView drv;
        switch (sourceName.ToLowerInvariant())
        {
            case "action":
                drv = (DataRowView)parameter;
                bool wasRejected = ValidationHelper.GetBoolean(drv["WasRejected"], false);

                // Get type of the steps
                WorkflowStepTypeEnum stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "StepType"), 0);
                WorkflowStepTypeEnum targetStepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "TargetStepType"), (int)stepType);
                WorkflowTransitionTypeEnum transitionType = (WorkflowTransitionTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "HistoryTransitionType"), 0);

                // Get name of steps
                string stepName = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "StepName"), String.Empty);
                string targetStepName = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "TargetStepName"), stepName);
                if (!wasRejected)
                {
                    // If step type defined, use it for identification
                    if (targetStepType != WorkflowStepTypeEnum.Undefined)
                    {
                        bool isAutomatic = (transitionType == WorkflowTransitionTypeEnum.Automatic);
                        string actionString = isAutomatic ? GetString("WorfklowProperties.Automatic") + " ({0})" : "{0}";
                        // Return correct step title
                        switch (targetStepType)
                        {
                            case WorkflowStepTypeEnum.DocumentArchived:
                                actionString = string.Format(actionString, GetString("WorfklowProperties.Archived"));
                                break;

                            case WorkflowStepTypeEnum.DocumentPublished:
                                actionString = string.Format(actionString, GetString("WorfklowProperties.Published"));
                                break;

                            case WorkflowStepTypeEnum.DocumentEdit:
                                actionString = GetString("WorfklowProperties.NewVersion");
                                break;

                            default:
                                if (stepType == WorkflowStepTypeEnum.DocumentEdit)
                                {
                                    actionString = GetString("WorfklowProperties.NewVersion");
                                }
                                else
                                {
                                    actionString = isAutomatic ? GetString("WorfklowProperties.Automatic") : GetString("WorfklowProperties.Approved");
                                }
                                break;
                        }

                        return actionString;
                    }
                    // Backward compatibility
                    else
                    {
                        // Return correct step title
                        switch (targetStepName.ToLowerInvariant())
                        {
                            case "archived":
                                return GetString("WorfklowProperties.Archived");

                            case "published":
                                return GetString("WorfklowProperties.Published");

                            case "edit":
                                return GetString("WorfklowProperties.NewVersion");

                            default:
                                if (CMSString.Equals(stepName, "edit", true))
                                {
                                    return GetString("WorfklowProperties.NewVersion");
                                }
                                return GetString("WorfklowProperties.Approved");
                        }
                    }
                }
                else
                {
                    return GetString("WorfklowProperties.Rejected");
                }

            // Get approved time
            case "approvedwhen":
            case "approvedwhentooltip":
                if (string.IsNullOrEmpty(parameter.ToString()))
                {
                    return string.Empty;
                }
                else
                {
                    if (currentUserInfo == null)
                    {
                        currentUserInfo = MembershipContext.AuthenticatedUser;
                    }

                    if (currentSiteInfo == null)
                    {
                        currentSiteInfo = SiteContext.CurrentSite;
                    }

                    if (sourceName.EqualsCSafe("approvedwhen", StringComparison.InvariantCultureIgnoreCase))
                    {
                        DateTime time = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                        return TimeZoneHelper.ConvertToUserTimeZone(time, true, currentUserInfo, currentSiteInfo);
                    }
                    else
                    {
                        return TimeZoneHelper.GetUTCLongStringOffset(currentUserInfo, currentSiteInfo);
                    }
                }

            case "stepname":
                drv = (DataRowView)parameter;
                string step = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "StepDisplayName"), String.Empty);
                string targetStep = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "TargetStepDisplayName"), String.Empty);
                if (!string.IsNullOrEmpty(targetStep))
                {
                    step += " -> " + targetStep;
                }
                return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(step));
        }
        return parameter;
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Reloads the page data.
    /// </summary>
    protected void ReloadData()
    {
        string where = string.Format("StepType IN ({0}, {1}, {2})", (int)WorkflowStepTypeEnum.DocumentEdit, (int)WorkflowStepTypeEnum.DocumentPublished, (int)WorkflowStepTypeEnum.DocumentArchived);
        // Hide custom steps if license doesn't allow them or check automatically publish changes
        if (!WorkflowInfoProvider.IsCustomStepAllowed())
        {
            gridSteps.WhereCondition = SqlHelper.AddWhereCondition(gridSteps.WhereCondition, where);
        }
        // Hide custom steps (without actual step) if functionality 'Automatically publish changes' is allowed
        else if ((WorkflowInfo != null) && WorkflowInfo.WorkflowAutoPublishChanges)
        {
            gridSteps.WhereCondition = SqlHelper.AddWhereCondition(gridSteps.WhereCondition, where);
            // Get current step info
            WorkflowStepInfo currentStep = WorkflowManager.GetStepInfo(Node);

            if (currentStep != null)
            {
                if (!currentStep.StepIsDefault)
                {
                    gridSteps.WhereCondition = SqlHelper.AddWhereCondition(gridSteps.WhereCondition, "(StepName = '" + SqlHelper.EscapeQuotes(currentStep.StepName) + "')", "OR");
                }
            }
        }

        // Do not display steps without order - advanced workflow steps
        gridSteps.WhereCondition = SqlHelper.AddWhereCondition(gridSteps.WhereCondition, "StepOrder IS NOT NULL");


        // Prepare the query parameters
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@DocumentID", 0);

        // Prepare the steps query parameters
        QueryDataParameters stepsParameters = new QueryDataParameters();
        stepsParameters.Add("@StepWorkflowID", 0);

        if (Node != null)
        {
            // Check read permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
            {
                RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName())));
            }

            // Prepare parameters
            parameters.Add("@DocumentID", Node.DocumentID);
            currentStepId = ValidationHelper.GetInteger(Node.GetValue("DocumentWorkflowStepID"), 0);

            if (WorkflowInfo != null)
            {
                plcBasic.Visible = WorkflowInfo.IsBasic;
                plcAdvanced.Visible = !plcBasic.Visible;
                if (plcAdvanced.Visible)
                {
                    ucDesigner.WorkflowID = WorkflowInfo.WorkflowID;
                    ucDesigner.SelectedStepID = currentStepId;
                }
                else
                {
                    stepsParameters.Add("@StepWorkflowID", WorkflowInfo.WorkflowID);
                }

                // Initialize grids
                gridHistory.OnExternalDataBound += gridHistory_OnExternalDataBound;
                gridSteps.OnExternalDataBound += gridSteps_OnExternalDataBound;
                gridHistory.ZeroRowsText = GetString("workflowproperties.nohistoryyet");
            }
        }
        else
        {
            pnlWorkflow.Visible = false;
        }

        // Initialize query parameters of grids
        gridSteps.QueryParameters = stepsParameters;
        gridHistory.QueryParameters = parameters;

        SetupForm();
        gridHistory.ReloadData();
        if (plcBasic.Visible)
        {
            gridSteps.ReloadData();
        }
    }


    private void ShowInfo(string message, bool persistent)
    {
        if (IsLiveSite)
        {
            ShowInformation(message, persistent: persistent);
        }
        else
        {
            DocumentManager.DocumentInfo = message;
        }
    }


    /// <summary>
    /// Reloads the form status.
    /// </summary>
    protected void SetupForm()
    {
        // Check modify permissions
        if ((MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied) &&
            !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Content", "ManageWorkflow"))
        {
            ShowInfo(String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName())), true);
        }
        else
        {
            if ((WorkflowInfo != null) || (currentStepId > 0))
            {
                // Setup the form
                WorkflowStepInfo stepInfo = null;
                if (WorkflowInfo != null)
                {
                    if (Node.IsPublished && (Node.DocumentCheckedOutVersionHistoryID <= 0))
                    {
                        stepInfo = WorkflowStepInfoProvider.GetPublishedStep(WorkflowInfo.WorkflowID);
                    }
                    else
                    {
                        stepInfo = WorkflowStepInfo.Provider.Get(currentStepId) ?? WorkflowStepInfoProvider.GetFirstStep(WorkflowInfo.WorkflowID);
                    }
                }

                if (stepInfo != null)
                {
                    currentStepId = stepInfo.StepID;
                    if (plcAdvanced.Visible)
                    {
                        ucDesigner.SelectedStepID = currentStepId;
                    }
                }
            }
        }

        // If no workflow scope set for node, hide content
        if (WorkflowInfo == null)
        {
            pnlWorkflow.Visible = false;
        }
    }

    #endregion
}
