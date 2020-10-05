using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.SocialMarketing;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_SocialMarketing_Pages_FacebookPostDetailDialog : CMSModalPage
{
    #region "Private variables"

    private FacebookPostInfo mPost;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets Facebook post which details will be displayed.
    /// </summary>
    private FacebookPostInfo Post
    {
        get
        {
            if (mPost == null)
            {
                int id = QueryHelper.GetInteger("postid", 0);
                mPost = FacebookPostInfoProvider.GetFacebookPostInfo(id);
            }
            return mPost;
        }
    }

    #endregion


    #region "Life-cycle methods"

    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        CheckLicense(FeatureEnum.SocialMarketingInsights);
        if (Post == null || !Post.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            RedirectToAccessDenied(ModuleName.SOCIALMARKETING, PermissionsEnum.Read.ToString());
        }

        PageTitle.TitleText = GetString("sm.facebook.posts.detail");
        LoadPostIntoDialog(Post);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads data from post info object into the dialog.
    /// </summary>
    /// <param name="post">Facebook post info object.</param>
    private void LoadPostIntoDialog(FacebookPostInfo post)
    {
        if ((post == null) || (post.FacebookPostSiteID != SiteContext.CurrentSiteID))
        {
            pnlPostDetail.Visible = false;
            ShowError(GetString("sm.facebook.posts.msg.postnotexist"));
            return;
        }

        lblPostStatus.Text = FacebookPostInfoProvider.GetPostPublishStateMessage(post, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, true);
        lblPeopleReachedValue.Text = ValidationHelper.GetString(post.FacebookPostInsightPeopleReached, String.Empty);
        if (post.FacebookPostCampaignID.HasValue)
        {
            CampaignInfo campaign = CampaignInfo.Provider.Get(post.FacebookPostCampaignID.Value);
            if (campaign != null)
            {
                lblCampaign.Text = HTMLHelper.HTMLEncode(campaign.CampaignDisplayName);
            }
        }
        else
        {
            pnlCampaign.Visible = false;
        }

        lblPostText.Text = GetPostTextHTML(post.FacebookPostText);
        lblPostLikesValue.Text = ValidationHelper.GetString(post.FacebookPostInsightLikesFromPage, String.Empty);
        lblPostCommentsValue.Text = ValidationHelper.GetString(post.FacebookPostInsightCommentsFromPage, String.Empty);
        lblPostSharesValue.Text = ValidationHelper.GetString(post.FacebookPostInsightSharesFromPage, String.Empty);

        lblTotalLikesValue.Text = ValidationHelper.GetString(post.FacebookPostInsightLikesTotal, String.Empty);
        lblTotalCommentsValue.Text = ValidationHelper.GetString(post.FacebookPostInsightCommentsTotal, String.Empty);

        lblHidePostValue.Text = ValidationHelper.GetString(post.FacebookPostInsightNegativeHidePost, String.Empty);
        lblHideAllPostsValue.Text = ValidationHelper.GetString(post.FacebookPostInsightNegativeHideAllPosts, String.Empty);
        lblReportSpamValue.Text = ValidationHelper.GetString(post.FacebookPostInsightNegativeReportSpam, String.Empty);
        lblUnlikePageValue.Text = ValidationHelper.GetString(post.FacebookPostInsightNegativeUnlikePage, String.Empty);
    }


    /// <summary>
    /// Gets post's content with HTML tags. New lines and URLs are replaced by HTML tags.
    /// </summary>
    /// <param name="postText">Plain post text.</param>
    /// <returns>Post text with HTML tags.</returns>
    private string GetPostTextHTML(string postText)
    {
        string result = HTMLHelper.HTMLEncodeLineBreaks(HTMLHelper.HTMLEncode(postText));

        URLParser urlParser = new URLParser();
        result = urlParser.Replace(result, match =>
        {
            string href = String.IsNullOrEmpty(match.Protocol) ? URLHelper.AddHTTPToUrl(match.URL) : match.URL;
            return String.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", href, match.URL);
        });

        return result;
    }

    #endregion
}
