using System;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_Layout_PageTemplateSelector : CMSAdminControl
{
    #region "Variables"

    private PageTemplateCategoryInfo mRootCategory;

    #endregion


    #region "Page template selector properties"

    /// <summary>
    /// If enabled ad-hoc templates will be displayed in selector.
    /// </summary>
    public bool ShowAdHoc
    {
        get;
        set;
    }

    #endregion


    #region "Selector properties"

    /// <summary>
    /// Gets or sets the value that indicates whether empty categories should be displayed.
    /// </summary>
    public bool ShowEmptyCategories
    {
        get
        {
            return treeElem.ShowEmptyCategories;
        }
        set
        {
            treeElem.ShowEmptyCategories = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether stratup focus fuctionallity is enabled.
    /// </summary>
    public bool UseStartUpFocus
    {
        get
        {
            return flatElem.UniFlatSelector.UseStartUpFocus;
        }
        set
        {
            flatElem.UniFlatSelector.UseStartUpFocus = value;
        }
    }


    /// <summary>
    /// Gets or set the flat panel selected item.
    /// </summary>
    public string SelectedItem
    {
        get
        {
            return flatElem.SelectedItem;
        }
        set
        {
            flatElem.SelectedItem = value;
        }
    }


    /// <summary>
    /// Selected item for tree.
    /// </summary>
    public String TreeSelectedCategory
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets selected item in tree.
    /// </summary>
    public string SelectedCategory
    {
        get
        {
            return ValidationHelper.GetString(treeElem.SelectedItem, "");
        }
        set
        {
            treeElem.SelectedItem = value;
        }
    }


    /// <summary>
    /// Gets or sets name of javascript function used for passing selected value from flat selector.
    /// </summary>
    public string SelectFunction
    {
        get
        {
            return flatElem.UniFlatSelector.SelectFunction;
        }
        set
        {
            flatElem.UniFlatSelector.SelectFunction = value;
        }
    }


    /// <summary>
    /// If enabled, flat selector remembers selected item trough postbacks.
    /// </summary>
    public bool RememberSelectedItem
    {
        get
        {
            return flatElem.UniFlatSelector.RememberSelectedItem;
        }
        set
        {
            flatElem.UniFlatSelector.RememberSelectedItem = value;
        }
    }


    /// <summary>
    /// Enables  or disables stop processing.
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
            flatElem.StopProcessing = value;
            treeElem.StopProcessing = value;
            EnableViewState = !value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            treeElem.IsLiveSite = value;
            flatElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets document id.
    /// </summary>
    public int DocumentID
    {
        get;
        set;
    }


    /// <summary>
    /// Whether selecting new page.
    /// </summary>
    public bool IsNewPage
    {
        get;
        set;
    }


    /// <summary>
    /// Root category ID.
    /// </summary>
    public int RootCategoryID
    {
        get;
        set;
    }


    /// <summary>
    /// Root category.
    /// </summary>
    private PageTemplateCategoryInfo RootCategory
    {
        get
        {
            if (mRootCategory == null)
            {
                mRootCategory = (RootCategoryID > 0) ? PageTemplateCategoryInfoProvider.GetPageTemplateCategoryInfo(RootCategoryID)
                    : PageTemplateCategoryInfoProvider.GetPageTemplateCategoryInfo("/");
            }

            return mRootCategory;
        }
    }

    #endregion


    #region "Page methods and events"

    /// <summary>
    /// OnInit.
    /// </summary>    
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        treeElem.SelectPath = "/";
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        SetRootCategory();

        if (!RequestHelper.IsPostBack())
        {
            // Preselect root category
            SelectRootCategory();
        }

        // Set node id
        flatElem.DocumentID = DocumentID;
        treeElem.DocumentID = DocumentID;

        flatElem.IsNewPage = IsNewPage;
        treeElem.IsNewPage = IsNewPage;
    }


    /// <summary>
    /// PreRender.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Pass currently selected category to flat selector
        if (RequestHelper.IsPostBack())
        {
            flatElem.TreeSelectedItem = treeElem.SelectedItem;
        }
    }


    /// <summary>
    /// On tree element item selected.
    /// </summary>
    /// <param name="selectedValue">Selected value</param> 
    protected void treeElem_OnItemSelected(string selectedValue)
    {
        flatElem.TreeSelectedItem = selectedValue;

        // Clear search box and pager
        flatElem.UniFlatSelector.ResetToDefault();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    /// <param name="reloadFlat">If true, flat selector is reloaded</param>
    public override void ReloadData(bool reloadFlat)
    {
        treeElem.ReloadData();
        if (reloadFlat)
        {
            flatElem.ReloadData();
        }
    }


    /// <summary>
    /// Sets the root category to the control
    /// </summary>
    protected void SetRootCategory()
    {
        if (RootCategory != null)
        {
            // Select and expand root node
            treeElem.RootPath = RootCategory.CategoryPath;
        }
    }


    /// <summary>
    /// Selects root category in tree, clears search condition and resets pager to first page.
    /// </summary>
    public void ResetToDefault()
    {
        SelectRootCategory();

        // Clear search condition and resets pager to first page
        flatElem.UniFlatSelector.ResetToDefault();
    }


    /// <summary>
    /// Selects the root category into selector.
    /// </summary>
    private void SelectRootCategory()
    {
        if (RootCategory != null)
        {
            flatElem.SelectedCategory = RootCategory;

            // Select and expand root node
            treeElem.SelectedItem = String.IsNullOrEmpty(TreeSelectedCategory) ? RootCategoryID.ToString() : TreeSelectedCategory;
            treeElem.SelectPath = RootCategory.CategoryPath;
        }
    }


    /// <summary>
    /// Add a reload script to the page which will update the page size (items count) according to the window size.
    /// </summary>
    /// <param name="forceResize">Indicates whether to invoke resizing of the page before calculating the items count</param>
    public void RegisterRefreshPageSizeScript(bool forceResize)
    {
        flatElem.RegisterRefreshPageSizeScript(forceResize);
    }

    #endregion
}