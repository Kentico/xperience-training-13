using System;
using System.Web.UI.HtmlControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_PageElements_TabsFrameset : CMSUserControl
{
    #region "Variables"

    private int mFrameHeight = 96;

    #endregion


    #region "Properties"

    /// <summary>
    /// URL of the header frame.
    /// </summary>
    public string HeaderUrl
    {
        get;
        set;
    }


    /// <summary>
    /// URL of the content frame.
    /// </summary>
    public string ContentUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Frameset control.
    /// </summary>
    public HtmlGenericControl RowsFrameset
    {
        get
        {
            return rowsFrameset;
        }
    }


    /// <summary>
    /// Frame height.
    /// </summary>
    public int FrameHeight
    {
        get
        {
            return mFrameHeight;
        }
        set
        {
            mFrameHeight = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the header URL
        string url = HeaderUrl;
        if (String.IsNullOrEmpty(url))
        {
            url = "Header.aspx";
        }

        url += RequestContext.CurrentQueryString;

        frmTabs.Attributes.Add("src", UrlResolver.ResolveUrl(url));

        // Setup the content URL
        url = ContentUrl;
        if (String.IsNullOrEmpty(url))
        {
            url = "Tab_General.aspx";
        }

        url += RequestContext.CurrentQueryString;

        frmContent.Attributes.Add("src", UrlResolver.ResolveUrl(url));
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        rowsFrameset.Attributes["rows"] = String.Format("{0}, *", FrameHeight);
        rowsFrameset.Attributes["onload"] = "document.getElementById(\"" + frmContent.ClientID +"\").focus();";
    }
}