using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DocumentEngine.Routing.Internal;

[assembly: RegisterCustomClass("SiteEditCulturesExtender", typeof(SiteEditCulturesExtender))]

/// <summary>
/// Site edit culture extender.
/// </summary>
public class SiteEditCulturesExtender : ControlExtender<UniSelector>
{
    #region "Variables"

    private SiteInfo siteInfo;
    private string currentValues;
    private const string ASSIGN_ARGUMENT_NAME = "assign";
    bool reloadData;

    #endregion


    #region "Methods"

    /// <summary>
    /// Extender initialization.
    /// </summary>
    public override void OnInit()
    {
        Control.OnSelectionChanged += Control_OnSelectionChanged;
        Control.Page.Load += Page_Load;
        Control.Page.PreRender += Page_PreRender;

        siteInfo = Control.EditedObject as SiteInfo;
    }


    /// <summary>
    /// Page pre-render event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (siteInfo == null)
        {
            return;
        }

        if (reloadData)
        {
            Control.Visible = true;
            Control.Reload(true);
        }

        // Check if site hasn't assigned more cultures than license approve
        if (!CultureSiteInfoProvider.LicenseVersionCheck(siteInfo.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Insert))
        {
            Control.ButtonAddItems.Enabled = false;
        }
        else if (!CultureSiteInfoProvider.LicenseVersionCheck(siteInfo.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Edit))
        {
            Control.ShowError(ResHelper.GetString("licenselimitation.siteculturesexceeded"));
            Control.ButtonAddItems.Enabled = false;
        }
    }


    /// <summary>
    /// Page load event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (siteInfo == null)
        {
            return;
        }

        HandleReAssigningCulture();

        Control.Visible = true;
        bool multilingual = LicenseHelper.CheckFeature(URLHelper.GetDomainName(siteInfo.DomainName), FeatureEnum.Multilingual);
        bool cultureOnSite = CultureSiteInfoProvider.IsCultureOnSite(CultureHelper.GetDefaultCultureCode(siteInfo.SiteName), siteInfo.SiteName);
        if (!multilingual && !cultureOnSite)
        {
            Control.Visible = false;

            // Add link that assign the default content culture to the site
            LocalizedHyperlink linkButton = new LocalizedHyperlink
            {
                ResourceString = "sitecultures.assigntodefault",
                NavigateUrl = "javascript:" + ControlsHelper.GetPostBackEventReference(Control.Page, ASSIGN_ARGUMENT_NAME) + ";"
            };

            Control.Parent.Controls.Add(linkButton);
        }
        else
        {
            // Redirect only if cultures not exceeded => to be able to unassign
            if (!CultureSiteInfoProvider.LicenseVersionCheck(siteInfo.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Edit))
            {
                LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetDomainName(siteInfo.DomainName), FeatureEnum.Multilingual);
            }
        }

        // Get the active cultures from DB
        var cultureIds = GetCurrentCultureIds();
        if (cultureIds.Count > 0)
        {
            currentValues = TextHelper.Join(";", cultureIds);
        }
    }


    /// <summary>
    /// Handles OnSelectionChanged event of the UniSelector.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Control_OnSelectionChanged(object sender, EventArgs e)
    {
        if (siteInfo == null)
        {
            return;
        }

        bool allCulturesProcessed = true;

        // Remove old items
        var removedCultureIds = GetRemovedCultureIds();
        if (removedCultureIds != null)
        {
            allCulturesProcessed &= RemoveFromSite(removedCultureIds);
        }

        // Add new items
        var newCultureIds = GetAddedCultureIds();
        if (newCultureIds != null)
        {
            allCulturesProcessed &= AddToSite(newCultureIds);
        }

        // Refresh selection with skipped cultures
        if (!allCulturesProcessed)
        {
            var cultureIds = GetCurrentCultureIds();
            if (cultureIds.Count > 0)
            {
                currentValues = TextHelper.Join(";", cultureIds);
                Control.Value = currentValues;
                Control.Reload(true);
            }
        }
    }


    /// <summary>
    /// Returns list of current culture identifiers.
    /// </summary>
    private IList<int> GetCurrentCultureIds()
    {
        return CultureInfoProvider.GetCultures()
                                  .Column("CultureID")
                                  .WhereIn("CultureID", CultureSiteInfoProvider.GetCultureSites()
                                                                               .Column("CultureID")
                                                                               .WhereEquals("SiteID", siteInfo.SiteID))
                                  .GetListResult<int>();
    }


    /// <summary>
    /// Returns list of culture identifiers which should be added to site.
    /// </summary>
    private ICollection<int> GetAddedCultureIds()
    {
        var selectedValues = ValidationHelper.GetString(Control.Value, String.Empty);
        var newValues = DataHelper.GetNewItemsInList(currentValues, selectedValues);
        if (string.IsNullOrEmpty(newValues))
        {
            return null;
        }

        return newValues
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();
    }


    /// <summary>
    /// Returns list of culture identifiers which should be removed from site.
    /// </summary>
    private ICollection<int> GetRemovedCultureIds()
    {
        var selectedValues = ValidationHelper.GetString(Control.Value, String.Empty);
        var removedValues = DataHelper.GetNewItemsInList(selectedValues, currentValues);
        if (string.IsNullOrEmpty(removedValues))
        {
            return null;
        }

        return removedValues
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();
    }


    /// <summary>
    /// Assigns given cultures to the edited site.
    /// </summary>
    /// <param name="cultureIds">Identifiers of cultures meant for adding.</param>
    /// <returns>Returns <c>true</c> if all cultures were successfully added.</returns>
    private bool AddToSite(ICollection<int> cultureIds)
    {
        bool allCulturesAdded = true;
        try
        {
            bool changed = false;
            var manager = new CulturePageUrlPathsManager(siteInfo.SiteName);

            foreach (int cultureId in cultureIds)
            {
                CultureSiteInfoProvider.AddCultureToSite(cultureId, siteInfo.SiteID);
                manager.Generate(cultureId);
                changed = true;
            }

            if (changed)
            {
                Control.ShowChangesSaved();
            }
        }
        catch (Exception ex)
        {
            allCulturesAdded = false;
            Control.ShowError(ex.Message);
        }

        return allCulturesAdded;
    }


    /// <summary>
    /// Removes cultures from edited site. Default culture is skipped and is not removed. Also cultures which
    /// are assigned to documents are not removed. 
    /// </summary>
    /// <param name="cultureIds">Set of culture identifiers to delete.</param>
    /// <returns>Returns <c>true</c> if all cultures were successfully removed.</returns>
    private bool RemoveFromSite(ICollection<int> cultureIds)
    {
        bool allCulturesDeleted = true;
        bool changed = false;
        var defaultCultureCode = CultureHelper.GetDefaultCultureCode(siteInfo.SiteName);
        var tree = new TreeProvider();
        var manager = new CulturePageUrlPathsManager(siteInfo.SiteName);

        // Remove all selected items from site
        foreach (int cultureId in cultureIds)
        {
            var culture = CultureInfoProvider.GetCultureInfo(cultureId);
            if (culture == null)
            {
                continue;
            }

            // Skip default culture deletion
            if (culture.CultureCode.EqualsCSafe(defaultCultureCode, true))
            {
                allCulturesDeleted = false;
                Control.AddError(String.Format(ResHelper.GetString("site_edit_cultures.errordeletedefaultculture"), HTMLHelper.HTMLEncode(culture.CultureName)) + '\n');
                continue;
            }

            var cultureDocumentsCount = tree.SelectNodes()
                                            .OnSite(siteInfo.SiteName)
                                            .CombineWithDefaultCulture(false)
                                            .Culture(culture.CultureCode)
                                            .Published(false)
                                            .Count;

            // Skip culture if any document translated to this culture
            if (cultureDocumentsCount > 0)
            {
                allCulturesDeleted = false;
                Control.AddError(String.Format(ResHelper.GetString("site_edit_cultures.errorremoveculturefromsite"), HTMLHelper.HTMLEncode(culture.CultureName)) + '\n');
                continue;
            }

            CultureSiteInfoProvider.RemoveCultureFromSite(culture.CultureCode, siteInfo.SiteName);
            manager.Delete(cultureId);
            changed = true;
        }        

        if (changed)
        {
            Control.ShowChangesSaved();
        }

        return allCulturesDeleted;
    }


    /// <summary>
    /// Assign the culture that is set as default content culture to the current site.
    /// </summary>
    private void HandleReAssigningCulture()
    {
        if (RequestHelper.IsPostBack())
        {
            string arg = ValidationHelper.GetString(Control.Page.Request[Page.postEventArgumentID], String.Empty);
            if (arg.EqualsCSafe(ASSIGN_ARGUMENT_NAME))
            {
                string culture = CultureHelper.GetDefaultCultureCode(siteInfo.SiteName);

                // Only default content culture is allowed to be assigned to the site in case there is no multilingual license
                CultureSiteInfoProvider.RemoveSiteCultures(siteInfo.SiteName);
                CultureSiteInfoProvider.AddCultureToSite(culture, siteInfo.SiteName);

                // Get info object of the default content culture to set value of the UniSelector
                CultureInfo ci = CultureInfoProvider.GetCultureInfoForCulture(culture);
                if (ci != null)
                {
                    Control.Value = Convert.ToString(ci.CultureID);
                    reloadData = true;
                }

                Control.ShowChangesSaved();
            }
        }
    }

    #endregion
}
