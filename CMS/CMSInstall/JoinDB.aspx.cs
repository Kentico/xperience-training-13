using System;
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WebFarmSync;
using CMS.WinServiceEngine;

using MessageTypeEnum = CMS.DataEngine.MessageTypeEnum;

[CheckLicence(FeatureEnum.DBSeparation)]
public partial class CMSInstall_JoinDB : GlobalAdminPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help topic page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "cm_database_rejoining";

    private const string PURPOSE = "ConnectionStringProtection";

    #endregion


    #region "Variables"

    private LocalizedButton mNextButton;
    private LocalizedButton mPreviousButton;
    private ISqlServerCapabilities mSqlServerCapabilities;
    private const string STORAGE_KEY = "SeparateDBProgressLog";

    #endregion


    #region "Properties"

    /// <summary>
    /// Help button on start step.
    /// </summary>
    protected HelpControl StartHelp
    {
        get
        {
            return (HelpControl)wzdInstaller.Controls[0].Controls[2].Controls[0].Controls[0].Controls[1].FindControl("hlpContext");
        }
    }


    /// <summary>
    /// Help button.
    /// </summary>
    protected HelpControl Help
    {
        get
        {
            return (HelpControl)wzdInstaller.Controls[0].Controls[2].Controls[0].Controls[2].Controls[1].FindControl("hlpContext");
        }
    }


    /// <summary>
    /// Return URL.
    /// </summary>
    private string ReturnUrl
    {
        get
        {
            string returnUrl = QueryHelper.GetString("returnurl", String.Empty);

            // Ensure that URL is valid
            if (string.IsNullOrEmpty(returnUrl) || returnUrl.StartsWith("~", StringComparison.Ordinal) || returnUrl.StartsWith("/", StringComparison.Ordinal) || QueryHelper.ValidateHash("hash"))
            {
                return returnUrl;
            }
            return null;
        }
    }


    /// <summary>
    /// Returns SQL server capabilities.
    /// </summary>
    private ISqlServerCapabilities SqlServerCapabilities
    {
        get
        {
            return mSqlServerCapabilities ?? (mSqlServerCapabilities = SqlServerCapabilitiesFactory.GetSqlServerCapabilities(ConnectionHelper.GetSqlConnectionString()));
        }
    }

    #endregion


    #region "Step wizard buttons"

    /// <summary>
    /// Previous button.
    /// </summary>
    public LocalizedButton PreviousButton
    {
        get
        {
            if (mPreviousButton == null)
            {
                mPreviousButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepPrevButton") as LocalizedButton;
            }
            return mPreviousButton;
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton
    {
        get
        {
            if (mNextButton == null)
            {
                mNextButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepNextButton") as LocalizedButton;
            }
            return mNextButton;
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton FinishNextButton
    {
        get
        {
            if (wzdInstaller.FindControl("FinishNavigationTemplateContainerID").FindControl("finishNavigation") != null)
            {
                return wzdInstaller.FindControl("FinishNavigationTemplateContainerID").FindControl("finishNavigation").FindControl("StepNextButton") as LocalizedButton;
            }
            return null;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ControlsHelper.EnsureScriptManager(Page);

        if (FinishNextButton != null)
        {
            FinishNextButton.Click += FinishNextButton_Click;
        }
        CurrentMaster.HeadElements.Text += @"<base target=""_self"" />";
        CurrentMaster.HeadElements.Visible = true;
        PageTitle.ShowFullScreenButton = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        SetControls();
        SetProgressStep();
        RegisterJS();

        if (String.IsNullOrEmpty(hdnConnString.Value) && SqlInstallationHelper.DatabaseIsSeparated())
        {
            hdnConnString.Value = ProtectConnectionString();
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        InitializeHeader(wzdInstaller.ActiveStepIndex);

        if (!String.IsNullOrEmpty(ReturnUrl))
        {
            CurrentMaster.HeaderContainer.CssClass += " separation-header";

            NextButton.Text = GetString("separationDB.return");
            NextButton.OnClientClick = "window.location =\"" + URLHelper.ResolveUrl(ReturnUrl) + "\"; return false;";
            PreviousButton.Visible = false;

            // Ensure the refresh script
            const string defaultCondition = "((top.frames['cmsdesktop'] != null) || (top.frames['propheader'] != null))";
            const string topWindowReplace = "top.window.location.replace(top.window.location.reload());";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "TopWindow", ScriptHelper.GetScript(" if " + defaultCondition + " { try {" + topWindowReplace + "} catch(err){} }"));
        }
    }

    #endregion


    #region "Wizard events"

    /// <summary>
    /// Next step button click.
    /// </summary>
    protected void wzdInstaller_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        bool validationResult = true;
        switch (wzdInstaller.ActiveStepIndex)
        {
            case 0:
                validationResult = joinPrerequisites.IsValidForDBJoin();
                break;
        }

        if (!validationResult)
        {
            e.Cancel = true;
        }
        else
        {
            RunAction();
        }
    }


    /// <summary>
    /// Finish button click.
    /// </summary>
    protected void FinishNextButton_Click(object sender, EventArgs e)
    {
        if (!SqlInstallationHelper.DatabaseIsSeparated())
        {
            string error = String.Empty;

            // If it doesn't support OpenQuery command, data could not have been moved so we cannot delete tables.
            if (SqlServerCapabilities.SupportsOpenQueryCommand)
            {
                var separationHelper = new DatabaseSeparationHelper();

                separationHelper.InstallScriptsFolder = SqlInstallationHelper.GetSQLInstallPathToObjects();
                separationHelper.ScriptsFolder = Server.MapPath("~/App_Data/DBSeparation");
                separationHelper.InstallationConnStringSeparate = UnprotectConnectionString();
                error = separationHelper.DeleteSourceTables(separationFinished.DeleteOldDB, false);
            }

            if (!String.IsNullOrEmpty(error))
            {
                separationFinished.ErrorLabel.Visible = true;
                separationFinished.ErrorLabel.Text = error;
                var logData = new EventLogData(EventTypeEnum.Error, "Contact management database join", "DELETEOLDDATA")
                {
                    EventDescription = error,
                    EventUrl = RequestContext.CurrentURL
                };

                Service.Resolve<IEventLogService>().LogEvent(logData);
            }
            else
            {
                EnableTasks();
                WebFarmHelper.CreateTask(new RestartApplicationWebFarmTask());
                ScriptHelper.RegisterStartupScript(this, typeof(string), "Close dialog", ScriptHelper.GetScript("RefreshParent(); CloseDialog();"));
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes controls.
    /// </summary>
    private void SetControls()
    {
        if (!RequestHelper.IsCallback())
        {
            PreviousButton.Attributes.Remove("disabled");
            NextButton.Attributes.Remove("disabled");
        }

        StartHelp.Tooltip = ResHelper.GetFileString("install.tooltip");
        StartHelp.TopicName = HELP_TOPIC_LINK;
        StartHelp.IconCssClass = "cms-icon-80";
        Response.Cache.SetNoStore();
        Help.Tooltip = ResHelper.GetFileString("install.tooltip");
        Help.IconCssClass = "cms-icon-80";
        separationFinished.InfoLabel.ResourceString = "separationDB.joinOK";
        progress.NextButtonClientID = NextButton.ClientID;
        progress.PreviousButtonClientID = PreviousButton.ClientID;
        asyncControl.OnFinished += asyncControl_OnFinished;
        asyncControl.OnError += asyncControl_OnError;
        PageTitle.TitleText = GetString("separationDB.joinTitle");
        CurrentMaster.Body.Attributes["class"] += " install-body-join";
        asyncControl.LogPanel.CssClass = "Hidden";
    }


    /// <summary>
    /// Initialize wizard header.
    /// </summary>
    /// <param name="index">Step index</param>
    private void InitializeHeader(int index)
    {
        Help.Visible = true;
        StartHelp.Visible = true;
        StartHelp.TopicName = Help.TopicName = HELP_TOPIC_LINK;

        lblHeader.Text = ResHelper.GetFileString("Install.Step") + " - ";

        string[] stepIcons = { " icon-cogwheel", " icon-merge", " icon-check-circle icon-style-allow" };
        string[] stepTitles = { GetString("install.prerequisites"), GetString("install.merging"), GetString("install.finishstep") };

        // Set common properties to each step icon
        for (var i = 0; i < stepIcons.Length; i++)
        {
            // Step panel
            var pnlStepIcon = new Panel();
            pnlStepIcon.ID = "stepPanel" + i;
            pnlStepIcon.CssClass = "install-step-panel";
            pnlHeaderImages.Controls.Add(pnlStepIcon);

            // Step icon
            var icon = new CMSIcon();
            icon.ID = "stepIcon" + i;
            icon.CssClass = "install-step-icon cms-icon-200" + stepIcons[i];
            icon.Attributes.Add("aria-hidden", "true");
            pnlStepIcon.Controls.Add(icon);

            // Step icon title
            var title = new HtmlGenericControl("title");
            title.ID = "stepTitle" + i;
            title.InnerText = stepTitles[i];
            title.Attributes.Add("class", "install-step-title");
            pnlStepIcon.Controls.Add(title);

            // Render separator only between step icons
            if (i < stepIcons.Length - 1)
            {
                // Separator panel
                var pnlSeparator = new Panel();
                pnlSeparator.ID = "separatorPanel" + i;
                pnlSeparator.CssClass = "install-step-icon-separator";
                pnlHeaderImages.Controls.Add(pnlSeparator);

                // Separator icon
                var separatorIcon = new CMSIcon();
                separatorIcon.CssClass = "icon-arrow-right cms-icon-150";
                separatorIcon.Attributes.Add("aria-hidden", "true");
                pnlSeparator.Controls.Add(separatorIcon);
            }
        }

        switch (index)
        {
            // Ready
            case 0:
                {
                    lblHeader.Text += GetString("separationDB.joinStep0");
                    SetSelectedCSSClass("stepPanel0");
                    break;
                }
            // Progress
            case 1:
                {
                    StartHelp.Visible = Help.Visible = false;
                    lblHeader.Text += ResHelper.GetFileString("separationDB.joinStep1");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }
            // Finish step
            case 2:
                {
                    lblHeader.Text += ResHelper.GetFileString("separationDB.StepFinish");
                    SetSelectedCSSClass("stepPanel2");
                    break;
                }
        }

        lblHeader.Text = string.Format(lblHeader.Text, wzdInstaller.ActiveStepIndex + 1);
    }


    private void SetSelectedCSSClass(string stepPanel)
    {
        var selectedPanel = pnlHeaderImages.FindControl(stepPanel) as Panel;
        selectedPanel.CssClass += " install-step-icon-selected";
    }


    /// <summary>
    /// Runs appropriate action for specific wizard step.
    /// </summary>
    private void RunAction()
    {
        switch (wzdInstaller.ActiveStepIndex)
        {
            case 0:
                NextButton.Attributes.Add("disabled", "true");
                PreviousButton.Attributes.Add("disabled", "true");
                PersistentStorageHelper.RemoveValue(STORAGE_KEY);
                progress.StartLogging();
                asyncControl.RunAsync(StartJoin, WindowsIdentity.GetCurrent());
                break;
        }
    }


    /// <summary>
    /// Launches separation process.
    /// </summary>
    private void StartJoin(object parameter)
    {
        try
        {
            JoinDB();
        }
        catch (Exception e)
        {
            progress.LogMessage(e.Message, MessageTypeEnum.Error, false);
            asyncControl.RaiseError(this, null);
            throw;
        }
        finally
        {
            DatabaseSeparationHelper.SeparationInProgress = false;
        }
    }


    /// <summary>
    /// Joins databases.
    /// </summary>
    private void JoinDB()
    {
        var separation = new DatabaseSeparationHelper();

        separation.InstallScriptsFolder = SqlInstallationHelper.GetSQLInstallPathToObjects();
        separation.ScriptsFolder = Server.MapPath("~/App_Data/DBSeparation");
        separation.LogMessage = progress.LogMessage;
        separation.JoinDatabase();

        progress.LogMessage(GetString("separationDB.joinOK"), MessageTypeEnum.Finished, false);
    }


    /// <summary>
    /// Set connection string into web.config.
    /// </summary>
    private void SetConnectionString()
    {
        if (!SystemContext.IsRunningOnAzure)
        {
            if (!SettingsHelper.RemoveConnectionString(DatabaseSeparationHelper.ConnStringSeparateName))
            {
                separationFinished.ErrorLabel.Visible = true;
                string resultStringDisplay = " <br/><br/><strong>&lt;add name=\"" + DatabaseSeparationHelper.ConnStringSeparateName + "\" connectionString=\"" + DatabaseSeparationHelper.ConnStringSeparate + "\"/&gt;</strong><br/><br/>";
                separationFinished.ErrorLabel.Text = GetString("separationDB.removeConnectionStringError") + resultStringDisplay;
            }
        }
        else
        {
            string connString = "&lt;add name=\"" + DatabaseSeparationHelper.ConnStringSeparateName + "\" connectionString=\"" + DatabaseSeparationHelper.ConnStringSeparate + "\"/&gt;";
            string appSetting = "&lt;Setting name=\"" + DatabaseSeparationHelper.ConnStringSeparateName + "\" value=\"" + DatabaseSeparationHelper.ConnStringSeparate + "\"/&gt;";

            separationFinished.AzureErrorLabel.Visible = true;
            separationFinished.AzureErrorLabel.Text = String.Format(GetString("separationDB.removeConnectionStringErrorAzure"), connString, appSetting);
        }

        // If it doesn't support OpenQuery command, show info about need to do manual data move.
        if (!SqlServerCapabilities.SupportsOpenQueryCommand)
        {
            separationFinished.AzureErrorLabel.Visible = true;
            separationFinished.AzureErrorLabel.Text += GetManualCopyText();
        }

        if (!separationFinished.ErrorLabel.Visible)
        {
            separationFinished.ErrorLabel.Text += "<br />";
        }
        else
        {
            separationFinished.ErrorLabel.Visible = true;
        }
    }


    /// <summary>
    /// Returns text for azure containing tables which need to be manually copied.
    /// </summary>
    private string GetManualCopyText()
    {
        SeparatedTables tables = new SeparatedTables(Server.MapPath("~/App_Data/DBSeparation"));
        return GetString("separationDB.manualcopy") + tables.GetTableNames("<br />");
    }


    /// <summary>
    ///  Skips dialog directly to progress step.
    /// </summary>
    private void SetProgressStep()
    {
        if (!String.IsNullOrEmpty(ReturnUrl) && DatabaseSeparationHelper.SeparationInProgress)
        {
            wzdInstaller.ActiveStepIndex = 1;
            NextButton.Attributes.Add("disabled", "true");
            PreviousButton.Attributes.Add("disabled", "true");
            progress.StartLogging();
        }
    }


    /// <summary>
    /// Enables tasks after separation is over.
    /// </summary>
    private void EnableTasks()
    {
        if (ValidationHelper.GetBoolean(PersistentStorageHelper.GetValue("CMSSchedulerTasksEnabled"), false))
        {
            SettingsKeyInfoProvider.SetGlobalValue("CMSSchedulerTasksEnabled", true);

            // Restart win service
            WinServiceHelper.RestartService(WinServiceHelper.HM_SERVICE_BASENAME, false);
        }
        PersistentStorageHelper.RemoveValue("CMSSchedulerTasksEnabled");
    }


    /// <summary>
    /// Registers JavaScript.
    /// </summary>
    private void RegisterJS()
    {
        ScriptHelper.RegisterWOpenerScript(Page);

        // Register refresh script to refresh wopener
        var script = new StringBuilder();
        script.Append(@"
function RefreshParent() {
  if ((wopener != null) && (wopener.RefreshPage != null)) {
      wopener.RefreshPage();
    }
}");
        // Register script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "WOpenerRefresh", ScriptHelper.GetScript(script.ToString()));
    }


    private static string ProtectConnectionString()
    {
        var protectedConnectionString = MachineKey.Protect(Encoding.UTF8.GetBytes(DatabaseSeparationHelper.ConnStringSeparate), PURPOSE);
        return Convert.ToBase64String(protectedConnectionString);
    }


    private string UnprotectConnectionString()
    {
        var connectionString = MachineKey.Unprotect(Convert.FromBase64String(hdnConnString.Value), PURPOSE);
        return Encoding.UTF8.GetString(connectionString);
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Thread with separation finished with exception
    /// </summary>
    void asyncControl_OnError(object sender, EventArgs e)
    {
        if (asyncControl.Worker.LastException != null)
        {
            progress.LogMessage(asyncControl.Worker.LastException.Message, MessageTypeEnum.Error, false);
        }
        PreviousButton.Attributes.Remove("disabled");
    }


    /// <summary>
    /// Thread with separation finished successfully.
    /// </summary>
    void asyncControl_OnFinished(object sender, EventArgs e)
    {
        NextButton.Attributes.Remove("disabled");
        wzdInstaller.ActiveStepIndex++;
        SetConnectionString();
    }

    #endregion
}
