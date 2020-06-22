using System;
using System.Collections;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Core;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


/// <summary>
/// Image editor for attachments and meta files.
/// </summary>
public partial class CMSAdminControls_ImageEditor_ImageEditor : CMSUserControl
{
    #region "Variables"

    private Guid attachmentGuid = Guid.Empty;
    private Guid metafileGuid = Guid.Empty;

    private DocumentAttachment attachment;
    private TreeNode node;

    private MetaFileInfo metafile;

    private string mCurrentSiteName;
    private bool mRefreshAfterAction = true;
    private string externalControlID;
    private int siteId;
    private bool mEnabled = true;

    private string filePath;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the GUID of the instance of the ImageEditor.
    /// </summary>
    public Guid InstanceGUID
    {
        get
        {
            return baseImageEditor.InstanceGUID;
        }
    }


    /// <summary>
    /// Indicates whether the refresh should be risen after edit action takes place.
    /// </summary>
    public bool RefreshAfterAction
    {
        get
        {
            return mRefreshAfterAction;
        }
        set
        {
            mRefreshAfterAction = value;
        }
    }


    /// <summary>
    /// Returns the site name from query string 'sitename' or 'siteid' if present, otherwise SiteContext.CurrentSiteName.
    /// </summary>
    private string CurrentSiteName
    {
        get
        {
            if (mCurrentSiteName == null)
            {
                mCurrentSiteName = QueryHelper.GetString("sitename", SiteContext.CurrentSiteName);

                siteId = QueryHelper.GetInteger("siteid", 0);

                SiteInfo site = SiteInfo.Provider.Get(siteId);
                if (site != null)
                {
                    mCurrentSiteName = site.SiteName;
                }
            }
            return mCurrentSiteName;
        }
    }


