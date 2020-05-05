using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Base;

public partial class CMSModules_Settings_Pages_Categories : CMSDeskPage
{
    /// <summary>
    /// OnPreLoad event. 
    /// </summary>
    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);
        RequireSite = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);

        TreeViewCategories.ShowHeaderPanel = CMSActionContext.CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
        TreeViewCategories.RootIsClickable = true;

        // Get selected category ID
        int categoryId;
        if (!RequestHelper.IsPostBack() && QueryHelper.Contains("selectedCategoryId"))
        {
            // Get from URL
            categoryId = QueryHelper.GetInteger("selectedCategoryId", 0);
        }
        else if (Request.Form["selectedCategoryId"] != null)
        {
            // Get from postback
            categoryId = ValidationHelper.GetInteger(Request.Form["selectedCategoryId"], 0);
        }
        else
        {
            // Select root by default
            categoryId = SettingsCategoryInfoProvider.GetRootSettingsCategoryInfo().CategoryID;
        }
        TreeViewCategories.CategoryID = categoryId;
    }


    /// <summary>
    /// Reloads tree content.
    /// </summary>
    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);

        // Reload tree after selected site has changed.
        if (RequestHelper.IsPostBack())
        {
            TreeViewCategories.ReloadData();
        }
    }
}
