using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_ChangePassword : CMSModalPage
{
    /// <summary>
    /// Page load event
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;
        title.TitleText = GetString("myaccount.changepassword");

        // Hide footer
        CurrentMaster.FooterContainer.Visible = false;

        ucChangePassword.OnPasswordChange += PasswordChanged;
        ucChangePassword.ForceDifferentPassword = true;
        ucChangePassword.IsLiveSite = false;
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

    /// <summary>
    /// Password changed event
    /// </summary>
    public void PasswordChanged(object sender, EventArgs e)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshWarningHeader" + ClientID, "if(wopener){wopener.top.HideWarning();}", true);
    }
}
