using System;

using CMS.Base;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;


[EditedObject("cms.documenttype", "objectid")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_General : GlobalAdminPage
{
    #region "Variables"

    private string oldClassName;
    private int oldInheritedID;
    private int oldResourceID;

    #endregion


    #region "Properties"

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

    #endregion


    #region "Methods"

    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get original value
        oldClassName = DocumentType.ClassName;
        oldInheritedID = DocumentType.ClassInheritsFromClassID;
        oldResourceID = DocumentType.ClassResourceID;

        var link = String.Format("<a target=\"_blank\" href=\"{0}\">{1}</a>", DocumentationHelper.GetDocumentationTopicUrl("routing_overview"), GetString("general.ourdocumentation"));
        tipUrlPattern.Content = string.Format(GetString("documenttype.urlpattern.smarttip.content"), link);
        tipUrlPattern.Visible = DocumentType.ClassHasURL;

        // Bind events
        editElem.OnAfterSave += editElem_OnAfterSave;
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void editElem_OnAfterSave(object sender, EventArgs e)
    {
        string newClassName = DocumentType.ClassName;
        bool classNameChanged = !string.Equals(oldClassName, newClassName, StringComparison.InvariantCultureIgnoreCase);

        if (classNameChanged)
        {
            // Move icons if class name was changed
            RefreshIcon(oldClassName, newClassName, DocumentType.ClassIsCoupledClass, null);
            RefreshIcon(oldClassName, newClassName, DocumentType.ClassIsCoupledClass, "48x48");
        }

        // Ensure (update) the inheritance
        int inheritedID = DocumentType.ClassInheritsFromClassID;
        if (inheritedID != oldInheritedID)
        {
            if (inheritedID > 0)
            {
                // Update the inherited fields
                DataClassInfo parentClass = DataClassInfoProvider.GetDataClassInfo(inheritedID);
                if (parentClass != null)
                {
                    FormHelper.UpdateInheritedClass(parentClass, DocumentType);
                }
            }
            else
            {
                // Remove the inherited fields
                FormHelper.RemoveInheritance(DocumentType, false);
            }

            // Clear class structures
            ClassStructureInfo.Remove(DocumentType.ClassName, true);
        }

        // Synchronize site bindings
        if (oldResourceID != DocumentType.ClassResourceID)
        {
            DocumentTypeHelper.SynchronizeSiteBindingsWithResource(DocumentType as DocumentTypeInfo);
        }
    }


    /// <summary>
    /// Refreshes document type icons.
    /// </summary>
    /// <param name="className">Original class name</param>
    /// <param name="newClassName">New class name</param>
    /// <param name="isCoupled">True if edited document type isn't only container</param>
    /// <param name="iconSet">Icon set</param>
    private void RefreshIcon(string className, string newClassName, bool isCoupled, string iconSet)
    {
        string sourceFile = UIHelper.GetDocumentTypeIconPath(this, className, iconSet);
        string targetFile = UIHelper.GetDocumentTypeIconPath(this, newClassName, iconSet, false);
        string defaultFile = UIHelper.GetDocumentTypeIconPath(this, "default", iconSet, false);

        sourceFile = Server.MapPath(sourceFile);
        targetFile = Server.MapPath(targetFile);
        defaultFile = Server.MapPath(defaultFile);

        // Ensure same extension
        if (sourceFile.ToLowerCSafe().EndsWithCSafe(".gif"))
        {
            targetFile = targetFile.Replace(".png", ".gif");
        }

        // Rename icon file
        if (File.Exists(sourceFile))
        {
            try
            {
                if (sourceFile != defaultFile)
                {
                    File.Move(sourceFile, targetFile);
                }
                else
                {
                    // Copy default file
                    if (!isCoupled)
                    {
                        sourceFile = UIHelper.GetDocumentTypeIconPath(this, "defaultcontainer", iconSet, false);
                        sourceFile = Server.MapPath(sourceFile);
                    }
                    if (File.Exists(sourceFile))
                    {
                        File.Copy(sourceFile, targetFile);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Service.Resolve<IEventLogService>().LogException("Page types", "MOVEICON", ex);
            }
        }
    }

    #endregion
}
