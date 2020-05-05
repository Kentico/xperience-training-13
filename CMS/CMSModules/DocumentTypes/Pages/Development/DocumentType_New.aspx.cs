using System;

using CMS.UIControls;


[Title(HelpTopic = "new_document_type")]

[Breadcrumb(0, "DocumentType_New.DocumentTypes", "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_List.aspx", "")]
[Breadcrumb(1, "DocumentType_New.CurrentDocumentType")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_New : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the inner control theme property
        newDocWizard.Theme = Theme;

        CurrentMaster.BodyClass += " FieldEditorWizardBody";
    }
}