using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Reporting;
using CMS.SiteProvider;
using CMS.SocialMarketing;
using CMS.UIControls;


public partial class CMSModules_SocialMarketing_Pages_InsightsMenu : CMSDeskPage
{

    #region "Variables"

    /// <summary>
    /// A collection of Web analytics report types.
    /// </summary>
    private readonly string[] mReportTypes = new string[] { "year", "month", "week", "day", "hour" };


    /// <summary>
    /// Code name of UI element for Facebook insights.
    /// </summary>
    private const string FACEBOOK_INSIGHTS_UIELEMENT_NAME = "FacebookPageInsights";


    /// <summary>
    /// Code name of UI element for LinkedIn insights.
    /// </summary>
    private const string LINKEDIN_INSIGHTS_UIELEMENT_NAME = "LinkedInProfileInsights";


    /// <summary>
    /// Code name of UI element for Twitter insights.
    /// </summary>
    private const string TWITTER_INSIGHTS_UIELEMENT_NAME = "TwitterChannelInsights";


    /// <summary>
    /// Source name for Facebook (see <see cref="Source"/>)
    /// </summary>
    private const string FACEBOOK_SOURCE = "facebook";


    /// <summary>
    /// Source name for LinkedIn (see <see cref="Source"/>)
    /// </summary>
    private const string LINKEDIN_SOURCE = "linkedin";


    /// <summary>
    /// Source name for Twitter (see <see cref="Source"/>)
    /// </summary>
    private const string TWITTER_SOURCE = "twitter";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets source (social network name in lowercase).
    /// </summary>
    private string Source
    {
        get
        {
            return QueryHelper.GetString("source", string.Empty).ToLowerInvariant();
        }
    }

    #endregion


    #region "Life-cycle methods and events"