    /// <summary>
    /// Version history ID.
    /// </summary>
    public int VersionHistoryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["VersionHistoryID"], 0);
        }
        set
        {
            ViewState["VersionHistoryID"] = value;
        }
    }


    /// <summary>
    /// Indicates if saving failed.
    /// </summary>
    public bool SavingFailed
    {
        get
        {
            return baseImageEditor.SavingFailed;
        }
        set
        {
            baseImageEditor.SavingFailed = value;
        }
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Loads image URL.
    /// </summary>
    private void baseImageEditor_LoadImageUrl()
    {
        // Use appropriate parameter from URL
        string url;
        if (attachmentGuid != Guid.Empty)
        {
            url = GetAttachmentUrl();
        }
        else if (metafileGuid != Guid.Empty)
        {
            url = GetMetafileUrl();
        }
        else
        {
            // Path to the physical file
            baseImageEditor.AttUrl = filePath;
            return;
        }

        baseImageEditor.AttUrl = AddUniqueVersionParameter(url);
    }


    private static string AddUniqueVersionParameter(string url)
    {
        return URLHelper.UpdateParameterInUrl(url, "chset", Guid.NewGuid().ToString());
    }
    

    /// <summary>
    /// Loads image type from querystring.
    /// </summary>
    private void baseImageEditor_LoadImageType()
    {
        baseImageEditor.ImageType = GetImageType();
    }


    private ImageHelper.ImageTypeEnum GetImageType()
    {
        ImageHelper.ImageTypeEnum imageType = ImageHelper.ImageTypeEnum.None;

        if (attachmentGuid != Guid.Empty)
        {
            imageType = ImageHelper.ImageTypeEnum.Attachment;
        }
        else if (metafileGuid != Guid.Empty)
        {
            imageType = ImageHelper.ImageTypeEnum.Metafile;
        }
        else if (!string.IsNullOrEmpty(filePath))
        {
            imageType = ImageHelper.ImageTypeEnum.PhysicalFile;
        }

        return imageType;
    }


    /// <summary>
    /// Initializes common properties used for processing image.
    /// </summary>
    private void baseImageEditor_InitializeProperties()
    {
        // Process attachment
        switch (baseImageEditor.ImageType)
        {
            // Process physical file
            case ImageHelper.ImageTypeEnum.PhysicalFile:
                {
                    if (!String.IsNullOrEmpty(filePath))
                    {
                        if (CheckPhysicalFilePermissions())
                        {
                            try
                            {
                                // Load the file from disk
                                string physicalPath = Server.MapPath(filePath);
                                byte[] data = File.ReadAllBytes(physicalPath);
                                baseImageEditor.ImgHelper = new ImageHelper(data);
                            }
                            catch
                            {
                                baseImageEditor.LoadingFailed = true;
                                baseImageEditor.ShowError(GetString("img.errors.loading"));
                            }
                        }
                        else
                        {
                            baseImageEditor.LoadingFailed = true;
                            baseImageEditor.ShowError(GetString("img.errors.rights"));
                        }
                    }
                    else
                    {
                        baseImageEditor.LoadingFailed = true;
                        baseImageEditor.ShowError(GetString("img.errors.loading"));
                    }
                }
                break;

            // Process metafile
            case ImageHelper.ImageTypeEnum.Metafile:
                {
                    // Get metafile
                    metafile = MetaFileInfoProvider.GetMetaFileInfoWithoutBinary(metafileGuid, CurrentSiteName, true);

                    // If file is not null and current user is global administrator then set image
                    if (metafile != null)
                    {
                        if (CheckMetafilePermissions())
                        {
                            // Ensure metafile binary data
                            metafile.MetaFileBinary = MetaFileInfoProvider.GetFile(metafile, CurrentSiteName);
                            if (metafile.MetaFileBinary != null)
                            {
                                baseImageEditor.ImgHelper = new ImageHelper(metafile.MetaFileBinary);
                            }
                            else
                            {
                                baseImageEditor.LoadingFailed = true;
                                baseImageEditor.ShowError(GetString("img.errors.loading"));
                            }
                        }
                        else
                        {
                            baseImageEditor.LoadingFailed = true;
                            baseImageEditor.ShowError(GetString("img.errors.rights"));
                        }
                    }
                    else
                    {
                        baseImageEditor.LoadingFailed = true;
                        baseImageEditor.ShowError(GetString("img.errors.loading"));
                    }
                }
                break;

            default:
                {
                    LoadAttachment();

                    if (attachment != null)
                    {
                        // If current user has appropriate permissions then set image
                        if ((node != null) &&
                            (
                                (IsLiveSite && QueryHelper.ValidateHash("hash")) || // Live site checks hash, not permissions
                                (!IsLiveSite && CheckAttachmentPermissions()) // Normal editing checks edit permission for document
                            ))
                        {
                            attachment.Generalized.EnsureBinaryData();

                            if (attachment.AttachmentBinary != null)
                            {
                                baseImageEditor.ImgHelper = new ImageHelper(attachment.AttachmentBinary);
                            }
                            else
                            {
                                baseImageEditor.LoadingFailed = true;
                                baseImageEditor.ShowError(GetString("img.errors.loading"));
                            }
                        }
                        else
                        {
                            baseImageEditor.LoadingFailed = true;
                            baseImageEditor.ShowError(GetString("img.errors.filemodify"));
                        }
                    }
                    else
                    {
                        baseImageEditor.LoadingFailed = true;
                        baseImageEditor.ShowError(GetString("img.errors.loading"));
                    }
                }
                break;
        }

        // Check that image is in supported formats
        if ((!baseImageEditor.LoadingFailed) && (baseImageEditor.ImgHelper.ImageFormatToString() == null))
        {
            baseImageEditor.LoadingFailed = true;
            baseImageEditor.ShowError(GetString("img.errors.format"));
        }

        // Disable editor if loading failed
        if (baseImageEditor.LoadingFailed)
        {
            Enabled = false;
        }
    }


    /// <summary>
    /// Initialize labels according to current image type.
    /// </summary>
    private void baseImageEditor_InitializeLabels(bool reloadName)
    {
        //Initialize strings depending on image type
        switch (baseImageEditor.ImageType)
        {
            case ImageHelper.ImageTypeEnum.PhysicalFile:
                {
                    if (!String.IsNullOrEmpty(filePath))
                    {
                        if (!RequestHelper.IsPostBack())
                        {
                            baseImageEditor.TxtFileName.Text = Path.GetFileNameWithoutExtension(filePath);
                        }
                        baseImageEditor.LblExtensionValue.Text = Path.GetExtension(filePath);

                        if (baseImageEditor.ImgHelper != null)
                        {
                            ImageHelper img = baseImageEditor.ImgHelper;
                            baseImageEditor.LblImageSizeValue.Text = DataHelper.GetSizeString(img.SourceData.Length);
                            baseImageEditor.LblWidthValue.Text = img.ImageWidth.ToString();
                            baseImageEditor.LblHeightValue.Text = img.ImageHeight.ToString();
                        }
                    }
                }
                break;

            case ImageHelper.ImageTypeEnum.Metafile:
                if (metafile != null)
                {
                    if (!RequestHelper.IsPostBack())
                    {
                        baseImageEditor.TxtFileName.Text = metafile.MetaFileName.Remove(metafile.MetaFileName.LastIndexOf(".", StringComparison.Ordinal));
                    }
                    baseImageEditor.LblExtensionValue.Text = metafile.MetaFileExtension.Substring(1, (metafile.MetaFileExtension.Length - 1));
                    baseImageEditor.LblImageSizeValue.Text = DataHelper.GetSizeString(metafile.MetaFileSize);
                    baseImageEditor.LblWidthValue.Text = metafile.MetaFileImageWidth.ToString();
                    baseImageEditor.LblHeightValue.Text = metafile.MetaFileImageHeight.ToString();
                    baseImageEditor.SetTitleAndDescription(metafile.MetaFileTitle, metafile.MetaFileDescription);
                    // Set metafile info object
                    baseImageEditor.SetMetaDataInfoObject(metafile);
                }
                break;

            default:
                if (attachment != null)
                {
                    if (reloadName)
                    {
                        baseImageEditor.TxtFileName.Text = attachment.AttachmentName.Remove(attachment.AttachmentName.LastIndexOf(".", StringComparison.Ordinal));
                    }
                    baseImageEditor.LblExtensionValue.Text = attachment.AttachmentExtension.Substring(1, (attachment.AttachmentExtension.Length - 1));
                    baseImageEditor.LblImageSizeValue.Text = DataHelper.GetSizeString(attachment.AttachmentSize);
                    baseImageEditor.LblWidthValue.Text = attachment.AttachmentImageWidth.ToString();
                    baseImageEditor.LblHeightValue.Text = attachment.AttachmentImageHeight.ToString();
                    baseImageEditor.SetTitleAndDescription(attachment.AttachmentTitle, attachment.AttachmentDescription);
                    // Set attachment info object
                    baseImageEditor.SetMetaDataInfoObject(attachment);
                }
                break;
        }
    }


    /// <summary>
    /// Saves modified image data.
    /// </summary>
    /// <param name="name">Image name</param>
    /// <param name="extension">Image extension</param>
    /// <param name="mimetype">Image mimetype</param>
    /// <param name="title">Image title</param>
    /// <param name="description">Image description</param>
    /// <param name="binary">Image binary data</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    private void baseImageEditor_SaveImage(string name, string extension, string mimetype, string title, string description, byte[] binary, int width, int height)
    {
        SaveImage(name, extension, mimetype, title, description, binary, width, height);
    }


    /// <summary>
    /// Returns image name, title and description according to image type.
    /// </summary>
    /// <returns>Image name, title and description</returns>
    private void baseImageEditor_GetMetaData()
    {
        LoadInfos();

        string name = string.Empty;
        string title = string.Empty;
        string description = string.Empty;

        switch (baseImageEditor.ImageType)
        {
            case ImageHelper.ImageTypeEnum.Attachment:
                if (attachment != null)
                {
                    name = Path.GetFileNameWithoutExtension(attachment.AttachmentName);
                    title = attachment.AttachmentTitle;
                    description = attachment.AttachmentDescription;
                }
                break;

            case ImageHelper.ImageTypeEnum.PhysicalFile:
                if (!String.IsNullOrEmpty(filePath))
                {
                    name = Path.GetFileNameWithoutExtension(filePath);
                }
                break;

            case ImageHelper.ImageTypeEnum.Metafile:
                if (metafile != null)
                {
                    name = Path.GetFileNameWithoutExtension(metafile.MetaFileName);
                    title = metafile.MetaFileTitle;
                    description = metafile.MetaFileDescription;
                }
                break;
        }

        baseImageEditor.GetNameResult = name;
        baseImageEditor.GetTitleResult = title;
        baseImageEditor.GetDescriptionResult = description;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize GUID from query
        attachmentGuid = QueryHelper.GetGuid("attachmentguid", Guid.Empty);
        metafileGuid = QueryHelper.GetGuid("metafileguid", Guid.Empty);
        if (!RequestHelper.IsPostBack())
        {
            VersionHistoryID = QueryHelper.GetInteger("VersionHistoryID", 0);
        }
        externalControlID = QueryHelper.GetString("clientid", null);

        String identifier = QueryHelper.GetString("identifier", null);
        if (!String.IsNullOrEmpty(identifier))
        {
            Hashtable props = WindowHelper.GetItem(identifier) as Hashtable;
            if (props != null)
            {
                filePath = ValidationHelper.GetString(props["filepath"], null);
            }
        }

        string siteName = SiteContext.CurrentSiteName;
        var filesLocationType = FileHelper.FilesLocationType(siteName);

        // Check only if store files in file system is enabled
        if (filesLocationType == FilesLocationTypeEnum.FileSystem)
        {
            // Get path to media file folder
            string path = DirectoryHelper.CombinePath(Server.MapPath("~/"), siteName);

            //  Attachments folder
            if (attachmentGuid != Guid.Empty)
            {
                path = DirectoryHelper.CombinePath(path, "files");
            }

            // Metafiles folder
            if (metafileGuid != Guid.Empty)
            {
                path = DirectoryHelper.CombinePath(path, "metafiles");
            }

            // Enable control if permissions are sufficient to edit image
            Enabled = DirectoryHelper.CheckPermissions(path, false, true, true, true);

            if (!Enabled)
            {
                // Set error message
                baseImageEditor.ShowError(GetString("img.errors.filesystempermissions"));
            }
        }

        baseImageEditor.LoadImageType += baseImageEditor_LoadImageType;
        baseImageEditor.LoadImageUrl += baseImageEditor_LoadImageUrl;
        baseImageEditor.InitializeProperties += baseImageEditor_InitializeProperties;
        baseImageEditor.InitializeLabels += baseImageEditor_InitializeLabels;
        baseImageEditor.SaveImage += baseImageEditor_SaveImage;
        baseImageEditor.GetMetaData += baseImageEditor_GetMetaData;
    }
    

    /// <summary>
    /// Saves modified image data.
    /// </summary>
    /// <param name="name">Image name</param>
    /// <param name="extension">Image extension</param>
    /// <param name="mimetype">Image mimetype</param>
    /// <param name="title">Image title</param>
    /// <param name="description">Image description</param>
    /// <param name="binary">Image binary data</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    private void SaveImage(string name, string extension, string mimetype, string title, string description, byte[] binary, int width, int height)
    {
        LoadInfos();

        // Save image data depending to image type
        switch (baseImageEditor.ImageType)
        {
            // Process attachment
            case ImageHelper.ImageTypeEnum.Attachment:
                if (attachment != null)
                {
                    if (!CheckAttachmentPermissions())
                    {
                        ReportSavingNotAuthorized();
                        return;
                    }

                    try
                    {
                        UpdateAttachmentNameAndExtension(name, extension);

                        if (!IsAttachmentNameUnique())
                        {
                            baseImageEditor.ShowError(GetString("img.namenotunique"));
                            SavingFailed = true;

                            return;
                        }

                        UpdateAttachmentProperties(mimetype, title, description, binary, width, height);

                        if (attachment.AttachmentFormGUID == Guid.Empty)
                        {
                            SaveDocumentAttachment();
                        }
                        else
                        {
                            SaveTemporaryAttachment();
                        }
                    }
                    catch (Exception ex)
                    {
                        ReportSavingFailed(ex);
                    }
                }
                break;

            case ImageHelper.ImageTypeEnum.PhysicalFile:
                if (!String.IsNullOrEmpty(filePath))
                {
                    if (!CheckPhysicalFilePermissions())
                    {
                        ReportSavingNotAuthorized();
                        return;
                    }

                    try
                    {
                        string physicalPath = Server.MapPath(filePath);
                        string newPath = physicalPath;

                        // Write binary data to the disk
                        if (binary != null)
                        {
                            File.WriteAllBytes(physicalPath, binary);
                        }

                        // Handle rename of the file
                        if (!String.IsNullOrEmpty(name))
                        {
                            newPath = DirectoryHelper.CombinePath(Path.GetDirectoryName(physicalPath), name);
                        }
                        if (!String.IsNullOrEmpty(extension))
                        {
                            string oldExt = Path.GetExtension(physicalPath);
                            newPath = newPath.Substring(0, newPath.Length - oldExt.Length) + extension;
                        }

                        // Move the file
                        if (newPath != physicalPath)
                        {
                            File.Move(physicalPath, newPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        ReportSavingFailed(ex);
                    }
                }
                break;

            // Process metafile
            case ImageHelper.ImageTypeEnum.Metafile:

                if (metafile != null)
                {
                    if (!CheckMetafilePermissions())
                    {
                        ReportSavingNotAuthorized();
                        return;
                    }

                    try
                    {
                        UpdateMetafileProperties(name, extension, mimetype, title, description, binary, width, height);

                        // Save new data
                        MetaFileInfo.Provider.Set(metafile);

                        if (RefreshAfterAction)
                        {
                            InitRefreshAfterAction(metafile.MetaFileGUID);
                        }
                    }
                    catch (Exception ex)
                    {
                        ReportSavingFailed(ex);
                    }
                }
                break;
        }
    }


    private void ReportSavingNotAuthorized()
    {
        baseImageEditor.ShowError(GetString("img.errors.rights"));
        SavingFailed = true;
    }


    private void ReportSavingFailed(Exception ex)
    {
        baseImageEditor.ShowError(GetString("img.errors.processing"), tooltipText: ex.Message);
        Service.Resolve<IEventLogService>().LogException("Image editor", "SAVEIMAGE", ex);
        SavingFailed = true;
    }


    private static bool CheckPhysicalFilePermissions()
    {
        return MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
    }


    /// <summary>
    /// Initializes refresh script after dialog is closed.
    /// </summary>
    /// <param name="imageGuid">Image GUID</param>
    private void InitRefreshAfterAction(Guid imageGuid)
    {
        var script = String.IsNullOrEmpty(externalControlID)
            ? "Refresh();"
            : String.Format("InitRefresh({0}, false, false, '{1}', 'refresh')", ScriptHelper.GetString(externalControlID), imageGuid);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshAfterAction", script, true);
    }


    /// <summary>
    /// Ensures the info objects.
    /// </summary>
    private void LoadInfos()
    {
        switch (baseImageEditor.ImageType)
        {
            case ImageHelper.ImageTypeEnum.Metafile:
                LoadMetafile();
                break;

            case ImageHelper.ImageTypeEnum.PhysicalFile:
                // Skip loading info for physical files
                break;

            default:
                LoadAttachment();
                break;
        }
    }

    #endregion


    #region "Metafile methods"

    private void LoadMetafile()
    {
        if (metafile == null)
        {
            metafile = MetaFileInfoProvider.GetMetaFileInfoWithoutBinary(metafileGuid, CurrentSiteName, true);
        }
    }


    private string GetMetafileUrl()
    {
        return "~/CMSPages/GetMetaFile.aspx?fileguid=" + metafileGuid + "&sitename=" + CurrentSiteName;
    }
        

    private bool CheckMetafilePermissions()
    {
        return UserInfoProvider.IsAuthorizedPerObject(metafile.MetaFileObjectType, metafile.MetaFileObjectID, PermissionsEnum.Modify, CurrentSiteName, MembershipContext.AuthenticatedUser);
    }


    private void UpdateMetafileProperties(string name, string extension, string mimetype, string title, string description, byte[] binary, int width, int height)
    {
        UpdateMetafileNameAndExtension(name, extension);

        if (mimetype != string.Empty)
        {
            metafile.MetaFileMimeType = mimetype;
        }

        metafile.MetaFileTitle = title;
        metafile.MetaFileDescription = description;

        if (binary != null)
        {
            metafile.MetaFileBinary = binary;
            metafile.MetaFileSize = binary.Length;
        }

        UpdateMetafileSize(width, height);
    }

    private void UpdateMetafileSize(int width, int height)
    {
        if (width > 0)
        {
            metafile.MetaFileImageWidth = width;
        }
        if (height > 0)
        {
            metafile.MetaFileImageHeight = height;
        }
    }

    private void UpdateMetafileNameAndExtension(string name, string extension)
    {
        // Test all parameters to empty values and update new value if available
        if (name != string.Empty)
        {
            if (!name.EndsWith(extension, StringComparison.Ordinal))
            {
                metafile.MetaFileName = name + extension;
            }
            else
            {
                metafile.MetaFileName = name;
            }
        }

        if (extension != string.Empty)
        {
            metafile.MetaFileExtension = extension;
        }
    }

    #endregion


    #region "Attachment methods"

    private string GetAttachmentUrl()
    {
        int documentId = (attachment != null) ? attachment.AttachmentDocumentID : 0;
        bool useLatestDoc = (IsLiveSite && (documentId > 0));

        var url = "~/CMSPages/GetFile.aspx?guid=" + attachmentGuid + "&sitename=" + CurrentSiteName;
        if ((VersionHistoryID != 0) && !useLatestDoc)
        {
            url += "&versionhistoryid=" + VersionHistoryID;
        }

        // Add latest version requirement for live site
        if (useLatestDoc)
        {
            // Add requirement for latest version of files for current document
            string newparams = "latestfordocid=" + documentId;
            newparams += "&hash=" + ValidationHelper.GetHashString("d" + documentId, new HashSettings(""));

            url += "&" + newparams;
        }

        return url;
    }


    private void LoadAttachment()
    {
        if (attachment == null)
        {
            baseImageEditor.Tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            attachment = DocumentHelper.GetAttachment(attachmentGuid, CurrentSiteName, true, out node);

            if ((attachment != null) && (attachment.AttachmentFormGUID != Guid.Empty))
            {
                // Get parent node ID in case attachment is edited for document not created yet (temporary attachment)
                int parentNodeId = QueryHelper.GetInteger("parentId", 0);

                node = baseImageEditor.Tree.SelectSingleNode(parentNodeId);
            }
        }
    }


    private bool IsAttachmentNameUnique()
    {
        if (attachment != null)
        {
            // Use correct identifier if attachment is under workflow
            int identifier = VersionHistoryID > 0 ? GetAttachmentHistoryId() : attachment.AttachmentID;

            // Check that the name is unique in the document or version context
            Guid attachmentFormGuid = QueryHelper.GetGuid("formguid", Guid.Empty);
            bool nameIsUnique;
            if ((attachment.AttachmentFormGUID == Guid.Empty) || (attachmentFormGuid == Guid.Empty))
            {
                // Get the node
                nameIsUnique = DocumentHelper.AttachmentHasUniqueName(node, attachment);
            }
            else
            {
                nameIsUnique = AttachmentInfoProvider.IsUniqueTemporaryAttachmentName(attachmentFormGuid, attachment.AttachmentName, attachment.AttachmentExtension, identifier);
            }

            return nameIsUnique;
        }

        return false;
    }
    

    private void SaveTemporaryAttachment()
    {
        attachment.Update();
    }


    private void SaveDocumentAttachment()
    {
        // Ensure automatic check-in/ check-out
        bool autoCheck = false;

        var wm = WorkflowManager.GetInstance(baseImageEditor.Tree);

        if (node != null)
        {
            // Get workflow info
            var wi = wm.GetNodeWorkflow(node);
            if (wi != null)
            {
                autoCheck = !wi.UseCheckInCheckOut(CurrentSiteName);
            }

            // Check out the document
            if (autoCheck)
            {
                var nextStep = node.VersionManager.CheckOut(node, node.IsPublished);
                VersionHistoryID = node.DocumentCheckedOutVersionHistoryID;

                if (IsWorkflowFinished(nextStep))
                {
                    attachment = (DocumentAttachment)AttachmentInfo.Provider.Get(attachmentGuid, SiteInfoProvider.GetSiteID(CurrentSiteName));
                }
            }
        }

        DocumentHelper.UpdateAttachment(node, attachment);

        // Check in the document
        if (autoCheck && (VersionHistoryID > 0))
        {
            node.VersionManager.CheckIn(node, null);
        }
    }


    private static bool IsWorkflowFinished(WorkflowStepInfo workflowStepInfo)
    {
        return workflowStepInfo == null;
    }


    private bool CheckAttachmentPermissions()
    {
        var permissionToCheck = (attachment.AttachmentFormGUID == Guid.Empty) ? NodePermissionsEnum.Modify : NodePermissionsEnum.Create;

        return (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, permissionToCheck) == AuthorizationResultEnum.Allowed);
    }


    private void UpdateAttachmentProperties(string mimetype, string title, string description, byte[] binary, int width, int height)
    {
        if (mimetype != "")
        {
            attachment.AttachmentMimeType = mimetype;
        }

        attachment.AttachmentTitle = title;
        attachment.AttachmentDescription = description;

        if (binary != null)
        {
            attachment.AttachmentBinary = binary;
            attachment.AttachmentSize = binary.Length;
        }

        UpdateAttachmentSize(width, height);
    }


    private void UpdateAttachmentSize(int width, int height)
    {
        if (width > 0)
        {
            attachment.AttachmentImageWidth = width;
        }
        if (height > 0)
        {
            attachment.AttachmentImageHeight = height;
        }
    }


    private void UpdateAttachmentNameAndExtension(string name, string extension)
    {
        // Test all parameters to empty values and update new value if available
        if (name != "")
        {
            if (!name.EndsWith(extension, StringComparison.Ordinal))
            {
                attachment.AttachmentName = name + extension;
            }
            else
            {
                attachment.AttachmentName = name;
            }
        }
        if (extension != "")
        {
            attachment.AttachmentExtension = extension;
        }
    }


    /// <summary>
    /// Gets current attachment history identifier in attachment is under workflow.
    /// </summary>
    private int GetAttachmentHistoryId()
    {
        var attachmentVersion = node.VersionManager.GetAttachmentVersion(VersionHistoryID, attachmentGuid);

        return attachmentVersion.AttachmentHistoryID;
    }

    #endregion


    #region "Undo redo functionality"

    /// <summary>
    /// Returns true if the files are stored only in DB or user has disk read/write permissions. Otherwise false.
    /// </summary>
    public bool IsUndoRedoPossible()
    {
        return baseImageEditor.IsUndoRedoPossible();
    }


    /// <summary>
    /// Returns true if there is a previous version of the file which is being modified.
    /// </summary>
    public bool IsUndoEnabled()
    {
        return baseImageEditor.IsUndoEnabled();
    }


    /// <summary>
    /// Returns true if there is a next version of the file which is being modified.
    /// </summary>
    public bool IsRedoEnabled()
    {
        return baseImageEditor.IsRedoEnabled();
    }


    /// <summary>
    /// Processes the undo action.
    /// </summary>
    public void ProcessUndo()
    {
        baseImageEditor.ProcessUndo();
    }


    /// <summary>
    /// Processes the redo action.
    /// </summary>
    public void ProcessRedo()
    {
        baseImageEditor.ProcessRedo();
    }


    /// <summary>
    /// Saves current version of image and discards all other versions.
    /// </summary>
    public void SaveCurrentVersion()
    {
        baseImageEditor.SaveCurrentVersion(true);
    }

    #endregion
}
