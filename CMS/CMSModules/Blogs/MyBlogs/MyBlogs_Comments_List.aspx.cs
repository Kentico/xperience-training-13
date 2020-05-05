using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.UIControls;


[UIElement(ModuleName.BLOGS, "MyBlogsComments")]
public partial class CMSModules_Blogs_MyBlogs_MyBlogs_Comments_List : CMSContentManagementPage
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