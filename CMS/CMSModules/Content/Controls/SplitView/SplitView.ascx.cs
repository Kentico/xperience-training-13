using System;

using CMS.Helpers;

using System.Text;

using CMS.Base.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_SplitView_SplitView : CMSUserControl
{
    #region "Variables"

    private string mBlankUrl = "about:blank";

    #endregion


    #region "Properties"

    /// <summary>
    /// Frame1 URL.
    /// </summary>
    public string Frame1Url
    {
        get;
        set;
    }


    /// <summary>
    /// Frame2 URL.
    /// </summary>
    public string Frame2Url
    {
        get;
        set;
    }


    /// <summary>
    /// Toolbar URL.
    /// </summary>
    public string ToolbarUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Separator URL.
    /// </summary>
    public string SeparatorUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Toolbar height.
    /// </summary>
    public int ToolbarHeight
    {
        get;
        set;
    }


    /// <summary>
    /// Blank URL.
    /// </summary>
    public string BlankUrl
    {
        get
        {
            return mBlankUrl;
        }
        set
        {
            mBlankUrl = value;
        }
    }


    /// <summary>
    /// Split mode.
    /// </summary>
    protected SplitModeEnum SplitMode
    {
        get
        {
            return PortalUIHelper.SplitMode;
        }
    }


    /// <summary>
    ///  Indicates if the split mode should be displayed.
    /// </summary>
    protected bool DisplaySplitMode
    {
        get
        {
            return PortalUIHelper.DisplaySplitMode;
        }
    }


    /// <summary>
    ///  Indicates if the scrollbars should be synchronized.
    /// </summary>
    protected bool SynchronizeScrollbar
    {
        get
        {
            return PortalUIHelper.SplitModeSyncScrollbars;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init split script
        if (DisplaySplitMode)
        {
            // Resolve URLs
            string frame1Url = ResolveUrl(Frame1Url);
            string frame2Url = ResolveUrl(Frame2Url);
            frame2Url = URLHelper.AddParameterToUrl(frame2Url, "compare", "1");
            string toolbarUrl = ResolveUrl(ToolbarUrl);
            string separatorUrl = ResolveUrl(SeparatorUrl);

            switch (SplitMode)
            {
                // Horizontal or insignificant mode
                case SplitModeEnum.Horizontal:
                    // Additional border size
                    mainFrameset.Attributes["rows"] = "*," + ToolbarHeight + ",*";
                    toolbarframe.Attributes["src"] = toolbarUrl;
                    frame1.Attributes["src"] = frame1Url;
                    verticalFrameset.Attributes["cols"] = "0%,0%,100%";
                    frame1Vertical.Attributes["src"] = BlankUrl;
                    frameSeparator.Attributes["src"] = BlankUrl;
                    frame2.Attributes["src"] = frame2Url;
                    break;

                // Vertical mode
                case SplitModeEnum.Vertical:
                    mainFrameset.Attributes["rows"] = "0%," + ToolbarHeight + ",*";
                    toolbarframe.Attributes["src"] = toolbarUrl;
                    frame1.Attributes["src"] = BlankUrl;
                    verticalFrameset.Attributes["cols"] = "*,8,*";
                    frame1Vertical.Attributes["src"] = frame1Url;
                    frameSeparator.Attributes["src"] = separatorUrl;
                    frame2.Attributes["src"] = frame2Url;
                    break;

                // Default mode
                default:
                    plcSplit.Visible = false;
                    mainFrameset.Attributes["rows"] = "*";
                    frame1.Attributes["src"] = Frame1Url;
                    frame2.Attributes["src"] = BlankUrl;
                    frame1Vertical.Attributes["src"] = BlankUrl;
                    frameSeparator.Attributes["src"] = BlankUrl;
                    toolbarframe.Attributes["src"] = BlankUrl;
                    break;
            }

            string syncScrollbars = SynchronizeScrollbar ? "1" : "0";
            string displaySplitMode = DisplaySplitMode ? "1" : "0";

            // Register Init script
            StringBuilder initScript = new StringBuilder();
            initScript.Append("FSP_Init('", frame1Url, "','", frame2Url, "','", BlankUrl, "','", separatorUrl, "','", mainFrameset.ClientID, "','", frame1.ClientID, "','",
                              frame1Vertical.ClientID, "','", frame2.ClientID, "','", verticalFrameset.ClientID, "','", frameSeparator.ClientID, "',", ToolbarHeight, ",'",
                              Convert.ToInt32(SplitMode), "',", syncScrollbars, ",", displaySplitMode, ");");

            // Register js scripts
            ScriptHelper.RegisterApplicationConstants(Page);
            ScriptHelper.RegisterJQueryCookie(Page);
            ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Content/Controls/SplitView/SplitView.js");
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "FSP_initFrames", ScriptHelper.GetScript(initScript.ToString()));

            if (CultureHelper.IsUICultureRTL())
            {
                ControlsHelper.ReverseFrames(verticalFrameset);
            }
        }

        // Set isSplitView flag to true for header breadcrumbs to stop getting more
        // breadcrumbs from this frame and from deeper frames
        RequestContext.ClientApplication.Add("isSplitView", true);
    }

    #endregion
}
