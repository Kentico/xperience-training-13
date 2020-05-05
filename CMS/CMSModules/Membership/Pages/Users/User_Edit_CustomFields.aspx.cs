using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Membership;
using CMS.UIControls;

// Edited object
[EditedObject(UserInfo.OBJECT_TYPE, "userid")]
public partial class CMSModules_Membership_Pages_Users_User_Edit_CustomFields : CMSUsersPage
{
    protected int userId;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (EditedObject != null)
        {
            // Check that only global administrator can edit global administrator's accounts
            UserInfo ui = (UserInfo)EditedObject;
            CheckUserAvaibleOnSite(ui);
            EditedObject = ui;

            if (!CheckGlobalAdminEdit(ui))
            {
                btnOk.Visible = false;
                plcUserCustomFields.Visible = false;
                plcUserSettingsCustomFields.Visible = false;
                ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));
                return;
            }

            // Setup user info for user custom fields dataform
            formUserCustomFields.Info = ui;

            // Setup user settings info for user settings custom fields dataform
            formUserSettingsCustomFields.Info = ui.UserSettings;

            formUserCustomFields.OnBeforeDataLoad += formCustomFields_OnBeforeDataLoad;
            formUserSettingsCustomFields.OnBeforeDataLoad += formCustomFields_OnBeforeDataLoad;
            
            formUserSettingsCustomFields.OnAfterSave += formUserSettingsCustomFields_OnAfterSave;
        }
        else
        {
            formUserCustomFields.StopProcessing = true;
            formUserSettingsCustomFields.StopProcessing = true;
        }
    }


    protected void formUserSettingsCustomFields_OnAfterSave(object sender, EventArgs e)
    {
        // Force staging task creation on save
        formUserSettingsCustomFields.Info.Generalized.TouchParent();
    }


    private void formCustomFields_OnBeforeDataLoad(object sender, EventArgs e)
    {
        DataForm form = (DataForm)sender;
        if (form != null)
        {
            // If table has not any custom field hide custom field placeholder
            if ((form.Info == null) || !form.FormInformation.GetFormElements(true, false, true).Any())
            {
                if (form.Parent is PlaceHolder)
                {
                    form.Parent.Visible = false;
                }
                form.StopProcessing = true;
            }
            else
            {
                // Setup the User DataForm
                form.HideSystemFields = true;
                form.CssClass = "ContentDataFormButton";
                form.SubmitButton.Visible = false;
            }
        }
    }


    protected void btnOk_OnClick(object sender, EventArgs e)
    {
        // Check permissions
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        bool saved = true;

        // Try to save first form
        if (formUserCustomFields.Visible && (formUserCustomFields != null))
        {
            saved &= formUserCustomFields.SaveData(null, false);
        }

        // When first form saved successfully, try to save the second one
        if (saved && formUserSettingsCustomFields.Visible && (formUserSettingsCustomFields != null))
        {
            saved &= formUserSettingsCustomFields.SaveData(null, false);
        }

        // Both forms were saved correctly
        if (saved)
        {
            ShowChangesSaved();
        }
    }
}