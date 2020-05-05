using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Blogs;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.DocumentEngine;
using CMS.SiteProvider;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSWebParts_Blogs_PostArchive : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the TOP N value.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), rptPostArchive.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            rptPostArchive.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), rptPostArchive.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            rptPostArchive.TransformationName = value;
        }
    }


    /// <summary>
    ///  Gets or sets the value that indicates whether control is not visible for empty datasource.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), rptPostArchive.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            rptPostArchive.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text value which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), rptPostArchive.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            rptPostArchive.ZeroRowsText = value;
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
            rptPostArchive.CacheItemName = value;
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
            rptPostArchive.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return base.CacheDependencies;
        }
        set
        {
            base.CacheDependencies = value;
            rptPostArchive.CacheDependencies = value;
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
    /// Item databound handler.
    /// </summary>
    protected void rptPostArchive_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        // Select only publish is depend on view mode
        bool selectOnlyPublished = (PageManager.ViewMode.IsLiveSite());

        // Get month NodeID        
        int parentId = DataHelper.GetIntValue(((DataRowView)e.Item.DataItem).Row, "NodeID");

        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Posts count, default '0'
        int count = 0;
        // Try to get data from cache
        using (var cs = new CachedSection<int>(ref count, CacheMinutes, true, CacheItemName, "postarchivecount", SiteContext.CurrentSiteName, rptPostArchive.Path, LocalizationContext.PreferredCultureCode, parentId, selectOnlyPublished))
        {
            if (cs.LoadData)
            {
                count = tree.SelectNodesCount(SiteContext.CurrentSiteName, rptPostArchive.Path, LocalizationContext.PreferredCultureCode, false, null, "NodeParentID = " + parentId, "", -1, selectOnlyPublished);

                // Save to cache
                if (cs.Cached)
                {
                    cs.CacheDependency = GetCacheDependency();
                }

                cs.Data = count;
            }
        }

        // Set post count as text to the label control in repeater transformation
        if (e.Item.Controls.Count > 0)
        {
            // Try find label control with id 'lblPostCount'
            Label lblCtrl = e.Item.Controls[0].FindControl("lblPostCount") as Label;
            if (lblCtrl != null)
            {
                lblCtrl.Text = count.ToString();
            }
        }
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            //Do nothing
            rptPostArchive.StopProcessing = true;
        }
        else
        {
            rptPostArchive.ControlContext = ControlContext;

            // Get current page info
            PageInfo currentPage = DocumentContext.CurrentPageInfo;

            // Select only publish is depend on view mode
            bool selectOnlyPublished = (PageManager.ViewMode.IsLiveSite());

            TreeNode blogNode = null;
            // Try to get data from cache
            using (var cs = new CachedSection<TreeNode>(ref blogNode, CacheMinutes, true, CacheItemName, "postarchivenode", currentPage.NodeAliasPath, SiteContext.CurrentSiteName, selectOnlyPublished))
            {
                if (cs.LoadData)
                {
                    blogNode = BlogHelper.GetParentBlog(currentPage.NodeAliasPath, SiteContext.CurrentSiteName, selectOnlyPublished);

                    // Save to cache
                    if (cs.Cached)
                    {
                        cs.CacheDependency = GetCacheDependency();
                    }

                    cs.Data = blogNode;
                }
            }

            // Set repeater path in accordance to blog node alias path
            if (blogNode != null)
            {
                rptPostArchive.Path = blogNode.NodeAliasPath + "/%";
            }
            else
            {
                rptPostArchive.Path = currentPage.NodeAliasPath + "/%";
            }

            // Set repeater properties
            rptPostArchive.TransformationName = TransformationName;
            rptPostArchive.SelectTopN = SelectTopN;
            rptPostArchive.HideControlForZeroRows = HideControlForZeroRows;
            rptPostArchive.ZeroRowsText = ZeroRowsText;
            rptPostArchive.CacheItemName = CacheItemName;
            rptPostArchive.CacheDependencies = CacheDependencies;
            rptPostArchive.CacheMinutes = CacheMinutes;
        }
    }


    /// <summary>
    /// Causes reloading data.
    /// </summary>
    public override void ReloadData()
    {
        // Call parent reload data
        base.ReloadData();

        // Re-call setup control to load current properties
        SetupControl();

        // Repeater reload data
        rptPostArchive.ReloadData(true);
    }


    /// <summary>
    /// Clear cache.
    /// </summary>
    public override void ClearCache()
    {
        rptPostArchive.ClearCache();
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        Visible = !StopProcessing;

        if (!rptPostArchive.HasData() && HideControlForZeroRows)
        {
            Visible = false;
        }
        base.OnPreRender(e);
    }
}