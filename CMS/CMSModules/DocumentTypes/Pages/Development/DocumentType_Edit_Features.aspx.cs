using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Internal;
using CMS.Helpers;
using CMS.UIControls;

[EditedObject("cms.documenttype", "objectid")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Features : GlobalAdminPage
{
    public DocumentTypeInfo DocumentType
    {
        get
        {
            return (DocumentTypeInfo)form.EditedObject;
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        form.OnBeforeSave += Form_OnBeforeSave;
        form.OnAfterDataLoad += Form_OnAfterDataLoad;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterTooltip(Page);
        ScriptHelper.AppendTooltip(iconHelpPageBuilder, GetString("documenttype_edit_features.pagebuilder.tooltip"), null);
        ScriptHelper.AppendTooltip(iconHelpNavigationItem, GetString("documenttype_edit_features.navigationitem.tooltip"), null);
        ScriptHelper.AppendTooltip(iconHelpHasUrl, GetString("documenttype_edit_features.url.tooltip"), null);
        ScriptHelper.AppendTooltip(iconHelpMetadata, GetString("documenttype_edit_features.metadata.tooltip"), null);
        ScriptHelper.RegisterModule(this, "CMS/PageTypeFeatures", new { highlightSelection = false });
    }


    private void Form_OnAfterDataLoad(object sender, EventArgs e)
    {
        chbPageBuilder.Checked = DocumentType.ClassUsesPageBuilder;
        chbNavigationItem.Checked = DocumentType.ClassIsNavigationItem;
        chbUrl.Checked = DocumentType.ClassHasURL;
        chbMetadata.Checked = DocumentType.ClassHasMetadata;

        if(ExistsPageCreatedOnPageType())
        {
            chbUrl.Enabled = false;
            lblHasURL.Enabled = false;
            chbUrl.ToolTip = ResHelper.GetString("documenttype_edit_features.url.disabledtitle");

            if (!DocumentType.ClassHasURL)
            {
                chbPageBuilder.Enabled = false;
                lblPageBuilder.Enabled = false;
                chbPageBuilder.ToolTip = ResHelper.GetString("documenttype_edit_features.pagebuilder.disabledtitle");
            }
        }
    }
   

    private void Form_OnBeforeSave(object sender, EventArgs e)
    {
        DocumentType.ClassUsesPageBuilder = chbPageBuilder.Checked;
        DocumentType.ClassIsNavigationItem = chbNavigationItem.Checked;
        DocumentType.ClassHasURL = chbUrl.Checked;
        DocumentType.ClassHasMetadata = chbMetadata.Checked;
    }


    private bool ExistsPageCreatedOnPageType()
    {
        return DocumentNodeDataInfoProvider.GetDocumentNodes()
            .WhereEquals("NodeClassID", DocumentType.ClassID)
            .Count > 0;
    }
}