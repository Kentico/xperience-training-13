using System;
using System.Security.Principal;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.TranslationServices;
using CMS.TranslationServices.Web.UI;
using CMS.TranslationServices.Web.UI.Internal;
using CMS.UIControls;


// Edited object
[EditedObject(TranslationSubmissionInfo.OBJECT_TYPE, "submissionId")]

// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "translationservice.translationsubmission.list", "~/CMSModules/Translations/Pages/Tools/TranslationSubmission/List.aspx", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "translationservice.translationsubmission.new", NewObject = true)]

// Title
[Title(ResourceString = "translationservice.translationsubmission.edit", HelpTopic = "translationservices_translationsubmission_edit", ExistingObject = true)]
[Title(ResourceString = "translationservice.translationsubmission.new", HelpTopic = "translationservices_translationsubmission_edit", NewObject = true)]

public partial class CMSModules_Translations_Pages_Tools_TranslationSubmission_Edit : CMSTranslationServicePage
{
    #region "Constants"

    private const string PROCESS_ACTION = "process";
    private const string RESUBMIT_ACTION = "resubmit";

    private const string SEPARATOR = "##SEP##";

    #endregion


    #region "Variables"

    TranslationSubmissionInfo mSubmissionInfo;
    
    #endregion


    #region "Properties"

    /// <summary>
    /// Returns submission info being edited.
    /// </summary>
    public TranslationSubmissionInfo SubmissionInfo
    {
        get
        {
            return mSubmissionInfo ?? (mSubmissionInfo = (TranslationSubmissionInfo)editElem.UIFormControl.EditedObject);
        }
    }

    #endregion


    #region "Page events"


    protected void Page_Init(object sender, EventArgs e)
    {
        var scopeIdentificator = new AsyncScopeIdentifierGenerator(GetDataForScopeIdentifier());
        ctlAsync.ProcessGUID = scopeIdentificator.Generate();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        btnShowMessage.Click += btnShowMessage_Click;
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        ctlAsync.OnError += worker_OnError;
        ctlAsync.OnFinished += worker_OnFinished;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (SubmissionInfo == null)
        {
            return;
        }

        CreateButtons();
        HandleAsyncProgress();

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "ShowUploadSuccess", "function ShowUploadSuccess() { " + ControlsHelper.GetPostBackEventReference(btnShowMessage) + " }", true);
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // There is already a running thread
        if (IsRunningThread())
        {
            return;
        }

        CheckModifyPermissions(true);

        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                if (!editElem.UIFormControl.SaveData(null))
                {
                    ShowError(GetString("translationservice.savesubmissionfailed"));
                }
                break;

            case RESUBMIT_ACTION:
                ProcessActionAsync(e.CommandName);
                break;

            case "updateandresubmit":
                if (!editElem.UIFormControl.SaveData(null))
                {
                    ShowError(GetString("translationservice.savesubmissionfailed"));
                }
                else
                {
                    ProcessActionAsync("resubmit");
                }
                break;

            case "cancel":
                string errCancel = TranslationServiceHelper.CancelSubmission(SubmissionInfo);
                if (string.IsNullOrEmpty(errCancel))
                {
                    ShowConfirmation(GetString("translationservice.submissioncanceled"));
                }
                else
                {
                    ShowError(errCancel);
                }
                editElem.UIFormControl.ReloadData();
                break;

