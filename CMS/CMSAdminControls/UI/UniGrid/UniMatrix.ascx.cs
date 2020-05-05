using System;
using System.Collections.Generic;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Helpers;

using System.Text;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.UIControls;


public partial class CMSAdminControls_UI_UniGrid_UniMatrix : UniMatrix
{
    #region "Variables"

    private string mPageSizeOptions = "10,20,50,100,##ALL##";
    private bool? mHasData;

    #endregion


    #region "Properties"

    /// <summary>
    /// Items per page.
    /// </summary>
    public override int ItemsPerPage
    {
        get
        {
            return base.ItemsPerPage;
        }
        set
        {
            base.ItemsPerPage = value;
            pagerElem.UniPager.PageSize = value;
        }
    }


    /// <summary>
    /// Gets the value that indicates whether current selector in multiple mode displays some data or whether the dropdown contains some data.
    /// </summary>
    public override bool HasData
    {
        get
        {
            // Ensure the data
            if (!mHasData.HasValue && !StopProcessing)
            {
                ReloadData(false);
                mHasData = true;
            }

            return ValidationHelper.GetBoolean(ViewState["HasData"], false);
        }
        protected set
        {
            ViewState["HasData"] = value;
            mHasData = true;
        }
    }


    /// <summary>
    /// Page size options for pager.
    /// Numeric values or macro ##ALL## separated with comma.
    /// </summary>
    public override string PageSizeOptions
    {
        get
        {
            return mPageSizeOptions;
        }
        set
        {
            mPageSizeOptions = value;
            pagerElem.PageSizeOptions = value;
        }
    }


    /// <summary>
    /// UniPager control of UniMatrix.
    /// </summary>
    public override UniPager Pager
    {
        get
        {
            return pagerElem.UniPager;
        }
    }


    /// <summary>
    /// Gets or sets HTML content to be rendered as additional content on the top of the matrix.
    /// </summary>
    public override TableRow ContentBeforeRow
    {
        get
        {
            return trContentBefore;
        }
    }


    /// <summary>
    /// Gets or sets the maximum input length for the search field
    /// </summary>
    public int MaxFilterLength
    {
        get
        {
            return txtFilter.MaxLength;
        }
        set
        {
            txtFilter.MaxLength = value;
        }
    }

    #endregion


    #region "Control events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        pagerElem.PagedControl = this;
        pagerElem.UniPager.PageControl = "uniMatrix";
        pagerElem.PageSizeOptions = PageSizeOptions;
        pagerElem.UniPager.PageSize = ItemsPerPage;
        pagerElem.PageSizeDropdown.SelectedIndexChanged += drpPageSize_SelectedIndexChanged;

