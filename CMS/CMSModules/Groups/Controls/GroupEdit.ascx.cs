using System;
using System.Data;

using CMS.Activities;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Community;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_GroupEdit : CMSAdminEditControl
{
    #region "Variables"

    private int mGroupId = 0;
    private int mSiteId = 0;
    private bool mHideWhenGroupIsNotSupplied = false;
    private bool mDisplayAdvanceOptions = false;
    private GroupInfo groupInfo = null;
    private bool mAllowChangeGroupDisplayName = false;
    private bool mAllowSelectTheme = false;

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
    /// Determines whether to hide the content of the control when GroupID is not supplied.
    /// </summary>
    public bool HideWhenGroupIsNotSupplied
    {
        get
        {
            return mHideWhenGroupIsNotSupplied;
        }
        set
        {
            mHideWhenGroupIsNotSupplied = value;
        }
    }


    /// <summary>
    /// Current group ID.
    /// </summary>
    public int GroupID
    {
        get
        {
            if (mGroupId <= 0)
            {
                mGroupId = ValidationHelper.GetInteger(GetValue("GroupID"), 0);
            }

            return mGroupId;
        }
        set
        {
            mGroupId = value;
        }
    }


    /// <summary>
    /// If true changing theme for group page is enabled.
    /// </summary>
    public bool AllowSelectTheme
    {
        get
        {
            return mAllowSelectTheme;
        }
        set
        {
            mAllowSelectTheme = value;
        }
    }


    /// <summary>
    /// If true group display name change allowed on live site.
    /// </summary>
    public bool AllowChangeGroupDisplayName
    {
        get
        {
            return mAllowChangeGroupDisplayName;
        }
        set
        {
            mAllowChangeGroupDisplayName = value;
        }
    }


    /// <summary>
    /// Current site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId <= 0)
            {
                mSiteId = ValidationHelper.GetInteger(GetValue("SiteID"), 0);
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether should be displayed advance fields like codename display name
    /// or document selector
    /// </summary>
    public bool DisplayAdvanceOptions
    {
        get
        {
            return mDisplayAdvanceOptions;
        }
        set
        {
            mDisplayAdvanceOptions = value;
            plcGroupLocation.Visible = value;
            plcAdvanceOptions.Visible = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Visible)
        {
            EnableViewState = false;
        }

        if (StopProcessing)
        {
            // Do nothing
            Visible = false;
            groupPictureEdit.StopProcessing = true;
            groupPageURLElem.StopProcessing = true;
        }
        else
        {
            string currSiteName = SiteContext.CurrentSiteName;
            plcGroupLocation.Visible = DisplayAdvanceOptions;
            plcAdvanceOptions.Visible = DisplayAdvanceOptions;
            groupPageURLElem.EnableSiteSelection = false;
            plcOnline.Visible = ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(currSiteName);

            ctrlSiteSelectStyleSheet.CurrentSelector.ReturnColumnName = "StyleSheetID";
            ctrlSiteSelectStyleSheet.SiteId = SiteContext.CurrentSiteID;
            ctrlSiteSelectStyleSheet.AllowEditButtons = false;
            ctrlSiteSelectStyleSheet.IsLiveSite = IsLiveSite;
            lblStyleSheetName.AssociatedControlClientID = ctrlSiteSelectStyleSheet.CurrentSelector.ClientID;

            // Is allow edit display name is set on live site
            if ((AllowChangeGroupDisplayName) && (IsLiveSite))
            {
                if (!plcAdvanceOptions.Visible)
                {
                    plcCodeName.Visible = false;
                }
                plcAdvanceOptions.Visible = true;
            }

            // Web parts theme selector visibility
            if ((!AllowSelectTheme) && (IsLiveSite))
            {
                plcStyleSheetSelector.Visible = false;
            }

            RaiseOnCheckPermissions(PERMISSION_READ, this);

            if (StopProcessing)
            {
                return;
            }

            if ((GroupID == 0) && HideWhenGroupIsNotSupplied)
            {
                Visible = false;
                return;
            }

            InitializeForm();

            groupInfo = GroupInfoProvider.GetGroupInfo(GroupID);
            if (groupInfo != null)
            {
                if (!RequestHelper.IsPostBack())
                {
                    // Handle existing Group editing - prepare the data
                    if (GroupID > 0)
                    {
                        HandleExistingGroup();
                    }
                }

                groupPictureEdit.GroupInfo = groupInfo;

                // UI Tools theme selector visibility
                if ((!IsLiveSite) && (groupInfo.GroupNodeGUID == Guid.Empty))
                {
                    plcStyleSheetSelector.Visible = false;
                }

                // Init theme selector
                if (plcStyleSheetSelector.Visible)
                {
                    if (!RequestHelper.IsPostBack())
                    {
                        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                        if (groupInfo.GroupNodeGUID != Guid.Empty)
                        {
                            TreeNode node = tree.SelectSingleNode(groupInfo.GroupNodeGUID, TreeProvider.ALL_CULTURES, SiteContext.CurrentSiteName);
                            if (node != null)
                            {
                                ctrlSiteSelectStyleSheet.Value = node.DocumentStylesheetID;
                            }
                        }
                    }
                }
            }
            else
            {
                plcStyleSheetSelector.Visible = false;
            }

            txtDescription.IsLiveSite = IsLiveSite;
            groupPictureEdit.IsLiveSite = IsLiveSite;
            groupPageURLElem.IsLiveSite = IsLiveSite;
        }
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        SaveData();
    }


    #region "Public methods"

    /// <summary>
    /// Sets the property value of the control, setting the value affects only local property value.
    /// </summary>
    /// <param name="propertyName">Property name to set</param>
    /// <param name="value">New property value</param>
    public override bool SetValue(string propertyName, object value)
    {
        // Allow change group display name
        if (CMSString.Compare(propertyName, "AllowChangeGroupDisplayName", true) == 0)
        {
            AllowChangeGroupDisplayName = ValidationHelper.GetBoolean(value, false);
        }

        // Allow change theme of group page
        if (CMSString.Compare(propertyName, "AllowSelectTheme", true) == 0)
        {
            AllowSelectTheme = ValidationHelper.GetBoolean(value, false);
        }

        // Call base method
        return base.SetValue(propertyName, value);
    }


    /// <summary>
    /// Updates the current Group or creates new if no GroupID is present.
    /// </summary>
    public void SaveData()
    {
        if (!CheckPermissions("cms.groups", PERMISSION_MANAGE, GroupID))
        {
            return;
        }

        // Validate form entries
        string errorMessage = ValidateForm();
        if (!String.IsNullOrEmpty(errorMessage))
        {
            // Display error message
            ShowError(errorMessage);
            return;
        }

        try
        {
            bool newGroup = false;

            // Update existing item
            if (groupInfo == null)
            {
                groupInfo = new GroupInfo();
                newGroup = true;
            }

            // Trim display name and code name
            string displayName = txtDisplayName.Text.Trim();
            string codeName = txtCodeName.Text.Trim();

            if (displayName != groupInfo.GroupDisplayName)
            {
                // Refresh a breadcrumb if used in the tabs layout
                ScriptHelper.RefreshTabHeader(Page, displayName);
            }

            if (DisplayAdvanceOptions)
            {
                // Update Group fields
                groupInfo.GroupDisplayName = displayName;
                groupInfo.GroupName = codeName;
                groupInfo.GroupNodeGUID = ValidationHelper.GetGuid(groupPageURLElem.Value, Guid.Empty);
            }

            if (AllowChangeGroupDisplayName && IsLiveSite)
            {
                groupInfo.GroupDisplayName = displayName;
            }

            groupInfo.GroupDescription = txtDescription.Text;
            groupInfo.GroupAccess = GetGroupAccess();
            groupInfo.GroupSiteID = SiteID;
            groupInfo.GroupApproveMembers = GetGroupApproveMembers();
            groupInfo.GroupSendJoinLeaveNotification = chkJoinLeave.Checked;
            groupInfo.GroupSendWaitingForApprovalNotification = chkWaitingForApproval.Checked;
            groupPictureEdit.UpdateGroupPicture(groupInfo);

            // If new group was created 
            if (newGroup)
            {
                // Set columns GroupCreatedByUserID and GroupApprovedByUserID to current user
                var user = MembershipContext.AuthenticatedUser;
                if (user != null)
                {
                    groupInfo.GroupCreatedByUserID = user.UserID;
                    groupInfo.GroupApprovedByUserID = user.UserID;
                    groupInfo.GroupApproved = true;
                }
            }

            if (!IsLiveSite && (groupInfo.GroupNodeGUID == Guid.Empty))
            {
                plcStyleSheetSelector.Visible = false;
            }

            // Save theme 
            int selectedSheetID = ValidationHelper.GetInteger(ctrlSiteSelectStyleSheet.Value, 0);
            if (plcStyleSheetSelector.Visible)
            {
                if (groupInfo.GroupNodeGUID != Guid.Empty)
                {
                    // Save theme for every site culture                            
                    var cultures = CultureSiteInfoProvider.GetSiteCultureCodes(SiteContext.CurrentSiteName);
                    if (cultures != null)
                    {
                        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

                        // Return class name of selected tree node
                        TreeNode treeNode = tree.SelectSingleNode(groupInfo.GroupNodeGUID, TreeProvider.ALL_CULTURES, SiteContext.CurrentSiteName);
                        if (treeNode != null)
                        {
                            // Return all culture version of node 
                            DataSet ds = tree.SelectNodes(SiteContext.CurrentSiteName, null, TreeProvider.ALL_CULTURES, false, treeNode.NodeClassName, "NodeGUID ='" + groupInfo.GroupNodeGUID + "'", String.Empty, -1, false);
                            if (!DataHelper.DataSourceIsEmpty(ds))
                            {
                                // Loop through all nodes
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    // Create node and set tree provider for user validation
                                    TreeNode node = TreeNode.New(ValidationHelper.GetString(dr["className"], String.Empty), dr);
                                    node.TreeProvider = tree;

                                    // Update stylesheet id if set
                                    if (selectedSheetID == 0)
                                    {
                                        node.DocumentStylesheetID = 0;
                                        node.DocumentInheritsStylesheet = true;
                                    }
                                    else
                                    {
                                        node.DocumentStylesheetID = selectedSheetID;
                                    }

                                    node.Update();
                                }
                            }
                        }
                    }
                }
            }

            if (!IsLiveSite && (groupInfo.GroupNodeGUID != Guid.Empty))
            {
                plcStyleSheetSelector.Visible = true;
            }

            if (plcOnline.Visible)
            {
                // On-line marketing setting is visible => set flag according to checkbox
                groupInfo.GroupLogActivity = chkLogActivity.Checked;
            }
            else
            {
                // On-line marketing setting is not visible => set flag to TRUE as default value
                groupInfo.GroupLogActivity = true;
            }

            // Save Group in the database
            GroupInfoProvider.SetGroupInfo(groupInfo);
            groupPictureEdit.GroupInfo = groupInfo;

            txtDisplayName.Text = groupInfo.GroupDisplayName;
            txtCodeName.Text = groupInfo.GroupName;

            // Flush cached information
            DocumentContext.CurrentDocument = null;
            DocumentContext.CurrentPageInfo = null;

            // Display information on success
            ShowChangesSaved();

            // If new group was created 
            if (newGroup)
            {
                GroupID = groupInfo.GroupID;
                RaiseOnSaved();
            }
        }
        catch (Exception ex)
        {
            // Display error message
            ShowError(GetString("general.saveerror"), ex.Message, null);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the controls in the form.
    /// </summary>
    private void InitializeForm()
    {
        // Initialize errors
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvCodeName.ErrorMessage = GetString("general.requirescodename");

        // Initialize buttons
        btnSave.Text = GetString("general.ok");
    }


    /// <summary>
    /// Returns correct number according to radiobutton selection.
    /// </summary>
    private SecurityAccessEnum GetGroupAccess()
    {
        if (radSiteMembers.Checked)
        {
            return SecurityAccessEnum.AuthenticatedUsers;
        }
        else if (radGroupMembers.Checked)
        {
            return SecurityAccessEnum.GroupMembers;
        }
        else
        {
            return SecurityAccessEnum.AllUsers;
        }
    }


    /// <summary>
    /// Returns correct number according to radiobutton selection.
    /// </summary>
    private GroupApproveMembersEnum GetGroupApproveMembers()
    {
        if (radMembersApproved.Checked)
        {
            return GroupApproveMembersEnum.ApprovedCanJoin;
        }
        else if (radMembersInvited.Checked)
        {
            return GroupApproveMembersEnum.InvitedWithoutApproval;
        }
        else
        {
            return GroupApproveMembersEnum.AnyoneCanJoin;
        }
    }


    /// <summary>
    /// Fills the data into form for specified Group.
    /// </summary>
    private void HandleExistingGroup()
    {
        if (groupInfo != null)
        {
            txtDisplayName.Text = groupInfo.GroupDisplayName;
            txtCodeName.Text = groupInfo.GroupName;
            txtDescription.Text = groupInfo.GroupDescription;
            if (DisplayAdvanceOptions)
            {
                groupPageURLElem.Value = groupInfo.GroupNodeGUID.ToString();
            }
            chkJoinLeave.Checked = groupInfo.GroupSendJoinLeaveNotification;
            chkWaitingForApproval.Checked = groupInfo.GroupSendWaitingForApprovalNotification;

            UserInfo ui = null;

            // Display created by user name
            if (groupInfo.GroupCreatedByUserID != 0)
            {
                plcCreatedBy.Visible = true;

                ui = UserInfoProvider.GetUserInfo(groupInfo.GroupCreatedByUserID);
                if (ui != null)
                {
                    lblCreatedByValue.Text = HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(ui.UserName, IsLiveSite));
                }
            }

            // Display approved by user name
            if (groupInfo.GroupApprovedByUserID != 0)
            {
                plcApprovedBy.Visible = true;

                if (groupInfo.GroupApprovedByUserID != groupInfo.GroupCreatedByUserID)
                {
                    ui = UserInfoProvider.GetUserInfo(groupInfo.GroupApprovedByUserID);
                }

                if (ui != null)
                {
                    lblApprovedByValue.Text = HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(ui.UserName, IsLiveSite));
                }
            }

            switch (groupInfo.GroupAccess)
            {
                case SecurityAccessEnum.AllUsers:
                    radAnybody.Checked = true;
                    break;

                case SecurityAccessEnum.AuthenticatedUsers:
                    radSiteMembers.Checked = true;
                    break;

                case SecurityAccessEnum.GroupMembers:
                    radGroupMembers.Checked = true;
                    break;
            }

            switch (groupInfo.GroupApproveMembers)
            {
                case GroupApproveMembersEnum.AnyoneCanJoin:
                    radMembersAny.Checked = true;
                    break;

                case GroupApproveMembersEnum.ApprovedCanJoin:
                    radMembersApproved.Checked = true;
                    break;

                case GroupApproveMembersEnum.InvitedWithoutApproval:
                    radMembersInvited.Checked = true;
                    break;
            }

            chkLogActivity.Checked = groupInfo.GroupLogActivity;
        }
    }


    /// <summary>
    /// Validates the form entries.
    /// </summary>
    private string ValidateForm()
    {
        // Validate file input
        if (!groupPictureEdit.IsValid())
        {
            return groupPictureEdit.ErrorMessage;
        }

        return null;
    }

    #endregion
}
