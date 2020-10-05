using System;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;

using MessageTypeEnum = CMS.DataEngine.MessageTypeEnum;

public partial class CMSInstall_install : CMSPage
{
    #region "InstallInfo"

    /// <summary>
    /// Installation info.
    /// </summary>
    [Serializable]
    private class InstallInfo
    {
        #region "Properties"

        /// <summary>
        /// Connection string.
        /// </summary>
        public string ConnectionString
        {
            get;
            set;
        }


        /// <summary>
        /// Scripts full path.
        /// </summary>
        public string ScriptsFullPath
        {
            get;
            set;
        }


        /// <summary>
        /// Log context
        /// </summary>
        public ILogContext LogContext
        {
            get;
            set;
        }

        #endregion


        #region "Methods"

        /// <summary>
        /// Clears the log
        /// </summary>
        public void ClearLog()
        {
            LogContext.Clear();
        }

        #endregion
    }

    #endregion


    #region "Constants"

    private const string WWAG_KEY = "CMSWWAGInstallation";

    // Short link to help topic page.
    private const string HELP_TOPIC_LINK = "database_installation_additional";

    // Short link to help topic page regarding disk permissions.
    private const string HELP_TOPIC_DISK_PERMISSIONS_LINK = "disk_permission_problems";

    // Short link to help topic page regarding SQL error.
    private const string HELP_TOPIC_SQL_ERROR_LINK = HELP_TOPIC_LINK;

    // Index of collation dialog step in wizard
    private const int COLLATION_DIALOG_INDEX = 6;

    private const string INSTALL_CHECK_PAYLOAD = "Install Check Payload";
    private const string INSTALL_CHECK_PURPOSE = "Install POST Check";
    private const string INSTALL_CHECK_COOKIE_NAME = "CMSInstallCheck";
    private const string INSTALL_CHECK_EXCEPTION_MESSAGE = "POST request validation error.";

    #endregion


    #region "Variables"

    private static readonly SafeDictionary<string, InstallInfo> mInstallInfos = new SafeDictionary<string, InstallInfo>();

    private readonly string hostName = RequestContext.URL.Host.ToLowerInvariant();
    private static bool writePermissions = true;

    private LocalizedButton mNextButton;
    private LocalizedButton mPreviousButton;
    private LocalizedButton mStartNextButton;

    #endregion


    #region "Properties"

