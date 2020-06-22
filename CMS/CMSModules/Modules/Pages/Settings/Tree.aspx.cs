using System;
using System.Linq;
using System.Text;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Modules_Pages_Settings_Tree : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            treeSettings.SelectPath = "/";

            int categoryId = QueryHelper.GetInteger("categoryid", -1);
            SettingsCategoryInfo category = SettingsCategoryInfo.Provider.Get(categoryId);
            // Select requested category
            if (category != null)
            {
                treeSettings.SelectPath = category.CategoryIDPath;
                treeSettings.CategoryID = category.CategoryID;
                treeSettings.ParentID = category.CategoryParentID;
                treeSettings.CategoryModuleID = category.CategoryResourceID;
                treeSettings.Value = category.CategoryID + "|" + category.CategoryParentID;
            }
            else
            {
                //  Select root
                SettingsCategoryInfo rootCat = treeSettings.RootCategory;
                if (rootCat != null)
                {
                    treeSettings.CategoryID = rootCat.CategoryID;
                    treeSettings.ParentID = rootCat.CategoryParentID;
                    treeSettings.Value = rootCat.CategoryID + "|" + rootCat.CategoryParentID;
                }
            }
        }
    }
}