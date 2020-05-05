using System;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_General_OutputCacheDependencies : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Search box width.
    /// </summary>
    public bool UseDefaultDependencies
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseDefaultDependencies"), true);
        }
        set
        {
            SetValue("UseDefaultDependencies", value);
        }
    }


    /// <summary>
    /// Search results box dimension.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CacheDependencies"), "");
        }
        set
        {
            SetValue("CacheDependencies", value);
        }
    }

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (PortalContext.ViewMode.IsLiveSite() && (CacheDependencies != ""))
            {
                base.OnPreRender(e);

                // Use default cache dependencies
                if (UseDefaultDependencies)
                {
                    DocumentContext.AddDefaultOutputCacheDependencies();
                }

                // More than one dependency is set
                string[] dependencies = CacheHelper.GetDependencyCacheKeys(CacheDependencies);
                CacheHelper.AddOutputCacheDependencies(dependencies);
            }
        }
    }
}