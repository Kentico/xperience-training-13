using System;

using CMS.Activities;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Forums;
using CMS.Helpers;
using CMS.OutputFilter;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Groups_GroupEdit : CMSAdminEditControl
{
    #region "Variables"

    private int mGroupId = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the group to edit.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupId;
        }
        set
        {
            mGroupId = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Visible)
        {
            EnableViewState = false;
        }

        // Hide code name edit for simple mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            plcCodeName.Visible = false;
            plcUseHtml.Visible = false;
        }

        txtGroupDisplayName.IsLiveSite = IsLiveSite;
        txtGroupDescription.IsLiveSite = IsLiveSite;
        txtOptInURL.IsLiveSite = IsLiveSite;

        // Control initializations				
        rfvGroupDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvGroupName.ErrorMessage = GetString("general.requirescodename");

        // Show on-line marketing settings
        string siteName = SiteContext.CurrentSiteName;        

        chkEnableOptIn.NotSetChoice.Text = chkSendOptInConfirmation.NotSetChoice.Text = GetString("general.sitesettings") + " (##DEFAULT##)";
        chkEnableOptIn.SetDefaultValue(ForumGroupInfoProvider.EnableDoubleOptIn(siteName));
        chkSendOptInConfirmation.SetDefaultValue(ForumGroupInfoProvider.SendOptInConfirmation(siteName));

        // Fill editing form
        if (!IsLiveSite && !RequestHelper.IsPostBack())
        {
            ReloadData();
        }

        // Show/hide URL textboxes
        plcBaseAndUnsubUrl.Visible = (DisplayMode != ControlDisplayModeEnum.Simple);

        SetUrl();
    }


    /// <summary>
    /// Sets base and unsubscription URLs.
    /// </summary>
    private void SetUrl()
    {
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CheckBoxes", ScriptHelper.GetScript(@"
                    function check(txtId,chk,inhV)  
                    {
                        txt = document.getElementById(txtId);
                        if ((txt != null)&&(chk != null))
                        {
                            if (chk.checked)
                            {
                                txt.disabled = 'disabled';
                                txt.value = inhV;
                            }
                            else
                            {
                                txt.disabled = '';
                            }
                        }
                    }
                   "));


        // Force output filter to not resolve URLs in textboxes
        OutputFilterContext.CanResolveAllUrls = false;

        if (DisplayMode != ControlDisplayModeEnum.Simple)
        {
            // Get base and unsubscription URL from settings
            string baseUrl = ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSForumBaseUrl"), "");
            string unsubscriptionUrl = ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSForumUnsubscriptionUrl"), "");

            // Set values for base URL
            if (chkInheritBaseUrl.Checked)
            {
                txtForumBaseUrl.Text = baseUrl;
                txtForumBaseUrl.Enabled = false;
            }
            else
            {
                txtForumBaseUrl.Enabled = true;
            }

            // Set values for unsubscription URL
            if (chkInheritUnsubUrl.Checked)
            {
                txtUnsubscriptionUrl.Text = unsubscriptionUrl;
                txtUnsubscriptionUrl.Enabled = false;
            }
            else
            {
                txtUnsubscriptionUrl.Enabled = true;
            }

            chkInheritBaseUrl.Attributes.Add("onclick", "check('" + txtForumBaseUrl.ClientID + "', this,'" + baseUrl + "')");
            chkInheritUnsubUrl.Attributes.Add("onclick", "check('" + txtUnsubscriptionUrl.ClientID + "', this,'" + unsubscriptionUrl + "')");
        }

        string optInApprovalUrl = ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSForumOptInApprovalPath"), "");

        // Set values for double opt-in approval URL
        if (chkInheritOptInURL.Checked)
        {
            txtOptInURL.Text = optInApprovalUrl;
            txtOptInURL.Enabled = false;
        }
        else
        {
            txtOptInURL.Enabled = true;
        }

        chkInheritOptInURL.Attributes.Add("onclick", "check('" + txtOptInURL.PathTextBox.ClientID + "', this,'" + optInApprovalUrl + "');ChangeState_" + txtOptInURL.ClientID + "(!this.checked);");
    }


    /// <summary>
    /// Reloads the data in the form.
    /// </summary>
    public override void ReloadData()
    {
        ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(GroupID);
        if (fgi != null)
        {
            txtGroupDescription.Text = fgi.GroupDescription;
            txtGroupDisplayName.Text = fgi.GroupDisplayName;
            txtGroupName.Text = fgi.GroupName;
            chkCaptcha.Checked = fgi.GroupUseCAPTCHA;
            chkForumDisplayEmails.Checked = fgi.GroupDisplayEmails;
            chkForumRequireEmail.Checked = fgi.GroupRequireEmail;
            chkUseHTML.Checked = fgi.GroupHTMLEditor;
            txtMaxAttachmentSize.Text = fgi.GroupAttachmentMaxFileSize.ToString();

            // Show/hide URL textboxes
            plcBaseAndUnsubUrl.Visible = (DisplayMode != ControlDisplayModeEnum.Simple);
            if (DisplayMode != ControlDisplayModeEnum.Simple)
            {
                txtForumBaseUrl.Text = fgi.GroupBaseUrl;
                txtUnsubscriptionUrl.Text = fgi.GroupUnsubscriptionUrl;

                chkInheritBaseUrl.Checked = (fgi.GetValue("GroupBaseUrl") == null);
                chkInheritUnsubUrl.Checked = (fgi.GetValue("GroupUnsubscriptionUrl") == null);

                txtForumBaseUrl.Enabled = !chkInheritBaseUrl.Checked;
                txtUnsubscriptionUrl.Enabled = !chkInheritUnsubUrl.Checked;
            }

            // Settings
            chkAuthorEdit.Checked = fgi.GroupAuthorEdit;
            chkAuthorDelete.Checked = fgi.GroupAuthorDelete;
            txtImageMaxSideSize.Text = fgi.GroupImageMaxSideSize.ToString();
            txtIsAnswerLimit.Text = fgi.GroupIsAnswerLimit.ToString();
            radTypeChoose.Checked = fgi.GroupType == 0;
            radTypeDiscussion.Checked = fgi.GroupType == 1;
            radTypeAnswer.Checked = fgi.GroupType == 2;

            // Discussion
            chkEnableQuote.Checked = fgi.GroupEnableQuote;
            chkEnableBold.Checked = fgi.GroupEnableFontBold;
            chkEnableItalic.Checked = fgi.GroupEnableFontItalics;
            chkEnableUnderline.Checked = fgi.GroupEnableFontUnderline;
            chkEnableStrike.Checked = fgi.GroupEnableFontStrike;
            chkEnableCode.Checked = fgi.GroupEnableCodeSnippet;
            chkEnableColor.Checked = fgi.GroupEnableFontColor;
            radImageAdvanced.Checked = fgi.GroupEnableAdvancedImage;
            radImageSimple.Checked = fgi.GroupEnableImage;
            radImageNo.Checked = !(fgi.GroupEnableImage || fgi.GroupEnableAdvancedImage);
            radUrlAdvanced.Checked = fgi.GroupEnableAdvancedURL;
            radUrlSimple.Checked = fgi.GroupEnableURL;
            radUrlNo.Checked = !(fgi.GroupEnableURL || fgi.GroupEnableAdvancedURL);
            chkLogActivity.Checked = fgi.GroupLogActivity;

            // Double opt-in
            chkEnableOptIn.InitFromThreeStateValue(fgi, "GroupEnableOptIn");
            chkSendOptInConfirmation.InitFromThreeStateValue(fgi, "GroupSendOptInConfirmation");
            txtOptInURL.Text = fgi.GroupOptInApprovalURL;
            chkInheritOptInURL.Checked = (fgi.GetValue("GroupOptInApprovalURL") == null);
            txtOptInURL.Enabled = !chkInheritOptInURL.Checked;

            SetUrl();
        }
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        string errorMessage = new Validator().NotEmpty(txtGroupDisplayName.Text, GetString("general.requiresdisplayname")).Result;

        if ((errorMessage == String.Empty) && (DisplayMode != ControlDisplayModeEnum.Simple))
        {
            errorMessage = new Validator().NotEmpty(txtGroupName.Text, GetString("general.requirescodename"))
                .IsCodeName(txtGroupName.Text.Trim(), GetString("general.errorcodenameinidentifierformat")).Result;
        }

        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        int communityGroupId = 0;

        ForumGroupInfo forumGroupObj = ForumGroupInfoProvider.GetForumGroupInfo(GroupID);
        if (forumGroupObj != null)
        {
            communityGroupId = forumGroupObj.GroupGroupID;
        }

        forumGroupObj = ForumGroupInfoProvider.GetForumGroupInfo(txtGroupName.Text.Trim(), SiteContext.CurrentSiteID, communityGroupId);
        if ((forumGroupObj == null) || (forumGroupObj.GroupID == mGroupId))
        {
            if (forumGroupObj == null)
            {
                forumGroupObj = ForumGroupInfoProvider.GetForumGroupInfo(GroupID);
            }

            if (forumGroupObj != null)
            {
                if (txtGroupDisplayName.Text.Trim() != forumGroupObj.GroupDisplayName)
                {
                    // Refresh a breadcrumb if used in the tabs layout
                    ScriptHelper.RefreshTabHeader(Page, txtGroupDisplayName.Text);
                }

                forumGroupObj.GroupDescription = txtGroupDescription.Text.Trim();
                forumGroupObj.GroupDisplayName = txtGroupDisplayName.Text.Trim();

                forumGroupObj.GroupUseCAPTCHA = chkCaptcha.Checked;
                forumGroupObj.GroupDisplayEmails = chkForumDisplayEmails.Checked;
                forumGroupObj.GroupRequireEmail = chkForumRequireEmail.Checked;

                if (DisplayMode != ControlDisplayModeEnum.Simple)
                {
                    forumGroupObj.GroupUnsubscriptionUrl = chkInheritUnsubUrl.Checked ? null : txtUnsubscriptionUrl.Text.Trim();
                    forumGroupObj.GroupBaseUrl = chkInheritBaseUrl.Checked ? null : txtForumBaseUrl.Text.Trim();
                    forumGroupObj.GroupName = txtGroupName.Text.Trim();
                    forumGroupObj.GroupHTMLEditor = chkUseHTML.Checked;
                }

                // Settings
                forumGroupObj.GroupAuthorEdit = chkAuthorEdit.Checked;
                forumGroupObj.GroupAuthorDelete = chkAuthorDelete.Checked;
                forumGroupObj.GroupImageMaxSideSize = ValidationHelper.GetInteger(txtImageMaxSideSize.Text, 400);
                forumGroupObj.GroupIsAnswerLimit = ValidationHelper.GetInteger(txtIsAnswerLimit.Text, 5);
                forumGroupObj.GroupType = (radTypeChoose.Checked ? 0 : (radTypeDiscussion.Checked ? 1 : 2));
                forumGroupObj.GroupAttachmentMaxFileSize = ValidationHelper.GetInteger(txtMaxAttachmentSize.Text, 0);

                // Discussion
                forumGroupObj.GroupEnableQuote = chkEnableQuote.Checked;
                forumGroupObj.GroupEnableFontBold = chkEnableBold.Checked;
                forumGroupObj.GroupEnableFontItalics = chkEnableItalic.Checked;
                forumGroupObj.GroupEnableFontUnderline = chkEnableUnderline.Checked;
                forumGroupObj.GroupEnableFontStrike = chkEnableStrike.Checked;
                forumGroupObj.GroupEnableCodeSnippet = chkEnableCode.Checked;
                forumGroupObj.GroupEnableFontColor = chkEnableColor.Checked;
                forumGroupObj.GroupEnableAdvancedImage = radImageAdvanced.Checked;
                forumGroupObj.GroupEnableAdvancedURL = radUrlAdvanced.Checked;
                forumGroupObj.GroupEnableImage = radImageSimple.Checked;
                forumGroupObj.GroupEnableURL = radUrlSimple.Checked;

                // Double opt-in
                forumGroupObj.GroupOptInApprovalURL = chkInheritOptInURL.Checked ? null : txtOptInURL.Text.Trim();
                chkEnableOptIn.SetThreeStateValue(forumGroupObj, "GroupEnableOptIn");
                chkSendOptInConfirmation.SetThreeStateValue(forumGroupObj, "GroupSendOptInConfirmation");

                if (plcOnline.Visible)
                {
                    forumGroupObj.GroupLogActivity = chkLogActivity.Checked;
                }

                ForumGroupInfoProvider.SetForumGroupInfo(forumGroupObj);

                ShowChangesSaved();

                // Load form with data
                ReloadData();

                RaiseOnSaved();
            }
            else
            {
                ShowError( GetString("Group_General.GroupNotFound"));
            }
        }
        else
        {
            ShowError(GetString("Group_General.GroupAlreadyExists"));
        }
    }
}
