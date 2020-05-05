using System;

using CMS.Base;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSInstall_Controls_LayoutPanels_Version : CMSUserControl
{
    /// <summary>
    /// Indicates if 'Contact support' label should be displayed. Applicable for installation only.
    /// </summary>
    public bool DisplaySupportLabel
    {
        get;
        set;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        lblSupport.Visible = DisplaySupportLabel;
        if (DisplaySupportLabel)
        {
            lblSupport.Text = ResHelper.GetFileString("install.Support");
        }
        lblVersion.Text = ResHelper.GetFileString("install.Version") + "&nbsp;" +
                          CMSVersion.GetFriendlySystemVersion(true);

    }
}