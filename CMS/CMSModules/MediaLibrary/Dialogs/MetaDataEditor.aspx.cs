using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Dialogs_MetaDataEditor : CMSModalPage
{
    private string mCurrentSiteName;

    
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

                SiteInfo site = SiteInfo.Provider.Get(siteId);
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
            // Reset current site name to load it from query string
            mCurrentSiteName = null;
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

            Save += btnSave_Click;

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

            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ltlScript.Text = ScriptHelper.GetScript("window.location = '" + url + "';");
        }
    }


    /// <summary>
    /// Saves metadata of media file.
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
