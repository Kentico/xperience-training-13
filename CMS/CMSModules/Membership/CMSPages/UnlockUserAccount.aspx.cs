using System;
using System.Linq;

using CMS.UIControls;


public partial class CMSModules_Membership_CMSPages_UnlockUserAccount : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;
        title.TitleText = GetString("invalidlogonattempts.unlockaccount.title");
    }
}