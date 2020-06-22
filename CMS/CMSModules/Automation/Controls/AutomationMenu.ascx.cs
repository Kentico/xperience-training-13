using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

using CMS.Automation;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Definitions;

public partial class CMSModules_Automation_Controls_AutomationMenu : BaseEditMenu
{
    #region "Variables"

    // Actions
    protected NextStepAction next;
    protected PreviousStepAction previous;
    protected HeaderAction delete;
    protected HeaderAction start;
    protected NextStepAction specific;
    protected HeaderAction history;

    private WorkflowStepInfo mStep;
    private CMSAutomationManager mAutomationManager;

    private bool? mAllowSave;
    private string originalClientId;

    private readonly string COMMAND_SHOW_HISTORY = "AutomationHistory";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Menu control
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return menu;
        }
    }


    /// <summary>
    /// Indicates if Save action is allowed.
    /// </summary>
    public override bool AllowSave
    {
        get
        {
            if (mAllowSave == null)
            {
                return AutomationManager.AllowSave;
            }

            return mAllowSave.Value;
        }
        set
        {
            mAllowSave = value;
        }
    }


    /// <summary>
    /// Indicates if the menu is enabled
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return menu.Enabled;
        }
        set
        {
            menu.Enabled = value;
        }
    }


    /// <summary>
    /// Object instance
    /// </summary>
    public BaseInfo InfoObject
    {
        get
        {
            return AutomationManager.InfoObject;
        }
    }


    /// <summary>
    /// State object
    /// </summary>
    public AutomationStateInfo StateObject
    {
        get
        {
            return AutomationManager.StateObject;
        }
    }


    /// <summary>
    /// Automation manager
    /// </summary>
    public AutomationManager Manager
    {
        get
        {
            return AutomationManager.Manager;
        }
    }


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


    /// <summary>
    /// If true, the access permissions to the items are checked.
    /// </summary>
    public override bool CheckPermissions
    {
        get
        {
            return AutomationManager.CheckPermissions;
        }
        set
        {
            AutomationManager.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return AutomationManager.StopProcessing;
        }
        set
        {
            AutomationManager.StopProcessing = value;
        }
    }


    /// <summary>
    /// Current step
    /// </summary>
    protected WorkflowStepInfo Step
    {
        get
        {
            return mStep ?? (mStep = AutomationManager.Step);
        }
    }

    #endregion


    #region "Constructors"

    /// <summary>
    /// Constructor
    /// </summary>
    public CMSModules_Automation_Controls_AutomationMenu()
    {
        RefreshInterval = 500;
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        // Store client ID before replaced by short ID (original ID is needed when sending AJAX request to identify the control)
        originalClientId = ClientID;

        var parameters = Request.Form["params"];
        if (parameters != null)
        {
            if (parameters.StartsWith(CALLBACK_ID + originalClientId, StringComparison.Ordinal))
            {
                string[] args = parameters.Split(new[] { CALLBACK_SEP }, StringSplitOptions.None);
                AutomationManager.Mode = FormModeEnum.Update;
                AutomationManager.ClearObject();
                AutomationManager.ObjectType = ValidationHelper.GetString(args[1], null);
                AutomationManager.ObjectID = ValidationHelper.GetInteger(args[2], 0);
                Response.Clear();
                if (Step != null)
                {
                    Response.Write(CALLBACK_ID + AutomationManager.GetAutomationInfo() + CALLBACK_SEP + AutomationManager.RefreshActionContent.ToString().ToLowerInvariant() + CALLBACK_SEP + Step.StepID);
                }
                else
                {
                    Response.Write(CALLBACK_ID + AutomationManager.GetAutomationInfo() + CALLBACK_SEP + "false" + CALLBACK_SEP + "0");
                }
                Response.End();
                return;
            }
        }

        base.OnInit(e);

        // Perform full post-back if not in update panel
        menu.PerformFullPostBack = !ControlsHelper.IsInUpdatePanel(this);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadMenu();

        // Display information
        AutomationManager.ShowAutomationInfo();

        // Hide menu if no actions
        plcMenu.Visible = menu.HasAnyVisibleAction() || !string.IsNullOrEmpty(pnlDoc.Label.Text) || lblInfo.Visible;

        if (plcMenu.Visible)
        {
            RegisterActionScripts();
        }
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);

        PostBackOptions opt = new PostBackOptions(this, null)
        {
            PerformValidation = false
        };

        Page.ClientScript.RegisterForEventValidation(opt);
    }


    /// <summary>
    /// Registers action scripts
    /// </summary>
    private void RegisterActionScripts()
    {
        StringBuilder sb = new StringBuilder();

        bool addComment = false;
        sb.Append("function CheckConsistency_", ClientID, "() { ", AutomationManager.GetJSFunction("CONS", null, null), "; } \n");
        if (next != null) { addComment = true; sb.Append("function MoveNext_", ClientID, "(stepId, comment) { ", next.OnClientClick, AutomationManager.GetJSFunction(next.EventName, "stepId", "comment"), "; } \n"); }
        if (specific != null) { addComment = true; sb.Append("function MoveSpecific_", ClientID, "(stepId, comment) { ", specific.OnClientClick, AutomationManager.GetJSFunction(specific.EventName, "stepId", "comment"), "; } \n"); }
        if (previous != null) { addComment = true; sb.Append("function MovePrevious_", ClientID, "(historyId, comment) { ", previous.OnClientClick, AutomationManager.GetJSFunction(previous.EventName, "historyId", "comment"), "; } \n"); }
        if (addComment) { sb.Append("function AddComment_", ClientID, "(name, stateId, menuId) { ", AutomationManager.GetJSFunction(ComponentEvents.COMMENT, "name|stateId|menuId", null), "; } \n"); }

        // Register the script
        if (sb.Length > 0)
        {
            ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "AutoMenuActions" + ClientID, ScriptHelper.GetScript(sb.ToString()));
        }
    }

    #endregion


    #region "Methods"

    private void ClearProperties()
    {
        // Clear actions
        next = null;
        specific = null;
        previous = null;
        delete = null;
        start = null;
        history = null;

        mStep = null;

        // Clear security result
        AutomationManager.ClearProperties();
    }


    private void ReloadMenu()
    {
        if (StopProcessing)
        {
            return;
        }

        // Handle several reloads
        ClearProperties();

        if (!HideStandardButtons)
        {
            // If content should be refreshed
            if (AutomationManager.RefreshActionContent)
            {
                // Display action message
                WorkflowActionInfo action = WorkflowActionInfo.Provider.Get(Step.StepActionID);
                string name = (action != null) ? action.ActionDisplayName : Step.StepDisplayName;
                string str = (action != null) ? "workflow.actioninprogress" : "workflow.stepinprogress";
                string text = string.Format(GetString(str), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(name)));
                text = ScriptHelper.GetLoaderInlineHtml(text);

                InformationText = text;
                EnsureRefreshScript();
            }

            // Object update
            if (AutomationManager.Mode == FormModeEnum.Update && InfoObject != null)
            {
                // Get current process
                WorkflowInfo process = AutomationManager.Process;
                string objectName = HTMLHelper.HTMLEncode(InfoObject.TypeInfo.GetNiceObjectTypeName().ToLowerInvariant());

                // Next step action
                if (AutomationManager.IsActionAllowed(ComponentEvents.AUTOMATION_MOVE_NEXT))
                {
                    next = new NextStepAction(Page)
                    {
                        Tooltip = string.Format(GetString("EditMenu.NextStep"), objectName),
                        OnClientClick = RaiseGetClientValidationScript(ComponentEvents.AUTOMATION_MOVE_NEXT, null),
                    };
                }

                // Move to specific step action
                if (AutomationManager.IsActionAllowed(ComponentEvents.AUTOMATION_MOVE_SPEC))
                {
                    var steps = WorkflowStepInfo.Provider.Get()
                        .Where("StepWorkflowID = " + process.WorkflowID + " AND StepType NOT IN (" + (int)WorkflowStepTypeEnum.Start + "," + (int)WorkflowStepTypeEnum.Note + ")")
                        .OrderBy("StepDisplayName");

                    specific = new NextStepAction(Page)
                    {
                        Text = GetString("AutoMenu.SpecificStepIcon"),
                        Tooltip = string.Format(GetString("AutoMenu.SpecificStepMultiple"), objectName),
                        CommandName = ComponentEvents.AUTOMATION_MOVE_SPEC,
                        EventName = ComponentEvents.AUTOMATION_MOVE_SPEC,
                        CssClass = "scrollable-menu",

                        // Make action inactive
                        OnClientClick = null,
                        Inactive = true
                    };

                    foreach (var s in steps)
                    {
                        string stepName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(s.StepDisplayName));
                        NextStepAction spc = new NextStepAction(Page)
                        {
                            Text = string.Format(GetString("AutoMenu.SpecificStepTo"), stepName),
                            Tooltip = string.Format(GetString("AutoMenu.SpecificStep"), objectName),
                            CommandName = ComponentEvents.AUTOMATION_MOVE_SPEC,
                            EventName = ComponentEvents.AUTOMATION_MOVE_SPEC,
                            CommandArgument = s.StepID.ToString(),
                            OnClientClick = RaiseGetClientValidationScript(ComponentEvents.AUTOMATION_MOVE_SPEC, "if(!confirm(" + ScriptHelper.GetString(string.Format(GetString("autoMenu.MoveSpecificConfirmation"), objectName, ResHelper.LocalizeString(s.StepDisplayName))) + ")) { return false; }"),
                        };

                        // Process action appearance
                        ProcessAction(spc, Step, s);

                        // Add step
                        specific.AlternativeActions.Add(spc);
                    }

                    // Add comment
                    AddCommentAction(ComponentEvents.AUTOMATION_MOVE_SPEC, specific, objectName);
                }

                // Previous step action
                if (AutomationManager.IsActionAllowed(ComponentEvents.AUTOMATION_MOVE_PREVIOUS))
                {
                    var prevSteps = Manager.GetPreviousSteps(InfoObject, StateObject);
                    int prevStepsCount = prevSteps.Count;

                    if (prevStepsCount > 0)
                    {
                        previous = new PreviousStepAction(Page)
                        {
                            Tooltip = string.Format(GetString("EditMenu.PreviousStep"), objectName),
                            OnClientClick = RaiseGetClientValidationScript(ComponentEvents.AUTOMATION_MOVE_PREVIOUS, null)
                        };

                        // For managers allow move to specified step
                        if (WorkflowStepInfoProvider.CanUserManageAutomationProcesses(MembershipContext.AuthenticatedUser, InfoObject.Generalized.ObjectSiteName))
                        {
                            if (prevStepsCount > 1)
                            {
                                foreach (var s in prevSteps)
                                {
                                    previous.AlternativeActions.Add(new PreviousStepAction(Page)
                                    {
                                        Text = string.Format(GetString("EditMenu.PreviousStepTo"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(s.StepDisplayName))),
                                        Tooltip = string.Format(GetString("EditMenu.PreviousStep"), objectName),
                                        OnClientClick = RaiseGetClientValidationScript(ComponentEvents.AUTOMATION_MOVE_PREVIOUS, null),
                                        CommandArgument = s.RelatedHistoryID.ToString()
                                    });
                                }
                            }
                        }

                        // Add comment
                        AddCommentAction(ComponentEvents.AUTOMATION_MOVE_PREVIOUS, previous, objectName);
                    }
                }

                if (AutomationManager.IsActionAllowed(ComponentEvents.AUTOMATION_REMOVE))
                {
                    delete = new HeaderAction
                    {
                        CommandName = ComponentEvents.AUTOMATION_REMOVE,
                        EventName = ComponentEvents.AUTOMATION_REMOVE,
                        Text = GetString("autoMenu.RemoveState"),
                        Tooltip = string.Format(GetString("autoMenu.RemoveStateDesc"), objectName),
                        OnClientClick = RaiseGetClientValidationScript(ComponentEvents.AUTOMATION_REMOVE, "if(!confirm(" + ScriptHelper.GetString(string.Format(GetString("autoMenu.RemoveStateConfirmation"), objectName)) + ")) { return false; }"),
                        ButtonStyle = ButtonStyle.Default
                    };
                }

                // Handle multiple next steps
                if (next != null)
                {
                    // Get next step info
                    List<WorkflowStepInfo> steps = AutomationManager.NextSteps;
                    int stepsCount = steps.Count;
                    if (stepsCount > 0)
                    {
                        var nextS = steps[0];

                        // Only one next step
                        if (stepsCount == 1)
                        {
                            if (nextS.StepIsFinished)
                            {
                                next.Text = GetString("EditMenu.IconFinish");
                                next.Tooltip = string.Format(GetString("EditMenu.Finish"), objectName);
                            }

                            // Process action appearance
                            ProcessAction(next, Step, nextS);
                        }
                        // Multiple next steps
                        else
                        {
                            // Check if not all steps finish steps
                            if (steps.Exists(s => !s.StepIsFinished))
                            {
                                next.Tooltip = string.Format(GetString("EditMenu.NextStepMultiple"), objectName);
                            }
                            else
                            {
                                next.Text = GetString("EditMenu.IconFinish");
                                next.Tooltip = string.Format(GetString("EditMenu.NextStepMultiple"), objectName);
                            }

                            // Make action inactive
                            next.OnClientClick = null;
                            next.Inactive = true;

                            // Process action appearance
                            ProcessAction(next, Step, null);

                            string itemText = "EditMenu.NextStepTo";
                            string itemDesc = "EditMenu.NextStep";

                            foreach (var s in steps)
                            {
                                NextStepAction nxt = new NextStepAction(Page)
                                {
                                    Text = string.Format(GetString(itemText), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(s.StepDisplayName))),
                                    Tooltip = string.Format(GetString(itemDesc), objectName),
                                    OnClientClick = RaiseGetClientValidationScript(ComponentEvents.AUTOMATION_MOVE_NEXT, null),
                                    CommandArgument = s.StepID.ToString()
                                };

                                if (s.StepIsFinished)
                                {
                                    nxt.Text = string.Format(GetString("EditMenu.FinishTo"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(s.StepDisplayName)));
                                    nxt.Tooltip = string.Format(GetString("EditMenu.Finish"), objectName);
                                }

                                // Process action appearance
                                ProcessAction(nxt, Step, s);

                                // Add step
                                next.AlternativeActions.Add(nxt);
                            }
                        }

                        // Add comment
                        AddCommentAction(ComponentEvents.AUTOMATION_MOVE_NEXT, next, objectName);
                    }
                    else
                    {
                        bool displayAction = false;
                        if (!Step.StepAllowBranch)
                        {
                            // Transition exists, but condition doesn't match
                            var transitions = Manager.GetStepTransitions(Step);
                            if (transitions.Count > 0)
                            {
                                WorkflowStepInfo s = WorkflowStepInfo.Provider.Get(transitions[0].TransitionEndStepID);

                                // Finish text
                                if (s.StepIsFinished)
                                {
                                    next.Text = GetString("EditMenu.IconFinish");
                                    next.Tooltip = string.Format(GetString("EditMenu.Finish"), objectName);
                                }

                                // Inform user
                                displayAction = true;
                                next.Enabled = false;

                                // Process action appearance
                                ProcessAction(next, Step, null);
                            }
                        }

                        if (!displayAction)
                        {
                            // There is not next step
                            next = null;
                        }
                    }
                }

                // Handle start button
                if (AutomationManager.IsActionAllowed(ComponentEvents.AUTOMATION_START) && (process.WorkflowRecurrenceType != ProcessRecurrenceTypeEnum.NonRecurring))
                {
                    start = new HeaderAction
                    {
                        CommandName = ComponentEvents.AUTOMATION_START,
                        EventName = ComponentEvents.AUTOMATION_START,
                        Text = GetString("autoMenu.StartState"),
                        Tooltip = process.WorkflowEnabled ? GetString("autoMenu.StartStateDesc") : GetString("autoMenu.DisabledStateDesc"),
                        CommandArgument = process.WorkflowID.ToString(),
                        Enabled = process.WorkflowEnabled,
                        OnClientClick = RaiseGetClientValidationScript(ComponentEvents.AUTOMATION_START, "if(!confirm(" + ScriptHelper.GetString(string.Format(GetString("autoMenu.startSameProcessConfirmation"), objectName)) + ")) { return false; }"),
                        ButtonStyle = ButtonStyle.Default
                    };
                }

                // History button
                history = new HeaderAction
                {
                    CommandName = COMMAND_SHOW_HISTORY,
                    Text = GetString("ma.history.show"),
                    OnClientClick = $"modalDialog('{GetProcessHistoryDialogUrl()}', 'AutomationHistory', '70%', '70%'); return false;",
                    ButtonStyle = ButtonStyle.Default
                };
            }
        }

        // Add actions in correct order
        AddAction(previous);
        AddAction(next);
        AddAction(specific);
        AddAction(delete);
        AddAction(start);
        AddAction(history);

        // Set the information text
        if (!String.IsNullOrEmpty(InformationText))
        {
            lblInfo.Text = InformationText;
            lblInfo.CssClass = "LeftAlign EditMenuInfo";
            lblInfo.Visible = true;
        }

        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterModule(Page, "CMS/ScrollPane", new { selector = ".scrollable-menu ul" });

        CssRegistration.RegisterCssLink(Page, "~/CMSScripts/jquery/jquery-jscrollpane.css");
    }


    private void EnsureRefreshScript()
    {
        PostBackOptions options = new PostBackOptions(this)
        {
            PerformValidation = false
        };

        string postback = ControlsHelper.GetPostBackEventReference(menu, options, false, true);
        string externalRefreshScript = null;
        if (!string.IsNullOrEmpty(OnClientStepChanged))
        {
            externalRefreshScript = string.Format("clearInterval(refTimerId_{0}); {1};", ClientID, OnClientStepChanged);
        }

        const string commonScript = @"
        String.prototype.startsWith = function (str) { return (this.match('^' + str) == str); };
        
        function VerifyData(incomingData) {
            if (incomingData.startsWith('" + CALLBACK_ID + @"')) {
                return incomingData.replace('" + CALLBACK_ID + @"', '');
            }
            else {
                data = null;
            }

            return data;
        }
        ";
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "refCommon", commonScript, true);

        var refScript = $@"
        var refTimerId_{ClientID} = 0;

        function RfMenu_DoPostBack_{ClientID}() {{{postback}}}

        function RfMenu_Succ_{ClientID}(data, textStatus, jqXHR) {{
            if(data != null) {{
                var hdn = document.getElementById('{hdnParam.ClientID}');
                var args = data.split('{CALLBACK_SEP}');
                var stop = (args[1] == 'false');
                var stepId = args[2];

                if(stop) {{
                    clearInterval(refTimerId_{ClientID});
                    setTimeout('RfMenu_DoPostBack_{ClientID}()', {RefreshInterval});
                }}
                else {{
                    // Step changed
                    if(hdn.value != stepId) {{
                        var lbl = document.getElementById('{AutomationManager.AutomationInfoLabel.ClientID}');
                        if(lbl != null) {{
                            lbl.innerHTML = args[0];
                        }}
                    {externalRefreshScript}
                    }}
                }}
                hdn.value = stepId;
            }}
            else {{
                clearInterval(refTimerId_{ClientID});
                setTimeout('RfMenu_DoPostBack_{ClientID}()', {RefreshInterval});
            }}
        }}

        function RfMenu_Err_{ClientID}(jqXHR, textStatus, errorThrown) {{
            var err = '';
            if ((errorThrown != undefined) && (errorThrown != null) && (errorThrown != '')) {{
                err = ' (' + errorThrown + ')';
                clearInterval(refTimerId_{ClientID});
                alert(err);
            }}
        }}

        function RfMenu_{ClientID}() {{
            $cmsj.ajax({{
                cache: false,
                type: 'POST',
                data: 'params={CALLBACK_ID + originalClientId + CALLBACK_SEP + AutomationManager.ObjectType + CALLBACK_SEP + AutomationManager.ObjectID + CALLBACK_SEP}',
                context: document.body,
                success: RfMenu_Succ_{ClientID},
                error: RfMenu_Err_{ClientID},
                dataType: 'text',
                dataFilter: VerifyData
            }});
        }}

        refTimerId_{ClientID} = setInterval('RfMenu_{ClientID}()', 200);";

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ref_" + ClientID, refScript, true);
    }


    private string GetProcessHistoryDialogUrl()
    {
        return ApplicationUrlHelper.ResolveDialogUrl($"~/CMSModules/ContactManagement/Pages/Tools/Automation/Process/Process_History.aspx?stateid={StateObject.StateID}");
    }


    private void ProcessAction(HeaderAction action, WorkflowStepInfo currentStep, WorkflowStepInfo nextStep)
    {
        if (action == null)
        {
            return;
        }

        string nextStepName = null;
        SourcePoint def = null;
        if (nextStep != null)
        {
            WorkflowTransitionInfo transition = nextStep.RelatedTransition;
            if (transition != null)
            {
                def = currentStep.GetSourcePoint(transition.TransitionSourcePointGUID);
                nextStepName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(nextStep.StepDisplayName));
            }
        }
        else
        {
            def = currentStep.StepDefinition.DefinitionPoint;
        }

        if (def != null)
        {
            if (!string.IsNullOrEmpty(def.Text))
            {
                action.Text = string.Format(HTMLHelper.HTMLEncode(ResHelper.LocalizeString(def.Text)), nextStepName);
            }

            if (!string.IsNullOrEmpty(def.Tooltip))
            {
                action.Tooltip = string.Format(ResHelper.LocalizeString(def.Tooltip), action.Text);
            }
        }
    }


    /// <summary>
    /// Adds menu action.
    /// </summary>
    /// <param name="action">Action</param>
    protected void AddAction(HeaderAction action)
    {
        if (action != null)
        {
            // Action
            menu.AddAction(action);
        }
    }


    /// <summary>
    /// Adds comment action.
    /// </summary>
    /// <param name="name">Action name</param>
    /// <param name="action">Current action</param>
    /// <param name="objectName">Object name</param>
    private void AddCommentAction(string name, HeaderAction action, string objectName)
    {
        AutomationManager.RenderScript = true;

        CommentAction comment = new CommentAction(name)
                                   {
                                       Tooltip = string.Format(GetString("EditMenu.Comment" + name), objectName),
                                       OnClientClick = string.Format("AddComment_{0}('{1}',{2},'{0}');", ClientID, name, StateObject.StateID),
                                   };
        action.AlternativeActions.Add(comment);
    }

    #endregion
}
