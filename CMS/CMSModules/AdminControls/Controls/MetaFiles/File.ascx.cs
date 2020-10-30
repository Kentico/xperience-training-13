using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_MetaFiles_File : ReadOnlyFormEngineUserControl
{
    #region "Variables"

    private bool mAlreadyUploadedDontDelete;
    protected bool columnUpdateVisible = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Object id.
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
            return ValidationHelper.GetString(ViewState["ObjectType"], "");
        }
        set
        {
            ViewState["ObjectType"] = value;
        }
    }


    /// <summary>
    /// Site id.
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
    /// Attachment category/group
    /// </summary>
    public string Category
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Category"], "");
        }
        set
        {
            ViewState["Category"] = value;
        }
    }


    /// <summary>
    /// Returns true if saving of the file failed.
    /// </summary>
    public bool SavingFailed
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["SavingFailed"], false);
        }
    }


    /// <summary>
    /// Returns true if deleting of the file failed.
    /// </summary>
    public bool DeletingFailed
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DeletingFailed"], false);
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
    /// Indicates if uploader is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return uploader.Enabled;
        }
        set
        {
            uploader.Enabled = value;
        }
    }


    /// <summary>
    /// Returns the currently posted file or null when no file posted.
    /// </summary>
    public HttpPostedFile PostedFile
    {
        get
        {
            return uploader.PostedFile;
        }
    }


    /// <summary>
    /// Currently handled meta file.
    /// </summary>
    public MetaFileInfo CurrentlyHandledMetaFile
    {
        get;
        set;
    }


    /// <summary>
    /// Allow modify flag.
    /// </summary>
    protected bool AllowModify
    {
        get
        {
            return Enabled && UserInfoProvider.IsAuthorizedPerObject(ObjectType, ObjectID, PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser);
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs after the meta file was uploaded.
    /// </summary>
    public event EventHandler OnAfterUpload;


    /// <summary>
    /// Raises the OnAfterUpload event.
    /// </summary>
    private void RaiseOnAfterUpload()
    {
        OnAfterUpload?.Invoke(this, EventArgs.Empty);
    }


    /// <summary>
    /// Occurs after the meta file was deleted.
    /// </summary>
    public event EventHandler OnAfterDelete;


    /// <summary>
    /// Raises the OnAfterDelete event.
    /// </summary>
    private void RaiseOnAfterDelete()
    {
        OnAfterDelete?.Invoke(this, EventArgs.Empty);
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register dialog script for Image Editor
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterTooltip(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "OpenImageEditor",
                                               ScriptHelper.GetScript(String.Format(@"
function OpenImageEditor(query) {{ 
    modalDialog('{0}' + query, 'EditImage', 905, 670); 
    return false; 
}}", URLHelper.ResolveUrl("~/CMSModules/Content/CMSDesk/Edit/ImageEditor.aspx"))));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "OpenEditor",
                                               ScriptHelper.GetScript(String.Format(@"
function OpenEditor(query) {{ 
    modalDialog('{0}' + query, 'EditMetadata', 500, 350); 
    return false; 
}} ", URLHelper.ResolveUrl("~/CMSModules/AdminControls/Controls/MetaFiles/MetaDataEditor.aspx"))));
        // Register javascript 'postback' function
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PostBack", ScriptHelper.GetScript(String.Format(@"
function UpdatePage(){{ 
    {0}; 
}}", Page.ClientScript.GetPostBackEventReference(hdnPostback, ""))));


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
            ControlsHelper.GetPostBackEventReference(hdnFullPostback, ""),
            ControlsHelper.GetPostBackEventReference(hdnPostback, ""),
            ScriptHelper.GetString(GetString("general.confirmdelete")));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MetafileScripts_" + ClientID, ScriptHelper.GetScript(script));

        BasicInit();
    }


    private void BasicInit()
    {
        // Init actions even if control is disabled. Enabled may be set late in some cases (team development).
        gridFile.OnAction += gridFile_OnAction;
        gridFile.OnExternalDataBound += gridFile_OnExternalDataBound;

        uploader.OnUploadFile += uploader_OnUploadFile;
        uploader.OnDeleteFile += uploader_OnDeleteFile;

        // Initialize UniGrid only if ObjectID is present
        if (ObjectID > 0)
        {
            gridFile.IsLiveSite = IsLiveSite;
            gridFile.WhereCondition = MetaFileInfoProvider.GetWhereCondition(ObjectID, ObjectType, Category);
            gridFile.StopProcessing = StopProcessing;
            gridFile.GridView.CssClass += " table-width-30";

            pnlGrid.Visible = true;
            pnlAttachmentList.CssClass = "AttachmentsList SingleAttachment";
        }
        else
        {
            pnlGrid.Visible = false;
        }
    }


    private void SetupControls()
    {
        ClearControl();

        if ((ObjectID > 0) && (ObjectType != "") && (Category != ""))
        {
            gridFile.ReloadData();
        }

        if (ObjectID > 0)
        {
            if (AllowModify)
            {
                // Initialize button for adding metafile
                newMetafileElem.ObjectID = ObjectID;
                newMetafileElem.ObjectType = ObjectType;
                newMetafileElem.Category = Category;
                newMetafileElem.ParentElemID = ClientID;
                newMetafileElem.SiteID = SiteID;
                newMetafileElem.InnerLoadingElementClass = "NewAttachmentLoading";
                if (AllowedExtensions != null)
                {
                    newMetafileElem.AllowedExtensions = AllowedExtensions;
                }
            }
            newMetafileElem.ForceLoad = true;
            newMetafileElem.Text = GetString("attach.uploadfile");
            newMetafileElem.InnerElementClass = "NewAttachment";
            newMetafileElem.IsLiveSite = IsLiveSite;
            newMetafileElem.SourceType = MediaSourceEnum.MetaFile;
            newMetafileElem.Visible = true;

            newMetafileElem.Enabled = AllowModify;
            plcOldUploader.Visible = false;
        }
        else
        {
            newMetafileElem.Visible = false;
            plcUploader.Visible = false;
            plcOldUploader.Visible = true;
        }
    }


    /// <summary>
    /// Reloads file uploader.
    /// </summary>
    public void ReloadData()
    {
        BasicInit();
        SetupControls();
    }


    /// <summary>
    /// Inits uploaded file name based on file name and GUID.
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="fileGuid">File GUID</param>
    public void InitUploader(string fileName, Guid fileGuid)
    {
        uploader.CurrentFileName = Path.GetFileName(fileName);
        uploader.CurrentFileUrl = "~/CMSPages/GetMetaFile.aspx?fileguid=" + fileGuid;
    }


    protected object gridFile_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        GridViewRow gvr;
        DataRowView drv;
        string fileGuid;

        switch (sourceName?.ToLowerInvariant())
        {
            case "edit":
                if (sender is CMSAccessibleButton)
                {
                    gvr = (GridViewRow)parameter;
                    drv = (DataRowView)gvr.DataItem;

                    fileGuid = ValidationHelper.GetString(drv["MetaFileGUID"], "");
                    string fileExtension = ValidationHelper.GetString(drv["MetaFileExtension"], "");

                    // Initialize properties
                    CMSGridActionButton btnImageEditor = (CMSGridActionButton)sender;
                    btnImageEditor.Visible = true;

                    // Display button only if 'Modify' is allowed
                    if (AllowModify)
                    {
                        string query = $"?refresh=1&metafileguid={fileGuid}&clientid={ClientID}";
                        query = URLHelper.AddUrlParameter(query, "hash", QueryHelper.GetHash(query));

                        // Display button only if metafile is in supported image format
                        if (ImageHelper.IsSupportedByImageEditor(fileExtension))
                        {
                            // Initialize button with script
                            btnImageEditor.OnClientClick = $"OpenImageEditor({ScriptHelper.GetString(query)}); return false;";
                        }
                        // Non-image metafile
                        else
                        {
                            // Initialize button with script
                            btnImageEditor.OnClientClick = $"OpenEditor({ScriptHelper.GetString(query)}); return false;";
                        }
                    }
                    else
                    {
                        btnImageEditor.Enabled = false;
                    }
                }
                break;

            case "delete":
                if (sender is CMSGridActionButton)
                {
                    CMSGridActionButton btnDelete = (CMSGridActionButton)sender;
                    btnDelete.Enabled = AllowModify;
                    
                }
                break;

            case "name":
                drv = (DataRowView)parameter;

                string fileName = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "MetaFileName"), string.Empty);
                fileGuid = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "MetaFileGUID"), string.Empty);
                string fileExt = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "MetaFileExtension"), string.Empty);

                bool isImage = ImageHelper.IsImage(fileExt);
                string fileUrl = $"{URLHelper.ResolveUrl("~/CMSPages/GetMetaFile.aspx")}?fileguid={fileGuid}&chset={Guid.NewGuid()}";

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
                    return $"<a href=\"#\" onclick=\"javascript: window.open('{fileUrl}'); return false;\" class=\"cms-icon-link\"><span id=\"{fileGuid}\" {tooltip}>{iconTag}{fileName}</span></a>";
                }
                else
                {
                    return $"<a href=\"{fileUrl}\" class=\"cms-icon-link\"><span id=\"{fileGuid}\" {tooltip}>{iconTag}{fileName}</span></a>";
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


    protected void gridFile_OnAction(string actionName, object actionArgument)
    {
        switch (actionName?.ToLowerInvariant())
        {
            case "delete":
                try
                {
                    // Delete the meta file
                    int metaFileId = ValidationHelper.GetInteger(actionArgument, 0);
                    MetaFileInfo.Provider.Get(metaFileId)?.Delete();

                    RaiseOnAfterDelete();
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
                break;
        }
    }


    protected void uploader_OnUploadFile(object sender, EventArgs e)
    {
        UploadFile();
    }


    protected void uploader_OnDeleteFile(object sender, EventArgs e)
    {
        // Careful with upload and delete in on postback - ignore delete request
        if (mAlreadyUploadedDontDelete)
        {
            return;
        }

        try
        {
            using (DataSet ds = MetaFileInfoProvider.GetMetaFiles(ObjectID, ObjectType, Category, null, null))
            {
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var mfi = new MetaFileInfo(dr);
                        if (string.Equals(mfi.MetaFileName, uploader.CurrentFileName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            MetaFileInfo.Provider.Get(mfi.MetaFileID)?.Delete();
                        }
                    }
                }
            }

            RaiseOnAfterDelete();

            SetupControls();
        }
        catch (Exception ex)
        {
            ViewState["DeletingFailed"] = true;
            ShowError(ex.Message);
            SetupControls();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        SetupControls();

        // Hide update column if modify is not enabled
        if (gridFile.NamedColumns.ContainsKey("Update"))
        {
            gridFile.NamedColumns["Update"].Visible = AllowModify;
        }

        if (ObjectID > 0)
        {
            bool gridHasData = !DataHelper.DataSourceIsEmpty(gridFile.GridView.DataSource);

            // Ensure uploader button
            plcUploader.Visible = !gridHasData;
        }
    }


    /// <summary>
    /// Returns true if entered data is valid. If data is invalid, it returns false and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        if ((plcOldUploader.Visible) && (uploader.PostedFile != null))
        {
            string extensions = string.IsNullOrEmpty(AllowedExtensions) ? SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSUploadExtensions") : AllowedExtensions;
            if (extensions != String.Empty)
            {
                string extension = Path.GetExtension(uploader.PostedFile.FileName).TrimStart('.').ToLowerInvariant();
                string haystack = $";{extensions};";
                string needle = $";{extension};";

                if (!haystack.Contains(needle))
                {
                    string format = GetString("attach.notallowedextension");
                    ValidationError = String.Format(format, extension, extensions);
                    return false;
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Uploads file.
    /// </summary>
    public void UploadFile()
    {
        if ((uploader.PostedFile != null) && (ObjectID > 0))
        {
            try
            {
                MetaFileInfo existing = null;

                // Check if uploaded file already exists and delete it
                DataSet ds = MetaFileInfoProvider.GetMetaFiles(ObjectID, ObjectType, Category, null, null);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    // Get existing record ID and delete it
                    existing = new MetaFileInfo(ds.Tables[0].Rows[0]);
                    MetaFileInfo.Provider.Delete(existing);
                }

                // Create new meta file
                MetaFileInfo mfi = new MetaFileInfo(uploader.PostedFile, ObjectID, ObjectType, Category);
                if (existing != null)
                {
                    // Preserve GUID
                    mfi.MetaFileGUID = existing.MetaFileGUID;
                    mfi.MetaFileTitle = existing.MetaFileTitle;
                    mfi.MetaFileDescription = existing.MetaFileDescription;
                }
                mfi.MetaFileSiteID = SiteID;

                // Save to the database
                MetaFileInfo.Provider.Set(mfi);

                CurrentlyHandledMetaFile = mfi;
                RaiseOnAfterUpload();

                SetupControls();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                ViewState["SavingFailed"] = true;
                SetupControls();
            }

            // File was uploaded, do not delete in one postback
            mAlreadyUploadedDontDelete = true;
        }
    }


    /// <summary>
    /// Clears the content (file name & file URL) of the control.
    /// </summary>
    public void ClearControl()
    {
        uploader.CurrentFileName = string.Empty;
        uploader.CurrentFileUrl = string.Empty;
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

                gridFile.ReloadData();
                updPanel.Update();
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    #endregion
}