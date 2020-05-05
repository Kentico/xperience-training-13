using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_RecycleBin_Pages_ViewVersion : CMSDeskPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Context help is not shown if comparison is disabled
        PageTitle.TitleText = GetString("RecycleBin.ViewVersion");
        // Register tooltip script
        ScriptHelper.RegisterTooltip(Page);

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        RequireSite = false;
    }

    #endregion
}
