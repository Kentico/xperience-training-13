using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_Layout_LayoutFlatSelector : CMSAdminControl
{
    #region "Flat selector properties"

    /// <summary>
    /// Retruns inner instance of UniFlatSelector control.    
    /// </summary>
    public UniFlatSelector UniFlatSelector
    {
        get
        {
            return flatElem;
        }
    }


    /// <summary>
    /// Gets or sets selected item in flat selector.
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
    /// Indicates if the control should perform the operations.
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
            EnableViewState = !value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        ScriptHelper.RegisterJQuery(Page);

        // Setup flat selector
        flatElem.QueryName = "cms.layout.selectall";
        flatElem.ValueColumn = "LayoutID";
        flatElem.SearchLabelResourceString = "layouts.layoutname";
        flatElem.SearchColumn = "LayoutDisplayName";
        flatElem.SelectedColumns = "LayoutID, LayoutCodeName, LayoutDisplayName, LayoutThumbnailGUID, LayoutIconClass";
        flatElem.PageSize = 15;
        flatElem.OrderBy = "LayoutDisplayName";
        flatElem.NotAvailableImageUrl = GetImageUrl("Objects/CMS_Layout/notavailable.png");
        flatElem.NoRecordsMessage = "layouts.norecordsincategory";
        flatElem.NoRecordsSearchMessage = "layouts.norecords";

        flatElem.OnItemSelected += flatElem_OnItemSelected;
    }


    /// <summary>
    /// On PreRender.
    /// </summary>    
    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Description area
        litCategory.Text = ShowInDescriptionArea(SelectedItem);
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// Updates description after item is selected in flat selector.
    /// </summary>
    protected string flatElem_OnItemSelected(string selectedValue)
    {
        return ShowInDescriptionArea(selectedValue);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        flatElem.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Add a reload script to the page which will update the page size (items count) according to the window size.
    /// </summary>
    /// <param name="forceResize">Indicates whether to invoke resizing of the page before calculating the items count</param>
    public void RegisterRefreshPageSizeScript(bool forceResize)
    {
        flatElem.RegisterRefreshPageSizeScript(forceResize);
    }


    /// <summary>
    /// Generates HTML text to be used in description area.
    /// </summary>
    ///<param name="selectedValue">Selected item for which generate description</param>
    private string ShowInDescriptionArea(string selectedValue)
    {
        string description = String.Empty;

        if (!String.IsNullOrEmpty(selectedValue))
        {
            int layoutId = ValidationHelper.GetInteger(selectedValue, 0);
            DataSet ds = LayoutInfoProvider.GetLayouts()
                .WhereEquals("LayoutID", layoutId)
                .Columns("LayoutDisplayName", "LayoutDescription");

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                description = ResHelper.LocalizeString(ValidationHelper.GetString(ds.Tables[0].Rows[0]["LayoutDescription"], ""));
            }
        }

        if (!String.IsNullOrEmpty(description))
        {
            return "<div class=\"Description\">" + HTMLHelper.HTMLEncode(description) + "</div>";
        }

        return String.Empty;
    }

    #endregion
}
