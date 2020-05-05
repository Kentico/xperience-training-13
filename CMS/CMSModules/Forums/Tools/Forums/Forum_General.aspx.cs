using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.FORUMS, "ForumGeneral")]
public partial class CMSModules_Forums_Tools_Forums_Forum_General : CMSForumsPage
{
    #region "Variables"

    private int mForumId = 0;
    private bool changeMaster = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the ID of the forum to edit.
    /// </summary>
    public int ForumID
    {
        get
        {
            return mForumId;
        }
        set
        {
            mForumId = value;
        }
    }

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // External call
        changeMaster = QueryHelper.GetBoolean("changemaster", false);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int forumID = QueryHelper.GetInteger("forumid", 0);

        ForumContext.CheckSite(0, forumID, 0);

        forumEdit.ForumID = forumID;
        mForumId = forumID;
        forumEdit.OnSaved += new EventHandler(forumEdit_OnSaved);
        forumEdit.IsLiveSite = false;
    }


    protected void forumEdit_OnSaved(object sender, EventArgs e)
    {
        // Refresh tree if external parent
        if (changeMaster)
        {
            ForumInfo fi = ForumInfoProvider.GetForumInfo(mForumId);
            if (fi != null)
            {
                ltlScript.Text += ScriptHelper.GetScript("window.parent.parent.frames['tree'].RefreshNode(" + ScriptHelper.GetString(fi.ForumDisplayName) + ", '" + mForumId + "');");
            }
        }
    }
}
