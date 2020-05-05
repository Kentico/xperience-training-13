using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

[HashValidation(HashValidationSalts.REDIRECT_PAGE)]
public partial class CMSMessages_Redirect : MessagePage
{
    /// <summary>
    /// OnInit event.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        string url = QueryHelper.GetString("url", String.Empty);     
        CheckHashValidationAttribute = !(URLHelper.IsLocalUrl(url));

        base.OnPreInit(e);
    }


    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {       
        titleElem.TitleText = GetString("Redirect.Header");
        lblInfo.Text = GetString("Redirect.Info");

        string url = QueryHelper.GetString("url", String.Empty);
        string target = QueryHelper.GetString("target", String.Empty);
        string frame = QueryHelper.GetString("frame", String.Empty);
        
        // Change view mode to live site
        bool liveSite = QueryHelper.GetBoolean("livesite", false);
        if (liveSite)
        {
            PortalContext.ViewMode = ViewModeEnum.LiveSite;
        }

        bool urlIsRelative = URLHelper.IsLocalUrl(url);

        url = ResolveUrl(url);

        // Information about the target page
        lnkTarget.Text = HTMLHelper.HTMLEncode(url);
        lnkTarget.NavigateUrl = url;
        lnkTarget.Target = HTMLHelper.EncodeForHtmlAttribute(target);

        string script = $"var url = encodeURI({ScriptHelper.GetString(url)}); ";
        // Generate redirect script
        if (urlIsRelative && frame.Equals("top", StringComparison.OrdinalIgnoreCase))
        {
            script += "if (self.location != top.location) { top.location = url; } else { document.location = url; }";
            ltlScript.Text += ScriptHelper.GetScript(script);
        }
        else if ((target == String.Empty) && (url != String.Empty))
        {
            script += "if (IsCMSDesk()) { window.open(url); } else { document.location = url; }";
            ltlScript.Text += ScriptHelper.GetScript(script);
        }             
    }
}
