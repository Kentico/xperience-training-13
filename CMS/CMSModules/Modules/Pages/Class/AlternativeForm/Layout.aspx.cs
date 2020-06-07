using System;

using CMS.Base;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[EditedObject(AlternativeFormInfo.OBJECT_TYPE, "altformid")]
public partial class CMSModules_Modules_Pages_Class_AlternativeForm_Layout : GlobalAdminPage
{
    /// <summary>
    /// Edited alternative form.
    /// </summary>
    private AlternativeFormInfo AlternativeForm
    {
        get
        {
            return (AlternativeFormInfo)EditedObject;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = string.Empty;

        // Form ID
        layoutElem.ObjectID = AlternativeForm.FormID;

        ResourceInfo resource = ResourceInfo.Provider.Get(QueryHelper.GetInteger("moduleid", 0));
        if (!SystemContext.DevelopmentMode && (resource != null) && !resource.ResourceIsInDevelopment && !AlternativeForm.FormIsCustom)
        {
            pnlCustomization.MessagesPlaceHolder = layoutElem.MessagesPlaceHolder;
            pnlCustomization.Columns = new string[] { "FormLayout", "FormLayoutType" };
            pnlCustomization.ObjectEditMenu = layoutElem.ObjectEditMenu;

            layoutElem.Enabled = pnlCustomization.IsObjectCustomized;
        }
        else
        {
            pnlCustomization.StopProcessing = true;
        }
    }
}