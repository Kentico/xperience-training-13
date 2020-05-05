using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_Macros_Dialogs_Header : CMSModalPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        InitTabs("insertContent");
        SetTab(0, GetString("insertmacro.insertmacrotree"), (URLHelper.ResolveUrl("~/CMSAdminControls/UI/Macros/Dialogs/Tab_InsertMacroTree.aspx") + RequestContext.CurrentQueryString), "");
        SetTab(1, GetString("insertmacro.insertmacrocode"), URLHelper.ResolveUrl("~/CMSAdminControls/UI/Macros/Dialogs/Tab_InsertMacroCode.aspx") + RequestContext.CurrentQueryString, "");

        CurrentMaster.Tabs.UrlTarget = "insertContent";

        PageTitle.TitleText = GetString("insertmacro.dialogtitle");
    }
}