using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Modules.Settings.EditCategory")]
public partial class CMSModules_Modules_Pages_Settings_Category_Edit : GlobalAdminPage
{
    #region "Variables"

    private int mParentId = -1;
    private int mCategoryId = -1;
    private int moduleId = QueryHelper.GetInteger("moduleid", 0);

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Id of parent if creating new category.
        mParentId = QueryHelper.GetInteger("parentid", -1);
        // Id of category if editing existing category.
        mCategoryId = QueryHelper.GetInteger("categoryid", -1);
        // Set isGroup flag.
        catEdit.IsGroupEdit = QueryHelper.GetBoolean("isgroup", false);

        catEdit.IncludeRootCategory = !catEdit.IsGroupEdit;

        if (ViewState["newId"] != null)
        {
            mCategoryId = ValidationHelper.GetInteger(ViewState["newId"], 0);
        }


        catEdit.OnSaved += catEdit_OnSaved;

        // Set up of root category in parent selector and refreshing
        catEdit.DisplayOnlyCategories = true;
        catEdit.SettingsCategoryID = mCategoryId;
        // Set tree refreshing
        catEdit.TreeRefreshUrl = "~/CMSModules/Modules/Pages/Settings/Tree.aspx?moduleid=" + moduleId;

        // Get root category: Settings or CustomSettings
        SettingsCategoryInfo settingsRoot = SettingsCategoryInfo.Provider.Get("CMS.Settings");

        if (mCategoryId <= 0)
        {
            catEdit.ShowParentSelector = false;

            if (catEdit.SettingsCategoryObj == null)
            {
                catEdit.RootCategoryID = mParentId;
            }

            // Redirect to editing form
            if (catEdit.IsGroupEdit)
            {
                catEdit.ContentRefreshUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.Settings.EditCategory", false), "moduleId=" + moduleId);
            }
            else
            {
                catEdit.ContentRefreshUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "EditSettingsCategory", false), "tabName=Modules.Settings.EditCategory&moduleId=" + moduleId);
            }         
        }
        else
        {
            SetEditEnabled(false);

            // Do not show root category
            if (catEdit.SettingsCategoryObj.CategoryID != settingsRoot.CategoryID)
            {
                SetEditEnabled(true);

                // Allow assigning of all categories in edit mode
                catEdit.RootCategoryID = settingsRoot.CategoryID;
                catEdit.IsGroupEdit = catEdit.SettingsCategoryObj.CategoryIsGroup;
            }
            else
            {
                ShowInformation(GetString("settingcategory.rootcategorywarning"));
            }
        }


    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Parent category info for level up link
        SettingsCategoryInfo parentCategoryInfo = null;
        var categoryBreadcrumb = new BreadcrumbItem();

        if (mCategoryId <= 0)
        {
            catEdit.ShowParentSelector = false;

            if (catEdit.SettingsCategoryObj == null)
            {
                categoryBreadcrumb.Text = GetString(catEdit.IsGroupEdit ? "settings.group.new" : "settingsedit.category_list.newitemcaption");
            }
            else
            {
                categoryBreadcrumb.Text = catEdit.SettingsCategoryObj.CategoryDisplayName;
            }
        }
        else
        {
            categoryBreadcrumb.Text = GetString(catEdit.IsGroupEdit ? catEdit.SettingsCategoryObj.CategoryDisplayName : "settingsedit.settingscategory.edit.headercaption");
        }

        var parentCategoryBreadcrumb = new BreadcrumbItem();

        parentCategoryInfo = SettingsCategoryInfo.Provider.Get(catEdit.SelectedParentCategory);

        // Set up title and breadcrumbs
        if (parentCategoryInfo != null)
        {
            parentCategoryBreadcrumb.Text = ResHelper.LocalizeString(parentCategoryInfo.CategoryDisplayName);
            parentCategoryBreadcrumb.RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.Settings.EditKeys", false), "categoryid=" + parentCategoryInfo.CategoryID + "&moduleid=" + moduleId);
        }

        if (mCategoryId <= 0 || catEdit.IsGroupEdit)
        {
            PageBreadcrumbs.AddBreadcrumb(parentCategoryBreadcrumb);
            PageBreadcrumbs.AddBreadcrumb(categoryBreadcrumb);
        }
    }

    #endregion


    #region "Protected methods"

    protected void catEdit_OnSaved(object sender, EventArgs e)
    {
        // Save id of newly created item for edit mode
        if (catEdit.SettingsCategoryObj != null)
        {
            ViewState.Add("newId", catEdit.SettingsCategoryObj.CategoryID);
        }
    }


    /// <summary>
    /// Sets visibility of export links and group properties.
    /// </summary>
    protected void SetEditEnabled(bool enabled)
    {
        // Set visibility of export links
        catEdit.Visible = enabled;
    }

    #endregion
}