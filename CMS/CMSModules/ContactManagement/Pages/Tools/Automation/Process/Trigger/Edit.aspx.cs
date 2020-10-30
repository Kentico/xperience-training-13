using System;
using System.Web.UI.WebControls;

using CMS.Activities;
using CMS.Automation;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Scheduler;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Web.UI;

[EditedObject(ObjectWorkflowTriggerInfo.OBJECT_TYPE, "objectworkflowtriggerId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Trigger_Edit : CMSAutomationPage
{
    #region "Variables"

    private WorkflowInfo mProcess;

    #endregion


    #region "Properties"

    /// <summary>
    /// A trigger to be edited.
    /// </summary>
    public ObjectWorkflowTriggerInfo Trigger => (ObjectWorkflowTriggerInfo)editForm.EditedObject;


    /// <summary>
    /// Current process.
    /// </summary>
    public WorkflowInfo Process => mProcess ?? (mProcess = WorkflowInfo.Provider.Get(QueryHelper.GetInteger("processid", 0)));


    /// <summary>
    /// Selected object type value derived from drop-down list of trigger types.
    /// </summary>
    protected string SelectedObjectType => ddlType.SelectedValue.Split('|')[1];


    /// <summary>
    /// Indicates whether the current setting might cause e-mail floods.
    /// </summary>
    protected bool IsDangerousRecurrence
    {
        get
        {
            if (Process.WorkflowRecurrenceType != ProcessRecurrenceTypeEnum.Recurring)
            {
                return false;
            }

            // Contact changed and activity created are dangerous triggers. Score changed trigger fires only once, when contact exceeds given score.
            bool contactChangedTrigger = Trigger.TriggerObjectType == ContactInfo.OBJECT_TYPE && Trigger.TriggerType == WorkflowTriggerTypeEnum.Change;
            bool activityTrigger = Trigger.TriggerObjectType == ActivityInfo.OBJECT_TYPE;
            return contactChangedTrigger || activityTrigger;
        }
    }


    /// <summary>
    /// Selected type value derived from drop-down list of trigger types.
    /// </summary>
    protected WorkflowTriggerTypeEnum SelectedTriggerType
    {
        get
        {
            int value = ValidationHelper.GetInteger(ddlType.SelectedValue.Split('|')[0], 0);
            return (WorkflowTriggerTypeEnum)Enum.Parse(typeof(WorkflowTriggerTypeEnum), value.ToString());
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CurrentMaster.PanelContent.CssClass += " automation-sidepanel-content";

        if (Process == null)
        {
            RedirectToInformation("editedobject.notexists");
        }

        if (Trigger.TriggerID != 0)
        {
            editForm.OnAfterSave += EditForm_OnAfterSave;
        }

        var triggersListingUrl = UIContextHelper.GetElementUrl(ModuleName.ONLINEMARKETING, "EditProcessTriggers", false);
        var triggersListingQuery = URLHelper.RemoveUrlParameter(RequestContext.CurrentQueryString, "objectworkflowtriggerId");
        triggersListingUrl = URLHelper.AppendQuery(triggersListingUrl, triggersListingQuery);

        // Add necessary parameters to the redirect URL
        editForm.RedirectUrlAfterCreate = URLHelper.AddParameterToUrl(triggersListingUrl, "saved", "1");

        // Add "Back to listing" button
        AddHeaderAction(new HeaderAction
        {
            Text = GetString("general.back"),
            IconCssClass = "icon-arrow-crooked-left",
            RedirectUrl = triggersListingUrl,
            ButtonStyle = ButtonStyle.Default
        });

        // Activity selector settings
        ucActivityType.UniSelector.ReturnColumnName = "ActivityTypeID";
        ucActivityType.ShowAll = false;
        ucActivityType.UniSelector.SpecialFields.Add(new SpecialField
        {
            Text = GetString("general.selectany"),
            Value = SpecialFieldValue.NONE.ToString()
        });

        // IScore type selector settings
        ucScoreType.SetValue("AllowAll", false);

        // Set condition builder context resolver name
        SetResolverName();

        // ScheduleInterval selector settings
        InitScheduleIntervalSelector();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (IsDangerousRecurrence && string.IsNullOrEmpty(MessagesPlaceHolder.ErrorText))
        {
            ShowWarning(GetString("ma.dangerousRecurrence.message"), GetString("ma.dangerousRecurrence.description"), null);
        }
        if (!RequestHelper.IsPostBack() && Trigger.TriggerID != 0)
        {
            LoadCustomFormValues();
        }

        VisibilitySettings();
        InitConditionBuilder();
        CheckMacroCondition();
    }


    /// <summary>
    /// After data load event handler.
    /// </summary>
    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Initialize new trigger
        if (Trigger.TriggerID == 0)
        {
            Trigger.TriggerWorkflowID = Process.WorkflowID;
        }

        FillDdlType();
    }


    /// <summary>
    /// Before save event handler.
    /// </summary>
    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Save custom values
        Trigger.TriggerType = SelectedTriggerType;
        Trigger.TriggerObjectType = SelectedObjectType;
        if (SelectedObjectType == ActivityInfo.OBJECT_TYPE)
        {
            Trigger.TriggerTargetObjectType = ActivityTypeInfo.OBJECT_TYPE;
            Trigger.TriggerTargetObjectID = ValidationHelper.GetInteger(ucActivityType.Value, 0);
            Trigger.TriggerParameters = null;
        }
        else if (SelectedObjectType == ScoreInfo.OBJECT_TYPE)
        {
            Trigger.TriggerTargetObjectType = ScoreInfo.OBJECT_TYPE;
            Trigger.TriggerTargetObjectID = GetValidScoreType();
            Trigger.TriggerParameters = GetValidScoreParameters();
        }
        else if (SelectedObjectType == TaskInfo.OBJECT_TYPE_OBJECTTASK)
        {
            Trigger.TriggerObjectType = TaskInfo.OBJECT_TYPE_OBJECTTASK;
            Trigger.TriggerParameters = GetValidScheduleInterval();
        }
    }


    protected void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        var workflowStepId = QueryHelper.GetInteger("workflowstepid", 0);
        var graphName = QueryHelper.GetString("graph", String.Empty);

        WorkflowScriptHelper.RefreshDesignerFromDialog(Page, workflowStepId, graphName);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Sets context resolver name for a selected object type.
    /// </summary>
    private void SetResolverName()
    {
        switch (SelectedObjectType)
        {
            case ActivityInfo.OBJECT_TYPE:
                tMacroCondition.EditingControl.ResolverName = "contactactivityresolver";
                break;

            case ScoreInfo.OBJECT_TYPE:
                tMacroCondition.EditingControl.ResolverName = "contactscoreresolver";
                break;

            default:
                tMacroCondition.EditingControl.ResolverName = "contactresolver";
                break;
        }
    }


    /// <summary>
    /// Initializes condition builder field and editor.
    /// </summary>
    private void InitConditionBuilder()
    {
        // Hide context macros as they are not working in trigger conditions
        tMacroCondition.EditingControl.SetValue("DisplayRuleType", 1);

        // Change label and display info message to select a subset of contacts when configuring a time-based trigger
        if (SelectedObjectType == TaskInfo.OBJECT_TYPE_OBJECTTASK)
        {
            tMacroCondition.ResourceString = "ma.trigger.timebased.macrocondition";
            tMacroCondition.EditingControl.SetValue("ShowGlobalRules", false);
            tMacroCondition.FormFieldInfo.SetPropertyValue(FormFieldPropertyEnum.ExplanationText, GetString("ma.allcontactsselected.warning"));
        }
    }


    /// <summary>
    /// Validates score and returns selected type of score.
    /// </summary>
    private int GetValidScoreType()
    {
        int score = ValidationHelper.GetInteger(ucScoreType.Value, 0);
        if (score == 0 && !editForm.StopProcessing)
        {
            ShowError(GetString("ma.trigger.noScore"));
            editForm.StopProcessing = true;
        }
        return score;
    }


    /// <summary>
    /// Validates score parameters and returns their string representation.
    /// </summary>
    private ObjectParameters GetValidScoreParameters()
    {
        if (!ucScoreProperties.HasValue && !editForm.StopProcessing)
        {
            ShowError(GetString("ma.trigger.invalidScoreParam"));
            editForm.StopProcessing = true;
        }
        return ucScoreProperties.Value as ObjectParameters;
    }


    /// <summary>
    /// Initializes schedule interval selector.
    /// </summary>
    private void InitScheduleIntervalSelector()
    {
        ucScheduleInterval.DefaultPeriod = SchedulingHelper.PERIOD_ONCE;
        ucScheduleInterval.DisplaySecond = false;
        ucScheduleInterval.DisplayMinute = false;
        ucScheduleInterval.DisplayHour = false;

        ucScheduleInterval.ResourcePrefix = "ma";

        ucScheduleInterval.ScheduleInterval = Trigger.EncodedInterval;
    }


    /// <summary>
    /// Validates and returns selected schedule interval.
    /// </summary>
    private ObjectParameters GetValidScheduleInterval()
    {
        if (!editForm.StopProcessing)
        {
            if (ucScheduleInterval.ScheduleInterval == String.Empty)
            {
                ShowError(GetString("ma.trigger.invalidinterval"));
                editForm.StopProcessing = true;
            }

            if (!DataTypeManager.IsValidDate(ucScheduleInterval.StartTime.SelectedDateTime))
            {
                ShowError($"{GetString("BasicForm.ErrorInvalidDateTime")} {DateTime.Now}.");
                editForm.StopProcessing = true;
            }

            if (!ucScheduleInterval.CheckOneDayMinimum())
            {
                ShowError(GetString("ma.trigger.nodayselected"));
                editForm.StopProcessing = true;
            }
        }

        var triggerParameters = new ObjectParameters();
        triggerParameters.SetValue(TriggerDataConstants.TRIGGER_DATA_INTERVAL, ucScheduleInterval.ScheduleInterval);

        return triggerParameters;
    }


    /// <summary>
    /// Loads custom form values to form.
    /// </summary>
    private void LoadCustomFormValues()
    {
        if (Trigger.TriggerObjectType == ActivityInfo.OBJECT_TYPE)
        {
            ucActivityType.Value = Trigger.TriggerTargetObjectID;
        }
        else if (Trigger.TriggerObjectType == ScoreInfo.OBJECT_TYPE)
        {
            ucScoreType.Value = Trigger.TriggerTargetObjectID;
            ucScoreProperties.Value = Trigger.TriggerParameters;
        }
        ddlType.SelectedValue = GetDdlTypeItem(Trigger.TriggerType, Trigger.TriggerObjectType).Value;
    }


    /// <summary>
    /// Sets visibility of components.
    /// </summary>
    private void VisibilitySettings()
    {
        plcActivityType.Visible = SelectedObjectType == ActivityInfo.OBJECT_TYPE;
        plcScoreCondition.Visible = SelectedObjectType == ScoreInfo.OBJECT_TYPE;
        plcScheduleInterval.Visible = SelectedObjectType == TaskInfo.OBJECT_TYPE_OBJECTTASK;

        ucScoreProperties.Visible = ucScoreType.HasData;
    }


    /// <summary>
    /// Fills drop-down list with items.
    /// </summary>
    private void FillDdlType()
    {
        AddDdlTypeListItem(WorkflowTriggerTypeEnum.Creation, ContactInfo.OBJECT_TYPE);
        AddDdlTypeListItem(WorkflowTriggerTypeEnum.Change, ContactInfo.OBJECT_TYPE);

        AddDdlTypeListItem(WorkflowTriggerTypeEnum.Creation, ActivityInfo.OBJECT_TYPE, GetString("ma.trigger.performed"));
        AddDdlTypeListItem(WorkflowTriggerTypeEnum.Change, ScoreInfo.OBJECT_TYPE, GetString("ma.trigger.scorereached.general"));

        AddDdlTypeListItem(WorkflowTriggerTypeEnum.TimeBased, TaskInfo.OBJECT_TYPE_OBJECTTASK, GetString("ma.trigger.timebased.general"));
    }


    /// <summary>
    /// Returns item from drop-down list of trigger types.
    /// </summary>
    /// <param name="type">Type of trigger</param>
    /// <param name="objectType">Object trigger</param>
    private ListItem GetDdlTypeItem(WorkflowTriggerTypeEnum type, string objectType)
    {
        string formatedValue = GetDdlTypeFormatedValue(type, objectType);
        return ddlType.Items.FindByValue(formatedValue);
    }


    /// <summary>
    /// Returns value in same format as in drop-down list of trigger types.
    /// </summary>
    /// <param name="type">Type of trigger</param>
    /// <param name="objectType">Object type</param>
    private string GetDdlTypeFormatedValue(WorkflowTriggerTypeEnum type, string objectType)
    {
        return String.Format("{0}|{1}", ((int)type).ToString(), objectType);
    }


    /// <summary>
    /// Adds item to drop-down list of trigger types.
    /// </summary>
    /// <param name="type">Type of trigger</param>
    /// <param name="objectType">Object type</param>
    private void AddDdlTypeListItem(WorkflowTriggerTypeEnum type, string objectType)
    {
        string text = AutomationHelper.GetTriggerName(type, objectType);
        string value = GetDdlTypeFormatedValue(type, objectType);
        ddlType.Items.Add(new ListItem(text, value));
    }


    /// <summary>
    /// Adds item to drop-down list of trigger types.
    /// </summary>
    /// <param name="type">Type of trigger</param>
    /// <param name="objectType">Object type</param>
    /// <param name="text">Text of the item</param>
    private void AddDdlTypeListItem(WorkflowTriggerTypeEnum type, string objectType, string text)
    {
        string value = GetDdlTypeFormatedValue(type, objectType);
        ddlType.Items.Add(new ListItem(text, value));
    }


    /// <summary>
    /// Check whether time-based trigger's macro condition is optimized.
    /// Show warning if not.
    /// </summary>
    private void CheckMacroCondition()
    {
        if (Trigger.TriggerID == 0 || Trigger.TriggerType != WorkflowTriggerTypeEnum.TimeBased)
        {
            return;
        }

        if (!String.IsNullOrEmpty(Trigger.TriggerMacroCondition))
        {
            try
            {
                var macroTree = CachedMacroRuleTrees.GetParsedTree(Trigger.TriggerMacroCondition);
                if ((macroTree == null) || !MacroRuleTreeAnalyzer.CanTreeBeTranslated(macroTree))
                {
                    var text = String.Format(ResHelper.GetString("om.macros.macro.slow"), DocumentationHelper.GetDocumentationTopicUrl("om_macro_performance"));
                    ShowWarning(text);
                }
            }
            catch
            {
                // in case that macro rule can't be parsed (deleted or changed, etc.)
            }
        }
    }

    #endregion
}
