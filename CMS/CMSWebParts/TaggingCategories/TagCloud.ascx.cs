using System;
using System.Data;
using System.Text;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.Taxonomy;
using CMS.DataEngine;
using CMS.PortalEngine;

public partial class CMSWebParts_TaggingCategories_TagCloud : CMSAbstractWebPart
{
    #region "Variables"

    private string aliasPath;
    private string siteName;
    private string cultureCode;

    private TagGroupInfo tgi;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the group of tags which should be displayed.
    /// </summary>
    public string TagGroupName
    {
        get
        {
            string tagGroupName = ValidationHelper.GetString(GetValue("TagGroupName"), String.Empty);

            // If tag group name not set
            if (String.IsNullOrEmpty(tagGroupName))
            {
                int tagGroupId;

                // Get tag group ID from current document or get its inherited value
                if (DocumentContext.CurrentDocument.DocumentTagGroupID != 0)
                {
                    tagGroupId = DocumentContext.CurrentDocument.DocumentTagGroupID;
                }
                else
                {
                    tagGroupId = ValidationHelper.GetInteger(DocumentContext.CurrentDocument.GetInheritedValue("DocumentTagGroupID", false), 0);
                }

                // Get tag group information
                TagGroupInfo tagGroup = TagGroupInfoProvider.GetTagGroupInfo(tagGroupId);
                if (tagGroup != null)
                {
                    // Get tag group name
                    tagGroupName = tagGroup.TagGroupName;
                }
            }

            return tagGroupName;
        }
        set
        {
            SetValue("TagGroupName", value);
        }
    }


