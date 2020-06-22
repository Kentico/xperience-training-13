using System;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;

[UIElement(ModuleName.CMS, "Modules.Settings.EditKeys")]
public partial class CMSModules_Modules_Pages_Settings_Category_List : GlobalAdminPage
{
    #region "Variables"

    /// <summary>
    /// Displayed category
    /// </summary>
    private SettingsCategoryInfo mCategory;

    /// <summary>
    /// Module ID
    /// </summary>
    private readonly int moduleId = QueryHelper.GetInteger("moduleid", 0);

    /// <summary>
    /// Resource 
    /// </summary>
    private ResourceInfo mResource;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the current resource.
    /// </summary>
    private ResourceInfo Resource
    {
        get
        {
            return mResource ?? (mResource = ResourceInfo.Provider.Get(moduleId));
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        int categoryId = QueryHelper.GetInteger("categoryid", -1);

        // Find category
        if (categoryId >= 0)
        {
            mCategory = SettingsCategoryInfo.Provider.Get(categoryId);
        }

        // Use root category for Settings if category not found or specified
        if ((categoryId == -1) || (mCategory == null))
        {
            mCategory = SettingsCategoryInfo.Provider.Get("CMS.Settings");
        }

        // Set edited object
        EditedObject = mCategory;

        if (mCategory.CategoryParentID != 0)
        {
            grpEdit.CategoryName = mCategory.CategoryName;
            grpEdit.ModuleID = moduleId;
            grpEdit.ActionPerformed += grpEdit_ActionPerformed;
            grpEdit.OnNewKey += grpEdit_OnNewKey;
            grpEdit.OnKeyAction += grpEdit_OnKeyAction;

            // Read data
            grpEdit.ReloadData();
        }

        if (!Resource.ResourceIsInDevelopment && !SystemContext.DevelopmentMode)
        {
            // Show information about installed module
            ShowInformation(GetString("settingcategory.installedmodule"));
        }
        else if ((mCategory.CategoryID == categoryId) && (mCategory.CategoryParentID == 0))
        {
            // Show information about creating module categories and groups
            ShowInformation(GetString("settingcategory.createmodulecategory"));
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (mCategory == null)
        {
            return;
        }

        // Disable inserting groups under root category
        if ((mCategory.CategoryName != "CMS.Settings") && ((Resource != null) && (Resource.ResourceIsInDevelopment) || SystemContext.DevelopmentMode))
        {
            var newGroup = new HeaderAction
            {
                Text = GetString("Development.CustomSettings.NewGroup"),
                RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.Settings.EditCategory", false), "isgroup=1&parentid=" + mCategory.CategoryID + "&moduleid=" + moduleId)
            };

            CurrentMaster.HeaderActions.ActionsList.Add(newGroup);
        }
    }

    #endregion


    #region "Events handling"

    /// <summary>
    /// Handles the whole category actions.
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    protected void grpEdit_ActionPerformed(object sender, CommandEventArgs e)
    {
        int categoryId = ValidationHelper.GetInteger(e.CommandArgument, 0);
        switch (e.CommandName.ToLowerCSafe())
        {
            case ("edit"):
                // Redirect to category edit page
                SettingsCategoryInfo sci = SettingsCategoryInfo.Provider.Get(categoryId);
                if (sci != null)
                {
                    URLHelper.Redirect(URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.Settings.EditCategory", false), "isgroup=1&categoryid=" + categoryId + "&moduleid=" + moduleId));
                }
                break;

            case ("delete"):
                try
                {
                    SettingsCategoryInfo settingGroup = SettingsCategoryInfo.Provider.Get(categoryId);
                    if (settingGroup != null)
                    {
                        // Register refresh tree script
                        StringBuilder sb = new StringBuilder();
                        sb.Append("if (window.parent != null) {");
                        sb.Append("if (window.parent.parent.frames['settingstree'] != null) {");
                        sb.Append("window.parent.parent.frames['settingstree'].location = '" + ResolveUrl("~/CMSModules/Modules/Pages/Settings/Tree.aspx") + "?categoryid=" + settingGroup.CategoryParentID + "&moduleid=" + moduleId + "&reloadtreeselect=1';");
                        sb.Append("}");
                        sb.Append("if (window.parent.frames['settingstree'] != null) {");
                        sb.Append("window.parent.frames['settingstree'].location =  '" + ResolveUrl("~/CMSModules/Modules/Pages/Settings/Tree.aspx") + "?categoryid=" + settingGroup.CategoryParentID + "&moduleid=" + moduleId + "&reloadtreeselect=1';");
                        sb.Append("}");
                        sb.Append("}");

                        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "treeGroupRefresh", ScriptHelper.GetScript(sb.ToString()));

                        SettingsCategoryInfo.Provider.Delete(settingGroup);
                    }
                }
                catch
                {
                    ShowError(GetString("settings.group.deleteerror"));
                }
                grpEdit.ReloadData();
                break;

            case ("moveup"):
                SettingsCategoryInfoProvider.MoveCategoryUp(categoryId);
                grpEdit.ReloadData();
                break;

            case ("movedown"):
                SettingsCategoryInfoProvider.MoveCategoryDown(categoryId);
                grpEdit.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Handles creation of new settings key.
    /// </summary>
    private void grpEdit_OnNewKey(object sender, CommandEventArgs e)
    {
        URLHelper.Redirect(URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.Settings.EditSettingsKey", false), e.CommandArgument.ToString()));
    }


    /// <summary>
    /// Handles the settings key action event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void grpEdit_OnKeyAction(string actionName, object actionArgument)
    {
        int keyId = ValidationHelper.GetInteger(actionArgument, 0);
        SettingsKeyInfo ski = SettingsKeyInfoProvider.GetSettingsKeyInfo(keyId);

        switch (actionName.ToLowerCSafe())
        {
            case ("edit"):
                // Redirect to key edit page
                if (ski != null)
                {
                    URLHelper.Redirect(URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.Settings.EditSettingsKey", false), "keyname=" + ski.KeyName + "&moduleid=" + moduleId));
                }
                break;

            case ("delete"):
                try
                {
                    SettingsKeyInfoProvider.DeleteSettingsKeyInfo(ski);
                }
                catch
                {
                    ShowError(GetString("settingsedit.settingskey_edit.errordelete"));
                }
                break;

            case ("moveup"):
                SettingsKeyInfoProvider.MoveSettingsKeyUp(ski.KeyName);
                break;

            case ("movedown"):
                SettingsKeyInfoProvider.MoveSettingsKeyDown(ski.KeyName);
                break;
        }
    }

    #endregion
}