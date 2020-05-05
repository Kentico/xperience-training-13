using System;

using CMS.Core;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


[Tabs("CMS.Content", "Pending", "content")]
public partial class CMSModules_MyDesk_WaitingForApproval_Header : CMSContentManagementPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Pending";
        PageTitle.TitleText = "Pending";
        OnTabCreated += Header_OnTabCreated;
    }


    protected void Header_OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        switch (e.TabIndex)
        {
            case 1:
                if ((!ModuleEntryManager.IsModuleLoaded(ModuleName.ONLINEMARKETING) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleName.ONLINEMARKETING, SiteContext.CurrentSiteName)))
                {
                    // Hide tab if module is not loaded
                    e.Tab = null;
                }
                break;
        }
    }

    #endregion
}
