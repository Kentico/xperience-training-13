using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[EditedObject("cms.class", "classid")]
public partial class CMSModules_Modules_Pages_Class_Layout : GlobalAdminPage
{
    #region "Private properties"

    /// <summary>
    /// Edited object.
    /// </summary>
    private DataClassInfo ClassInfo
    {
        get
        {
            return (DataClassInfo)EditedObject;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = string.Empty;

        // Set layout type in order to class type
        if (ClassInfo != null)
        {
            if (ClassInfo.ClassIsDocumentType)
            {
                layoutElem.FormLayoutType = FormLayoutTypeEnum.Document;
            }
            else if (ClassInfo.ClassIsCustomTable)
            {
                layoutElem.FormLayoutType = FormLayoutTypeEnum.CustomTable;
            }
            else
            {
                layoutElem.FormLayoutType = FormLayoutTypeEnum.SystemTable;
            }

            layoutElem.ObjectID = ClassInfo.ClassID;
        }

        ResourceInfo resource = ResourceInfo.Provider.Get(QueryHelper.GetInteger("moduleid", 0));
        if (!SystemContext.DevelopmentMode && (resource != null) && !resource.ResourceIsInDevelopment)
        {
            pnlCustomization.MessagesPlaceHolder = layoutElem.MessagesPlaceHolder;
            pnlCustomization.Columns = new string[] { "ClassFormLayout", "ClassFormLayoutType" };
            pnlCustomization.HeaderActions = HeaderActions;

            layoutElem.Enabled = pnlCustomization.IsObjectCustomized;
        }
        else
        {
            pnlCustomization.StopProcessing = true;
        }
    }

    #endregion
}
