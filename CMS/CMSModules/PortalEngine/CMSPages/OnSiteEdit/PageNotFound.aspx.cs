using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_CMSPages_OnSiteEdit_PageNotFound : CMSPage
{
    public CMSModules_PortalEngine_CMSPages_OnSiteEdit_PageNotFound()
    {
        if (PortalContext.ViewMode != ViewModeEnum.EditLive)
        {
            SetLiveCulture();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (PortalContext.ViewMode != ViewModeEnum.EditLive)
        {
            // Try skip IIS http errors
            Response.TrySkipIisCustomErrors = true;

            // Set page not found state
            Response.StatusCode = 404;
        }
        else if (SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSAllowOnSiteEditing"))
        {
            CssRegistration.RegisterDesignMode(Page);

            CMSAbstractPortalUserControl editToolbar = (CMSAbstractPortalUserControl)Page.LoadUserControl("~/CMSModules/PortalEngine/Controls/OnsiteEdit/EditToolbar.ascx");
            editToolbar.ID = "editToolbar";
            editToolbar.ShortID = "et";
            plcMain.Controls.Add(editToolbar);
        }

        titleElem.TitleText = GetString("404.Header");
        lblInfo.Text = String.Format(GetString("404.Info"), HTMLHelper.HTMLEncode(RequestContext.CurrentURL) + " (" + LocalizationContext.PreferredCultureCode + ")");
        lblRootDoc.Text = "<a href=\"" + URLHelper.GetApplicationUrl() + "\" target=\"_self\">" + HTMLHelper.HTMLEncode(GetString("onsiteedit.rootredirect")) + "<a>";
    }
}
