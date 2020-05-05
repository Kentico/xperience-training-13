using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Community;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_Members_Members : CMSAdminControl
{
    #region "Variables"

    private bool mHideWhenGroupIsNotSupplied = false;

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
    /// Gets or sets the group ID for which the members should be displayed.
    /// </summary>
    public int GroupID
    {
        get
        {
            if (memberListElem.GroupID <= 0)
            {
                memberListElem.GroupID = ValidationHelper.GetInteger(GetValue("GroupID"), 0);
            }

            return memberListElem.GroupID;
        }
        set
        {
            memberListElem.GroupID = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            actionsElem.IsLiveSite = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        #region "Security"

        memberListElem.OnCheckPermissions += new CheckPermissionsEventHandler(memberListElem_OnCheckPermissions);
        memberEditElem.OnCheckPermissions += memberEditElem_OnCheckPermissions;

        #endregion


        if (!Visible)
        {
            EnableViewState = false;
        }

        if (StopProcessing)
        {
            actionsElem.StopProcessing = true;
            memberListElem.StopProcessing = true;
            memberEditElem.StopProcessing = true;
        }
        else
        {
            if ((GroupID == 0) && HideWhenGroupIsNotSupplied)
            {
                Visible = false;
                return;
            }

            memberListElem.OnAction += new CommandEventHandler(memberListElem_GridOnAction);

            lnkBackHidden.Click += lnkBackHidden_Click;

            InitializeBreadcrumbs();

            HeaderAction action = new HeaderAction();
            action.Text = GetString("groupinvitation.invite");
            action.OnClientClick = "OpenInvite(); return false;";
            action.CommandName = "invitemember";
            actionsElem.AddAction(action);

            string script = "function OpenInvite() {\n" +
                            "modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Groups/CMSPages/InviteToGroup.aspx") + "?groupid=" + GroupID + "','inviteToGroup', 550, 470); \n" +
                            " } \n";

            // Register menu management scripts
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Members", ScriptHelper.GetScript(script));

            // Register the dialog script
            ScriptHelper.RegisterDialogScript(Page);
        }
    }


    #region "Security handlers"

    protected void memberEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void memberListElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        lblInfo.Visible = true;
        plcList.Visible = true;
        plcEdit.Visible = false;
        ViewState["UserID"] = null;
        memberListElem.ReloadGrid();
    }


    protected void memberListElem_GridOnAction(object sender, CommandEventArgs args)
    {
        switch (args.CommandName.ToLowerCSafe())
        {
            case "approve":
                lblInfo.Text = GetString("group.member.userhasbeenapproved");
                lblInfo.Visible = true;
                break;

            case "reject":
                lblInfo.Text = GetString("group.member.userhasbeenrejected");
                lblInfo.Visible = true;
                break;

            case "edit":
                int memberId = ValidationHelper.GetInteger(args.CommandArgument, 0);
                memberEditElem.MemberID = memberId;
                memberEditElem.GroupID = GroupID;
                plcList.Visible = false;
                plcEdit.Visible = true;
                memberEditElem.Visible = true;
                memberEditElem.ReloadData();

                GroupMemberInfo gmi = GroupMemberInfoProvider.GetGroupMemberInfo(memberId);
                if (gmi != null)
                {
                    UserInfo ui = UserInfoProvider.GetUserInfo(gmi.MemberUserID);
                    if (ui != null)
                    {
                        ucBreadcrumbs.Items[1].Text = ui.FullName;
                        ViewState["UserID"] = ui.UserID;
                    }
                }
                break;
        }
    }


    /// <summary>
    /// Initializes breadcrumbs items.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("group.members"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        int userId = ValidationHelper.GetInteger(ViewState["UserID"], 0);
        UserInfo ui = null;
        if (userId > 0)
        {
            ui = UserInfoProvider.GetUserInfo(userId);
        }

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem { 
            Text = (ui != null) ? ui.FullName : "",
        });
    }
}
