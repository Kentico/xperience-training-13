using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineForms;
using CMS.OnlineForms.Web.UI;
using CMS.UIControls;

// Edited object
[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
// Check permissions
[Security(Resource = "CMS.Form", UIElements = "Forms.OnlineMarketing")]
[Security(Resource = "CMS.ContactManagement", Permission = "Modify")]
[Security(Resource = "CMS.Form", UIElements = "Forms.Properties")]
[SaveAction(0)]
[UIElement("CMS.Form", "Forms.OnlineMarketing")]
public partial class CMSModules_BizForms_Tools_BizForm_Edit_OnlineMarketing : CMSBizFormPage
{
    #region "Variables"

    private CMSUserControl mapControl = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Get form info
        BizFormInfo formInfo = EditedObject as BizFormInfo;
        if (formInfo == null)
        {
            return;
        }

        // Get class of the form
        DataClassInfo classInfo = DataClassInfoProvider.GetDataClassInfo(formInfo.FormClassID);

        // Load mapping dialog control and initialize it
        plcMapping.Controls.Clear();
        mapControl = (CMSUserControl)Page.LoadUserControl("~/CMSModules/ContactManagement/Controls/UI/Contact/MappingDialog.ascx");
        if (mapControl != null)
        {
            mapControl.ID = "ctrlMapping";
            mapControl.SetValue("classname", classInfo.ClassName);
            mapControl.SetValue("allowoverwrite", classInfo.ClassContactOverwriteEnabled);
            plcMapping.Controls.Add(mapControl);
        }

        if (!RequestHelper.IsPostBack())
        {
            // Initialize checkbox value and mapping dialog visibility
            chkLogActivity.Checked = formInfo.FormLogActivity;
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Actions handler - saves the changes.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Update the form object and its class
        BizFormInfo form = EditedObject as BizFormInfo;
        if ((form != null) && (mapControl != null))
        {
            if (plcMapping.Visible)
            {
                // Update mapping of the form class only if mapping dialog is visible
                DataClassInfo classInfo = DataClassInfoProvider.GetDataClassInfo(form.FormClassID);
                if (classInfo != null)
                {
                    classInfo.ClassContactOverwriteEnabled = ValidationHelper.GetBoolean(mapControl.GetValue("allowoverwrite"), false);
                    classInfo.ClassContactMapping = ValidationHelper.GetString(mapControl.GetValue("mappingdefinition"), string.Empty);
                    DataClassInfoProvider.SetDataClassInfo(classInfo);
                }
            }

            // Update the form
            form.FormLogActivity = chkLogActivity.Checked;
            BizFormInfo.Provider.Set(form);

            // Show save information
            ShowChangesSaved();
        }
    }

    #endregion
}