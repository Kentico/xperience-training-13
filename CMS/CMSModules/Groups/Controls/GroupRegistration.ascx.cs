using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Community;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EmailEngine;
using CMS.Forums;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.Protection;
using CMS.Search;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_GroupRegistration : CMSUserControl
{
    #region "Constants"

    private const string FORUM_DOCUMENT_ALIAS = "forums";

    #endregion


    #region "Variables"

    private int mSiteId = 0;
    private bool mRequireApproval = true;
    private string mGroupTemplateSourceAliasPath = null;
    private string mGroupTemplateTargetAliasPath = null;
    private string mGroupProfileURLPath = String.Empty;
    private bool mCombineWithDefaultCulture = false;
    private string mRedirectToURL = string.Empty;
    private string mGroupNameLabelText = null;
    private string mSuccessfullRegistrationText;
    private string mSuccessfullRegistrationWaitingForApprovalText;
    private bool mHideFormAfterRegistration = true;
    private bool mCreateForum = true;
    private bool mCreateMediaLibrary = true;
    private bool mCreateSearchIndexes = true;
    private string mSendWaitingForApprovalEmailTo = String.Empty;
    private TreeProvider mTreeProvider = null;

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
    /// Gets or sets the value that indicates whether form should be hidden after successful registration.
    /// </summary>
    public bool HideFormAfterRegistration
    {
        get
        {
            return mHideFormAfterRegistration;
        }
        set
        {
            mHideFormAfterRegistration = value;
        }
    }


    /// <summary>
    /// Gets or sets text which should be displayed after successful registration.
    /// </summary>
    public string SuccessfullRegistrationText
    {
        get
        {
            return DataHelper.GetNotEmpty(mSuccessfullRegistrationText, GetString("group.group.succreg"));
        }
        set
        {
            mSuccessfullRegistrationText = value;
        }
    }


    /// <summary>
    /// Gets or sets text which should be displayed after successful registration and waiting for approving.
    /// </summary>
    public string SuccessfullRegistrationWaitingForApprovalText
    {
        get
        {
            return DataHelper.GetNotEmpty(mSuccessfullRegistrationWaitingForApprovalText, GetString("group.group.succregapprove"));
        }
        set
        {
            mSuccessfullRegistrationWaitingForApprovalText = value;
        }
    }


    /// <summary>
    /// Gets or sets the label text of group name.
    /// </summary>
    public string GroupNameLabelText
    {
        get
        {
            return mGroupNameLabelText ?? (mGroupNameLabelText = GetString("general.displayname") + ResHelper.Colon);
        }
        set
        {
            mGroupNameLabelText = value;
            lblDisplayName.Text = value;
        }
    }


    /// <summary>
    /// Current site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// If true, the group must be approved before it can be active.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return mCombineWithDefaultCulture;
        }
        set
        {
            mCombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// If true, the group must be approved before it can be active.
    /// </summary>
    public bool RequireApproval
    {
        get
        {
            return mRequireApproval;
        }
        set
        {
            mRequireApproval = value;
        }
    }


    /// <summary>
    /// Alias path of the document structure which will be copied as the group content.
    /// </summary>
    public string GroupTemplateSourceAliasPath
    {
        get
        {
            return mGroupTemplateSourceAliasPath;
        }
        set
        {
            mGroupTemplateSourceAliasPath = value;
        }
    }


    /// <summary>
    /// Alias where the group content will be created by copying the source template.
    /// </summary>
    public string GroupTemplateTargetAliasPath
    {
        get
        {
            return mGroupTemplateTargetAliasPath;
        }
        set
        {
            mGroupTemplateTargetAliasPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the document URL under which will be accessible the profile of newly created group.
    /// </summary>
    public string GroupProfileURLPath
    {
        get
        {
            return mGroupProfileURLPath;
        }
        set
        {
            mGroupProfileURLPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the url, where is user redirected after registration.
    /// </summary>
    public string RedirectToURL
    {
        get
        {
            return mRedirectToURL;
        }
        set
        {
            mRedirectToURL = value;
        }
    }


    /// <summary>
    /// Emails of admins capable of approving the group.
    /// </summary>
    public string SendWaitingForApprovalEmailTo
    {
        get
        {
            return mSendWaitingForApprovalEmailTo;
        }
        set
        {
            mSendWaitingForApprovalEmailTo = value;
        }
    }


    /// <summary>
    /// Indicates if group forum should be created.
    /// </summary>
    public bool CreateForum
    {
        get
        {
            return mCreateForum;
        }
        set
        {
            mCreateForum = value;
        }
    }


    /// <summary>
    /// Indicates if search indexes should be created.
    /// </summary>
    public bool CreateSearchIndexes
    {
        get
        {
            return mCreateSearchIndexes;
        }
        set
        {
            mCreateSearchIndexes = value;
        }
    }


    /// <summary>
    /// Indicates if group media libraries should be created.
    /// </summary>
    public bool CreateMediaLibrary
    {
        get
        {
            return mCreateMediaLibrary;
        }
        set
        {
            mCreateMediaLibrary = value;
        }
    }


    /// <summary>
    /// Gets instance of tree provider.
    /// </summary>
    private TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeForm();
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        SaveData();
    }


    #region "Public methods"

    /// <summary>
    /// Updates the current Group or creates new if no GroupID is present.
    /// </summary>
    public void SaveData()
    {
        // Check banned IP
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            ShowError(GetString("General.BannedIP"));
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
            GroupInfo group = new GroupInfo();
            group.GroupDisplayName = txtDisplayName.Text;
            group.GroupDescription = txtDescription.Text;
            group.GroupAccess = GetGroupAccess();
            group.GroupSiteID = mSiteId;
            group.GroupApproveMembers = GetGroupApproveMembers();
            // Automatic code name can be set after display name + site id is set
            group.Generalized.EnsureCodeName();

            // Set columns GroupCreatedByUserID and GroupApprovedByUserID to current user
            var user = MembershipContext.AuthenticatedUser;

            if (user != null)
            {
                group.GroupCreatedByUserID = user.UserID;

                if ((!RequireApproval) || (CurrentUserIsAdmin()))
                {
                    group.GroupApprovedByUserID = user.UserID;
                    group.GroupApproved = true;
                }
            }

            // Save Group in the database
            GroupInfoProvider.SetGroupInfo(group);

            // Create group admin role
            RoleInfo roleInfo = new RoleInfo();
            roleInfo.RoleDisplayName = "Group admin";
            roleInfo.RoleName = group.GroupName + "_groupadmin";
            roleInfo.RoleGroupID = group.GroupID;
            roleInfo.RoleIsGroupAdministrator = true;
            roleInfo.SiteID = mSiteId;
            // Save group admin role
            RoleInfo.Provider.Set(roleInfo);

            if (user != null)
            {
                // Set user as member of group
                GroupMemberInfo gmi = new GroupMemberInfo();
                gmi.MemberUserID = user.UserID;
                gmi.MemberGroupID = group.GroupID;
                gmi.MemberJoined = DateTime.Now;
                gmi.MemberStatus = GroupMemberStatus.Approved;
                gmi.MemberApprovedWhen = DateTime.Now;
                gmi.MemberApprovedByUserID = user.UserID;

                // Save user as member of group
                GroupMemberInfoProvider.SetGroupMemberInfo(gmi);

                // Set user as member of admin group role
                UserRoleInfo userRole = new UserRoleInfo();
                userRole.UserID = user.UserID;
                userRole.RoleID = roleInfo.RoleID;

                // Save user as member of admin group role
                UserRoleInfo.Provider.Set(userRole);
            }

            // Clear user session a request
            MembershipContext.AuthenticatedUser.Generalized.Invalidate(false);
            MembershipContext.AuthenticatedUser = null;

            string culture = CultureHelper.EnglishCulture.ToString();
            if (DocumentContext.CurrentDocument != null)
            {
                culture = DocumentContext.CurrentDocument.DocumentCulture;
            }

            // Copy document
            errorMessage = GroupInfoProvider.CopyGroupDocument(group, GroupTemplateSourceAliasPath, GroupTemplateTargetAliasPath, GroupProfileURLPath, culture, CombineWithDefaultCulture, MembershipContext.AuthenticatedUser, roleInfo);

            if (!String.IsNullOrEmpty(errorMessage))
            {
                // Display error message
                ShowError(errorMessage);
                return;
            }

            // Create group forum
            if (CreateForum)
            {
                CreateGroupForum(group);

                // Create group forum search index 
                if (CreateSearchIndexes)
                {
                    CreateGroupForumSearchIndex(group);
                }
            }

            // Create group media library
            if (CreateMediaLibrary)
            {
                CreateGroupMediaLibrary(group);
            }

            // Create search index for group documents
            if (CreateSearchIndexes)
            {
                CreateGroupContentSearchIndex(group);
            }

            // Display information on success
            ShowConfirmation(GetString("group.group.createdinfo"));

            // After registration message
            if ((RequireApproval) && (!CurrentUserIsAdmin()))
            {
                ShowConfirmation(SuccessfullRegistrationWaitingForApprovalText);

                // Send approval email to admin
                if (!String.IsNullOrEmpty(SendWaitingForApprovalEmailTo))
                {
                    var siteName = SiteContext.CurrentSiteName;

                    // Create the message
                    EmailTemplateInfo eti = EmailTemplateInfo.Provider.Get("Groups.WaitingForApproval", SiteContext.CurrentSiteID);
                    if (eti != null)
                    {
                        MacroResolver resolver = MacroContext.CurrentResolver;
                        resolver.SetAnonymousSourceData(group);
                        resolver.SetNamedSourceData("Group", group);

                        EmailMessage message = new EmailMessage
                        {
                            From = SettingsKeyInfoProvider.GetValue(siteName + ".CMSSendEmailNotificationsFrom"),
                            Recipients = SendWaitingForApprovalEmailTo
                        };

                        // Send the message using email engine
                        EmailSender.SendEmailWithTemplateText(siteName, message, eti, resolver, false);
                    }
                }
            }
            else
            {
                string groupPath = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSGroupProfilePath");
                string url = String.Empty;

                ShowConfirmation(String.Format(SuccessfullRegistrationText, url));
            }

            // Hide form
            if (HideFormAfterRegistration)
            {
                plcForm.Visible = false;
            }
            else
            {
                ClearForm();
            }
        }
        catch (Exception ex)
        {
            // Display error message
            ShowError(GetString("general.saveerror"), ex.Message);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Clears the fields of the form to default values.
    /// </summary>
    private void ClearForm()
    {
        txtDescription.Text = string.Empty;
        txtDisplayName.Text = string.Empty;
        radGroupMembers.Checked = false;
        radSiteMembers.Checked = false;
        radMembersApproved.Checked = false;
        radMembersInvited.Checked = false;
        radMembersAny.Checked = true;
        radAnybody.Checked = true;
    }


    /// <summary>
    /// Returns true if current user is Global administrator or Community administrator.
    /// </summary>
    private bool CurrentUserIsAdmin()
    {
        var ui = MembershipContext.AuthenticatedUser;
        if (ui != null)
        {
            SiteInfo si = SiteInfo.Provider.Get(SiteID);
            if (si != null)
            {
                return ui.IsInRole("CMSCommunityAdmin", si.SiteName) || ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
            }
        }
        return false;
    }


    /// <summary>
    /// Initializes the controls in the form.
    /// </summary>
    private void InitializeForm()
    {
        // Initialize labels
        lblDisplayName.Text = GroupNameLabelText;
        lblDescription.Text = GetString("general.description") + ResHelper.Colon;
        lblApproveMembers.Text = GetString("group.group.approvemembers") + ResHelper.Colon;
        lblContentAccess.Text = GetString("group.group.contentaccess") + ResHelper.Colon;

        // Initialize radiobuttons
        radAnybody.Text = GetString("group.group.accessanybody");
        radGroupMembers.Text = GetString("group.group.accessgroupmembers");
        radSiteMembers.Text = GetString("group.group.accesssitemembers");
        radMembersAny.Text = GetString("group.group.approveany");
        radMembersApproved.Text = GetString("group.group.approveapproved");
        radMembersInvited.Text = GetString("group.group.approveinvited");

        // Initialize errors
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");

        // Initialize buttons
        btnSave.Text = GetString("general.ok");
        txtDisplayName.IsLiveSite = IsLiveSite;
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
    /// Validates the form entries.
    /// </summary>
    /// <returns>Empty string if validation passed otherwise error message is returned</returns>
    private string ValidateForm()
    {
        return new Validator().NotEmpty(txtDisplayName.Text, rfvDisplayName.ErrorMessage).Result;
    }


    /// <summary>
    /// Creates group forum.
    /// </summary>
    /// <param name="group">Particular group info object</param>
    private void CreateGroupForum(GroupInfo group)
    {
        #region "Create forum group"

        // Get forum group code name
        string forumGroupCodeName = "Forums_group_" + group.GroupGUID;

        // Check if forum group with given name already exists
        if (ForumGroupInfoProvider.GetForumGroupInfo(forumGroupCodeName, SiteContext.CurrentSiteID) != null)
        {
            return;
        }

        ForumGroupInfo forumGroupObj = new ForumGroupInfo();
        const string suffix = " forums";
        forumGroupObj.GroupDisplayName = TextHelper.LimitLength(group.GroupDisplayName, 200 - suffix.Length, string.Empty) + suffix;
        forumGroupObj.GroupName = forumGroupCodeName;
        forumGroupObj.GroupOrder = 0;
        forumGroupObj.GroupEnableQuote = true;
        forumGroupObj.GroupGroupID = group.GroupID;
        forumGroupObj.GroupSiteID = SiteContext.CurrentSiteID;

        // Additional settings
        forumGroupObj.GroupEnableCodeSnippet = true;
        forumGroupObj.GroupEnableFontBold = true;
        forumGroupObj.GroupEnableFontColor = true;
        forumGroupObj.GroupEnableFontItalics = true;
        forumGroupObj.GroupEnableFontStrike = true;
        forumGroupObj.GroupEnableFontUnderline = true;
        forumGroupObj.GroupEnableQuote = true;
        forumGroupObj.GroupEnableURL = true;
        forumGroupObj.GroupEnableImage = true;

        // Set forum group info
        ForumGroupInfoProvider.SetForumGroupInfo(forumGroupObj);

        #endregion


        #region "Create forum"

        string codeName = "General_discussion_group_" + group.GroupGUID;

        // Check if forum with given name already exists
        if (ForumInfoProvider.GetForumInfo(codeName, SiteContext.CurrentSiteID, group.GroupID) != null)
        {
            return;
        }

        // Create new forum object
        ForumInfo forumObj = new ForumInfo();
        forumObj.ForumSiteID = SiteContext.CurrentSiteID;
        forumObj.ForumIsLocked = false;
        forumObj.ForumOpen = true;
        forumObj.ForumDisplayEmails = false;
        forumObj.ForumRequireEmail = false;
        forumObj.ForumDisplayName = "General discussion";
        forumObj.ForumName = codeName;
        forumObj.ForumGroupID = forumGroupObj.GroupID;
        forumObj.ForumCommunityGroupID = group.GroupID;
        forumObj.ForumModerated = false;
        forumObj.ForumAccess = 40000;
        forumObj.ForumPosts = 0;
        forumObj.ForumThreads = 0;
        forumObj.ForumPostsAbsolute = 0;
        forumObj.ForumThreadsAbsolute = 0;
        forumObj.ForumOrder = 0;
        forumObj.ForumUseCAPTCHA = false;
        forumObj.SetValue("ForumHTMLEditor", null);

        // Set security
        forumObj.AllowAccess = SecurityAccessEnum.GroupMembers;
        forumObj.AllowAttachFiles = SecurityAccessEnum.GroupMembers;
        forumObj.AllowMarkAsAnswer = SecurityAccessEnum.GroupMembers;
        forumObj.AllowPost = SecurityAccessEnum.GroupMembers;
        forumObj.AllowReply = SecurityAccessEnum.GroupMembers;
        forumObj.AllowSubscribe = SecurityAccessEnum.GroupMembers;

        if (ForumInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Forums, ObjectActionEnum.Insert))
        {
            ForumInfoProvider.SetForumInfo(forumObj);
        }

        #endregion
    }


    /// <summary>
    /// Creates group media library.
    /// </summary>
    /// <param name="group">Particular group info object</param>
    private void CreateGroupMediaLibrary(GroupInfo group)
    {
        // Set general values
        string codeName = "Library_group_" + group.GroupGUID;

        // Check if library with same name already exists
        MediaLibraryInfo mlInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(codeName, SiteContext.CurrentSiteID, group.GroupID);
        if (mlInfo == null)
        {
            // Create new object (record) if needed
            mlInfo = new MediaLibraryInfo();
            const string suffix = " media";
            mlInfo.LibraryDisplayName = TextHelper.LimitLength(@group.GroupDisplayName, 200 - suffix.Length, string.Empty) + suffix;
            mlInfo.LibraryFolder = @group.GroupName;
            mlInfo.LibraryName = codeName;
            mlInfo.LibraryDescription = string.Empty;
            mlInfo.LibraryGroupID = @group.GroupID;
            mlInfo.LibrarySiteID = SiteContext.CurrentSiteID;

            // Set security
            mlInfo.FileCreate = SecurityAccessEnum.GroupMembers;
            mlInfo.FileDelete = SecurityAccessEnum.GroupMembers;
            mlInfo.FileModify = SecurityAccessEnum.GroupMembers;
            mlInfo.FolderCreate = SecurityAccessEnum.GroupMembers;
            mlInfo.FolderDelete = SecurityAccessEnum.GroupMembers;
            mlInfo.FolderModify = SecurityAccessEnum.GroupMembers;
            mlInfo.Access = SecurityAccessEnum.GroupMembers;

            try
            {
                MediaLibraryInfo.Provider.Set(mlInfo);
            }
            catch
            {
            }

            // Create additional folders
            //MediaLibraryInfoProvider.CreateMediaLibraryFolder(SiteContext.CurrentSiteName, mlInfo.LibraryID, "Videos", false);
            //MediaLibraryInfoProvider.CreateMediaLibraryFolder(SiteContext.CurrentSiteName, mlInfo.LibraryID, "Other", false);
            //MediaLibraryInfoProvider.CreateMediaLibraryFolder(SiteContext.CurrentSiteName, mlInfo.LibraryID, "Photos & Images", false);
        }
    }


    /// <summary>
    /// Creates content search index.
    /// </summary>
    /// <param name="group">Particular group info object</param>
    private void CreateGroupContentSearchIndex(GroupInfo group)
    {
        string codeName = "default_group_" + group.GroupGUID;

        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(codeName);
        if (sii == null)
        {
            // Create search index info
            sii = new SearchIndexInfo();
            sii.IndexName = codeName;
            const string suffix = " - Default";
            sii.IndexDisplayName = TextHelper.LimitLength(group.GroupDisplayName, 200 - suffix.Length, string.Empty) + suffix;
            sii.IndexAnalyzerType = SearchAnalyzerTypeEnum.StandardAnalyzer;
            sii.IndexType = TreeNode.OBJECT_TYPE;
            sii.IndexIsCommunityGroup = false;
            sii.IndexProvider = SearchIndexInfo.LUCENE_SEARCH_PROVIDER;

            // Create search index settings info
            SearchIndexSettingsInfo sisi = new SearchIndexSettingsInfo();
            sisi.ID = Guid.NewGuid();
            sisi.Path = mGroupTemplateTargetAliasPath + "/" + group.GroupName + "/%";
            sisi.SiteName = SiteContext.CurrentSiteName;
            sisi.Type = SearchIndexSettingsInfo.TYPE_ALLOWED;
            sisi.ClassNames = "";

            // Create settings item
            SearchIndexSettings sis = new SearchIndexSettings();

            // Update settings item
            sis.SetSearchIndexSettingsInfo(sisi);

            // Update xml value
            sii.IndexSettings = sis;
            SearchIndexInfoProvider.SetSearchIndexInfo(sii);

            // Assign to current website and current culture
            SearchIndexSiteInfo.Provider.Add(sii.IndexID, SiteContext.CurrentSiteID);
            CultureInfo ci = DocumentContext.CurrentDocumentCulture;
            if (ci != null)
            {
                SearchIndexCultureInfo.Provider.Add(sii.IndexID, ci.CultureID);
            }

            // Register rebuild index action
            SearchTaskInfoProvider.CreateTask(SearchTaskTypeEnum.Rebuild, null, null, sii.IndexName, sii.IndexID);
        }
    }


    /// <summary>
    /// Creates forum search index.
    /// </summary>
    /// <param name="group">Particular group info object</param>
    private void CreateGroupForumSearchIndex(GroupInfo group)
    {
        string codeName = "forums_group_" + group.GroupGUID;

        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(codeName);
        if (sii == null)
        {
            // Create search index info
            sii = new SearchIndexInfo();
            sii.IndexName = codeName;
            const string suffix = " - Forums";
            sii.IndexDisplayName = TextHelper.LimitLength(group.GroupDisplayName, 200 - suffix.Length, string.Empty) + suffix;
            sii.IndexAnalyzerType = SearchAnalyzerTypeEnum.StandardAnalyzer;
            sii.IndexType = PredefinedObjectType.FORUM;
            sii.IndexIsCommunityGroup = false;
            sii.IndexProvider = SearchIndexInfo.LUCENE_SEARCH_PROVIDER;

            // Create search index settings info
            SearchIndexSettingsInfo sisi = new SearchIndexSettingsInfo();
            sisi.ID = Guid.NewGuid();
            sisi.ForumNames = "*_group_" + group.GroupGUID;
            sisi.Type = SearchIndexSettingsInfo.TYPE_ALLOWED;
            sisi.SiteName = SiteContext.CurrentSiteName;

            // Create settings item
            SearchIndexSettings sis = new SearchIndexSettings();

            // Update settings item
            sis.SetSearchIndexSettingsInfo(sisi);

            // Update xml value
            sii.IndexSettings = sis;
            SearchIndexInfoProvider.SetSearchIndexInfo(sii);

            // Assing to current website and current culture
            SearchIndexSiteInfo.Provider.Add(sii.IndexID, SiteContext.CurrentSiteID);
            CultureInfo ci = DocumentContext.CurrentDocumentCulture;
            if (ci != null)
            {
                SearchIndexCultureInfo.Provider.Add(sii.IndexID, ci.CultureID);
            }
        }
    }

    #endregion
}