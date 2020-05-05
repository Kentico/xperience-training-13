using System;
using System.Collections;

using CMS.ContactManagement;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_FormControls_Cloning_OM_AccountSettings : CloneSettingsControl
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
    /// Excluded binding types.
    /// </summary>
    public override string ExcludedBindingTypes
    {
        get
        {
            return ContactGroupMemberInfo.OBJECT_TYPE_ACCOUNT;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblSubsidiaries.ToolTip = GetString("clonning.settings.account.subsidiaries.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[AccountInfo.OBJECT_TYPE + ".subsidiaries"] = chkSubsidiaries.Checked;
        result[AccountInfo.OBJECT_TYPE + ".contactgroup"] = chkContactGroup.Checked;
        return result;
    }

    #endregion
}