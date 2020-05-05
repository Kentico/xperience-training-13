using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Internal;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.WebAnalytics.Web.UI;
using CMS.WebAnalytics.Web.UI.Internal;

[UIElement(ModuleName.WEBANALYTICS, "Campaigns")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_List : CMSCampaignPage
{
    private const string CAMPAIGN_ELEMENT_CODENAME = "CampaignProperties";
    private const string ANALYTICS_CAMPAIGNS_TABNAME = "analytics.campaigns.campaigns";

    private object DataFromServer
    {
        get
        {
            return new
            {
                Resources = new Dictionary<string, string>
                {
                    {"general.deleteconfirmation", GetString("general.deleteconfirmation")},
                    {"campaigns.list.campaign.new", GetString("campaigns.list.campaign.new")},
                    {"campaigns.list.campaign.edit", GetString("campaigns.list.campaign.edit")},
                    {"campaigns.list.campaign.delete", GetString("campaigns.list.campaign.delete")},
                    {"campaign.list.objective", GetString("campaign.list.objective")},
                    {"campaign.list.conversions", GetString("campaign.list.conversions")},
                    {"campaign.list.uniquevisitors", GetString("campaign.list.uniquevisitors")},
                    {"campaigns.campaign.status.running", GetString("campaigns.campaign.status.running")},
                    {"campaigns.campaign.status.scheduled", GetString("campaigns.campaign.status.scheduled")},
                    {"campaigns.campaign.status.draft", GetString("campaigns.campaign.status.draft")},
                    {"campaigns.campaign.status.finished", GetString("campaigns.campaign.status.finished")},
                    {"campaign.status", GetString("general.status")},
                    {"campaign.name", GetString("general.name")},
                    {"campaign.filterby", GetString("campaign.filterby")},
                    {"campaign.sortby", GetString("campaign.sortby")},
                    {"campaign.filterby.placeholder", GetString("campaign.filterby.placeholder")},
                    {"campaigns.campaign.scheduled.info", GetString("campaigns.campaign.scheduled.info")},
                    {"campaigns.campaign.scheduledtoday.info", GetString("campaigns.campaign.scheduledtoday.info")},
                    {"campaigns.campaign.scheduledtomorrow.info", GetString("campaigns.campaign.scheduledtomorrow.info")},
                    {"campaigns.campaign.draft.info", GetString("campaigns.campaign.draft.info")},
                    {"campaign.codename", GetString("campaign.codename")},
                    {"general.all", GetString("general.all")},
                },
                Campaigns = GetCampaigns(),
                NewCampaignLink = GetCreateCampaignLink()
            };
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        ScriptHelper.RegisterAngularModule("CMS.WebAnalytics/Module", DataFromServer);

        base.OnPreRender(e);

        CurrentMaster.PanelContent.CssClass = "";
    }


    private static IList<CampaignListItemViewModel> GetCampaigns()
    {
        var now = DateTime.Now;
        return CampaignInfoProvider.GetCampaigns()
                                   .OnSite(SiteContext.CurrentSiteID)
                                   .ToList()
                                   .OrderBy(campaign => (int)campaign.GetCampaignStatus(now))
                                   .ThenBy(campaign => campaign.CampaignDisplayName)
                                   .Select(campaign => CreateCampaignViewModel(campaign, now))
                                   .ToList();
    }


    private static CampaignListItemViewModel CreateCampaignViewModel(CampaignInfo campaign, DateTime now)
    {
        return Service.Resolve<ICampaignListItemViewModelService>().GetModel(campaign, now);
    }


    private static string GetCreateCampaignLink()
    {
        return URLHelper.GetAbsoluteUrl(Service.Resolve<IUILinkProvider>().GetSingleObjectLink(CampaignInfo.TYPEINFO.ModuleName, CAMPAIGN_ELEMENT_CODENAME, new ObjectDetailLinkParameters
        {
            ParentTabName = ANALYTICS_CAMPAIGNS_TABNAME,
            AllowNavigationToListing = true,
        }));
    }
}
