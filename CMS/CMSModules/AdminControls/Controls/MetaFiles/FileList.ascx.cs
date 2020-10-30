using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_MetaFiles_FileList : CMSUserControl
{
    #region "Variables"

    protected string colActions = String.Empty;
    protected string colFileName = String.Empty;
    protected string colFileSize = String.Empty;
    protected string colURL = String.Empty;

    private bool mCheckObjectPermissions = true;
    private bool loaded;

    #endregion

    
    #region "Properties"

    /// <summary>
    /// Object ID.
    /// </summary>
    public int ObjectID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ObjectID"], 0);
        }
        set
        {
            ViewState["ObjectID"] = value;
        }
    }


    /// <summary>
    /// Object type.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ObjectType"], String.Empty);
        }
        set
        {
            ViewState["ObjectType"] = value;
        }
    }


    /// <summary>
    /// Site ID of uploaded files.
    /// </summary>
    public int SiteID
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
    /// Attachment category/group.
    /// </summary>
    public string Category
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Category"], String.Empty);
        }
        set
        {
            ViewState["Category"] = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public string Where
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Where"], String.Empty);
        }
        set
        {
            ViewState["Where"] = value;
        }
    }


    /// <summary>
    /// Order by.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(ViewState["OrderBy"], "MetaFileName");
        }
        set
        {
            ViewState["OrderBy"] = value;
        }
    }


    /// <summary>
    /// Allows pasting file URLs into editable areas.
    /// </summary>
    public bool AllowPasteAttachments
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["AllowPasteAttachments"], false);
        }
        set
        {
            ViewState["AllowPasteAttachments"] = value;
        }
    }


    /// <summary>
    /// Indicates whether virtual path should be used when pasting file URLs into editable areas.
    /// </summary>
    public bool UseVirtualPathOnPaste
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["UseVirtualPathOnPaste"], false);
        }
        set
        {
            ViewState["UseVirtualPathOnPaste"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the object menu should be hidden.
    /// </summary>
    public bool HideObjectMenu
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["HideObjectMenu"], false);
        }
        set
        {
            ViewState["HideObjectMenu"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the file list allows file upload and files manipulation (edit/delete).
    /// </summary>
    private bool AllowModify
    {
        get
        {
            return Enabled && (!CheckObjectPermissions || UserInfoProvider.IsAuthorizedPerObject(ObjectType, ObjectID, PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser));
        }
    }


    /// <summary>
    /// Indicates whether object permissions should be checked. It is true by default.
    /// </summary>
    public bool CheckObjectPermissions
    {
        get
        {
            return mCheckObjectPermissions;
        }
        set
        {
            mCheckObjectPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets JavaScript for upload button.
    /// </summary>
    public string UploadOnClickScript
    {
        get
        {
            return btnUpload.OnClientClick;
        }
        set
        {
            btnUpload.OnClientClick = value;
        }
    }


    /// <summary>
    /// Currently handled (uploaded or being deleted) meta file.
    /// </summary>
    public MetaFileInfo CurrentlyHandledMetaFile
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether the control is enabled and allows uploading/editing/removing files).
    /// </summary>
    public bool Enabled
    {
        get
        {
            return uploader.Enabled;
        }
        set
        {
            uploader.Enabled = value;
            plcUploader.Visible = value;
            plcUploaderDisabled.Visible = !value;
            gridFiles.ShowObjectMenu = value;
        }
    }


    /// <summary>
    /// Sets the default page size of the file list.
    /// </summary>
    public int ItemsPerPage
    {
        get
        {
            return gridFiles.Pager.DefaultPageSize;
        }
        set
        {
            gridFiles.Pager.DefaultPageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the page size selector of the file list should be visible.
    /// </summary>
    public bool ShowPageSize
    {
        get
        {
            return gridFiles.Pager.ShowPageSize;
        }
        set
        {
            gridFiles.Pager.ShowPageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets upload mode of direct file uploader control.
    /// </summary>
    public MultifileUploaderModeEnum UploadMode
    {
        get
        {
            return newMetafileElem.UploadMode;
        }
        set
        {
            newMetafileElem.UploadMode = value;
        }
    }


    /// <summary>
    /// Gets the file count from the files grid.
    /// </summary>
    public int FileCount
    {
        get
        {
            return gridFiles.GridView.Rows.Count;
        }
    }


    /// <summary>
    /// Gets or sets the semicolon-separated list of allowed file extensions (without dots).
    /// </summary>
    public string AllowedExtensions
    {
        get
        {
            return ValidationHelper.GetString(ViewState["AllowedExtensions"], null);
        }
        set
        {
            ViewState["AllowedExtensions"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that defines whether to show object menu (menu containing relationships, export/backup, destroy object, clone ... functionality) .
    /// </summary>
    public bool ShowObjectMenu
    {
        get
        {
            return gridFiles.ShowObjectMenu;
        }
        set
        {
            gridFiles.ShowObjectMenu = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Event fired before upload processing.
    /// </summary>
    public event CancelEventHandler OnBeforeUpload;


    /// <summary>
    /// Event fired after upload processing.
    /// </summary>
    public event EventHandler OnAfterUpload;


    /// <summary>
    /// Raises the OnAfterUpload event.
    /// </summary>
    private void RaiseOnAfterUpload()
    {
        if (OnAfterUpload != null)
        {
            OnAfterUpload(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Event fired before delete processing.
    /// </summary>
    public event CancelEventHandler OnBeforeDelete;


    /// <summary>
    /// Event fired after delete processing.
    /// </summary>
    public event EventHandler OnAfterDelete;

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterScripts();

        colActions = GetString("general.action");
        colFileName = GetString("general.filename");
        colFileSize = GetString("filelist.unigrid.colfilesize");
        colURL = GetString("filelist.unigrid.colurl");

        ControlsHelper.RegisterPostbackControl(btnUpload);
        btnUpload.Text = GetString("filelist.btnupload");
        btnUpload.Enabled = AllowModify;

        gridFiles.OnAction += gridFiles_OnAction;
        gridFiles.OnExternalDataBound += gridFiles_OnExternalDataBound;
        gridFiles.OnBeforeDataReload += gridFiles_OnBeforeDataReload;
        gridFiles.IsLiveSite = IsLiveSite;
        gridFiles.OrderBy = OrderBy;

        ReloadData(false);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (AllowModify)
        {
            if (ObjectID > 0)
            {
                // Initialize button for adding metafile
                newMetafileElem.ObjectID = ObjectID;
                newMetafileElem.ObjectType = ObjectType;
                newMetafileElem.Category = Category;
                newMetafileElem.SiteID = SiteID;
                newMetafileElem.ParentElemID = ClientID;
                newMetafileElem.ForceLoad = true;
                newMetafileElem.Text = GetString("attach.newattachment");
                newMetafileElem.InnerElementClass = "NewAttachment";
                newMetafileElem.InnerLoadingElementClass = "NewAttachmentLoading";
                newMetafileElem.IsLiveSite = IsLiveSite;
                newMetafileElem.SourceType = MediaSourceEnum.MetaFile;
                if (AllowedExtensions != null)
                {
                    newMetafileElem.AllowedExtensions = AllowedExtensions;
                }

                plcDirectUploder.Visible = Enabled;
                plcUploaderDisabled.Visible = !Enabled;
                plcUploader.Visible = false;
            }
            else
            {
                plcDirectUploder.Visible = false;
                plcUploaderDisabled.Visible = false;
                plcUploader.Visible = true;
            }
        }
        else
        {
            plcDirectUploder.Visible = false;
            plcUploaderDisabled.Visible = true;
            plcUploader.Visible = false;

            // Hide the update column
            gridFiles.GridView.Columns[1].Visible = false;
        }

        // Hide grid placeholder if unigird is hidden
        plcGridFiles.Visible = gridFiles.Visible;

        base.OnPreRender(e);
    }


    /// <summary>
    /// BtnUpload click event handler.
    /// </summary>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (!AllowModify || (uploader.PostedFile == null) || (uploader.PostedFile.FileName.Trim() == String.Empty))
        {
            return;
        }

        try
        {
            // Fire before upload event
            CancelEventArgs beforeUploadArgs = new CancelEventArgs();
            if (OnBeforeUpload != null)
            {
                OnBeforeUpload(this, beforeUploadArgs);
            }

            // If upload was not cancelled
            if (!beforeUploadArgs.Cancel)
            {
                // Create new meta file
                MetaFileInfo mfi = new MetaFileInfo(uploader.PostedFile, ObjectID, ObjectType, Category)
                                       {
                                           MetaFileSiteID = SiteID
                                       };

                // Save meta file
                MetaFileInfo.Provider.Set(mfi);

                CurrentlyHandledMetaFile = mfi;
                RaiseOnAfterUpload();

                gridFiles.ReloadData();
            }
        }
        catch (Exception ex)
        {
            lblError.Visible = true;
            lblError.Text = ex.Message;
        }
    }


    protected void hdnPostback_Click(object sender, EventArgs e)
    {
        try
        {
            int fileId = ValidationHelper.GetInteger(hdnField.Value, 0);
            if (fileId > 0)
            {
                CurrentlyHandledMetaFile = MetaFileInfo.Provider.Get(fileId);
                RaiseOnAfterUpload();

                gridFiles.ReloadData();
            }
        }
        catch (Exception ex)
        {
            lblError.Visible = true;
            lblError.Text = ex.Message;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads control.
    /// </summary>
    /// <param name="forceReload">Indicates if the control should be reloaded even if it has been already loaded</param>
    public void ReloadData(bool forceReload)
    {
        if (!loaded || forceReload)
        {
            if (StopProcessing)
            {
                gridFiles.StopProcessing = StopProcessing;
                return;
            }
            else
            {
                gridFiles.Visible = true;
            }

            // Set where condition
            gridFiles.WhereCondition = MetaFileInfoProvider.GetWhereCondition(ObjectID, ObjectType, Category, Where);

            if (forceReload)
            {
                // Reload grid
                gridFiles.ReloadData();
            }

            loaded = true;
        }
    }

    #endregion


    #region "Private methods"

    private void RegisterScripts()
    {
        // Register tool tip script
        ScriptHelper.RegisterTooltip(Page);

        // Register dialog script for Image Editor
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "OpenImageEditor",
                                               ScriptHelper.GetScript(String.Format(@"
function OpenImageEditor(query) {{ 
    modalDialog('{0}' + query, 'EditImage', 905, 670); 
    return false; 
}}", URLHelper.ResolveUrl("~/CMSModules/Content/CMSDesk/Edit/ImageEditor.aspx"))));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "OpenEditor",
                                               ScriptHelper.GetScript(String.Format(@"
function OpenEditor(query) {{ 
    modalDialog('{0}' + query, 'EditMetadata', 680, 320); 
    return false; 
}} ", URLHelper.ResolveUrl("~/CMSModules/AdminControls/Controls/MetaFiles/MetaDataEditor.aspx"))));

        // Register javascript 'postback' function
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PostBack", ScriptHelper.GetScript(String.Format(@"
function UpdatePage(){{ 
    {0}; 
}}", Page.ClientScript.GetPostBackEventReference(hdnPostback, String.Empty))));

        // Refresh script
        string script = String.Format(@"
function InitRefresh_{0}(msg, fullRefresh, action, fileId)
{{
    if((msg != null) && (msg != '')){{ 
        alert(msg); action='error'; 
    }}
    var hidden = document.getElementById('{1}');
    if (hidden) {{
        hidden.value = fileId;
    }}
    if(fullRefresh){{
        {2}
    }}
    else {{
        {3}
    }}
}}
function ConfirmDelete() {{
    return confirm({4});
}}
 ",
                                      ClientID,
                                      hdnField.ClientID,
                                      ControlsHelper.GetPostBackEventReference(hdnFullPostback, String.Empty),
                                      ControlsHelper.GetPostBackEventReference(hdnPostback, String.Empty),
                                      ScriptHelper.GetString(GetString("general.confirmdelete")));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MetafileScripts_" + ClientID, ScriptHelper.GetScript(script));
    }

    #endregion


    #region "UniGrid Events"

    protected object gridFiles_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        GridViewRow gvr;
        DataRowView drv;
        string fileGuid;

        switch (sourceName.ToLowerCSafe())
        {
            case "edit":

                var btnImageEditor = (CMSGridActionButton)sender;
                gvr = (GridViewRow)parameter;
                drv = (DataRowView)gvr.DataItem;

                fileGuid = ValidationHelper.GetString(drv["MetaFileGUID"], String.Empty);
                string fileExtension = ValidationHelper.GetString(drv["MetaFileExtension"], String.Empty);

                // Initialize properties
                btnImageEditor.Visible = true;

                // Display button only if 'Modify' is allowed
                if (AllowModify)
                {
                    string query = String.Format("?metafileguid={0}&clientid={1}", fileGuid, ClientID);
                    query = URLHelper.AddUrlParameter(query, "hash", QueryHelper.GetHash(query));

                    // Display button only if metafile is in supported image format
                    if (ImageHelper.IsSupportedByImageEditor(fileExtension))
                    {
                        // Initialize button with script
                        btnImageEditor.OnClientClick = String.Format("OpenImageEditor({0}); return false;", ScriptHelper.GetString(query));
                    }
                    // Non-image metafile
                    else
                    {
                        // Initialize button with script
                        btnImageEditor.OnClientClick = String.Format("OpenEditor({0}); return false;", ScriptHelper.GetString(query));
                    }
                }
                else
                {
                    btnImageEditor.Enabled = false;
                }

                break;

            case "paste":

                var btnPaste = (CMSGridActionButton)sender;
                gvr = (GridViewRow)parameter;
                drv = (DataRowView)gvr.DataItem;

                fileGuid = ValidationHelper.GetString(drv["MetaFileGUID"], String.Empty);
                int fileWidth = ValidationHelper.GetInteger(drv["MetaFileImageWidth"], 0);
                int fileHeight = ValidationHelper.GetInteger(drv["MetaFileImageHeight"], 0);

                if (AllowPasteAttachments)
                {
                    if ((fileWidth > 0) && (fileHeight > 0))
                    {
                        string appPath = SystemContext.ApplicationPath;
                        if ((appPath == null) || (appPath == "/"))
                        {
                            appPath = String.Empty;
                        }
                        btnPaste.OnClientClick = String.Format("PasteImage('{0}/CMSPages/GetMetaFile.aspx?fileguid={1}'); return false", UseVirtualPathOnPaste ? "~" : appPath, fileGuid);
                    }
                    else
                    {
                        btnPaste.Enabled = false;
                    }
                }
                else
                {
                    btnPaste.Visible = false;
                }

                break;

            case "delete":              
                if (!AllowModify)
                {
                    var btnDelete = (CMSGridActionButton)sender;
                    btnDelete.Enabled = false;
                }

                break;

            case "#objectmenu":
                if (HideObjectMenu)
                {
                    ((CMSGridActionButton)sender).Visible = false;
                }
                break;

            case "name":
                drv = (DataRowView)parameter;

                string fileName = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "MetaFileName"), string.Empty);
                fileGuid = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "MetaFileGUID"), string.Empty);
                string fileExt = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "MetaFileExtension"), string.Empty);

                bool isImage = ImageHelper.IsImage(fileExt);
                string fileUrl = String.Format("{0}?fileguid={1}&chset={2}", URLHelper.ResolveUrl("~/CMSPages/GetMetaFile.aspx"), fileGuid, Guid.NewGuid());

                // Tooltip
                string title = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "MetaFileTitle"), string.Empty);
                string description = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "MetaFileDescription"), string.Empty);
                int imageWidth = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "MetaFileImageWidth"), 0);
                int imageHeight = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "MetaFileImageHeight"), 0);
                string tooltip = UIHelper.GetTooltipAttributes(fileUrl, imageWidth, imageHeight, title, fileName, fileExt, description, null, 300);

                // Icon
                string iconTag = UIHelper.GetFileIcon(Page,fileExt, tooltip: fileName);
                if (isImage)
                {
                    return String.Format("<a href=\"#\" onclick=\"javascript: window.open('{0}'); return false;\" class=\"cms-icon-link\"><span id=\"{1}\" {2}>{3}{4}</span></a>", fileUrl, fileGuid, tooltip, iconTag, fileName);
                }
                else
                {
                    fileUrl = URLHelper.AddParameterToUrl(fileUrl, "disposition", "attachment");

                    // NOTE: OnClick here is needed to avoid loader to show because even for download links, the pageUnload event is fired
                    return String.Format("<a href=\"{0}\" onclick=\"javascript: {5}\" class=\"cms-icon-link\"><span id=\"{1}\" {2}>{3}{4}</span></a>", fileUrl, fileGuid, tooltip, iconTag, fileName, ScriptHelper.GetDisableProgressScript());
                }

            case "size":
                return DataHelper.GetSizeString(ValidationHelper.GetLong(parameter, 0));

            case "update":
                {
                    drv = (DataRowView)parameter;
                    Panel pnlBlock = new Panel
                    {
                        ID = "pnlBlock"
                    };

                    // Add update control
                    // Dynamically load uploader control
                    var dfuElem = Page.LoadUserControl("~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx") as DirectFileUploader;
                    if (dfuElem != null)
                    {
                        dfuElem.ID = "dfuElem" + ObjectID;
                        dfuElem.SourceType = MediaSourceEnum.MetaFile;
                        dfuElem.ControlGroup = "Uploader_" + ObjectID;
                        dfuElem.DisplayInline = true;
                        dfuElem.ForceLoad = true;
                        dfuElem.MetaFileID = ValidationHelper.GetInteger(drv["MetaFileID"], 0);
                        dfuElem.ObjectID = ObjectID;
                        dfuElem.ObjectType = ObjectType;
                        dfuElem.Category = Category;
                        dfuElem.ParentElemID = ClientID;
                        dfuElem.ShowIconMode = true;
                        dfuElem.InsertMode = false;
                        dfuElem.ParentElemID = ClientID;
                        dfuElem.IncludeNewItemInfo = true;
                        dfuElem.SiteID = SiteID;
                        dfuElem.IsLiveSite = IsLiveSite;

                        // Setting of the direct single mode
                        dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
                        dfuElem.Width = 16;
                        dfuElem.Height = 16;
                        dfuElem.MaxNumberToUpload = 1;

                        if (AllowedExtensions != null)
                        {
                            dfuElem.AllowedExtensions = AllowedExtensions;
                        }

                        pnlBlock.Controls.Add(dfuElem);
                    }

                    return pnlBlock;
                }
        }

        return parameter;
    }


    protected void gridFiles_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                try
                {
                    // Get meta file ID
                    int metaFileId = ValidationHelper.GetInteger(actionArgument, 0);

                    // Get meta file
                    MetaFileInfo mfi = MetaFileInfo.Provider.Get(metaFileId);

                    // Set currently handled meta file
                    CurrentlyHandledMetaFile = mfi;

                    // Fire before delete event
                    CancelEventArgs beforeDeleteArgs = new CancelEventArgs();
                    if (OnBeforeDelete != null)
                    {
                        OnBeforeDelete(this, beforeDeleteArgs);
                    }

                    // If delete was not cancelled
                    if (!beforeDeleteArgs.Cancel)
                    {
                        // Delete meta file
                        MetaFileInfo.Provider.Get(metaFileId)?.Delete();

                        // Fire after delete event
                        if (OnAfterDelete != null)
                        {
                            OnAfterDelete(this, EventArgs.Empty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblError.Visible = true;
                    lblError.Text = ex.Message;
                }
                break;
        }
    }


    protected void gridFiles_OnBeforeDataReload()
    {
        gridFiles.NamedColumns["update"].Visible = AllowModify;
        gridFiles.NamedColumns["filename"].Visible = false;
    }

    #endregion
}
