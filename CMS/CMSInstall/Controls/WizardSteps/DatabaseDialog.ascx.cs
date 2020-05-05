using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Scheduler;
using CMS.UIControls;
using CMS.WinServiceEngine;

public partial class CMSInstall_Controls_WizardSteps_DatabaseDialog : CMSUserControl
{
    #region "Variables"

    private ISqlServerCapabilities mSqlServerCapabilities;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets new database name.
    /// </summary>
    public string NewDatabaseName
    {
        get
        {
            return txtNewDatabaseName.Text.Trim();
        }
    }


    /// <summary>
    /// Indicates if database objects should be created.
    /// </summary>
    public bool CreateDatabaseObjects
    {
        get
        {
            return chkCreateDatabaseObjects.Checked;
        }
    }


    /// <summary>
    /// Gets or sets if use existing radio button is checked.
    /// </summary>
    public bool UseExistingChecked
    {
        get
        {
            return radUseExisting.Checked;
        }
        set
        {
            radUseExisting.Checked = value;
        }
    }


    /// <summary>
    /// Indicates if create new is checked.
    /// </summary>
    public bool CreateNewChecked
    {
        get
        {
            return radCreateNew.Checked;
        }
        set
        {
            radCreateNew.Checked = value;
        }
    }


    /// <summary>
    /// Existing database name,
    /// </summary>
    public string ExistingDatabaseName
    {
        get
        {
            return txtExistingDatabaseName.Text;
        }
        set
        {
            txtExistingDatabaseName.Text = value;
        }
    }