        ScriptHelper.RegisterJQuery(Page);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsCallback())
        {
            ReloadData(false);
        }
    }


    protected void drpPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        pagerElem.UniPager.CurrentPage = 1;
        ItemsPerPage = ValidationHelper.GetInteger(pagerElem.PageSizeDropdown.SelectedValue, -1);

        if (pagerElem.UniPager.PagedControl != null)
        {
            pagerElem.UniPager.PagedControl.ReBind();
        }
    }


    /// <summary>
    /// Filters the content.
    /// </summary>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        pagerElem.UniPager.CurrentPage = 1;

        // Get the expression
        txtFilter.Text = txtFilter.Text.Trim().Truncate(MaxFilterLength);
        string expr = txtFilter.Text;

        SetFilterWhere(expr);

        txtFilter.Focus();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the control data.
    /// </summary>
    /// <param name="forceReload">Force the reload of the control</param>
    public override void ReloadData(bool forceReload)
    {
        if (StopProcessing)
        {
            plcPager.Visible = false;
            tblMatrix.Visible = false;
            return;
        }

        base.ReloadData(forceReload);

        SetPageSize(forceReload);

        // Clear filter if forced reload
        if (forceReload)
        {
            txtFilter.Text = "";
            FilterWhere = null;
        }

        if (forceReload || !mLoaded)
        {
            // Prepare parameters for data loading
            int currentPage = pagerElem.UniPager.CurrentPage;
            string where = SqlHelper.AddWhereCondition(WhereCondition, FilterWhere);
            string orderBy = GetOrderByClause();

            mTotalRows = 0;
            bool headersOnly = false;

            // Load matrix data
            var headerData = LoadData(where, orderBy, currentPage, ref headersOnly);

            if (HasData)
            {
                tblMatrix.Visible = true;
                if (!headersOnly)
                {
                    RegisterScripts();
                }

                // Set CSS classes
                thcFirstColumn.CssClass = FirstColumnClass;
                tblMatrix.AddCssClass(CssClass);
                trContentBefore.AddCssClass(ContentBeforeRowCssClass);

                // Set the correct number of columns
                ColumnsCount = headerData.Count;
                mTotalRows = mTotalRows / ColumnsCount;

                GenerateMatrixContent(headerData, headersOnly);

                SetupMatrixFilter();

                // Show content before rows and pager
                ContentBeforeRow.Visible = ShowContentBeforeRow && !headersOnly;
            }
            else
            {
                tblMatrix.Visible = false;

                // If no-records message set, hide everything and show message
                if (!String.IsNullOrEmpty(NoRecordsMessage))
                {
                    lblInfo.Text = NoRecordsMessage;
                    lblInfo.Visible = true;
                }
            }

            // Setup pager visibility
            plcPager.Visible = HasData && !headersOnly;
            if (HasData)
            {
                // Set correct ID for direct page control
                pagerElem.DirectPageControlID = ((float)mTotalRows / pagerElem.CurrentPageSize > 20.0f) ? "txtPage" : "drpPage";
            }

            mLoaded = true;

            RaiseOnPageBinding();
        }
    }


    /// <summary>
    /// Sets pager to first page.
    /// </summary>
    public void ResetPager()
    {
        pagerElem.UniPager.CurrentPage = 1;
    }


    /// <summary>
    /// Resets the pager and filter.
    /// </summary>
    public void ResetMatrix()
    {
        pagerElem.UniPager.CurrentPage = 1;
        txtFilter.Text = String.Empty;
        FilterWhere = null;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Sets page size dropdown list according to PageSize property.
    /// </summary>
    private void SetPageSize(bool forceReload)
    {
        var pageSizeDropdown = pagerElem.PageSizeDropdown;

        SetPageSize(forceReload, pageSizeDropdown);
    }


    /// <summary>
    /// Generate matrix content
    /// </summary>
    /// <param name="matrixData">Data of the matrix to be generated</param>
    /// <param name="generateOnlyHeader">Indicates if only matrix header should be generated</param>
    private void GenerateMatrixContent(List<DataRow> matrixData, bool generateOnlyHeader)
    {
        GenerateMatrixHeader(matrixData, thrFirstRow);

        if (generateOnlyHeader)
        {
            lblInfoAfter.Text = NoRecordsMessage;
            lblInfoAfter.Visible = true;
        }
        else
        {
            GenerateMatrixBody(matrixData, tblMatrix);
        }
    }


    /// <summary>
    /// Register custom scripts for matrix
    /// </summary>
    private void RegisterScripts()
    {
        // Register the scripts
        string script =
            "function UM_ItemChanged_" + ClientID + "(item) {" + Page.ClientScript.GetCallbackEventReference(this, "item.id + ':' + item.checked", "UM_ItemSaved_" + ClientID, "item.id") + "; } \n" +
            "function UM_ItemSaved_" + ClientID + "(rvalue, context) { var contentBefore = $cmsj(\"#" + trContentBefore.ClientID + "\"); if(contentBefore){ contentBefore.replaceWith(rvalue);}}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UniMatrix_" + ClientID, ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Setup matrix filter
    /// </summary>
    private void SetupMatrixFilter()
    {
        // Show filter / header
        bool showFilter = ((FilterLimit <= 0) || !String.IsNullOrEmpty(FilterWhere) || (mTotalRows >= FilterLimit));
        pnlFilter.Visible = showFilter;

        // Show label in corner if text given and filter is hidden
        if (!showFilter && !string.IsNullOrEmpty(CornerText))
        {
            thcFirstColumn.Text += HTMLHelper.HTMLEncode(CornerText);
        }

        // Initialize filter if displayed
        if (ShowFilterRow && showFilter)
        {
            btnFilter.ResourceString = "general.search";
            thrFirstRow.AddCssClass("with-filter");
        }
        else if (!ShowHeaderRow)
        {
            plcFilter.Visible = false;
        }
    }

    #endregion
}
