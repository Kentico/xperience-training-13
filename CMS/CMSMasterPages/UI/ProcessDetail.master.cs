using System;
using System.Web.UI;

using CMS.ContactManagement.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSMasterPages_UI_ProcessDetail : CMSProcessDetailMasterPage
{
    /// <summary>
    /// Automation manager component.
    /// </summary>
    protected override CMSAutomationManager AutomationManager => autoMan;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        AutomationManager.StateObjectID = QueryHelper.GetInteger("stateid", 0);
        if (AutomationManager.Process != null)
        {
            menuElem.OnClientStepChanged = Page.ClientScript.GetPostBackEventReference(pnlUp, null);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        pnlDetail.Enabled = !AutomationManager.ProcessingAction;
    }
}