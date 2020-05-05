using System;

using CMS.Core;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.UIControls;
using CMS.EventLog;

public partial class CMSMessages_FeatureNotAvailable : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string domain = QueryHelper.GetText("domainname", String.Empty);

        if (domain != String.Empty)
        {
            LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(domain);
            if (lki != null)
            {    
                string feature = QueryHelper.GetText("feature", String.Empty);

                var logService = Service.Resolve<IEventLogService>();

                if (feature != String.Empty)
                {
                    LabelMessage.Text = String.Format(GetString("general.specifiedFeatureNotAvailable"), feature);

                    // Log message to event log
                    logService.LogWarning(
                        "Feature not available page",
                        "FeatureNotAvailable",
                        String.Format(GetString("license.featurelog.detail"), feature, LicenseHelper.GetEditionName(lki.Edition))
                    );
                }
                else
                {
                    LabelMessage.Text = GetString("general.FeatureNotAvailable");

                    // Log message to event log
                    logService.LogWarning(
                        "Feature not available page",
                        "FeatureNotAvailable",
                        String.Format(GetString("license.featurelog"), LicenseHelper.GetEditionName(lki.Edition))
                    );
                }
            }
            else
            {
                LabelMessage.Text = GetString("general.licensenotfound").Replace("%%name%%", domain);
            }
        }
        titleElem.TitleText = GetString("general.AccessDenied");
    }
}