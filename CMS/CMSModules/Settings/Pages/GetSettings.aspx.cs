using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Settings_Pages_GetSettings : GlobalAdminPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Get query string parameters
        int siteId = QueryHelper.GetInteger("siteid", 0);
        int categoryId = QueryHelper.GetInteger("categoryid", 0);
        string searchForText = QueryHelper.GetString("search", "");
        bool searchInDescription = QueryHelper.GetBoolean("description", false);

        // Get category
        SettingsCategoryInfo category = SettingsCategoryInfo.Provider.Get(categoryId);

        // Either category or search text needs to be set
        if ((category == null) && (!string.IsNullOrEmpty(searchForText)))
        {
            return;
        }

        // Get site
        SiteInfo site = SiteInfo.Provider.Get(siteId);

        // Export settings
        Export(category, searchForText, searchInDescription, site);
    }


    #region "Export methods"

    /// <summary>
    /// Builds the export file and writes it to the response.
    /// </summary>
    /// <param name="category">Settings category</param>
    /// <param name="searchText">Text to search for</param>
    /// <param name="searchInDescription">True, if key description should be included in the search</param>
    /// <param name="site">Site</param>
    private void Export(SettingsCategoryInfo category, string searchText, bool searchInDescription, SiteInfo site = null)
    {
        var isSearch = !string.IsNullOrEmpty(searchText);

        var isSite = (site != null);
        var siteName = isSite ? site.SiteName : null;

        var sb = new StringBuilder();

        // Append file title
        if (isSite)
        {
            sb.Append("Settings for site \"", site.DisplayName, "\"");
        }
        else
        {
            sb.Append("Global settings");
        }
        sb.Append(", ");
        if (isSearch)
        {
            sb.Append("search text \"", searchText, "\"");
        }
        else
        {
            string categoryNames = ResHelper.LocalizeString(FormatCategoryPathNames(category.CategoryIDPath));
            sb.Append("category \"", categoryNames, "\"");
        }
        sb.AppendLine();
        sb.AppendLine();

        // Get groups
        var groups = GetGroups(category, isSearch);

        // Append groups and keys
        foreach (var group in groups)
        {
            // Get keys
            var keys = GetKeys(group, isSite, searchText, searchInDescription).ToArray();
            if (!keys.Any())
            {
                // Skip empty groups
                continue;
            }

            // Get formatted keys
            string keysFormatted = FormatKeys(keys, siteName);
            if (string.IsNullOrEmpty(keysFormatted))
            {
                // Skip empty groups
                continue;
            }

            // Append group title
            sb.AppendLine();
            if (isSearch)
            {
                string categoryNames = ResHelper.LocalizeString(FormatCategoryPathNames(group.CategoryIDPath));
                sb.Append(categoryNames);
            }
            else
            {
                string groupDisplayNameLocalized = ResHelper.LocalizeString(group.CategoryDisplayName);
                sb.Append(groupDisplayNameLocalized);
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            sb.AppendLine();

            // Append group keys
            sb.Append(keysFormatted);
        }

        // Write to response
        var fileName = string.Format("{0}_{1}.txt", (site == null ? "Global" : site.DisplayName), ValidationHelper.GetIdentifier(category.CategoryName, "_"));
        HTTPHelper.AddContentDispositionHeader(Response, true, fileName);
        Response.ContentType = "text/plain";
        Response.Write(sb.ToString());
        RequestHelper.EndResponse();
    }


    private IEnumerable<SettingsCategoryInfo> GetGroups(SettingsCategoryInfo category, bool isSearch)
    {
        if (isSearch)
        {
            var groups = SettingsCategoryInfoProvider.GetSettingsCategories("CategoryIsGroup = 1", "CategoryName");
            return groups;
        }
        else
        {
            var groups = SettingsCategoryInfoProvider.GetChildSettingsCategories(category.CategoryID);
            return groups.Cast<SettingsCategoryInfo>().Where(c => c.CategoryIsGroup).OrderBy(c => c.CategoryOrder);
        }
    }


    private IEnumerable<SettingsKeyInfo> GetKeys(SettingsCategoryInfo group, bool isSite, string searchText, bool searchInDescription)
    {
        bool isSearch = !string.IsNullOrEmpty(searchText);

        IEnumerable<SettingsKeyInfo> keys = SettingsKeyInfoProvider.GetSettingsKeys(group.CategoryID).OrderBy("KeyOrder", "KeyDisplayName");

        if (isSearch)
        {
            keys = keys.Where(k => SettingsKeyInfoProvider.SearchSettingsKey(k, searchText, searchInDescription));
        }
        
        if (isSite)
        {
            keys = keys.Where(k => !k.KeyIsGlobal);
        }
        
        return keys;
    }


    /// <summary>
    /// Gets formatted string with name and value of given keys for a specified site.
    /// If site name is not specified, global settings are retrieved.
    /// </summary>
    /// <param name="keys">Keys</param>
    /// <param name="siteName">Site name</param>
    private string FormatKeys(IEnumerable<SettingsKeyInfo> keys, string siteName)
    {
        var sb = new StringBuilder("");
        foreach (var key in keys)
        {
            // Append key display name
            var displayName = ResHelper.LocalizeString(key.KeyDisplayName);
            sb.Append(displayName + (displayName.EndsWithCSafe(":") ? " " : ": "));

            // Append value
            var value = SettingsKeyInfoProvider.GetValue(key.KeyName, siteName);
            sb.Append(value);

            // Append extra information if required
            var isValueInherited = SettingsKeyInfoProvider.IsValueInherited(key.KeyName, siteName);
            if (isValueInherited)
            {
                sb.Append(" (inherited)");
            }
            if (key.KeyIsCustom)
            {
                sb.Append(" (custom)");
            }

            sb.AppendLine();
            sb.AppendLine();
        }
        return sb.ToString();
    }


    /// <summary>
    /// Gets formatted string of display names of all predecessors of given category and name of the category itself separated by ">".
    /// </summary>
    /// <param name="categoryIdPath">Category ID path</param>
    private string FormatCategoryPathNames(string categoryIdPath)
    {
        var categories = SettingsCategoryInfoProvider.GetCategoriesOnPath(categoryIdPath).Cast<SettingsCategoryInfo>().Select(c => c.CategoryDisplayName);
        var formatted = categories.Join(" > ");
        return formatted;
    }

    #endregion
}