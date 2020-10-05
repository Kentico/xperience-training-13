using System;

using CMS.UIControls;


public partial class CMSModules_Membership_CMSPages_ChangePassword : CMSPage
{
    /// <summary>
    /// Page load event
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;
        title.TitleText = GetString("myaccount.changepassword");
        title.IsDialog = false;

        // Hide footer
        CurrentMaster.FooterContainer.Visible = false;

        ucChangePassword.ForceDifferentPassword = true;
        ucChangePassword.IsLiveSite = true;
    }


    /// <summary>
    /// PreRender event
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        RegisterEscScript();
        RegisterModalPageScripts();
    }
}