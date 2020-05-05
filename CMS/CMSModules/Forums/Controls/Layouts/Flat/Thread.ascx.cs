using System;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Controls_Layouts_Flat_Thread : ForumViewer
{
    private bool onlyPublish = true;
    protected string allowedInlineControls = "none";


    protected void Page_Load(object sender, EventArgs e)
    {
        GenerateActionScripts = true;
        ReloadData();
    }


    /// <summary>
    /// Reloads the data of the forum control.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        // Enable Inline macros resolving only if the dialogs are allowed
        if (ForumContext.CurrentForum != null)
        {
            if (ForumContext.CurrentForum.ForumEnableAdvancedImage)
            {
                allowedInlineControls = ControlsHelper.ALLOWED_FORUM_CONTROLS;
            }
        }

        UniPager1.Enabled = EnablePostsPaging;
        UniPager1.PageSize = PostsPageSize;

        // Hide selected area if forum is AdHoc
        if (IsAdHocForum)
        {
            plcHeader.Visible = false;
        }

        ForumViewModeSelector1.Text = "<span>" + GetString("flatforum.modeslabel") + "</span>&nbsp;";

        if (ForumContext.CurrentMode != ForumMode.TopicMove)
        {
            plcMoveThread.Visible = false;
        }
        else
        {
            plcMoveThread.Visible = true;
            threadMove.TopicMoved += TopicMoved;
        }

        listForums.OuterData = ForumContext.CurrentForum;
        onlyPublish = !ForumContext.UserIsModerator(ForumID, CommunityGroupID);

        // Retrieve data just for the current page
        int currentOffset = EnablePostsPaging ? PostsPageSize * (UniPager1.CurrentPage - 1) : 0;
        int maxRecords = EnablePostsPaging ? PostsPageSize : 0;
        int totalRecords = 0;

        listForums.DataSource = ForumPostInfoProvider.SelectForumPosts(ForumID, "/%", "PostIDPath Like '" + ForumPostInfoProvider.GetPath(null, ThreadID) + "%'", ThreadOrderBy, MaxRelativeLevel, onlyPublish, -1, null, currentOffset, maxRecords, ref totalRecords);

        // Set the total number of records to the pager
        UniPager1.PagedControl.PagerForceNumberOfResults = totalRecords;

        // Redirect to the upper level if there is no post
        if (DataHelper.DataSourceIsEmpty(listForums.DataSource))
        {
            ForumInfo fi = ForumContext.CurrentForum;
            if (fi != null)
            {
                string url = GetURL(fi, ForumActionType.Forum);
                if (!String.IsNullOrEmpty(url))
                {
                    URLHelper.Redirect(UrlResolver.ResolveUrl(url));
                }
            }

            URLHelper.Redirect(URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, "threadid"));
        }

        listForums.DataBind();
    }


    /// <summary>
    /// Topic moved action handler.
    /// </summary>
    protected void TopicMoved(object sender, EventArgs e)
    {
        listForums.Visible = false;
        ForumViewModeSelector1.Visible = false;
    }


    /// <summary>
    /// Returns text(separator if could be displayed).
    /// </summary>
    /// <param name="data">Container.DataItem</param>
    /// <param name="text">Separator text</param>
    /// <param name="type">Type of separator, 0 - reply/quote or reply/subscribe, 1 - quote/subscribe</param>
    public string Separator(object data, string text, int type)
    {
        bool reply = IsAvailable(data, ForumActionType.Reply);
        bool quote = IsAvailable(data, ForumActionType.Quote);
        bool subscribe = IsAvailable(data, ForumActionType.SubscribeToPost);

        if (type == 0)
        {
            if ((reply) && (quote))
            {
                return text;
            }
            else if ((reply) && (!quote) && (subscribe))
            {
                return text;
            }
        }
        else if (type == 1)
        {
            if ((quote) && (subscribe))
            {
                return text;
            }
        }

        return "";
    }


    /// <summary>
    /// Encode text.
    /// </summary>
    /// <param name="value">Input value</param>
    public string Encode(object value)
    {
        return HTMLHelper.HTMLEncode(ValidationHelper.GetString(value, ""));
    }
}