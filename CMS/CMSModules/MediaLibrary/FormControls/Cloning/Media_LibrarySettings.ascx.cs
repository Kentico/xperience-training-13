using System;
using System.Collections;

using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_FormControls_Cloning_Media_LibrarySettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Gets excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            // Media files are explicitly handled in Media library info code so they don't need separate action
            return MediaFileInfo.OBJECT_TYPE;
        }
    }
    
    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblFiles.ToolTip = GetString("clonning.settings.medialibrary.files.tooltip");
        lblFolderName.ToolTip = GetString("clonning.settings.medialibrary.foldername.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            string originalPath = MediaLibraryInfoProvider.GetMediaLibraryFolderPath(InfoToClone.Generalized.ObjectID);
            txtFolderName.Text = DirectoryInfo.New(FileHelper.GetUniqueDirectoryName(originalPath)).Name;
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[MediaLibraryInfo.OBJECT_TYPE + ".foldername"] = txtFolderName.Text;
        result[MediaLibraryInfo.OBJECT_TYPE + ".files"] = chkFiles.Checked;
        return result;
    }

    #endregion
}