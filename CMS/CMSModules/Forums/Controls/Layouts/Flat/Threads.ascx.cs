using System;

using CMS.Forums;
using CMS.Forums.Web.UI;


public partial class CMSModules_Forums_Controls_Layouts_Flat_Threads : ForumViewer
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }


    /// <summary>
    /// Reloads the data of the forum control.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        UniPager1.Enabled = EnableThreadPaging;
        UniPager1.PageSize = ThreadPageSize;

        // Hide selected area if forum is AdHoc
        if (IsAdHocForum)
        {
            plcHeader.Visible = false;
            if (ForumContext.CurrentForum.ForumID == 0)
            {
                plcContent.Visible = false;
            }
        }

        // Get thread and subscribe link url
        bool newThread = IsAvailable(null, ForumActionType.NewThread);
        bool newSubscription = IsAvailable(null, ForumActionType.SubscribeToForum);
        bool newFavorites = IsAvailable(null, ForumActionType.AddForumToFavorites);
        bool newBreadCrumbs = (ForumBreadcrumbs1.GenerateBreadcrumbs() != "");

        // Hide separators according to the link visibility
        // Each separator is hidden if the item preceeding the separator is invisible or
        // no item is behind the separator.
        if (!newThread || (!newSubscription && !newFavorites && !newBreadCrumbs))
        {
            plcActionSeparator.Visible = false;
        }
        if (!newSubscription || (!newFavorites && !newBreadCrumbs))
        {
            plcAddToFavoritesSeparator.Visible = false;
        }
        if (!newFavorites || !newBreadCrumbs)
        {
            plcBreadcrumbsSeparator.Visible = false;
        }

        if ((ForumContext.CurrentForum != null) && (ForumContext.CurrentForum.ForumID > 0))
        {
            string orderBy = String.Format("PostStickOrder DESC, {0} DESC", ForumContext.UserIsModerator(ForumID, CommunityGroupID) ? "PostThreadLastPostTimeAbsolute" :  "PostThreadLastPostTime");
            
            // Retrieve data just for the current page
            int currentOffset = EnableThreadPaging ? ThreadPageSize * (UniPager1.CurrentPage - 1) : 0;
            int maxRecords = EnableThreadPaging ? ThreadPageSize : 0;
            int totalRecords = 0;

            listForums.DataSource = ForumPostInfoProvider.SelectForumPosts(ForumID, "/%", null, orderBy, 0, !ForumContext.UserIsModerator(ForumID, CommunityGroupID), -1, null, currentOffset, maxRecords, ref totalRecords);

            // Set the total number of records to the pager
            UniPager1.PagedControl.PagerForceNumberOfResults = totalRecords;

            listForums.DataBind();
        }
    }
}