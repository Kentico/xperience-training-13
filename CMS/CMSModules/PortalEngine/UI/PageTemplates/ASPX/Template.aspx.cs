using System;

using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_PageTemplates_ASPX_Template : TemplatePage
{
    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        PageManager = CMSPortalManager;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ltlTags.Text = HeaderTags;
    }
}