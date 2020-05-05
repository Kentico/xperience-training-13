using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Sites_Pages_CultureChange : CMSModalGlobalAdminPage
{
    private int siteId;
    private string currentCulture;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get data from parameters
        siteId = QueryHelper.GetInteger("siteid", 0);
        currentCulture = QueryHelper.GetString("culture", "");

        // Strings and images
        lblNewCulture.Text = GetString("SiteDefaultCultureChange.NewCulture");

        PageTitle.TitleText = GetString("SiteDefaultCultureChange.Title");
        SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
        if (si != null)
        {
            // Check licensing policy
            if (LicenseHelper.CheckFeature(URLHelper.GetDomainName(si.DomainName), FeatureEnum.Multilingual))
            {
                // Get only site cultures
                cultureSelector.SiteID = siteId;
            }
            else
            {
                // Get all cultures for non multilingual
                cultureSelector.DisplayAllCultures = true;

                // Have to change culture of documents
                chkDocuments.Enabled = false;
            }

            // Check version limitations
            if (!CultureSiteInfoProvider.LicenseVersionCheck(si.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Edit))
            {
                lblError.Text = GetString("licenselimitation.siteculturesexceeded");
                lblError.Visible = true;
                pnlForm.Enabled = false;
            }


            currentCulture = CultureHelper.GetDefaultCultureCode(si.SiteName);
            if (!RequestHelper.IsPostBack())
            {
                cultureSelector.Value = currentCulture;
            }
        }
    }


    /// <summary>
    /// OkClick Handler.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        string culture = ValidationHelper.GetString(cultureSelector.Value, "");

        if ((culture != "") && ((currentCulture.ToLowerCSafe() != culture.ToLowerCSafe()) || chkDocuments.Checked))
        {
            // Set new culture
            SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
            if (si != null)
            {
                try
                {
                    // Set default culture and change current culture label
                    SettingsKeyInfoProvider.SetValue("CMSDefaultCultureCode", si.SiteName, culture.Trim());

                    // Change culture of documents
                    if (chkDocuments.Checked)
                    {
                        // Change culture of the documents
                        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                        tree.ChangeCulture(si.SiteName, currentCulture, culture);
                    }

                    if (!LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.Multilingual))
                    {
                        // If not multilingual, remove all cultures from the site and assign new culture
                        CultureSiteInfoProvider.RemoveSiteCultures(si.SiteName);
                        CultureSiteInfoProvider.AddCultureToSite(culture, si.SiteName);
                    }

                    ltlScript.Text = ScriptHelper.GetScript("wopener.ChangeCulture('" + chkDocuments.Checked + "'); CloseDialog();");
                }
                catch (Exception ex)
                {
                    LogAndShowError("Sites", "ChangeDefaultCulture", ex);
                }
            }
        }
        else
        {
            ltlScript.Text = ScriptHelper.GetScript("CloseDialog();");
        }
    }
}
