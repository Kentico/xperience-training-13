using System.Collections;

using CMS.ContactManagement;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_FormControls_Cloning_OM_ContactGroupSettings : CloneSettingsControl
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
    /// Exclued other binding types.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return ContactGroupMemberInfo.OBJECT_TYPE_ACCOUNT + ";" + ContactGroupMemberInfo.OBJECT_TYPE_CONTACT;
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
        result[ContactGroupInfo.OBJECT_TYPE + ".accounts"] = chkCloneAccounts.Checked;
        result[ContactGroupInfo.OBJECT_TYPE + ".contacts"] = chkCloneContacts.Checked;
        return result;
    }

    #endregion
}