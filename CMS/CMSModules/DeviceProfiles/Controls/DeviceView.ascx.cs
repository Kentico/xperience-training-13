using System;

using CMS.Base.Web.UI;
using CMS.DeviceProfiles;
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
    /// If true, device should be displayed rotated.
    /// </summary>
    public bool RotateDevice
    {
        get;
        set;
    }


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
        // Get device ID from query string
        String deviceName = QueryHelper.GetString(DeviceProfileInfoProvider.DEVICENAME_QUERY_PARAM, String.Empty);

        // If device profile not set, use current device profile
        DeviceProfileInfo deviceProfile = DeviceContext.CurrentDeviceProfile;

        // Add hash control for X-Frame-Option
        if (deviceName != String.Empty)
        {
            ViewPage = URLHelper.UpdateParameterInUrl(ViewPage, DeviceProfileInfoProvider.DEVICENAME_QUERY_PARAM, deviceName);

            String query = URLHelper.GetQuery(ViewPage);
            string hash = ValidationHelper.GetHashString(query, new HashSettings(RequestContext.UserName));
            ViewPage += String.Format("&clickjackinghash={0}", hash);
        }

        // If device's boundaries are set, use iframe
        if ((deviceProfile != null) && (deviceProfile.ProfilePreviewWidth > 0) && (deviceProfile.ProfilePreviewHeight > 0))
        {
            // Remove frame scrolling
            mFramescroll = "no";

            // Register device css from site name folder or design folder
            RegisterDeviceProfileCss(deviceProfile.ProfileName);

            pnlDevice.CssClass += " " + deviceProfile.ProfileName;

            string deviceScript = "CMSView.RotateCookieName = '" + CookieName.CurrentDeviceProfileRotate + "';";
            deviceScript += String.Format("CMSView.InitializeFrame({0}, {1}, {2}); CMSView.ResizeContentArea();",
                     deviceProfile.ProfilePreviewWidth,
                     deviceProfile.ProfilePreviewHeight,
                     (RotateDevice ? "true" : "false"));

            ScriptHelper.RegisterStartupScript(this, typeof(string), "InitializeDeviceFrame", deviceScript, true);
        }
        else
        {
            // Hide all device frame divs
            pnlTop.Visible = false;
            pnlBottom.Visible = false;
            pnlLeft.Visible = false;
            pnlRight.Visible = false;
            pnlCenter.CssClass = String.Empty;
            pnlCenterLine.CssClass = String.Empty;
        }
    }
    

    /// <summary>
    /// Registers global device profiles style sheet and style sheet for given profile if exists.
    /// </summary>
    /// <param name="profileName">Profile code name used as folder name in ~/App_Themes/Components/DeviceProfile/ folder</param>
    private void RegisterDeviceProfileCss(string profileName)
    {
        // Global device profiles css
        CssRegistration.RegisterCssLink(Page, "~/App_Themes/Design/DeviceProfile.css");

        string styleUrl = string.Format("~/App_Themes/Components/DeviceProfile/{0}/DeviceProfile.css", profileName);
        if (FileHelper.FileExists(styleUrl))
        {
            CssRegistration.RegisterCssLink(Page, styleUrl);
        }
    }

    #endregion
}
