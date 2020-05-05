using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.OnlineMarketing.Web.UI;
using CMS.UIControls;


[Title("mvtest.new")]
[Security(Resource = "CMS.MVTest", UIElements = "MVTestListing;New")]
[UIElement("CMS.MVTest", "New")]
public partial class CMSModules_OnlineMarketing_Pages_Content_MVTest_New : CMSMVTestPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ucDisabledModule.ParentPanel = pnlDisabled;

        InitBreadcrumbs();
        InitTitle();
    }


    /// <summary>
    /// Creates breadcrumbs.
    /// </summary>
    private void InitBreadcrumbs()
    {
        string url = UIContextHelper.GetElementUrl("CMS.MVTest", "MVTestListing");
        if (NodeID > 0)
        {
            url = URLHelper.AddParameterToUrl(url, "NodeID", NodeID.ToString());
        }

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = GetString("mvtest.list"),
            RedirectUrl = ResolveUrl(url)
        });

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = GetString("mvtest.new")
        });
    }


    /// <summary>
    /// Sets title if not in content.
    /// </summary>
    private void InitTitle()
    {
        if (NodeID <= 0)
        {
            SetTitle(GetString("analytics_codename.mvtests"));
        }
    }

    #endregion
}