using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Forums;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Forums_ForumPostsWaitingForApproval : CMSAdminListControl
{
    #region "Variables"

    private string mWhereCondition = String.Empty;
    private bool process = true;
    private string mGroupNames = String.Empty;
    private string mItemsPerPage = string.Empty;
    private string mZeroRowText = String.Empty;
    private string mSiteName = String.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Filter site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return mSiteName;
        }
        set
        {
            mSiteName = value;
        }
    }


    /// <summary>
    /// If no data found - hide control.
    /// </summary>
    public bool HideControlForNoData
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets ID of current group.
    /// </summary>
    public int CommunityGroupId
    {
        get;
        set;
    }


    /// <summary>
    /// Additional WHERE condition to filter data.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
        set
        {
            mWhereCondition = value;
        }
    }


    /// <summary>
    /// Returns datasource of gid.
    /// </summary>
    public object DataSource
    {
        get
        {
            return gridApprove.GridView.DataSource;
        }
    }


    /// <summary>
    /// Group names filter.
    /// </summary>
    public string GroupNames
    {
        get
        {
            return mGroupNames;
        }
        set
        {
            mGroupNames = value;
        }
    }


    /// <summary>
    /// Text for no data.
    /// </summary>
    public string ZeroRowText
    {
        get
        {
            return mZeroRowText;
        }
        set
        {
            mZeroRowText = value;
        }
    }


    /// <summary>
    /// Items per page.
    /// </summary>
    public string ItemsPerPage
    {
        get
        {
            return mItemsPerPage;
        }
        set
        {
            mItemsPerPage = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        process = true;
        // If control is not visible don't process anything       
        if (!Visible || StopProcessing)
        {
            process = false;
            EnableViewState = false;
            return;
        }
        string forumIDs = null;
        // Group where condition part    
        string groupWhere = String.Empty;
        if (SiteName == string.Empty)
        {
            SiteName = SiteContext.CurrentSiteName;
        }
        if (SiteName != TreeProvider.ALL_SITES)
        {
            groupWhere = "GroupSiteID IN (SELECT SiteID FROM CMS_Site WHERE SiteName = N'" + SqlHelper.GetSafeQueryString(SiteName, false) + "')";
        }

        if (CommunityGroupId > 0)
        {
            groupWhere = SqlHelper.AddWhereCondition(groupWhere, "GroupGroupID = " + CommunityGroupId);
        }

        // Add where condition from property
        if (WhereCondition != String.Empty)
        {
            groupWhere = SqlHelper.AddWhereCondition(groupWhere, WhereCondition);
        }

        bool hasGroupRights = false;

        if (CommunityGroupId > 0)
        {
            if (MembershipContext.AuthenticatedUser.IsGroupAdministrator(CommunityGroupId) ||
                MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", "Manage"))
            {
                hasGroupRights = true;
            }
        }

            // Get forums moderated by current user
        else if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            // Get forumId where the user is moderator and forum satisfy group where condition
            string whereCond = "UserID =" + MembershipContext.AuthenticatedUser.UserID;
            if (groupWhere != String.Empty)
            {
                whereCond += " AND ForumID IN ( SELECT ForumID FROM Forums_Forum WHERE " +
                             "ForumGroupID IN (SELECT GroupID FROM Forums_ForumGroup WHERE " + groupWhere + "))";
            }

            // Get forums where user is moderator
            DataSet ds = ForumModeratorInfoProvider.GetGroupForumsModerators(whereCond);

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                forumIDs = "";

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    forumIDs += ValidationHelper.GetString(dr["ForumID"], "") + ",";
                }

                // Remove ending ,
                forumIDs = forumIDs.TrimEnd(',');
            }
        }

        string zeroRowText = String.Empty;
        if (ZeroRowText == String.Empty)
        {
            zeroRowText = GetString("general.nodatafound");
        }
        else
        {
            zeroRowText = HTMLHelper.HTMLEncode(ZeroRowText);
        }

        // Hide approvals
        if ((!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)) && (String.IsNullOrEmpty(forumIDs)) && (hasGroupRights == false))
        {
            if (!HideControlForNoData)
            {
                gridApprove.StopProcessing = true;
                process = false;
                lblInfo.Text = zeroRowText;
                lblInfo.Visible = true;
                return;
            }
            else
            {
                Visible = false;
            }
        }

        gridApprove.ZeroRowsText = zeroRowText;
        gridApprove.OnAction += gridApprove_OnAction;
        gridApprove.GridView.AllowSorting = false;
        gridApprove.OnExternalDataBound += gridApprove_OnExternalDataBound;
        gridApprove.IsLiveSite = IsLiveSite;
        gridApprove.HideControlForZeroRows = false;

        if ((!RequestHelper.IsPostBack()) && (!string.IsNullOrEmpty(ItemsPerPage)))
        {
            gridApprove.Pager.DefaultPageSize = ValidationHelper.GetInteger(ItemsPerPage, -1);
        }

        if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) || hasGroupRights)
        {
            if (groupWhere != String.Empty)
            {
                gridApprove.WhereCondition = "(PostApproved IS NULL OR PostApproved = 0) AND (PostForumID IN (SELECT ForumID FROM [Forums_Forum] WHERE ForumGroupID IN (SELECT GroupID FROM [Forums_ForumGroup] WHERE " + groupWhere + ")))";
            }
            // Show only posts waiting for approval
            else
            {
                gridApprove.WhereCondition = "(PostApproved IS NULL OR PostApproved = 0)";
            }
        }
        else if (forumIDs != null)
        {
            gridApprove.WhereCondition = "((PostApproved IS NULL) OR (PostApproved = 0)) AND (PostForumID IN  (SELECT ForumID FROM [Forums_Forum] WHERE (ForumID IN (" +
                                         forumIDs + "))";
            if (groupWhere != String.Empty)
            {
                gridApprove.WhereCondition += " AND (ForumGroupID IN (SELECT GroupID FROM [Forums_ForumGroup] WHERE " + groupWhere + "))))";
            }
            else
            {
                gridApprove.WhereCondition += "))";
            }
        }


        //Filter group names
        if (GroupNames != String.Empty)
        {
            string where = String.Empty;
            string parsedNames = String.Empty;
            string[] names = GroupNames.Split(';');
            if (names.Length > 0)
            {
                foreach (string name in names)
                {
                    parsedNames += "'" + SqlHelper.GetSafeQueryString(name, false) + "',";
                }
                parsedNames = parsedNames.TrimEnd(',');
                where = "(PostForumID IN (SELECT ForumID FROM [Forums_Forum] WHERE (ForumGroupID IN (SELECT GroupID FROM [Forums_ForumGroup] WHERE GroupName IN (" + parsedNames + ")))))";
                gridApprove.WhereCondition = SqlHelper.AddWhereCondition(gridApprove.WhereCondition, where);
            }
        }
    }


    /// <summary>
    /// Reloads the grid data.
    /// </summary>
    public override void ReloadData()
    {
        gridApprove.ReloadData();
        base.ReloadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!IsLiveSite && process)
        {
            ReloadData();
        }

        string dilaogUrl;
        if (CommunityGroupId > 0)
        {
            dilaogUrl = IsLiveSite ? "~/CMSModules/Groups/CMSPages/LiveForumPostApprove.aspx" : "~/CMSModules/Groups/CMSPages/ForumPostApprove.aspx";
        }
        else
        {
            dilaogUrl = "~/CMSModules/Forums/CMSPages/ForumPostApprove.aspx";
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ForumPostApproveScript", ScriptHelper.GetScript(@"
            function ForumPostApprove(id) {
                var url = '" + UrlResolver.ResolveUrl(dilaogUrl) + @"';
                url = url + '?postid=' + id;
                modalDialog(url, 'forumPostApproveDialog', 900, 650);
                return false;
            }

            function RefreshPage(){
                window.location.replace(document.URL);
            }
        "));

        base.OnPreRender(e);
    }


    #region "UniGrid events"

    /// <summary>
    /// OnExterna databound.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    private object gridApprove_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "forum":
                ForumInfo fi = ForumInfoProvider.GetForumInfo(ValidationHelper.GetInteger(parameter, 0));
                if (fi != null)
                {
                    return HTMLHelper.HTMLEncode(fi.ForumDisplayName);
                }
                break;

            case "content":
                DataRowView dr = parameter as DataRowView;
                if (dr != null)
                {
                    string toReturn = "<strong>" + HTMLHelper.HTMLEncode(ValidationHelper.GetString(dr["PostUserName"], "")) + ":</strong> ";
                    toReturn += HTMLHelper.HTMLEncode(ValidationHelper.GetString(dr["PostSubject"], "")) + "<br />";
                    toReturn += HTMLHelper.HTMLEncode(TextHelper.LimitLength(ValidationHelper.GetString(dr["PostText"], ""), 150));
                    return toReturn;
                }
                break;
        }

        return "";
    }


    /// <summary>
    /// Approve, reject or delete post.
    /// </summary>
    protected void gridApprove_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "deletepost":
                ForumPostInfoProvider.DeleteForumPostInfo(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "approve":
                ForumPostInfo fpi = ForumPostInfoProvider.GetForumPostInfo(ValidationHelper.GetInteger(actionArgument, 0));
                if (fpi != null)
                {
                    fpi.PostApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                    fpi.PostApproved = true;
                    ForumPostInfoProvider.SetForumPostInfo(fpi);
                }
                break;
        }

        RaiseOnAction(actionName, actionArgument);
    }

    #endregion
}
