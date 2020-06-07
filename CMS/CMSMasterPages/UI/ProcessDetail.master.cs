using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSMasterPages_UI_ProcessDetail : CMSProcessDetailMasterPage
{
    /// <summary>
    /// Automation manager component.
    /// </summary>
    protected override CMSAutomationManager AutomationManager => autoMan;


    /// <summary>
    /// HeaderActions control.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get => menuElem.HeaderActions;
        set => menuElem.HeaderActions = value;
    }


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