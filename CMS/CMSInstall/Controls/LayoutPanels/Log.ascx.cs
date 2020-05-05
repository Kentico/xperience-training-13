using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSInstall_Controls_LayoutPanels_Log : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblLog.Text = ResHelper.GetFileString("Install.lblLog");
    }


    /// <summary>
    /// Displays log.
    /// </summary>
    /// <param name="text">Text to be displayed</param>
    public void DisplayLog(string text)
    {
        txtLog.Text = text;
        plcLog.Visible = true;
    }
}