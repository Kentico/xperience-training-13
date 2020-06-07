using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[EditedObject("cms.documenttype", "objectid")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Sites : GlobalAdminPage
{
    /// <summary>
    /// Edited document type.
    /// </summary>
    private DataClassInfo DocumentType
    {
        get
        {
            return (DataClassInfo)EditedObject;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (DocumentType.ClassResourceID > 0)
        {
            // Page type belongs to module -> display just simple message
            var moduleName = ResHelper.LocalizeString(ResourceInfo.Provider.Get(DocumentType.ClassResourceID).ResourceDisplayName);
            ShowInformation(String.Format(GetString("DocumentType_Edit_Sites.ModuleInfo"), moduleName));
            classSites.Visible = false;
        }
        else
        {
            // Page type doesn't belong to module -> display site binding edit
            classSites.TitleString = GetString("DocumentType_Edit_Sites.Info");
            classSites.ClassId = DocumentType.ClassID;
            classSites.CheckLicense = false;
        }
    }
}