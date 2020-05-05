using System;

using CMS.UIControls;

public partial class _Default : TemplatePage
{
    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        PageManager = manPortal;
        ManagersContainer = plcManagers;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblText.Text = "The web site doesn't contain any content. Sign in to <a href=\"" + ResolveUrl("~/Admin/cmsadministration.aspx") + "\">administration</a> and edit the content.";
        ltlTags.Text = HeaderTags;
    }
}