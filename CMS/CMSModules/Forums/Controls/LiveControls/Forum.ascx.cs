using System;

using CMS.Base;
using CMS.DocumentEngine.Web.UI.Configuration;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_LiveControls_Forum : CMSAdminItemsControl
{
    #region "Variables"

    private int mForumId = 0;
    private bool displayControlPerformed = false;
    private bool tabVisible = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the Forum ID.
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


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Visible || StopProcessing)
        {
            EnableViewState = false;
        }

        // Show controls
        if (!displayControlPerformed)
        {
            tabVisible = tabSubscriptions.Visible;
            tabSubscriptions.Visible = true;
            subscriptionElem.Visible = true;
        }


        #region "Security"

        // Add permission handlers
        postEdit.OnCheckPermissions += new CheckPermissionsEventHandler(postEdit_OnCheckPermissions);
        forumEditElem.OnCheckPermissions += new CheckPermissionsEventHandler(forumEditElem_OnCheckPermissions);
        subscriptionElem.OnCheckPermissions += new CheckPermissionsEventHandler(subscriptionElem_OnCheckPermissions);
        moderatorEdit.OnCheckPermissions += new CheckPermissionsEventHandler(moderatorEdit_OnCheckPermissions);
        securityElem.OnCheckPermissions += new CheckPermissionsEventHandler(securityElem_OnCheckPermissions);

        #endregion


        // Set properties
        tabElem.TabControlIdPrefix = "forum";
        forumEditElem.ForumID = mForumId;
        forumEditElem.DisplayMode = DisplayMode;

        moderatorEdit.ForumID = mForumId;

        securityElem.ForumID = mForumId;
        subscriptionElem.ForumID = mForumId;
        postEdit.ForumID = mForumId;
        securityElem.IsGroupForum = true;

        InitializeTabs();
    }


    #region "Security"

    // Security handlers
    private void securityElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void moderatorEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void subscriptionElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void forumEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void postEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    /// <summary>
    /// Show correct tab.
    /// </summary>
    protected void forumTabElem_OnTabChanged(object sender, EventArgs e)
    {
        int tab = tabElem.SelectedTab;

        // Switch tab content
        if (tab == 0)
        {
            DisplayControl("posts");
        }
        else if (tab == 1)
        {
            DisplayControl("general");
        }
        else if (tab == 2)
        {
            DisplayControl("subscriptions");
        }
        else if (tab == 3)
        {
            DisplayControl("moderators");
        }
        else if (tab == 4)
        {
            DisplayControl("security");
        }
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    public override void ReloadData()
    {
        // Reload properties in control
        forumEditElem.ForumID = mForumId;
        moderatorEdit.ForumID = mForumId;

        securityElem.ForumID = mForumId;
        subscriptionElem.ForumID = mForumId;
        postEdit.ForumID = mForumId;

        DisplayControl("post");
    }


    /// <summary>
    /// Initializes the tabs.
    /// </summary>
    private void InitializeTabs()
    {
        // Initialize forum tabs
        tabElem.AddTab(new TabItem()
        {
            Text = GetString("Forum_Edit.Posts"),
        });
        tabElem.AddTab(new TabItem()
        {
            Text = GetString("general.general"),
        });
        tabElem.AddTab(new TabItem()
        {
            Text = GetString("Forum_Edit.Subscriptions"),
        });
        tabElem.AddTab(new TabItem()
        {
            Text = GetString("Forum_Edit.Moderating"),
        });
        tabElem.AddTab(new TabItem()
        {
            Text = GetString("general.Security"),
        });
     
        tabElem.OnTabClicked += new EventHandler(forumTabElem_OnTabChanged);
    }


    private void DisplayControl(string selectedControl)
    {
        displayControlPerformed = true;

        // Hide all tabs
        tabGeneral.Visible = false;
        tabModerators.Visible = false;
        tabPosts.Visible = false;
        tabSecurity.Visible = false;
        tabSubscriptions.Visible = false;
        tabElem.SelectedTab = 0;

        switch (selectedControl.ToLowerCSafe())
        {
                // Show general tab
            case "general":
                forumEditElem.ReloadData();
                tabGeneral.Visible = true;
                tabElem.SelectedTab = 1;
                break;

                // Show moderators tab
            case "moderators":
                moderatorEdit.ReloadData(true);
                tabModerators.Visible = true;
                tabElem.SelectedTab = 3;
                break;

                // Show security tab
            case "security":
                securityElem.ReloadData();
                tabSecurity.Visible = true;
                tabElem.SelectedTab = 4;
                break;


                // Show subscriptions tab
            case "subscriptions":
                subscriptionElem.ReloadData();
                tabSubscriptions.Visible = true;
                tabElem.SelectedTab = 2;
                break;

                // Show posts tab
            default:
                postEdit.ReloadData();
                tabPosts.Visible = true;
                break;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!displayControlPerformed)
        {
            // Set visibility of forum tabs
            tabSubscriptions.Visible = tabVisible;
        }

        base.OnPreRender(e);
    }
}