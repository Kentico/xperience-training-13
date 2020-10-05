using System;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Localization;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.TranslationServices;
using CMS.TranslationServices.Web.UI.Internal;
using CMS.UIControls;


public partial class CMSModules_Translations_Controls_UI_TranslationSubmission_List : CMSAdminListControl
{
    #region "Constants"

    private const string PROCESS_ACTION = "process";
    private const string RESUBMIT_ACTION = "resubmit";
    private const string UPDATE_STATUSES_ACTION = "updatestatuses";

    private const string SEPARATOR = "##SEP##";

    #endregion


    #region "Variables"

    private bool modifyAllowed;

    #endregion


    #region "Properties"

    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
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
            gridElem.StopProcessing = value;
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

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        var scopeIdentificator = new AsyncScopeIdentifierGenerator(GetDataForScopeIdentifier());
        ctlAsync.ProcessGUID = scopeIdentificator.Generate();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        modifyAllowed = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.TranslationServices", "Modify");

        gridElem.WhereCondition = "SubmissionSiteID = " + SiteContext.CurrentSiteID;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnAction += gridElem_OnAction;
        ctlAsync.OnError += worker_OnError;
        ctlAsync.OnFinished += worker_OnFinished;
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        Page.PreRender += Page_OnPreRender;
    }


    private void Page_OnPreRender(object o, EventArgs args)
    {
        ScriptHelper.RegisterDialogScript(Page);

        CreateButtons();

        HandleAsyncProgress();
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // There is already a running thread
        if (IsRunningThread())
        {
            return;
        }

        switch (e.CommandName)
        {
            case UPDATE_STATUSES_ACTION:
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.TranslationServices", "Modify"))
                {
                    RedirectToAccessDenied("CMS.TranslationServices", "Modify");
                }

                UpdateStatusesAsync();
                break;
        }
    }


    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        // There is already a running thread
        if (IsRunningThread())
        {
            return;
        }

        string err = null;
        string info = null;

        // Check modify permission for all actions except for download ZIP
        if (!string.Equals(actionName, "downloadzip", StringComparison.OrdinalIgnoreCase))
        {
            if (!modifyAllowed)
            {
                RedirectToAccessDenied("CMS.TranslationServices", "Modify");
            }
        }

        // Get submission
        var submissionInfo = TranslationSubmissionInfo.Provider.Get(ValidationHelper.GetInteger(actionArgument, 0));
        if (submissionInfo == null)
        {
            return;
        }

        switch (actionName.ToLowerInvariant())
        {
            case "downloadzip":
                TranslationServiceHelper.DownloadXLIFFinZIP(submissionInfo, Page.Response);
                break;

            case RESUBMIT_ACTION:
                ProcessActionAsync(actionName, submissionInfo);
                break;

            case PROCESS_ACTION:
                ProcessActionAsync(actionName, submissionInfo);
                break;

            case "cancel":
                err = TranslationServiceHelper.CancelSubmission(submissionInfo);
                info = GetString("translationservice.submissioncanceled");
                break;

            case "delete":
                var serviceInfo = TranslationServiceInfo.Provider.Get(submissionInfo.SubmissionServiceID);
                if (serviceInfo.TranslationServiceSupportsCancel)
                {
                    err = TranslationServiceHelper.CancelSubmission(submissionInfo);
                }
                
                if (String.IsNullOrEmpty(err))
                {
                    submissionInfo.Delete();
                }
                info = GetString("translationservice.submissiondeleted");
                break;
        }

        if (!string.IsNullOrEmpty(err))
        {
            ShowError(err);
        }
        else if (!string.IsNullOrEmpty(info))
        {
            ShowConfirmation(info);
        }
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // For all actions
        if (sourceName.EndsWith("action", StringComparison.OrdinalIgnoreCase))
        {
            var img = sender as CMSGridActionButton;
            if (img != null)
            {
                img.Enabled = !IsRunningThread();
            }
        }

        switch (sourceName.ToLowerInvariant())
        {
            case "resubmitaction":
            case "processaction":
            case "cancelaction":
                var img = sender as CMSGridActionButton;
                if (img != null)
                {
                    img.Enabled &= modifyAllowed;

                    var gvr = parameter as GridViewRow;
                    if (gvr == null)
                    {
                        return img;
                    }

                    var drv = gvr.DataItem as DataRowView;
                    if (drv == null)
                    {
                        return img;
                    }

                    var status = (TranslationStatusEnum)ValidationHelper.GetInteger(drv["SubmissionStatus"], 0);

                    switch (sourceName.ToLowerInvariant())
                    {
                        case "resubmitaction":
                            img.Enabled &= modifyAllowed && ((status == TranslationStatusEnum.WaitingForTranslation) || (status == TranslationStatusEnum.SubmissionError));
                            break;

                        case "processaction":
                            img.Enabled &= modifyAllowed && ((status == TranslationStatusEnum.TranslationReady) || (status == TranslationStatusEnum.TranslationCompleted) || (status == TranslationStatusEnum.ProcessingError));
                            break;

                        case "cancelaction":
                            var service = TranslationServiceInfo.Provider.Get(ValidationHelper.GetInteger(drv["SubmissionServiceID"], 0));
                            if (service != null)
                            {
                                bool serviceSupportsCancel = service.TranslationServiceSupportsCancel;

                                img.Enabled &= modifyAllowed && (status == TranslationStatusEnum.WaitingForTranslation) && serviceSupportsCancel;

                                if (!serviceSupportsCancel)
                                {
                                    // Display tooltip for disabled cancel
                                    img.ToolTip = String.Format(GetString("translationservice.cancelnotsupported"), service.TranslationServiceDisplayName);
                                }
                            }
                            break;
                    }
                }
                return img;

            case "submissionstatus":
                TranslationStatusEnum submissionstatus = (TranslationStatusEnum)ValidationHelper.GetInteger(parameter, 0);
                return TranslationServiceHelper.GetFormattedStatusString(submissionstatus);

            case "submissionprice":
                string price = GetString("general.notavailable");
                double priceVal = ValidationHelper.GetDouble(parameter, -1);
                if (priceVal >= 0)
                {
                    price = priceVal.ToString();
                }
                return price;

            case "submissiontargetculture":
                {
                    string[] cultureCodes = ValidationHelper.GetString(parameter, "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    StringBuilder sb = new StringBuilder();

                    int count = cultureCodes.Length;
                    int counter = 0;
                    foreach (var cultureCode in cultureCodes)
                    {
                        // Limit cultures list
                        if (counter == 5)
                        {
                            sb.Append("&nbsp;");
                            sb.AppendFormat(ResHelper.GetString("translationservices.submissionnamesuffix"), count - 5);
                            break;
                        }
                        // Separate cultures by comma
                        if (counter > 0)
                        {
                            sb.Append(",&nbsp;");
                        }

                        var culture = CultureInfo.Provider.Get(cultureCode);
                        if (culture == null)
                        {
                            continue;
                        }

                        sb.AppendFormat("<span title=\"{0}\"><img class=\"cms-icon-80\" src=\"{1}\" alt=\"{2}\" />&nbsp;{3}</span>", HTMLHelper.EncodeForHtmlAttribute(culture.CultureName), UIHelper.GetFlagIconUrl(null, culture.CultureCode, "16x16"), HTMLHelper.EncodeForHtmlAttribute(culture.CultureName), HTMLHelper.HTMLEncode(culture.CultureShortName));
                        ++counter;
                    }

                    return sb.ToString();
                }
        }

        return parameter;
    }


    private void worker_OnFinished(object sender, EventArgs e)
    {
        var parameter = ctlAsync.Parameter;
        if (parameter == null)
        {
            return;
        }

        string error;
        string submissionName;
        string commandName;

        ParseParameter(parameter, out commandName, out error, out submissionName);

        if (!string.IsNullOrEmpty(error))
        {
            // Show error from the service
            ShowError(error);
        }
        else
        {
            string message = String.Empty;

            // Get correct confirmation message
            switch (commandName)
            {
                case PROCESS_ACTION:
                    message = "translationservice.name.translationsimported";
                    break;

                case RESUBMIT_ACTION:
                    message = "translationservice.name.translationresubmitted";
                    break;

                case UPDATE_STATUSES_ACTION:
                    message = "translationservice.updatingstatusesfinished";
                    break;
            }

            if (!String.IsNullOrEmpty(message))
            {
                ShowConfirmation(string.Format(GetString(message), HTMLHelper.HTMLEncode(submissionName)));
            }
        }
    }


    private static void ParseParameter(string parameter, out string action, out string error, out string submissionName)
    {
        var parameters = TextHelper.SplitByString(parameter, SEPARATOR, 3);

        action = ValidationHelper.GetString(parameters[0], "").ToLowerInvariant();
        error = ValidationHelper.GetString(parameters[1], "");
        submissionName = ValidationHelper.GetString(parameters[2], "");
    }


    private void worker_OnError(object sender, EventArgs e)
    {
        ShowError(GetString("translationservice.actionfailed"));
    }


    /// <summary>
    /// Processes the action asynchronously
    /// </summary>
    /// <param name="commandName">Command name</param>
    /// <param name="submission">Translation submission</param>
    private void ProcessActionAsync(string commandName, TranslationSubmissionInfo submission)
    {
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

        return action + SEPARATOR + error + SEPARATOR + submissionName;
    }


    /// <summary>
    /// Updates statuses of submissions asynchronously
    /// </summary>
    private void UpdateStatusesAsync()
    {
        // Run action
        ctlAsync.Parameter = null;
        ctlAsync.RunAsync(p => UpdateStatuses(SiteContext.CurrentSiteName), WindowsIdentity.GetCurrent());

        Grid.ReloadData();
    }


    /// <summary>
    /// Processes action
    /// </summary>
    /// <param name="siteName">Site name</param>
    private void UpdateStatuses(string siteName)
    {
        var error = TranslationServiceHelper.CheckAndDownloadTranslations(siteName);

        // Set result of the action
        ctlAsync.Parameter = GetParameter(UPDATE_STATUSES_ACTION, error, null);
    }


    /// <summary>
    /// Indicates if running thread exists
    /// </summary>
    private bool IsRunningThread()
    {
        return ctlAsync.Status == AsyncWorkerStatusEnum.Running;
    }


    /// <summary>
    /// Ensures changes in UI to indicate asynchronous progress
    /// </summary>
    private void HandleAsyncProgress()
    {
        if (Grid.IsEmpty)
        {
            return;
        }

        string statusCheck = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSTranslationsLastStatusCheck");
        if (string.IsNullOrEmpty(statusCheck))
        {
            statusCheck = GetString("general.notavailable");
        }

        ShowInformation(string.Format(GetString("translationservice.laststatuscheck"), statusCheck));

        var page = Page as CMSPage;
        if (page == null)
        {
            return;
        }

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

                    ParseParameter(parameter, out action, out error, out submissionName);

                    if (action == UPDATE_STATUSES_ACTION)
                    {
                        text = GetString("translationservice.updatingstatuses");
                    }
                    else
                    {
                        var status = (action == PROCESS_ACTION) ? TranslationStatusEnum.ProcessingSubmission : TranslationStatusEnum.ResubmittingSubmission;

                        text = string.Format(GetString(status.ToLocalizedString("translations.status.name")), HTMLHelper.HTMLEncode(submissionName));
                    }
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
    }


    /// <summary>
    /// Creates buttons in header actions
    /// </summary>
    private void CreateButtons()
    {
        var updateAction = new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            CommandName = UPDATE_STATUSES_ACTION,
            Tooltip = GetString("translationservice.updatestatusestooltip"),
            Text = GetString("translationservice.updatestatuses"),
            Enabled = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.TranslationServices", "Modify") && !Grid.IsEmpty
        };

        string translateUrl = ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Translations/Pages/TranslateDocuments.aspx") + "?select=1&dialog=1";
        translateUrl = URLHelper.AddParameterToUrl(translateUrl, "hash", QueryHelper.GetHash(URLHelper.GetQuery(translateUrl)));

        // Check if any human translation is enabled
        bool enabled = TranslationServiceInfoProvider.GetTranslationServices("(TranslationServiceEnabled = 1) AND (TranslationServiceIsMachine = 0)", null, 0, "TranslationServiceID, TranslationServiceName").Any(t => TranslationServiceHelper.IsServiceAvailable(t.TranslationServiceName, SiteContext.CurrentSiteName));

        var submitAction = new HeaderAction
        {
            OnClientClick = "modalDialog('" + translateUrl + "', 'SubmitTranslation', 988, 640);",
            Tooltip = GetString(enabled ? "translationservice.submittranslationtooltip" : "translationservice.noenabledservices"),
            Text = GetString("translationservice.submittranslation"),
            Enabled = enabled && MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Content", "SubmitForTranslation")
        };

        AddHeaderAction(submitAction);
        AddHeaderAction(updateAction);

        HeaderActions.ReloadData();
    }


    private static string GetDataForScopeIdentifier()
    {
        return "UpdateStatuses" + SiteContext.CurrentSite.SiteGUID;
    }

    #endregion
}