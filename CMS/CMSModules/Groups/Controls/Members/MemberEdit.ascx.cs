using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Community;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_Members_MemberEdit : CMSAdminEditControl
{
    #region "Variables"

    private GroupMemberInfo mCurrentMember;
    private bool newItem;
    private bool currentRolesLoaded;

    protected string currentValues = string.Empty;

    #endregion


    #region "Events"

    public event EventHandler OnApprove;
    public event EventHandler OnReject;

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
    /// Current group member ID.
    /// </summary>
    public int MemberID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["MemberID"], 0);
        }
        set
        {
            ViewState["MemberID"] = value;
        }
    }


    /// <summary>
    /// Current group ID.
    /// </summary>
    public int GroupID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["GroupID"], 0);
        }
        set
        {
            ViewState["GroupID"] = value;
        }
    }

    #endregion


    #region "Private properties"

    private GroupMemberInfo CurrentMember
    {
        get
        {
            return mCurrentMember ?? (mCurrentMember = (MemberID > 0) ? GroupMemberInfoProvider.GetGroupMemberInfo(MemberID) : new GroupMemberInfo());
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        if (StopProcessing)
        {
            return;
        }

        // Is live site
        userSelector.IsLiveSite = IsLiveSite;
        userSelector.ShowSiteFilter = false;
        userSelector.HideHiddenUsers = true;
        userSelector.HideDisabledUsers = true;
        usRoles.IsLiveSite = IsLiveSite;
        if (GroupID > 0)
        {
            usRoles.ObjectType = RoleInfo.OBJECT_TYPE_GROUP;
        }

        // In case of uniselector's callback calling must be where condition set here
        string where = CreateWhereCondition();
        usRoles.WhereCondition = where;

        if (!RequestHelper.IsPostBack() && !IsLiveSite)
        {
            // Reload data
            ReloadData();
        }

        // Set edited object
        EditedObject = CurrentMember;

        // Initialize user selector
        if (SiteContext.CurrentSite != null)
        {
            userSelector.SiteID = SiteContext.CurrentSite.SiteID;
        }

        // Add onclick handlers
        btnSave.Click += btnSave_Click;
        btnApprove.Click += btnApprove_Click;
        btnReject.Click += btnReject_Click;
        usRoles.OnSelectionChanged += usRoles_OnSelectionChanged;

        // Initialize buttons
        btnSave.Text = GetString("general.ok");
        btnApprove.Text = GetString("general.approve");
        btnReject.Text = GetString("general.reject");
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        // Get roles for current user 
        LoadCurrentRoles();

        string where = CreateWhereCondition();
        usRoles.WhereCondition = where;

        // Show message or uniselector?
        RoleInfo role = RoleInfo.Provider.Get().Where(where).Columns("RoleID").TopN(1).FirstOrDefault();
        if (role == null)
        {
            usRoles.Visible = false;
            lblRole.Visible = true;
        }
        else
        {
            usRoles.Visible = true;
            lblRole.Visible = false;
        }

        // Enable or disable buttons according to state of user's approval process
        if ((CurrentMember != null) && (CurrentMember.MemberGroupID > 0))
        {
            // Current user cannot approve/reject him self
            if (IsLiveSite && (CurrentMember.MemberUserID == MembershipContext.AuthenticatedUser.UserID))
            {
                // Member can nothing
                btnApprove.Enabled = false;
                btnReject.Enabled = false;
            }
            else if (CurrentMember.MemberStatus == GroupMemberStatus.Approved)
            {
                // Member can be rejected
                btnApprove.Enabled = false;
                btnReject.Enabled = true;
            }
            else if (CurrentMember.MemberStatus == GroupMemberStatus.Rejected)
            {
                // Member can be approved
                btnApprove.Enabled = true;
                btnReject.Enabled = false;
            }
            else if (CurrentMember.MemberStatus == GroupMemberStatus.WaitingForApproval)
            {
                // Member can be rejected and approved
                btnApprove.Enabled = true;
                btnReject.Enabled = true;
            }
        }

        InitializeForm();
        usRoles.Value = currentValues;
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (SaveData())
        {
            // Save data
            ShowChangesSaved();

            RaiseOnSaved();
        }
    }


    protected void btnApprove_Click(object sender, EventArgs e)
    {
        if (ApproveMember())
        {
            // Approve member
            ShowConfirmation(GetString("group.member.userhasbeenapproved"));
            ReloadData();

            if (OnApprove != null)
            {
                OnApprove(this, null);
            }
        }
    }


    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (RejectMember())
        {
            // Reject member
            ShowConfirmation(GetString("group.member.userhasbeenrejected"));
            ReloadData();

            if (OnReject != null)
            {
                OnReject(this, null);
            }
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Resets the form to default values.
    /// </summary>
    public override void ClearForm()
    {
        // Clear form
        txtComment.Text = "";
        chkApprove.Checked = true;
    }


    /// <summary>
    /// Approves member.
    /// </summary>
    public bool ApproveMember()
    {
        // Check MANAGE permission for groups module
        if (!CheckPermissions("cms.groups", PERMISSION_MANAGE, GroupID))
        {
            return false;
        }

        if ((CurrentMember != null) && (MembershipContext.AuthenticatedUser != null))
        {
            // Set properties
            CurrentMember.MemberApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
            CurrentMember.MemberStatus = GroupMemberStatus.Approved;
            CurrentMember.MemberApprovedWhen = DateTime.Now;
            CurrentMember.MemberRejectedWhen = DateTimeHelper.ZERO_TIME;
            GroupMemberInfoProvider.SetGroupMemberInfo(CurrentMember);
            GroupInfo group = GroupInfoProvider.GetGroupInfo(GroupID);
            if ((group != null) && (group.GroupSendWaitingForApprovalNotification))
            {
                // Send notification email
                GroupMemberInfoProvider.SendNotificationMail("Groups.MemberApproved", SiteContext.CurrentSiteName, CurrentMember, false);
            }

            lblMemberApproved.Text = GetApprovalInfoText(CurrentMember.MemberApprovedWhen, CurrentMember.MemberApprovedByUserID);
            lblMemberRejected.Text = GetApprovalInfoText(CurrentMember.MemberRejectedWhen, CurrentMember.MemberApprovedByUserID);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Approves member.
    /// </summary>
    public bool RejectMember()
    {
        // Check MANAGE permission for groups module
        if (!CheckPermissions("cms.groups", PERMISSION_MANAGE, GroupID))
        {
            return false;
        }

        if ((CurrentMember != null) && (MembershipContext.AuthenticatedUser != null))
        {
            // Set properties
            CurrentMember.MemberApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
            CurrentMember.MemberStatus = GroupMemberStatus.Rejected;
            CurrentMember.MemberApprovedWhen = DateTimeHelper.ZERO_TIME;
            CurrentMember.MemberRejectedWhen = DateTime.Now;

            // Save to database
            GroupMemberInfoProvider.SetGroupMemberInfo(CurrentMember);

            GroupInfo group = GroupInfoProvider.GetGroupInfo(GroupID);
            if ((group != null) && (group.GroupSendWaitingForApprovalNotification))
            {
                // Send notification email
                GroupMemberInfoProvider.SendNotificationMail("Groups.MemberRejected", SiteContext.CurrentSiteName, CurrentMember, false);
            }

            lblMemberApproved.Text = GetApprovalInfoText(CurrentMember.MemberApprovedWhen, CurrentMember.MemberApprovedByUserID);
            lblMemberRejected.Text = GetApprovalInfoText(CurrentMember.MemberRejectedWhen, CurrentMember.MemberApprovedByUserID);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Updates the current Group or creates new if no MemberID is present.
    /// </summary>
    public bool SaveData()
    {
        // Check MANAGE permission for groups module
        if (!CheckPermissions("cms.groups", PERMISSION_MANAGE, GroupID))
        {
            return false;
        }

        newItem = (MemberID == 0);

        if (CurrentMember != null)
        {
            if (CurrentMember.MemberID > 0)
            {
                // Get user info
                UserInfo ui = UserInfo.Provider.Get(CurrentMember.MemberUserID);
                if (ui != null)
                {
                    // Save user roles
                    SaveRoles(ui.UserID);

                    CurrentMember.MemberComment = txtComment.Text;
                    GroupMemberInfoProvider.SetGroupMemberInfo(CurrentMember);

                    return true;
                }
            }
            else
            {
                // New member
                if (newItem)
                {
                    int userId = ValidationHelper.GetInteger(userSelector.Value, 0);

                    // Check if some user was selected
                    if (userId == 0)
                    {
                        ShowError(GetString("group.member.selectuser"));
                        return false;
                    }

                    // Check if user is not already group member
                    var existing = GroupMemberInfoProvider.GetGroupMemberInfo(userId, GroupID);
                    if (existing != null)
                    {
                        ShowError(GetString("group.member.userexists"));
                        return false;
                    }

                    // New member object
                    CurrentMember.MemberGroupID = GroupID;
                    CurrentMember.MemberJoined = DateTime.Now;
                    CurrentMember.MemberUserID = userId;
                    CurrentMember.MemberComment = txtComment.Text;

                    if (chkApprove.Checked)
                    {
                        // Approve member
                        CurrentMember.MemberStatus = GroupMemberStatus.Approved;
                        CurrentMember.MemberApprovedWhen = DateTime.Now;
                        CurrentMember.MemberApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                    }
                    else
                    {
                        CurrentMember.MemberStatus = GroupMemberStatus.WaitingForApproval;
                        CurrentMember.MemberApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                    }

                    // Save member to database
                    GroupMemberInfoProvider.SetGroupMemberInfo(CurrentMember);
                    GroupInfo group = GroupInfoProvider.GetGroupInfo(GroupID);
                    if (group != null)
                    {
                        // Send notification email
                        if ((chkApprove.Checked) && (group.GroupSendWaitingForApprovalNotification))
                        {
                            GroupMemberInfoProvider.SendNotificationMail("Groups.MemberJoinedConfirmation", SiteContext.CurrentSiteName, CurrentMember, false);
                        }
                        else
                        {
                            if (group.GroupSendWaitingForApprovalNotification)
                            {
                                GroupMemberInfoProvider.SendNotificationMail("Groups.MemberJoinedWaitingForApproval ", SiteContext.CurrentSiteName, CurrentMember, false);
                            }
                        }
                    }

                    // Save user roles
                    SaveRoles(userId);

                    MemberID = CurrentMember.MemberID;
                    return true;
                }
            }
        }

        return false;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets roles for current user.
    /// </summary>
    private void LoadCurrentRoles()
    {
        if (CurrentMember != null)
        {
            // Get user roles
            var data = UserRoleInfo.Provider.Get().Where("UserID = " + CurrentMember.MemberUserID + "AND RoleID IN (SELECT RoleID FROM CMS_Role WHERE RoleGroupID = " + CurrentMember.MemberGroupID + ")").Columns("RoleID");
            if (data.Any())
            {
                currentValues = TextHelper.Join(";", DataHelper.GetStringValues(data.Tables[0], "RoleID"));
            }

            currentRolesLoaded = true;
        }
    }


    /// <summary>
    /// Creates where condition for unigrid with roles.
    /// </summary>
    private string CreateWhereCondition()
    {
        string where;

        if ((CurrentMember != null) && (CurrentMember.MemberGroupID > 0))
        {
            // Member
            where = "(RoleGroupID = " + CurrentMember.MemberGroupID + ")";
        }
        else
        {
            // Group
            where = "(RoleGroupID = " + GroupID + ")";
        }

        return where;
    }


    /// <summary>
    /// Initializes the contols in the form.
    /// </summary>
    private void InitializeForm()
    {
        newItem = (MemberID == 0);

        // Intialize UI
        plcEdit.Visible = !newItem;
        plcNew.Visible = newItem;
        userSelector.Visible = newItem;
        btnApprove.Visible = !newItem;
        btnReject.Visible = !newItem;
        lblFullName.Visible = !newItem;
        headRoles.ResourceString = "group.member.memberinroles";

        if (newItem)
        {
            headRoles.ResourceString = "group.member.addmemberinroles";
        }

        // Get strings
        lblFullNameLabel.Text = GetString("general.user") + ResHelper.Colon;
        lblComment.Text = GetString("group.member.comment") + ResHelper.Colon;
        lblMemberApprovedLabel.Text = GetString("group.member.approved") + ResHelper.Colon;
        lblMemberApprove.Text = GetString("general.approve") + ResHelper.Colon;
        lblMemberRejectedLabel.Text = GetString("group.member.rejected") + ResHelper.Colon;
        lblMemberJoinedLabel.Text = GetString("group.member.joined") + ResHelper.Colon;

        ClearForm();

        // Handle existing Group editing - prepare the data
        if (MemberID > 0)
        {
            HandleExistingMember(CurrentMember);
        }
    }


    /// <summary>
    /// Fills the data into form for specified Group member.
    /// </summary>
    private void HandleExistingMember(GroupMemberInfo groupMemberInfo)
    {
        if (groupMemberInfo != null)
        {
            // Fill controls with data from existing user
            int userId = ValidationHelper.GetInteger(groupMemberInfo.MemberUserID, 0);
            UserInfo ui = UserInfo.Provider.Get(userId);
            if (ui != null)
            {
                lblFullName.Text = HTMLHelper.HTMLEncode(ui.FullName);
            }

            txtComment.Text = groupMemberInfo.MemberComment;

            string approved = GetApprovalInfoText(groupMemberInfo.MemberApprovedWhen, groupMemberInfo.MemberApprovedByUserID);
            if (String.IsNullOrWhiteSpace(approved))
            {
                rowApproved.Visible = false;
            }
            else
            {
                lblMemberApproved.Text = approved;
                rowApproved.Visible = true;
            }

            string rejected = GetApprovalInfoText(groupMemberInfo.MemberRejectedWhen, groupMemberInfo.MemberApprovedByUserID);
            if (String.IsNullOrWhiteSpace(rejected))
            {
                rowRejected.Visible = false;
            }
            else
            {
                lblMemberRejected.Text = rejected;
                rowRejected.Visible = true;
            }

            lblMemberJoined.Text = TimeZoneUIMethods.ConvertDateTime(groupMemberInfo.MemberJoined, this).ToString();
        }
    }


    /// <summary>
    /// Returns the approval text in format "date (approved by user full name)".
    /// </summary>
    /// <param name="date">Date time</param>
    /// <param name="userId">User id</param>
    private string GetApprovalInfoText(DateTime date, int userId)
    {
        string retval = "";

        if (date != DateTimeHelper.ZERO_TIME)
        {
            // Get current time
            retval = TimeZoneUIMethods.ConvertDateTime(date, this).ToString();

            UserInfo ui = UserInfo.Provider.Get(userId);
            if (ui != null)
            {
                // Add user's full name
                retval += " (" + HTMLHelper.HTMLEncode(ui.FullName) + ")";
            }
        }
        return retval;
    }


    /// <summary>
    /// Saves roles of specified user.
    /// </summary>    
    private void SaveRoles(int userID)
    {
        // Load user's roles
        if (!currentRolesLoaded)
        {
            LoadCurrentRoles();
        }

        // Remove old items
        string newValues = ValidationHelper.GetString(usRoles.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            var newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Removes relationship between user and role
            foreach (string item in newItems)
            {
                int roleID = ValidationHelper.GetInteger(item, 0);

                var uri = UserRoleInfo.Provider.Get(userID, roleID);
                UserRoleInfo.Provider.Delete(uri);
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            var newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add relationship between user and role
            foreach (string item in newItems)
            {
                int roleID = ValidationHelper.GetInteger(item, 0);
                UserRoleInfo.Provider.Add(userID, roleID);
            }
        }
    }


    /// <summary>
    /// Handles the OnSelectionChanged event of the usRoles control.
    /// Saves the user roles when they are changed.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
    private void usRoles_OnSelectionChanged(object sender, EventArgs e)
    {
        // Check MANAGE permission for groups module
        if (!CheckPermissions("cms.groups", PERMISSION_MANAGE, GroupID))
        {
            return;
        }

        if (CurrentMember != null)
        {
            UserInfo ui = UserInfo.Provider.Get(CurrentMember.MemberUserID);
            if (ui != null)
            {
                // Save user roles
                SaveRoles(ui.UserID);
            }
        }
    }

    #endregion
}