    /// <summary>
    /// Database is created.
    /// </summary>
    private bool DBCreated
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DBCreated"], false);
        }
    }


    /// <summary>
    /// Database is installed.
    /// </summary>
    private bool DBInstalled
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DBInstalled"], false);
        }
        set
        {
            ViewState["DBInstalled"] = value;
        }
    }


    /// <summary>
    /// Process GUID.
    /// </summary>
    private Guid ProcessGUID
    {
        get
        {
            if (ViewState["ProcessGUID"] == null)
            {
                ViewState["ProcessGUID"] = Guid.NewGuid();
            }

            return ValidationHelper.GetGuid(ViewState["ProcessGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Install info.
    /// </summary>
    private InstallInfo Info
    {
        get
        {
            string key = "instInfos_" + ProcessGUID;

            return mInstallInfos[key] ?? (mInstallInfos[key] = new InstallInfo());
        }
    }


    /// <summary>
    /// Authentication type.
    /// </summary>
    private SQLServerAuthenticationModeEnum AuthenticationType
    {
        get
        {
            if ((ViewState["authentication"] == null) && RequestHelper.IsPostBack())
            {
                throw new InvalidOperationException("Connection information was lost!");
            }

            return (SQLServerAuthenticationModeEnum)ViewState["authentication"];
        }
        set
        {
            ViewState["authentication"] = value;
        }
    }


    /// <summary>
    /// Database name.
    /// </summary>
    private string Database
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Database"], String.Empty);
        }
        set
        {
            ViewState["Database"] = value;
        }
    }


    /// <summary>
    /// New site name.
    /// </summary>
    private string SiteName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SiteName"], String.Empty);
        }
    }


    /// <summary>
    /// Connection string.
    /// </summary>
    private string ConnectionString
    {
        get
        {
            if (ViewState["connString"] == null)
            {
                ViewState["connString"] = String.Empty;
            }
            return (string)ViewState["connString"];
        }
        set
        {
            ViewState["connString"] = value;
        }
    }


    /// <summary>
    /// Step index.
    /// </summary>
    private int StepIndex
    {
        get
        {
            if (ViewState["stepIndex"] == null)
            {
                ViewState["stepIndex"] = 1;
            }
            return (int)ViewState["stepIndex"];
        }
        set
        {
            ViewState["stepIndex"] = value;
        }
    }


    private string Result
    {
        get
        {
            if (ViewState["result"] == null)
            {
                if (RequestHelper.IsPostBack())
                {
                    throw new Exception("Information was lost!");
                }
            }
            return (string)ViewState["result"];
        }
        set
        {
            ViewState["result"] = value;
        }
    }


    private bool DisplayLog
    {
        get
        {
            if (ViewState["displLog"] == null)
            {
                if (RequestHelper.IsPostBack())
                {
                    throw new Exception("Information was lost!");
                }
                return false;
            }
            return (bool)ViewState["displLog"];
        }
        set
        {
            ViewState["displLog"] = value;
        }
    }


    /// <summary>
    /// Flag - indicate whether DB objects will be created.
    /// </summary>
    private bool CreateDBObjects
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["CreateDBObjects"], true);
        }
        set
        {
            ViewState["CreateDBObjects"] = value;
        }
    }


    /// <summary>
    /// Help control displayed on the navigation for the first step.
    /// </summary>
    protected HelpControl StartHelp
    {
        get
        {
            Control startStepNavigation = wzdInstaller.FindControl("StartNavigationTemplateContainerID$startStepNavigation");
            return (HelpControl)startStepNavigation.FindControl("hlpContext");
        }
    }


    /// <summary>
    /// Help control displayed on the navigation for all remaining steps.
    /// </summary>
    protected HelpControl Help
    {
        get
        {
            Control stepNavigation = wzdInstaller.FindControl("StepNavigationTemplateContainerID$stepNavigation");
            return (HelpControl)stepNavigation.FindControl("hlpContext");
        }
    }


    /// <summary>
    /// Previous step index.
    /// </summary>
    private int PreviousStep
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["PreviousStep"], 0);
        }
        set
        {
            ViewState["PreviousStep"] = value;
        }
    }

    /// <summary>
    /// Current step index.
    /// </summary>
    private int ActualStep
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ActualStep"], 0);
        }
        set
        {
            ViewState["ActualStep"] = value;
        }
    }


    private int StepOperation
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["StepOperation"], 0);
        }
        set
        {
            ViewState["StepOperation"] = value;
        }
    }


    /// <summary>
    ///  User password.
    /// </summary>
    private string Password
    {
        get
        {
            return Convert.ToString(ViewState["install.password"]);
        }
        set
        {
            ViewState["install.password"] = value;
        }
    }

    #endregion


    #region "Step wizard buttons"

    /// <summary>
    /// Previous button.
    /// </summary>
    private LocalizedButton PreviousButton
    {
        get
        {
            return mPreviousButton ?? (mPreviousButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepPrevButton") as LocalizedButton);
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    private LocalizedButton NextButton
    {
        get
        {
            return mNextButton ?? (mNextButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepNextButton") as LocalizedButton);
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    private LocalizedButton StartNextButton
    {
        get
        {
            return mStartNextButton ?? (mStartNextButton = wzdInstaller.FindControl("StartNavigationTemplateContainerID").FindControl("startStepNavigation").FindControl("StepNextButton") as LocalizedButton);
        }
    }

    #endregion


    #region "Methods"

    private void ValidatePostRequest()
    {
        if (RequestHelper.IsPostBack())
        {
            bool isValidPostRequest;
            try
            {
                var cookieValue = CookieHelper.GetValue(INSTALL_CHECK_COOKIE_NAME);
                var value = MachineKey.Unprotect(Convert.FromBase64String(cookieValue), INSTALL_CHECK_PURPOSE);
                isValidPostRequest = INSTALL_CHECK_PAYLOAD.Equals(Encoding.UTF8.GetString(value), StringComparison.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new SecurityException(INSTALL_CHECK_EXCEPTION_MESSAGE, ex);
            }

            if (!isValidPostRequest)
            {
                throw new SecurityException(INSTALL_CHECK_EXCEPTION_MESSAGE);
            }
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ValidatePostRequest();

        // Disable CSS minification
        CssLinkHelper.MinifyCurrentRequest = false;
        ScriptHelper.MinifyCurrentRequestScripts = false;

        SetBrowserClass(false);

        if (!RequestHelper.IsCallback())
        {
            EnsureApplicationConfiguration();

            ctlAsyncDB.OnFinished += workerDB_OnFinished;
            databaseDialog.ServerName = userServer.ServerName;

            // Register script for pendingCallbacks repair
            // Cannot use ScriptHelper.FixPendingCallbacks as during installation the DB is not available
            ScriptManager.RegisterClientScriptInclude(this, GetType(), "cms.js", UrlResolver.ResolveUrl("~/CMSScripts/cms.js"));
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "fixPendingCallbacks", "WebForm_CallbackComplete = WebForm_CallbackComplete_SyncFixed", true);

            // Javascript functions
            string script = $@"
function Finished(sender) {{
    var errorElement = document.getElementById('{pnlError.ClientID}');

    var errorText = sender.getErrors();
    if (errorText != '') {{
        errorElement.innerHTML = errorText;
    }}

    var warningElement = document.getElementById('{pnlWarning.ClientID}');

    var warningText = sender.getWarnings();
    if (warningText != '') {{
        warningElement.innerHTML = warningText;
    }}

    var actDiv = document.getElementById('actDiv');
    if (actDiv != null) {{
        actDiv.style.display = 'none';
    }}

    BTN_Disable('{PreviousButton.ClientID}');
    BTN_Enable('{NextButton.ClientID}');
}}";

            // Register the script to perform get flags for showing buttons retrieval callback
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "InstallFunctions", ScriptHelper.GetScript(script));

            StartHelp.Tooltip = ResHelper.GetFileString("install.tooltip");
            StartHelp.TopicName = HELP_TOPIC_LINK;
            StartHelp.IconCssClass = "cms-icon-80";

            Response.Cache.SetNoStore();

            Help.Tooltip = ResHelper.GetFileString("install.tooltip");
            Help.IconCssClass = "cms-icon-80";

            btnPermissionTest.Click += btnPermissionTest_Click;
            btnPermissionSkip.Click += btnPermissionSkip_Click;
            btnPermissionContinue.Click += btnPermissionContinue_Click;

            // If the connection string is set, redirect
            if (!RequestHelper.IsPostBack())
            {
                if (ConnectionHelper.ConnectionAvailable)
                {
                    URLHelper.Redirect("~/default.aspx");
                }

                var protectedValue = MachineKey.Protect(Encoding.UTF8.GetBytes(INSTALL_CHECK_PAYLOAD), INSTALL_CHECK_PURPOSE);
                CookieHelper.SetValue(INSTALL_CHECK_COOKIE_NAME, Convert.ToBase64String(protectedValue), DateTime.MinValue);

                bool checkPermission = QueryHelper.GetBoolean("checkpermission", true);
                bool testAgain = QueryHelper.GetBoolean("testagain", false);

                string dir = HttpContext.Current.Server.MapPath("~/");

                // Do not test write permissions in WWAG mode
                if (!ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()[WWAG_KEY], false))
                {
                    if (!DirectoryHelper.CheckPermissions(dir) && checkPermission)
                    {
                        writePermissions = false;
                        pnlWizard.Visible = false;
                        pnlHeaderImages.Visible = false;
                        pnlPermission.Visible = true;
                        pnlButtons.Visible = true;

                        lblPermission.Text = String.Format(ResHelper.GetFileString("Install.lblPermission"), WindowsIdentity.GetCurrent().Name, dir);
                        btnPermissionSkip.Text = ResHelper.GetFileString("Install.btnPermissionSkip");
                        btnPermissionTest.Text = ResHelper.GetFileString("Install.btnPermissionTest");

                        // Show troubleshoot link
                        pnlError.DisplayError("Install.ErrorPermissions", HELP_TOPIC_DISK_PERMISSIONS_LINK);
                        return;
                    }

                    if (testAgain)
                    {
                        pnlWizard.Visible = false;
                        pnlHeaderImages.Visible = false;
                        pnlPermission.Visible = false;
                        pnlButtons.Visible = false;
                        pnlPermissionSuccess.Visible = true;
                        lblPermissionSuccess.Text = ResHelper.GetFileString("Install.lblPermissionSuccess");
                        btnPermissionContinue.Text = ResHelper.GetFileString("Install.btnPermissionContinue");
                        writePermissions = true;
                        return;
                    }
                }
            }

            pnlWizard.Visible = true;
            pnlPermission.Visible = false;
            pnlButtons.Visible = false;

            if (!RequestHelper.IsPostBack())
            {
                if ((HttpContext.Current != null) && !ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()[WWAG_KEY], false))
                {
                    userServer.ServerName = SystemContext.MachineName;
                }
                AuthenticationType = SQLServerAuthenticationModeEnum.SQLServerAuthentication;

                wzdInstaller.ActiveStepIndex = 0;
            }
            else if (Password == null)
            {
                Password = userServer.DBPassword;
            }

            // Load the strings
            DisplayLog = false;

            lblCompleted.Text = ResHelper.GetFileString("Install.DBSetupOK");
            lblMediumTrustInfo.Text = ResHelper.GetFileString("Install.MediumTrustInfo");

            ltlScript.Text = ScriptHelper.GetScript($@"
function NextStep(btnNext,elementDiv) {{
    btnNext.disabled=true;
    try {{
        BTN_Disable('{PreviousButton.ClientID}');
    }} catch(err) {{}}
    {ClientScript.GetPostBackEventReference(btnHiddenNext, null)}
}}

function PrevStep(btnPrev,elementDiv) {{
    btnPrev.disabled=true;
    try {{
        BTN_Disable('{NextButton.ClientID}');
    }} catch(err) {{}}
    {ClientScript.GetPostBackEventReference(btnHiddenBack, null)}
}}");
            Result = String.Empty;

            // Sets connection string panel
            lblConnectionString.Text = ResHelper.GetFileString("Install.lblConnectionString");
            wzdInstaller.StartNextButtonText = ResHelper.GetFileString("general.next") + " >";
            wzdInstaller.FinishCompleteButtonText = ResHelper.GetFileString("Install.Finish");
            wzdInstaller.FinishPreviousButtonText = ResHelper.GetFileString("Install.BackStep");
            wzdInstaller.StepNextButtonText = ResHelper.GetFileString("general.next") + " >";
            wzdInstaller.StepPreviousButtonText = ResHelper.GetFileString("Install.BackStep");

            // Show WWAG dialog instead of license dialog (if running in WWAG mode)
            if (ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()[WWAG_KEY], false))
            {
                ucLicenseDialog.Visible = false;
                ucWagDialog.Visible = true;
            }
        }

        // Set the active step as 1 if connection string already initialized
        if (!RequestHelper.IsPostBack() && ConnectionHelper.IsConnectionStringInitialized)
        {
            wzdInstaller.ActiveStepIndex = 1;
            databaseDialog.UseExistingChecked = true;
        }

        NextButton.Attributes.Remove("disabled");
        PreviousButton.Attributes.Remove("disabled");

        wzdInstaller.ActiveStepChanged += wzdInstaller_ActiveStepChanged;

        RegisterRequiredMacroFields();
    }


    private static void RegisterRequiredMacroFields()
    {
        // application is not initialized and some macros cannot be resolved
        MacroContext.GlobalResolver.SetNamedSourceData(
            new MacroField("AppPath", x => SystemContext.ApplicationPath.TrimEnd('/')), false);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display the log if result filled
        if (DisplayLog)
        {
            logPanel.DisplayLog(Result);
        }

        InitializeHeader(wzdInstaller.ActiveStepIndex);
        EnsureDefaultButton();

        PreviousButton.Visible = (!ConnectionHelper.IsConnectionStringInitialized && (wzdInstaller.ActiveStepIndex != 0) && (wzdInstaller.ActiveStepIndex != 4)) ||
            (wzdInstaller.ActiveStepIndex == 6);
    }


    private void wzdInstaller_ActiveStepChanged(object sender, EventArgs e)
    {
        switch (wzdInstaller.ActiveStepIndex)
        {
            case 1:
                break;
            // Finish step
            case 5:
                // Set current user default culture of the site
                LocalizationContext.PreferredCultureCode = SettingsKeyInfoProvider.GetValue(SiteName + ".CMSDefaultCultureCode");

                // Ensure virtual path provider registration if enabled
                VirtualPathHelper.RegisterVirtualPathProvider();

                // Check whether virtual path provider is running
                if (!VirtualPathHelper.UsingVirtualPathProvider)
                {
                    btnWebSite.Text = ResHelper.GetFileString("Install.lnkMediumTrust");
                    lblMediumTrustInfo.Visible = true;
                }
                else
                {
                    btnWebSite.Text = ResHelper.GetFileString("Install.lnkWebsite");
                }
                break;
        }
    }


    private void btnPermissionContinue_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(RequestContext.URL.GetLeftPart(UriPartial.Path));
    }


    private void btnPermissionSkip_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(RequestContext.URL.GetLeftPart(UriPartial.Path) + "?checkpermission=0");
    }


    private void btnPermissionTest_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(RequestContext.URL.GetLeftPart(UriPartial.Path) + "?testagain=1");
    }


    protected void btnWebSite_onClick(object sender, EventArgs e)
    {
        if (!VirtualPathHelper.UsingVirtualPathProvider)
        {
            AuthenticationHelper.AuthenticateUser(UserInfoProvider.AdministratorUserName, false);
            URLHelper.Redirect(ApplicationUrlHelper.GetApplicationUrl("cms", "administration"));
        }
        else
        {
            URLHelper.Redirect(ResolveUrl("~/default.aspx"));
        }
    }


    protected void btnHiddenBack_onClick(object sender, EventArgs e)
    {
        StepOperation = -1;
        if (wzdInstaller.ActiveStepIndex == COLLATION_DIALOG_INDEX || wzdInstaller.ActiveStepIndex == 3)
        {
            StepIndex = 2;
            wzdInstaller.ActiveStepIndex = 1;
        }
        else
        {
            StepIndex--;
            wzdInstaller.ActiveStepIndex--;
        }
    }


    protected void btnHiddenNext_onClick(object sender, EventArgs e)
    {
        StepOperation = 1;
        StepIndex++;

        switch (wzdInstaller.ActiveStepIndex)
        {
            case 0:
                // Set the authentication type
                AuthenticationType = userServer.WindowsAuthenticationChecked ? SQLServerAuthenticationModeEnum.WindowsAuthentication : SQLServerAuthenticationModeEnum.SQLServerAuthentication;

                // Check the server name
                if (String.IsNullOrEmpty(userServer.ServerName))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorServerEmpty"));
                    return;
                }

                // Do not allow to use empty user name or password
                bool isSQLAuthentication = AuthenticationType == SQLServerAuthenticationModeEnum.SQLServerAuthentication;
                if  (isSQLAuthentication && (String.IsNullOrEmpty(userServer.DBUsername) || String.IsNullOrEmpty(userServer.DBPassword)))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorUserNamePasswordEmpty"));
                    return;
                }

                Password = userServer.DBPassword;

                // Check if it is possible to connect to the database
                string res = ConnectionHelper.TestConnection(AuthenticationType, userServer.ServerName, String.Empty, userServer.DBUsername, Password);
                if (!String.IsNullOrEmpty(res))
                {
                    HandleError(res, "Install.ErrorSqlTroubleshoot", HELP_TOPIC_SQL_ERROR_LINK);
                    return;
                }

                // Set credentials for the next step
                databaseDialog.AuthenticationType = AuthenticationType;
                databaseDialog.Password = Password;
                databaseDialog.Username = userServer.DBUsername;
                databaseDialog.ServerName = userServer.ServerName;

                // Move to the next step
                wzdInstaller.ActiveStepIndex = 1;
                break;

            case 1:
            case COLLATION_DIALOG_INDEX:
                // Get database name
                Database = TextHelper.LimitLength(databaseDialog.CreateNewChecked ? databaseDialog.NewDatabaseName : databaseDialog.ExistingDatabaseName, 100);

                if (String.IsNullOrEmpty(Database))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorDBNameEmpty"));
                    return;
                }

                // Set up the connection string
                if (ConnectionHelper.IsConnectionStringInitialized)
                {
                    ConnectionString = ConnectionHelper.ConnectionString;
                }
                else
                {
                    ConnectionString = ConnectionHelper.BuildConnectionString(AuthenticationType, userServer.ServerName, Database, userServer.DBUsername, Password, SqlInstallationHelper.DB_CONNECTION_TIMEOUT);
                }

                // Check if existing DB has the same version as currently installed CMS
                if (databaseDialog.UseExistingChecked && !databaseDialog.CreateDatabaseObjects)
                {
                    string dbVersion = null;
                    try
                    {
                        dbVersion = SqlInstallationHelper.GetDatabaseVersion(ConnectionString);
                    }
                    catch
                    {
                    }

                    if (String.IsNullOrEmpty(dbVersion))
                    {
                        // Unable to get DB version => DB objects missing
                        HandleError(ResHelper.GetFileString("Install.DBObjectsMissing"));
                        return;
                    }

                    if (dbVersion != CMSVersion.MainVersion)
                    {
                        // Get wrong version number
                        HandleError(ResHelper.GetFileString("Install.WrongDBVersion"));
                        return;
                    }
                }

                Info.LogContext = ctlAsyncDB.LogContext;

                // Use existing database
                if (databaseDialog.UseExistingChecked)
                {
                    // Check if DB exists
                    if (!DatabaseHelper.DatabaseExists(ConnectionString))
                    {
                        HandleError(String.Format(ResHelper.GetFileString("Install.ErrorDatabseDoesntExist"), Database));
                        return;
                    }

                    // Get collation of existing DB
                    string collation = DatabaseHelper.GetDatabaseCollation(ConnectionString);
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
                            DatabaseHelper.ChangeDatabaseCollation(ConnectionString, Database, DatabaseHelper.DEFAULT_DB_COLLATION);
                        }
                    }
                }
                else
                {
                    // Create a new database
                    if (!CreateDatabase(null))
                    {
                        HandleError(String.Format(ResHelper.GetFileString("Install.ErrorCreateDB"), databaseDialog.NewDatabaseName));
                        return;
                    }

                    databaseDialog.ExistingDatabaseName = databaseDialog.NewDatabaseName;
                    databaseDialog.CreateNewChecked = false;
                    databaseDialog.UseExistingChecked = true;
                }

                if ((!SystemContext.IsRunningOnAzure && writePermissions) || ConnectionHelper.IsConnectionStringInitialized)
                {
                    if (databaseDialog.CreateDatabaseObjects)
                    {
                        if (DBInstalled && DBCreated)
                        {
                            ctlAsyncDB.RaiseFinished(this, EventArgs.Empty);
                        }
                        else
                        {
                            // Run SQL installation
                            RunSQLInstallation();
                        }
                    }
                    else
                    {
                        CreateDBObjects = false;

                        // Set connection string
                        if (SettingsHelper.SetConnectionString(ConnectionHelper.ConnectionStringName, ConnectionString))
                        {
                            // Set the application connection string
                            SetAppConnectionString();

                            // Check if license key for current domain is present
                            LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(hostName);
                            wzdInstaller.ActiveStepIndex = (lki == null) ? 4 : 5;
                            ucLicenseDialog.SetLicenseExpired();
                        }
                        else
                        {
                            ManualConnectionStringInsertion();
                        }
                    }
                }
                else
                {
                    ManualConnectionStringInsertion();
                }

                break;

            // After connection string save error
            case 2:
                // Check whether connection string is defined
                if (String.IsNullOrWhiteSpace(Service.Resolve<IConnectionStringService>()[ConnectionHelper.ConnectionStringName]))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorAddConnString"));
                    return;
                }

                ConnectionString = Service.Resolve<IConnectionStringService>()[ConnectionHelper.ConnectionStringName];

                if (CreateDBObjects)
                {
                    if (DBInstalled)
                    {
                        FinalizeDBInstallation();
                    }
                    else
                    {
                        // Run SQL installation
                        RunSQLInstallation();
                    }
                }
                else
                {
                    // If this is installation to existing DB and objects are not created
                    if ((hostName != "localhost") && (hostName != "127.0.0.1"))
                    {
                        wzdInstaller.ActiveStepIndex = 4;
                    }
                    else
                    {
                        wzdInstaller.ActiveStepIndex = 5;
                    }
                }
                break;

            // After DB install
            case 3:
                break;

            // After license entering
            case 4:
                try
                {
                    if (ucLicenseDialog.Visible)
                    {
                        ucLicenseDialog.SetLicenseKey();
                        wzdInstaller.ActiveStepIndex = 5;
                    }
                    else if (ucWagDialog.ProcessRegistration(ConnectionString))
                    {
                        wzdInstaller.ActiveStepIndex = 5;
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex.Message);
                }
                break;

            default:
                wzdInstaller.ActiveStepIndex++;
                break;
        }
    }


    /// <summary>
    /// Runs SQL installation scripts
    /// </summary>
    private void RunSQLInstallation()
    {
        // Setup the installation
        var info = Info;

        info.ScriptsFullPath = SqlInstallationHelper.GetSQLInstallPath();
        info.ConnectionString = ConnectionString;

        info.ClearLog();

        // Start the installation process
        ctlAsyncDB.RunAsync(InstallDatabase, WindowsIdentity.GetCurrent());

        NextButton.Attributes.Add("disabled", "true");
        PreviousButton.Attributes.Add("disabled", "true");
        wzdInstaller.ActiveStepIndex = 3;
    }


    private void workerDB_OnFinished(object sender, EventArgs e)
    {
        CreateDBObjects = databaseDialog.CreateDatabaseObjects;

        DBInstalled = true;

        // Try to set connection string into db only if not running on Azure
        bool setConnectionString = !SystemContext.IsRunningOnAzure && writePermissions;

        // Connection string could not be saved to web.config
        if (!ConnectionHelper.IsConnectionStringInitialized && (!setConnectionString || !SettingsHelper.SetConnectionString(ConnectionHelper.ConnectionStringName, ConnectionString)))
        {
            ManualConnectionStringInsertion();
            return;
        }

        FinalizeDBInstallation();
    }


    /// <summary>
    /// Sets connection string, inits application and finalizes the database data (macro signatures, time zones, license).
    /// </summary>
    private void FinalizeDBInstallation()
    {
        SetAppConnectionString();

        UpdateMacroSignatures();

        // Recalculate time zone daylight saving start and end.
        TimeZoneInfoProvider.GenerateTimeZoneRules();

        CheckLicense();
    }


    /// <summary>
    /// Check if license for current domain is valid. Try to add trial license if possible.
    /// </summary>
    private void CheckLicense()
    {
        // Try to add trial license
        if (CreateDBObjects)
        {
            if (AddTrialLicenseKeys())
            {
                if (hostName != "localhost" && hostName != "127.0.0.1")
                {
                    // Check if license key for current domain is present
                    LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(hostName);
                    wzdInstaller.ActiveStepIndex = (lki == null) ? 4 : 5;
                }
                else
                {
                    wzdInstaller.ActiveStepIndex = 5;
                }
            }
            else
            {
                wzdInstaller.ActiveStepIndex = 4;
                ucLicenseDialog.SetLicenseExpired();
            }
        }
    }


    /// <summary>
    /// Sets step, that prompts user to enter connection string manually to web.config. ConnectionString is built inside the method.
    /// </summary>
    private void ManualConnectionStringInsertion()
    {
        var encodedPassword = HttpUtility.HtmlEncode(HttpUtility.HtmlEncode(Password));
        var connectionString = ConnectionHelper.BuildConnectionString(AuthenticationType, userServer.ServerName, Database, userServer.DBUsername, encodedPassword, SqlInstallationHelper.DB_CONNECTION_TIMEOUT, isForAzure: SystemContext.IsRunningOnAzure);

        // Set error message
        var connectionStringEntry = $"&lt;add name=\"CMSConnectionString\" connectionString=\"{connectionString}\"/&gt;";
        var applicationSettingsEntry = $"&lt;Setting name=\"CMSConnectionString\" value=\"{connectionString}\"/&gt;";

        lblErrorConnMessage.Text = SystemContext.IsRunningOnAzure
            ? String.Format(ResHelper.GetFileString("Install.ConnectionStringAzure"), connectionStringEntry, applicationSettingsEntry)
            : String.Format(ResHelper.GetFileString("Install.ConnectionStringError"), connectionStringEntry);

        // Set step that prompts user to enter connection string to web.config
        wzdInstaller.ActiveStepIndex = 2;

        if (!SystemContext.IsRunningOnAzure)
        {
            // Show troubleshoot link
            pnlError.DisplayError("Install.ErrorPermissions", HELP_TOPIC_DISK_PERMISSIONS_LINK);
        }
    }


    /// <summary>
    /// Sets the application connection string and initializes the application.
    /// </summary>
    private void SetAppConnectionString()
    {
        ConnectionHelper.ConnectionString = ConnectionString;

        // Init core
        CMSApplication.Init();
    }


    /// <summary>
    /// Ensures required web.config keys.
    /// </summary>
    private void EnsureApplicationConfiguration()
    {
        // Ensure hash salt in web.config
        if (String.IsNullOrEmpty(ValidationHelper.GetDefaultHashStringSalt()))
        {
            SettingsHelper.SetConfigValue(ValidationHelper.APP_SETTINGS_HASH_STRING_SALT, Guid.NewGuid().ToString());
        }

        // Ensure application GUID in web.config
        if (String.IsNullOrEmpty(Service.Resolve<IAppSettingsService>()[SystemHelper.APP_GUID_KEY_NAME]))
        {
            SettingsHelper.SetConfigValue(SystemHelper.APP_GUID_KEY_NAME, Guid.NewGuid().ToString());
        }
    }


    protected void wzdInstaller_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
        --StepIndex;
        wzdInstaller.ActiveStepIndex--;
    }


    /// <summary>
    /// Adds trial license keys to DB. No license is added when running in web application gallery mode.
    /// </summary>
    private bool AddTrialLicenseKeys()
    {
        // Skip creation of trial license keys if running in WWAG mode
        if (ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()[WWAG_KEY], false))
        {
            return false;
        }

        string licenseKey = ValidationHelper.GetString(Service.Resolve<IAppSettingsService>()["CMSTrialKey"], String.Empty);
        if (licenseKey != String.Empty)
        {
            return LicenseHelper.AddTrialLicenseKeys(licenseKey, true, false);
        }

        pnlError.ErrorLabelText = ResHelper.GetFileString("Install.ErrorTrialLicense");

        return false;
    }


    /// <summary>
    /// Initialize wizard header
    /// </summary>
    /// <param name="index">Step index</param>
    private void InitializeHeader(int index)
    {
        Help.Visible = true;
        StartHelp.Visible = true;
        StartHelp.TopicName = Help.TopicName = HELP_TOPIC_LINK;

        lblHeader.Text = $"{ResHelper.GetFileString("Install.Step")} - ";

        string[] stepIcons =
        {
            " icon-cogwheel",
            " icon-database",
            " icon-check-circle icon-style-allow"
        };

        string[] stepTitles =
        {
            ResHelper.GetFileString("install.sqlsetting"),
            ResHelper.GetFileString("install.lbldatabase"),
            ResHelper.GetFileString("install.finishstep")
        };

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
            // SQL server and authentication mode
            case 0:
                lblHeader.Text += ResHelper.GetFileString("Install.Step0");
                SetSelectedCSSClass("stepPanel0");
                break;

            // Database
            case 1:
                lblHeader.Text += ResHelper.GetFileString("Install.Step1");
                SetSelectedCSSClass("stepPanel1");
                break;

            // web.config permissions
            case 2:
                StartHelp.Visible = Help.Visible = false;
                lblHeader.Text += ResHelper.GetFileString("Install.Step3");
                SetSelectedCSSClass("stepPanel1");
                break;

            // Database creation log
            case 3:
                StartHelp.Visible = Help.Visible = false;
                lblHeader.Text += ResHelper.GetFileString("Install.Step2");
                lblDBProgress.Text = ResHelper.GetFileString("Install.lblDBProgress");
                SetSelectedCSSClass("stepPanel1");
                break;

            // License import
            case 4:
                lblHeader.Text += ResHelper.GetFileString("Install.Step4");
                SetSelectedCSSClass("stepPanel1");
                break;

            // Finish step
            case 5:
                lblHeader.Text += ResHelper.GetFileString("Install.Step5");
                SetSelectedCSSClass("stepPanel2");
                break;

            case COLLATION_DIALOG_INDEX:
                lblHeader.Text += ResHelper.GetFileString("Install.Step6");
                SetSelectedCSSClass("stepPanel1");
                break;
        }

        // Calculate step number
        if (PreviousStep == index)
        {
            StepOperation = 0;
        }
        ActualStep += StepOperation;
        lblHeader.Text = String.Format(lblHeader.Text, ActualStep + 1);
        PreviousStep = index;
    }


    private void SetSelectedCSSClass(string stepPanel)
    {
        var selectedPanel = pnlHeaderImages.FindControl(stepPanel) as Panel;
        selectedPanel.CssClass += " install-step-icon-selected";
    }


    private void EnsureDefaultButton()
    {
        if (wzdInstaller.ActiveStep != null)
        {
            Page.Form.DefaultButton = (wzdInstaller.ActiveStep.StepType == WizardStepType.Start) ? StartNextButton.UniqueID : NextButton.UniqueID;
        }
    }

    #endregion


    #region "Installation methods"

    private bool CreateDatabase(string collation)
    {
        try
        {
            var message = $"{ResHelper.GetFileString("Installer.LogCreatingDatabase")} {databaseDialog.NewDatabaseName}";
            AddResult(message);
            LogProgressState(LogStatusEnum.Info, message);

            var connectionString = ConnectionHelper.BuildConnectionString(AuthenticationType, userServer.ServerName, String.Empty, userServer.DBUsername, Password, SqlInstallationHelper.DB_CONNECTION_TIMEOUT);

            // Use default collation, if none specified
            if (String.IsNullOrEmpty(collation))
            {
                collation = DatabaseHelper.DatabaseCollation;
            }

            if (!DBCreated)
            {
                SqlInstallationHelper.CreateDatabase(databaseDialog.NewDatabaseName, connectionString, collation);
            }

            return true;
        }
        catch (Exception ex)
        {
            DisplayLog = true;
            var message = $"{ResHelper.GetFileString("Intaller.LogErrorCreateDB")} {ex.Message}";
            AddResult(message);
            LogProgressState(LogStatusEnum.Error, message);
        }

        return false;
    }


    /// <summary>
    /// Logs message to install log.
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="type">Type of message ("E" - error, "I" - info)</param>
    private void Log(string message, MessageTypeEnum type)
    {
        AddResult(message);
        switch (type)
        {
            case MessageTypeEnum.Error:
                LogProgressState(LogStatusEnum.Error, message);
                break;

            case MessageTypeEnum.Info:
                LogProgressState(LogStatusEnum.Info, message);
                break;
        }
    }


    /// <summary>
    /// Installs database (table structure + default data).
    /// </summary>
    /// <param name="parameter">Async action param</param>
    private void InstallDatabase(object parameter)
    {
        if (!DBInstalled)
        {
            TryResetUninstallationTokens();

            var settings = new DatabaseInstallationSettings
            {
                ConnectionString = Info.ConnectionString,
                ScriptsFolder = Info.ScriptsFullPath,
                ApplyHotfix = true,
                DatabaseObjectInstallationErrorMessage = ResHelper.GetFileString("Installer.LogErrorCreateDBObjects"),
                DataInstallationErrorMessage = ResHelper.GetFileString("Installer.LogErrorDefaultData"),
                Logger = Log
            };
            bool success = SqlInstallationHelper.InstallDatabase(settings);

            if (success)
            {
                LogProgressState(LogStatusEnum.Finish, ResHelper.GetFileString("Installer.DBInstallFinished"));
            }
            else
            {
                throw new Exception("[InstallDatabase]: Error during database creation.");
            }
        }
    }


    /// <summary>
    /// Tries to reset uninstallation tokens of installable modules.
    /// Logs the result into event log.
    /// </summary>
    private static void TryResetUninstallationTokens()
    {
        try
        {
            ModulesModule.ResetUninstallationTokensOfInstallableModules();
            Service.Resolve<IEventLogService>().LogInformation("Installation", "RESETUNINSTALLATIONTOKENS", "Uninstallation tokens of installable modules have been automatically reset due to database installation.");
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogError("Installation", "RESETUNINSTALLATIONTOKENS", String.Format("Reset of uninstallation tokens of installable modules has failed. The modules can not be installed again until their tokens are reset. To recover from such state, uninstall the modules and run the instance so it can recover (i.e. remove the uninstallation tokens for modules during application startup). Then install the modules again. {0}", ex));
        }
    }


    /// <summary>
    /// Signs all macros using global administrator identity.
    /// </summary>
    private static void UpdateMacroSignatures()
    {
        var eventLogService = Service.Resolve<IEventLogService>();

        void logErrorAction(string message)
        {
            using (new CMSActionContext { LogEvents = true })
            {
                eventLogService.LogError("Macros - Refresh security parameters", "ERROR", message);
            }
        }

        MacroSecurityInstallationHelper.UpdateAllMacroSignatures(logErrorAction);
    }

    #endregion


    #region "Error handling methods"

    protected void HandleError(string message)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        pnlError.ErrorLabelText = message;
    }


    protected void HandleError(string message, string resourceString, string topic)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        pnlError.ErrorLabelText = message;
        pnlError.DisplayError(resourceString, topic);
    }

    #endregion


    #region "Logging methods"

    /// <summary>
    /// Appends the result string to the result message.
    /// </summary>
    /// <param name="result">String to append</param>
    private void AddResult(string result)
    {
        Result = $"{result}\n{Result}";
    }


    /// <summary>
    /// Logs progress state.
    /// </summary>
    /// <param name="type">Type of the message</param>
    /// <param name="message">Message to be logged</param>
    private void LogProgressState(LogStatusEnum type, string message)
    {
        message = HTMLHelper.HTMLEncode(message);

        string logMessage = null;
        string messageType = null;

        switch (type)
        {
            case LogStatusEnum.Info:
            case LogStatusEnum.Start:
                logMessage = message;
                break;

            case LogStatusEnum.Error:
                {
                    messageType = "##ERROR##";
                    logMessage = "<strong>" + ResHelper.GetFileString("Global.ErrorSign", "ERROR:") + "&nbsp;</strong>" + message;
                }
                break;

            case LogStatusEnum.Warning:
                {
                    messageType = "##WARNING##";
                    logMessage = "<strong>" + ResHelper.GetFileString("Global.Warning", "WARNING:") + "&nbsp;</strong>" + message;
                }
                break;

            case LogStatusEnum.Finish:
            case LogStatusEnum.UnexpectedFinish:
                logMessage = "<strong>" + message + "</strong>";
                break;
        }

        logMessage = messageType + logMessage;

        // Log to context
        Info.LogContext.AppendText(logMessage);
    }

    #endregion
}
