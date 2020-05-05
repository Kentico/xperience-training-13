using System;

using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSPages_Dialogs_UserRegistration : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;
        title.TitleText = GetString("mem.reg.approvaltext");

        // Set administrator e-mail
        registrationApproval.AdministratorEmail = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSAdminEmailAddress");
        registrationApproval.FromAddress = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress");
    }
}