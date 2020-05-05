using System;
using System.Data;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;

public partial class CMSWebParts_Forums_ForumTopContributors : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return CacheHelper.GetCacheDependencies(base.CacheDependencies, "cms.user|all");
        }
        set
        {
            base.CacheDependencies = CacheHelper.GetCacheDependencies(value, "cms.user|all");
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
                repTopContributors.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
                repTopContributors.HideControlForZeroRows = HideControlForZeroRows;
                repTopContributors.ZeroRowsText = ZeroRowsText;

                DataSet ds = null;

                // Try to get data from cache
                using (var cs = new CachedSection<DataSet>(ref ds, CacheMinutes, true, CacheItemName, "forumtopcontributors", SiteContext.CurrentSiteName, WhereCondition, SelectTopN))
                {
                    if (cs.LoadData)
                    {
                        // Get the data
                        ds = GetData();

                        // Save to the cache
                        if (cs.Cached)
                        {
                            // Save to the cache
                            cs.CacheDependency = GetCacheDependency();
                        }

                        cs.Data = ds;
                    }
                }

                repTopContributors.DataSource = ds;

                if (!DataHelper.DataSourceIsEmpty(repTopContributors.DataSource))
                {
                    repTopContributors.DataBind();
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
        if (((repTopContributors.DataSource == null) || (DataHelper.DataSourceIsEmpty(repTopContributors.DataSource))) && (HideControlForZeroRows))
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Returns the Users data.
    /// </summary>
    private DataSet GetData()
    {
        string where = "(UserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID = " + SiteContext.CurrentSiteID + "))";

        if (!String.IsNullOrEmpty(WhereCondition))
        {
            where += " AND (" + WhereCondition + ")";
        }

        return UserInfoProvider.GetUsersDataWithSettings().Where(new WhereCondition(where)).TopN(SelectTopN).OrderByDescending("UserForumPosts");
    }

    #endregion
}