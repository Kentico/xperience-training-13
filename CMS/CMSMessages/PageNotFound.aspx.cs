using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSMessages_PageNotFound : MessagePage
{
    public CMSMessages_PageNotFound()
    {
        SetLiveCulture();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;
        // Set page not found state
        Response.StatusCode = 404;
        
        titleElem.TitleText = GetString("404.Header");
        lblInfo.Text = String.Format(GetString("404.Info"), QueryHelper.GetText("aspxerrorpath", String.Empty));

        lnkBack.Text = GetString("404.Back");
        lnkBack.NavigateUrl = URLHelper.ResolveUrl("~/");
    }
}