using System;

using CMS.UIControls;


[Title("banner.bannercategory_list.newcategory")]

[Breadcrumbs()]
[Breadcrumb(0, ResourceString = "banner.bannercategory_new.title", TargetUrl = "~/CMSModules/BannerManagement/Tools/Category/Category_List.aspx?siteid={?siteid?}")]
[Breadcrumb(1, "banner.bannercategory_new.new")]
[UIElement("CMS.BannerManagement", "BannerManagement.AddBannerCategory")]
public partial class CMSModules_BannerManagement_Tools_Category_Category_New : CMSBannerManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
}