    protected override void OnInit(EventArgs e)
    {
        CheckLicense(FeatureEnum.SocialMarketingInsights);

        // Select appropriate UIElement for tree root.
        switch (Source)
        {
            case FACEBOOK_SOURCE:
                treeElem.ElementName = FACEBOOK_INSIGHTS_UIELEMENT_NAME;
                break;

            case LINKEDIN_SOURCE:
                treeElem.ElementName = LINKEDIN_INSIGHTS_UIELEMENT_NAME;
                break;

            case TWITTER_SOURCE:
                treeElem.ElementName = TWITTER_INSIGHTS_UIELEMENT_NAME;
                break;
        }

        base.OnInit(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        try
        {
            PageContext context = GetPageContext();
            foreach (Insight insight in GetInsights(context))
            {
                TreeNode node = CreateMenuItem(insight, context);
                treeElem.RootNode.ChildNodes.Add(node);
            }
        }
        catch (Exception exception)
        {
            LogAndShowError("CMSModules_SocialMarketing_Pages_InsightsMenu", "Cannot display content", exception);
        }
        base.OnPreRender(e);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Retrieves a collection of insights where reports are available, and returns it.
    /// </summary>
    /// <returns>A collection of insights where reports are available.</returns>
    private IEnumerable<Insight> GetInsights(PageContext context)
    {
        Dictionary<string, Insight> insights = new Dictionary<string, Insight>(StringComparer.InvariantCultureIgnoreCase);
        foreach (ReportInfo report in new ObjectQuery<ReportInfo>().Where(ReportInfo.TYPEINFO.CodeNameColumn, QueryOperator.Like, context.ReportNamePrefix + "%"))
        {
            string[] tokens = report.ReportName.Split('.');
            string codeName = tokens[1];
            if (!insights.ContainsKey(codeName))
            {
                string resourceName = String.Format(context.DisplayNameResourceNameFormat, codeName);
                Insight insight = new Insight
                {
                    CodeName = codeName,
                    PeriodType = tokens[2],
                    DisplayName = GetString(resourceName)
                };
                insights.Add(codeName, insight);
            }
        }

        return insights.Values.OrderBy(x => x.CodeName).ToArray();
    }


    /// <summary>
    /// Creates a menu item for the specified insight, and returns it.
    /// </summary>
    /// <param name="insight">The insight details.</param>
    /// <param name="context">Information that this page requires to display content.</param>
    /// <returns>A menu item for the specified insight.</returns>
    private TreeNode CreateMenuItem(Insight insight, PageContext context)
    {
        TreeNode node = new TreeNode
        {
            Text = String.Format("<span id='node_{0}' class='ContentTreeItem' name='treeNode'>{2}<span class='Name'>{1}</span></span>", insight.CodeName.Replace('.', '_'), HTMLHelper.HTMLEncode(insight.DisplayName), UIHelper.GetAccessibleIconTag("icon-piechart", size: FontIconSizeEnum.Standard)),
            NavigateUrl = String.Format("~/CMSModules/SocialMarketing/Pages/InsightsReport.aspx?reportCodeNames={0}&periodType={1}&externalId={2}", HttpUtility.UrlEncode(GetReportCodeNames(insight, context)), insight.PeriodType, context.ExternalId),
            Target = treeElem.TargetFrame
        };

        return node;
    }
    

    /// <summary>
    /// Creates a list of report code names for the specified insight that is compatible with the Web analytics module, and returns it.
    /// </summary>
    /// <param name="insight">The insight details.</param>
    /// <param name="context">Information that this page requires to display content.</param>
    /// <returns>A list of report code names for the specified insight that is compatible with the Web analytics module.</returns>
    private string GetReportCodeNames(Insight insight, PageContext context)
    {
        StringBuilder builder = new StringBuilder();
        foreach (string reportType in mReportTypes)
        {
            if (builder.Length > 0)
            {
                builder.Append(';');
            }
            builder.AppendFormat(context.ReportNameFormat, insight.CodeName, insight.PeriodType, reportType);
        }

        return builder.ToString();
    }


    /// <summary>
    /// Creates an object that represents information required by this page to display content, and returns it.
    /// </summary>
    /// <returns>An object that represents information required by this page to display content.</returns>
    private PageContext GetPageContext()
    {
        int objectId = QueryHelper.GetInteger("objectid", 0);

        switch (Source)
        {
            case FACEBOOK_SOURCE:
                return GetPageContextForFacebook(objectId);

            case LINKEDIN_SOURCE:
                return GetPageContextForLinkedIn(objectId);

            case TWITTER_SOURCE:
                return GetPageContextForTwitter(objectId);
        }

        throw new Exception("[CMSModules_SocialMarketing_Pages_InsightsMenu.GetPageContext]: Unknown social network.");
    }

    
    /// <summary>
    /// Creates an object that represents information required by this page to display Facebook insights, and returns it.
    /// </summary>
    /// <param name="accountId">The Facebook account identifier.</param>
    /// <returns>An object that represents information required by this page to display Facebook insights.</returns>
    private PageContext GetPageContextForFacebook(int accountId)
    {
        FacebookAccountInfo account = FacebookAccountInfoProvider.GetFacebookAccountInfo(accountId);

        if (account == null || account.FacebookAccountSiteID != SiteContext.CurrentSiteID)
        {
            throw new Exception("[CMSModules_SocialMarketing_Pages_InsightsMenu.GetPageContextForFacebook]: Facebook account with the specified identifier does not exist.");
        }
        if (!account.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            RedirectToAccessDenied(ModuleName.SOCIALMARKETING, PermissionsEnum.Read.ToString());
        }
        
        return new PageContext
        {
            ExternalId = account.FacebookAccountPageID,
            ReportNamePrefix = "Facebook.page_",
            ReportNameFormat = "Facebook.{0}.{1}.{2}report",
            DisplayNameResourceNameFormat = "sm.ins.facebook.{0}"
        };
    }


    /// <summary>
    /// Creates an object that represents information required by this page to display LinkedIn insights, and returns it.
    /// </summary>
    /// <param name="accountId">The LinkedIn account identifier.</param>
    /// <returns>An object that represents information required by this page to display LinkedIn insights.</returns>
    private PageContext GetPageContextForLinkedIn(int accountId)
    {
        LinkedInAccountInfo account = LinkedInAccountInfoProvider.GetLinkedInAccountInfo(accountId);

        if (account == null || account.LinkedInAccountSiteID != SiteContext.CurrentSiteID)
        {
            throw new Exception("[CMSModules_SocialMarketing_Pages_InsightsMenu.GetPageContextForLinkedIn]: LinkedIn account with the specified identifier does not exist.");
        }
        if (!account.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            RedirectToAccessDenied(ModuleName.SOCIALMARKETING, PermissionsEnum.Read.ToString());
        }

        return new PageContext
        {
            ExternalId = account.LinkedInAccountProfileID,
            ReportNamePrefix = "LinkedIn.",
            ReportNameFormat = "LinkedIn.{0}.{1}.{2}report",
            DisplayNameResourceNameFormat = "sm.ins.linkedin.{0}"
        };
    }


    /// <summary>
    /// Creates an object that represents information required by this page to display Twitter insights, and returns it.
    /// </summary>
    /// <param name="accountId">The Twitter account identifier.</param>
    /// <returns>An object that represents information required by this page to display Twitter insights.</returns>
    private PageContext GetPageContextForTwitter(int accountId)
    {
        TwitterAccountInfo account = TwitterAccountInfoProvider.GetTwitterAccountInfo(accountId);
        if (account == null || account.TwitterAccountSiteID != SiteContext.CurrentSiteID)
        {
            throw new Exception("[CMSModules_SocialMarketing_Pages_InsightsMenu.GetPageContextForTwitter]: Twitter account with the specified identifier does not exist.");
        } 
        if (!account.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            RedirectToAccessDenied(ModuleName.SOCIALMARKETING, PermissionsEnum.Read.ToString());
        }
        
        return new PageContext
        {
            ExternalId = account.TwitterAccountUserID,
            ReportNamePrefix = "Twitter.channel_",
            ReportNameFormat = "Twitter.{0}.{1}.{2}report",
            DisplayNameResourceNameFormat = "sm.ins.twitter.{0}"
        };
    }

    #endregion


    #region "Inner classes"

    /// <summary>
    /// Represents basic information about an insight.
    /// </summary>
    private class Insight
    {

        /// <summary>
        /// Insight code name.
        /// </summary>
        public string CodeName;
        

        /// <summary>
        /// Insight period type.
        /// </summary>
        public string PeriodType;
        
        
        /// <summary>
        /// Insight display name.
        /// </summary>
        public string DisplayName;

    }


    /// <summary>
    /// Represents information that this page requires to display content.
    /// </summary>
    private class PageContext
    {

        /// <summary>
        /// The external identifier of the social network object.
        /// </summary>
        public string ExternalId;


        /// <summary>
        /// The prefix of the report code names that is used to find insight reports.
        /// </summary>
        public string ReportNamePrefix;


        /// <summary>
        /// The format of the report code names.
        /// </summary>
        public string ReportNameFormat;


        /// <summary>
        /// The format of the resource string for insight display name.
        /// </summary>
        public string DisplayNameResourceNameFormat;

    }

    #endregion

}
