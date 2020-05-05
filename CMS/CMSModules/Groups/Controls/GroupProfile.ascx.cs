using System;

using CMS.Base;
using CMS.Community;
using CMS.DocumentEngine.Web.UI.Configuration;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_GroupProfile : CMSAdminEditControl
{
    #region "Variables"

    private int mGroupId = 0;

    private bool mShowContentTab = true;
    private bool mShowGeneralTab = true;
    private bool mShowSecurityTab = true;
    private bool mShowMembersTab = true;
    private bool mShowRolesTab = true;
    private bool mShowForumsTab = true;
    private bool mShowMediaTab = true;
    private bool mShowMessageBoardsTab = true;
    private bool mShowPollsTab = true;

    private int generalTabIndex = 0;
    private int securityTabIndex = 0;
    // Set -1 it there is a check for a presence of a module
    private int membersTabIndex = -1;
    private int rolesTabIndex = -1;
    private int forumsTabIndex = -1;
    private int mediaTabIndex = -1;
    private int messageBoardsTabIndex = -1;
    private int pollsTabIndex = -1;

    private bool mHideWhenGroupIsNotSupplied = false;
    private GroupInfo gi = null;
    private CMSAdminControl ctrl = null;
    private bool mAllowChangeGroupDisplayName = false;
    private bool mAllowSelectTheme = false;

    #endregion


    #region "Public properties"

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
    /// Gets or sets the ID of the group to be edited.
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


    /// <summary>
    /// Gets or sets the value which determines whether to show the content tab.
    /// </summary>
    public bool ShowContentTab
    {
        get
        {
            return mShowContentTab;
        }
        set
        {
            mShowContentTab = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to show the general tab.
    /// </summary>
    public bool ShowGeneralTab
    {
        get
        {
            return mShowGeneralTab;
        }
        set
        {
            mShowGeneralTab = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to show the security tab.
    /// </summary>
    public bool ShowSecurityTab
    {
        get
        {
            return mShowSecurityTab;
        }
        set
        {
            mShowSecurityTab = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to show the members tab.
    /// </summary>
    public bool ShowMembersTab
    {
        get
        {
            return mShowMembersTab;
        }
        set
        {
            mShowMembersTab = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to show the roles tab.
    /// </summary>
    public bool ShowRolesTab
    {
        get
        {
            return mShowRolesTab;
        }
        set
        {
            mShowRolesTab = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to show the forums tab.
    /// </summary>
    public bool ShowForumsTab
    {
        get
        {
            return mShowForumsTab;
        }
        set
        {
            mShowForumsTab = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to show the media tab.
    /// </summary>
    public bool ShowMediaTab
    {
        get
        {
            return mShowMediaTab;
        }
        set
        {
            mShowMediaTab = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to show the message boards tab.
    /// </summary>
    public bool ShowMessageBoardsTab
    {
        get
        {
            return mShowMessageBoardsTab;
        }
        set
        {
            mShowMessageBoardsTab = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to show the polls tab.
    /// </summary>
    public bool ShowPollsTab
    {
        get
        {
            return mShowPollsTab;
        }
        set
        {
            mShowPollsTab = value;
        }
    }


    /// <summary>
    /// Gets or sets switch to display appropriate controls.
    /// </summary>
    public string SelectedPage
    {
        get
        {
            return ValidationHelper.GetString(ViewState[ClientID + "SelectedPage"], "");
        }
        set
        {
            ViewState[ClientID + "SelectedPage"] = (object)value;
        }
    }

    #endregion


    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        // Get page url
        string page = QueryHelper.GetText("tab", SelectedPage);
        if (!String.IsNullOrEmpty(page))
        {
            page = page.ToLowerCSafe();
        }

        // Check MANAGE permission
        if (RaiseOnCheckPermissions(PERMISSION_MANAGE, this))
        {
            if (StopProcessing)
            {
                return;
            }
        }

        if ((GroupID == 0) && HideWhenGroupIsNotSupplied)
        {
            // Hide if groupID == 0
            Visible = false;
            return;
        }

        gi = GroupInfoProvider.GetGroupInfo(GroupID);

        // If no group, display the info and return
        if (gi == null)
        {
            lblInfo.Text = GetString("group.groupprofile.nogroup");
            lblInfo.Visible = true;
            tabMenu.Visible = false;
            pnlContent.Visible = false;
            return;
        }

        // Get current URL
        string absoluteUri = RequestContext.CurrentURL;

        // Menu initialization
        tabMenu.TabControlIdPrefix = "GroupProfile";
        tabMenu.UrlTarget = "_self";       
        tabMenu.UsePostback = true;

        int i = 0;
        string defaultTab = null;
        
        #region "Show/hide tabs"

        // Show general tab
        if (ShowGeneralTab)
        {
            tabMenu.AddTab(new TabItem()
            {
                Text = GetString("General.General"),
                RedirectUrl = URLHelper.AddParameterToUrl(absoluteUri, "tab", "general"),
            });
            defaultTab = "general";
            generalTabIndex = i;
            i++;
        }

        // Show security tab
        if (ShowSecurityTab)
        {
            tabMenu.AddTab(new TabItem()
            {
                Text = GetString("General.Security"),
                RedirectUrl = URLHelper.AddParameterToUrl(absoluteUri, "tab", "security"),
            });
            if (String.IsNullOrEmpty(defaultTab))
            {
                defaultTab = "security";
            }

            securityTabIndex = i;
            i++;
        }

        // Show members tab
        if (ShowMembersTab)
        {
            tabMenu.AddTab(new TabItem()
            {
                Text = GetString("Group.Members"),
                RedirectUrl = URLHelper.AddParameterToUrl(absoluteUri, "tab", "members"),
            });
            if (String.IsNullOrEmpty(defaultTab))
            {
                defaultTab = "members";
            }

            membersTabIndex = i;
            i++;
        }

        // Show roles tab
        if (ShowRolesTab && ResourceSiteInfoProvider.IsResourceOnSite("CMS.Roles", SiteContext.CurrentSiteName))
        {
            tabMenu.AddTab(new TabItem()
            {
                Text = GetString("general.roles"),
                RedirectUrl = URLHelper.AddParameterToUrl(absoluteUri, "tab", "roles"),
            });
            if (String.IsNullOrEmpty(defaultTab))
            {
                defaultTab = "roles";
            }

            rolesTabIndex = i;
            i++;
        }

        // Show forums tab
        if (ShowForumsTab && ResourceSiteInfoProvider.IsResourceOnSite("CMS.Forums", SiteContext.CurrentSiteName))
        {
            tabMenu.AddTab(new TabItem()
            {
                Text = GetString("Group.Forums"),
                RedirectUrl = URLHelper.AddParameterToUrl(absoluteUri, "tab", "forums"),
            });
            if (String.IsNullOrEmpty(defaultTab))
            {
                defaultTab = "forums";
            }

            forumsTabIndex = i;
            i++;
        }

        // Show media tab
        if (ShowMediaTab && ResourceSiteInfoProvider.IsResourceOnSite("CMS.MediaLibrary", SiteContext.CurrentSiteName))
        {
            tabMenu.AddTab(new TabItem()
            {
                Text = GetString("Group.MediaLibrary"),
                RedirectUrl = URLHelper.AddParameterToUrl(absoluteUri, "tab", "medialibrary"),
            });
            if (String.IsNullOrEmpty(defaultTab))
            {
                defaultTab = "medialibrary";
            }

            mediaTabIndex = i;
            i++;
        }

        // Show message boards tab
        if (ShowMessageBoardsTab && ResourceSiteInfoProvider.IsResourceOnSite("CMS.MessageBoards", SiteContext.CurrentSiteName))
        {
            tabMenu.AddTab(new TabItem()
            {
                Text = GetString("Group.MessageBoards"),
                RedirectUrl = URLHelper.AddParameterToUrl(absoluteUri, "tab", "messageboards"),
            });
            if (String.IsNullOrEmpty(defaultTab))
            {
                defaultTab = "messageboards";
            }

            messageBoardsTabIndex = i;
            i++;
        }

        // Show polls tab
        if (ShowPollsTab && ResourceSiteInfoProvider.IsResourceOnSite("CMS.Polls", SiteContext.CurrentSiteName))
        {
            tabMenu.AddTab(new TabItem()
            {
                Text = GetString("Group.Polls"),
                RedirectUrl = URLHelper.AddParameterToUrl(absoluteUri, "tab", "polls"),
            });
            if (String.IsNullOrEmpty(defaultTab))
            {
                defaultTab = "polls";
            }

            pollsTabIndex = i;
            i++;
        }

        #endregion


        if (string.IsNullOrEmpty(page))
        {
            page = defaultTab;
            tabMenu.SelectedTab = 0;
        }

        // Select current page
        switch (page)
        {
            case "general":
                tabMenu.SelectedTab = generalTabIndex;

                // Show general content
                if (ShowGeneralTab)
                {
                    ctrl = this.LoadUserControl("~/CMSModules/Groups/Controls/GroupEdit.ascx") as CMSAdminControl;
                    ctrl.ID = "groupEditElem";

                    if (ctrl != null)
                    {
                        ctrl.SetValue("GroupID", gi.GroupID);
                        ctrl.SetValue("SiteID", SiteContext.CurrentSiteID);
                        ctrl.SetValue("IsLiveSite", IsLiveSite);
                        ctrl.SetValue("AllowChangeGroupDisplayName", AllowChangeGroupDisplayName);
                        ctrl.SetValue("AllowSelectTheme", AllowSelectTheme);
                        ctrl.OnCheckPermissions += new CheckPermissionsEventHandler(ctrl_OnCheckPermissions);
                        pnlContent.Controls.Add(ctrl);
                    }
                }
                break;

            case "security":
                tabMenu.SelectedTab = securityTabIndex;

                // Show security content
                if (ShowSecurityTab)
                {
                    ctrl = this.LoadUserControl("~/CMSModules/Groups/Controls/Security/GroupSecurity.ascx") as CMSAdminControl;
                    ctrl.ID = "securityElem";

                    if (ctrl != null)
                    {
                        ctrl.SetValue("GroupID", gi.GroupID);
                        ctrl.SetValue("IsLiveSite", IsLiveSite);
                        ctrl.OnCheckPermissions += new CheckPermissionsEventHandler(ctrl_OnCheckPermissions);
                        pnlContent.Controls.Add(ctrl);
                    }
                }
                break;

            case "members":
                if (membersTabIndex >= 0)
                {
                    tabMenu.SelectedTab = membersTabIndex;

                    // Show members content
                    if (ShowMembersTab)
                    {
                        ctrl = this.LoadUserControl("~/CMSModules/Groups/Controls/Members/Members.ascx") as CMSAdminControl;
                        ctrl.ID = "securityElem";

                        if (ctrl != null)
                        {
                            ctrl.SetValue("GroupID", gi.GroupID);
                            ctrl.SetValue("IsLiveSite", IsLiveSite);
                            ctrl.OnCheckPermissions += new CheckPermissionsEventHandler(ctrl_OnCheckPermissions);
                            pnlContent.Controls.Add(ctrl);
                        }
                    }
                }
                break;

            case "forums":
                if (forumsTabIndex >= 0)
                {
                    tabMenu.SelectedTab = forumsTabIndex;

                    // Show forums content
                    if (ShowForumsTab)
                    {
                        ctrl = this.LoadUserControl("~/CMSModules/Forums/Controls/LiveControls/Groups.ascx") as CMSAdminControl;
                        ctrl.ID = "forumElem";

                        if (ctrl != null)
                        {
                            ctrl.SetValue("GroupID", gi.GroupID);
                            ctrl.SetValue("CommunityGroupGUID", gi.GroupGUID);
                            ctrl.SetValue("IsLiveSite", IsLiveSite);
                            ctrl.DisplayMode = ControlDisplayModeEnum.Simple;
                            ctrl.OnCheckPermissions += new CheckPermissionsEventHandler(ctrl_OnCheckPermissions);

                            pnlContent.Controls.Add(ctrl);
                        }
                    }
                }
                break;

            case "roles":
                if (rolesTabIndex >= 0)
                {
                    tabMenu.SelectedTab = rolesTabIndex;
                    // Show roles content
                    if (ShowRolesTab)
                    {
                        ctrl = this.LoadUserControl("~/CMSModules/Membership/Controls/Roles/Roles.ascx") as CMSAdminControl;
                        ctrl.ID = "rolesElem";

                        if (ctrl != null)
                        {
                            ctrl.SetValue("GroupID", gi.GroupID);
                            ctrl.SetValue("GroupGUID", gi.GroupGUID);
                            ctrl.SetValue("SiteID", SiteContext.CurrentSiteID);
                            ctrl.SetValue("IsLiveSite", IsLiveSite);
                            ctrl.DisplayMode = ControlDisplayModeEnum.Simple;
                            ctrl.OnCheckPermissions += new CheckPermissionsEventHandler(ctrl_OnCheckPermissions);

                            pnlContent.Controls.Add(ctrl);
                        }
                    }
                }
                break;

            case "polls":
                if (pollsTabIndex >= 0)
                {
                    tabMenu.SelectedTab = pollsTabIndex;

                    // Show polls content
                    if (ShowPollsTab)
                    {
                        ctrl = this.LoadUserControl("~/CMSModules/Polls/Controls/Polls.ascx") as CMSAdminControl;
                        ctrl.ID = "pollsElem";

                        if (ctrl != null)
                        {
                            ctrl.SetValue("GroupID", gi.GroupID);
                            ctrl.SetValue("GroupGUID", gi.GroupGUID);
                            ctrl.SetValue("SiteID", SiteContext.CurrentSiteID);
                            ctrl.SetValue("IsLiveSite", IsLiveSite);
                            ctrl.DisplayMode = ControlDisplayModeEnum.Simple;
                            ctrl.OnCheckPermissions += new CheckPermissionsEventHandler(ctrl_OnCheckPermissions);
                            pnlContent.Controls.Add(ctrl);
                        }
                    }
                }
                break;

            case "messageboards":
                if (messageBoardsTabIndex >= 0)
                {
                    tabMenu.SelectedTab = messageBoardsTabIndex;

                    // Show message boards content
                    if (ShowMessageBoardsTab)
                    {
                        ctrl = this.LoadUserControl("~/CMSModules/MessageBoards/Controls/LiveControls/Boards.ascx") as CMSAdminControl;
                        ctrl.ID = "boardElem";

                        if (ctrl != null)
                        {
                            ctrl.SetValue("GroupID", gi.GroupID);
                            ctrl.SetValue("IsLiveSite", IsLiveSite);
                            ctrl.DisplayMode = ControlDisplayModeEnum.Simple;
                            ctrl.OnCheckPermissions += new CheckPermissionsEventHandler(ctrl_OnCheckPermissions);

                            pnlContent.Controls.Add(ctrl);
                        }
                    }
                }
                break;

            case "medialibrary":
                if (mediaTabIndex >= 0)
                {
                    tabMenu.SelectedTab = mediaTabIndex;

                    // Show media content
                    if (ShowMediaTab)
                    {
                        ctrl = this.LoadUserControl("~/CMSModules/MediaLibrary/Controls/LiveControls/MediaLibraries.ascx") as CMSAdminControl;
                        ctrl.ID = "libraryElem";

                        if (ctrl != null)
                        {
                            ctrl.SetValue("GroupGUID", gi.GroupID);
                            ctrl.SetValue("GroupID", gi.GroupID);
                            ctrl.SetValue("IsLiveSite", IsLiveSite);
                            ctrl.DisplayMode = ControlDisplayModeEnum.Simple;
                            ctrl.OnCheckPermissions += new CheckPermissionsEventHandler(ctrl_OnCheckPermissions);

                            pnlContent.Controls.Add(ctrl);
                        }
                    }
                }
                break;


            default:
                break;
        }

        if (!RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public override void ReloadData()
    {
        if (ctrl != null)
        {
            // Reload data
            ctrl.ReloadData();
        }
    }


    #region "Security handlers"

    /// <summary>
    /// Control - Check permission event handler.
    /// </summary>
    /// <param name="permissionType">Type of the permission to check</param>
    /// <param name="sender">Sender admin control</param>
    private void ctrl_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!RaiseOnCheckPermissions(permissionType, sender))
        {
            // Check if user is allowed to create or modify the module records
            if ((!MembershipContext.AuthenticatedUser.IsGroupAdministrator(GroupID)) && (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Groups", PERMISSION_MANAGE)))
            {
                AccessDenied("CMS.Groups", PERMISSION_MANAGE);
            }
        }
    }

    #endregion
}