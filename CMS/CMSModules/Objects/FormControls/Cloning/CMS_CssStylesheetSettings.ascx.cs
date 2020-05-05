using System;
using System.Collections;

using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Objects_FormControls_Cloning_CMS_CssStylesheetSettings : CloneSettingsControl
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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblFiles.ToolTip = GetString("clonning.settings.layouts.appthemesfolder.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[CssStylesheetInfo.OBJECT_TYPE + ".appthemes"] = chkFiles.Checked;
        return result;
    }

    #endregion
}