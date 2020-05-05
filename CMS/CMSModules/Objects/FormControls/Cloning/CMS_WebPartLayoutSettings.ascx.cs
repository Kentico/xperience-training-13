using System;
using System.Collections;

using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Objects_FormControls_Cloning_CMS_WebPartLayoutSettings : CloneSettingsControl
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

    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[WebPartLayoutInfo.OBJECT_TYPE + ".appthemes"] = chkAppThemes.Checked;
        return result;
    }

    #endregion
}