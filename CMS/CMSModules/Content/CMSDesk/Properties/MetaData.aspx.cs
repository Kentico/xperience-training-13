using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


[Security(Resource = "CMS.Content", UIElements = "Properties.MetaData")]
public partial class CMSModules_Content_CMSDesk_Properties_MetaData : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Enable split mode
        EnableSplitMode = true;

        // Register the scripts
        ScriptHelper.RegisterLoader(Page);

        MetaDataControlExtender extender = new MetaDataControlExtender();
        extender.UIModuleName = "cms.content";
        extender.UIPageElementName = "metadata.page";
        extender.UITagsElementName = "metadata.tags";
        extender.Init(editForm);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        pnlContent.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlContent.Enabled = DocumentManager.AllowSave;
    }

    #endregion
}
