using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


[Security(Resource = "CMS.Content", UIElements = "Properties.Tags")]
public partial class CMSModules_Content_CMSDesk_Properties_Tags : CMSPropertiesPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Enable split mode
        EnableSplitMode = true;

        DocumentManager.UseDocumentHelper = false;


        // Register the scripts
        ScriptHelper.RegisterLoader(Page);

        MetaDataControlExtender extender = new MetaDataControlExtender();
        extender.UIModuleName = "cms.content";
        extender.UITagsElementName = "properties.tags";
        extender.Init(editForm);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlContent.Enabled = DocumentManager.AllowSave;
    }
}
