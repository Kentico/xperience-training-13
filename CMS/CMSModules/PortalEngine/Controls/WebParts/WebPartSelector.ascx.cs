using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_WebParts_WebPartSelector : CMSAdminControl
{
    #region "Webpart selector properties"

    /// <summary>
    /// Indicates whether inherited webpart will be displayed in selector.
    /// </summary>
    public bool ShowInheritedWebparts { get; set; } = true;


    /// <summary>
    /// Indicates whether show only basic webparts.
    /// </summary>
    public bool ShowOnlyBasicWebparts
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether display UI webparts.
    /// </summary>
    public bool ShowUIWebparts
    {
        get;
        set;
    }

    #endregion


    #region "Selector properties"

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

    #endregion


    #region "Page methods and events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        if (ShowOnlyBasicWebparts)
        {
            treeElem.MultipleRoots = false;
            treeUI.Visible = false;
            treeUI.StopProcessing = true;
        }

        treeElem.OnItemSelected += treeElem_OnItemSelected;
        treeUI.OnItemSelected += treeElem_OnItemSelected;


        string where = null;

        // Filter for inherited webparts
        if (!ShowInheritedWebparts)
        {
            where = SqlHelper.AddWhereCondition(flatElem.UniFlatSelector.WhereCondition, "WebPartParentID IS NULL");
        }

        if (!ShowUIWebparts)
        {
            treeUI.StopProcessing = true;
            treeUI.Visible = false;
        }
        else
        {
            treeElem.StopProcessing = true;
            treeElem.Visible = false;
        }

        flatElem.UniFlatSelector.WhereCondition = where;

        // Preselect root category
        if (!RequestHelper.IsPostBack())
        {
            ResetToDefault();
        }
    }


    /// <summary>
    /// Page prerender.
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
            flatElem.TreeSelectedItem = ShowUIWebparts ? treeUI.SelectedItem : treeElem.SelectedItem;
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
    /// Selects root category in tree, clears search condition and resets pager to first page.
    /// </summary>
    public void ResetToDefault()
    {
        string rootCategory = ShowUIWebparts ? "UIWebparts" : "/";

        // Get root webpart category
        WebPartCategoryInfo wci = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName(rootCategory);
        if (wci != null)
        {
            flatElem.SelectedCategory = wci;

            string selItem = wci.CategoryID.ToString();

            // Expand root node
            if (ShowUIWebparts)
            {
                treeUI.SelectedItem = selItem;
                treeUI.SelectPath = "/";

                // Select tree elem also for proper first time flat elem load (see prerender)
                treeElem.SelectedItem = selItem;
            }
            else
            {
                treeElem.SelectedItem = selItem;
                treeElem.SelectPath = "/";
            }

            flatElem.TreeSelectedItem = selItem;
        }

        // Clear search condition and resets pager to first page
        flatElem.UniFlatSelector.ResetToDefault();
    }

    #endregion
}