using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


[UIElement("CMS.Categories", "MyProfile.Categories")]
public partial class CMSModules_MyDesk_MyProfile_MyProfile_Categories : CMSContentManagementPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ScriptHelper.HideVerticalTabs(this);
    }
}
