using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_MetaFileList : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return elemFileList.Enabled;
        }
        set
        {
            elemFileList.Enabled = value;
        }
    }


    /// <summary>
    /// Id of the object whose metafiles will be shown.
    /// </summary>
    public int MetaFileObjectID
    {
        get
        {
            return GetIntContextValue("MetaFileObjectID", 0);
        }
    }


    /// <summary>
    /// Type of the object whose metafiles will be shown.
    /// </summary>
    public string MetaFileObjectType
    {
        get
        {
            return GetStringContextValue("MetaFileObjectType");
        }
    }


    /// <summary>
    /// Metafiles from this group will be shown.
    /// </summary>
    public string MetaFileGroupName
    {
        get
        {
            return GetStringContextValue("MetaFileGroupName");
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs eventArgs)
    {
        if (MetaFileObjectID != 0)
        {
            elemFileList.ObjectID = MetaFileObjectID;
        }
        if (!string.IsNullOrEmpty(MetaFileObjectType))
        {
            elemFileList.ObjectType = MetaFileObjectType;
        }
        if (!string.IsNullOrEmpty(MetaFileGroupName))
        {
            elemFileList.Category = MetaFileGroupName;
        }

        IEnumerable<string> allowedExtensionsFromSettings = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSUploadExtensions").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        IEnumerable<string> imageExtensions = new[] { "gif", "png", "bmp", "jpg", "jpeg", "webp", "tiff", };

        // Allow only image types which are also allowed system wide (by setting CMSUploadExtensions) to be uploaded. If there are no extensions specified in settings, allow all image types.
        IEnumerable<string> allowedExtensions = allowedExtensionsFromSettings.Any() ? allowedExtensionsFromSettings.Intersect(imageExtensions) : imageExtensions;

        elemFileList.AllowedExtensions = string.Join(";", allowedExtensions);
    }

    #endregion
}