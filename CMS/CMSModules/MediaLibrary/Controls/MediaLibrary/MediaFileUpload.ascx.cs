using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

using Image = System.Drawing.Image;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileUpload : CMSUserControl
{
    #region "Variables"

    private string mInnerDivClass;
    private string mInnerLoadingDivClass;

    private MediaLibraryInfo mLibraryInfo;
    private MediaFileInfo mFileInfo;

    private string previewPath;
    private string previewName;
    private string previewExt;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value which indicates whether the control should be enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["Enabled"], true);
        }
        set
        {
            ViewState["Enabled"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which indicates whether update control shold be be enabled.
    /// </summary>
    public bool EnableUpdate
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["EnableUpdate"], true);
        }
        set
        {
            ViewState["EnableUpdate"] = value;
        }
    }


    /// <summary>
    /// ID of the current library.
    /// </summary>
    public int LibraryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["LibraryID"], 0);
        }
        set
        {
            ViewState["LibraryID"] = value;
            if (newFileElem != null)
            {
                newFileElem.LibraryID = value;
            }
        }
    }


    /// <summary>
    /// Info on library media files are created for.
    /// </summary>
    public MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (LibraryID > 0))
            {
                mLibraryInfo = MediaLibraryInfo.Provider.Get(LibraryID);
            }
            return mLibraryInfo;
        }
    }


    /// <summary>
    /// ID of the media file.
    /// </summary>
    public int MediaFileID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["MediaFileID"], 0);
        }
        set
        {
            ViewState["MediaFileID"] = value;
            if (newFileElem != null)
            {
                newFileElem.MediaFileID = value;
            }
        }
    }


    /// <summary>
    /// Info on currently processed media file.
    /// </summary>
    public MediaFileInfo FileInfo
    {
        get
        {
            if ((mFileInfo == null) && (MediaFileID > 0))
            {
                mFileInfo = MediaFileInfo.Provider.Get(MediaFileID);
            }
            return mFileInfo;
        }
        set
        {
            mFileInfo = value;
        }
    }


    /// <summary>
    /// Determines whether the uploader should upload media file thumbnail or basic media file.
    /// </summary>
    public bool IsMediaThumbnail
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsMediaThumbnail"], false);
        }
        set
        {
            ViewState["IsMediaThumbnail"] = value;
            if (newFileElem != null)
            {
                newFileElem.IsMediaThumbnail = value;
            }
        }
    }


    /// <summary>
    /// Folder path of the current library.
    /// </summary>
    public string LibraryFolderPath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["LibraryFolderPath"], "");
        }
        set
        {
            ViewState["LibraryFolderPath"] = value;
            if (newFileElem != null)
            {
                newFileElem.LibraryFolderPath = value;
            }
        }
    }


    /// <summary>
    /// CSS class of the new attachment link.
    /// </summary>
    public string InnerDivClass
    {
        get
        {
            return (String.IsNullOrEmpty(mInnerDivClass) ? "NewAttachment" : mInnerDivClass);
        }
        set
        {
            mInnerDivClass = value;
        }
    }


    /// <summary>
    /// CSS class of the new attachment loading element.
    /// </summary>
    public string InnerLoadingDivClass
    {
        get
        {
            return (String.IsNullOrEmpty(mInnerLoadingDivClass) ? "NewAttachmentLoading" : mInnerLoadingDivClass);
        }
        set
        {
            mInnerLoadingDivClass = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for tooltips
        ScriptHelper.RegisterTooltip(Page);

        string refreshScript =
            @"function RefreshUpdatePanel(hiddenFieldID, action) {
                var hiddenField = document.getElementById(hiddenFieldID);
                if (hiddenField) {
                    __doPostBack(hiddenFieldID, action);
                }
            }

            function FullRefresh(hiddenFieldID, action) {
                if(RefreshTree != null)
                {
                    RefreshTree();
                }

                RefreshUpdatePanel(hiddenFieldID, action);
            }

            function FullPageRefresh_" + ClientID + @"(guid) {
                if(RefreshTree != null)
                {
                    RefreshTree();
                }

                var hiddenField = document.getElementById('" + hdnFullPostback.ClientID + "');" +
            @"if (hiddenField) {
                    __doPostBack('" + hdnFullPostback.ClientID + "', 'refresh|' + guid);" +
            @"}
            }";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(Page), "RefreshUpdatePanel", ScriptHelper.GetScript(refreshScript));

        // Initialize refresh script for update panel
        string initRefreshScript =
            "function InitRefresh_" + ClientID + "(msg, fullRefresh, guid, action)\n" +
            "{\n" +
            "   if((msg != null) && (msg != \"\")){ alert(msg); action='error'; }\n" +
            "   if(fullRefresh) { FullRefresh('" + hdnFullPostback.ClientID + "', action + '|' + guid); }\n" +
            "   else { RefreshUpdatePanel('" + hdnPostback.ClientID + "', action + '|' + guid); }\n" +
            "}\n";
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(Page), "AfterUploadRefresh", ScriptHelper.GetScript(initRefreshScript));

        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);

        string editorUrl;

        const string MEDIA_LIBRARY_FOLDER = "~/CMSModules/MediaLibrary/";

        if (IsLiveSite)
        {
            if (AuthenticationHelper.IsAuthenticated())
            {
                editorUrl = UrlResolver.ResolveUrl("~/CMS/Dialogs/CMSModules/MediaLibrary/CMSPages/ImageEditor.aspx");
            }
            else
            {
                editorUrl = UrlResolver.ResolveUrl(MEDIA_LIBRARY_FOLDER + "CMSPages/ImageEditor.aspx");
            }
        }
        else
        {
            editorUrl = UrlResolver.ResolveUrl(MEDIA_LIBRARY_FOLDER + "Controls/MediaLibrary/ImageEditor.aspx");
        }

        // Dialog for editing image
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(Page), "EditThumbnailImage",
                                               ScriptHelper.GetScript("function EditThumbnailImage(query) { " +
                                                                      "modalDialog('" + editorUrl + "' + query, 'imageEditorDialog', 905, 670); " +
                                                                      " } "
                                                   ));
        // Grid initialization
        gridAttachments.IsLiveSite = IsLiveSite;
        gridAttachments.Visible = true;
        gridAttachments.OnExternalDataBound += GridOnExternalDataBound;
        gridAttachments.OnAction += GridOnAction;
        pnlGrid.Attributes.Add("style", "padding-top: 2px;");

        // Load data
        ReloadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblError.Visible = (lblError.Text != "");
        lblInfo.Visible = (lblInfo.Text != "");

        // Ensure correct layout
        bool gridHasData = !DataHelper.DataSourceIsEmpty(gridAttachments.DataSource);
        pnlGrid.Visible = gridHasData;
        plcUploaderDisabled.Visible = !gridHasData && !Enabled;
        plcUploader.Visible = !gridHasData;

        // Initialize button for adding files
        plcUploader.Visible = !gridHasData;
    }

    #endregion


    #region "Private & protected methods"

    public void ReloadData()
    {
        if (StopProcessing)
        {
            newFileElem.StopProcessing = true;
            gridAttachments.StopProcessing = true;
        }
        else
        {
            // Initialize button for adding attachments
            newFileElem.DisplayInline = false;
            newFileElem.Text = GetString("attach.uploadfile");
            newFileElem.InnerElementClass = InnerDivClass;
            newFileElem.InnerLoadingElementClass = InnerLoadingDivClass;
            newFileElem.IsLiveSite = IsLiveSite;
            newFileElem.SourceType = MediaSourceEnum.MediaLibraries;
            newFileElem.MediaFileID = MediaFileID;
            newFileElem.ParentElemID = ClientID;
            newFileElem.LibraryFolderPath = LibraryFolderPath;
            newFileElem.IsMediaThumbnail = IsMediaThumbnail;
            newFileElem.LibraryID = LibraryID;
            newFileElem.ReloadData();

            // Get preview info
            if (IsMediaThumbnail && (FileInfo != null))
            {
                SiteInfo si = SiteInfo.Provider.Get(LibraryInfo.LibrarySiteID);
                if (si != null)
                {
                    previewPath = MediaFileInfoProvider.GetPreviewFilePath(FileInfo.FilePath, si.SiteName, LibraryInfo.LibraryID);
                    if (previewPath.Length < 260)
                    {
                        string previewFolder = Path.GetDirectoryName(previewPath);
                        if (Directory.Exists(previewFolder))
                        {
                            string[] files = Directory.GetFiles(previewFolder, Path.GetFileName(previewPath));
                            if (files.Length > 0)
                            {
                                previewPath = files[0];
                                previewName = Path.GetFileNameWithoutExtension(previewPath);
                                previewExt = Path.GetExtension(previewPath).TrimStart('.');
                            }
                            else
                            {
                                previewPath = "";
                            }
                        }
                        else
                        {
                            previewPath = "";
                        }
                    }
                    else
                    {
                        previewPath = "";
                    }
                }
            }

            // Bind UniGrid to DataSource
            gridAttachments.GridView.AllowPaging = false;
            gridAttachments.GridView.AllowSorting = false;

            // Get the data
            if (IsMediaThumbnail)
            {
                if (!string.IsNullOrEmpty(previewPath))
                {
                    // Create DataSet manually for preview
                    FileInfo file = CMS.IO.FileInfo.New(previewPath);
                    if (file.Exists)
                    {
                        DataSet ds = new DataSet();
                        DataTable table = ds.Tables.Add();
                        table.Columns.Add("FileID", typeof(int));
                        table.Columns.Add("FileSize", typeof(long));
                        table.Columns.Add("FileName", typeof(string));
                        table.Rows.Add(0, file.Length, previewName);
                        gridAttachments.DataSource = ds;
                    }
                }
                else
                {
                    gridAttachments.DataSource = null;
                }
            }
            else
            {
                gridAttachments.DataSource = MediaFileInfoProvider.GetMediaFiles("FileID = " + MediaFileID);
            }

            gridAttachments.ReloadData();
            updPanel.Update();
        }
    }


    /// <summary>
    /// UniGrid action buttons event handler.
    /// </summary>
    protected void GridOnAction(string actionName, object actionArgument)
    {
        // Process proper action
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                if (IsMediaThumbnail)
                {
                    // Delete thumbnail file
                    if (LibraryInfo != null)
                    {
                        // Check 'File delete' permission
                        if (MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify"))
                        {
                            MediaFileInfoProvider.DeleteMediaFilePreview(SiteContext.CurrentSiteName, LibraryID, FileInfo.FilePath, false);

                            if (FileInfo != null)
                            {
                                SiteInfo si = SiteInfo.Provider.Get(FileInfo.FileSiteID);
                                if (si != null)
                                {
                                    // Log synchronization task
                                    SynchronizationHelper.LogObjectChange(FileInfo, TaskTypeEnum.UpdateObject);
                                }

                                // Drop the cache dependencies
                                CacheHelper.TouchKeys(MediaFileInfoProvider.GetDependencyCacheKeys(FileInfo, true));
                            }
                        }
                        else
                        {
                            lblError.Text = MediaLibraryHelper.GetAccessDeniedMessage("filemodify");
                        }
                    }

                    // Ensure recent action is forgotten
                    gridAttachments.ClearActions();
                }
                else
                {
                    if (LibraryInfo != null)
                    {
                        // Check 'File delete' permission
                        if (MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filedelete"))
                        {
                            // Delete Media File
                            if (FileInfo != null)
                            {
                                MediaFileInfo.Provider.Delete(FileInfo);
                            }
                        }
                    }
                }

                // Force reload data
                ReloadData();
                break;
        }
    }


    /// <summary>
    /// UniGrid external data bound.
    /// </summary>
    protected object GridOnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "update":
                PlaceHolder plcUpd = new PlaceHolder();
                plcUpd.ID = "plcUdateAction";

                // Dynamically load uploader control
                DirectFileUploader dfuElem = Page.LoadUserControl("~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx") as DirectFileUploader;

                // Set uploader's properties
                if (dfuElem != null)
                {
                    dfuElem.ID = "dfuElem" + LibraryID;
                    dfuElem.DisplayInline = true;
                    dfuElem.SourceType = MediaSourceEnum.MediaLibraries;
                    dfuElem.MediaFileID = MediaFileID;
                    dfuElem.LibraryID = LibraryID;
                    dfuElem.LibraryFolderPath = LibraryFolderPath;
                    dfuElem.ParentElemID = ClientID;
                    dfuElem.IsMediaThumbnail = IsMediaThumbnail;
                    dfuElem.ShowIconMode = true;
                    dfuElem.InsertMode = false;
                    dfuElem.ForceLoad = true;
                    dfuElem.IsLiveSite = IsLiveSite;
                    // New settings added
                    dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
                    dfuElem.Height = 16;
                    dfuElem.Width = 16;
                    dfuElem.MaxNumberToUpload = 1;

                    if (Enabled && EnableUpdate)
                    {
                        dfuElem.Enabled = true;
                    }
                    else
                    {
                        dfuElem.Enabled = false;
                    }

                    plcUpd.Controls.Add(dfuElem);
                }
                return plcUpd;

            case "edit":
                // Get file extension
                if ((FileInfo != null) && (LibraryInfo != null))
                {
                    var editButton = (CMSGridActionButton)sender;

                    if (AuthenticationHelper.IsAuthenticated())
                    {
                        string fileExt = (IsMediaThumbnail ? previewExt : FileInfo.FileExtension);

                        // If the file is not an image don't allow image editing
                        if (!ImageHelper.IsSupportedByImageEditor(fileExt) || !Enabled)
                        {
                            // Disable edit icon in case that attachment is not an image
                            editButton.Enabled = false;
                        }
                        else
                        {
                            string query = string.Format("?refresh=1&siteid={0}&MediaFileGUID={1}{2}", LibraryInfo.LibrarySiteID, FileInfo.FileGUID, (IsMediaThumbnail ? "&isPreview=1" : ""));
                            query = URLHelper.AddUrlParameter(query, "hash", QueryHelper.GetHash(query));
                            editButton.OnClientClick = "EditThumbnailImage('" + query + "'); return false;";
                        }
                        editButton.ToolTip = GetString("general.edit");
                    }
                    else
                    {
                        editButton.Visible = false;
                    }
                }
                break;

            case "delete":
                var deleteButton = (CMSGridActionButton)sender;

                if (!Enabled)
                {
                    // Disable delete icon in case that editing is not allowed
                    deleteButton.Enabled = false;
                }

                break;

            case "filename":
                if ((LibraryInfo != null) && (FileInfo != null))
                {
                    string fileUrl = "";
                    string fileExt = "";
                    string fileName = "";

                    // Get file extension
                    if (IsMediaThumbnail)
                    {
                        fileName = previewName;
                        fileExt = previewExt;
                        fileUrl = ResolveUrl("~/CMSPages/GetMediaFile.aspx?preview=1&fileguid=" + FileInfo.FileGUID);
                    }
                    else
                    {
                        fileExt = FileInfo.FileExtension;
                        fileName = FileInfo.FileName;
                        fileUrl = MediaFileURLProvider.GetMediaFileAbsoluteUrl(FileInfo.FileGUID, AttachmentHelper.GetFullFileName(FileInfo.FileName, FileInfo.FileExtension));
                    }
                    fileUrl = URLHelper.UpdateParameterInUrl(fileUrl, "chset", Guid.NewGuid().ToString());

                    string tooltip = null;
                    string iconTag = UIHelper.GetFileIcon(Page, fileExt, tooltip: fileName);
                    bool isImage = ImageHelper.IsImage(fileExt);

                    if (isImage)
                    {
                        tooltip = "";

                        if (File.Exists(previewPath))
                        {
                            FileStream file = FileStream.New(previewPath, FileMode.Open, FileAccess.Read);
                            Image img = Image.FromStream(file);
                            file.Close();
                            if (img != null)
                            {
                                int[] imgDims = ImageHelper.EnsureImageDimensions(0, 0, 150, img.Width, img.Height);
                                string setRTL = (CultureHelper.IsUICultureRTL() ? ", LEFT, true" : "");
                                tooltip = "onmouseout=\"UnTip()\" onmouseover=\"Tip('<div style=\\'width:" + imgDims[0] + "px; text-align:center;\\'><img src=\\'" + URLHelper.AddParameterToUrl(fileUrl, "maxsidesize", "150") + "\\' alt=\\'" + fileName + "\\' /></div>'" + setRTL + ")\"";

                                // Dispose image
                                img.Dispose();
                            }
                        }
                    }

                    if (isImage)
                    {
                        return "<a href=\"#\" onclick=\"javascript: window.open('" + fileUrl + "'); return false;\" class=\"cms-icon-link\"><span " + tooltip + ">" + iconTag + fileName + "</span></a>";
                    }
                    else
                    {
                        return "<a href=\"" + fileUrl + "\" class=\"cms-icon-link\">" + iconTag + fileName + "</a>";
                    }
                }

                return "";

            case "filesize":
                return DataHelper.GetSizeString(ValidationHelper.GetLong(parameter, 0));
        }

        return parameter;
    }

    #endregion
}
