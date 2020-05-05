using System;
using System.Collections;

using CMS.Newsletters;
using CMS.UIControls;


public partial class CMSModules_Newsletters_FormControls_Cloning_Newsletter_NewsletterSettings : CloneSettingsControl
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
    /// Excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return string.Join(";", OpenedEmailInfo.OBJECT_TYPE, LinkInfo.OBJECT_TYPE);
        }
    }


    /// <summary>
    /// Excluded other binding types.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return SubscriberNewsletterInfo.OBJECT_TYPE;
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
        result[NewsletterInfo.OBJECT_TYPE + ".subscribers"] = chkSubscribers.Checked;
        return result;
    }

    #endregion
}