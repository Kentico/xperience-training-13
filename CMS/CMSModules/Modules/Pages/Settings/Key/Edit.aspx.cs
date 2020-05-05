using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Modules.Settings.EditSettingsKey")]
public partial class CMSModules_Modules_Pages_Settings_Key_Edit : GlobalAdminPage
{
    #region "Variables"

    private string mKeyName = "";
    private string mParentGroup = "";
    private SettingsKeyInfo mEditedKey;

    private readonly int mModuleId = QueryHelper.GetInteger("moduleid", 0);

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        //Get parameters
        mKeyName = QueryHelper.GetString("keyname", "");
        mParentGroup = QueryHelper.GetString("parentgroup", null);

        skeEditKey.OnSaved += skeEditKey_OnSaved;

        // Set up editing mode
        if (!string.IsNullOrEmpty(mKeyName))
        {
            mEditedKey = SettingsKeyInfoProvider.GetSettingsKeyInfo(mKeyName, 0);

            // Set id of key
            if (mEditedKey != null)
            {
                skeEditKey.SettingsKeyID = mEditedKey.KeyID;
            }
        }
        // Set up creating mode
        else
        {
            if (mParentGroup != null)
            {
                SettingsCategoryInfo parentCat = SettingsCategoryInfoProvider.GetSettingsCategoryInfoByName(mParentGroup);
                if (parentCat != null)
                {
                    skeEditKey.SelectedGroupID = parentCat.CategoryID;
                }
            }
        }

        // Check if there is something right created to edit.
        if (ViewState["newId"] != null)
        {
            skeEditKey.SettingsKeyID = ValidationHelper.GetInteger(ViewState["newId"], 0);
        }

        skeEditKey.ModuleID = mModuleId;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        var categoryBreadcrumb = new BreadcrumbItem();

        var settingBreadcrumb = new BreadcrumbItem();

        // Set bradcrumbs for editing
        if (skeEditKey.SettingsKeyObj != null)
        {
            var sci = SettingsCategoryInfoProvider.GetSettingsCategoryInfo(skeEditKey.SettingsKeyObj.KeyCategoryID);

            categoryBreadcrumb.Text = sci.CategoryDisplayName;
            categoryBreadcrumb.RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.Settings.EditKeys", false), "categoryid=" + sci.CategoryParentID + "&moduleid=" + mModuleId);

            settingBreadcrumb.Text = skeEditKey.SettingsKeyObj.KeyDisplayName;
        }
        // Set bradcrumbs for creating new key
        else
        {
            if (mParentGroup != null)
            {
                var parentCat = SettingsCategoryInfoProvider.GetSettingsCategoryInfoByName(mParentGroup);
                if (parentCat != null)
                {
                    categoryBreadcrumb.Text = parentCat.CategoryDisplayName;
                    categoryBreadcrumb.RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.Settings.EditKeys", false), "categoryid=" + parentCat.CategoryParentID + "&moduleid=" + mModuleId);

                    settingBreadcrumb.Text = GetString("Development.CustomSettings.NewKey");
                }
            }
        }

        PageBreadcrumbs.AddBreadcrumb(categoryBreadcrumb);
        PageBreadcrumbs.AddBreadcrumb(settingBreadcrumb);

    }

    #endregion


    #region "Event handler"

    private void skeEditKey_OnSaved(object sender, EventArgs e)
    {
        // Store new keyId for use in edit mode.
        ViewState.Add("newId", skeEditKey.SettingsKeyObj.KeyID);
    }

    #endregion
}