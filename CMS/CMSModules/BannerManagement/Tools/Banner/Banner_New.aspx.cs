using CMS.BannerManagement;
using CMS.UIControls;


[Breadcrumbs()]
[Breadcrumb(0, "general.banners", TargetUrl = "~/CMSModules/BannerManagement/Tools/Category/Category_Edit_Banners.aspx?siteid={?siteid?}&parentobjectid={?parentobjectid?}")]
[Breadcrumb(1, "banner.bannercategory_edit_banner.newbanner")]

[Help("banner_new")]

// Parent object
[ParentObject(BannerCategoryInfo.OBJECT_TYPE, "parentobjectid")]
public partial class CMSModules_BannerManagement_Tools_Banner_Banner_New : CMSBannerManagementEditPage
{
    #region "Properties"

    protected override int? EditedSiteID
    {
        get
        {
            return ((BannerCategoryInfo)UIContext.EditedObjectParent).BannerCategorySiteID;
        }
    }

    #endregion
}