    /// <summary>
    /// Gets or sets the SQL order by expression.
    /// </summary>
    public string OrderBy
    {
        get
        {
            string orderBy = ValidationHelper.GetString(GetValue("OrderBy"), "");
            return (orderBy == "") ? "TagName ASC" : orderBy;
        }
        set
        {
            SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Gets or sets the SQL TOP N number.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), -1);
        }
        set
        {
            SetValue("SelectTopN", value);
        }
    }


    /// <summary>
    /// Gets or sets the HTML code which is placed between each two tags.
    /// </summary>
    public string TagSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TagSeparator"), " ");
        }
        set
        {
            SetValue("TagSeparator", value);
        }
    }


    /// <summary>
    /// Gets or sets the miminal font size (for the tag with the lowest count).
    /// </summary>
    public int MinTagSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MinTagSize"), 10);
        }
        set
        {
            SetValue("MinTagSize", value);
        }
    }


    /// <summary>
    /// Gets or sets the miminal font size (for the tag with the highest count).
    /// </summary>
    public int MaxTagSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxTagSize"), 20);
        }
        set
        {
            SetValue("MaxTagSize", value);
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), "");
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Gets or sets the alias path.
    /// </summary>
    public string AliasPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AliasPath"), "");
        }
        set
        {
            SetValue("AliasPath", value);
        }
    }


    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CultureCode"), "");
        }
        set
        {
            SetValue("CultureCode", value);
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), "");
        }
        set
        {
            SetValue("WhereCondition", value);
        }
    }


    /// <summary>
    /// Gets or sets combine with default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), SiteInfoProvider.CombineWithDefaultCulture(SiteName));
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
        }
    }


    /// <summary>
    /// Gets or sets select only published.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), true);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
        }
    }


    /// <summary>
    /// Gets or sets max relative level.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), -1);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
        }
    }


    /// <summary>
    /// Gets or sets the path of the document which will be used as a link URL for tags.
    /// </summary>
    public string DocumentListPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DocumentListURL"), "");
        }
        set
        {
            SetValue("DocumentListURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the query string parameter which will pass the TagID to the DocumentListURL.
    /// </summary>
    public string QueryStringName
    {
        get
        {
            string name = Convert.ToString(GetValue("QueryStringName"));
            if (String.IsNullOrEmpty(name))
            {
                name = "tagid";
            }
            return name;
        }
        set
        {
            SetValue("QueryStringName", value);
        }
    }


    /// <summary>
    /// Indicates if the control should be hidden when no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), false);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
        }
    }


    /// <summary>
    /// Indicates if the tags should be retrieved according to document filter settings.
    /// </summary>
    public bool UseDocumentFilter
    {
        get
        {
            object value = GetValue("UseDocumentFilter");
            if (value == null)
            {
                return !((AliasPath == TreeProvider.ALL_DOCUMENTS) && (CultureCode == TreeProvider.ALL_CULTURES) && String.IsNullOrEmpty(WhereCondition) && (MaxRelativeLevel < 0));
            }
            else
            {
                return ValidationHelper.GetBoolean(value, false);
            }
        }
        set
        {
            SetValue("UseDocumentFilter", value);
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed to the user when no data found.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), "");
        }
        set
        {
            SetValue("ZeroRowsText", value);
        }
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Clears the cached items.
    /// </summary>
    public override void ClearCache()
    {
        string useCacheItemName = DataHelper.GetNotEmpty(CacheItemName, CacheHelper.BaseCacheKey + "|" + RequestContext.CurrentURL + "|" + ClientID);

        CacheHelper.ClearCache(useCacheItemName);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            try
            {
                // Prepare alias path
                aliasPath = AliasPath;
                if (String.IsNullOrEmpty(aliasPath))
                {
                    aliasPath = "/%";
                }
                aliasPath = MacroResolver.ResolveCurrentPath(aliasPath);

                // Prepare site name
                siteName = SiteName;
                if (String.IsNullOrEmpty(siteName))
                {
                    siteName = SiteContext.CurrentSiteName;
                }

                // Prepare culture code
                cultureCode = CultureCode;
                if (String.IsNullOrEmpty(cultureCode))
                {
                    cultureCode = LocalizationContext.PreferredCultureCode;
                }

                // Base URL of the links
                string url = RequestContext.CurrentURL;
                url = UrlResolver.ResolveUrl(url);

                string renderedTags = null;

                // Try to get data from cache
                using (var cs = new CachedSection<string>(ref renderedTags, CacheMinutes, true, CacheItemName, "tagcloud", TagGroupName, OrderBy, SelectTopN, url, TagSeparator, QueryStringName, MaxTagSize, MinTagSize, "documents", siteName, aliasPath, CacheHelper.GetCultureCacheKey(cultureCode), CombineWithDefaultCulture, WhereCondition, SelectOnlyPublished, MaxRelativeLevel))
                {
                    if (cs.LoadData)
                    {
                        // Get the correct range
                        int maxSize = Math.Max(MaxTagSize, MinTagSize);
                        int minSize = Math.Min(MaxTagSize, MinTagSize);

                        // Get the tags
                        SiteInfo si = SiteInfoProvider.GetSiteInfo(siteName);
                        int siteId = 0;
                        if (si != null)
                        {
                            siteId = si.SiteID;
                        }

                        // Get tag group info
                        tgi = TagGroupInfoProvider.GetTagGroupInfo(TagGroupName, siteId);

                        // Get the data
                        DataSet ds = null;
                        if (!UseDocumentFilter)
                        {
                            // Get the tag group
                            if (tgi != null)
                            {
                                // Get the tags for group
                                ds = TagInfoProvider.GetTags("TagGroupID = " + tgi.TagGroupID, OrderBy, SelectTopN);
                            }
                        }
                        else
                        {
                            // Get the tags for documents
                            string comleteWhere = TreeProvider.GetCompleteWhereCondition(siteName, aliasPath, cultureCode, CombineWithDefaultCulture, WhereCondition, SelectOnlyPublished, MaxRelativeLevel);
                            ds = TagInfoProvider.GetTags(TagGroupName, siteId, comleteWhere, OrderBy, SelectTopN);
                        }

                        // DS must have at least three columns (fist for IDs, second for names, third for counts)
                        if (!DataHelper.DataSourceIsEmpty(ds))
                        {
                            // First we need to find the maximum and minimum
                            int max = Int32.MinValue;
                            int min = Int32.MaxValue;
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                int tagCount = ValidationHelper.GetInteger(dr["TagCount"], 0);
                                max = Math.Max(tagCount, max);
                                min = Math.Min(tagCount, min);
                            }

                            // Now generate the tags
                            int count = ds.Tables[0].Rows.Count;

                            StringBuilder sb = new StringBuilder(count * 100);
                            int index = 0;

                            // Process the tags
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                if (index > 0)
                                {
                                    sb.Append(TagSeparator + "\n");
                                }

                                // Count the percentage and get the final size of the tag
                                int tagCount = ValidationHelper.GetInteger(dr["TagCount"], 0);
                                int val = (min == max ? 100 : (((tagCount - min) * 100) / (max - min)));
                                int pixelSize = minSize + ((val * (maxSize - minSize)) / 100);

                                // Create the link with query string parameter
                                string paramUrl = URLHelper.AddParameterToUrl(url, QueryStringName, ValidationHelper.GetString(dr["TagID"], ""));
                                sb.Append("<span><a href=\"" + HTMLHelper.HTMLEncode(paramUrl) + "\" style=\"font-size:" + pixelSize.ToString() + "px;\" >" + HTMLHelper.HTMLEncode(dr["TagName"].ToString()) + "</a></span>");

                                index++;
                            }

                            renderedTags = sb.ToString();
                        }

                        // Save to cache
                        if (cs.Cached)
                        {
                            cs.CacheDependency = GetCacheDependency();
                        }

                        cs.Data = renderedTags;
                    }
                }

                if (String.IsNullOrEmpty(renderedTags))
                {
                    // Ensure no data behavior
                    if (HideControlForZeroRows)
                    {
                        Visible = false;
                    }
                    else
                    {
                        renderedTags = ZeroRowsText;
                    }
                }

                // Display the tags
                ltlTags.Text = renderedTags;
            }
            catch (Exception ex)
            {
                // Display the error
                ltlTags.Text = "<div style=\"color: red\">" + ex.Message + "</div>";
            }
        }
    }


    /// <summary>
    /// Gets the default cache dependencies for the data source.
    /// </summary>
    public override string GetDefaultCacheDependendencies()
    {
        string result = base.GetDefaultCacheDependendencies();

        // Add the document dependencies (based on path and site name)
        if (UseDocumentFilter)
        {
            if (result != null)
            {
                result += "\n";
            }
            result += String.Join("\n", DocumentDependencyCacheKeysBuilder.GetDependencyCacheKeys(siteName, null, aliasPath));
        }

        // Add tag group dependency
        if (tgi != null)
        {
            if (result != null)
            {
                result += "\n";
            }
            result += "cms.taggroup|byid|" + tgi.TagGroupID;
        }

        return result;
    }
}