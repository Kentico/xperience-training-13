using System;

using CMS.Blogs.Web.UI;
using CMS.DataEngine;
using CMS.UIControls;


[UIElement("CMS.Blog", "Comments")]
public partial class CMSModules_Blogs_Tools_Blogs_Comments_List : CMSBlogsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // No cms.blog doc. type
        if (DataClassInfoProvider.GetDataClassInfo("cms.blog") == null)
        {
            RedirectToInformation(GetString("blog.noblogdoctype"));
        }
    }
}