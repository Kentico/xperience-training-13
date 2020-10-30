using System;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Text;
using System.Web;
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
public partial class CMSInstall_SeparateDB : GlobalAdminPage
{
    #region "Constants"

    // Short link to help topic page.
    private const string HELP_TOPIC_LINK = "cm_database_separating";

    // Index of collation dialog step in wizard
    private const int COLLATION_DIALOG_INDEX = 4;

    #endregion


    #region "Variables"

    private LocalizedButton mNextButton;
    private LocalizedButton mPreviousButton;
    private DatabaseSeparationHelper separation;
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
    /// User password.
    /// </summary>
    private string UserPassword
    {
        get
        {
            return ValidationHelper.GetString(ViewState["UserPassword"], null);
        }
        set
        {
            ViewState["UserPassword"] = value;
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
            if (string.IsNullOrEmpty(returnUrl) || returnUrl.StartsWithCSafe("~") || returnUrl.StartsWithCSafe("/") || QueryHelper.ValidateHash("hash"))
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

        progress.NextButtonClientID = NextButton.ClientID;
        progress.PreviousButtonClientID = PreviousButton.ClientID;
        asyncControl.OnFinished += asyncControl_OnFinished;
        asyncControl.OnError += asyncControl_OnError;
        PageTitle.TitleText = GetString("separationDB.Title");
        userServer.IsDBSeparation = true;
        asyncControl.LogPanel.CssClass = "Hidden";
        if (databaseDialog.UseExistingChecked && (wzdInstaller.ActiveStepIndex == 3))
        {
            separationFinished.Database = databaseDialog.ExistingDatabaseName;
            separationFinished.ConnectionString = databaseDialog.ConnectionString;
            separationFinished.DisplayCollationDialog();
        }
        databaseDialog.ServerName = userServer.ServerName;
        SetProgressStep();
        RegisterJS();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        InitializeHeader(wzdInstaller.ActiveStepIndex);
        CurrentMaster.Body.Attributes["class"] += " install-body install-body-separation";

        if (!String.IsNullOrEmpty(ReturnUrl))
        {
            CurrentMaster.HeaderContainer.CssClass += " separation-header";

            NextButton.Text = GetString("separationDB.return");
            NextButton.OnClientClick = "window.location = \"" + URLHelper.ResolveUrl(ReturnUrl) + "\"; return false;";
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
                validationResult = CheckSqlSettings();
                break;

            case 1:
            case COLLATION_DIALOG_INDEX:
                validationResult = databaseDialog.ValidateForSeparation();

                if (validationResult && databaseDialog.UseExistingChecked)
                {
                    var connectionString = databaseDialog.ConnectionString;
                    var databaseName = databaseDialog.ExistingDatabaseName;

                    string collation = DatabaseHelper.GetDatabaseCollation(connectionString);
                    DatabaseHelper.DatabaseCollation = collation;

                    if (wzdInstaller.ActiveStepIndex != COLLATION_DIALOG_INDEX)
                    {
                        // Check target database collation and inform the user if it is not fully supported
                        if (!DatabaseHelper.IsSupportedDatabaseCollation(collation))
                        {
                            ucCollationDialog.IsSqlAzure = AzureHelper.IsSQLAzureServer(userServer.ServerName);
                            ucCollationDialog.Collation = collation;
                            ucCollationDialog.InitControls();

                            // Move to "collation dialog" step
                            wzdInstaller.ActiveStepIndex = COLLATION_DIALOG_INDEX;
                            return;
                        }
                    }
                    else
                    {
                        // Change database collation for regular database
                        if (ucCollationDialog.ChangeCollationRequested)
                        {
                            DatabaseHelper.ChangeDatabaseCollation(connectionString, databaseName, DatabaseHelper.DEFAULT_DB_COLLATION);
                        }
                    }
                }

                // Move to "progress" step
                wzdInstaller.ActiveStepIndex = 2;
                break;
        }

        if (!validationResult)
        {
            e.Cancel = true;
        }
        else
        {
            databaseDialog.TasksManuallyStopped = false;
            RunAction();
        }
    }


    /// <summary>
    /// Finish button click.
    /// </summary>
    protected void FinishNextButton_Click(object sender, EventArgs e)
    {
        if (SqlInstallationHelper.DatabaseIsSeparated())
        {
            string error = String.Empty;
            // If it doesn't support OpenQuery command, data could not have been moved so we cannot delete tables.
            if (SqlServerCapabilities.SupportsOpenQueryCommand)
            {
                var separationHelper = new DatabaseSeparationHelper();

                separationHelper.InstallationConnStringSeparate = DatabaseSeparationHelper.ConnStringSeparate;
                separationHelper.InstallScriptsFolder = SqlInstallationHelper.GetSQLInstallPathToObjects();
                separationHelper.ScriptsFolder = Server.MapPath("~/App_Data/DBSeparation");
                error = separationHelper.DeleteSourceTables(false, true);
            }

            if (!String.IsNullOrEmpty(error))
            {
                separationFinished.ErrorLabel.Visible = true;
                separationFinished.ErrorLabel.Text = error;
                var logData = new EventLogData(EventTypeEnum.Error, "Contact management database separation", "DELETEOLDDATA")
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
    /// Initialize wizard header.
    /// </summary>
    /// <param name="index">Step index</param>
    private void InitializeHeader(int index)
    {
        Help.Visible = true;
        StartHelp.Visible = true;
        StartHelp.TopicName = Help.TopicName = HELP_TOPIC_LINK;

        lblHeader.Text = ResHelper.GetFileString("Install.Step") + " - ";

        string[] stepIcons = { " icon-cogwheel", " icon-database", " icon-separate", " icon-check-circle icon-style-allow" };
        string[] stepTitles = { GetString("install.sqlsetting"), GetString("install.lbldatabase"), GetString("install.separation"), GetString("install.finishstep") };

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

        var currentStepIndex = wzdInstaller.ActiveStepIndex;
        switch (index)
        {
            // SQL server and authentication mode
            case 0:
                lblHeader.Text += GetString("separationDB.Step0");
                SetSelectedCSSClass("stepPanel0");
                break;
            // Database
            case 1:
            case COLLATION_DIALOG_INDEX:
                lblHeader.Text += ResHelper.GetFileString("separationDB.Step1");
                SetSelectedCSSClass("stepPanel1");
                currentStepIndex = 1;
                break;
            // Separation
            case 2:
                StartHelp.Visible = Help.Visible = false;
                lblHeader.Text += ResHelper.GetFileString("separationDB.Step2");
                SetSelectedCSSClass("stepPanel2");
                break;
            // Finish step
            case 3:
                lblHeader.Text += ResHelper.GetFileString("separationDB.StepFinish");
                SetSelectedCSSClass("stepPanel3");
                break;
        }

        lblHeader.Text = string.Format(lblHeader.Text, currentStepIndex + 1);
    }


    private void SetSelectedCSSClass(string stepPanel)
    {
        var selectedPanel = pnlHeaderImages.FindControl(stepPanel) as Panel;
        selectedPanel.CssClass += " install-step-icon-selected";
    }


    /// <summary>
    /// Checks if SQL settings on first page are correct.
    /// </summary>
    private bool CheckSqlSettings()
    {
        UserPassword = userServer.DBPassword;

        if (!userServer.ValidateForSeparation())
        {
            return false;
        }

        databaseDialog.AuthenticationType = userServer.WindowsAuthenticationChecked ? SQLServerAuthenticationModeEnum.WindowsAuthentication : SQLServerAuthenticationModeEnum.SQLServerAuthentication;
        databaseDialog.Password = UserPassword;
        databaseDialog.Username = userServer.DBUsername;
        databaseDialog.ServerName = userServer.ServerName;

        return true;
    }


    /// <summary>
    /// Runs appropriate action for specific wizard step.
    /// </summary>
    private void RunAction()
    {
        switch (wzdInstaller.ActiveStepIndex)
        {
            case 2:
                PreviousButton.Attributes.Add("disabled", "true");
                NextButton.Attributes.Add("disabled", "true");
                PersistentStorageHelper.RemoveValue(STORAGE_KEY);
                progress.StartLogging();
                asyncControl.RunAsync(StartSeparation, WindowsIdentity.GetCurrent());
                break;
        }
    }


    /// <summary>
    /// Launches separation process.
    /// </summary>
    private void StartSeparation(object parameter)
    {
        try
        {
            if (CreateDB())
            {
                SeparateDB();
            }
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
    /// Creates new database.
    /// </summary>
    private bool CreateDB()
    {
        bool continueSeparation = true;
        if (databaseDialog.CreateNewChecked && SqlServerCapabilities.SupportsDatabaseCreation)
        {
            progress.LogMessage(GetString("separationDB.creatingDB"), MessageTypeEnum.Info, false);

            var connectionString = new SqlConnectionStringBuilder(databaseDialog.ConnectionString);
            connectionString.InitialCatalog = "";

            try
            {
                using (new CMSConnectionScope(connectionString.ConnectionString, false))
                {
                    SqlInstallationHelper.CreateDatabase(databaseDialog.NewDatabaseName, connectionString.ConnectionString, null);
                }
            }
            catch (Exception ex)
            {
                string message = ResHelper.GetFileString("Intaller.LogErrorCreateDB") + " " + ex.Message;
                progress.LogMessage(message, MessageTypeEnum.Error, false);
                continueSeparation = false;
            }
        }
        return continueSeparation;
    }


    /// <summary>
    /// Separates database.
    /// </summary>
    private void SeparateDB()
    {
        separation = new DatabaseSeparationHelper();

        separation.InstallationConnStringSeparate = databaseDialog.ConnectionString;
        separation.InstallScriptsFolder = SqlInstallationHelper.GetSQLInstallPathToObjects();
        separation.ScriptsFolder = Server.MapPath("~/App_Data/DBSeparation");
        separation.LogMessage = progress.LogMessage;

        separation.SeparateDatabase();

        progress.LogMessage(GetString("SeparationDB.OK"), MessageTypeEnum.Finished, false);
    }


    /// <summary>
    /// Set connection string into web.config.
    /// </summary>
    private void SetConnectionString()
    {
        string connectionStringName = DatabaseSeparationHelper.ConnStringSeparateName;
        string encodedPassword = HttpUtility.HtmlEncode(HttpUtility.HtmlEncode(databaseDialog.Password));
        if (!SystemContext.IsRunningOnAzure)
        {
            if (!SettingsHelper.SetConnectionString(connectionStringName, databaseDialog.ConnectionString))
            {
                string connStringDisplay = ConnectionHelper.BuildConnectionString(databaseDialog.AuthenticationType, databaseDialog.ServerName, databaseDialog.Database, databaseDialog.Username, encodedPassword, 240);
                string resultStringDisplay = "&lt;add name=\"" + connectionStringName + "\" connectionString=\"" + connStringDisplay + "\"/&gt;";

                separationFinished.ErrorLabel.Visible = true;
                separationFinished.ErrorLabel.Text = String.Format(ResHelper.GetFileString("Install.ConnectionStringError"), resultStringDisplay);
            }
        }
        else
        {
            string connStringValue = ConnectionHelper.BuildConnectionString(databaseDialog.AuthenticationType, databaseDialog.ServerName, databaseDialog.Database, databaseDialog.Username, encodedPassword, 240, isForAzure: true);
            string connString = "&lt;add name=\"" + connectionStringName + "\" connectionString=\"" + connStringValue + "\"/&gt;";
            string appSetting = "&lt;Setting name=\"" + connectionStringName + "\" value=\"" + connStringValue + "\"/&gt;";

            separationFinished.AzureErrorLabel.Visible = true;
            separationFinished.AzureErrorLabel.Text = String.Format(ResHelper.GetFileString("Install.ConnectionStringAzure"), connString, appSetting);
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
            wzdInstaller.ActiveStepIndex = 2;
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

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Thread with separation finished with exception.
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
