using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;


public partial class CMSModules_Objects_Controls_Versioning_ViewObjectVersion : CMSUserControl, IPostBackEventHandler
{
    #region "Variables"

    private ObjectVersionHistoryInfo mVersion = null;
    private ObjectVersionHistoryInfo mVersionCompare = null;
    private TimeZoneInfo mServerTimeZone = null;
    private TimeZoneInfo mUsedTimeZone = null;
    private bool mObjectDataOnly = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if comparison is available
    /// </summary>
    public bool NoComparison
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets object version ID to display
    /// </summary>
    public int VersionID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets object version ID to compare
    /// </summary>
    public int VersionCompareID
    {
        get;
        set;
    }


    /// <summary>
    /// List of excluded table names separated by semicolon (;)
    /// </summary>
    public string ExcludedTableNames
    {
        get
        {
            return viewDataSet.ExcludedTableNames;
        }
        set
        {
            viewDataSet.ExcludedTableNames = value;
        }
    }


    /// <summary>
    /// Object version history to display
    /// </summary>
    public ObjectVersionHistoryInfo Version
    {
        get
        {
            if (mVersion == null)
            {
                if (VersionID > 0)
                {
                    mVersion = ObjectVersionHistoryInfo.Provider.Get(VersionID);
                }
            }
            return mVersion;
        }
        set
        {
            mVersion = value;
        }
    }


    /// <summary>
    /// Object version history to compare
    /// </summary>
    public ObjectVersionHistoryInfo VersionCompare
    {
        get
        {
            if (mVersionCompare == null)
            {
                if (VersionCompareID > 0)
                {
                    mVersionCompare = ObjectVersionHistoryInfo.Provider.Get(VersionCompareID);
                }
            }
            return mVersionCompare;
        }
        set
        {
            mVersionCompare = value;
        }
    }


    /// <summary>
    /// Indicates if only object data or also additional child objects data should be processed
    /// </summary>
    public bool ObjectDataOnly
    {
        get
        {
            return mObjectDataOnly;
        }
        set
        {
            mObjectDataOnly = value;
        }
    }


