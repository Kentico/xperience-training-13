using System;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Widgets_Controls_WidgetSelector : CMSAdminControl
{
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

        treeElem.OnItemSelected += treeElem_OnItemSelected;

        if (!RequestHelper.IsPostBack())
        {
            // Select root by default
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
    /// Selects root category in tree, clears search condition and resets pager to first page.
    /// </summary>
    public void ResetToDefault()
    {
        // Select root by default            
        WidgetCategoryInfo wci = WidgetCategoryInfoProvider.GetWidgetCategoryInfo("/");
        if (wci != null)
        {
            flatElem.SelectedCategory = wci;

            // Select and expand root node                
            treeElem.SelectedItem = wci.WidgetCategoryID.ToString();
            treeElem.SelectPath = "/";
        }

        // Clear search condition and resets pager to first page
        flatElem.UniFlatSelector.ResetToDefault();
    }

    #endregion
}