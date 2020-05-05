using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Subscriptions_UserSubscriptionList : CMSAdminControl
{
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
    /// User ID.
    /// </summary>
    public int UserID
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing || !Visible)
        {
            EnableViewState = false;
            forumSubscriptions.StopProcessing = true;
            postSubscription.StopProcessing = true;
            return;
        }

        // If control should be hidden save view state memory
        if (!Visible)
        {
            EnableViewState = false;
        }

        // Initialize controls
        SetupControls();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if ((postSubscription.RowsCount == 0) && (forumSubscriptions.RowsCount == 0))
        {
            pnlSubscriptions.Visible = false;
            pnlNoSubscription.Visible = true;
        }
        else
        {
            pnlSubscriptions.Visible = true;
            pnlNoSubscription.Visible = false;

            // Hide info label if unigrid is empty
            if (postSubscription.RowsCount == 0)
            {
                lblMessagePost.Visible = false;
            }

            if (forumSubscriptions.RowsCount == 0)
            {
                lblMessage.Visible = false;
            }


            string dialogUrl;
            string queryParam = "";

            ScriptHelper.RegisterDialogScript(Page);

            if (IsLiveSite)
            {
                dialogUrl = "~/CMSModules/Forums/CMSPages/LiveUserUnsubscribe.aspx";
            }
            else
            {
                dialogUrl = "~/CMSModules/Forums/CMSPages/ForumPostApprove.aspx";
                queryParam = "&mode=subscription&userid=" + UserID;
            }

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ForumPostUnsubscribeScript", ScriptHelper.GetScript(@"
            function ForumPostUnsubscribe(id) {
                var url = '" + UrlResolver.ResolveUrl(dialogUrl) + @"';
                url = url + '?postid=' + id + '" + queryParam + @"'
                modalDialog(url, 'forumPostApproveDialog', 900, 650);
                return false;
            }

            function RefreshPage(){
                window.location.replace(document.URL);
            }
        "));
        }

        pnlSeparator.Visible = (forumSubscriptions.RowsCount != 0);
    }


    /// <summary>
    /// Initializes controls on the page.
    /// </summary>
    private void SetupControls()
    {
        if (UserID > 0)
        {
            // Setup forum subscriptions UniGrid control     
            forumSubscriptions.Visible = true;
            forumSubscriptions.IsLiveSite = IsLiveSite;
            forumSubscriptions.Pager.DefaultPageSize = 10;
            forumSubscriptions.OnDataReload += forumSubscriptions_OnDataReload;
            forumSubscriptions.OnAction += forumSubscriptions_OnAction;
            forumSubscriptions.ShowActionsMenu = true;
            forumSubscriptions.HideControlForZeroRows = true;

            // Setup forum post subscriptions UniGrid control     
            postSubscription.Visible = true;
            postSubscription.IsLiveSite = IsLiveSite;
            postSubscription.Pager.DefaultPageSize = 10;
            postSubscription.OnDataReload += postSubscription_OnDataReload;
            postSubscription.OnAction += forumSubscriptions_OnAction;
            postSubscription.OnExternalDataBound += postSubscription_OnExternalDataBound;
            postSubscription.ShowActionsMenu = true;
            postSubscription.HideControlForZeroRows = true;
        }
        else
        {
            forumSubscriptions.Visible = false;
            postSubscription.Visible = false;
        }
    }

    object postSubscription_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "forumname":
                ForumInfo fi = ForumInfoProvider.GetForumInfo(ValidationHelper.GetInteger(parameter, 0));
                if (fi != null)
                {
                    return HTMLHelper.HTMLEncode(fi.ForumDisplayName);
                }
                break;
        }
        return parameter;
    }


    #region "UniGrid events handling"


    /// <summary>
    /// On action event handling.
    /// </summary>
    /// <param name="actionName">Name of the action</param>
    /// <param name="actionArgument">Value of the action</param>
    void forumSubscriptions_OnAction(string actionName, object actionArgument)
    {
        int id = ValidationHelper.GetInteger(actionArgument, 0);

        DataSet ds;
        if (actionName.ToLowerCSafe() == "forumunsubscribe")
        {
            ds = ForumSubscriptionInfoProvider.GetSubscriptions("(SubscriptionUserID = " + UserID + ") AND (SubscriptionForumID = " + id + ") AND (SubscriptionPostID IS NULL) AND (ISNULL(SubscriptionApproved, 1) = 1) ", null, 0, "SubscriptionID");
        }
        else
        {
            ds = ForumSubscriptionInfoProvider.GetSubscriptions("(SubscriptionUserID = " + UserID + ") AND (SubscriptionPostID = " + id + ") AND (ISNULL(SubscriptionApproved, 1) = 1)", null, 0, "SubscriptionID");
        }

        ForumSubscriptionInfo fsi = new ForumSubscriptionInfo(ds.Tables[0].Rows[0]);

        switch (actionName.ToLowerCSafe())
        {
            case "forumunsubscribe":
            case "postunsubscribe":
                if (RaiseOnCheckPermissions(PERMISSION_MANAGE, this))
                {
                    if (StopProcessing)
                    {
                        return;
                    }
                }

                try
                {
                    ForumSubscriptionInfoProvider.DeleteForumSubscriptionInfo(fsi);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }

                break;
        }
    }


    DataSet postSubscription_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        string where = "PostId IN (SELECT SubscriptionPostID FROM Forums_ForumSubscription WHERE (SubscriptionUserID = " + UserID + ") AND (ISNULL(SubscriptionApproved, 1) = 1)) AND (PostApproved = 1)";
        if (!String.IsNullOrEmpty(completeWhere))
        {
            where += " AND (" + completeWhere + ")";
        }

        DataSet ds = ForumPostInfoProvider.GetForumPosts().Where(where).OrderBy("PostSubject").Columns("PostID, PostForumID, PostSubject");
        totalRecords = DataHelper.GetItemsCount(ds);
        return ds;
    }


    private DataSet forumSubscriptions_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        string where = "ForumID IN (SELECT SubscriptionForumID FROM Forums_ForumSubscription WHERE (SubscriptionUserID = " + UserID + ") AND (ISNULL(SubscriptionApproved, 1) = 1) AND (SubscriptionPostID IS NULL))";
        if (!String.IsNullOrEmpty(completeWhere))
        {
            where += " AND (" + completeWhere + ")";
        }

        DataSet ds = ForumInfoProvider.GetForums().Where(where).OrderBy("ForumDisplayName").Columns("ForumId, ForumDisplayName").TypedResult;
        totalRecords = DataHelper.GetItemsCount(ds);
        return ds;
    }


    #endregion


    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        if (propertyName != null)
        {
            switch (propertyName.ToLowerCSafe())
            {
                case "userid":
                    UserID = ValidationHelper.GetInteger(value, 0);
                    break;
                case "islivesite":
                    IsLiveSite = ValidationHelper.GetBoolean(value, true);
                    break;
            }
        }

        return true;
    }
}
