using System;

using CMS.Community;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_Community_MediaLibrary_GroupMediaLibraryDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), String.Empty);
        }
        set
        {
            SetValue("WhereCondition", value);
            srcMedia.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), String.Empty);
        }
        set
        {
            SetValue("OrderBy", value);
            srcMedia.OrderBy = value;
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
            srcMedia.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), String.Empty);
        }
        set
        {
            SetValue("FilterName", value);
            srcMedia.SourceFilterName = value;
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
            srcMedia.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcMedia.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcMedia.CacheDependencies = value;
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
            srcMedia.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gest or sets selected columns.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), String.Empty);
        }
        set
        {
            SetValue("Columns", value);
            srcMedia.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public string GroupName
    {
        get
        {
            string groupName = ValidationHelper.GetString(GetValue("GroupName"), "");
            if ((string.IsNullOrEmpty(groupName) || groupName == GroupInfoProvider.CURRENT_GROUP) && (CommunityContext.CurrentGroup != null))
            {
                return CommunityContext.CurrentGroup.GroupName;
            }
            if (groupName == GroupInfoProvider.CURRENT_GROUP)
            {
                return QueryHelper.GetString("groupname", "");
            }
            return groupName;
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
            srcMedia.WhereCondition = WhereCondition;
            srcMedia.OrderBy = OrderBy;
            srcMedia.TopN = SelectTopN;
            srcMedia.FilterName = ID;
            srcMedia.SourceFilterName = FilterName;
            srcMedia.CacheItemName = CacheItemName;
            srcMedia.CacheDependencies = CacheDependencies;
            srcMedia.CacheMinutes = CacheMinutes;
            srcMedia.SelectedColumns = Columns;

            // Set group ID
            srcMedia.GroupID = -1;
            if (!string.IsNullOrEmpty(GroupName))
            {
                GroupInfo gi = GroupInfoProvider.GetGroupInfo(GroupName, SiteContext.CurrentSiteName);
                if (gi != null)
                {
                    srcMedia.GroupID = gi.GroupID;
                }
            }
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcMedia.ClearCache();
    }

    #endregion
}