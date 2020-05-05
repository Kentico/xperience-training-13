using System;
using System.Data;
using System.Web;
using System.Web.UI;

using CMS.Blogs;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.DocumentEngine;
using CMS.SiteProvider;

public partial class CMSWebParts_Blogs_RecentPosts : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, rptRecentPosts.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            rptRecentPosts.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Returns true if the control processing should be stopped.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            rptRecentPosts.StopProcessing = value;
        }
    }


    /// <summary>
    /// Gets or sets the TOP N value.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), rptRecentPosts.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            rptRecentPosts.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the path to the recent posts.
    /// </summary>
    public string PathToRecentPosts
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PathToRecentPosts"), rptRecentPosts.Path);
        }
        set
        {
            SetValue("PathToRecentPosts", value);
            rptRecentPosts.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), rptRecentPosts.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            rptRecentPosts.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control is not visible for empty datasource.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), rptRecentPosts.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            rptRecentPosts.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text value which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), rptRecentPosts.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            rptRecentPosts.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), rptRecentPosts.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            rptRecentPosts.SiteName = value;
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
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
            rptRecentPosts.StopProcessing = true;
        }
        else
        {
            rptRecentPosts.ControlContext = ControlContext;

            // Get current page info
            PageInfo currentPage = DocumentContext.CurrentPageInfo;

            // If recent posts is empty
            if (PathToRecentPosts == "")
            {
                // Get current parent blog
                TreeNode blogNode = BlogHelper.GetParentBlog(currentPage.NodeAliasPath, SiteContext.CurrentSiteName, false);

                // Set repeater path in accordance to blog node alias path
                if (blogNode != null)
                {
                    rptRecentPosts.Path = blogNode.NodeAliasPath + "/%";
                }
            }
            // If recent posts not empty
            else
            {
                rptRecentPosts.Path = PathToRecentPosts;
            }

            // Set repeater properties
            rptRecentPosts.TransformationName = TransformationName;
            rptRecentPosts.SelectTopN = SelectTopN;
            rptRecentPosts.HideControlForZeroRows = HideControlForZeroRows;
            rptRecentPosts.ZeroRowsText = ZeroRowsText;
            rptRecentPosts.CacheDependencies = CacheDependencies;
            rptRecentPosts.SiteName = SiteName;
        }
    }


    /// <summary>
    /// Clear cache.
    /// </summary>
    public override void ClearCache()
    {
        rptRecentPosts.ClearCache();
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        rptRecentPosts.ReloadData(true);
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        Visible = !StopProcessing;

        if (!rptRecentPosts.HasData() && HideControlForZeroRows)
        {
            Visible = false;
        }
        base.OnPreRender(e);
    }
}