    /// <summary>
    /// Indicates if panel for creating new database should be always displayed.
    /// </summary>
    public bool IsDBSeparation
    {
        get;
        set;
    }


    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ConnectionString"], null);
        }
        set
        {
            ViewState["ConnectionString"] = value;
        }
    }


    /// <summary>
    /// Password;
    /// </summary>
    public string Password
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Password"], null);
        }
        set
        {
            ViewState["Password"] = value;
        }
    }


    /// <summary>
    /// Server name.
    /// </summary>
    public string ServerName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ServerName"], null);
        }
        set
        {
            ViewState["ServerName"] = value;
        }
    }


    /// <summary>
    /// Authentication type.
    /// </summary>
    public SQLServerAuthenticationModeEnum AuthenticationType
    {
        get
        {
            return (SQLServerAuthenticationModeEnum)ViewState["AuthenticationType"];
        }
        set
        {
            ViewState["AuthenticationType"] = value;
        }
    }


    /// <summary>
    /// User name.
    /// </summary>
    public string Username
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Username"], null);
        }
        set
        {
            ViewState["Username"] = value;
        }
    }


    /// <summary>
    /// Database.
    /// </summary>
    public string Database
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Database"], null);
        }
        set
        {
            ViewState["Database"] = value;
        }
    }


    /// <summary>
    /// Indicates if scheduled tasks were manually stopped by a user.
    /// </summary>
    public bool TasksManuallyStopped
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["TasksManuallyStopped"], false);
        }
        set
        {
            ViewState["TasksManuallyStopped"] = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblExistingDatabaseName.Text = ResHelper.GetFileString("Install.lblExistingDatabaseName");
        lblNewDatabaseName.Text = ResHelper.GetFileString("Install.lblNewDatabaseName");
        lblDatabase.Text = IsDBSeparation ? GetString("separationDB.database") : ResHelper.GetFileString("Install.lblDatabase");
        radCreateNew.Text = ResHelper.GetFileString("Install.radCreateNew");
        radUseExisting.Text = ResHelper.GetFileString("Install.radUseExisting");
        chkCreateDatabaseObjects.Text = ResHelper.GetFileString("Install.chkCreateDatabaseObjects");
        plcRunningTasks.Visible = IsDBSeparation;
        btnStopTasks.Click += btnStopTasks_Click;
        if (IsDBSeparation)
        {
            iconHelp.ToolTip = spanScreenReader.Text = GetString("separationDB.enabledtasksseparation");
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (IsDBSeparation)
        {
            DisplayTaskStatus();
        }

        if (AzureHelper.IsSQLAzureServer(ServerName))
        {
            plcNewDB.Visible = false;
            plcExistingRadio.Visible = false;
            plcRadNew.Visible = false;
            plcEmptyLine.Visible = false;

            radCreateNew.Checked = false;
            radUseExisting.Checked = true;
        }
        else
        {
            radCreateNew.Enabled = true;
        }

        plcCreateDatabaseObjects.Visible = !IsDBSeparation;
        txtNewDatabaseName.Enabled = radCreateNew.Checked;
        txtExistingDatabaseName.Enabled = radUseExisting.Checked;
        txtExistingDatabaseName.Text = null;

        if (radCreateNew.Checked)
        {
            chkCreateDatabaseObjects.Checked = true;
            chkCreateDatabaseObjects.Enabled = false;
        }
        else
        {
            chkCreateDatabaseObjects.Enabled = true;
        }

        if (!RequestHelper.IsPostBack())
        {
            txtNewDatabaseName.Text = TextHelper.LimitLength(GetWebDirectory(), 90);

            if (ConnectionHelper.IsConnectionStringInitialized && !IsDBSeparation)
            {
                radUseExisting.Enabled = true;
                txtExistingDatabaseName.Text = DatabaseHelper.GetDatabaseName(ConnectionHelper.ConnectionString);
                txtExistingDatabaseName.Enabled = false;
                plcNewDB.Visible = false;
            }
        }
    }


    /// <summary>
    /// Gets web directory.
    /// </summary>
    private string GetWebDirectory()
    {
        string webDirectory = SystemContext.WebApplicationVirtualPath.Replace("/", "");
        if ((webDirectory == "") || (webDirectory == "~"))
        {
            webDirectory = "KenticoCMS";
        }
        return webDirectory;
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


    /// <summary>
    /// Validates control.
    /// </summary>
    public bool ValidateForSeparation()
    {
        // Get database name
        if (radCreateNew.Checked && SqlServerCapabilities.SupportsDatabaseCreation)
        {
            Database = txtNewDatabaseName.Text;
        }
        else
        {
            Database = txtExistingDatabaseName.Text;
        }
        Database = TextHelper.LimitLength(Database, 100, String.Empty);

        if (String.IsNullOrEmpty(Database))
        {
            DisplaySeparationError(GetString("Install.ErrorDBNameEmpty"));
            return false;
        }

        // Set up the connection string
        ConnectionString = ConnectionHelper.BuildConnectionString(AuthenticationType, ServerName, Database, Username, Password, 240);

        // Test existing DB
        if (radUseExisting.Checked || !SqlServerCapabilities.SupportsDatabaseCreation)
        {
            if (SqlServerCapabilities.ControlServerPermissionAvailable &&
                !DatabaseHelper.CheckDBPermission(DatabasePermission.ModifyDatabase, AuthenticationType, ServerName,Username, Password))
            {
                DisplaySeparationError(GetString("separationDB.permissionmodify"));
                return false;
            }

            if (!String.IsNullOrEmpty(ConnectionHelper.TestConnection(AuthenticationType, ServerName, Database, Username, Password)))
            {
                DisplaySeparationError(String.Format(ResHelper.GetFileString("Install.ErrorDatabseDoesntExist"), Database));
                return false;
            }

            if (DatabaseSeparationHelper.CheckCMDatabase(ConnectionString))
            {
                DisplaySeparationError(GetString("separationDB.errorCMexists"));
                return false;
            }
        }
        // Test new DB
        else
        {
            if (DatabaseHelper.DatabaseExists(ConnectionString))
            {
                DisplaySeparationError(GetString("separationDB.ErrorDBExists"));
                return false;
            }
            if (SqlServerCapabilities.ControlServerPermissionAvailable)
            {
                if (!DatabaseHelper.CheckDBPermission(DatabasePermission.ModifyDatabase, AuthenticationType, ServerName, Username, Password))
                {
                    DisplaySeparationError(GetString("separationDB.permissionmodify"));
                    return false;
                }
                if (!DatabaseHelper.CheckDBPermission(DatabasePermission.CreateDatabase, AuthenticationType, ServerName, Username, Password))
                {
                    DisplaySeparationError(GetString("separationDB.permissioncreate"));
                    return false;
                }
            }
        }

        // Test if tasks are stopped
        if (SchedulingHelper.EnableScheduler || SchedulingHelper.IsAnyTaskRunning())
        {
            DisplaySeparationError(GetString("separationDB.stoptaskserror"));
            return false;
        }
        iconHelp.Visible = btnStopTasks.Visible = true;

        // Test if separation process is not already started.
        if (DatabaseSeparationHelper.SeparationInProgress)
        {
            DisplaySeparationError(GetString("separationDB.processalreadystarted"));
            return false;
        }

        return true;
    }


    /// <summary>
    /// Checks if any tasks are running.
    /// </summary>
    private void DisplayTaskStatus()
    {
        if (SchedulingHelper.EnableScheduler || SchedulingHelper.IsAnyTaskRunning())
        {
            if (!btnStopTasks.Visible && TasksManuallyStopped && (hdnTurnedOff.Value != bool.TrueString))
            {
                DisplayStoppingTasks();
            }
            else
            {
                lblStatusValue.Text = "<span class=\"task-error\">" + GetString("general.enabled") + "</span>";
                plcRunningTasks.Visible = true;
                lblTaskStatus.Visible = true;
                lblStatusValue.Visible = true;
                ltlStatus.Visible = false;
                iconHelp.Visible = btnStopTasks.Visible = true;
            }
        }
        else
        {
            if (TasksManuallyStopped)
            {
                lblTaskStatus.Visible = true;
                lblStatusValue.Visible = true;
                plcRunningTasks.Visible = true;
                ltlStatus.Visible = false;
                ltlStatus.Text = null;
                lblStatusValue.Text = "<span class=\"task-success\">" + GetString("general.disabled") + "</span>";
                iconHelp.Visible = btnStopTasks.Visible = false;
            }
            else
            {
                plcRunningTasks.Visible = true;
                lblTaskStatus.Visible = false;
                lblStatusValue.Visible = false;
                ltlStatus.Visible = false;
                iconHelp.Visible = btnStopTasks.Visible = false;
            }
            hdnTurnedOff.Value = bool.TrueString;
        }
    }


    /// <summary>
    /// Stop tasks.
    /// </summary>
    void btnStopTasks_Click(object sender, EventArgs e)
    {
        // Stop tasks
        PersistentStorageHelper.SetValue("CMSSchedulerTasksEnabled", SettingsKeyInfoProvider.GetBoolValue("CMSSchedulerTasksEnabled"));
        if (SchedulingHelper.EnableScheduler)
        {
            SettingsKeyInfoProvider.SetGlobalValue("CMSSchedulerTasksEnabled", false);
        }

        WinServiceHelper.RestartService(WinServiceHelper.HM_SERVICE_BASENAME, false);

        // Display stopping progress
        iconHelp.Visible = btnStopTasks.Visible = false;
        DisplayStoppingTasks();
        TasksManuallyStopped = true;
    }


    /// <summary>
    /// Displays separation error.
    /// </summary>
    private void DisplaySeparationError(string error)
    {
        plcSeparationError.Visible = true;
        lblError.Text = HTMLHelper.HTMLEncode(TextHelper.LimitLength(error, 180));
        lblError.ToolTip = error;
    }


    /// <summary>
    /// Displays that tasks are being stopped.
    /// </summary>
    private void DisplayStoppingTasks()
    {
        ltlStatus.Text = ScriptHelper.GetLoaderInlineHtml(GetString("general.disabling"));
        plcRunningTasks.Visible = true;
        lblTaskStatus.Visible = true;
        lblStatusValue.Visible = true;
        ltlStatus.Visible = true;
        iconHelp.Visible = btnStopTasks.Visible = false;
    }

    #endregion
}
