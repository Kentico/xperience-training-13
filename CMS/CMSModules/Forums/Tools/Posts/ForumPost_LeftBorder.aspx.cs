using System;

using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Tools_Posts_ForumPost_LeftBorder : CMSForumsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.GetBoolean("changemaster", false))
        {
            pnlInner.Visible = false;
        }
    }
}