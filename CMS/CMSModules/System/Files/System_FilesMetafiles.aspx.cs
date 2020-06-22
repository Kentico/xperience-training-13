using System;
using System.Collections.Generic;
using System.Data;

using CMS.Base;

using System.Linq;
using System.Security.Principal;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_System_Files_System_FilesMetafiles : GlobalAdminPage
{
    #region "Variables"

    protected int currentSiteId = 0;
    protected int siteId = 0;

    protected string filterWhere = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ctlAsyncLog.ProcessData.Error;
        }
        set
        {
            ctlAsyncLog.ProcessData.Error = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterTooltip(Page);

        // Initialize events
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        if (RequestHelper.IsCallback())
        {
            pnlContent.Visible = false;

            gridFiles.StopProcessing = true;
            siteSelector.StopProcessing = true;

            return;
        }

        // Setup the controls
        gridFiles.OnExternalDataBound += gridFiles_OnExternalDataBound;
        gridFiles.OnAction += gridFiles_OnAction;

        ControlsHelper.RegisterPostbackControl(btnOk);

        currentSiteId = SiteContext.CurrentSiteID;

        CurrentMaster.DisplaySiteSelectorPanel = true;

        // Setup the filters
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

        if (!RequestHelper.IsPostBack() && (currentSiteId > 0))
        {
            siteSelector.Value = currentSiteId;
        }

        siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        if (siteId > 0)
        {
            filterWhere = "MetaFileSiteID = " + siteId;
            gridFiles.WhereCondition = filterWhere;
            UIContext["SiteID"] = siteId;
        }
        else if (siteId == UniSelector.US_GLOBAL_RECORD)
        {
            // Global files
            filterWhere = "MetaFileSiteID IS NULL";
            gridFiles.WhereCondition = filterWhere;
        }

        // Fill the objecttype DDL
        if (!RequestHelper.IsPostBack())
        {
            LoadObjectTypes();
        }

        // Add object type condition
        string selectedType = SqlHelper.GetSafeQueryString(drpObjectType.SelectedValue, false);
        if (!string.IsNullOrEmpty(selectedType))
        {
            filterWhere = SqlHelper.AddWhereCondition(filterWhere, "MetaFileObjectType = '" + selectedType + "'", "AND");
            gridFiles.WhereCondition = filterWhere;
        }

        if (!RequestHelper.IsPostBack())
        {
            // Fill in the actions
            drpAction.Items.Add(new ListItem(GetString("general.selectaction"), ""));

            bool copyDB = true;
            bool copyFS = true;
            bool deleteDB = true;
            bool deleteFS = true;

            if (siteId > 0)
            {
                var locationType = GetFilesLocationType(siteId);

                bool fs = locationType != FilesLocationTypeEnum.Database;
                bool db = locationType != FilesLocationTypeEnum.FileSystem;

                copyFS = deleteDB = fs;
                deleteFS = db;
                copyDB = db && fs;
            }

            if (copyDB)
            {
                drpAction.Items.Add(new ListItem("Copy to database", "copytodatabase"));
            }
            if (copyFS)
            {
                drpAction.Items.Add(new ListItem("Copy to file system", "copytofilesystem"));
            }
            if (deleteDB)
            {
                drpAction.Items.Add(new ListItem("Delete from database", "deleteindatabase"));
            }
            if (deleteFS)
            {
                drpAction.Items.Add(new ListItem("Delete from file system", "deleteinfilesystem"));
            }
        }
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        LoadObjectTypes();
    }


    private void LoadObjectTypes()
    {
        drpObjectType.Items.Clear();
        drpObjectType.Items.Add(new ListItem(GetString("general.selectall"), ""));

        // Used object types
        var whereCondition = siteId > 0 ? new WhereCondition().WhereEquals("MetaFileSiteID", siteId) : new WhereCondition();

        var objTypes = MetaFileInfo.Provider.Get().Column("MetaFileObjectType").Distinct().Where(whereCondition).GetListResult<string>();
        ListItem[] items = new ListItem[objTypes.Count];
        int i = 0;

        foreach (string type in objTypes)
        {
            items[i++] = new ListItem(GetString(TypeHelper.GetTasksResourceKey(type)), type);
        }

        Array.Sort(items, CompareObjectType);
        drpObjectType.Items.AddRange(items);
    }


    /// <summary>
    /// Comparison method for two ListItems (to sort the drop down with object types).
    /// </summary>
    /// <param name="item1">First item to compare</param>
    /// <param name="item2">Second item to compare</param>
    private static int CompareObjectType(ListItem item1, ListItem item2)
    {
        return item1.Text.CompareToCSafe(item2.Text, true);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if ((siteId > 0) || (siteId == UniSelector.US_GLOBAL_RECORD))
        {
            // Hide the Site ID column
            if (gridFiles.GridView.Columns.Count > 0)
            {
                gridFiles.NamedColumns["SiteName"].Visible = false;
            }
        }

        pnlFooter.Visible = (gridFiles.GridView.Rows.Count > 0);
    }


    protected void gridFiles_OnAction(string actionName, object actionArgument)
    {
        int fileId = ValidationHelper.GetInteger(actionArgument, 0);
        string name = null;

        if (ProcessFile(fileId, actionName, ref name))
        {
            gridFiles.ReloadData();

            switch (actionName)
            {
                case "copytodatabase":
                    // Copy the file from file system to the database
                    ShowConfirmation("The file '" + name + "' was copied to the database.");
                    break;

                case "copytofilesystem":
                    // Copy to file system
                    ShowConfirmation("The file '" + name + "' was copied to the file system.");
                    break;

                case "deleteindatabase":
                    // Delete from database
                    ShowConfirmation("The file '" + name + "' binary was deleted from the database.");
                    break;

                case "deleteinfilesystem":
                    // Delete from file system
                    ShowConfirmation("The file '" + name + "' binary was deleted from the file system.");
                    break;
            }
        }
    }


    /// <summary>
    /// Copies the file binary to the database.
    /// </summary>
    /// <param name="fileId">MetaFile ID</param>
    /// <param name="name">Returning the metafile name</param>
    protected bool CopyToDatabase(int fileId, ref string name)
    {
        // Copy the file from file system to the database
        MetaFileInfo mi = MetaFileInfo.Provider.Get(fileId);
        if (mi != null)
        {
            name = mi.MetaFileName;

            if (mi.MetaFileBinary == null)
            {
                // Ensure the binary data
                mi.MetaFileBinary = MetaFileInfoProvider.GetFile(mi, SiteInfoProvider.GetSiteName(mi.MetaFileSiteID));
                mi.Generalized.UpdateData();

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Copies the file binary to the file system.
    /// </summary>
    /// <param name="fileId">MetaFile ID</param>
    /// <param name="name">Returning the metafile name</param>
    protected bool CopyToFileSystem(int fileId, ref string name)
    {
        // Copy the file from database to the file system
        MetaFileInfo mi = MetaFileInfo.Provider.Get(fileId);
        if (mi != null)
        {
            name = mi.MetaFileName;

            // Ensure the physical file
            MetaFileInfoProvider.EnsurePhysicalFile(mi, SiteInfoProvider.GetSiteName(mi.MetaFileSiteID));

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes the file binary from the database.
    /// </summary>
    /// <param name="fileId">MetaFile ID</param>
    /// <param name="name">Returning the metafile name</param>
    protected bool DeleteFromDatabase(int fileId, ref string name)
    {
        // Delete the file in database and ensure it in the file system
        MetaFileInfo mi = MetaFileInfo.Provider.Get(fileId);
        if (mi != null)
        {
            name = mi.MetaFileName;

            MetaFileInfoProvider.EnsurePhysicalFile(mi, SiteInfoProvider.GetSiteName(mi.MetaFileSiteID));

            // Clear the binary data
            mi.MetaFileBinary = null;
            mi.Generalized.UpdateData();

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes the file binary from the file system.
    /// </summary>
    /// <param name="fileId">MetaFile ID</param>
    /// <param name="name">Returning the metafile name</param>
    protected bool DeleteFromFileSystem(int fileId, ref string name)
    {
        // Delete the file in file system
        MetaFileInfo mi = MetaFileInfo.Provider.Get(fileId);
        if (mi != null)
        {
            name = mi.MetaFileName;

            // Ensure the binary column first (check if exists)
            DataSet ds = MetaFileInfoProvider.GetMetaFiles("MetaFileID = " + fileId, null, "CASE WHEN MetaFileBinary IS NULL THEN 0 ELSE 1 END AS HasBinary", -1);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                bool hasBinary = ValidationHelper.GetBoolean(ds.Tables[0].Rows[0][0], false);
                if (!hasBinary)
                {
                    // Copy the binary data to database
                    mi.MetaFileBinary = MetaFileInfoProvider.GetFile(mi, SiteInfoProvider.GetSiteName(mi.MetaFileSiteID));
                    mi.Generalized.UpdateData();
                }

                // Delete the file from the disk
                MetaFileInfoProvider.DeleteFile(SiteInfoProvider.GetSiteName(mi.MetaFileSiteID), mi.MetaFileGUID.ToString(), true, false);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Processes the given file.
    /// </summary>
    /// <param name="fileId">MetaFile ID</param>
    /// <param name="actionName">Action name</param>
    /// <param name="name">Returning the file name</param>
    protected bool ProcessFile(int fileId, string actionName, ref string name)
    {
        if (fileId > 0)
        {
            switch (actionName)
            {
                case "copytodatabase":
                    // Copy the file from file system to the database
                    return CopyToDatabase(fileId, ref name);

                case "copytofilesystem":
                    // Copy to file system
                    return CopyToFileSystem(fileId, ref name);

                case "deleteindatabase":
                    // Delete from database
                    return DeleteFromDatabase(fileId, ref name);

                case "deleteinfilesystem":
                    // Delete from file system
                    return DeleteFromFileSystem(fileId, ref name);
            }
        }

        return false;
    }


    /// <summary>
    /// Processes the files.
    /// </summary>
    /// <param name="parameter">Parameter for the action</param>
    protected void ProcessFiles(object parameter)
    {
        // Begin log
        AddLog("Processing files ...");

        object[] parameters = (object[])parameter;

        List<string> items = (List<string>)parameters[0];
        string action = (string)parameters[1];

        if ((items != null) && (items.Count > 0))
        {
            string name = null;

            // Process all items
            foreach (string id in items)
            {
                // Process the file
                int fileId = ValidationHelper.GetInteger(id, 0);

                if (ProcessFile(fileId, action, ref name))
                {
                    AddLog(name);
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    AddLog(name + " SKIPPED");
                }
            }
        }
    }


    /// <summary>
    /// Grid external data bound handler.
    /// </summary>
    protected object gridFiles_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // Get the data row view from parameter
        DataRowView drv = null;
        if (parameter is DataRowView)
        {
            drv = (DataRowView)parameter;
        }
        else if (parameter is GridViewRow)
        {
            // Get data from the grid view row
            drv = (DataRowView)((GridViewRow)parameter).DataItem;
        }

        // Get the action button
        CMSGridActionButton btn = sender as CMSGridActionButton;

        switch (sourceName)
        {
            case "delete":
                {
                    // Delete action
                    int siteId = ValidationHelper.GetInteger(drv["MetaFileSiteID"], 0);
                    string siteName = SiteInfoProvider.GetSiteName(siteId);

                    Guid guid = ValidationHelper.GetGuid(drv["MetaFileGUID"], Guid.Empty);
                    string extension = ValidationHelper.GetString(drv["MetaFileExtension"], "");

                    // Check if the file is in DB
                    bool db = ValidationHelper.GetBoolean(drv["HasBinary"], false);

                    // Check if the file is in the file system
                    bool fs = false;
                    string path = MetaFileInfoProvider.GetFilePhysicalPath(siteName, guid.ToString(), extension);
                    if (File.Exists(path))
                    {
                        fs = true;
                    }

                    // If the file is present in both file system and database, delete is allowed
                    if (fs && db)
                    {
                        var locationType = GetFilesLocationType(siteId);

                        // If the files are stored in file system or use both locations, delete is allowed in database 
                        if (locationType != FilesLocationTypeEnum.Database)
                        {
                            btn.OnClientClick = btn.OnClientClick.Replace("'delete'", "'deleteindatabase'");
                            btn.ToolTip = "Delete from database";
                            return parameter;
                        }
                        
                        // Files are stored in database, delete is allowed in file system
                        btn.OnClientClick = btn.OnClientClick.Replace("'delete'", "'deleteinfilesystem'");
                        btn.ToolTip = "Delete from file system";
                        return parameter;
                    }

                    btn.Visible = false;
                }
                break;

            case "copy":
                {
                    // Delete action
                    int siteId = ValidationHelper.GetInteger(drv["MetaFileSiteID"], 0);
                    string siteName = SiteInfoProvider.GetSiteName(siteId);

                    Guid guid = ValidationHelper.GetGuid(drv["MetaFileGUID"], Guid.Empty);
                    string extension = ValidationHelper.GetString(drv["MetaFileExtension"], "");

                    // Check if the file is in DB
                    bool db = ValidationHelper.GetBoolean(drv["HasBinary"], false);

                    // Check if the file is in the file system
                    bool fs = false;
                    string path = MetaFileInfoProvider.GetFilePhysicalPath(siteName, guid.ToString(), extension);
                    if (File.Exists(path))
                    {
                        fs = true;
                    }

                    var locationType = GetFilesLocationType(siteId);

                    // If the file is stored in file system and the file is not present in database, copy to database is allowed
                    if (fs && !db && (locationType == FilesLocationTypeEnum.Both))
                    {
                        btn.OnClientClick = btn.OnClientClick.Replace("'copy'", "'copytodatabase'");
                        btn.ToolTip = "Copy to database";
                        //btn.ImageUrl = 
                        return parameter;
                    }
                    // If the file is stored in database and the file is not present in file system, copy to file system is allowed
                    if (db && !fs && (locationType != FilesLocationTypeEnum.Database))
                    {
                        btn.OnClientClick = btn.OnClientClick.Replace("'copy'", "'copytofilesystem'");
                        btn.ToolTip = "Copy to file system";
                        return parameter;
                    }

                    btn.Visible = false;
                }
                break;

            case "name":
                {
                    // MetaFile name
                    string name = ValidationHelper.GetString(drv["MetaFileName"], "");
                    Guid guid = ValidationHelper.GetGuid(drv["MetaFileGUID"], Guid.Empty);
                    int siteId = ValidationHelper.GetInteger(drv["MetaFileSiteID"], 0);
                    string extension = ValidationHelper.GetString(drv["MetaFileExtension"], "");

                    // File name
                    name = Path.GetFileNameWithoutExtension(name);

                    string url = ResolveUrl("~/CMSPages/GetMetaFile.aspx?fileguid=") + guid;
                    if (siteId != currentSiteId)
                    {
                        // Add the site name to the URL if not current site
                        SiteInfo si = SiteInfo.Provider.Get(siteId);
                        if (si != null)
                        {
                            url += "&sitename=" + si.SiteName;
                        }
                    }

                    string tooltipSpan = name;
                    bool isImage = ImageHelper.IsImage(extension);
                    if (isImage)
                    {
                        int imageWidth = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "MetaFileImageWidth"), 0);
                        int imageHeight = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "MetaFileImageHeight"), 0);

                        string tooltip = UIHelper.GetTooltipAttributes(url, imageWidth, imageHeight, null, name, extension, null, null, 300);
                        tooltipSpan = "<span id=\"" + guid + "\" " + tooltip + ">" + name + "</span>";
                    }

                    return UIHelper.GetFileIcon(Page, extension, tooltip: name) + "&nbsp;<a href=\"" + url + "\" target=\"_blank\">" + tooltipSpan + "</a>";
                }

            case "size":
                // File size
                return DataHelper.GetSizeString(ValidationHelper.GetInteger(parameter, 0));

            case "yesno":
                // Yes / No
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "site":
                {
                    int siteId = ValidationHelper.GetInteger(parameter, 0);
                    if (siteId > 0)
                    {
                        SiteInfo si = SiteInfo.Provider.Get(siteId);
                        if (si != null)
                        {
                            return si.DisplayName;
                        }
                    }
                    return null;
                }

            case "storedinfilesystem":
                {
                    // Delete action
                    int siteId = ValidationHelper.GetInteger(drv["MetaFileSiteID"], 0);
                    string siteName = SiteInfoProvider.GetSiteName(siteId);

                    Guid guid = ValidationHelper.GetGuid(drv["MetaFileGUID"], Guid.Empty);
                    string extension = ValidationHelper.GetString(drv["MetaFileExtension"], "");

                    // Check if the file is in DB
                    bool db = ValidationHelper.GetBoolean(drv["HasBinary"], false);

                    // Check if the file is in the file system
                    bool fs = false;
                    string path = MetaFileInfoProvider.GetFilePhysicalPath(siteName, guid.ToString(), extension);
                    if (File.Exists(path))
                    {
                        fs = true;
                    }

                    return UniGridFunctions.ColoredSpanYesNo(fs);
                }
        }

        return parameter;
    }


    /// <summary>
    /// Action button handler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(drpAction.SelectedValue))
        {
            List<string> items = null;

            if (drpWhat.SelectedValue == "all")
            {
                // Get only the appropriate set of items
                string where = filterWhere;
                switch (drpAction.SelectedValue)
                {
                    case "deleteindatabase":
                    case "copytofilesystem":
                        // Only process those where binary is available in DB
                        where = SqlHelper.AddWhereCondition(where, "MetaFileBinary IS NOT NULL");
                        break;

                    case "copytodatabase":
                        // Only copy those where the binary is missing
                        where = SqlHelper.AddWhereCondition(where, "MetaFileBinary IS NULL");
                        break;
                }

                // Get all, build the list of items
                DataSet ds = MetaFileInfoProvider.GetMetaFiles(where, null, "MetaFileID", 0);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    items = new List<string>();

                    // Process all rows
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int fileId = ValidationHelper.GetInteger(dr["MetaFileID"], 0);
                        items.Add(fileId.ToString());
                    }
                }
            }
            else
            {
                // Take selected items
                items = gridFiles.SelectedItems;
            }

            if ((items != null) && (items.Count > 0))
            {
                // Setup the async log
                pnlLog.Visible = true;
                pnlContent.Visible = false;

                ctlAsyncLog.TitleText = drpAction.SelectedItem.Text;

                CurrentError = string.Empty;

                // Process the file asynchronously
                var parameter = new object[] { items, drpAction.SelectedValue };
                ctlAsyncLog.RunAsync(p => ProcessFiles(parameter), WindowsIdentity.GetCurrent());
            }
        }
    }


    /// <summary>
    /// Gets the files location type
    /// </summary>
    /// <param name="siteId">Site identifier</param>
    private static FilesLocationTypeEnum GetFilesLocationType(int siteId)
    {
        string siteName = null;
        if (siteId > 0)
        {
            // Get site name if site is specified
            var siteIdentifier = new SiteInfoIdentifier(siteId);
            siteName = siteIdentifier.ObjectCodeName;
        }
        return FileHelper.FilesLocationType(siteName);
    }

    #endregion


    #region "Handling async thread"

    /// <summary>
    /// Cancel event handler.
    /// </summary>
    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        string canceled = "The operation was canceled.";
        AddLog(canceled);
        ShowConfirmation(canceled);
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }

        pnlContent.Visible = true;
        pnlLog.Visible = false;

        // Reload the grid
        gridFiles.ResetSelection();
        gridFiles.ReloadData();
    }


    /// <summary>
    /// Error event handler.
    /// </summary>
    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        if (ctlAsyncLog.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsyncLog.Stop();
        }

        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }

        // Reload the grid
        gridFiles.ResetSelection();
        gridFiles.ReloadData();
    }


    /// <summary>
    /// Finished event handler.
    /// </summary>
    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }

        pnlContent.Visible = true;
        pnlLog.Visible = false;

        // Reload the grid
        gridFiles.ResetSelection();
        gridFiles.ReloadData();
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    /// <summary>
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = (error + "<br />" + CurrentError);
    }

    #endregion
}
