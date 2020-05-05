using System;

using CMS.Base.Web.UI;
using CMS.Community;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_GroupLeave : CMSAdminControl
{
    #region "Variables"

    private string mLeaveText;
    private string mSuccessfulLeaveText;
    private string mUnSuccessfulLeaveText;
    private GroupInfo mGroup = null;
    private bool mIsOnModalPage = true;
    private CMSButton mLeaveButton = null;
    private CMSButton mCancelButton = null;

    #endregion


    #region "Private properties"

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

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the text which should be displayed on join dialog.
    /// </summary>
    public string LeaveText
    {
        get
        {
            return DataHelper.GetNotEmpty(mLeaveText, GetString("Community.Group.Leave") + HTMLHelper.HTMLEncode(GroupName) + "?");
        }
        set
        {
            mLeaveText = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which should be displayed on join dialog after successful join action.
    /// </summary>
    public string SuccessfulLeaveText
    {
        get
        {
            return DataHelper.GetNotEmpty(mSuccessfulLeaveText, GetString("Community.Group.SuccessfulLeave").Replace("##GroupName##", HTMLHelper.HTMLEncode(GroupName)));
        }
        set
        {
            mSuccessfulLeaveText = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which should be displayed on join dialog if join action was unsuccessful.
    /// </summary>
    public string UnSuccessfulLeaveText
    {
        get
        {
            return DataHelper.GetNotEmpty(mUnSuccessfulLeaveText, GetString("Community.Group.UnSuccessfulLeave"));
        }
        set
        {
            mUnSuccessfulLeaveText = value;
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
    /// Specifies if control is placed on modal page or not.
    /// </summary>
    public bool IsOnModalPage
    {
        get
        {
            return mIsOnModalPage;
        }
        set
        {
            mIsOnModalPage = value;
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
    /// Leave button.
    /// </summary>
    public CMSButton LeaveButton
    {
        get
        {
            if (mLeaveButton == null)
            {
                mLeaveButton = btnLeave;
            }
            return mLeaveButton;
        }
        set
        {
            mLeaveButton = value;
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

        LeaveButton.Click += new EventHandler(btnLeave_Click);
        LeaveButton.Text = GetString("General.Yes");
        CancelButton.Text = GetString("General.No");

        // Set up js action if webpart is placed on modal page
        if (IsOnModalPage)
        {
            CancelButton.OnClientClick = "CloseDialog()";
        }
        else
        {
            // Get return url
            string returnUrl = QueryHelper.GetString("returnurl", "");
            if (URLHelper.IsLocalUrl(returnUrl))
            {
                // Redirect
                URLHelper.Redirect(returnUrl);
            }
        }
        lblInfo.Text = LeaveText;
    }


    /// <summary>
    /// Join handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    protected void btnLeave_Click(object sender, EventArgs e)
    {
        LeaveButton.Visible = false;
        // Set up js action if webpart is placed on modal page
        if (IsOnModalPage)
        {
            CancelButton.Text = GetString("General.Close");
            CancelButton.OnClientClick = "if (wopener != null) {wopener.ReloadPage();} CloseDialog();";
        }
        else
        {
            CancelButton.Text = GetString("General.Ok");
            CancelButton.Click += new EventHandler(btnCancel_Click);
        }

        if (Group == null)
        {
            return;
        }

        // Get group member info        
        GroupMemberInfo gmi = GroupMemberInfoProvider.GetGroupMemberInfo(MembershipContext.AuthenticatedUser.UserID, Group.GroupID);
        if (gmi != null)
        {
            GroupMemberInfoProvider.DeleteGroupMemberInfo(gmi);

            if (Group.GroupSendJoinLeaveNotification)
            {
                GroupMemberInfoProvider.SendNotificationMail("Groups.MemberLeave", SiteContext.CurrentSiteName, gmi, true);
            }

            lblInfo.Text = SuccessfulLeaveText;
            return;
        }


        lblInfo.Text = SuccessfulLeaveText;
    }


    /// <summary>
    /// Cancel click.
    /// </summary>    
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(RequestContext.CurrentURL);
    }
}
