using System;

using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Import_cms_resource : ImportExportControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (CheckVersion())
        {
            pnlWarning.Visible = true;
            lblWarning.Text = GetString("ImportObjects.WarningObjectVersion");
        }
        else if (CheckHotfixVersion())
        {
            pnlWarning.Visible = true;
            lblWarning.Text = GetString("ImportObjects.WarningObjectHotfixVersion");
        }
    }
}