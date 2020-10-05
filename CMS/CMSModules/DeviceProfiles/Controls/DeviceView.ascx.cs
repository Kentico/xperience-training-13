using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_DeviceProfiles_Controls_DeviceView : CMSAdminControl
{
    #region "Variables"

    private string mViewPage;
    private string mFramescroll = "auto";

    #endregion


    #region "Properties"

    /// <summary>
    /// URL of page in device's iframe.
    /// </summary>
    public String ViewPage
    {
        get
        {
            return mViewPage ?? (mViewPage = QueryHelper.GetString("viewpage", string.Empty));
        }
        set
        {
            mViewPage = value;
        }
    }


    /// <summary>
    /// ID of the device's iframe.
    /// </summary>
    public string FrameID
    {
        get;
    } = "pageview";


    /// <summary>
    /// Returns encoded URL of page
    /// </summary>
    protected String ViewPageSource
    {
        get
        {
            return HTMLHelper.EncodeForHtmlAttribute(ViewPage);
        }
    }


    /// <summary>
    /// Returns frame scrolling value depending on device
    /// </summary>
    protected string FrameScroll
    {
        get
        {
            return mFramescroll;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register JS
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterJQueryCookie(Page);

        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/jquery-jscrollpane.js");
        CssRegistration.RegisterCssLink(Page, "~/CMSScripts/jquery/jquery-jscrollpane.css");

        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Content/CMSDesk/View/View.js");

        // Init preview
        InitializeDevicePreview();
    }


    /// <summary>
    /// Initialize device preview frame.
    /// </summary>
    private void InitializeDevicePreview()
    {
        // Hide all device frame divs
        pnlTop.Visible = false;
        pnlBottom.Visible = false;
        pnlLeft.Visible = false;
        pnlRight.Visible = false;
        pnlCenter.CssClass = String.Empty;
        pnlCenterLine.CssClass = String.Empty;
    }

    #endregion
}
