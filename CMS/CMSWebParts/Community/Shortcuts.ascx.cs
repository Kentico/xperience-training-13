using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Community;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_Community_Shortcuts : CMSAbstractWebPart
{
    #region "Variables"

    /// <summary>
    /// Current user info.
    /// </summary>
    protected CurrentUserInfo currentUser = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets property to mark if Sign in link should be displayed.
    /// </summary>
    public bool DisplaySignIn
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplaySignIn"), true);
        }
        set
        {
            SetValue("DisplaySignIn", value);
        }
    }


    /// <summary>
    /// Gets or sets path to be redirected to when user clicks SignIn link.
    /// </summary>
    public string SignInPath
    {
        get
        {
            // Get path from path selector
            string signInPath = ValidationHelper.GetString(GetValue("SignInPath"), "");

            // If empty then use default logon page from settings
            if (signInPath == "")
            {
                signInPath = ResolveUrl(AuthenticationHelper.DEFAULT_LOGON_PAGE);
            }
            else
            {
                signInPath = GetUrl(signInPath);
            }

            return signInPath;
        }
        set
        {
            SetValue("SignInPath", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the SignIn link.
    /// </summary>
    public string SignInText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SignInText"), GetString("webparts_membership_signoutbutton.signin"));
        }
        set
        {
            SetValue("SignInText", value);
            lnkSignIn.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets property to mark if Join the community link should be displayed.
    /// </summary>
    public bool DisplayJoinCommunity
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayJoinCommunity"), true);
        }
        set
        {
            SetValue("DisplayJoinCommunity", value);
        }
    }


    /// <summary>
    /// Gets or sets path to be redirected to when user clicks Join the community link.
    /// </summary>
    public string JoinCommunityPath
    {
        get
        {
            // Get path from path selector
            return ValidationHelper.GetString(GetValue("JoinCommunityPath"), "");
        }
        set
        {
            SetValue("JoinCommunityPath", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the join community link.
    /// </summary>
    public string JoinCommunityText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("JoinCommunityText"), GetString("group.member.join"));
        }
        set
        {
            SetValue("JoinCommunityText", value);
            lnkJoinCommunity.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets property to mark if My profile link should be displayed.
    /// </summary>
    public bool DisplayMyProfileLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyProfileLink"), true);
        }
        set
        {
            SetValue("DisplayMyProfileLink", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the my profile link.
    /// </summary>
    public string MyProfileText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("MyProfileText"), null), GetString("shortcuts.myprofile"));
        }
        set
        {
            SetValue("MyProfileText", value);
            lnkMyProfile.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets property to mark if Edit my profile link should be displayed.
    /// </summary>
    public bool DisplayEditMyProfileLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayEditMyProfileLink"), true);
        }
        set
        {
            SetValue("DisplayEditMyProfileLink", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the edit my profile link.
    /// </summary>
    public string EditMyProfileText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("EditMyProfileText"), null), GetString("shortcuts.editmyprofile"));
        }
        set
        {
            SetValue("EditMyProfileText", value);
            lnkEditMyProfile.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets property to mark if Sign out link should be displayed.
    /// </summary>
    public bool DisplaySignOut
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplaySignOut"), true);
        }
        set
        {
            SetValue("DisplaySignOut", value);
        }
    }


    /// <summary>
    /// Gets or sets path to be redirected to when user clicks Sign out link.
    /// </summary>
    public string SignOutPath
    {
        get
        {
            // Get path from path selector
            return ValidationHelper.GetString(GetValue("SignOutPath"), "");
        }
        set
        {
            SetValue("SignOutPath", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the SignOut link.
    /// </summary>
    public string SignOutText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SignOutText"), null), GetString("signoutbutton.signout"));
        }
        set
        {
            SetValue("SignOutText", value);
            btnSignOut.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets property to mark if Create new group link should be displayed.
    /// </summary>
    public bool DisplayCreateNewGroup
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayCreateNewGroup"), true);
        }
        set
        {
            SetValue("DisplayCreateNewGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets path to be redirected to when user clicks Create new group link.
    /// </summary>
    public string CreateNewGroupPath
    {
        get
        {
            // Get path from path selector
            return ValidationHelper.GetString(GetValue("CreateNewGroupPath"), "");
        }
        set
        {
            SetValue("CreateNewGroupPath", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the create new group link.
    /// </summary>
    public string CreateNewGroupText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CreateNewGroupText"), null), GetString("group.creategroup"));
        }
        set
        {
            SetValue("CreateNewGroupText", value);
            lnkCreateNewGroup.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets property to mark if Create new blok link should be displayed.
    /// </summary>
    public bool DisplayCreateNewBlog
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayCreateNewBlog"), true);
        }
        set
        {
            SetValue("DisplayCreateNewBlog", value);
        }
    }


    /// <summary>
    /// Gets or sets path to be redirected to when user clicks Create new blog link.
    /// </summary>
    public string CreateNewBlogPath
    {
        get
        {
            // Get path from path selector
            return ValidationHelper.GetString(GetValue("CreateNewBlogPath"), "");
        }
        set
        {
            SetValue("CreateNewBlogPath", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the create new blog link.
    /// </summary>
    public string CreateNewBlogText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CreateNewBlogText"), null), GetString("blog.createblog"));
        }
        set
        {
            SetValue("CreateNewBlogText", value);
            lnkCreateNewBlog.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets property to mark if Join/Leave the group link should be displayed.
    /// </summary>
    public bool DisplayGroupLinks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayGroupLinks"), true);
        }
        set
        {
            SetValue("DisplayGroupLinks", value);
        }
    }


    /// <summary>
    /// Gets or sets path to be redirected to when user clicks Join the group link.
    /// </summary>
    public string JoinGroupPath
    {
        get
        {
            // Get path from path selector
            return ValidationHelper.GetString(GetValue("JoinGroupPath"), "");
        }
        set
        {
            SetValue("JoinGroupPath", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the join group link.
    /// </summary>
    public string JoinGroupText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("JoinGroupText"), null), GetString("group.joingroup"));
        }
        set
        {
            SetValue("JoinGroupText", value);
            lnkJoinGroup.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets path to be redirected to when user clicks Leave the group link.
    /// </summary>
    public string LeaveGroupPath
    {
        get
        {
            // Get path from path selector
            return ValidationHelper.GetString(GetValue("LeaveGroupPath"), "");
        }
        set
        {
            SetValue("LeaveGroupPath", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the leave group link.
    /// </summary>
    public string LeaveGroupText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("LeaveGroupText"), null), GetString("group.leavegroup"));
        }
        set
        {
            SetValue("LeaveGroupText", value);
            lnkLeaveGroup.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets property to mark if Invite to group link should be displayed.
    /// </summary>
    public bool DisplayInviteToGroup
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayInviteToGroup"), true);
        }
        set
        {
            SetValue("DisplayInviteToGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets path to be redirected to when user clicks Invite to the group link.
    /// </summary>
    public string InviteGroupPath
    {
        get
        {
            // Get path from path selector
            return ValidationHelper.GetString(GetValue("InviteGroupPath"), string.Empty);
        }
        set
        {
            SetValue("InviteGroupPath", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the ivite to group link.
    /// </summary>
    public string InviteGroupText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("InviteGroupText"), null), GetString("groupinvitation.invite"));
        }
        set
        {
            SetValue("InviteGroupText", value);
            lnkInviteToGroup.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets property to mark if Manage the group should be displayed.
    /// </summary>
    public bool DisplayManageGroup
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayManageGroup"), true);
        }
        set
        {
            SetValue("DisplayManageGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the manage group link.
    /// </summary>
    public string ManageGroupText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ManageGroupText"), null), GetString("group.manage"));
        }
        set
        {
            SetValue("ManageGroupText", value);
            lnkManageGroup.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets property to mark if My invitations link should be displayed.
    /// </summary>
    public bool DisplayMyInvitations
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyInvitations"), true);
        }
        set
        {
            SetValue("DisplayMyInvitations", value);
        }
    }


    /// <summary>
    /// Gets or sets path to be redirected to when user clicks My messages link.
    /// </summary>
    public string MyInvitationsPath
    {
        get
        {
            // Get path from path selector
            return ValidationHelper.GetString(GetValue("MyInvitationsPath"), string.Empty);
        }
        set
        {
            SetValue("MyInvitationsPath", value);
        }
    }


    /// <summary>
    /// Gets or sets text for the my invitations link.
    /// </summary>
    public string MyInvitationsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("MyInvitationsText"), null), GetString("shortcuts.myinvitations"));
        }
        set
        {
            SetValue("MyInvitationsText", value);
            lnkMyInvitations.Text = value;
        }
    }

    #endregion


    #region "Events and methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Initialize properties
            string script = "";

            // Set current user
            currentUser = MembershipContext.AuthenticatedUser;

            // Initialize strings
            lnkSignIn.Text = SignInText;
            lnkJoinCommunity.Text = JoinCommunityText;
            lnkMyProfile.Text = MyProfileText;
            lnkEditMyProfile.Text = EditMyProfileText;
            btnSignOut.Text = SignOutText;
            lnkCreateNewGroup.Text = CreateNewGroupText;
            lnkCreateNewBlog.Text = CreateNewBlogText;
            lnkJoinGroup.Text = JoinGroupText;
            lnkLeaveGroup.Text = LeaveGroupText;
            lnkInviteToGroup.Text = InviteGroupText;
            lnkManageGroup.Text = ManageGroupText;
            lnkMyInvitations.Text = MyInvitationsText;

            // If current user is public...
            if (currentUser.IsPublic())
            {
                // Display Sign In link if set so
                if (DisplaySignIn)
                {
                    // SignInPath returns URL - because of settings value
                    lnkSignIn.NavigateUrl = MacroResolver.ResolveCurrentPath(SignInPath);
                    pnlSignIn.Visible = true;
                    pnlSignInOut.Visible = true;
                }

                // Display Join the community link if set so
                if (DisplayJoinCommunity)
                {
                    lnkJoinCommunity.NavigateUrl = GetUrl(JoinCommunityPath);
                    pnlJoinCommunity.Visible = true;
                    pnlPersonalLinks.Visible = true;
                }
            }
            // If user is logged in
            else
            {
                // Display Sign out link if set so
                if (DisplaySignOut && !AuthenticationMode.IsWindowsAuthentication())
                {
                    pnlSignOut.Visible = true;
                    pnlSignInOut.Visible = true;
                }

                // Display Edit my profile link if set so
                if (DisplayEditMyProfileLink)
                {
                    lnkEditMyProfile.NavigateUrl = "";
                    pnlEditMyProfile.Visible = true;
                    pnlProfileLinks.Visible = true;
                }

                // Display My profile link if set so
                if (DisplayMyProfileLink)
                {
                    lnkMyProfile.NavigateUrl = "";
                    pnlMyProfile.Visible = true;
                    pnlProfileLinks.Visible = true;
                }

                // Display Create new group link if set so
                if (DisplayCreateNewGroup)
                {
                    lnkCreateNewGroup.NavigateUrl = GetUrl(CreateNewGroupPath);
                    pnlCreateNewGroup.Visible = true;
                    pnlGroupLinks.Visible = true;
                }

                // Display Create new blog link if set so
                if (DisplayCreateNewBlog)
                {
                    // Check that Community Module is present
                    var entry = ModuleManager.GetModule(ModuleName.BLOGS);
                    if (entry != null)
                    {
                        lnkCreateNewBlog.NavigateUrl = GetUrl(CreateNewBlogPath);
                        pnlCreateNewBlog.Visible = true;
                        pnlBlogLinks.Visible = true;
                    }
                }

                // Display My invitations link
                if (DisplayMyInvitations)
                {
                    lnkMyInvitations.NavigateUrl = GetUrl(MyInvitationsPath);
                    pnlMyInvitations.Visible = true;
                    pnlPersonalLinks.Visible = true;
                }

                GroupMemberInfo gmi = null;

                if (CommunityContext.CurrentGroup != null)
                {
                    // Get group info from community context
                    GroupInfo currentGroup = CommunityContext.CurrentGroup;

                    if (DisplayGroupLinks)
                    {
                        // Display Join group link if set so and user is visiting a group page
                        gmi = GetGroupMember(MembershipContext.AuthenticatedUser.UserID, currentGroup.GroupID);
                        if (gmi == null)
                        {
                            if (String.IsNullOrEmpty(JoinGroupPath))
                            {
                                script += "function JoinToGroupRequest() {\n" +
                                          "modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Groups/CMSPages/JoinTheGroup.aspx") + "?groupid=" + currentGroup.GroupID + "','requestJoinToGroup', 500, 180); \n" +
                                          " } \n";

                                lnkJoinGroup.Attributes.Add("onclick", "JoinToGroupRequest();return false;");
                                lnkJoinGroup.NavigateUrl = RequestContext.CurrentURL;
                            }
                            else
                            {
                                lnkJoinGroup.NavigateUrl = GetUrl(JoinGroupPath);
                            }
                            pnlJoinGroup.Visible = true;
                            pnlGroupLinks.Visible = true;
                        }
                        else if ((gmi.MemberStatus == GroupMemberStatus.Approved) || (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)))
                        // Display Leave the group link if user is the group member
                        {
                            if (String.IsNullOrEmpty(LeaveGroupPath))
                            {
                                script += "function LeaveTheGroupRequest() {\n" +
                                          "modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Groups/CMSPages/LeaveTheGroup.aspx") + "?groupid=" + currentGroup.GroupID + "','requestLeaveThGroup', 500, 180); \n" +
                                          " } \n";

                                lnkLeaveGroup.Attributes.Add("onclick", "LeaveTheGroupRequest();return false;");
                                lnkLeaveGroup.NavigateUrl = RequestContext.CurrentURL;
                            }
                            else
                            {
                                lnkLeaveGroup.NavigateUrl = GetUrl(LeaveGroupPath);
                            }

                            pnlLeaveGroup.Visible = true;
                            pnlGroupLinks.Visible = true;
                        }
                    }

                    // Display Manage the group link if set so and user is logged as group administrator and user is visiting a group page
                    if (DisplayManageGroup && (currentUser.IsGroupAdministrator(currentGroup.GroupID) || (currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))))
                    {
                        lnkManageGroup.NavigateUrl = "";
                        pnlManageGroup.Visible = true;
                        pnlGroupLinks.Visible = true;
                    }
                }

                // Get user info from site context
                UserInfo siteContextUser = MembershipContext.CurrentUserProfile;

                if (DisplayInviteToGroup)
                {
                    // Get group info from community context
                    GroupInfo currentGroup = CommunityContext.CurrentGroup;

                    // Display invite to group link for user who is visiting a group page
                    if (currentGroup != null)
                    {
                        // Get group user
                        if (gmi == null)
                        {
                            gmi = GetGroupMember(MembershipContext.AuthenticatedUser.UserID, currentGroup.GroupID);
                        }

                        if (((gmi != null) && (gmi.MemberStatus == GroupMemberStatus.Approved)) || (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)))
                        {
                            pnlInviteToGroup.Visible = true;

                            if (String.IsNullOrEmpty(InviteGroupPath))
                            {
                                script += "function InviteToGroup() {\n modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Groups/CMSPages/InviteToGroup.aspx") + "?groupid=" + currentGroup.GroupID + "','inviteToGroup', 800, 450); \n } \n";
                                lnkInviteToGroup.Attributes.Add("onclick", "InviteToGroup();return false;");
                                lnkInviteToGroup.NavigateUrl = RequestContext.CurrentURL;
                            }
                            else
                            {
                                lnkInviteToGroup.NavigateUrl = GetUrl(InviteGroupPath);
                            }
                        }
                    }
                    // Display invite to group link for user who is visiting another user's page
                    else if ((siteContextUser != null) && (siteContextUser.UserName != currentUser.UserName) && (GroupInfoProvider.GetUserGroupsCount(currentUser, SiteContext.CurrentSite) != 0))
                    {
                        pnlInviteToGroup.Visible = true;

                        if (String.IsNullOrEmpty(InviteGroupPath))
                        {
                            script += "function InviteToGroup() {\n modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Groups/CMSPages/InviteToGroup.aspx") + "?invitedid=" + siteContextUser.UserID + "','inviteToGroup', 700, 400); \n } \n";
                            lnkInviteToGroup.Attributes.Add("onclick", "InviteToGroup();return false;");
                            lnkInviteToGroup.NavigateUrl = RequestContext.CurrentURL;
                        }
                        else
                        {
                            lnkInviteToGroup.NavigateUrl = GetUrl(InviteGroupPath);
                        }
                    }
                }
            }

            // Register menu management scripts
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Shortcuts_" + ClientID, ScriptHelper.GetScript(script));

            // Register the dialog script
            ScriptHelper.RegisterDialogScript(Page);
        }
    }


    /// <summary>
    /// SignOut click event handler.
    /// </summary>
    protected void btnSignOut_Click(object sender, EventArgs e)
    {
        if (currentUser == null)
        {
            currentUser = MembershipContext.AuthenticatedUser;
        }
        if (AuthenticationHelper.IsAuthenticated())
        {

            string redirectUrl = SignOutPath != "" ? GetUrl(SignOutPath) : RequestContext.CurrentURL;
            
            AuthenticationHelper.SignOut();

            Response.Cache.SetNoStore();
            URLHelper.Redirect(UrlResolver.ResolveUrl(redirectUrl));
        }
    }


    /// <summary>
    /// Gets URL from given path.
    /// </summary>
    /// <param name="path">Node alias path</param>
    private string GetUrl(string path)
    {
        return "";
    }


    /// <summary>
    /// Returns group member info, reult is cached in request.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="groupId">Group ID</param>
    private GroupMemberInfo GetGroupMember(int userId, int groupId)
    {
        GroupMemberInfo gmi = RequestStockHelper.GetItem("CommunityShortCuts" + userId.ToString() + "_" + groupId.ToString()) as GroupMemberInfo;
        if ((gmi == null) && (!RequestStockHelper.Contains("CommunityShortCuts" + userId.ToString() + "_" + groupId.ToString())))
        {
            gmi = GroupMemberInfoProvider.GetGroupMemberInfo(userId, groupId);
            if (gmi != null)
            {
                RequestStockHelper.Add("CommunityShortCuts" + userId.ToString() + "_" + groupId.ToString(), gmi);
            }
            else
            {
                RequestStockHelper.Add("CommunityShortCuts" + userId.ToString() + "_" + groupId.ToString(), false);
            }
        }

        return gmi;
    }

    #endregion
}
