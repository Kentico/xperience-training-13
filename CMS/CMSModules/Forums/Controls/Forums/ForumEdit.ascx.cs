using System;

using CMS.Activities;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Forums_ForumEdit : CMSAdminEditControl
{
    #region "Constants"

    private const string DEFAULT_SUFFIX = " (##DEFAULT##)";

    #endregion


    #region "Variables"

    protected int mForumId = 0;
    protected int groupId = 0;
    protected string script = "";

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
    /// Gets or sets the ID of the forum to edit.
    /// </summary>
    public int ForumID
    {
        get
        {
            return mForumId;
        }
        set
        {
            mForumId = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Visible)
        {
            EnableViewState = false;
        }
        
        txtForumDisplayName.IsLiveSite = IsLiveSite;
        txtForumDescription.IsLiveSite = IsLiveSite;
        txtOptInURL.IsLiveSite = IsLiveSite;

        // Hide code name in simple mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            plcCodeName.Visible = false;
            plcUseHtml.Visible = false;
        }

        // Control initializations
        rfvForumDisplayName.ErrorMessage = GetString("Forum_General.EmptyDisplayName");
        rfvForumName.ErrorMessage = GetString("Forum_General.EmptyCodeName");

        // Get strings for labels
        lblForumOpen.Text = GetString("Forum_Edit.ForumOpenLabel");
        lblForumLocked.Text = GetString("Forum_Edit.ForumLockedLabel");
        lblForumDisplayEmails.Text = GetString("Forum_Edit.ForumDisplayEmailsLabel");
        lblForumRequireEmail.Text = GetString("Forum_Edit.ForumRequireEmailLabel");
        lblForumDisplayName.Text = GetString("Forum_Edit.ForumDisplayNameLabel");
        lblForumName.Text = GetString("Forum_Edit.ForumNameLabel");
        lblUseHTML.Text = GetString("Forum_Edit.UseHtml");
        lblBaseUrl.Text = GetString("Forum_Edit.lblBaseUrl");
        lblCaptcha.Text = GetString("Forum_Edit.useCaptcha");
        lblUnsubscriptionUrl.Text = GetString("Forum_Edit.lblUnsubscriptionUrl");

        chkInheritBaseUrl.Text = GetString("Forum_Edit.InheritBaseUrl");
        chkInheritUnsubscribeUrl.Text = GetString("Forum_Edit.InheritUnsupscriptionUrl");

        chkEnableOptIn.NotSetChoice.Text = chkSendOptInConfirmation.NotSetChoice.Text =
        chkForumRequireEmail.NotSetChoice.Text = chkForumDisplayEmails.NotSetChoice.Text =
        chkUseHTML.NotSetChoice.Text = chkCaptcha.NotSetChoice.Text = chkAuthorEdit.NotSetChoice.Text =
        chkAuthorDelete.NotSetChoice.Text = GetString("forum.settings.inheritfromgroup") + DEFAULT_SUFFIX;

        // Create scripts 
        script = @"
                function LoadOption(clientId, value)
                {
                    var obj = document.getElementById(clientId);
                    if(obj!=null)
                    {
                        obj.checked = value;
                    }
                }                

                function LoadSetting(clientId, value, enabled, type)
                {
                    SetInheritance(clientId, value, type);
                    var obj = document.getElementById(clientId);
                    if (obj != null) {
                        obj.disabled = enabled;
                    }
                }

                function SetInheritance(clientIds, values, type)
                {
                    var idArray = clientIds.split(';');
                    var valueArray = values.toString().split(';');

                    for(var i = 0;i<idArray.length;i++)
                    {
                        var clientId = idArray[i];
                        var value = valueArray[i];
                        var obj = document.getElementById(clientId);
                        if (obj != null) {
                            obj.disabled = !obj.disabled;
                            if (obj.disabled)
                            {
                                if (type == 'txt') {
                                    obj.value = value;
                                } else {
                                    obj.checked = (value == 'true');
                                }
                            }
                        }
                    }
                }
                ";

        ltrScript.Text += ScriptHelper.GetScript(script);
        script = "";

        // Load object info from database
        ForumInfo forumObj = ForumInfoProvider.GetForumInfo(mForumId);
        if (forumObj != null)
        {
            groupId = forumObj.ForumGroupID;

            if (!IsLiveSite && !RequestHelper.IsPostBack())
            {
                ReloadData(forumObj);
            }
            else
            {
                // Handle base and unsubscription URLs 
                txtBaseUrl.Enabled = !chkInheritBaseUrl.Checked;
                txtUnsubscriptionUrl.Enabled = !chkInheritUnsubscribeUrl.Checked;
                txtMaxAttachmentSize.Enabled = !chkInheritMaxAttachmentSize.Checked;
                txtIsAnswerLimit.Enabled = !chkInheritIsAnswerLimit.Checked;
                txtImageMaxSideSize.Enabled = !chkInheritMaxSideSize.Checked;
            }

            ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(forumObj.ForumGroupID);

            // Set default values
            if (fgi != null)
            {
                chkForumRequireEmail.SetDefaultValue(fgi.GroupRequireEmail);
                chkForumDisplayEmails.SetDefaultValue(fgi.GroupDisplayEmails);
                chkUseHTML.SetDefaultValue(fgi.GroupHTMLEditor);
                chkCaptcha.SetDefaultValue(fgi.GroupUseCAPTCHA);
                chkAuthorDelete.SetDefaultValue(fgi.GroupAuthorDelete);
                chkAuthorEdit.SetDefaultValue(fgi.GroupAuthorEdit);
                chkEnableOptIn.SetDefaultValue(fgi.GroupEnableOptIn);
                chkSendOptInConfirmation.SetDefaultValue(fgi.GroupSendOptInConfirmation);
            }
        }

        ltrScript.Text += ScriptHelper.GetScript(script);

        // Show/hide URL textboxes
        plcBaseAndUnsubUrl.Visible = (DisplayMode != ControlDisplayModeEnum.Simple);
    }


    /// <summary>
    /// Reloads the data of the editing forum.
    /// </summary>
    public override void ReloadData()
    {
        ForumInfo forumObj = ForumInfoProvider.GetForumInfo(mForumId);
        if (forumObj != null)
        {
            ReloadData(forumObj);
        }
    }


    /// <summary>
    /// Reloads the data of the editing forum.
    /// </summary>
    /// <param name="forumObj">Forum object</param>
    private void ReloadData(ForumInfo forumObj)
    {
        // Set main properties
        chkForumOpen.Checked = forumObj.ForumOpen;
        chkForumLocked.Checked = forumObj.ForumIsLocked;
        txtForumDescription.Text = forumObj.ForumDescription;
        txtForumDisplayName.Text = forumObj.ForumDisplayName;
        txtForumName.Text = forumObj.ForumName;

        // Set other settings
        txtImageMaxSideSize.Text = forumObj.ForumImageMaxSideSize.ToString();
        txtIsAnswerLimit.Text = forumObj.ForumIsAnswerLimit.ToString();
        txtMaxAttachmentSize.Text = forumObj.ForumAttachmentMaxFileSize.ToString();
        txtBaseUrl.Text = forumObj.ForumBaseUrl;
        txtUnsubscriptionUrl.Text = forumObj.ForumUnsubscriptionUrl;
        txtOptInURL.Text = forumObj.ForumOptInApprovalURL;

        // Three state checkboxes
        chkForumRequireEmail.InitFromThreeStateValue(forumObj, "ForumRequireEmail");
        chkCaptcha.InitFromThreeStateValue(forumObj, "ForumUseCAPTCHA");
        chkForumDisplayEmails.InitFromThreeStateValue(forumObj, "ForumDisplayEmails");
        chkUseHTML.InitFromThreeStateValue(forumObj, "ForumHTMLEditor");
        chkAuthorDelete.InitFromThreeStateValue(forumObj, "ForumAuthorDelete");
        chkAuthorEdit.InitFromThreeStateValue(forumObj, "ForumAuthorEdit");
        chkEnableOptIn.InitFromThreeStateValue(forumObj, "ForumEnableOptIn");
        chkSendOptInConfirmation.InitFromThreeStateValue(forumObj, "ForumSendOptInConfirmation");

        // Check if is inherited value
        bool inheritImageMaxSideSize = (forumObj.GetValue("ForumImageMaxSideSize") == null);
        bool inheritMaxAttachmentSize = (forumObj.GetValue("ForumAttachmentMaxFileSize") == null);
        bool inheritIsAnswerLimit = (forumObj.GetValue("ForumIsAnswerLimit") == null);
        bool inheritType = (forumObj.GetValue("ForumType") == null);
        bool inheritBaseUrl = (forumObj.GetValue("ForumBaseUrl") == null);
        bool inheritUnsubscriptionUrl = (forumObj.GetValue("ForumUnsubscriptionUrl") == null);
        bool inheritLogActivity = (forumObj.GetValue("ForumLogActivity") == null);
        bool inheritOptInApprovalUrl = (forumObj.GetValue("ForumOptInApprovalURL") == null);
        // Discussion
        bool inheritDiscussion = (forumObj.GetValue("ForumDiscussionActions") == null);

        // Set properties
        txtImageMaxSideSize.Enabled = !inheritImageMaxSideSize;
        chkInheritMaxSideSize.Checked = inheritImageMaxSideSize;
        txtIsAnswerLimit.Enabled = !inheritIsAnswerLimit;
        chkInheritIsAnswerLimit.Checked = inheritIsAnswerLimit;
        chkInheritType.Checked = inheritType;
        txtMaxAttachmentSize.Enabled = !inheritMaxAttachmentSize;
        chkInheritMaxAttachmentSize.Checked = inheritMaxAttachmentSize;
        txtBaseUrl.Enabled = !inheritBaseUrl;
        chkInheritBaseUrl.Checked = inheritBaseUrl;
        txtUnsubscriptionUrl.Enabled = !inheritUnsubscriptionUrl;
        chkInheritUnsubscribeUrl.Checked = inheritUnsubscriptionUrl;
        txtOptInURL.Enabled = !inheritOptInApprovalUrl;
        chkInheritOptInURL.Checked = inheritOptInApprovalUrl;

        plcOnline.Visible = ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(SiteContext.CurrentSiteName);
        chkInheritLogActivity.Checked = inheritLogActivity;

        // Discussion
        chkInheritDiscussion.Checked = inheritDiscussion;

        // Create script for update inherit values
        ltrScript.Text += ScriptHelper.GetScript(
            "LoadSetting('" + chkLogActivity.ClientID + "', " + (forumObj.ForumLogActivity ? "true" : "false") + ", " + (inheritLogActivity ? "true" : "false") + ", 'chk');" +
            // Discussion
            "LoadSetting('" + radImageSimple.ClientID + "', " + (forumObj.ForumEnableImage ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'rad');" +
            "LoadSetting('" + radImageAdvanced.ClientID + "', " + (forumObj.ForumEnableAdvancedImage ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'rad');" +
            "LoadSetting('" + radImageNo.ClientID + "', " + (!(forumObj.ForumEnableAdvancedImage || forumObj.ForumEnableImage) ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'rad');" +
            "LoadSetting('" + radUrlSimple.ClientID + "', " + (forumObj.ForumEnableURL ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'rad');" +
            "LoadSetting('" + radUrlAdvanced.ClientID + "', " + (forumObj.ForumEnableAdvancedURL ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'rad');" +
            "LoadSetting('" + radUrlNo.ClientID + "', " + (!(forumObj.ForumEnableAdvancedURL || forumObj.ForumEnableURL) ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'rad');" +
            "LoadSetting('" + chkEnableQuote.ClientID + "', " + (forumObj.ForumEnableQuote ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'chk');" +
            "LoadSetting('" + chkEnableBold.ClientID + "', " + (forumObj.ForumEnableFontBold ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'chk');" +
            "LoadSetting('" + chkEnableItalic.ClientID + "', " + (forumObj.ForumEnableFontItalics ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'chk');" +
            "LoadSetting('" + chkEnableStrike.ClientID + "', " + (forumObj.ForumEnableFontStrike ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'chk');" +
            "LoadSetting('" + chkEnableUnderline.ClientID + "', " + (forumObj.ForumEnableFontUnderline ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'chk');" +
            "LoadSetting('" + chkEnableCode.ClientID + "', " + (forumObj.ForumEnableCodeSnippet ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'chk');" +
            "LoadSetting('" + chkEnableColor.ClientID + "', " + (forumObj.ForumEnableFontColor ? "true" : "false") + ", " + (inheritDiscussion ? "true" : "false") + ", 'chk');" +
            "LoadSetting('" + radTypeAnswer.ClientID + "', " + (forumObj.ForumType == 2 ? "true" : "false") + ", " + (inheritType ? "true" : "false") + ", 'rad');" +
            "LoadSetting('" + radTypeDiscussion.ClientID + "', " + (forumObj.ForumType == 1 ? "true" : "false") + ", " + (inheritType ? "true" : "false") + ", 'rad');" +
            "LoadSetting('" + radTypeChoose.ClientID + "', " + (forumObj.ForumType == 0 ? "true" : "false") + ", " + (inheritType ? "true" : "false") + ", 'rad');");

        ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(forumObj.ForumGroupID);

        chkInheritUnsubscribeUrl.Attributes.Add("onclick", "SetInheritance('" + txtUnsubscriptionUrl.ClientID + "', '" + fgi.GroupUnsubscriptionUrl.Replace("'", "\\\'") + "', 'txt');");
        chkInheritBaseUrl.Attributes.Add("onclick", "SetInheritance('" + txtBaseUrl.ClientID + "', '" + fgi.GroupBaseUrl.Replace("'", "\\\'") + "', 'txt');");
        chkInheritOptInURL.Attributes.Add("onclick", "SetInheritance('" + txtOptInURL.PathTextBox.ClientID + "', '" + fgi.GroupOptInApprovalURL.Replace("'", "\\\'") + "', 'txt');ChangeState_" + txtOptInURL.ClientID + "(!this.checked);");

        // Set default values
        chkForumRequireEmail.SetDefaultValue(fgi.GroupRequireEmail);
        chkForumDisplayEmails.SetDefaultValue(fgi.GroupDisplayEmails);
        chkUseHTML.SetDefaultValue(fgi.GroupHTMLEditor);
        chkCaptcha.SetDefaultValue(fgi.GroupUseCAPTCHA);
        chkAuthorDelete.SetDefaultValue(fgi.GroupAuthorDelete);
        chkAuthorEdit.SetDefaultValue(fgi.GroupAuthorEdit);
        chkEnableOptIn.SetDefaultValue(fgi.GroupEnableOptIn);
        chkSendOptInConfirmation.SetDefaultValue(fgi.GroupSendOptInConfirmation);

        // Settings inheritance
        chkInheritIsAnswerLimit.Attributes.Add("onclick", "SetInheritance('" + txtIsAnswerLimit.ClientID + "', '" + fgi.GroupIsAnswerLimit.ToString().Replace("'", "\\\'") + "', 'txt');");
        chkInheritMaxSideSize.Attributes.Add("onclick", "SetInheritance('" + txtImageMaxSideSize.ClientID + "', '" + fgi.GroupImageMaxSideSize.ToString().Replace("'", "\\\'") + "', 'txt');");
        chkInheritType.Attributes.Add("onclick", "SetInheritance('" + radTypeAnswer.ClientID + "', '" + (fgi.GroupType == 2 ? "true" : "false") + "', 'rad');" +
                                                 "SetInheritance('" + radTypeDiscussion.ClientID + "', '" + (fgi.GroupType == 1 ? "true" : "false") + "', 'rad');" +
                                                 "SetInheritance('" + radTypeChoose.ClientID + "', '" + (fgi.GroupType == 0 ? "true" : "false") + "', 'rad');");
        chkInheritMaxAttachmentSize.Attributes.Add("onclick", "SetInheritance('" + txtMaxAttachmentSize.ClientID + "','" + fgi.GroupAttachmentMaxFileSize.ToString().Replace("'", "\\\'") + "', 'txt');");
        chkInheritLogActivity.Attributes.Add("onclick", "SetInheritance('" + chkLogActivity.ClientID + "','" + fgi.GroupLogActivity.ToString().ToLowerCSafe() + "', 'chk');");

        // Discussion
        string chkList = "'" + radImageSimple.ClientID + ";" + chkEnableBold.ClientID + ";" + chkEnableCode.ClientID + ";" +
                         chkEnableColor.ClientID + ";" + radUrlSimple.ClientID + ";" + chkEnableItalic.ClientID + ";" +
                         radImageAdvanced.ClientID + ";" + radUrlAdvanced.ClientID + ";" + chkEnableQuote.ClientID + ";" +
                         chkEnableStrike.ClientID + ";" + chkEnableUnderline.ClientID + ";" + radImageNo.ClientID + ";" + radUrlNo.ClientID + "'";
        string chkListValues = "'" + fgi.GroupEnableImage.ToString() + ";" + fgi.GroupEnableFontBold.ToString() + ";" + fgi.GroupEnableCodeSnippet.ToString() + ";" +
                               fgi.GroupEnableFontColor.ToString() + ";" + fgi.GroupEnableURL.ToString() + ";" + fgi.GroupEnableFontItalics.ToString() + ";" +
                               fgi.GroupEnableAdvancedImage.ToString() + ";" + fgi.GroupEnableAdvancedURL.ToString() + ";" + fgi.GroupEnableQuote.ToString() + ";" +
                               fgi.GroupEnableFontStrike.ToString() + ";" + fgi.GroupEnableFontUnderline.ToString() + ";" +
                               !(fgi.GroupEnableAdvancedImage || fgi.GroupEnableImage) + ";" + !(fgi.GroupEnableAdvancedURL || fgi.GroupEnableURL) + "'";
        chkInheritDiscussion.Attributes.Add("onclick", "SetInheritance(" + chkList + ", " + chkListValues.ToLowerCSafe() + ", 'chk');");
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check MODIFY permission
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        // Check required fields
        string errorMessage = new Validator().NotEmpty(txtForumDisplayName.Text, GetString("Forum_General.EmptyDisplayName")).Result;

        if ((errorMessage == String.Empty) && (DisplayMode != ControlDisplayModeEnum.Simple))
        {
            errorMessage = new Validator().NotEmpty(txtForumName.Text, GetString("Forum_General.EmptyCodeName")).IsCodeName(txtForumName.Text.Trim(), GetString("general.errorcodenameinidentifierformat")).Result;
        }

        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        // Forum must be on some site
        if (SiteContext.CurrentSite != null)
        {
            int communityGroupId = 0;
            ForumInfo fi = ForumInfoProvider.GetForumInfo(mForumId);
            if (fi != null)
            {
                ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(fi.ForumGroupID);
                if (fgi != null)
                {
                    communityGroupId = fgi.GroupGroupID;
                }
            }

            ForumInfo forumObj = ForumInfoProvider.GetForumInfo(txtForumName.Text.Trim(), SiteContext.CurrentSiteID, communityGroupId);

            // If forum exists
            if ((forumObj == null) || (forumObj.ForumID == mForumId))
            {
                if (forumObj == null)
                {
                    forumObj = ForumInfoProvider.GetForumInfo(mForumId);
                }

                if (forumObj != null)
                {
                    if (txtForumDisplayName.Text.Trim() != forumObj.ForumDisplayName)
                    {
                        // Refresh a breadcrumb if used in the tabs layout
                        ScriptHelper.RefreshTabHeader(Page, txtForumDisplayName.Text);
                    }

                    // Set properties
                    forumObj.ForumIsLocked = chkForumLocked.Checked;
                    forumObj.ForumOpen = chkForumOpen.Checked;
                    chkForumDisplayEmails.SetThreeStateValue(forumObj, "ForumDisplayEmails");
                    forumObj.ForumDescription = txtForumDescription.Text.Trim();
                    chkForumRequireEmail.SetThreeStateValue(forumObj, "ForumRequireEmail");
                    forumObj.ForumDisplayName = txtForumDisplayName.Text.Trim();
                    chkCaptcha.SetThreeStateValue(forumObj, "ForumUseCAPTCHA");

                    // If display mode is default set other properties
                    if (DisplayMode != ControlDisplayModeEnum.Simple)
                    {
                        chkUseHTML.SetThreeStateValue(forumObj, "ForumHTMLEditor");
                        forumObj.ForumName = txtForumName.Text.Trim();

                        // Base URL
                        if (chkInheritBaseUrl.Checked)
                        {
                            forumObj.SetValue("ForumBaseUrl", null);
                        }
                        else
                        {
                            forumObj.ForumBaseUrl = txtBaseUrl.Text;
                        }

                        // Unsubcription URL
                        if (chkInheritUnsubscribeUrl.Checked)
                        {
                            forumObj.SetValue("ForumUnsubscriptionUrl", null);
                        }
                        else
                        {
                            forumObj.ForumUnsubscriptionUrl = txtUnsubscriptionUrl.Text.Trim();
                        }
                    }

                    // Author can delete own posts
                    chkAuthorDelete.SetThreeStateValue(forumObj, "ForumAuthorDelete");

                    // Author can delete own posts
                    chkAuthorEdit.SetThreeStateValue(forumObj, "ForumAuthorEdit");

                    // Image max side size
                    if (chkInheritMaxSideSize.Checked)
                    {
                        forumObj.SetValue("ForumImageMaxSideSize", null);
                    }
                    else
                    {
                        forumObj.ForumImageMaxSideSize = ValidationHelper.GetInteger(txtImageMaxSideSize.Text, 400);
                    }

                    // Answer limit
                    if (chkInheritIsAnswerLimit.Checked)
                    {
                        forumObj.SetValue("ForumIsAnswerLimit", null);
                    }
                    else
                    {
                        forumObj.ForumIsAnswerLimit = ValidationHelper.GetInteger(txtIsAnswerLimit.Text, 5);
                    }

                    // Forum type
                    if (chkInheritType.Checked)
                    {
                        forumObj.SetValue("ForumType", null);
                    }
                    else
                    {
                        forumObj.ForumType = (radTypeChoose.Checked ? 0 : (radTypeDiscussion.Checked ? 1 : 2));
                    }

                    // Inherited values
                    if (chkInheritBaseUrl.Checked)
                    {
                        forumObj.ForumBaseUrl = null;
                    }

                    if (chkInheritUnsubscribeUrl.Checked)
                    {
                        forumObj.ForumUnsubscriptionUrl = null;
                    }

                    if (chkInheritMaxAttachmentSize.Checked)
                    {
                        forumObj.ForumAttachmentMaxFileSize = -1;
                    }
                    else
                    {
                        forumObj.ForumAttachmentMaxFileSize = ValidationHelper.GetInteger(txtMaxAttachmentSize.Text, 0);
                    }

                    // Double opt-in
                    forumObj.ForumOptInApprovalURL = chkInheritOptInURL.Checked ? null : txtOptInURL.Text.Trim();
                    chkEnableOptIn.SetThreeStateValue(forumObj, "ForumEnableOptIn");
                    chkSendOptInConfirmation.SetThreeStateValue(forumObj, "ForumSendOptInConfirmation");

                    // Only if on-line marketing is available
                    if (plcOnline.Visible)
                    {
                        if (chkInheritLogActivity.Checked)
                        {
                            forumObj.SetValue("ForumLogActivity", null);
                        }
                        else
                        {
                            forumObj.ForumLogActivity = chkLogActivity.Checked;
                        }
                    }


                    #region "Discussion"

                    if (chkInheritDiscussion.Checked)
                    {
                        // Inherited
                        forumObj.SetValue("ForumDiscussionActions", null);
                    }
                    else
                    {
                        // Set discussion properties
                        forumObj.ForumEnableQuote = chkEnableQuote.Checked;
                        forumObj.ForumEnableFontBold = chkEnableBold.Checked;
                        forumObj.ForumEnableFontItalics = chkEnableItalic.Checked;
                        forumObj.ForumEnableFontUnderline = chkEnableUnderline.Checked;
                        forumObj.ForumEnableFontStrike = chkEnableStrike.Checked;
                        forumObj.ForumEnableFontColor = chkEnableColor.Checked;
                        forumObj.ForumEnableCodeSnippet = chkEnableCode.Checked;
                        forumObj.ForumEnableAdvancedImage = radImageAdvanced.Checked;
                        forumObj.ForumEnableAdvancedURL = radUrlAdvanced.Checked;
                        forumObj.ForumEnableImage = radImageSimple.Checked;
                        forumObj.ForumEnableURL = radUrlSimple.Checked;
                    }

                    #endregion


                    ForumInfoProvider.SetForumInfo(forumObj);

                    ReloadData();

                    RaiseOnSaved();

                    ShowChangesSaved();
                }
                else
                {
                    // Forum does not exist
                    ShowError(GetString("Forum_Edit.ForumDoesNotExist"));
                }
            }
            else
            {
                // Forum already exists
                ShowError(GetString("Forum_Edit.ForumAlreadyExists"));
            }
        }
    }

    #endregion
}
