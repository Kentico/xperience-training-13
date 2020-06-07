using System;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base.UploadExtensions;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;


public partial class CMSModules_Membership_FormControls_Avatars_UserPictureEdit : FormEngineUserControl
{
    #region "Variables"

    private UserInfo mUserInfo;
    private UserInfo mFormUserInfo;
    private UserInfo mEditedUser;
    private int mMaxSideSize;
    private int avatarID;
    private CMSAdminControls_UI_UserPicture mUserPicture;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets user info from basic form.
    /// </summary>
    private UserInfo FormUserInfo
    {
        get
        {
            if ((mFormUserInfo == null) && (Data != null))
            {
                // Get edited user from basic form
                mFormUserInfo = Data as UserInfo;
            }
            return mFormUserInfo;
        }
    }


    /// <summary>
    /// Gets edited user's info. User info from basic form has higher priority than user info.
    /// </summary>
    private UserInfo EditedUser
    {
        get
        {
            return mEditedUser ?? (mEditedUser = FormUserInfo ?? UserInfo);
        }
    }


    /// <summary>
    /// Gets user picture control
    /// </summary>
    private CMSAdminControls_UI_UserPicture UserPicture
    {
        get
        {
            if (mUserPicture == null)
            {
                mUserPicture = (CMSAdminControls_UI_UserPicture)LoadControl("~/CMSAdminControls/UI/UserPicture.ascx");
            }
            return mUserPicture;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            btnDeleteImage.Enabled = value;
            uplFilePicture.Enabled = value;
            base.Enabled = value;
        }
    }


    /// <summary>
    /// Max picture width.
    /// </summary>
    public int MaxPictureWidth
    {
        get
        {
            return UserPicture.Width;
        }
    }


    /// <summary>
    /// Max picture height.
    /// </summary>
    public int MaxPictureHeight
    {
        get
        {
            return UserPicture.Height;
        }
    }


    /// <summary>
    /// Keep aspect ratio.
    /// </summary>
    public bool KeepAspectRatio
    {
        get
        {
            return UserPicture.KeepAspectRatio;
        }
        set
        {
            UserPicture.KeepAspectRatio = value;
        }
    }


    /// <summary>
    /// Max upload file/picture field width.
    /// </summary>
    public int FileUploadFieldWidth
    {
        get
        {
            return (int)uplFilePicture.Width.Value;
        }
        set
        {
            uplFilePicture.Width = Unit.Pixel(value);
        }
    }


    /// <summary>
    /// User information.
    /// </summary>
    public UserInfo UserInfo
    {
        get
        {
            return mUserInfo;
        }
        set
        {
            mUserInfo = value;
            if (mUserInfo != null)
            {
                if (mUserInfo.UserAvatarID != 0)
                {
                    ShowUserAvatar(mUserInfo);
                }
                else
                {
                    HideAvatar();
                }
            }
        }
    }


    /// <summary>
    /// Maximal side size.
    /// </summary>
    public int MaxSideSize
    {
        get
        {
            return mMaxSideSize > 0 ? mMaxSideSize : SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarMaxSideSize");
        }
        set
        {
            UserPicture.Width = value;
            UserPicture.Height = value;
            UserPicture.KeepAspectRatio = true;
            mMaxSideSize = value;
        }
    }


