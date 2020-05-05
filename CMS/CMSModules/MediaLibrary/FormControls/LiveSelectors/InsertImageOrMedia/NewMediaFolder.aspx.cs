using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_FormControls_LiveSelectors_InsertImageOrMedia_NewMediaFolder : CMSLiveModalPage
{
    #region "Private variables"

    private int mLibraryId;
    private string mFolderPath = "";
    private MediaLibraryInfo mLibrary;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current library ID.
    /// </summary>
    private MediaLibraryInfo Library
    {
        get
        {
            if ((mLibrary == null) && (mLibraryId > 0))
            {
                mLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo(mLibraryId);
            }
            return mLibrary;
        }
        set
        {
            mLibrary = value;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            // Check site availability
            if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.MediaLibrary", SiteContext.CurrentSiteName))
            {
                RedirectToResourceNotAvailableOnSite("CMS.MediaLibrary");
            }

            // Initialize controls
            SetupControls();
        }
        else
        {
            createFolder.Visible = false;
            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "redirect", ScriptHelper.GetScript(String.Format("if (window.parent != null) {{ window.parent.location = '{0}' }}", url)));
        }
    }


    /// <summary>
    /// Initializes controls.
    /// </summary>
    private void SetupControls()
    {
        String identifier = QueryHelper.GetString("identifier", null);
        if (!String.IsNullOrEmpty(identifier))
        {
            Hashtable properties = WindowHelper.GetItem(identifier) as Hashtable;
            if (properties != null)
            {
                // Get data
                mLibraryId = ValidationHelper.GetInteger(properties["libraryid"], 0);
                mFolderPath = ValidationHelper.GetString(properties["path"], String.Empty);
                EditedObject = Library;

                if (Library != null)
                {
                    createFolder.OnFolderChange += createFolder_OnFolderChange;
                    createFolder.IsLiveSite = true;

                    // Initialize information on library
                    createFolder.LibraryID = mLibraryId;
                    createFolder.LibraryFolder = Library.LibraryFolder;
                    createFolder.FolderPath = mFolderPath;
                }

                Page.Header.Title = GetString("dialogs.newfoldertitle");

                PageTitle.TitleText = GetString("media.folder.new");
            }
        }
    }


    #region "Event handlers"

    protected void createFolder_OnFolderChange(string pathToSelect)
    {
        ltlScript.Text = ScriptHelper.GetScript(String.Format("wopener.SetAction('newfolder', '{0}'); wopener.RaiseHiddenPostBack(); CloseDialog();", pathToSelect.Replace('\\', '|').Replace("'", "\\'")));
    }

    #endregion
}
