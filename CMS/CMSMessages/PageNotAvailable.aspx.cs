using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

public partial class CMSMessages_PageNotAvailable : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string reason = QueryHelper.GetString("reason", string.Empty);
        bool showLink = QueryHelper.GetBoolean("showlink", true);
        string docname = QueryHelper.GetText("docname", string.Empty);
        string title = null;

        switch (reason.ToLowerCSafe())
        {
            case "missingculture":
                title = GetString("MissingCulture.Header");
                lblInfo.Text = GetString("MissingCulture.Info");
                break;

            case "splitviewmissingculture":
                title = GetString("MissingCulture.Header");
                lblInfo.Text = GetString("SplitviewMissingCulture.Info");
                break;

            case "notpublished":
                title = GetString("NotPublished.Header");
                lblInfo.Text = GetString("NotPublished.Info");
                break;

            default:
                title = GetString("NotAvailable.Header");
                lblInfo.Text = GetString("NotAvailable.Info");
                break;
        }

        titleElem.TitleText = String.Format(title, docname);
        if (showLink)
        {
            lnkBack.Text = GetString("404.Back");
            lnkBack.NavigateUrl = "~/";
        }
    }
}