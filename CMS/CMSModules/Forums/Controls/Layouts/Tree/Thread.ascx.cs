using System;

using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Controls_Layouts_Tree_Thread : ForumViewer
{
    protected void Page_Load(object sender, EventArgs e)
    {
        URLHelper.Redirect(URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, "threadid"));
    }
}