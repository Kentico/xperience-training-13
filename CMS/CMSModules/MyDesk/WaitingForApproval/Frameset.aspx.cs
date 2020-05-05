using System;

using CMS.UIControls;


public partial class CMSModules_MyDesk_WaitingForApproval_Frameset : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        frm.FrameHeight = TabsOnlyHeight;
    }
}
