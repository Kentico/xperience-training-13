using System;
using System.Web;
using System.Web.UI;

using CMS.UIControls;


public partial class CMSModules_AdminControls_Pages_UIElementControlPage : CMSUIPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (UIElement != null)
        {
            // Load control
            String controlPath = UIElement.ElementTargetURL;
            if (controlPath != String.Empty)
            {
                Control ctrl = Page.LoadControl(HttpUtility.UrlDecode(ResolveUrl(controlPath)));
                pnlControl.Controls.Add(ctrl);
            }
        }
    }
}
