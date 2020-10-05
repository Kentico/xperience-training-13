using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Content_CMSPages_Versions_ViewVersion : CMSPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("Content.ViewVersion");
        // Register tooltip script
        ScriptHelper.RegisterTooltip(Page);

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this);
    }

    #endregion
}
