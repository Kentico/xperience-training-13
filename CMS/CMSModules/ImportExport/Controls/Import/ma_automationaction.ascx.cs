using System;

using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Import_ma_automationaction : ImportExportControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (CheckVersion())
        {
            pnlWarning.Visible = true;
            lblWarning.Text = GetString("ImportObjects.WarningObjectDefinitionVersion");
        }
        else if (CheckHotfixVersion())
        {
            pnlWarning.Visible = true;
            lblWarning.Text = GetString("ImportObjects.WarningObjectDefinitionHotfixVersion");
        }
    }
}
