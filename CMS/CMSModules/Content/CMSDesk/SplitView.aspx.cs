using System;
using System.Collections.Specialized;

using CMS.Helpers;

using System.Text;
using System.Web;

using CMS.Base.Web.UI;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_SplitView : CMSContentPage
{
    #region "Properties"

    private static string ScriptPrefix
    {
        get
        {
            // Set script prefix
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("culture", "##c##");
            string scriptPrefix = VirtualContext.GetVirtualContextPrefix(parameters);
            return scriptPrefix;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        EnsureDocumentManager = false;
        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string url = QueryHelper.GetString("splitUrl", null);

        if (!string.IsNullOrEmpty(url) && PortalUIHelper.DisplaySplitMode)
        {
            // Register script files
            ltlScript.Text += ScriptHelper.GetScriptTag("~/CMSModules/Content/CMSDesk/SplitView.js");

            // Decode URL
            url = HttpUtility.UrlDecode(url);
            docSplitView.NodeID = ValidationHelper.GetInteger(URLHelper.GetQueryValue(url, "nodeid"), 0);
            docSplitView.Culture = ValidationHelper.GetString(URLHelper.GetQueryValue(url, "culture"), null);

            // Update view mode (e.g. to remember edit tabs)
            PortalContext.UpdateViewMode(PortalContext.ViewMode);

            // Set frame1 URL
            docSplitView.Frame1Url = url;

            // Get the URL of the compare frame
            UIPageURLSettings settings = new UIPageURLSettings
                                               {
                                                   Mode = Mode,
                                                   NodeID = NodeID,
                                                   Culture = PortalUIHelper.SplitModeCultureCode,
                                                   SplitViewSourceURL = url,
                                                   TransformToCompareUrl = true
                                               };
            // Get URL
            docSplitView.Frame2Url = DocumentUIHelper.GetDocumentPageUrl(settings);

            StringBuilder script = new StringBuilder();
            script.Append(
@"
function InitSplitViewSyncScroll(frameElement, body, refreshSameCulture, unbind) {
  if (InitSyncScroll) {
    InitSyncScroll(frameElement, body, refreshSameCulture, unbind);
  }
}
function SplitModeRefreshFrame() {
  if (FSP_SplitModeRefreshFrame) {
    FSP_SplitModeRefreshFrame();
  }
}

var FSP_appPref = '", URLHelper.GetFullApplicationUrl(), @"';
var FSP_cntPref = '", ScriptPrefix, @"';
");

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "splitViewSync_" + Page.ClientID, script.ToString(), true);
        }
    }

    #endregion
}
