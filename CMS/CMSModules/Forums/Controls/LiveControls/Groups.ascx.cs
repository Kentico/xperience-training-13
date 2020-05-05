using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_LiveControls_Groups : CMSAdminItemsControl
{
    #region "Variables"

    private int mGroupId = 0;
    private Guid mCommunityGroupGUID = Guid.Empty;
    private bool mHideWhenGroupIsNotSupplied = false;
    private bool displayControlPerformed = false;
    private bool listVisible = false;

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
    /// Gets or sets the Community group GUID.
    /// </summary>
    public Guid CommunityGroupGUID
    {
        get
        {
            if (mCommunityGroupGUID == Guid.Empty)
            {
                mCommunityGroupGUID = ValidationHelper.GetGuid(GetValue("CommunityGroupGUID"), Guid.Empty);
            }

            return mCommunityGroupGUID;
        }
        set
        {
            mCommunityGroupGUID = value;
        }
    }


    /// <summary>
    /// Gets or sets the Community group ID.
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

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        listVisible = plcList.Visible;
        plcList.Visible = true;
        groupList.Visible = true;


        #region "Security"

        groupList.OnCheckPermissions += groupList_OnCheckPermissions;
        groupEdit.OnCheckPermissions += groupEdit_OnCheckPermissions;
        groupNew.OnCheckPermissions += groupNew_OnCheckPermissions;

        #endregion


        if (!Visible || StopProcessing)
        {
            //this.groupList.Visible = false;
            //this.groupEdit.Visible = false;
            //this.groupNew.Visible = false;
        }
        else
        {
            groupNew.IsLiveSite = true;
            groupList.IsLiveSite = true;
            groupEdit.IsLiveSite = true;

            groupList.OnAction += subscriptionList_OnAction;

            if (GroupID > 0)
            {
                groupList.CommunityGroupId = GroupID;
            }
            else
            {
                // Hide controls if the control should be hidden
                if (HideWhenGroupIsNotSupplied)
                {
                    Visible = false;
                    return;
                }
            }

            groupNew.CommunityGroupID = GroupID;

            int groupId = ValidationHelper.GetInteger(ViewState["GroupID"], 0);
            groupEdit.GroupID = groupId;

            HeaderAction action = new HeaderAction();
            action.Text = GetString("Group_List.NewItemCaption");
            action.CommandName = "newgroup";
            action.CssClass += " new-group-button";
            actionsElem.AddAction(action);
            actionsElem.ActionPerformed += actionsElem_ActionPerformed;

            groupNew.OnSaved += groupNew_OnSaved;

            // Set display mode
            groupEdit.DisplayMode = DisplayMode;
            groupEdit.CommunityGroupGUID = CommunityGroupGUID;

            groupNew.DisplayMode = DisplayMode;
            groupNew.CommunityGroupGUID = CommunityGroupGUID;

            InitializeBreadcrumbs();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            DisplayControl("list");
        }
        else if (!displayControlPerformed)
        {
            plcList.Visible = listVisible;
            groupList.Visible = listVisible;
            if (listVisible)
            {
                groupList.ReloadData();
            }
        }

        // Ensure breadcrumbs
        if (String.IsNullOrEmpty(ucBreadcrumbs.Items[1].Text))
        {
            int groupID = ValidationHelper.GetInteger(ViewState["GroupID"], 0);
            if (groupID > 0)
            {
                var fgi = ForumGroupInfoProvider.GetForumGroupInfo(groupID);
                ucBreadcrumbs.Items[1].Text = (fgi != null) ? fgi.GroupDisplayName : "";
                ucBreadcrumbs.HideBreadcrumbs = false;
            }
        }
    }


    #region "Security handlers"

    private void groupNew_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void groupEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void groupList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    protected void groupNew_OnSaved(object sender, EventArgs e)
    {
        int groupId = groupNew.GroupID;
        ViewState["GroupID"] = groupId;

        ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(groupId);

        groupEdit.GroupID = groupId;
        DisplayControl("edit");
    }


    /// <summary>
    /// New subscription link handler.
    /// </summary>
    protected void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "newgroup":
                DisplayControl("new");
                break;
        }
    }


    /// <summary>
    /// Edit subscription handler.
    /// </summary>
    protected void subscriptionList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "edit":

                DisplayControl("edit");

                int groupId = ValidationHelper.GetInteger(e.CommandArgument, 0);
                ViewState["GroupID"] = groupId;

                groupEdit.GroupID = groupId;
                groupEdit.ReloadData();

                ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(groupId);

                ucBreadcrumbs.Items[1].Text = (fgi == null) ? GetString("Group_General.NewGroup") : fgi.GroupDisplayName;
                ucBreadcrumbs.HideBreadcrumbs = false;

                break;

            default:
                DisplayControl("list");
                break;
        }
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    public override void ReloadData()
    {
        DisplayControl("list");
    }


    /// <summary>
    /// Initializes the breadcrumbs.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        lnkBackHidden.Click += lnkBackHidden_Click;

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = GetString("Group_General.GroupList"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem());
    }


    private void SetBreadcrumbs()
    {
        int forumGroupId = ValidationHelper.GetInteger(ViewState["GroupID"], 0);
        if (forumGroupId > 0)
        {
            var forumGroupInfo = ForumGroupInfoProvider.GetForumGroupInfo(forumGroupId);
            ucBreadcrumbs.Items[1].Text = (forumGroupInfo != null) ? forumGroupInfo.GroupDisplayName : "";
        }
        else
        {
            ucBreadcrumbs.Items[1].Text = GetString("Group_General.NewGroup");
        }
    }


    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        DisplayControl("list");
        ViewState["GroupID"] = null;
    }


    /// <summary>
    /// Displays only specified control. Other controls hides.
    /// </summary>
    /// <param name="selectedControl">Selected control</param>
    private void DisplayControl(string selectedControl)
    {
        displayControlPerformed = true;

        // Disable all controls
        plcEdit.Visible = false;
        plcList.Visible = false;
        plcNew.Visible = false;

        switch (selectedControl.ToLowerCSafe())
        {
                // Show group edit control
            case "edit":
                plcEdit.Visible = true;
                groupEdit.Visible = true;
                groupEdit.ReloadData();
                ucBreadcrumbs.HideBreadcrumbs = false;
                SetBreadcrumbs();
                break;

                // Show group new control
            case "new":
                plcNew.Visible = true;
                groupNew.Visible = true;
                groupNew.ReloadData();
                ucBreadcrumbs.HideBreadcrumbs = false;
                SetBreadcrumbs();
                break;

                // Show group list control
            default:
                plcList.Visible = true;
                groupList.Visible = true;
                ucBreadcrumbs.HideBreadcrumbs = true;
                groupList.ReloadData();
                break;
        }
    }
}