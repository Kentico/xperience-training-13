using System.Collections;

using CMS.BannerManagement;
using CMS.UIControls;


public partial class CMSModules_Objects_FormControls_Cloning_CMS_BannerCategorySettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Defines a custom close refresh script which preserves selected item in site drop down list.
    /// </summary>
    public override string CloseScript
    {
        get
        {
            return "wopener.RefreshUsingPostBack(); CloseDialog();";
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[BannerCategoryInfo.OBJECT_TYPE + ".banner"] = chkCloneBanners.Checked;
        return result;
    }

    #endregion
}