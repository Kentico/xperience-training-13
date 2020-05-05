using System;

using CMS.Base;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_UIPersonalization_Pages_Administration_UI_Editor : CMSUIPersonalizationPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SiteID = CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) ? 0 : SiteContext.CurrentSiteID;
        editElem.SiteID = SiteID;
        editElem.HideSiteSelector = (SiteID != 0);
        ResourceInfo ri = ResourceInfoProvider.GetResourceInfo("CMS.WYSIWYGEditor");
        if (ri != null)
        {
            editElem.ResourceID = ri.ResourceID;
            editElem.IsLiveSite = false;
        }
    }
}