using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DocumentEngine.Web.UI.Configuration;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_LiveControls_Group : CMSAdminItemsControl
{
    #region "Variables"

    private int mGroupId = 0;
    private Guid mCommunityGroupGUID = Guid.Empty;
    private bool displayControlPerformed = false;
    private bool listVisible = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the Group ID.
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
    /// Gets or sets the Group GUID.
    /// </summary>
    public Guid CommunityGroupGUID
    {
        get
        {
            return mCommunityGroupGUID;
        }
        set
        {
            mCommunityGroupGUID = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Visible)
        {
            EnableViewState = false;
        }


        #region "Security"

        forumList.OnCheckPermissions += forumList_OnCheckPermissions;
        forumEdit.OnCheckPermissions += forumEdit_OnCheckPermissions;
        forumNew.OnCheckPermissions += forumNew_OnCheckPermissions;
        groupEdit.OnCheckPermissions += groupEdit_OnCheckPermissions;

        #endregion


        listVisible = plcForumList.Visible;
        tabForums.Visible = true;
        plcForumList.Visible = true;
        forumList.Visible = true;

        tabElem.TabControlIdPrefix = "group";
        groupEdit.GroupID = mGroupId;
        groupEdit.DisplayMode = DisplayMode;

        forumNew.GroupID = mGroupId;
        forumNew.DisplayMode = DisplayMode;

        forumList.GroupID = mGroupId;
        forumList.DisplayMode = DisplayMode;

        int forumId = ValidationHelper.GetInteger(ViewState["ForumID"], 0);
        forumEdit.ForumID = forumId;
        forumEdit.DisplayMode = DisplayMode;

        forumList.OnAction += forumList_OnAction;
        forumNew.OnSaved += forumNew_OnSaved;

        HeaderAction action = new HeaderAction();
        action.Text = GetString("Forum_List.NewItemCaption");
        action.CommandName = "newforum";
        actionsElem.AddAction(action);

        actionsElem.ActionPerformed += actionsElem_ActionPerformed;

        InitializeTabs();
        InitializeBreadcrumbs();

        titleElemEdit.TitleText = GetString("forum_edit.headercaption");
    }


    #region "Security handlers"

    private void groupEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void forumNew_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void forumEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void forumList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    /// <summary>
    /// New forum saved handler.
    /// </summary>
    protected void forumNew_OnSaved(object sender, EventArgs e)
    {
        plcForumEdit.Visible = true;
        plcForumList.Visible = false;
        tabNewForum.Visible = false;
        tabForums.Visible = true;

        int forumId = forumNew.ForumID;
        ViewState["ForumID"] = forumId;

        ForumInfo fi = ForumInfoProvider.GetForumInfo(forumId);
        if (fi != null)
        {
            ucBreadcrumbs.Items[1].Text = fi.ForumDisplayName;
        }

        forumEdit.ForumID = forumId;
        forumEdit.ReloadData();
    }


    /// <summary>
    /// New forum click handler.
    /// </summary>
    protected void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "newforum":
                DisplayControl("new");
                break;
        }
    }


    /// <summary>
    /// Edit forum action.
    /// </summary>
    protected void forumList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToString())
        {
            case "edit":

                int forumId = ValidationHelper.GetInteger(e.CommandArgument, 0);
                ViewState["ForumID"] = forumId;

                ForumInfo fi = ForumInfoProvider.GetForumInfo(forumId);
                if (fi != null)
                {
                    ucBreadcrumbs.Items[1].Text = fi.ForumDisplayName;
                }

                forumEdit.ForumID = forumId;
                DisplayControl("edit");

                break;

            default:
                DisplayControl("list");
                break;
        }
    }


    /// <summary>
    /// Show correct tab.
    /// </summary>
    protected void forumTabElem_OnTabChanged(object sender, EventArgs e)
    {
        int tab = tabElem.SelectedTab;
        if (tab == 0)
        {
            DisplayControl("list");
        }
        else if (tab == 1)
        {
            DisplayControl("group");
        }
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    public override void ReloadData()
    {
        groupEdit.GroupID = mGroupId;
        groupEdit.DisplayMode = DisplayMode;

        forumNew.GroupID = mGroupId;
        forumNew.DisplayMode = DisplayMode;

        forumList.GroupID = mGroupId;

        DisplayControl("list");
    }


    /// <summary>
    /// Initializes the tabs.
    /// </summary>
    private void InitializeTabs()
    {
        tabElem.AddTab(new TabItem()
        {
            Text = GetString("Group_General.Forums"),
        });
        tabElem.AddTab(new TabItem()
        {
            Text = GetString("general.general"),
        });

        tabElem.OnTabClicked += forumTabElem_OnTabChanged;
    }


    /// <summary>
    /// Initializes the breadcrumbs.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        lnkBackHidden.Click += lnkBackHidden_Click;

        ucBreadcrumbs.Items.Clear();
        ucBreadcrumbsNewForum.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("forum_list.headercaption"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        int forumId = ValidationHelper.GetInteger(ViewState["ForumID"], 0);
        ForumInfo fi = ForumInfoProvider.GetForumInfo(forumId);

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = (fi != null) ? fi.ForumDisplayName : "",
        });

        ucBreadcrumbsNewForum.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("forum_list.headercaption"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        ucBreadcrumbsNewForum.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("Forum_Edit.NewForum"),
        });
    }


    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        DisplayControl("list");
    }


    private void DisplayControl(string selectedControl)
    {
        tabElem.SelectedTab = 0;
        plcForumList.Visible = false;
        plcForumEdit.Visible = false;
        tabForums.Visible = false;
        tabGeneral.Visible = false;
        tabNewForum.Visible = false;

        displayControlPerformed = true;

        switch (selectedControl.ToLowerCSafe())
        {
            // New forum
            case "new":
                forumNew.ReloadData();
                tabNewForum.Visible = true;
                break;

            // Edit forum
            case "edit":
                forumEdit.ReloadData();
                plcForumEdit.Visible = true;
                tabForums.Visible = true;
                break;

            // Edit forum group
            case "group":
                groupEdit.ReloadData();
                tabGeneral.Visible = true;
                tabElem.SelectedTab = 1;
                break;

            // Forum list
            default:
                plcForumList.Visible = true;
                tabForums.Visible = true;
                forumList.ReloadData();
                break;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!displayControlPerformed)
        {
            plcForumList.Visible = listVisible;
            if (listVisible)
            {
                forumList.ReloadData();
            }
        }

        base.OnPreRender(e);
    }
}