            case PROCESS_ACTION:
                ProcessActionAsync(e.CommandName);
                break;
        }
    }


    private void worker_OnFinished(object sender, EventArgs e)
    {
        var parameter = ctlAsync.Parameter;
        if (parameter == null)
        {
            return;
        }

        string action;
        string error;
        string submissionName;
        int submissionId;

        ParseParameter(parameter, out action, out error, out submissionName, out submissionId);

        bool current = (submissionId == SubmissionInfo.SubmissionID);

        if (!string.IsNullOrEmpty(error))
        {
            // Show error from the service for current submission
            if (current)
            {
                ShowError(error);
            }
        }
        else
        {
            string message = String.Empty;

            // Get correct confirmation message
            switch (action)
            {
                case PROCESS_ACTION:
                    message = current ? "translationservice.translationsimported" : "translationservice.name.translationsimported";
                    break;

                case RESUBMIT_ACTION:
                    message = current ? "translationservice.translationresubmitted" : "translationservice.name.translationresubmitted";
                    break;
            }

            if (!String.IsNullOrEmpty(message))
            {
                var text = !current ? string.Format(GetString(message), HTMLHelper.HTMLEncode(submissionName)) : GetString(message);
                ShowConfirmation(text);
            }
        }

        // Reload is required because status of the submission could have been changed and form has to be reloaded
        editElem.UIFormControl.ReloadData();
    }


    private static void ParseParameter(string parameter, out string commandName, out string error, out string submissionName, out int submissionId)
    {
        var parameters = TextHelper.SplitByString(parameter, SEPARATOR, 4);

        commandName = ValidationHelper.GetString(parameters[0], "").ToLowerInvariant();
        error = ValidationHelper.GetString(parameters[1], "");
        submissionName = ValidationHelper.GetString(parameters[2], "");
        submissionId = ValidationHelper.GetInteger(parameters[3], 0);
    }


    private void worker_OnError(object sender, EventArgs e)
    {
        ShowError(GetString("translationservice.actionfailed"));

        // Reload is required because status of the submission could have been changed and form has to be reloaded
        editElem.UIFormControl.ReloadData();
    }


    protected void btnShowMessage_Click(object sender, EventArgs e)
    {
        ShowInformation(GetString("translationservice.translationuploadedsuccessfully"));

        // Reload is required because status of the submission could have been changed and form has to be reloaded
        editElem.UIFormControl.ReloadData();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Processes the action asynchronously
    /// </summary>
    /// <param name="commandName">Command name</param>
    private void ProcessActionAsync(string commandName)
    {
        var submission = SubmissionInfo;
        
        // Set flag
        submission.SubmissionStatus = string.Equals(commandName, RESUBMIT_ACTION, StringComparison.OrdinalIgnoreCase) ? TranslationStatusEnum.ResubmittingSubmission : TranslationStatusEnum.ProcessingSubmission;
        submission.Update();

        // Run action
        var parameter = new Tuple<string, TranslationSubmissionInfo>(commandName, submission);

        ctlAsync.Parameter = GetParameter(commandName, null, submission);
        ctlAsync.RunAsync(p => ProcessAction(parameter), WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Processes action
    /// </summary>
    /// <param name="parameters">Parameter</param>
    private void ProcessAction(Tuple<string, TranslationSubmissionInfo> parameters)
    {
        if (parameters == null || parameters.Item1 == null)
        {
            return;
        }

        var commandName = parameters.Item1.ToLowerInvariant();
        var submissionInfo = parameters.Item2;
        var error = String.Empty;

        switch (commandName)
        {
            case RESUBMIT_ACTION:
                error = TranslationServiceHelper.ResubmitSubmission(submissionInfo);
                break;

            case PROCESS_ACTION:
                error = TranslationServiceHelper.ProcessSubmission(submissionInfo);
                break;
        }

        // Set result of the action
        ctlAsync.Parameter = GetParameter(commandName, error, submissionInfo);
    }


    private static string GetParameter(string action, string error, TranslationSubmissionInfo submissionInfo)
    {
        var submissionName = (submissionInfo != null) ? submissionInfo.SubmissionName : null;
        var submissionId = (submissionInfo != null) ? submissionInfo.SubmissionID : 0;

        return action + SEPARATOR + error + SEPARATOR + submissionName + SEPARATOR + submissionId;
    }


    /// <summary>
    /// Checks Modify permissions for given translation submission.
    /// </summary>
    /// <param name="redirect">If true, redirects user to Access denied</param>
    private bool CheckModifyPermissions(bool redirect)
    {
        var submission = EditedObject as TranslationSubmissionInfo;
        if (submission == null)
        {
            return true;
        }

        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerObject(PermissionsEnum.Modify, submission, SiteInfoProvider.GetSiteName(submission.SubmissionSiteID)))
        {
            return true;
        }

        if (redirect)
        {
            RedirectToAccessDenied("CMS.TranslationServices", "Modify");
        }

        return false;
    }


    /// <summary>
    /// Ensures changes in UI to indicate asynchronous progress
    /// </summary>
    private void HandleAsyncProgress()
    {
        var threadRunning = IsRunningThread();
        if (threadRunning)
        {
            var label = LoadUserControl("~/CMSFormControls/Basic/LabelControl.ascx") as FormEngineUserControl;
            if (label != null)
            {
                label.ID = "lblStatus";

                string text;

                var parameter = ctlAsync.Parameter;
                if (parameter != null)
                {
                    string error;
                    string action;
                    string submissionName;
                    int submissionId;

                    ParseParameter(parameter, out action, out error, out submissionName, out submissionId);

                    var status = (action == PROCESS_ACTION) ? TranslationStatusEnum.ProcessingSubmission : TranslationStatusEnum.ResubmittingSubmission;
                    var current = (submissionId == SubmissionInfo.SubmissionID);

                    text = current ? GetString(status.ToLocalizedString(null)) : string.Format(GetString(status.ToLocalizedString("translations.status.name")), HTMLHelper.HTMLEncode(submissionName));
                }
                else
                {
                    text = GetString("translationservice.updatingstatuses");
                }

                label.Value = ScriptHelper.GetLoaderInlineHtml(text);
                HeaderActions.AdditionalControls.Add(label);
                HeaderActions.AdditionalControlsCssClass = "header-actions-label control-group-inline";
                HeaderActions.ReloadAdditionalControls();
            }
        }

        HeaderActions.Enabled = !threadRunning;
        editElem.UIFormControl.Enabled = !threadRunning;
    }


    /// <summary>
    /// Indicates if running thread exists
    /// </summary>
    private bool IsRunningThread()
    {
        return ctlAsync.Status == AsyncWorkerStatusEnum.Running;
    }


    /// <summary>
    /// Creates buttons in header actions
    /// </summary>
    private void CreateButtons()
    {
        bool allowed = CheckModifyPermissions(false);

        var processAction = new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            CommandName = PROCESS_ACTION,
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("translationservice.confirmprocesstranslations")) + ")) { return false; }",
            Tooltip = GetString("translationservice.importtranslationstooltip"),
            Text = GetString("translationservice.importtranslations"),
            Enabled = allowed && ((SubmissionInfo.SubmissionStatus == TranslationStatusEnum.TranslationReady) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.TranslationCompleted) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.ProcessingError))
        };

        var resubmitAction = new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            CommandName = RESUBMIT_ACTION,
            Tooltip = GetString("translationservice.resubmittooltip"),
            Text = GetString("translationservice.resubmit"),
            Enabled = allowed && ((SubmissionInfo.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.SubmissionError))
        };

        var updateAction = new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            CommandName = "updateandresubmit",
            Tooltip = GetString("translationservice.updateandresubmittooltip"),
            Text = GetString("translationservice.updateandresubmit"),
            Enabled = allowed && ((SubmissionInfo.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.SubmissionError))
        };

        var saveAction = new SaveAction();
        saveAction.Enabled = allowed;

        var actions = HeaderActions.ActionsList;
        actions.AddRange(new[]
        {
            saveAction,
            updateAction,
            resubmitAction,
            processAction
        });

        var service = TranslationServiceInfoProvider.GetTranslationServiceInfo(SubmissionInfo.SubmissionServiceID);
        if (service != null)
        {
            bool serviceSupportsCancel = service.TranslationServiceSupportsCancel;

            var cancelAction = new HeaderAction
            {
                ButtonStyle = ButtonStyle.Default,
                CommandName = "cancel",
                Tooltip = serviceSupportsCancel ? GetString("translationservice.cancelsubmissiontooltip") : String.Format(GetString("translationservice.cancelnotsupported"), service.TranslationServiceDisplayName),
                Text = GetString("translationservice.cancelsubmission"),
                Enabled = allowed && (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation) && serviceSupportsCancel
            };

            actions.Add(cancelAction);
        }

        HeaderActions.ReloadData();
    }


    private string GetDataForScopeIdentifier()
    {
        var submission = EditedObject as TranslationSubmissionInfo;
        if (submission == null)
        {
            return null;
        }

        var submissionSite = SiteInfoProvider.GetSiteInfo(submission.SubmissionSiteID);
        if (submissionSite == null)
        {
            return null;
        }

        return "UpdateStatuses" + submissionSite.SiteGUID;
    }

    #endregion
}