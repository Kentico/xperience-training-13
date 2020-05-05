using System;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Layouts_Flat_ThreadEdit : ForumViewer
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsAdHocForum)
        {
            plcHeader.Visible = false;
        }

        forumEdit.OnPreview += new EventHandler(forumEdit_OnPreview);
        forumEdit.OnModerationRequired += new EventHandler(forumEdit_OnModerationRequired);

        // Check whether subscription is for forum or post
        if (ForumContext.CurrentReplyThread == null)
        {
            ltrTitle.Text = GetString("Forums_ForumNewPost_Header.NewThread");

            if (ForumContext.CurrentPost != null && ForumContext.CurrentMode == ForumMode.Edit)
            {
                ltrTitle.Text = GetString("Forums_ForumNewPost_Header.EditPost");
            }
        }
        else
        {
            plcPreview.Visible = true;

            ltrTitle.Text = GetString("Forums_ForumNewPost_Header.Reply");

            ltrAvatar.Text = AvatarImage(ForumContext.CurrentReplyThread);
            ltrSubject.Text = HTMLHelper.HTMLEncode(ForumContext.CurrentReplyThread.PostSubject);
            if (ForumContext.CurrentForum != null)
            {
                if (ForumContext.CurrentForum.ForumEnableAdvancedImage)
                {
                    ltrText.AllowedControls = ControlsHelper.ALLOWED_FORUM_CONTROLS;
                }
                else
                {
                    ltrText.AllowedControls = "none";
                }
                ltrText.Text = ResolvePostText(ForumContext.CurrentReplyThread.PostText);
            }
            ltrUserName.Text = HTMLHelper.HTMLEncode(ForumContext.CurrentReplyThread.PostUserName);
            ltrTime.Text = TimeZoneUIMethods.ConvertDateTime(ForumContext.CurrentReplyThread.PostTime, this).ToString();

            UserSettingsInfo usi = UserSettingsInfoProvider.GetUserSettingsInfoByUser(ForumContext.CurrentReplyThread.PostUserID);
            string badgeName = null;
            string badgeImageUrl = null;            

            ltrBadge.Text = GetNotEmpty(badgeName, "<div class=\"Badge\">" + badgeName + "</div>", "<div class=\"Badge\">" + GetString("Forums.PublicBadge") + "</div>", ForumActionType.Badge) +
                            GetNotEmpty(badgeImageUrl, "<div class=\"BadgeImage\"><img alt=\"" + badgeName + "\" src=\"" + GetImageUrl(ValidationHelper.GetString(badgeImageUrl, "")) + "\" /></div>", "", ForumActionType.Badge);
        }
    }


    /// <summary>
    /// OnModeration required.
    /// </summary>
    private void forumEdit_OnModerationRequired(object sender, EventArgs e)
    {
        CMSUserControl ctrl = sender as CMSUserControl;
        if (ctrl != null)
        {
            ctrl.StopProcessing = true;
        }

        plcPreview.Visible = false;
        forumEdit.Visible = false;
        plcModerationRequired.Visible = true;
        ltrTitle.Visible = false;
        lblModerationInfo.Text = GetString("forums.requiresmoderationafteraction");
        btnOk.Text = GetString("general.ok");
    }


    /// <summary>
    /// OK click hadler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Redirect back to the forum or forum thread
        URLHelper.Redirect(ClearURL());
    }


    /// <summary>
    /// OnPreview handler.
    /// </summary>
    private void forumEdit_OnPreview(object sender, EventArgs e)
    {
        // Preview title
        ltrTitle.Text = GetString("Forums_ForumNewPost_Header.Preview");
        // Display placeholder
        plcPreview.Visible = true;

        // Post properties
        ForumPostInfo fpi = sender as ForumPostInfo;
        if (fpi != null)
        {
            ltrAvatar.Text = AvatarImage(fpi);
            ltrText.Text = ResolvePostText(fpi.PostText);
            ltrUserName.Text = HTMLHelper.HTMLEncode(fpi.PostUserName);
            ltrTime.Text = TimeZoneUIMethods.ConvertDateTime(fpi.PostTime, this).ToString();

            string badgeName = null;
            string badgeImageUrl = null;
            

            ltrBadge.Text = GetNotEmpty(badgeName, "<div class=\"Badge\">" + badgeName + "</div>", "<div class=\"Badge\">" + GetString("Forums.PublicBadge") + "</div>", ForumActionType.Badge) +
                            GetNotEmpty(badgeImageUrl, "<div class=\"BadgeImage\"><img alt=\"" + badgeName + "\" src=\"" + GetImageUrl(ValidationHelper.GetString(badgeImageUrl, "")) + "\" /></div>", "", ForumActionType.Badge);
        }
    }
}