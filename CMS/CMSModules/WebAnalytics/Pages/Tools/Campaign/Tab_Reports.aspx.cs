using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.Internal;
using CMS.ContactManagement.Web.UI.Internal;
using CMS.Core;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.WebAnalytics.Internal;
using CMS.WebAnalytics.Web.UI;


[EditedObject(CampaignInfo.OBJECT_TYPE, "campaignId")]
[UIElement("CMS.WebAnalytics", "Campaign.Reports")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Reports : CMSCampaignPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var campaignId = QueryHelper.GetInteger("campaignId", 0);

        if (campaignId > 0)
        {
            var campaign = EditedObject as CampaignInfo;
            if ((campaign != null) && (campaign.CampaignSiteID == SiteContext.CurrentSiteID))
            {
                var moduleId = "CMS.WebAnalytics/CampaignReport/build";
                var localizationProvider = Service.Resolve<IClientLocalizationProvider>();
                var reportViewModelService = Service.Resolve<ICampaignReportViewModelService>();
                var demographicsLinkBuilder = Service.Resolve<IContactDemographicsLinkBuilder>();

                ScriptHelper.RegisterAngularModule(moduleId, new {
                    Resources = localizationProvider.GetClientLocalization(moduleId),
                    Report = reportViewModelService.GetViewModel(campaign),
                    DemographicsLink = URLHelper.ResolveUrl(demographicsLinkBuilder.GetDemographicsLink("campaign")),
                    DefaultUTMSourceName = CampaignProcessorConstants.DEFAULT_UTM_SOURCE
                });
            }
            else
            {
                EditedObject = null;
            }
        }
        else
        {
            RedirectToInformation(GetString("campaign.nodata"));
        }
    }
}
