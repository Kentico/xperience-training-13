using System;
using System.Web.UI;

using CMS.Base;
using CMS.Helpers;

#pragma warning disable BH3502 // Should inherit from some abstract CMSPage.
public partial class Admin_Default : Page
#pragma warning restore BH3502 // Should inherit from some abstract CMSPage.
{
    protected override void OnPreInit(EventArgs e)
    {
        string customAdminPath = AdministrationUrlHelper.GetCustomAdministrationPath();
        if (String.IsNullOrEmpty(customAdminPath) || customAdminPath.EqualsCSafe(AdministrationUrlHelper.DEFAULT_ADMINISTRATION_PATH, true))
        {
            URLHelper.Redirect("~/Admin/CMSAdministration.aspx" + RequestContext.URL.Query);
        }
        else
        {
            RequestHelper.Respond404();
        }
        base.OnPreInit(e);
    }
}