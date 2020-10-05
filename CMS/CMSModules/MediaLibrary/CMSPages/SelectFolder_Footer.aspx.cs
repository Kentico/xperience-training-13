using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_CMSPages_SelectFolder_Footer : CMSPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        SetBrowserClass();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            // Register custom css if exists
            RegisterDialogCSSLink();
        }
        else
        {
            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "redirect", ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }"));
        }
    }
}
