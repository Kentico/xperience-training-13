using System;

using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Preview_PreviewFooter : CMSUserControl
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        btnSaveAndClose.Text = GetString("general.saveandclose");
    }
}
