using System;

using CMS.UIControls;


public partial class CMSModules_Membership_CMSPages_ResetPassword : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;
        title.TitleText = GetString("passreset.title");
    }
}