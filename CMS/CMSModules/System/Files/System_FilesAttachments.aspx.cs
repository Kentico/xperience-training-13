using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_System_Files_System_FilesAttachments : GlobalAdminPage
{
    #region "Variables"

    protected int currentSiteId = 0;
    protected int siteId = 0;

    protected string siteWhere = null;

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

        ctlAsyncLog.Buttons.CssClass = "cms-edit-menu";

        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("srch.clearattachmentcache"),
            Tooltip = GetString("srch.clearattachmentcache.tooltip"),
            CommandName = "clearcache"
        });

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

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

        // Setup the site selection
        siteSelector.DropDownSingleSelect.AutoPostBack = true;

        if (!RequestHelper.IsPostBack() && (currentSiteId > 0))
        {
            siteSelector.Value = currentSiteId;
        }

        siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        if (siteId > 0)
        {
            siteWhere = "AttachmentSiteID = " + siteId;
            gridFiles.WhereCondition = siteWhere;
            UIContext["SiteID"] = siteId;
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
                var filesLocationType = GetFilesLocationType(siteId);

                bool fs = filesLocationType != FilesLocationTypeEnum.Database;
                bool db = filesLocationType != FilesLocationTypeEnum.FileSystem;

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


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (siteId > 0)
        {
            // Hide the Site ID column
            if (gridFiles.GridView.Columns.Count > 0)
            {
                gridFiles.NamedColumns["SiteName"].Visible = false;
            }
        }

        pnlFooter.Visible = (gridFiles.GridView.Rows.Count > 0);
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "clearcache":
                AttachmentInfoProvider.ClearSearchCache();
                ShowConfirmation(GetString("srch.attachmentcachedcleared"));
                break;
        }
    }


    protected void gridFiles_OnAction(string actionName, object actionArgument)
    {
        int attachmentId = ValidationHelper.GetInteger(actionArgument, 0);
        string name = null;

        if (ProcessFile(attachmentId, actionName, ref name))
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
    /// <param name="attachmentId">Attachment ID</param>
    /// <param name="name">Returning the attachment name</param>
    protected bool CopyToDatabase(int attachmentId, ref string name)
    {
        // Copy the file from file system to the database
        var ai = AttachmentInfoProvider.GetAttachmentInfo(attachmentId, true);
        if (ai != null)
        {
            name = ai.AttachmentName;

            if (ai.AttachmentBinary == null)
            {
                // Ensure the binary data
                ai.AttachmentBinary = AttachmentBinaryHelper.GetAttachmentBinary((DocumentAttachment)ai);
                ai.Generalized.UpdateData();
 
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Copies the file binary to the file system.
    /// </summary>
    /// <param name="attachmentId">Attachment ID</param>
    /// <param name="name">Returning the attachment name</param>
    protected bool CopyToFileSystem(int attachmentId, ref string name)
    {
        // Copy the file from database to the file system
        var ai = AttachmentInfoProvider.GetAttachmentInfo(attachmentId, true);
        if (ai != null)
        {
            name = ai.AttachmentName;

            // Ensure the physical file
            AttachmentBinaryHelper.EnsurePhysicalFile((DocumentAttachment)ai);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes the file binary from the database.
    /// </summary>
    /// <param name="attachmentId">Attachment ID</param>
    /// <param name="name">Returning the attachment name</param>
    protected bool DeleteFromDatabase(int attachmentId, ref string name)
    {
        // Delete the file in database and ensure it in the file system
        var ai = AttachmentInfoProvider.GetAttachmentInfo(attachmentId, false);
        if (ai != null)
        {
            name = ai.AttachmentName;

            AttachmentBinaryHelper.EnsurePhysicalFile((DocumentAttachment)ai);

            // Clear the binary data
            ai.AttachmentBinary = null;
            ai.Generalized.UpdateData();

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes the file binary from the file system.
    /// </summary>
    /// <param name="attachmentId">Attachment ID</param>
    /// <param name="name">Returning the attachment name</param>
    protected bool DeleteFromFileSystem(int attachmentId, ref string name)
    {
        // Delete the file in file system
        var ai = AttachmentInfoProvider.GetAttachmentInfo(attachmentId, false);
        if (ai != null)
        {
            name = ai.AttachmentName;

            // Ensure the binary column first (check if exists)
            DataSet ds = AttachmentInfoProvider.GetAttachments()
                .WhereEquals("AttachmentID", attachmentId)
                .BinaryData(true)
                .Columns("CASE WHEN AttachmentBinary IS NULL THEN 0 ELSE 1 END AS HasBinary");

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                bool hasBinary = ValidationHelper.GetBoolean(ds.Tables[0].Rows[0][0], false);
                if (!hasBinary)
                {
                    // Copy the binary data to database
                    ai.AttachmentBinary = AttachmentBinaryHelper.GetAttachmentBinary((DocumentAttachment)ai);
                    ai.Generalized.UpdateData();
                }

                // Delete the file from the disk
                AttachmentBinaryHelper.DeleteFile(ai.AttachmentGUID, SiteInfoProvider.GetSiteName(ai.AttachmentSiteID), true);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Processes the given file.
    /// </summary>
    /// <param name="attachmentId">Attachment ID</param>
    /// <param name="actionName">Action name</param>
    /// <param name="name">Returning the file name</param>
    protected bool ProcessFile(int attachmentId, string actionName, ref string name)
    {
        if (attachmentId > 0)
        {
            switch (actionName)
            {
                case "copytodatabase":
                    // Copy the file from file system to the database
                    return CopyToDatabase(attachmentId, ref name);

                case "copytofilesystem":
                    // Copy to file system
                    return CopyToFileSystem(attachmentId, ref name);

                case "deleteindatabase":
                    // Delete from database
                    return DeleteFromDatabase(attachmentId, ref name);

                case "deleteinfilesystem":
                    // Delete from file system
                    return DeleteFromFileSystem(attachmentId, ref name);
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
                int attachmentId = ValidationHelper.GetInteger(id, 0);

                if (ProcessFile(attachmentId, action, ref name))
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
        var btn = sender as CMSGridActionButton;

        switch (sourceName)
        {
            case "delete":
                {
                    // Delete action
                    int siteId = ValidationHelper.GetInteger(drv["AttachmentSiteID"], 0);
                    string siteName = SiteInfoProvider.GetSiteName(siteId);

                    Guid guid = ValidationHelper.GetGuid(drv["AttachmentGUID"], Guid.Empty);
                    string extension = ValidationHelper.GetString(drv["AttachmentExtension"], "");

                    // Check if the file is in DB
                    bool db = ValidationHelper.GetBoolean(drv["HasBinary"], false);

                    // Check if the file is in the file system
                    bool fs = false;
                    string path = AttachmentBinaryHelper.GetFilePhysicalPath(siteName, guid.ToString(), extension);
                    if (File.Exists(path))
                    {
                        fs = true;
                    }

                    // If the file is present in both file system and database, delete is allowed
                    if (fs && db)
                    {
                        var filesLocationType = GetFilesLocationType(siteId);

                        // If the files are stored in file system or use both locations, delete is allowed in database 
                        if (filesLocationType != FilesLocationTypeEnum.Database)
                        {
                            btn.OnClientClick = btn.OnClientClick.Replace("'delete'", "'deleteindatabase'");
                            btn.ToolTip = "Delete from database";
                            return parameter;
                        }

                        // Else the files are stored in database, delete is allowed in file system
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
                    int siteId = ValidationHelper.GetInteger(drv["AttachmentSiteID"], 0);
                    string siteName = SiteInfoProvider.GetSiteName(siteId);

                    Guid guid = ValidationHelper.GetGuid(drv["AttachmentGUID"], Guid.Empty);
                    string extension = ValidationHelper.GetString(drv["AttachmentExtension"], "");

                    // Check if the file is in DB
                    bool db = ValidationHelper.GetBoolean(drv["HasBinary"], false);

                    // Check if the file is in the file system
                    bool fs = false;
                    string path = AttachmentBinaryHelper.GetFilePhysicalPath(siteName, guid.ToString(), extension);
                    if (File.Exists(path))
                    {
                        fs = true;
                    }

                    var filesLocationType = GetFilesLocationType(siteId);

                    // If the file is stored in file system and the file is not present in database, copy to database is allowed
                    if (fs && !db && (filesLocationType == FilesLocationTypeEnum.Both))
                    {
                        btn.OnClientClick = btn.OnClientClick.Replace("'copy'", "'copytodatabase'");
                        btn.ToolTip = "Copy to database";
                        //btn.ImageUrl = 
                        return parameter;
                    }
                    // If the file is stored in database and the file is not present in file system, copy to file system is allowed
                    if (db && !fs && filesLocationType != FilesLocationTypeEnum.Database)
                    {
                        btn.OnClientClick = btn.OnClientClick.Replace("'copy'", "'copytofilesystem'");
                        btn.ToolTip = "Copy to file system";
                        //btn.ImageUrl = 
                        return parameter;
                    }

                    btn.Visible = false;
                }
                break;

            case "name":
                return GetAttachmentHtml(new DataRowContainer(drv));

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
                        SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
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
                    int siteId = ValidationHelper.GetInteger(drv["AttachmentSiteID"], 0);
                    string siteName = SiteInfoProvider.GetSiteName(siteId);

                    Guid guid = ValidationHelper.GetGuid(drv["AttachmentGUID"], Guid.Empty);
                    string extension = ValidationHelper.GetString(drv["AttachmentExtension"], "");

                    // Check if the file is in DB
                    bool db = ValidationHelper.GetBoolean(drv["HasBinary"], false);

                    // Check if the file is in the file system
                    bool fs = false;
                    string path = AttachmentBinaryHelper.GetFilePhysicalPath(siteName, guid.ToString(), extension);
                    if (File.Exists(path))
                    {
                        fs = true;
                    }

                    return UniGridFunctions.ColoredSpanYesNo(fs);
                }
        }

        return parameter;
    }


    private object GetAttachmentHtml(ISimpleDataContainer container)
    {
        // Attachment name
        string name = ValidationHelper.GetString(container["AttachmentName"], "");
        Guid guid = ValidationHelper.GetGuid(container["AttachmentGUID"], Guid.Empty);
        int siteId = ValidationHelper.GetInteger(container["AttachmentSiteID"], 0);
        string extension = ValidationHelper.GetString(container["AttachmentExtension"], "");

        // File name
        name = Path.GetFileNameWithoutExtension(name);

        string url = ResolveUrl("~/CMSPages/GetFile.aspx?guid=") + guid;
        if (siteId != currentSiteId)
        {
            // Add the site name to the URL if not current site
            SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
            if (si != null)
            {
                url += "&sitename=" + si.SiteName;
            }
        }

        string tooltipSpan = name;
        bool isImage = ImageHelper.IsImage(extension);

        string variant = ValidationHelper.GetString(container["AttachmentVariantDefinitionIdentifier"], "");

        if (isImage)
        {
            int imageWidth = ValidationHelper.GetInteger(container["AttachmentImageWidth"], 0);
            int imageHeight = ValidationHelper.GetInteger(container["AttachmentImageHeight"], 0);

            string id = guid.ToString();
            if (!String.IsNullOrEmpty(variant))
            {
                url += "&variant=" + variant;
                id += "_" + variant;
            }

            string tooltip = UIHelper.GetTooltipAttributes(url, imageWidth, imageHeight, null, name, extension, null, null, 300);
            tooltipSpan = "<span id=\"" + id + "\" " + tooltip + ">" + name + "</span>";
        }

        var html = UIHelper.GetFileIcon(Page, extension, tooltip: name) + "&nbsp;<a href=\"" + url + "\" target=\"_blank\">" + tooltipSpan + "</a>";

        if (!String.IsNullOrEmpty(variant))
        {
            var displayedVariantName = ResHelper.GetAPIString("AttachmentVariant." + variant, variant);
            html += String.Format(" ({0})", displayedVariantName);
        }

        return html;
    }


    /// <summary>
    /// Gets the files location type
    /// </summary>
    /// <param name="siteId">Site identifier</param>
    private static FilesLocationTypeEnum GetFilesLocationType(int siteId)
    {
        var siteIdentifier = new SiteInfoIdentifier(siteId);
        return FileHelper.FilesLocationType(siteIdentifier.ObjectCodeName);
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
                var whereCondition = new WhereCondition(siteWhere);
                switch (drpAction.SelectedValue)
                {
                    case "deleteindatabase":
                    case "copytofilesystem":
                        // Only process those where binary is available in DB
                        whereCondition.And().WhereNotNull("AttachmentBinary");
                        break;

                    case "copytodatabase":
                        // Only copy those where the binary is missing
                        whereCondition.And().WhereNull("AttachmentBinary");
                        break;
                }

                // Get all, build the list of items
                DataSet ds = AttachmentInfoProvider.GetAttachments()
                    .Where(whereCondition)
                    .OrderBy(gridFiles.SortDirect)
                    .BinaryData(false)
                    .Columns("AttachmentID");

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    items = new List<string>();

                    // Process all rows
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int attachmentId = ValidationHelper.GetInteger(dr["AttachmentID"], 0);
                        items.Add(attachmentId.ToString());
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

    #endregion


    #region "Handling async thread"

    /// <summary>
    /// Cancel event handler.
    /// </summary>
    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        const string canceled = "The operation was canceled.";

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