    /// <summary>
    /// User time zone
    /// </summary>
    private TimeZoneInfo UsedTimeZone
    {
        get
        {
            if (mUsedTimeZone == null)
            {
                mUsedTimeZone = TimeZoneHelper.GetTimeZoneInfo(MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
            }
            return mUsedTimeZone;
        }
    }


    /// <summary>
    /// Server time zone
    /// </summary>
    private TimeZoneInfo ServerTimeZone
    {
        get
        {
            if (mServerTimeZone == null)
            {
                mServerTimeZone = TimeZoneHelper.ServerTimeZone;
            }
            return mServerTimeZone;
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Page load
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Register WOpener script
        ScriptHelper.RegisterWOpenerScript(Page);

        if (QueryHelper.GetBoolean("rollbackok", false))
        {
            lblInfo.Visible = true;
            lblInfo.Text = GetString("objectversioning.rollbackOK");
        }

        // No comparing available in Recycle bin
        pnlControl.Visible = !NoComparison;

        if (Version != null)
        {
            SetupControls();
        }
        else
        {
            lblInfo.Text = GetString("objectversion.notexists");
            lblInfo.Visible = true;
            pnlControl.Visible = false;
            viewDataSet.Visible = false;
        }
    }


    /// <summary>
    /// Page pre-render
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if ((Version != null) && (VersionCompare != null))
        {
            // Prepare header with rollback controls
            TableRow tr = new TableHeaderRow() { TableSection = TableRowSection.TableHeader };
            tr.CssClass = "unigrid-head";

            TableHeaderCell th = new TableHeaderCell();
            th.Text = GetString("lock.versionnumber");
            tr.Cells.Add(th);

            // Switch header sides if necessary
            if (VersionCompare.VersionID < Version.VersionID)
            {
                tr.Cells.Add(GetRollbackTableHeaderCell("compare", VersionCompare));
                tr.Cells.Add(GetRollbackTableHeaderCell("source", Version));
            }
            else
            {
                tr.Cells.Add(GetRollbackTableHeaderCell("source", Version));
                tr.Cells.Add(GetRollbackTableHeaderCell("compare", VersionCompare));
            }

            if ((viewDataSet.DataSet.Tables.Count <= 1) && (viewDataSet.CompareDataSet.Tables.Count <= 1))
            {
                viewDataSet.Table.Rows.RemoveAt(0);
            }
            viewDataSet.Table.Rows.AddAt(0, tr);
        }
    }


    /// <summary>
    /// Setup controls
    /// </summary>
    private void SetupControls()
    {
        if (!RequestHelper.IsPostBack())
        {
            LoadDropDown();
            chkDisplayAllData.Checked = !ObjectDataOnly;
        }

        drpCompareTo.SelectedIndexChanged += drpCompareTo_SelectedIndexChanged;

        if (!NoComparison)
        {
            ObjectDataOnly = !chkDisplayAllData.Checked;
        }

        var man = SyncManager.GetInstance();
        man.OperationType = OperationTypeEnum.Versioning;

        // Get object version DataSet
        DataSet dsObject = man.GetDataSet(Version.VersionXML, TaskTypeEnum.UpdateObject, Version.VersionObjectType);
        DataSet dsCompare = null;

        // Get object compare version DataSet
        if (VersionCompare != null)
        {
            dsCompare = man.GetDataSet(VersionCompare.VersionXML, TaskTypeEnum.UpdateObject, VersionCompare.VersionObjectType);
        }

        // Filter out data if necessary
        if (ObjectDataOnly)
        {
            // Get object data table name
            GeneralizedInfo obj = ModuleManager.GetReadOnlyObject(Version.VersionObjectType);
            string objectTable = ObjectHelper.GetSerializationTableName(obj);

            dsObject = CreateTableDataSet(dsObject, objectTable, obj);
            dsCompare = CreateTableDataSet(dsCompare, objectTable, obj);
        }

        // Switch version data to ensure lower version is on the left side
        if ((Version != null) && (VersionCompare != null) && (VersionCompare.VersionID < Version.VersionID))
        {
            viewDataSet.DataSet = dsCompare;
            viewDataSet.CompareDataSet = dsObject;
        }
        else
        {
            viewDataSet.DataSet = dsObject;
            viewDataSet.CompareDataSet = dsCompare;
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Load comparison drop-down list with data
    /// </summary>
    private void LoadDropDown()
    {
        drpCompareTo.Items.Clear();

        DataSet dsVersions = ObjectVersionManager.GetObjectHistory(Version.VersionObjectType, Version.VersionObjectID, "VersionID != " + Version.VersionID, "VersionModifiedWhen DESC, VersionNumber DESC", -1, "VersionID, VersionModifiedWhen, VersionNumber");

        // Converting modified time to correct time zone
        if (!DataHelper.DataSourceIsEmpty(dsVersions))
        {
            foreach (DataRow dr in dsVersions.Tables[0].Rows)
            {
                string verId = ValidationHelper.GetString(dr["VersionID"], String.Empty);
                string verNumber = ValidationHelper.GetString(dr["VersionNumber"], String.Empty);
                DateTime verModified = ValidationHelper.GetDateTime(dr["VersionModifiedWhen"], DateTimeHelper.ZERO_TIME);
                drpCompareTo.Items.Add(new ListItem(GetVersionNumber(verNumber, verModified), verId));
            }
        }

        // If history to compare is available
        if (drpCompareTo.Items.Count > 0)
        {
            drpCompareTo.Items.Insert(0, "(select version)");
        }
        // Otherwise hide dropdown list
        else
        {
            pnlDropDown.Visible = false;
        }

        // Pre-select dropdown list
        if (VersionCompare != null)
        {
            drpCompareTo.SelectedValue = VersionCompare.VersionID.ToString();
        }
    }


    /// <summary>
    /// Get version date string in required format 
    /// </summary>
    /// <param name="dateModified">DateTime when was version modified</param>
    private string GetVersionDateString(DateTime dateModified)
    {
        CultureInfo cultureInfo = CultureHelper.GetCultureInfo(MembershipContext.AuthenticatedUser.PreferredUICultureCode);
        if (UsedTimeZone != null)
        {
            return TimeZoneHelper.ConvertTimeZoneDateTime(dateModified, ServerTimeZone, UsedTimeZone).ToString(cultureInfo) + TimeZoneHelper.GetUTCStringOffset(UsedTimeZone, " GMT{0: + 00.00; - 00.00}");
        }
        else
        {
            return dateModified.ToString(cultureInfo);
        }
    }


    /// <summary>
    /// Get version number together with version date string
    /// </summary>
    /// <param name="versionNumber">Version number</param>
    /// <param name="versionModified">Version modified DateTime</param>
    private string GetVersionNumber(string versionNumber, DateTime versionModified)
    {
        versionNumber += " (" + GetVersionDateString(versionModified) + ")";
        return versionNumber;
    }


    /// <summary>
    /// Gets new table header cell which contains label and rollback image.
    /// </summary>
    /// <param name="suffixID">ID suffix</param>
    /// <param name="objectVersion">VersionHistoryInfo object</param>
    private TableHeaderCell GetRollbackTableHeaderCell(string suffixID, ObjectVersionHistoryInfo objectVersion)
    {
        TableHeaderCell tblHeaderCell = new TableHeaderCell();

        // Label
        Label lblValue = new Label();
        lblValue.ID = "lbl" + suffixID;
        lblValue.Text = HTMLHelper.HTMLEncode(GetVersionNumber(objectVersion.VersionNumber, objectVersion.VersionModifiedWhen)) + "&nbsp;";
        tblHeaderCell.Controls.Add(lblValue);

        // Add rollback controls if user authorized to modify selected object
        if (objectVersion.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            // Rollback image
            var imgRollback = new HyperLink();
            imgRollback.ID = "imgRollback" + suffixID;
            imgRollback.CssClass = "table-header-action";
            imgRollback.NavigateUrl = "#";

            string tooltip = null;
            string confirmText = null;

            var info = ProviderHelper.GetInfoById(Version.VersionObjectType, Version.VersionObjectID);
            var rollbackEnabled = !SynchronizationHelper.IsCheckedOutByOtherUser(info);

            // Set image action and description according to roll back type
            if (chkDisplayAllData.Checked)
            {
                tooltip = GetString("objectversioning.versionlist.versionfullrollbacktooltip");
                confirmText = GetString("objectversioning.versionlist.confirmfullrollback");
            }
            else
            {
                tooltip = GetString("history.versionrollbacktooltip");
                confirmText = GetString("Unigrid.ObjectVersionHistory.Actions.Rollback.Confirmation");
            }

            imgRollback.Text = tooltip;
            imgRollback.Enabled = rollbackEnabled;

            // Prepare onclick script
            if (rollbackEnabled)
            {
                var confirmScript = "if (confirm(\"" + confirmText + "\")) { ";
                confirmScript += ControlsHelper.GetPostBackEventReference(this, objectVersion.VersionID + "|" + chkDisplayAllData.Checked) + "; } return false;";
                imgRollback.Attributes.Add("onclick", confirmScript);
            }

            tblHeaderCell.Controls.Add(imgRollback);
        }

        return tblHeaderCell;
    }


    /// <summary>
    /// Create new DataSet from table with specified table name
    /// </summary>
    /// <param name="sourceDs">Source DataSet</param>
    /// <param name="tableName">Main table name</param>
    /// <param name="obj">Object which data contains DataSet</param>
    /// <returns>Result DataSet</returns>
    private DataSet CreateTableDataSet(DataSet sourceDs, string tableName, GeneralizedInfo obj)
    {
        if (!DataHelper.DataSourceIsEmpty(sourceDs))
        {
            DataTable dt = sourceDs.Tables[tableName];

            DataSet dsResult = new DataSet();
            if (!DataHelper.DataSourceIsEmpty(dt))
            {
                dsResult.Tables.Add(dt.Copy());
                return dsResult;
            }
        }

        return null;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Dropdown list selection changed
    /// </summary>
    private void drpCompareTo_SelectedIndexChanged(object sender, EventArgs e)
    {
        string url = RequestContext.CurrentURL;

        url = URLHelper.RemoveParameterFromUrl(url, "rollbackok");

        if (drpCompareTo.SelectedIndex == 0)
        {
            url = URLHelper.RemoveParameterFromUrl(url, "comparehistoryid");
        }
        else
        {
            url = URLHelper.AddParameterToUrl(url, "comparehistoryid", drpCompareTo.SelectedValue);
        }
        url = URLHelper.AddParameterToUrl(url, "showall", chkDisplayAllData.Checked.ToString());

        url = URLHelper.RemoveParameterFromUrl(url, "hash");
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));
        URLHelper.Redirect(url);
    }

    #endregion


    #region IPostBackEventHandler Members

    /// <summary>
    /// Raises event postback event
    /// </summary>
    /// <param name="eventArgument">Argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('|');
        if (args.Length == 2)
        {
            int rollbackVersionId = ValidationHelper.GetInteger(args[0], 0);
            bool processChilds = ValidationHelper.GetBoolean(args[1], false);
            if (rollbackVersionId > 0)
            {
                try
                {
                    // Rollback version
                    int newVersionHistoryId = ObjectVersionManager.RollbackVersion(rollbackVersionId, processChilds);

                    lblInfo.Text = GetString("objectversioning.rollbackOK");

                    string url = RequestContext.CurrentURL;

                    // Add URL parameters
                    url = URLHelper.AddParameterToUrl(url, "versionhistoryid", newVersionHistoryId.ToString());
                    url = URLHelper.AddParameterToUrl(url, "comparehistoryid", VersionCompare.VersionID.ToString());
                    url = URLHelper.AddParameterToUrl(url, "rollbackok", "1");
                    url = URLHelper.AddParameterToUrl(url, "showall", processChilds.ToString());
                    url = URLHelper.RemoveParameterFromUrl(url, "hash");
                    url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));

                    // Prepare URL
                    url = ScriptHelper.GetString(UrlResolver.ResolveUrl(url), true);

                    // Prepare script for refresh parent window and this dialog
                    StringBuilder builder = new StringBuilder();
                    builder.Append("if (wopener != null) {\n");

                    string clientId = QueryHelper.GetControlClientId("clientid", "");
                    if (!String.IsNullOrEmpty(clientId))
                    {
                        builder.Append("if (wopener.RefreshVersions_", clientId, " != null) {wopener.RefreshVersions_", clientId, "();}",
                                       "if (wopener.RefreshRelatedContent_", clientId, " != null) {wopener.RefreshRelatedContent_", clientId, "();}}");
                    }

                    builder.Append("window.document.location.replace(" + url + ");");

                    string script = ScriptHelper.GetScript(builder.ToString());
                    ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshAndReload", script);
                }
                catch (Exception ex)
                {
                    lblError.Text = GetString("objectversioning.recyclebin.restorationfailed") + " " + GetString("general.seeeventlog");

                    // Log to event log
                    Service.Resolve<IEventLogService>().LogException("View object version", "OBJECTRESTORE", ex);
                }
            }
        }
    }

    #endregion
}
