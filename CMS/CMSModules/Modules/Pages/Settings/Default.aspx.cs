using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Modules.Settings")]
public partial class CMSModules_Modules_Pages_Settings_Default : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        int moduleId = QueryHelper.GetInteger("moduleid", 0);
        int catId = QueryHelper.GetInteger("categoryid", -1);

        if (catId <= 0)
        {
            string whereCond = moduleId > 0 ? "CategoryResourceID = " + moduleId : "CategoryParentID = (SELECT CategoryId FROM CMS_SettingsCategory WHERE CategoryName = 'CMS.Settings')";
            DataSet ds = SettingsCategoryInfoProvider.GetSettingsCategories(whereCond, "CategoryLevel, CategoryOrder", 1, "CategoryIsGroup, CategoryID, CategoryParentID");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                DataRow row = ds.Tables[0].Rows[0];
                catId = ValidationHelper.GetBoolean(row["CategoryIsGroup"], false) ? ValidationHelper.GetInteger(row["CategoryParentID"], 0) : ValidationHelper.GetInteger(row["CategoryID"], 0);
            }
            else
            {
                catId = SettingsCategoryInfoProvider.GetRootSettingsCategoryInfo().CategoryID;
            }
        }

        frameTree.Attributes["src"] = "Tree.aspx?categoryid=" + catId + "&moduleid=" + moduleId;

        var queryString = "categoryid=" + catId + "&moduleid=" + moduleId + "&objectID=" + catId + "&objectParentID=" + moduleId;
        frameMain.Attributes["src"] =  URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "EditSettingsCategory", false), queryString);

        ScriptHelper.HideVerticalTabs(this);
    }
}