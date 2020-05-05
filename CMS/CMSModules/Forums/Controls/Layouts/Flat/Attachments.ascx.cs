using System;

using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Controls_Layouts_Flat_Attachments : ForumViewer
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int moderated = QueryHelper.GetInteger("moderated", 0);
        if (moderated != 0)
        {
            plcModerationRequired.Visible = true;
            lblModerationInfo.Text = GetString("forums.requiresmoderationafteraction");
        }
    }
}