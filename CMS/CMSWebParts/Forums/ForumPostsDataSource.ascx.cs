using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.Base;
using CMS.DataEngine;
using CMS.SiteProvider;

public partial class CMSWebParts_Forums_ForumPostsDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the web service URL.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
            srcPosts.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets forum name for which blog posts should be obtained.
    /// </summary>
    public string ForumName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ForumName"), "");
        }
        set
        {
            SetValue("ForumName", value);
            srcPosts.ForumName = value;
        }
    }


    /// <summary>
    /// Gets or sets Select only approved property.
    /// </summary>
    public bool SelectOnlyApproved
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyApproved"), true);
        }
        set
        {
            SetValue("SelectOnlyApproved", value);
            srcPosts.SelectOnlyApproved = value;
        }
    }


    /// <summary>
    /// Gets or sets Check permissions property.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), true);
        }
        set
        {
            SetValue("CheckPermissions", value);
            srcPosts.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets WHERE condition.
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
            srcPosts.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "");
        }
        set
        {
            SetValue("OrderBy", value);
            srcPosts.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets top N selected documents.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), 0);
        }
        set
        {
            SetValue("SelectTopN", value);
            srcPosts.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), "");
        }
        set
        {
            SetValue("FilterName", value);
            srcPosts.SourceFilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            srcPosts.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcPosts.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcPosts.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            srcPosts.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gest or sets selected columns.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), "");
        }
        set
        {
            SetValue("Columns", value);
            srcPosts.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Indicates if group posts should be included.
    /// </summary>
    public bool ShowGroupPosts
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowGroupPosts"), false);
        }
        set
        {
            SetValue("ShowGroupPosts", value);
            srcPosts.ShowGroupPosts = value;
        }
    }


    /// <summary>
    /// Gets or sets community group name.
    /// </summary>
    public string GroupName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GroupName"), "");
        }
        set
        {
            SetValue("GroupName", value);
        }
    }

    #endregion


    #region "Methods"

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
            // Do nothing
        }
        else
        {
            srcPosts.SiteName = SiteName;
            srcPosts.ForumName = ForumName;
            srcPosts.WhereCondition = WhereCondition;
            srcPosts.OrderBy = OrderBy;
            srcPosts.TopN = SelectTopN;
            srcPosts.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcPosts.SourceFilterName = FilterName;
            srcPosts.CacheItemName = CacheItemName;
            srcPosts.CacheDependencies = CacheDependencies;
            srcPosts.CacheMinutes = CacheMinutes;
            srcPosts.SelectOnlyApproved = SelectOnlyApproved;
            srcPosts.CheckPermissions = CheckPermissions;
            srcPosts.SelectedColumns = Columns;
            srcPosts.ShowGroupPosts = ShowGroupPosts && String.IsNullOrEmpty(ForumName);

            // Set data source groupid according to group name
            if (!String.IsNullOrEmpty(GroupName))
            {
                if (GroupName == PredefinedObjectType.COMMUNITY_CURRENT_GROUP)
                {
                    srcPosts.GroupID = ModuleCommands.CommunityGetCurrentGroupID();
                }
                else
                {
                    GeneralizedInfo gi = ModuleCommands.CommunityGetGroupInfoByName(GroupName, SiteName);
                    if (gi != null)
                    {
                        srcPosts.GroupID = ValidationHelper.GetInteger(gi.GetValue("GroupID"), 0);
                    }
                    else
                    {
                        srcPosts.StopProcessing = true;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcPosts.ClearCache();
    }

    #endregion
}