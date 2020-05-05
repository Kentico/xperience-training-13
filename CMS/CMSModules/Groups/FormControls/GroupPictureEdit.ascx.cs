using System;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.UploadExtensions;
using CMS.Base.Web.UI;
using CMS.Community;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSModules_Groups_FormControls_GroupPictureEdit : FormEngineUserControl
{
    #region "Variables and private properties"

    private SiteInfo site;
    private GroupInfo mGroupInfo;
    private int mMaxSideSize = 100;
    private int avatarID;
    public string divId = string.Empty;
    private CMSAdminControls_UI_UserPicture mGroupPicture;


    /// <summary>
    /// Group picture control
    /// </summary>
    private CMSAdminControls_UI_UserPicture GroupPicture
    {
        get
        {
            if (mGroupPicture == null)
            {
                mGroupPicture = (CMSAdminControls_UI_UserPicture)LoadControl("~/CMSAdminControls/UI/UserPicture.ascx");
            }
            return mGroupPicture;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        set
        {
            btnDeleteImage.Enabled = value;
            uplFilePicture.Enabled = value;
            base.Enabled = value;
        }
        get
        {
            return base.Enabled;
        }
    }


    /// <summary>
    /// Max picture width.
    /// </summary>
    public int MaxPictureWidth
    {
        get
        {
            return GroupPicture.Width;
        }
        set
        {
            GroupPicture.Width = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Max picture height.
    /// </summary>
    public int MaxPictureHeight
    {
        get
        {
            return GroupPicture.Height;
        }
        set
        {
            GroupPicture.Height = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Keep aspect ratio.
    /// </summary>
    public bool KeepAspectRatio
    {
        get
        {
            return GroupPicture.KeepAspectRatio;
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
    /// Group information.
    /// </summary>
    public GroupInfo GroupInfo
    {
        get
        {
            return mGroupInfo;
        }
        set
        {
            mGroupInfo = value;
            if (mGroupInfo != null)
            {
                plcImageActions.Visible = mGroupInfo.GroupAvatarID != 0;
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
            return mMaxSideSize;
        }
        set
        {
            GroupPicture.Width = value;
            GroupPicture.Height = value;
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
            return avatarID;
        }
        set
        {
            avatarID = ValidationHelper.GetInteger(value, 0);
            if (GroupInfo != null)
            {
                GroupInfo.GroupAvatarID = avatarID;
            }
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        plcGroupPicture.Controls.Add(GroupPicture);

        if (Form != null)
        {
            Form.OnBeforeDataRetrieval += Form_OnBeforeDataRetrieval;
        }
    }


    /// <summary>
    /// OnBeforeDataRetrieval event handler.
    /// </summary>
    protected void Form_OnBeforeDataRetrieval(object sender, EventArgs e)
    {
        UpdateGroupPicture(null);
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


    /// <summary>
    /// Setups controls.
    /// </summary>
    private void SetupControls()
    {
        // Setup site info        
        site = SiteContext.CurrentSite;

        // Setup delete image properties
        btnDeleteImage.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/delete.png");
        btnDeleteImage.OnClientClick = "return deleteAvatar('" + hiddenDeleteAvatar.ClientID + "', '" + hiddenAvatarGuid.ClientID + "', '" + plcImageActions.ClientID + "' );";
        btnDeleteImage.AlternateText = GetString("general.delete");

        // Setup show gallery button
        btnShowGallery.Text = GetString("avat.selector.select");
        btnShowGallery.Visible = SettingsKeyInfoProvider.GetBoolValue(site.SiteName + ".CMSEnableDefaultAvatars");

        RegisterScripts();

        // Try to load avatar 
        if (avatarID > 0)
        {
            plcImageActions.Visible = true;
            GroupPicture.AvatarID = avatarID;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Registers helper scripts.
    /// </summary>
    private void RegisterScripts()
    {

        ltlScript.Text = ScriptHelper.GetScript("function UpdateForm(){ ; } \n");


        // Get image size param(s) for preview
        string sizeParams = string.Empty;
        // Keep aspect ratio is set - property was set directly or indirectly by max side size property.  
        if (KeepAspectRatio)
        {
            sizeParams += "&maxsidesize=" + (MaxPictureWidth > MaxPictureHeight ? MaxPictureWidth : MaxPictureHeight);
        }
        else
        {
            sizeParams += "&width=" + MaxPictureWidth + "&height=" + MaxPictureHeight;
        }

        string getAvatarPath = ResolveUrl("~/CMSPages/GetAvatar.aspx");
        // Create id for div with selected image preview
        divId = ClientID + "imgDiv";

        // Javascript which creates selected image preview and saves image guid  to hidden field
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append(@"
function ", ClientID, @"updateHidden(guidPrefix, clientId)
{
    if (clientId == '", ClientID, @"')
    {
        avatarGuid = guidPrefix.substring(4);
        if (avatarGuid != '')
        {
            hidden = document.getElementById('", hiddenAvatarGuid.ClientID, @"');
            hidden.value = avatarGuid ;
            div = document.getElementById('", divId, @"');
            div.style.display='';
            div.innerHTML = '<img src=""", getAvatarPath, @"?avatarguid=' + avatarGuid + '", sizeParams, @""" />&#13;&#10;&nbsp;<img src=""", btnDeleteImage.ImageUrl, @""" border=""0"" onclick=""deleteImagePreview(\'", hiddenAvatarGuid.ClientID, @"\',\'", divId, @"\')"" style=""cursor:pointer""/>';
            placeholder = document.getElementById('", plcImageActions.ClientID, @"');
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
function deleteImagePreview(hiddenId, divId)
{
    if(confirm(", ScriptHelper.GetString(GetString("myprofile.pictdeleteconfirm")), @"))
    {   
        hidden = document.getElementById(hiddenId);
        hidden.value = '' ;
        div = document.getElementById(divId);
        div.style.display='none';
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
        hidden = document.getElementById(hiddenDeleteId);
        hidden.value = 'true' ;
        placeholder = document.getElementById(placeholderId);
        placeholder.style.display='none';
        hidden = document.getElementById(hiddenGuidId);
        hidden.value = '' ;
    }
    return false;
}");
        // JavaScript which pseudo deletes avatar 
        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "deleteAvatar", ScriptHelper.GetScript(sbScript.ToString()));
    }


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
    /// Updates picture of current group.
    /// </summary>
    /// <param name="gi">Group info object</param>
    public void UpdateGroupPicture(GroupInfo gi)
    {
        AvatarInfo ai = null;

        if (gi != null)
        {
            // Delete avatar if needed
            if (ValidationHelper.GetBoolean(hiddenDeleteAvatar.Value, false))
            {
                DeleteOldGroupPicture(gi);
            }

            // If some file was uploaded
            if ((uplFilePicture.PostedFile != null) && (uplFilePicture.PostedFile.ContentLength > 0) && ImageHelper.IsImage(Path.GetExtension(uplFilePicture.PostedFile.FileName)))
            {
                // Check if this group has some avatar and if so check if is custom
                ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(gi.GroupAvatarID);
                if (ai != null)
                {
                    ReplaceExistingAvatar(ai);
                }
                else
                {
                    // Delete old picture
                    DeleteOldGroupPicture(gi);
                    ai = CreateNewAvatar();
                }

                AvatarInfo.Provider.Set(ai);

                // Update group info
                gi.GroupAvatarID = ai.AvatarID;
                GroupInfoProvider.SetGroupInfo(gi);


                plcImageActions.Visible = true;
            }
            // If predefined was chosen
            else if (!string.IsNullOrEmpty(hiddenAvatarGuid.Value))
            {
                // Delete old picture 
                DeleteOldGroupPicture(gi);

                Guid guid = ValidationHelper.GetGuid(hiddenAvatarGuid.Value, Guid.NewGuid());
                ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(guid);

                // Update group info
                if (ai != null)
                {
                    gi.GroupAvatarID = ai.AvatarID;
                    GroupInfoProvider.SetGroupInfo(gi);
                }

                plcImageActions.Visible = true;
            }
            else
            {
                plcImageActions.Visible = false;
            }
        }
        else
        {
            bool pseudoDelete = ValidationHelper.GetBoolean(hiddenDeleteAvatar.Value, false);
            // Try to get avatar info
            if (avatarID != 0)
            {
                ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(avatarID);
            }

            // If some new picture was uploaded
            if ((uplFilePicture.PostedFile != null) && (uplFilePicture.PostedFile.ContentLength > 0) && ImageHelper.IsImage(Path.GetExtension(uplFilePicture.PostedFile.FileName)))
            {
                // Change delete to false because file will be replaced
                pseudoDelete = false;

                // If some avatar exists and is custom
                if (ai != null)
                {
                    // Delete file and upload new
                    AvatarInfoProvider.DeleteAvatarFile(ai.AvatarGUID.ToString(), ai.AvatarFileExtension, false, false);
                    ReplaceExistingAvatar(ai);
                }
                else
                {
                    ai = CreateNewAvatar();
                }

                // Update database
                AvatarInfo.Provider.Set(ai);
            }
            // If some predefined avatar was selected
            else if (!string.IsNullOrEmpty(hiddenAvatarGuid.Value))
            {
                // Change delete to false because file will be replaced
                pseudoDelete = false;

                // If some avatar exists and is custom
                if (ai != null)
                {
                    AvatarInfoProvider.DeleteAvatarFile(ai.AvatarGUID.ToString(), ai.AvatarFileExtension, false, false);
                    AvatarInfo.Provider.Delete(ai);
                }

                Guid guid = ValidationHelper.GetGuid(hiddenAvatarGuid.Value, Guid.NewGuid());
                ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(guid);
            }

            // If file was deleted - not replaced
            if (pseudoDelete)
            {
                // Delete it
                ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(avatarID);
                if (ai != null)
                {
                    AvatarInfo.Provider.Delete(ai);
                }

                ai = null;
                avatarID = 0;
                plcImageActions.Visible = false;
                GroupPicture.AvatarID = 0;
            }

            // Update avatar id
            if (ai != null)
            {
                GroupPicture.AvatarID = avatarID = ai.AvatarID;
                plcImageActions.Visible = true;
            }
        }
    }


    /// <summary>
    /// Creates new avatar object
    /// </summary>
    private AvatarInfo CreateNewAvatar()
    {
        AvatarInfo ai = new AvatarInfo(uplFilePicture.PostedFile.ToUploadedFile(),
                             SettingsKeyInfoProvider.GetIntValue(site.SiteName + ".CMSGroupAvatarWidth"),
                             SettingsKeyInfoProvider.GetIntValue(site.SiteName + ".CMSGroupAvatarHeight"),
                             SettingsKeyInfoProvider.GetIntValue(site.SiteName + ".CMSGroupAvatarMaxSideSize"));

        if (CommunityContext.CurrentGroup != null)
        {
            ai.AvatarName = AvatarInfoProvider.GetUniqueAvatarName(GetString("avat.custom") + " " + CommunityContext.CurrentGroup.GroupName);
        }
        else
        {
            ai.AvatarName = AvatarInfoProvider.GetUniqueAvatarName(ai.AvatarFileName.Substring(0, ai.AvatarFileName.LastIndexOfCSafe(".")));
        }

        ai.AvatarGUID = Guid.NewGuid();

        return ai;
    }


    /// <summary>
    /// Replaces existing avatar by the new one
    /// </summary>
    /// <param name="ai">Existing avatar info</param>
    private void ReplaceExistingAvatar(AvatarInfo ai)
    {
        AvatarInfoProvider.UploadAvatar(ai, uplFilePicture.PostedFile.ToUploadedFile(),
                                        SettingsKeyInfoProvider.GetIntValue(site.SiteName + ".CMSGroupAvatarWidth"),
                                        SettingsKeyInfoProvider.GetIntValue(site.SiteName + ".CMSGroupAvatarHeight"),
                                        SettingsKeyInfoProvider.GetIntValue(site.SiteName + ".CMSGroupAvatarMaxSideSize"));
    }


    /// <summary>
    /// Deletes group picture.
    /// </summary>
    /// <param name="gi">GroupInfo</param>
    public static void DeleteOldGroupPicture(GroupInfo gi)
    {
        // Delete old picture if needed
        if (gi.GroupAvatarID != 0)
        {
            // Delete avatar info provider if needed
            AvatarInfo ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(gi.GroupAvatarID);
            if (ai != null)
            {
                AvatarInfo.Provider.Delete(ai);
                AvatarInfoProvider.DeleteAvatarFile(ai.AvatarGUID.ToString(), ai.AvatarFileExtension, false, false);

                gi.GroupAvatarID = 0;
                GroupInfoProvider.SetGroupInfo(gi);
            }
        }
    }

    #endregion
}