using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Blogs_CMSPages_SubscriptionApproval : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var blogPost = subscriptionApproval.SubscriptionSubject;
        var blogPostTitle = (blogPost != null) ? ScriptHelper.GetString(blogPost.GetValue("BlogPostTitle", string.Empty)) : null;

        PageTitle.TitleText = HTMLHelper.HTMLEncode(string.Format(GetString("blog.subscriptionconfirmation"), blogPostTitle));
    }
}
