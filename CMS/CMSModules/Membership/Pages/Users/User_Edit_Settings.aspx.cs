using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Users_User_Edit_Settings : CMSUsersPage
{
    #region "Variables"

    private UserInfo userInfo;
    private bool error = false;

    #endregion


    #region "Public methods"
    /// <summary>
    /// Shows the specified error message, optionally with a tooltip text.
    /// </summary>
    /// <param name="text">Error message text</param>
    /// <param name="description">Additional description</param>
    /// <param name="tooltipText">Tooltip text</param>
    /// <param name="persistent">Indicates if the message is persistent</param>
    public override void ShowError(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        base.ShowError(text, description, tooltipText, persistent);
        error = true;
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Handles onInit, fill timezone dropdownlist.
    /// </summary>    
    protected override void OnInit(EventArgs e)
    {
        timeZone.ReloadData();
        base.OnInit(e);
    }


    /// <summary>
    /// Handles Page Load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        //Load labels
        lblActivatedByUser.Text = GetString("adm.user.lblActivatedByUser");
        lblActivationDate.Text = GetString("adm.user.lblActivationDate");
        lblCampaign.Text = GetString("adm.user.lblCampaign");
        lblNickName.Text = GetString("adm.user.lblNickName");
        lblRegInfo.Text = GetString("adm.user.lblRegInfo");
        lblTimeZone.Text = GetString("adm.user.lblTimeZone");
        lblURLReferrer.Text = GetString("adm.user.lblURLReferrer");
        lblUserPicture.Text = GetString("adm.user.lblUserPicture");
        lblUserSignature.Text = GetString("adm.user.lblUserSignature");
        lblWaitingForActivation.Text = GetString("adm.user.lblWaitingForActivation");

        lblUserGender.Text = GetString("adm.user.lblUserGender");
        lblUserDateOfBirth.Text = GetString("adm.user.lblUserDateOfBirth");
        lblPosition.Text = GetString("adm.user.lblUserPosition");
        lblUserSkype.Text = GetString("adm.user.lblUserSkype");
        lblUserIM.Text = GetString("adm.user.lblUserIM");
        lblUserPhone.Text = GetString("adm.user.lblUserPhone");

        if (!RequestHelper.IsPostBack())
        {
            rbtnlGender.Items.Add(new ListItem(GetString("general.unknown"), "0"));
            rbtnlGender.Items.Add(new ListItem(GetString("general.male"), "1"));
            rbtnlGender.Items.Add(new ListItem(GetString("general.female"), "2"));
        }

        // Get userid from query string
        int userId = QueryHelper.GetInteger("userID", 0);

        // Check that only global administrator can edit global administrator's accounts
        if (userId > 0)
        {
            userInfo = UserInfo.Provider.Get(userId);
            CheckUserAvaibleOnSite(userInfo);
            EditedObject = userInfo;

            if (!CheckGlobalAdminEdit(userInfo))
            {
                plcTable.Visible = false;
                ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));
            }
        }

        UserPictureFormControl.IsLiveSite = false;

        //Load user data
        LoadData();
    }


    /// <summary>
    /// Loads data of edited user from DB.
    /// </summary>
    protected void LoadData()
    {
        if (userInfo == null)
        {
            // User does not exist
            return;
        }

        // Load user picture, even for post-back
        SetUserPictureArea();

        if (RequestHelper.IsPostBack())
        {
            // Do not re-set static content on post-back
            return;
        }

        if ((userInfo.UserSettings != null) && (userInfo.UserSettings.UserActivatedByUserID > 0))
        {
            UserInfo user = UserInfo.Provider.Get(userInfo.UserSettings.UserActivatedByUserID);
            if (user != null)
            {
                lblUserFullName.Text = HTMLHelper.HTMLEncode(user.FullName);
            }
        }

        if (String.IsNullOrEmpty(lblUserFullName.Text))
        {
            lblUserFullName.Text = GetString("general.na");
        }

        activationDate.SelectedDateTime = userInfo.UserSettings.UserActivationDate;
        txtCampaign.Text = userInfo.UserCampaign;
        txtNickName.Text = userInfo.UserNickName;
        LoadRegInfo(userInfo.UserSettings);
        timeZone.Value = userInfo.UserSettings.UserTimeZoneID;
        txtURLReferrer.Text = userInfo.UserURLReferrer;
        txtUserSignature.Text = userInfo.UserSignature;
        txtUserDescription.Text = userInfo.UserSettings.UserDescription;
        chkWaitingForActivation.Checked = userInfo.UserSettings.UserWaitingForApproval;
        chkLogActivities.Checked = userInfo.UserSettings.UserLogActivities;
        chkUserShowIntroTile.Checked = userInfo.UserSettings.UserShowIntroductionTile;
        rbtnlGender.SelectedValue = userInfo.UserSettings.UserGender.ToString();
        dtUserDateOfBirth.SelectedDateTime = userInfo.UserSettings.UserDateOfBirth;
        txtUserSkype.Text = userInfo.UserSettings.UserSkype;
        txtUserIM.Text = userInfo.UserSettings.UserIM;
        txtPhone.Text = userInfo.UserSettings.UserPhone;
        txtPosition.Text = userInfo.UserSettings.UserPosition;
    }


    /// <summary>
    /// Displays user's registration information.
    /// </summary>
    protected void LoadRegInfo(UserSettingsInfo settings)
    {
        if ((settings.UserRegistrationInfo != null) && (settings.UserRegistrationInfo.ColumnNames != null) && (settings.UserRegistrationInfo.ColumnNames.Count > 0))
        {
            foreach (string column in settings.UserRegistrationInfo.ColumnNames)
            {
                Panel grp = new Panel
                {
                    CssClass = "control-group-inline"
                };
                plcUserLastLogonInfo.Controls.Add(grp);
                Label lbl = new Label();
                grp.Controls.Add(lbl);
                lbl.Text = HTMLHelper.HTMLEncode(TextHelper.LimitLength((string)settings.UserRegistrationInfo[column], 80, "..."));
                lbl.ToolTip = HTMLHelper.HTMLEncode(column + " - " + (string)settings.UserRegistrationInfo[column]);
            }
        }
        else
        {
            plcUserLastLogonInfo.Controls.Add(new LocalizedLabel
            {
                ResourceString = "general.na",
                CssClass = "form-control-text"
            });
        }
    }


    /// <summary>
    /// Saves data of edited user from TextBoxes into DB.
    /// </summary>
    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        if (!UserPictureFormControl.IsValid())
        {
            ShowError(UserPictureFormControl.ErrorMessage);
            return;
        }

        if (!dtUserDateOfBirth.IsValidRange() || !activationDate.IsValidRange())
        {
            ShowError(GetString("general.errorinvaliddatetimerange"));
            return;
        }
        
        // Clean from empty strings
        txtNickName.Text = txtNickName.Text.Trim();
        txtUserSignature.Text = txtUserSignature.Text.Trim();
        txtUserDescription.Text = txtUserDescription.Text.Trim();
        
        if (userInfo == null)
        {
            // only update valid user info
            return;
        }

        userInfo.UserSettings.UserActivationDate = activationDate.SelectedDateTime;
        userInfo.UserCampaign = txtCampaign.Text;
        userInfo.UserNickName = txtNickName.Text;
        
        userInfo.UserSettings.UserTimeZoneID = ValidationHelper.GetInteger(timeZone.Value, 0);
        userInfo.UserURLReferrer = txtURLReferrer.Text;
        userInfo.UserSignature = txtUserSignature.Text;
        userInfo.UserSettings.UserDescription = txtUserDescription.Text;

        userInfo.UserSettings.UserWaitingForApproval = chkWaitingForActivation.Checked;
        userInfo.UserSettings.UserLogActivities = chkLogActivities.Checked;

        userInfo.UserSettings.UserShowIntroductionTile = chkUserShowIntroTile.Checked;

        userInfo.UserSettings.UserGender = ValidationHelper.GetInteger(rbtnlGender.SelectedValue, 0);
        userInfo.UserSettings.UserDateOfBirth = dtUserDateOfBirth.SelectedDateTime;

        userInfo.UserSettings.UserPosition =  txtPosition.Text;
        userInfo.UserSettings.UserSkype = txtUserSkype.Text;
        userInfo.UserSettings.UserIM = txtUserIM.Text;
        userInfo.UserSettings.UserPhone = txtPhone.Text;

        // Set user picture to DB
        UserPictureFormControl.UpdateUserPicture(userInfo);
        UserInfo.Provider.Set(userInfo);

        if (error)
        {
            // Display info label only if no error occurred
            return;
        }

        SetUserPictureArea();
        ShowChangesSaved();
    }

    #endregion


    #region "Private methods"
       
    /// <summary>
    /// Sets user picture control.
    /// </summary>    
    private void SetUserPictureArea()
    {
        UserPictureFormControl.UserInfo = userInfo;
        UserPictureFormControl.MaxSideSize = 100;
    }

    #endregion
}