using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSPages_unsubscribe : CMSPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        Guid subGuid = QueryHelper.GetGuid("forumsubguid", Guid.Empty);
        string forumSubscriptionHash = QueryHelper.GetString("forumsubscriptionhash", string.Empty);

        string redirectionUrl = null;
        var queryString = RequestContext.CurrentQueryString;

        if ((subGuid != Guid.Empty) || !string.IsNullOrEmpty(forumSubscriptionHash))
        {
            redirectionUrl = "~/CMSModules/Forums/CMSPages/Unsubscribe.aspx";
        }
        else
        {
            redirectionUrl = "~/CMSModules/Newsletters/CMSPages/Unsubscribe.aspx";
        }

        redirectionUrl = URLHelper.AppendQuery(redirectionUrl, queryString);
        URLHelper.Redirect(redirectionUrl);
    }
}