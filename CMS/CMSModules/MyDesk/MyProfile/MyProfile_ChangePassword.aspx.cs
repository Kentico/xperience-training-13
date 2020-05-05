using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


[UIElement("CMS", "MyProfile.ChangePassword")]
public partial class CMSModules_MyDesk_MyProfile_MyProfile_ChangePassword : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ucChangePassword.OnPasswordChange += PasswordChanged;
    }


    public void PasswordChanged(object sender, EventArgs e)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshWarningHeader" + ClientID, "window.top.HideWarning();", true);
    }
}
