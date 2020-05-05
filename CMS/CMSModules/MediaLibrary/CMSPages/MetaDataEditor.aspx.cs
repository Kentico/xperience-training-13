using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_CMSPages_MetaDataEditor : CMSLiveModalPage
{
    private string mCurrentSiteName = null;

 
    /// <summary>
    /// Returns the site name from query string 'sitename' or 'siteid' if present, otherwise SiteContext.CurrentSiteName.
    /// </summary>
    protected new string CurrentSiteName
    {
        get
        {
            if (mCurrentSiteName == null)
            {
                mCurrentSiteName = QueryHelper.GetString("sitename", SiteContext.CurrentSiteName);

                int siteId = QueryHelper.GetInteger("siteid", 0);

                SiteInfo site = SiteInfoProvider.GetSiteInfo(siteId);
                if (site != null)
                {
                    mCurrentSiteName = site.SiteName;
                }
            }
            return mCurrentSiteName;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize modal page
        RegisterEscScript();

        if (QueryHelper.ValidateHash("hash"))
        {
            Guid guid = QueryHelper.GetGuid("mediafileguid", Guid.Empty);
            string title = GetString("general.editmetadata");
            Page.Title = title;
            PageTitle.TitleText = title;
            
            // Register update script on before unload
            ScriptHelper.RegisterJQuery(Page);

            var statusUpdate = ScriptHelper.GetScript(@"
$cmsj(window).on('beforeunload', function () {
    if (wopener.EditDialogStateUpdate) { 
        wopener.EditDialogStateUpdate('false'); 
    }
})
$cmsj(window).unload(function () {
    if (wopener.imageEdit_Refresh) { 
        wopener.imageEdit_Refresh('" + ScriptHelper.GetString(guid.ToString(), false) + @"|" + ScriptHelper.GetString(CurrentSiteName, false) + @"'); 
    }
})");
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "DialogStateUpdate", statusUpdate);

            btnSave.Click += btnSave_Click;

            AddNoCacheTag();

            // Set metadata editor properties
            metaDataEditor.ObjectGuid = guid;
            metaDataEditor.ObjectType = MediaFileInfo.OBJECT_TYPE;
            metaDataEditor.SiteName = CurrentSiteName;
        }
        else
        {
            // Hide all controls
            metaDataEditor.Visible = false;
            btnSave.Visible = false;

            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ltlScript.Text = ScriptHelper.GetScript("window.location = '" + url + "';");
        }
    }


    /// <summary>
    /// Save meta data of media file.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Argument</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (metaDataEditor.SaveMetadata())
        {
            ltlScript.Text = ScriptHelper.GetScript("CloseDialog();");
        }
    }
}
