using System;
using System.Collections;

using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_CMSPages_SelectFolder_Content : CMSLiveModalPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        SetBrowserClass();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            String identifier = QueryHelper.GetString("identifier", null);
            if (!String.IsNullOrEmpty(identifier))
            {
                Hashtable properties = WindowHelper.GetItem(identifier) as Hashtable;
                if (properties != null)
                {
                    // Get query values
                    selectFolder.MediaLibraryID = ValidationHelper.GetInteger(properties["libraryid"], 0);
                    selectFolder.Action = QueryHelper.GetString("action", "");
                    selectFolder.FolderPath = Path.EnsureBackslashes(ValidationHelper.GetString(properties["path"], ""));
                    selectFolder.Files = ValidationHelper.GetString(properties["files"], "").Trim('|');
                    selectFolder.AllFiles = ValidationHelper.GetBoolean(properties["allFiles"], false);
                }
            }
        }
    }
}