using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Internal;
using CMS.Helpers;
using CMS.UIControls;

[EditedObject("cms.documenttype", "objectid")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Features : GlobalAdminPage
{
    private const string REFRESH_PARENT_SCRIPT = @"
var currentParentUrl = window.parent.location.href.replace(/&saved=1/g, '').replace(/&tabname=([^&]*)/g, '');
window.parent.location.href = currentParentUrl + '&tabname=features&saved=1'";


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

        if (ExistsPageCreatedOnPageType())
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
        var urlFeatureChanged = DocumentType.ClassHasURL != chbUrl.Checked;

        DocumentType.ClassUsesPageBuilder = chbPageBuilder.Checked;
        DocumentType.ClassIsNavigationItem = chbNavigationItem.Checked;
        DocumentType.ClassHasURL = chbUrl.Checked;
        DocumentType.ClassHasMetadata = chbMetadata.Checked;

        if (urlFeatureChanged && !DocumentType.ClassHasURL)
        {
            if (!DocumentType.ClassIsCoupledClass && DocumentType.ClassSearchEnabled)
            {
                // Disable search when search tab is hidden
                DocumentType.ClassSearchEnabled = false;
            }

            if (DocumentType.ClassIsCoupledClass && DocumentType.GetValue("ClassSearchIndexDataSource") != null && DocumentType.ClassSearchIndexDataSource != SearchIndexDataSourceEnum.ContentFields)
            {
                // Set ContentFields data source for coupled pages without URL feature
                DocumentType.ClassSearchIndexDataSource = SearchIndexDataSourceEnum.ContentFields;
            }
        }

        if (urlFeatureChanged)
        {
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "RefreshParent", ScriptHelper.GetScript(REFRESH_PARENT_SCRIPT));
        }
    }


    private bool ExistsPageCreatedOnPageType()
    {
        return DocumentNodeDataInfo.Provider.Get()
            .WhereEquals("NodeClassID", DocumentType.ClassID)
            .Count > 0;
    }
}