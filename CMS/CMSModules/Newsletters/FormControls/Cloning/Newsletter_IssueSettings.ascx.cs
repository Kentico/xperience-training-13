using System;
using System.Collections;
using System.Linq;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Newsletters_FormControls_Cloning_Newsletter_IssueSettings : CloneSettingsControl
{
    /// <summary>
    /// Gets properties hashtable
    /// </summary>
    public override Hashtable CustomParameters => GetProperties();


    public override string ExcludedChildTypes => string.Join(";", LinkInfo.OBJECT_TYPE, OpenedEmailInfo.OBJECT_TYPE);


    public override string ExcludedBindingTypes => TargetFeed.NewsletterType == EmailCommunicationTypeEnum.Newsletter ? IssueContactGroupInfo.OBJECT_TYPE : String.Empty;


    private IssueInfo IssueToClone => (IssueInfo)InfoToClone;


    private NewsletterInfo TargetFeed
    {
        get
        {
            var targetFeedID = ValidationHelper.GetInteger(drpNewsletters.Value, IssueToClone.IssueNewsletterID);
            return NewsletterInfo.Provider.Get(targetFeedID);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        drpNewsletters.UniSelector.ReturnColumnName = "NewsletterID";

        if (!RequestHelper.IsPostBack())
        {
            drpNewsletters.Value = IssueToClone.IssueNewsletterID;
        }
    }


    public override bool IsValid(CloneSettings settings)
    {
        // It is not possible to clone ABTest issue without its children,
        // because children are variants of the AB Tests and it makes no sense to clone without variants
        if (IssueToClone.IssueIsABTest)
        {
            if (!settings.IncludeChildren || (settings.MaxRelativeLevel == 0))
            {
                ShowError(GetString("newsletters.cannotcloneabtestissuewithoutchildren"));

                return false;
            }
        }

        var targetFeed = TargetFeed;
        if (targetFeed == null || targetFeed.NewsletterSiteID != SiteContext.CurrentSiteID)
        {
            ShowError(GetString("newsletters.invalidtargetemailfeed"));

            return false;
        }

        if (IssueToClone.IssueTemplateID > 0)
        {
            // Check if the email template of the cloned issue is supported in the target parent newsletter feed
            var templateIsSupported = EmailTemplateNewsletterInfo.Provider.Get()
                 .WhereEquals("TemplateID", IssueToClone.IssueTemplateID)
                 .And()
                 .WhereEquals("NewsletterID", ValidationHelper.GetInteger(drpNewsletters.Value, 0))
                 .TopN(1)
                 .Any();

            if (!templateIsSupported)
            {
                ShowError(string.Format(GetString("newsletters.cannotcloneissuetofeedwithrestrictedtemplate"), IssueToClone.IssueDisplayName, targetFeed.NewsletterDisplayName));

                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        return new Hashtable
            {
                {
                    nameof(IssueInfo.IssueNewsletterID), ValidationHelper.GetInteger(drpNewsletters.Value, IssueToClone.IssueNewsletterID)
                }
            };
    }
}
