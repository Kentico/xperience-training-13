using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.Base;
using CMS.DataEngine;
using CMS.SiteProvider;

public partial class CMSWebParts_Forums_ForumMostActiveThreads : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gest or sest the cache item name.
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
            forumDataSource.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, forumDataSource.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            forumDataSource.CacheDependencies = value;
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
            forumDataSource.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets transformation name.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), "");
        }
        set
        {
            SetValue("TransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets forum groups names, use semicolon like separator.
    /// </summary>
    public string ForumGroups
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ForumGroups"), "");
        }
        set
        {
            SetValue("ForumGroups", value);
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
            forumDataSource.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), String.Empty);
        }
        set
        {
            SetValue("Columns", value);
            forumDataSource.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets HideControlForZeroRows property.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), true);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            repMostActiveThread.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets ZeroRowsText property.
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
            repMostActiveThread.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets TopN.
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
            forumDataSource.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
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
            forumDataSource.SiteName = value;
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
            forumDataSource.ShowGroupPosts = value;
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


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
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
            if (!String.IsNullOrEmpty(TransformationName))
            {
                repMostActiveThread.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
                repMostActiveThread.HideControlForZeroRows = HideControlForZeroRows;
                repMostActiveThread.ZeroRowsText = ZeroRowsText;

                // Set data source groupid according to group name 
                if (!String.IsNullOrEmpty(GroupName))
                {
                    if (GroupName == PredefinedObjectType.COMMUNITY_CURRENT_GROUP)
                    {
                        forumDataSource.GroupID = ModuleCommands.CommunityGetCurrentGroupID();
                    }
                    else
                    {
                        GeneralizedInfo gi = ModuleCommands.CommunityGetGroupInfoByName(GroupName, SiteName);
                        if (gi != null)
                        {
                            forumDataSource.GroupID = ValidationHelper.GetInteger(gi.GetValue("GroupID"), 0);
                        }
                        else
                        {
                            forumDataSource.StopProcessing = true;
                        }
                    }
                }

                if (!forumDataSource.StopProcessing)
                {
                    forumDataSource.TopN = SelectTopN;
                    forumDataSource.OrderBy = "PostThreadPosts DESC";
                    forumDataSource.CacheItemName = CacheItemName;
                    forumDataSource.CacheDependencies = CacheDependencies;
                    forumDataSource.CacheMinutes = CacheMinutes;
                    forumDataSource.SelectOnlyApproved = false;
                    forumDataSource.SiteName = SiteName;
                    forumDataSource.ShowGroupPosts = ShowGroupPosts && String.IsNullOrEmpty(ForumGroups);
                    forumDataSource.SelectedColumns = Columns;

                    #region "Complete where condition"

                    string where = "";

                    // Get groups part of where condition
                    string[] groups = ForumGroups.Split(';');
                    foreach (string group in groups)
                    {
                        if (group != "")
                        {
                            if (where != "")
                            {
                                where += " OR ";
                            }
                            where += "(GroupName = N'" + SqlHelper.GetSafeQueryString(group, false) + "')";
                        }
                    }
                    where = "(" + (where == "" ? "(GroupName NOT LIKE 'AdHoc%')" : "(" + where + ")") + " AND (PostLevel = 0))";

                    // Append where condition and set PostLevel to 0 (only threads are needed)
                    // and filter out AdHoc forums
                    if (!String.IsNullOrEmpty(WhereCondition))
                    {
                        where += " AND (" + WhereCondition + ")";
                    }

                    #endregion


                    forumDataSource.WhereCondition = where;
                    forumDataSource.CheckPermissions = true;
                    repMostActiveThread.DataSourceControl = forumDataSource;
                    repMostActiveThread.DataBind();
                }
            }
        }
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide control for zero rows
        if (((repMostActiveThread.DataSource == null) || (DataHelper.DataSourceIsEmpty(repMostActiveThread.DataSource))) && (HideControlForZeroRows))
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        forumDataSource.ClearCache();
    }
}