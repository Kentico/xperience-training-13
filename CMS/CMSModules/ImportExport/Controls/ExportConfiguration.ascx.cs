using System;
using System.Data;

using CMS.Base;

using System.Text;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_ExportConfiguration : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessages;
        }
    }


    /// <summary>
    /// Export settings.
    /// </summary>
    public SiteExportSettings Settings
    {
        get;
        set;
    }


    /// <summary>
    /// Site ID.
    /// </summary>
    public int SiteId
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["SiteID"], 0);
        }
        set
        {
            ViewState["SiteID"] = value;
        }
    }


    /// <summary>
    /// Export history.
    /// </summary>
    public bool ExportHistory
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            if (!RequestHelper.IsCallback())
            {
                siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

                radAll.Text = GetString("ExportConfiguration.All");
                radDate.Text = GetString("ExportConfiguration.Date");
                radExport.Text = GetString("ExportConfiguration.Export");
                radNone.Text = GetString("ExportConfiguration.None");

                // Client script for setting controls
                StringBuilder script = new StringBuilder();
                script.AppendLine("function SetupControls()");
                script.AppendLine("{");
                script.AppendLine(" var listBoxElem = document.getElementById('" + lstExports.ClientID + "');");
                script.AppendLine(" var exportRadElem = document.getElementById('" + radExport.ClientID + "');");
                script.AppendLine(" var dateRadElem = document.getElementById('" + radDate.ClientID + "');");
                script.AppendLine(" if((exportRadElem != null) && (listBoxElem != null))");
                script.AppendLine(" {");
                script.AppendLine("     listBoxElem.disabled = exportRadElem.checked ? '' : 'disabled';");
                script.AppendLine(" }");
                script.AppendLine(" if(dateRadElem != null)");
                script.AppendLine(" {");
                script.AppendLine("     SetEnabled_" + dtDate.ClientID + "(dateRadElem.checked);");
                script.AppendLine(" }");
                script.AppendLine("}");

                ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "setupHistory", ScriptHelper.GetScript(script.ToString()));
                ScriptHelper.RegisterStartupScript(Page, typeof(string), "setupControlsOnLoad", ScriptHelper.GetScript("SetupControls();"));

                radAll.Attributes.Add("onclick", "SetupControls(this);");
                radDate.Attributes.Add("onclick", "SetupControls(this);");
                radExport.Attributes.Add("onclick", "SetupControls(this);");
                radNone.Attributes.Add("onclick", "SetupControls(this);");

                // Add additional option for global objects export
                plcNone.Visible = (SiteId == 0);

                // Load sites list
                LoadSites();

                // Load export histories list
                if (!RequestHelper.IsPostBack())
                {
                    LoadExportHistories();

                    // Select default option
                    SetDefaultOption();
                }
            }
        }
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        LoadExportHistories();

        // Display additional option for global objects export
        SiteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        plcNone.Visible = (SiteId == 0);

        // Select default option
        SetDefaultOption();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads export histories list.
    /// </summary> 
    private void LoadExportHistories()
    {
        lstExports.Items.Clear();
        var where = new WhereCondition();

        int siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        if (siteId != 0)
        {
            where.Where("ExportSiteID", QueryOperator.Equals ,siteSelector.Value);
        }
        else
        {
            where.WhereNull("ExportSiteID");
        }

        var histories = ExportHistoryInfoProvider.GetExportHistories()
            .Where(where)
            .OrderByDescending("ExportDateTime")
            .Columns(new [] { "ExportDateTime", "ExportFileName", "ExportID" });

        if (DataHelper.DataSourceIsEmpty(histories))
        {
            return;
        }

        radExport.Enabled = true;
        foreach (DataRow dr in histories.Tables[0].Rows)
        {
            lstExports.Items.Add(new ListItem(ValidationHelper.GetString(dr["ExportDateTime"], "yyyy-mm-ddd") + " - " + ValidationHelper.GetString(dr["ExportFileName"], "filename"), ValidationHelper.GetString(dr["ExportID"], null)));
        }
    }


    private void SetDefaultOption()
    {
        if (SiteId == 0)
        {
            radNone.Checked = true;
            radAll.Checked = false;
        }
        else
        {
            radNone.Checked = false;
            radAll.Checked = true;
        }
    }


    /// <summary>
    /// Loads sites list.
    /// </summary> 
    private void LoadSites()
    {
        // Set site selector
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.AllowAll = false;
        siteSelector.UniSelector.SpecialFields.Add(new SpecialField { Text = GetString("ExportConfiguration.NoSite"), Value = "0" });

        if (!RequestHelper.IsPostBack())
        {
            if (SiteId != 0)
            {
                siteSelector.Value = SiteId;
                siteSelector.Enabled = false;
            }
            else
            {
                siteSelector.Value = "0";
            }
        }
    }


    public void InitControl()
    {
        if (String.IsNullOrEmpty(txtFileName.Text))
        {
            txtFileName.Text = ImportExportHelper.GenerateExportFileName("export");
        }
    }


    public bool ApplySettings()
    {
        txtFileName.Text = txtFileName.Text.Trim();

        // Validate the file name 
        string result = ImportExportHelper.ValidateExportFileName(Settings, txtFileName.Text);

        if (string.IsNullOrEmpty(result))
        {
            if (Path.GetExtension(txtFileName.Text).ToLowerCSafe() != ".zip")
            {
                txtFileName.Text = txtFileName.Text.TrimEnd('.') + ".zip";
            }

            // Set current user information
            Settings.CurrentUser = MembershipContext.AuthenticatedUser;

            Settings.SiteId = SiteId;
            Settings.DefaultProcessObjectType = ProcessObjectEnum.Selected;

            // Additional settings
            Settings.SetSettings(ImportExportHelper.SETTINGS_BIZFORM_DATA, true);
            Settings.SetSettings(ImportExportHelper.SETTINGS_CUSTOMTABLE_DATA, true);
            Settings.SetSettings(ImportExportHelper.SETTINGS_FORUM_POSTS, true);
            Settings.SetSettings(ImportExportHelper.SETTINGS_BOARD_MESSAGES, true);
            Settings.SetSettings(ImportExportHelper.SETTINGS_GLOBAL_FOLDERS, true);
            Settings.SetSettings(ImportExportHelper.SETTINGS_SITE_FOLDERS, true);
            Settings.SetSettings(ImportExportHelper.SETTINGS_COPY_ASPX_TEMPLATES_FOLDER, true);

            ExportTypeEnum exportType = (SiteId != 0) ? ExportTypeEnum.Site : ExportTypeEnum.All;

            // Init default values
            if (radNone.Checked)
            {
                // None objects
                Settings.TimeStamp = DateTimeHelper.ZERO_TIME;
                Settings.ExportType = ExportTypeEnum.None;
            }
            else if (radAll.Checked)
            {
                // All objects
                Settings.TimeStamp = DateTimeHelper.ZERO_TIME;
                Settings.ExportType = exportType;
            }
            else if (radDate.Checked)
            {
                if (dtDate.SelectedDateTime != DateTimeHelper.ZERO_TIME)
                {
                    if (!dtDate.IsValidRange())
                    {
                        ShowError(GetString("general.errorinvaliddatetimerange"));
                        return false;
                    }
                    else
                    {
                        // From specified date
                        Settings.TimeStamp = dtDate.SelectedDateTime;
                        Settings.ExportType = exportType;
                    }
                }
                else
                {
                    ShowError(GetString("ExportSite.SelectDateTime"));
                    return false;
                }
            }
            else
            {
                // From previous export
                int historyId = ValidationHelper.GetInteger(lstExports.SelectedValue, 0);
                if (historyId == 0)
                {
                    ShowError(GetString("ExportSite.SelectExportHistory"));
                    return false;
                }
                else
                {
                    ExportHistoryInfo history = ExportHistoryInfoProvider.GetExportHistoryInfo(historyId);
                    if (history != null)
                    {
                        // Load history settings
                        SiteExportSettings settings = new SiteExportSettings(MembershipContext.AuthenticatedUser);
                        settings.SetInfo(ImportExportHelper.INFO_HISTORY_NAME, history.ExportFileName);
                        settings.SetInfo(ImportExportHelper.INFO_HISTORY_DATE, history.ExportDateTime);
                        settings.LoadFromXML(history.ExportSettings);
                        settings.TargetPath = Settings.TargetPath;
                        settings.PersistentSettingsKey = Settings.PersistentSettingsKey;
                        Settings = settings;
                        ExportHistory = true;
                    }
                    else
                    {
                        ShowError(GetString("ExportSite.ErrorLoadingExportHistory"));
                        return false;
                    }
                }
            }

            // Keep current file name
            Settings.TargetFileName = txtFileName.Text;
        }
        else
        {
            ShowError(result);
            return false;
        }

        return true;
    }

    #endregion
}