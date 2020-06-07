using System;
using System.Collections.Generic;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing.Internal;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;

[assembly: RegisterCustomClass("CultureSitesExtender", typeof(CultureSitesExtender))]

/// <summary>
/// Culture list extender
/// </summary>
public class CultureSitesExtender : ControlExtender<UniSelector>
{
    #region "Variables"

    private CultureInfo cultureInfo;
    private string currentValues;

    #endregion


    public override void OnInit()
    {
        Control.OnSelectionChanged += Control_OnSelectionChanged;
        Control.Page.Load += Page_Load;

        cultureInfo = GetCulture();
    }


    /// <summary>
    /// Page load event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (cultureInfo == null)
        {
            return;
        } 
        
        var siteIds = GetCurrentSiteIds();
        if (siteIds.Count > 0)
        {
            currentValues = TextHelper.Join(";", siteIds);
        }
    }


    private void Control_OnSelectionChanged(object sender, EventArgs e)
    {
        if (cultureInfo == null)
        {
            return;
        }

        bool allSitesProcessed = true;

        // Remove old items
        var removedSiteIds = GetRemovedSiteIds();
        if (removedSiteIds != null)
        {
            allSitesProcessed &= RemoveFromCulture(removedSiteIds);
        }

        // Add new items
        var newSiteIds = GetAddedSiteIds();
        if (newSiteIds != null)
        {
            allSitesProcessed &= AddToCulture(newSiteIds);
        }

        // Refresh selection with skipped sites
        if (!allSitesProcessed)
        {
            var cultureIds = GetCurrentSiteIds();
            if (cultureIds.Count > 0)
            {
                currentValues = TextHelper.Join(";", cultureIds);
                Control.Value = currentValues;
                Control.Reload(true);
            }
        }
    }


    /// <summary>
    /// Returns list of site identifiers which should be added to culture.
    /// </summary>
    private IEnumerable<int> GetAddedSiteIds()
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
    /// Returns list of site identifiers which should be removed from culture.
    /// </summary>
    private IEnumerable<int> GetRemovedSiteIds()
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
    /// Assigns given sites to the edited culture.
    /// </summary>
    /// <param name="siteIds">Identifiers of sites meant for adding.</param>
    /// <returns>Returns <c>true</c> if all cultures were successfully added.</returns>
    private bool AddToCulture(IEnumerable<int> siteIds)
    {
        bool allSitesAdded = true;
        bool changed = false;
        try
        {
            foreach (int siteId in siteIds)
            {
                var site = SiteInfo.Provider.Get(siteId);
                if (site == null)
                {
                    continue;
                }

                if (CultureSiteInfoProvider.LicenseVersionCheck(site.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Insert))
                {
                    CultureSiteInfo.Provider.Add(cultureInfo.CultureID, siteId);
                    new CulturePageUrlPathsManager(siteId).Generate(cultureInfo.CultureID);
                    changed = true;
                }
                else
                {
                    allSitesAdded = false;
                    Control.ShowError(Control.GetString("licenselimitation.siteculturesexceeded"));
                    break;
                } 
            }

            if (changed)
            {
                Control.ShowChangesSaved();
            }
        }
        catch (Exception ex)
        {
            allSitesAdded = false;
            Control.ShowError(ex.Message);
        }

        return allSitesAdded;
    }


    /// <summary>
    /// Removes sites from edited culture.Sites where culture is set as default culture are skipped and not removed. 
    /// Also sites where are translated documents in edited culture are not removed. 
    /// </summary>
    /// <param name="siteIds">Set of site identifiers to delete.</param>
    /// <returns>Returns <c>true</c> if all sites were successfully removed.</returns>
    private bool RemoveFromCulture(IEnumerable<int> siteIds)
    {
        bool allCulturesDeleted = true;
        var changed = false;
        var tree = new TreeProvider();

        // Remove all selected items
        foreach (int siteId in siteIds)
        {
            var site = SiteInfo.Provider.Get(siteId);
            if (site == null)
            {
                continue;
            }

            // Skip sites deletion which have culture set as default one
            if (CultureHelper.GetDefaultCultureCode(site.SiteName).EqualsCSafe(cultureInfo.CultureCode, true))
            {
                allCulturesDeleted = false;
                Control.AddError(string.Format(Control.GetString("culture.errordeletedefaultculture"), site.DisplayName) + '\n');
                continue;
            }

            var cultureDocumentsCount = tree.SelectNodes()
                                            .OnSite(site.SiteName)
                                            .CombineWithDefaultCulture(false)
                                            .Culture(cultureInfo.CultureCode)
                                            .Published(false)
                                            .Count;

            // Skip culture if any document translated to this culture
            if (cultureDocumentsCount > 0)
            {
                allCulturesDeleted = false;
                Control.AddError(string.Format(Control.GetString("culture.ErrorRemoveSiteFromCulture"), site.DisplayName) + '\n');
                continue;
            }

            CultureSiteInfo.Provider.Remove(cultureInfo.CultureID, site.SiteID);
            new CulturePageUrlPathsManager(siteId).Delete(cultureInfo.CultureID);
            changed = true;
        }

        if (changed)
        {
            Control.ShowChangesSaved();
        }

        return allCulturesDeleted;
    }


    private CultureInfo GetCulture()
    {
        int cultureId = QueryHelper.GetInteger("objectid", 0);
        if (cultureId <= 0)
        {
            return null;
        }

        try
        {
            return CultureInfo.Provider.Get(cultureId);
        }
        catch (Exception)
        {
            return null;
        }
    }


    /// <summary>
    /// Returns list of current site identifiers.
    /// </summary>
    private IList<int> GetCurrentSiteIds()
    {
        return CultureSiteInfo.Provider.Get()
                                      .Column("SiteID")
                                      .WhereEquals("CultureID", cultureInfo.CultureID)
                                      .GetListResult<int>();
    }
}