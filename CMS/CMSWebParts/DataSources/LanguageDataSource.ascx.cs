using System;
using System.Linq;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_DataSources_LanguageDataSource : CMSAbstractWebPart
{
    #region Properties

    /// <summary>
    /// Indicates whether the link for current culture should be created. Default value is false.
    /// </summary>
    public bool ExcludeCurrentCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExcludeCurrentCulture"), false);
        }
        set
        {
            SetValue("ExcludeCurrentCulture", value);
        }
    }


    /// <summary>
    /// Indicates whether a collection of culture specific data is generated for documents that aren’t translated. By default, data is generated for translated documents only.
    /// </summary>
    public bool ExcludeUntranslatedDocuments
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExcludeUntranslatedDocuments"), true);
        }
        set
        {
            SetValue("ExcludeUntranslatedDocuments", value);
        }
    }

    #endregion


    #region "Methods""

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            return;
        }
        var node = DocumentContext.CurrentDocument;

        srcCultureBasedURLs.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
        srcCultureBasedURLs.CacheItemName = CacheItemName;
        srcCultureBasedURLs.CacheDependencies = CacheDependencies;
        srcCultureBasedURLs.CacheMinutes = CacheMinutes;
        srcCultureBasedURLs.Node = node;
        srcCultureBasedURLs.ExcludedCultureCode = (ExcludeCurrentCulture) ? node.DocumentCulture : "";
        srcCultureBasedURLs.UrlOptions = UrlOptionsEnum.UseCultureSpecificURLs;

        if (ExcludeUntranslatedDocuments)
        {
            srcCultureBasedURLs.UrlOptions |= UrlOptionsEnum.ExcludeUntranslatedDocuments;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcCultureBasedURLs.ClearCache();
    }

    #endregion
}
