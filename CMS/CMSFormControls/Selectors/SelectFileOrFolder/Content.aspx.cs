using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_SelectFileOrFolder_Content : CMSModalPage
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        ValidateHash();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);
        CMSDialogHelper.RegisterDialogHelper(Page);
        ScriptManager.RegisterStartupScript(Page, typeof(Page), "InitResizers", "$cmsj(InitResizers());", true);

        fileSystem.InitFromQueryString();
    }
}
