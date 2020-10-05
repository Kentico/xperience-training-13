using System;
using System.Data;
using System.Globalization;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Scheduler;
using CMS.UIControls;

public partial class CMSModules_Scoring_Controls_UI_Rule_List : CMSAdminListControl
{
    #region "Variables"

    private HeaderAction mButtonRecalculate;
    private String mRecalculationNeededResourceString = "om.score.recalculationrequired2";
    private String mRecalculationNotNeededTooltipResourceString = "om.score.recalculationnotrequired";

    #endregion


    #region "Properties"

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
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// ID of current Score object.
    /// </summary>
    public int ScoreId
    {
        get;
        set;
    }


    /// <summary>
    /// URL of each rule edit action.
    /// </summary>
    public string EditActionUrl
    {
        get
        {
            return gridElem.EditActionUrl;
        }
        set
        {
            gridElem.EditActionUrl = value;
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
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the module name for checking permissions.
    /// </summary>
    public string ModuleNameForPermissionCheck
    {
        get;
        set;
    }

    #endregion


    #region "Overriding UI Texts"

    /// <summary>
    /// This class holds information about text displayed on the page which will be displayed instead the default texts.
    /// </summary>
    public class UITexts
    {
        public string RuleValueCaptionResourceString
        {
            get;
            set;
        }
        public string ZeroRowsTextResourceString
        {
            get;
            set;
        }
        public string RecalculationNeededResourceString
        {
            get;
            set;
        }
        public string RecalculationNotNeededResourceString
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Sets different values for the specified columns.
    /// </summary>
    /// <param name="uiTexts">New columns names</param>
    public void OverrideUITexts(UITexts uiTexts)
    {
        if (uiTexts == null)
        {
            throw new ArgumentNullException("uiTexts");
        }

        colRuleValue.Caption = GetString(uiTexts.RuleValueCaptionResourceString);
        gridElem.ZeroRowsText = GetString(uiTexts.ZeroRowsTextResourceString);
        mRecalculationNeededResourceString = uiTexts.RecalculationNeededResourceString;
        mRecalculationNotNeededTooltipResourceString = uiTexts.RecalculationNotNeededResourceString;
    }


    /// <summary>
    /// Overrieds object types used to get data in unigrid.
    /// </summary>
    /// <param name="objectType">New object type name</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> is null</exception>
    public void OverrideObjectType(string objectType)
    {
        if (objectType == null)
        {
            throw new ArgumentNullException("objectType");
        }

        gridElem.ObjectType = objectType;
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Sets properties of the Recalculate button. Sets whether the button should be enabled or not and its tooltip text.
    /// Must be used on the PreRender phase.
    /// </summary>
    /// <param name="enabled">True if button should be enabled, false otherwise</param>
    /// <param name="tooltip">Text of the tooltip. If null, tooltip won't be changed</param>
    public void SetRecalcuateButtonProperties(bool enabled, string tooltip = null)
    {
        mButtonRecalculate.Enabled = enabled;
        if (tooltip != null)
        {
            mButtonRecalculate.Tooltip = tooltip;
        }
    }

    #endregion


    #region "Event methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterApplicationConstants(Page);

        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.WhereCondition = "RuleScoreID = " + ScoreId;
        InitHeaderActions();
        InitWarnings(ScoreId);

        var deleteAction = gridElem.GridActions.GetActionByName("#delete");
        deleteAction.ModuleName = ModuleNameForPermissionCheck;
        deleteAction.Permissions = PermissionsEnum.Modify.ToString();
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        gridElem.ZeroRowsText = GetString("om.rule.nodatafound");
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "validity":
                DataRowView rowView = (DataRowView)parameter;
                RuleInfo rule = new RuleInfo(rowView.Row);
                return GetValidity(rule);

            case "ruletype":
                string name = "om.score.";
                name += Enum.GetName(typeof (RuleTypeEnum), parameter);
                return GetString(name);
            case "delete":
                if(!CurrentUser.IsAuthorizedPerResource(ModuleNameForPermissionCheck, PermissionsEnum.Modify.ToString()))
                {
                    var imgDel = (CMSGridActionButton)sender;

                    imgDel.Enabled = false;
                    imgDel.Style.Add("cursor", "default");
                    return imgDel;
                }
                break;
        }
        return sender;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets validity string. Either in units of time (xx days) or an expiration date.
    /// </summary>
    private static string GetValidity(RuleInfo rule)
    {
        if (rule.RuleValidUntil != DateTimeHelper.ZERO_TIME)
        {
            return rule.RuleValidUntil.ToShortDateString();
        }
        if (rule.RuleValidity != ValidityEnum.Until)
        {
            return rule.RuleValidFor + " " + Enum.GetName(typeof (ValidityEnum), rule.RuleValidity).ToLowerCSafe();
        }
        return String.Empty;
    }


    /// <summary>
    /// Initiates header actions.
    /// </summary>
    private void InitHeaderActions()
    {
        var recalculationQuery = QueryHelper.BuildQueryWithHash("scoreID", ScoreId.ToString(CultureInfo.InvariantCulture));
        var recalculationURL = ResolveUrl("~/CMSModules/Scoring/Pages/ScheduleRecalculationDialog.aspx") + recalculationQuery;
        var score = ScoreInfo.Provider.Get(ScoreId);

        mButtonRecalculate = mButtonRecalculate ?? new HeaderAction
        {
            Text = GetString("om.score.recalculate"),
            OnClientClick = "modalDialog('" + recalculationURL + @"', '', 660, 320);",
            Enabled = false,
            ResourceName = score.TypeInfo.ModuleName,
            Permission = "modify"
        };
        HeaderActions.AddAction(mButtonRecalculate);
    }


    /// <summary>
    /// Gets formatted score status. Score can be disabled, it can be scheduled to rebuild in the future or its status is one of <see cref="ScoreStatusEnum"/>.
    /// </summary>
    private void InitWarnings(int scoreID)
    {
        var info = ScoreInfo.Provider.Get(scoreID);
        if (info == null)
        {
            return;
        }

        // "Recalculation scheduled" status
        if (info.ScoreScheduledTaskID > 0)
        {
            TaskInfo taskInfo = TaskInfo.Provider.Get(info.ScoreScheduledTaskID);
            if (taskInfo != null && taskInfo.TaskEnabled)
            {
                ShowInformation(String.Format(GetString("om.score.recalculatescheduledat"), taskInfo.TaskNextRunTime.ToString()));
                mButtonRecalculate.Enabled = true;
                return;
            }
        }

        // Other statuses
        switch (info.ScoreStatus)
        {
            case ScoreStatusEnum.Ready:
                // Score should be up to date, no need to inform user about anything
                if (info.ScoreEnabled)
                {
                    mButtonRecalculate.Tooltip = GetString(mRecalculationNotNeededTooltipResourceString);
                }
                else
                {
                    mButtonRecalculate.Enabled = true;
                }
                break;
            case ScoreStatusEnum.RecalculationRequired:
                ShowInformation(GetString(mRecalculationNeededResourceString));
                mButtonRecalculate.Enabled = true;
                break;
            case ScoreStatusEnum.Recalculating:
                ShowInformation(GetString("om.score.recalculationstarted2"));
                gridElem.GridView.Enabled = false;
                break;
            case ScoreStatusEnum.RecalculationFailed:
                ShowInformation(GetString("om.score.recalcfailed"));
                mButtonRecalculate.Enabled = true;
                break;
            default:
            case ScoreStatusEnum.Unspecified:
                throw new Exception("[RuleList.InitInformation]: Score status not specified.");
        }

        if (!CurrentUser.IsAuthorizedPerResource(ModuleNameForPermissionCheck, PermissionsEnum.Modify.ToString()))
        {
            mButtonRecalculate.Enabled = false;
            mButtonRecalculate.Tooltip = GetString("general.modifynotallowed");
        }
    }

    #endregion
}