    /// <summary>
    /// Gets or sets value - AvatarID.
    /// </summary>
    public override object Value
    {
        get
        {
            // Return null instead of 0 to avoid FK violation
            return avatarID <= 0 ? null : (object)avatarID;
        }
        set
        {
            avatarID = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Gets value for validation purposes.
    /// Returns -1, if no file (avatar) is currently selected, but a new file has been submitted to the server, and that file has not been processed yet (or a predefined avatar has been chosen).
    /// Returns actual <see cref="Value"/> otherwise.
    /// </summary>
    public override object ValueForValidation
    {
        get
        {
            if (avatarID == 0)
            {
                // No avatar is currently selected, but might be after the UpdateAvatar() is called
                if (IsNewPictureUploaded() || !String.IsNullOrEmpty(hiddenAvatarGuid.Value))
                {
                    return -1;
                }
            }

            return Value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// OnInit handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // In basic form use special event to save data
        if (Form != null)
        {
            Form.OnBeforeDataRetrieval += Form_OnBeforeDataRetrieval;
        }

        plcUserPicture.Controls.Add(UserPicture);
    }


    /// <summary>
    /// OnBeforeDataRetrieval
    /// </summary>
    protected void Form_OnBeforeDataRetrieval(object sender, EventArgs e)
    {
        UpdateAvatar();
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Is valid override.
    /// </summary>
    public override bool IsValid()
    {
        if ((uplFilePicture.PostedFile != null) && (uplFilePicture.PostedFile.ContentLength > 0) && !ImageHelper.IsImage(Path.GetExtension(uplFilePicture.PostedFile.FileName)))
        {
            ErrorMessage = GetString("avat.filenotvalid");
            return false;
        }
        return true;
    }


    /// <summary>
    /// Updates given user's picture using values of this control.
    /// </summary>
    /// <param name="user">User info object.</param>
    public void UpdateUserPicture(UserInfo user)
    {
        AvatarInfo avatar = null;

        // Check if some avatar should be deleted
        if (ValidationHelper.GetBoolean(hiddenDeleteAvatar.Value, false))
        {
            DeleteOldUserPicture(user);
            hiddenDeleteAvatar.Value = String.Empty;
        }

        // If some file was uploaded
        if (IsNewPictureUploaded())
        {
            avatar = StoreUploadedPictureForUser(user);
        }
        // If predefined was chosen
        else if (!string.IsNullOrEmpty(hiddenAvatarGuid.Value))
        {
            // Delete old picture 
            DeleteOldUserPicture(user);

            // Get predefined avatar by GUID
            Guid guid = ValidationHelper.GetGuid(hiddenAvatarGuid.Value, Guid.NewGuid());
            avatar = AvatarInfoProvider.GetAvatarInfoWithoutBinary(guid);
            hiddenAvatarGuid.Value = String.Empty;
        }


        // Save user changes and show new avatar
        if (avatar != null)
        {
            avatarID = user.UserAvatarID = avatar.AvatarID;
        }

        UserInfo.Provider.Set(user);
        ShowUserAvatar(user);
    }


    /// <summary>
    /// Deletes picture of given user.
    /// </summary>
    /// <param name="user">User info object.</param>
    public static void DeleteOldUserPicture(UserInfo user)
    {
        // Delete old picture if needed
        if (user.UserAvatarID != 0)
        {
            // Delete avatar info if needed
            AvatarInfo avatar = AvatarInfoProvider.GetAvatarInfoWithoutBinary(user.UserAvatarID);
            if (avatar != null)
            {
                DeleteCustomAvatar(avatar);

                user.UserAvatarID = 0;
                UserInfo.Provider.Set(user);
                if (user.UserID == MembershipContext.AuthenticatedUser.UserID)
                {
                    MembershipContext.AuthenticatedUser.UserAvatarID = 0;
                }
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Setups all controls.
    /// </summary>
    private void SetupControls()
    {
        SetupControlsForAvatar();

        LoadData();
    }


    /// <summary>
    /// Setups controls for Avatar type of user picture.
    /// </summary>
    private void SetupControlsForAvatar()
    {
        // Setup delete image properties
        btnDeleteImage.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/delete.png");
        btnDeleteImage.OnClientClick = "return deleteAvatar('" + hiddenDeleteAvatar.ClientID + "', '" + hiddenAvatarGuid.ClientID + "', '" + pnlAvatarImage.ClientID + "' );";
        btnDeleteImage.ToolTip = GetString("general.delete");
        btnDeleteImage.AlternateText = btnDeleteImage.ToolTip;

        plcUploader.Visible = true;
        imgHelp.Visible = false;

        RegisterScripts();
    }


    /// <summary>
    /// Loads data into control and shows actual user picture.
    /// </summary>
    private void LoadData()
    {
        // User info from form has the highest priority.
        if (FormUserInfo != null)
        {
            avatarID = FormUserInfo.UserAvatarID;
        }

        if (avatarID > 0)
        {
            ShowAvatar(avatarID);
        }
        else if (EditedUser != null)
        {
            ShowUserAvatar(EditedUser);
        }

        // Changes must be preserved after postback
        if (RequestHelper.IsPostBack())
        {
            // Predefined avatar was selected by user so it has to be shown
            if (!String.IsNullOrEmpty(hiddenAvatarGuid.Value))
            {
                Guid guid = ValidationHelper.GetGuid(hiddenAvatarGuid.Value, Guid.NewGuid());
                AvatarInfo avatar = AvatarInfoProvider.GetAvatarInfoWithoutBinary(guid);
                if (avatar != null)
                {
                    ShowAvatar(avatar.AvatarID);
                }
            }
            else if (ValidationHelper.GetBoolean(hiddenDeleteAvatar.Value, false))
            {
                // Avatar has been removed by user
                HideAvatar();
            }
        }
    }


    /// <summary>
    /// Updates avatar represented by control's Value.
    /// </summary>
    private void UpdateAvatar()
    {
        // Try to get avatar info
        AvatarInfo avatar = null;
        if (avatarID > 0)
        {
            avatar = AvatarInfoProvider.GetAvatarInfoWithoutBinary(avatarID);
        }

        // If some new picture was uploaded
        if (IsNewPictureUploaded())
        {
            avatar = StoreUploadedPicture(avatarID, EditedUser);
            hiddenDeleteAvatar.Value = String.Empty;
        }
        // If some predefined avatar was selected
        else if (!string.IsNullOrEmpty(hiddenAvatarGuid.Value))
        {
            DeleteCustomAvatar(avatar);

            Guid guid = ValidationHelper.GetGuid(hiddenAvatarGuid.Value, Guid.NewGuid());
            avatar = AvatarInfoProvider.GetAvatarInfoWithoutBinary(guid);
            hiddenAvatarGuid.Value = String.Empty;
        }

        if (ValidationHelper.GetBoolean(hiddenDeleteAvatar.Value, false))
        {
            // Avatar has to be deleted
            DeleteCustomAvatar(avatar);
            hiddenDeleteAvatar.Value = String.Empty;
            avatar = null;
        }

        // Update avatar id
        avatarID = (avatar != null) ? avatar.AvatarID : 0;
        if (EditedUser != null)
        {
            // Save avatar type for user given by basic form
            EditedUser.UserAvatarID = avatarID;
            UserInfo.Provider.Set(EditedUser);

            ShowUserAvatar(EditedUser);
        }
        else if (avatarID > 0)
        {
            ShowAvatar(avatarID);
        }
        else
        {
            HideAvatar();
        }
    }


    /// <summary>
    /// Deletes given custom avatar. Does nothing when given avatar is not custom.
    /// </summary>
    /// <param name="avatar">Avatar to delete.</param>
    private static void DeleteCustomAvatar(AvatarInfo avatar)
    {
        // Delete avatar if some exists and is custom
        if (avatar != null)
        {
            AvatarInfoProvider.DeleteAvatarFile(avatar.AvatarGUID.ToString(), avatar.AvatarFileExtension, false, false);
            AvatarInfo.Provider.Delete(avatar);
        }
    }


    /// <summary>
    /// Stores uploaded picture in the system and assigns it to the given user.
    /// </summary>
    /// <param name="user">User info.</param>
    /// <returns>Stored avatar info.</returns>
    private AvatarInfo StoreUploadedPictureForUser(UserInfo user)
    {
        return StoreUploadedPicture(user.UserAvatarID, user);
    }


    /// <summary>
    /// Stores uploaded picture in the system. Old avatar info is changed if its identifier is given.
    /// </summary>
    /// <param name="avatarId">Old avatar identifier (avatar picture is changed only).</param>
    /// <param name="user">User's username is used for new avatar name.</param>
    /// <returns>Stored avatar info.</returns>
    private AvatarInfo StoreUploadedPicture(int avatarId, UserInfo user)
    {
        // Check if avatar exists and if so check if is custom
        AvatarInfo avatar = AvatarInfoProvider.GetAvatarInfoWithoutBinary(avatarId);
        if (avatar != null)
        {
            // Replace old custom avatar
            AvatarInfoProvider.UploadAvatar(avatar, uplFilePicture.PostedFile.ToUploadedFile(),
                                        SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarWidth"),
                                        SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarHeight"),
                                        SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarMaxSideSize"));
        }
        else
        {
            // Old avatar is not custom, so create new custom avatar
            avatar = CreateAvatar(user);
        }

        AvatarInfo.Provider.Set(avatar);
        return avatar;
    }


    /// <summary>
    /// Indicates whether new image is uploaded or not.
    /// </summary>
    /// <returns></returns>
    private bool IsNewPictureUploaded()
    {
        return ((uplFilePicture.PostedFile != null) && (uplFilePicture.PostedFile.ContentLength > 0) && ImageHelper.IsImage(Path.GetExtension(uplFilePicture.PostedFile.FileName)));
    }


    /// <summary>
    /// Creates a new avatar object.
    /// </summary>
    /// <param name="ui"> Username of given user is used for avatar name.</param>
    private AvatarInfo CreateAvatar(UserInfo ui)
    {
        AvatarInfo ai = new AvatarInfo(uplFilePicture.PostedFile.ToUploadedFile(),
                            SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarWidth"),
                            SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarHeight"),
                            SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarMaxSideSize"));

        if ((ui == null) && (MembershipContext.AuthenticatedUser != null) && AuthenticationHelper.IsAuthenticated() && PortalContext.ViewMode.IsLiveSite())
        {
            ui = MembershipContext.AuthenticatedUser;
        }

        ai.AvatarName = AvatarInfoProvider.GetUniqueAvatarName($"custom {ui.UserName}");

        ai.AvatarGUID = Guid.NewGuid();

        return ai;
    }


    /// <summary>
    /// Hides avatar.
    /// </summary>
    private void HideAvatar()
    {
        UserPicture.AvatarID = 0;
        UserPicture.UserID = 0;
        pnlAvatarImage.Visible = false;
        btnDeleteImage.Visible = false;
    }


    /// <summary>
    /// Shows avatar of given user.
    /// </summary>
    /// <param name="user">User info.</param>
    private void ShowUserAvatar(UserInfo user)
    {
        UserPicture.UserID = user.UserID;
        UserPicture.AvatarID = 0;
        pnlAvatarImage.Visible = (user.UserAvatarID > 0);
        btnDeleteImage.Visible = (user.UserAvatarID > 0);
    }


    /// <summary>
    /// Shows given avatar.
    /// </summary>
    /// <param name="avatarId">Avatar's identifier.</param>
    private void ShowAvatar(int avatarId)
    {
        UserPicture.UserID = 0;
        UserPicture.AvatarID = avatarId;
        pnlAvatarImage.Visible = true;
        btnDeleteImage.Visible = true;
    }


    /// <summary>
    /// Registers helper client scripts.
    /// </summary>
    private void RegisterScripts()
    {
        ltlScript.Text = ScriptHelper.GetScript("function UpdateForm(){ ; } \n ");

        // Get image size param(s) for preview
        string sizeParams = string.Empty;

        // Keep aspect ratio is set - property was set directly or indirectly by max side size property.  
        if (KeepAspectRatio)
        {
            sizeParams += "?maxsidesize=" + (MaxPictureWidth > MaxPictureHeight ? MaxPictureWidth : MaxPictureHeight);
        }
        else
        {
            sizeParams += "?width=" + MaxPictureWidth + "&height=" + MaxPictureHeight;
        }

        // JavaScript which creates selected image preview and saves image guid  to hidden field
        string getAvatarPath = ResolveUrl("~/getavatar/");

        StringBuilder sbScript = new StringBuilder();
        sbScript.Append(@"
function ", ClientID, @"updateHidden(guidPrefix, clientId)
{
    if (clientId == '", ClientID, @"')
    {
        avatarGuid = guidPrefix.substring(4);
        if (avatarGuid != '')
        {
            var hdnAvatarGuid = document.getElementById('", hiddenAvatarGuid.ClientID, @"');
            hdnAvatarGuid.value = avatarGuid;

            var div = document.getElementById('", pnlPreview.ClientID, @"');
            div.style.display = '';
            div.innerHTML = '<img src=""", getAvatarPath, @"' + avatarGuid + '/avatar", sizeParams, @""" />&#13;&#10;&nbsp;<img src=""", btnDeleteImage.ImageUrl, @""" border=""0"" onclick=""deleteImagePreview(\'", hiddenAvatarGuid.ClientID, @"\',\'", pnlPreview.ClientID, @"\')"" style=""cursor:pointer""/>';

            var placeholder = document.getElementById('", pnlAvatarImage.ClientID, @"');
            if ( placeholder != null)
            {
                placeholder.style.display='none';
            }
        } 
    }
}");
        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), ClientID + "updateHidden", ScriptHelper.GetScript(sbScript.ToString()));
        sbScript.Clear();

        sbScript.Append(@"
function deleteImagePreview(hiddenGuidId, divId)
{
    if(confirm(", ScriptHelper.GetString(GetString("myprofile.pictdeleteconfirm")), @"))
    {   
        var hdnAvatarGuid = document.getElementById(hiddenGuidId);
        hdnAvatarGuid.value = '';

        var div = document.getElementById(divId);
        div.style.display = 'none';
        div.innerHTML = '';
    }
}");
        // JavaScript which deletes image preview
        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "deleteImagePreviewScript", ScriptHelper.GetScript(sbScript.ToString()));
        sbScript.Clear();

        sbScript.Append(@"
function deleteAvatar(hiddenDeleteId, hiddenGuidId, placeholderId)
{
    if(confirm(", ScriptHelper.GetString(GetString("myprofile.pictdeleteconfirm")), @"))
    {
        var hdnDelete = document.getElementById(hiddenDeleteId);
        hdnDelete.value = 'true';
        
        var placeholder = document.getElementById(placeholderId);
        placeholder.style.display = 'none';
        
        var hdnAvatarGuid = document.getElementById(hiddenGuidId);
        hdnAvatarGuid.value = '' ;
    }
    return false;
}");
        // JavaScript which pseudo deletes avatar 
        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "deleteAvatar", ScriptHelper.GetScript(sbScript.ToString()));
    }

    #endregion
}