using System;

using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

[UIElement("CMS", "MyProfile.Details")]
public partial class CMSModules_MyDesk_MyProfile_MyProfile_MyDetails : CMSContentManagementPage
{
    /// <summary>
    /// Init event handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get up-to-date info of current user and use it for the form
        var user = UserInfo.Provider.Get(CurrentUser.UserID);
        if (user != null)
        {
            editProfileForm.Info = user.Clone();
        }
    }


    /// <summary>
    /// OnAfterDataLoad handler.
    /// </summary>
    protected void editProfileForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Alter username according to GetFormattedUserName function
        if (editProfileForm.FieldEditingControls != null)
        {
            var userControl = editProfileForm.FieldEditingControls["UserName"];
            if (userControl != null)
            {
                string userName = ValidationHelper.GetString(userControl.Value, String.Empty);

                // Set back formatted username
                userControl.Value = HTMLHelper.HTMLEncode(UserInfoProvider.GetFormattedUserName(userName, null));
            }
        }
    }


    /// <summary>
    /// OnAfterSave handler.
    /// </summary>
    protected void editProfileForm_OnAfterSave(object sender, EventArgs e)
    {
        // Update current contact info
        var classInfo = DataClassInfoProvider.GetDataClassInfo(editProfileForm.ClassName);
        ContactInfoProvider.UpdateContactFromExternalData(editProfileForm.Info, classInfo.ClassContactOverwriteEnabled, ModuleCommands.OnlineMarketingGetCurrentContactID());
    }


    /// <summary>
    /// OnBeforeSave handler.
    /// </summary>
    protected void editProfileForm_OnBeforeSave(object sender, EventArgs e)
    {
        // If avatarUser id column is set
        var avatarId = editProfileForm.Data.GetValue("UserAvatarID");
        if (avatarId != DBNull.Value)
        {
            // If Avatar not set, rewrite to null value
            if (ValidationHelper.GetInteger(avatarId, 0) == 0)
            {
                editProfileForm.Data.SetValue("UserAvatarID", DBNull.Value);
            }
        }

        // Set full name as first name + last name
        if (CurrentUser != null)
        {
            string firstName = ValidationHelper.GetString(editProfileForm.Data.GetValue("FirstName"), "");
            string lastName = ValidationHelper.GetString(editProfileForm.Data.GetValue("LastName"), "");
            string middleName = ValidationHelper.GetString(editProfileForm.Data.GetValue("MiddleName"), "");

            String fullName = ValidationHelper.GetString(editProfileForm.Data.GetValue("FullName"), "");
            var cui = CurrentUser;

            // Change full name only if it was automatically generated (= is equals to first + middle + last name)
            if (fullName == UserInfoProvider.GetFullName(cui.FirstName, cui.MiddleName, cui.LastName))
            {
                editProfileForm.Data.SetValue("FullName", UserInfoProvider.GetFullName(firstName, middleName, lastName));
            }
        }

        // Ensure unique user email
        string email = ValidationHelper.GetString(editProfileForm.Data.GetValue("Email"), "").Trim();

        // Get current user info
        UserInfo user = editProfileForm.Info as UserInfo;

        // Check if user email is unique in all sites where he belongs
        if (!UserInfoProvider.IsEmailUnique(email, user))
        {
            ShowError(GetString("UserInfo.EmailAlreadyExist"));
            editProfileForm.StopProcessing = true;
        }
    }
}