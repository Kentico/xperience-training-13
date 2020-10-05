using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_Attachments_DirectFileUploader_PublicFileUploader : CMSPage
{
    #region "Private variables"

    private MediaSourceEnum mSourceType = MediaSourceEnum.DocumentAttachments;
    private Hashtable properties = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets current source type.
    /// </summary>
    private MediaSourceEnum SourceType
    {
        get
        {
            return mSourceType;
        }
        set
        {
            mSourceType = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Validate query string
        if (!QueryHelper.ValidateHash("hash"))
        {
            // Do nothing
        }
        else
        {
            String identifier = QueryHelper.GetString("identifier", null);
            if (String.IsNullOrEmpty(identifier))
            {
                return;
            }

            properties = WindowHelper.GetItem(identifier) as Hashtable;
            if (properties == null)
            {
                return;
            }

            // Get information on current source type
            string sourceType = ValidationHelper.GetString(GetProp("source"), "attachments");
            SourceType = CMSDialogHelper.GetMediaSource(sourceType);

            // Ensure additional styles
            CurrentMaster.HeadElements.Visible = true;
            CurrentMaster.HeadElements.Text += CssHelper.GetStyle("*{direction:ltr !important;}body{background:transparent !important;}input,input:focus,input:hover,input:active{border:none;border-color:transparent;outline:none;}");

            // Get uploader control based on the current source type
            string uploaderPath = "";
            if (SourceType == MediaSourceEnum.MediaLibraries)
            {
                // If media library module is running
                if (ModuleManager.IsModuleLoaded(ModuleName.MEDIALIBRARY))
                {
                    uploaderPath = "~/CMSModules/MediaLibrary/Controls/Dialogs/DirectFileUploader/DirectMediaFileUploaderControl.ascx";
                }
            }
            else
            {
                uploaderPath = "~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploaderControl.ascx";
            }

            // Load direct file uploader
            if (uploaderPath != "")
            {
                DirectFileUploader fileUploaderElem = this.LoadUserControl(uploaderPath) as DirectFileUploader;
                if (fileUploaderElem != null)
                {
                    // Insert uploader to parent container
                    pnlUploaderElem.Controls.Add(fileUploaderElem);

                    // Initialize uploader control properties by query string values
                    fileUploaderElem.AttachmentGUID = ValidationHelper.GetGuid(GetProp("attachmentguid"), Guid.Empty);
                    fileUploaderElem.AttachmentGroupGUID = ValidationHelper.GetGuid(GetProp("attachmentgroupguid"), Guid.Empty);
                    fileUploaderElem.AttachmentGUIDColumnName = ValidationHelper.GetString(GetProp("attachmentguidcolumnname"), null);
                    fileUploaderElem.FormGUID = ValidationHelper.GetGuid(GetProp("formguid"), Guid.Empty);
                    fileUploaderElem.DocumentID = ValidationHelper.GetInteger(GetProp("documentid"), 0);
                    fileUploaderElem.NodeParentNodeID = ValidationHelper.GetInteger(GetProp("parentid"), 0);
                    fileUploaderElem.NodeClassName = ValidationHelper.GetString(GetProp("classname"), "");
                    fileUploaderElem.InsertMode = ValidationHelper.GetBoolean(GetProp("insertmode"), false);
                    fileUploaderElem.OnlyImages = ValidationHelper.GetBoolean(GetProp("onlyimages"), false);
                    fileUploaderElem.ParentElemID = QueryHelper.GetString("parentelemid", String.Empty);
                    fileUploaderElem.CheckPermissions = ValidationHelper.GetBoolean(GetProp("checkperm"), true);
                    fileUploaderElem.IsLiveSite = false;
                    fileUploaderElem.RaiseOnClick = ValidationHelper.GetBoolean(GetProp("click"), false);
                    fileUploaderElem.NodeSiteName = ValidationHelper.GetString(GetProp("sitename"), null);
                    fileUploaderElem.SourceType = SourceType;

                    // Metafile upload
                    fileUploaderElem.SiteID = ValidationHelper.GetInteger(GetProp("siteid"), 0);
                    fileUploaderElem.Category = ValidationHelper.GetString(GetProp("category"), String.Empty);
                    fileUploaderElem.ObjectID = ValidationHelper.GetInteger(GetProp("objectid"), 0);
                    fileUploaderElem.ObjectType = ValidationHelper.GetString(GetProp("objecttype"), String.Empty);
                    fileUploaderElem.MetaFileID = ValidationHelper.GetInteger(GetProp("metafileid"), 0);

                    // Library info initialization;
                    fileUploaderElem.LibraryID = ValidationHelper.GetInteger(GetProp("libraryid"), 0);
                    fileUploaderElem.MediaFileID = ValidationHelper.GetInteger(GetProp("mediafileid"), 0);
                    fileUploaderElem.MediaFileName = ValidationHelper.GetString(GetProp("filename"), null);
                    fileUploaderElem.IsMediaThumbnail = ValidationHelper.GetBoolean(GetProp("ismediathumbnail"), false);
                    fileUploaderElem.LibraryFolderPath = ValidationHelper.GetString(GetProp("path"), "");
                    fileUploaderElem.IncludeNewItemInfo = ValidationHelper.GetBoolean(GetProp("includeinfo"), false);

                    string siteName = SiteContext.CurrentSiteName;
                    string allowed = ValidationHelper.GetString(GetProp("allowedextensions"), null);
                    if (allowed == null)
                    {
                        if (fileUploaderElem.SourceType == MediaSourceEnum.MediaLibraries)
                        {
                            allowed = SettingsKeyInfoProvider.GetValue(siteName + ".CMSMediaFileAllowedExtensions");
                        }
                        else
                        {
                            allowed = SettingsKeyInfoProvider.GetValue(siteName + ".CMSUploadExtensions");
                        }
                    }
                    fileUploaderElem.AllowedExtensions = allowed;

                    // Auto resize width
                    int autoResizeWidth = ValidationHelper.GetInteger(GetProp("autoresize_width"), -1);
                    if (autoResizeWidth == -1)
                    {
                        autoResizeWidth = SettingsKeyInfoProvider.GetIntValue(siteName + ".CMSAutoResizeImageWidth");
                    }
                    fileUploaderElem.ResizeToWidth = autoResizeWidth;

                    // Auto resize height
                    int autoResizeHeight = ValidationHelper.GetInteger(GetProp("autoresize_height"), -1);
                    if (autoResizeHeight == -1)
                    {
                        autoResizeHeight = SettingsKeyInfoProvider.GetIntValue(siteName + ".CMSAutoResizeImageHeight");
                    }
                    fileUploaderElem.ResizeToHeight = autoResizeHeight;

                    // Auto resize max side size
                    int autoResizeMaxSideSize = ValidationHelper.GetInteger(GetProp("autoresize_maxsidesize"), -1);
                    if (autoResizeMaxSideSize == -1)
                    {
                        autoResizeMaxSideSize = SettingsKeyInfoProvider.GetIntValue(siteName + ".CMSAutoResizeImageMaxSideSize");
                    }
                    fileUploaderElem.ResizeToMaxSideSize = autoResizeMaxSideSize;

                    fileUploaderElem.AfterSaveJavascript = ValidationHelper.GetString(GetProp("aftersave"), String.Empty);
                    fileUploaderElem.TargetFolderPath = ValidationHelper.GetString(GetProp("targetfolder"), String.Empty);
                    fileUploaderElem.TargetFileName = ValidationHelper.GetString(GetProp("targetfilename"), String.Empty);
                }
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns property from properties hashtable if exists.
    /// </summary>
    /// <param name="name">Name of property</param>
    private object GetProp(string name)
    {
        if (properties.ContainsKey(name))
        {
            return properties[name];
        }

        return null;
    }

    #endregion
}