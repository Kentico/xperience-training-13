using System;

using CMS.UIControls;

public partial class CMSMasterPages_LiveSite_LiveTree : CMSLiveMasterPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        PageStatusContainer = plcStatus;
    }
}