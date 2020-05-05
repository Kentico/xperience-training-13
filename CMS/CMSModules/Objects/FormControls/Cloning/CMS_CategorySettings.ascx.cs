using System;
using System.Collections;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Objects_FormControls_Cloning_CMS_CategorySettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Gets script used for closing clone dialog.
    /// </summary>
    public override string CloseScript
    {
        get
        {
            return @"if ((wopener != null) && wopener.Refresh) { wopener.Refresh({0}," + drpCategories.CategoryID + "); } CloseDialog();";
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        drpCategories.ExcludeCategoryID = InfoToClone.Generalized.ObjectID;
        drpCategories.DisableSiteCategories = InfoToClone.Generalized.ObjectSiteID == 0;
        drpCategories.SiteID = InfoToClone.Generalized.ObjectSiteID;
        
        int userId = InfoToClone.GetIntegerValue("CategoryUserID", 0);
        if (userId > 0)
        {
            drpCategories.UserID = userId;
        }

        if (!RequestHelper.IsPostBack())
        {
            drpCategories.CategoryID = InfoToClone.GetIntegerValue("CategoryParentID", 0);    
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();

        var ti = InfoToClone.TypeInfo;

        result[ti.ObjectType + ".parentcategory"] = drpCategories.CategoryID;
        result[ti.ObjectType + ".subcategories"] = chkSubcategories.Checked;

        return result;
    }

    #endregion
}