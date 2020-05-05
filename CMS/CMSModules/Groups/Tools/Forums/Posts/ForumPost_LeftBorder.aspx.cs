using System;

using CMS.Community.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Groups_Tools_Forums_Posts_ForumPost_LeftBorder : CMSGroupForumPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.GetBoolean("changemaster", false))
        {
            pnlInner.Visible = false;
        }
    }
}