using System;
using System.Collections;

using CMS.Forums;
using CMS.UIControls;


public partial class CMSModules_Forums_FormControls_Cloning_Forums_ForumSettings : CloneSettingsControl
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
            return ForumPostInfo.OBJECT_TYPE;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblCloneForumPosts.ToolTip = GetString("clonning.settings.forum.cloneposts.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[ForumInfo.OBJECT_TYPE + ".posts"] = chkCloneForumPosts.Checked;
        return result;
    }

    #endregion
}