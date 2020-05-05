using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Blogs;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Blogs_Controls_CommentFilter : CMSUserControl
{
    #region "Variables"

    private SiteInfo currentSite;
    private CurrentUserInfo currentUser;
    private bool mDisplayAllRecord = true;

    #endregion


    #region "Events"

    /// <summary>
    /// Event which raises when the search button is clicked.
    /// </summary>
    public event EventHandler SearchPerformed;


    /// <summary>
    /// Raises the action performed event.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    protected void RaiseSearchPerformed(Object sender, EventArgs e)
    {
        if (SearchPerformed != null)
        {
            SearchPerformed(sender, e);
        }
    }

    #endregion


    #region "Private property"

    /// <summary>
    /// Cookie key 
    /// </summary>
    private readonly string mSessionKey = "BlogCommentsFilter" + (MembershipContext.AuthenticatedUser == null ? String.Empty : MembershipContext.AuthenticatedUser.UserID.ToString());

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value which determines whether to display (all) record in Blog dropdown.
    /// </summary>
    public bool DisplayAllRecord
    {
        get
        {
            return mDisplayAllRecord;
        }
        set
        {
            mDisplayAllRecord = value;
        }
    }


    /// <summary>
    /// Gets the Blog part of the WHERE condition.
    /// </summary>
    public string BlogWhereCondition
    {
        get
        {
            string blogWhere = "";

            string val = ValidationHelper.GetString(uniSelector.Value, "");
            if (val == "")
            {
                val = (DisplayAllRecord ? "##ALL##" : "##MYBLOGS##");
            }

            // Blogs dropdown list
            switch (val)
            {
                case "##ALL##":
                    // If current user isn't Global admin or user with 'Manage' permissions for blogs
                    if (!currentUser.IsAuthorizedPerResource("cms.blog", "Manage"))
                    {
                        blogWhere = "(NodeOwner=" + currentUser.UserID +
                                    " OR (';' + BlogModerators + ';' LIKE N'%;" + SqlHelper.EscapeQuotes(currentUser.UserName) + ";%'))";
                    }
                    break;

                case "##MYBLOGS##":
                    blogWhere = "NodeOwner = " + currentUser.UserID;
                    break;

                default:
                    blogWhere = "BlogID = " + ValidationHelper.GetInteger(uniSelector.Value, 0);
                    break;
            }

            return blogWhere;
        }
    }


    /// <summary>
    /// Gets the Comment part of the WHERE condition.
    /// </summary>
    public string CommentWhereCondition
    {
        get
        {
            string where = "";

            // Approved dropdown list
            if (drpApproved.SelectedIndex > 0)
            {
                switch (drpApproved.SelectedValue)
                {
                    case "YES":
                        where += " CommentApproved = 1 AND";
                        break;

                    case "NO":
                        where += " (CommentApproved = 0 OR CommentApproved IS NULL ) AND";
                        break;
                }
            }
            // Spam dropdown list
            if (drpSpam.SelectedIndex > 0)
            {
                switch (drpSpam.SelectedValue)
                {
                    case "YES":
                        where += " CommentIsSpam = 1 AND";
                        break;

                    case "NO":
                        where += " (CommentIsSpam = 0 OR CommentIsSpam IS NULL) AND";
                        break;
                }
            }
            if (txtUserName.Text.Trim() != "")
            {
                where += " CommentUserName LIKE '%" + txtUserName.Text.Trim().Replace("'", "''") + "%' AND";
            }
            if (txtComment.Text.Trim() != "")
            {
                where += " CommentText LIKE '%" + txtComment.Text.Trim().Replace("'", "''") + "%' AND";
            }
            if (where != "")
            {
                where = where.Remove(where.Length - 4); // 4 = " AND".Length
            }

            return where;
        }
    }


    /// <summary>
    /// Gets the filter query string.
    /// </summary>
    public string FilterQueryString
    {
        get
        {
            return "&blog=" + HttpUtility.UrlEncode(ValidationHelper.GetString(uniSelector.Value, String.Empty)) +
                    "&user=" + HTMLHelper.HTMLEncode(txtUserName.Text) +
                   "&comment=" + HTMLHelper.HTMLEncode(txtComment.Text) +
                   "&approved=" + drpApproved.SelectedItem.Value +
                   "&isspam=" + drpSpam.SelectedItem.Value;
        }
    }


    /// <summary>
    /// Indicates if controls is in MyDesk section.
    /// </summary>
    public bool IsInMydesk
    {
        get;
        set;
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        currentSite = SiteContext.CurrentSite;
        currentUser = MembershipContext.AuthenticatedUser;

        // Check 'Manage' permission
        var manageBlogs = currentUser.IsAuthorizedPerResource("cms.blog", "Manage");

        // There is no sense to display (ALL) in blogs DDL when
        // user does not have manage blogs permission
        if (!manageBlogs)
        {
            DisplayAllRecord = false;
        }

        var condition = new WhereCondition().WhereEquals("NodeSiteID", currentSite.SiteID);
        if (!currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && !manageBlogs)
        {
            condition.Where(BlogHelper.GetBlogsWhere(currentUser.UserID, currentUser.UserName, null));
        }
        
        // Culture priority column to handle duplicities
        var culturePriority = new RowNumberColumn("CulturePriority", "BlogID");
        culturePriority.PartitionBy = "NodeID";

        // Init Blog selector
        uniSelector.DisplayNameFormat = "{%BlogName%}";
        uniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
        uniSelector.WhereCondition = condition.ToString(true);
        uniSelector.ReturnColumnName = "BlogID";
        uniSelector.AdditionalColumns = culturePriority.ToString();
        uniSelector.ObjectType = BlogHelper.BLOG_OBJECT_TYPE;
        uniSelector.ResourcePrefix = "unisiteselector";
        uniSelector.AllowEmpty = false;
        uniSelector.AllowAll = false;
        uniSelector.OnAfterRetrieveData += uniSelector_OnAfterRetrieveData;

        // Preselect my blogs
        if (IsInMydesk && !RequestHelper.IsPostBack())
        {
            uniSelector.Value = "##MYBLOGS##";
        }
    }


    /// <summary>
    /// OnAfterRetrieveData event handler
    /// </summary>
    /// <param name="ds">DataSet with data</param>
    protected DataSet uniSelector_OnAfterRetrieveData(DataSet ds)
    {
        // Get duplicates and remove them
        var duplicatedRows = ds.Tables[0].Select("CulturePriority > 1");
        foreach (var duplicatedRow in duplicatedRows)
        {
            ds.Tables[0].Rows.Remove(duplicatedRow);
        }

        ds.AcceptChanges();

        return ds;
    }


    protected void uniSelector_OnSpecialFieldsLoaded(object sender, EventArgs e)
    {
        if (DisplayAllRecord)
        {
            uniSelector.SpecialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = "##ALL##" });
        }

        uniSelector.SpecialFields.Add(new SpecialField { Text = GetString("myblogs.comments.selectmyblogs"), Value = "##MYBLOGS##" });
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
            uniSelector.StopProcessing = true;
        }
        else
        {
            btnFilter.Text = GetString("General.Search");

            if (!RequestHelper.IsPostBack())
            {
                // Fill dropdowns
                HandleDropdowns();

                // Preselect filter data
                PreselectFilter();
            }
        }
    }


    protected void btnFilter_Click(object sender, EventArgs e)
    {
        SessionHelper.SetValue(mSessionKey, FilterQueryString);
        RaiseSearchPerformed(null, null);
    }


    protected void HandleDropdowns()
    {
        // Filter approved dropdown
        drpApproved.Items.Add(new ListItem(GetString("general.selectall"), "ALL"));
        drpApproved.Items.Add(new ListItem(GetString("general.yes"), "YES"));
        drpApproved.Items.Add(new ListItem(GetString("general.no"), "NO"));

        drpApproved.SelectedValue = QueryHelper.GetString("approved", (IsInMydesk ? "NO" : "ALL"));

        // Filter spam dropdown
        drpSpam.Items.Add(new ListItem(GetString("general.selectall"), "ALL"));
        drpSpam.Items.Add(new ListItem(GetString("general.yes"), "YES"));
        drpSpam.Items.Add(new ListItem(GetString("general.no"), "NO"));
    }


    /// <summary>
    /// Gets the information on last selected filter configuration and pre-selects the actual values.
    /// </summary>
    private void PreselectFilter()
    {
        string queryString = ValidationHelper.GetString(SessionHelper.GetValue(mSessionKey), "");
        string username = QueryHelper.GetString("user", "");
        if (String.IsNullOrEmpty(username))
        {
            username = URLHelper.GetQueryValue(queryString, "user");
        }
        string comment = QueryHelper.GetString("comment", "");
        if (String.IsNullOrEmpty(comment))
        {
            comment = URLHelper.GetQueryValue(queryString, "comment");
        }
        string approved = QueryHelper.GetString("approved", "");
        if (String.IsNullOrEmpty(approved))
        {
            approved = URLHelper.GetQueryValue(queryString, "approved");
        }
        string isspam = QueryHelper.GetString("isspam", "");
        if (String.IsNullOrEmpty(isspam))
        {
            isspam = URLHelper.GetQueryValue(queryString, "isspam");
        }
        string blog = QueryHelper.GetString("blog", "");
        if (String.IsNullOrEmpty(blog))
        {
            blog = URLHelper.GetQueryValue(queryString, "blog");
        }

        if (username != "")
        {
            txtUserName.Text = username;
        }

        if (comment != "")
        {
            txtComment.Text = comment;
        }

        if (approved != "")
        {
            if (drpApproved.Items.Count > 0)
            {
                drpApproved.SelectedValue = approved;
            }
        }

        if (isspam != "")
        {
            if (drpSpam.Items.Count > 0)
            {
                drpSpam.SelectedValue = isspam;
            }
        }

        if (!String.IsNullOrEmpty(blog))
        {
            uniSelector.Value = HttpUtility.UrlDecode(blog);
        }
    }
}