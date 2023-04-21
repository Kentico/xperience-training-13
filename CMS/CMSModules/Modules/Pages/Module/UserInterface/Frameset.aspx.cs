using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Modules.UserInterface")]
public partial class CMSModules_Modules_Pages_Module_UserInterface_Frameset : GlobalAdminPage
{
    private const string UI_LAYOUT_KEY = nameof(CMSModules_Modules_Pages_Module_UserInterface_Frameset);


    protected void Page_Load(object sender, EventArgs e)
    {
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(uiFrameset);
        }

        int moduleId = QueryHelper.GetInteger("moduleid", 0);
        int elementId = QueryHelper.GetInteger("elementId", 0);

        treeFrame.Attributes["src"] = "Tree.aspx?moduleId=" + moduleId + "&objectParentId=" + moduleId + "&elementId=" + elementId + "&objectId=" + elementId;

        ScriptHelper.HideVerticalTabs(this);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!RequestHelper.IsPostBack() && !RequestHelper.IsCallback())
        {
            var width = UILayoutHelper.GetLayoutWidth(UI_LAYOUT_KEY);
            if (width.HasValue)
            {
                uiFrameset.Attributes["cols"] = $"{width}, *";
            }
        }
    }
}
