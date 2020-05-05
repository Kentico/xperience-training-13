using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSMessages_BannedIP : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;
        // Set forbidden state
        Response.StatusCode = 403;
        // IP address rejected
        Response.SubStatusCode = 6;

        titleElem.TitleText = GetString("banip.cmsmessagesbantitle");
        lblMessage.Text = GetString("banip.banmessage");
    }
}