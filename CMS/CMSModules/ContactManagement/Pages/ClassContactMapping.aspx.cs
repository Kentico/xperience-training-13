using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Modules;
using CMS.UIControls;


/// <summary>
/// Page which displays control for mapping contact properties to user/subscriber/customer properties.
/// </summary>
[EditedObject("cms.class", "classid")]
[UIElement("CMS.ContactManagement", "ClassContactMapping")]
public partial class CMSModules_ContactManagement_Pages_ClassContactMapping : GlobalAdminPage
{
    #region "Private variables"

    private CMSUserControl mapControl;

    #endregion


    #region "Properties"

    /// <summary>
    /// Edited class info.
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
        // Init header actions
        CurrentMaster.HeaderActions.ActionsList.Add(new SaveAction());

        // Register event for save action
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, SaveButton_Click);

        // Get data class info
        if (ClassInfo == null)
        {
            return;
        }

        // Load mapping dialog control and initialize it
        plcMapping.Controls.Clear();
        mapControl = (CMSUserControl)Page.LoadUserControl("~/CMSModules/ContactManagement/Controls/UI/Contact/MappingDialog.ascx");
        mapControl.ID = "ctrlMapping";
        mapControl.SetValue("classname", ClassInfo.ClassName);
        mapControl.SetValue("allowoverwrite", ClassInfo.ClassContactOverwriteEnabled);
        mapControl.IsLiveSite = false;
        plcMapping.Controls.Add(mapControl);

        ResourceInfo resource = ResourceInfo.Provider.Get(QueryHelper.GetInteger("moduleid", 0));
        if (!SystemContext.DevelopmentMode && (resource != null) && !resource.ResourceIsInDevelopment)
        {
            pnlCustomization.MessagesPlaceHolder = MessagesPlaceHolder;
            pnlCustomization.Columns = new string[] { "ClassContactMapping", "ClassContactOverwriteEnabled" };
            pnlCustomization.HeaderActions = HeaderActions;
        }
        else
        {
            pnlCustomization.StopProcessing = true;
        }
    }


    /// <summary>
    /// Actions handler - saves the changes.
    /// </summary>
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        // Update the class object
        if ((ClassInfo != null) && (mapControl != null))
        {
            ClassInfo.ClassContactOverwriteEnabled = ValidationHelper.GetBoolean(mapControl.GetValue("allowoverwrite"), false);
            ClassInfo.ClassContactMapping = ValidationHelper.GetString(mapControl.GetValue("mappingdefinition"), string.Empty);
            DataClassInfoProvider.SetDataClassInfo(ClassInfo);

            // Show save information
            ShowChangesSaved();
        }
    }

    #endregion
}