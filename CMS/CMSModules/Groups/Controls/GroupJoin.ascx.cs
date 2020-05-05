using System;

using CMS.Base.Web.UI;
using CMS.Community;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_GroupJoin : CMSAdminControl
{
    #region "Variables"

    private string mJoinText;
    private string mSuccessfulJoinText;
    private string mSuccessfulJoinTextWaitingForApproval;
    private string mUnSuccessfulJoinText;
    private GroupInfo mGroup = null;
    private CMSButton mJoinButton = null;
    private CMSButton mCancelButton = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Returns group name of current group.
    /// </summary>
    private string GroupName
    {
        get
        {
            if (Group != null)
            {
                return " " + Group.GroupDisplayName;
            }

            return "";
        }
    }


    /// <summary>
    /// Gets or sets the text which should be displayed on join dialog.
    /// </summary>
    public string JoinText
    {
        get
        {
            return DataHelper.GetNotEmpty(mJoinText, GetString("Community.Group.Join") + HTMLHelper.HTMLEncode(GroupName) + "?");
        }
        set
        {
            mJoinText = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which should be displayed on join dialog after successful join action.
    /// </summary>
    public string SuccessfulJoinText
    {
        get
        {
            return DataHelper.GetNotEmpty(mSuccessfulJoinText, GetString("Community.Group.SuccessfulJoin").Replace("##GroupName##", HTMLHelper.HTMLEncode(GroupName)));
        }
        set
        {
            mSuccessfulJoinText = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which should be displayed on join dialog after successful join action but must be approved by group admin.
    /// </summary>
    public string SuccessfulJoinTextWaitingForApproval
    {
        get
        {
            return DataHelper.GetNotEmpty(mSuccessfulJoinTextWaitingForApproval, GetString("Community.Group.SuccessfulJoinApproval").Replace("##GroupName##", HTMLHelper.HTMLEncode(GroupName)));
        }
        set
        {
            mSuccessfulJoinTextWaitingForApproval = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which should be displayed on join dialog if join actin was unsuccessful.
    /// </summary>
    public string UnSuccessfulJoinText
    {
        get
        {
            return DataHelper.GetNotEmpty(mUnSuccessfulJoinText, GetString("Community.Group.UnSuccessfulJoin"));
        }
        set
        {
            mUnSuccessfulJoinText = value;
        }
    }


    /// <summary>
    /// Gets or sets the group info object for destination group.
    /// </summary>
    public GroupInfo Group
    {
        get
        {
            return mGroup;
        }
        set
        {
            mGroup = value;
        }
    }


    /// <summary>
    /// Indicates if control buttons should be displayed.
    /// </summary>
    public bool DisplayButtons
    {
        get
        {
            return plcButtons.Visible;
        }
        set
        {
            plcButtons.Visible = value;
        }
    }


    /// <summary>
    /// Join button.
    /// </summary>
    public CMSButton JoinButton
    {
        get
        {
            if (mJoinButton == null)
            {
                mJoinButton = btnJoin;
            }
            return mJoinButton;
        }
        set
        {
            mJoinButton = value;
        }
    }


    /// <summary>
    /// Cancel button.
    /// </summary>
    public CMSButton CancelButton
    {
        get
        {
            if (mCancelButton == null)
            {
                mCancelButton = btnCancel;
            }
            return mCancelButton;
        }
        set
        {
            mCancelButton = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterWOpenerScript(Page);
        JoinButton.Click += new EventHandler(btnJoin_Click);
        CancelButton.Text = GetString("General.Cancel");
        JoinButton.Text = GetString("General.Join");

        if (MembershipContext.AuthenticatedUser.IsPublic())
        {
            JoinButton.Visible = false;
            lblInfo.Visible = false;
            return;
        }

        // Check whether user is not already a member, has requested membership or has been denied.
        GroupMemberInfo groupMemberInfo = GroupMemberInfoProvider.GetGroupMemberInfo(MembershipContext.AuthenticatedUser.UserID, Group.GroupID);
        if (groupMemberInfo != null)
        {
            if ((groupMemberInfo.MemberStatus == GroupMemberStatus.Approved) || (groupMemberInfo.MemberStatus == GroupMemberStatus.GroupAdmin))
            {
                lblInfo.Text = GetString("community.group.join.alreadyamember");
            }
            else
            {
                lblInfo.Text = GetString("community.group.join.pendingordenied");
            }
            JoinButton.Visible = false;
            CancelButton.Text = GetString("General.Close");
        }
        else
        {
            lblInfo.Text = JoinText;
        }

        CancelButton.OnClientClick = "CloseDialog()";
    }


    /// <summary>
    /// Join handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    protected void btnJoin_Click(object sender, EventArgs e)
    {
        if (MembershipContext.AuthenticatedUser.IsPublic())
        {
            return;
        }

        CancelButton.Text = GetString("General.Close");
        JoinButton.Enabled = false;
        CancelButton.OnClientClick = "if ((wopener != null)&&(wopener.ReloadPage != null)) {wopener.ReloadPage();} CloseDialog();";

        if (Group == null)
        {
            return;
        }


        // Check whether user is not already in group or group is not approved or group is not enabled
        if (GroupMemberInfoProvider.IsMemberOfGroup(MembershipContext.AuthenticatedUser.UserID, Group.GroupID) ||
            !Group.GroupApproved)
        {
            lblInfo.Text = UnSuccessfulJoinText;
            return;
        }

        GroupMemberInfo gmi = new GroupMemberInfo();
        ;
        gmi.MemberJoined = DateTime.Now;
        gmi.MemberRejectedWhen = DateTimeHelper.ZERO_TIME;
        gmi.MemberStatus = GroupMemberStatus.Rejected;

        switch (Group.GroupApproveMembers)
        {
                // Only approved members can join
            case GroupApproveMembersEnum.ApprovedCanJoin:
                // Only invited members
            case GroupApproveMembersEnum.InvitedWithoutApproval:
                gmi.MemberGroupID = Group.GroupID;
                gmi.MemberUserID = MembershipContext.AuthenticatedUser.UserID;
                gmi.MemberStatus = GroupMemberStatus.WaitingForApproval;
                GroupMemberInfoProvider.SetGroupMemberInfo(gmi);

                if (Group.GroupSendWaitingForApprovalNotification)
                {
                    GroupMemberInfoProvider.SendNotificationMail("Groups.MemberWaitingForApproval", SiteContext.CurrentSiteName, gmi, true);
                    GroupMemberInfoProvider.SendNotificationMail("Groups.MemberJoinedWaitingForApproval", SiteContext.CurrentSiteName, gmi, false);
                }
                lblInfo.Text = SuccessfulJoinTextWaitingForApproval;
                break;

                // Any site members can join
            case GroupApproveMembersEnum.AnyoneCanJoin:
                gmi.MemberGroupID = Group.GroupID;
                gmi.MemberUserID = MembershipContext.AuthenticatedUser.UserID;
                gmi.MemberStatus = GroupMemberStatus.Approved;
                gmi.MemberApprovedWhen = DateTime.Now;
                GroupMemberInfoProvider.SetGroupMemberInfo(gmi);
                if (Group.GroupSendJoinLeaveNotification)
                {
                    GroupMemberInfoProvider.SendNotificationMail("Groups.MemberJoin", SiteContext.CurrentSiteName, gmi, true);
                    GroupMemberInfoProvider.SendNotificationMail("Groups.MemberJoinedConfirmation", SiteContext.CurrentSiteName, gmi, false);
                }
                lblInfo.Text = SuccessfulJoinText;
                break;
        }
    }
}