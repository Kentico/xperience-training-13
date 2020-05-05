using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_UniSelector_UniFlatSelector : UniFlatSelector, IPostBackEventHandler, ICallbackEventHandler
{
    #region "Variables"

    // Repeater
    private string mOrderBy = string.Empty;
    private string mWhereCondition = string.Empty;
    private int mSelectTopN = 0;
    private string mQueryName = string.Empty;
    private string mSelectedColumns = string.Empty;

    // Pager 
    private int mPageSize = 10;
    private UniPagerMode mPagingMode = UniPagerMode.Querystring;
    private string mQueryStringKey = "";
    private bool mDisplayFirstLastAutomatically = false;
    private bool mDisplayPreviousNextAutomatically = false;
    private bool mHidePagerForSinglePage = false;
    private int mMaxPages = 200;

    // Other
    private string callBackHandlerFunction = string.Empty;
    private string selectItemFunction = string.Empty;

    private bool searchExecuted = false;

    #endregion


    #region "Repeater properties"

    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public override string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(mOrderBy, repItems.OrderBy);
        }
        set
        {
            mOrderBy = value;
            repItems.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(mWhereCondition, repItems.WhereCondition);
        }
        set
        {
            mWhereCondition = value;
            repItems.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public override int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(mSelectTopN, repItems.TopN);
        }
        set
        {
            repItems.TopN = value;
            mSelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the query name.
    /// </summary>
    public override string QueryName
    {
        get
        {
            return DataHelper.GetNotEmpty(mQueryName, repItems.QueryName);
        }
        set
        {
            mQueryName = value;
            repItems.QueryName = value;
        }
    }


    /// <summary>
    /// Gets or sets the selected columns.
    /// </summary>
    public override string SelectedColumns
    {
        get
        {
            return DataHelper.GetNotEmpty(mSelectedColumns, repItems.SelectedColumns);
        }
        set
        {
            mSelectedColumns = value;
            repItems.SelectedColumns = value;
        }
    }


    /// <summary>
    /// CSS selector describing items in this selector.
    /// </summary>
    public string ItemsCSSSelector
    {
        get
        {
            return "#" + pnlRepeater.ClientID + " div[id^='fi_']";
        }
    }

    #endregion


    #region "Unipager properties"

    /// <summary>
    /// Gets or sets page size.
    /// </summary>
    public override int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(mPageSize, pgrItems.CurrentPageSize);
        }
        set
        {
            mPageSize = value;
            pgrItems.DefaultPageSize = value;
            pgrItems.CurrentPageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets search option.
    /// </summary>
    public override UniPagerMode PagingMode
    {
        get
        {
            return mPagingMode;
        }
        set
        {
            mPagingMode = value;
            pgrItems.PagerMode = value;
        }
    }


    /// <summary>
    /// Gets or sets query string key.
    /// </summary>
    public override string QueryStringKey
    {
        get
        {
            return mQueryStringKey;
        }
        set
        {
            mQueryStringKey = value;
            pgrItems.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets group size.
    /// </summary>
    public override int GroupSize
    {
        get
        {
            return pgrItems.GroupSize;
        }
        set
        {
            pgrItems.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public override bool DisplayFirstLastAutomatically
    {
        get
        {
            return mDisplayFirstLastAutomatically;
        }
        set
        {
            mDisplayFirstLastAutomatically = value;
            pgrItems.UniPager.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public override bool DisplayPreviousNextAutomatically
    {
        get
        {
            return mDisplayPreviousNextAutomatically;
        }
        set
        {
            mDisplayPreviousNextAutomatically = value;
            pgrItems.UniPager.DisplayPreviousNextAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether pager should be hidden for single page.
    /// </summary>
    public override bool HidePagerForSinglePage
    {
        get
        {
            return mHidePagerForSinglePage;
        }
        set
        {
            mHidePagerForSinglePage = value;
            pgrItems.HidePagerForSinglePage = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager max pages.
    /// </summary>
    public override int MaxPages
    {
        get
        {
            return mMaxPages;
        }
        set
        {
            mMaxPages = value;
            pgrItems.UniPager.MaxPages = value;
        }
    }


    /// <summary>
    /// If set, size of one page won't be computed automatically by JavaScript (method <see cref="RegisterRefreshPageSizeScript"/>) and size of the page will be constant.
    /// If value is null, size of one page will by computed dynamically.
    /// </summary>
    public int? ManualPageSize
    {
        get;
        set;
    }

    #endregion


    #region "Other properties"

    /// <summary>
    /// Gets or sets the value that indicates whether first item should be selected
    /// </summary>
    public bool PreSelectFirstItem
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the additional placeholder
    /// </summary>
    public PlaceHolder AdditionalPlaceHolder
    {
        get
        {
            return plcAdditional;
        }
    }


    /// <summary>
    /// Gets or sets selected item.
    /// </summary>
    public override string SelectedItem
    {
        get
        {
            return hdnSelectedItem.Value;
        }
        set
        {
            hdnSelectedItem.Value = value;
        }
    }


    /// <summary>
    /// Gets the client ID of hidden field with selected item value.
    /// </summary>
    public string SelectedItemFieldId
    {
        get
        {
            return hdnSelectedItem.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets error text.
    /// </summary>
    public string ErrorText
    {
        get
        {
            return lblError.Text;
        }

        set
        {
            lblError.Text = value;
        }
    }


    /// <summary>
    /// Gets the search check box. The check box behavior is to be defined in the upper control.
    /// </summary>
    public CMSCheckBox SearchCheckBox
    {
        get
        {
            return chkSearch;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event argumets</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Set default selected value to javascript-side variable
        if (!RequestHelper.IsPostBack())
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "FirstSelectorValue", ScriptHelper.GetScript("selectedValue = '" + ScriptHelper.GetString(SelectedItem, false) + "'; selectedSkipDialog = false; selectedFlatItem = $cmsj('.FlatSelectedItem'); selectedItemName = selectedFlatItem.find('.SelectorFlatText').text().trim();"));
        }

        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterFlatResize(Page);

        // Ensure that the number of the displayed items has always a maximum set
        int itemsCount = ValidationHelper.GetInteger(hdnItemsCount.Value, 0);
        if (itemsCount < 1)
        {
            itemsCount = 0;
        }

        // Set the page size. This value is set by a javascript function according the window size. If ManualPageSize is set, it has higher priority than value from JavaScript.
        PageSize = ManualPageSize ?? itemsCount;


        // Generate unique name for generic select function
        selectItemFunction = ClientID + "_SelectItem";
        callBackHandlerFunction = ClientID + "_Handler";

        // Custom function name
        if (!String.IsNullOrEmpty(JavascriptFunction))
        {
            selectItemFunction = JavascriptFunction;
        }

        ScriptHelper.RegisterStartupScript(this, typeof(string), "SelfValue", ScriptHelper.GetScript("function UniFlat_GetSelectedValue(){ objItem = document.getElementById('" + SelectedItemFieldId + "'); if (objItem != null) { return objItem.value; } return ''; }"));

        repItems.ItemTemplate = ItemTemplate;
        repItems.AlternatingItemTemplate = AlternatingItemTemplate;
        repItems.FooterTemplate = FooterTemplate;
        repItems.HeaderTemplate = HeaderTemplate;
        repItems.SeparatorTemplate = SeparatorTemplate;

        // Disable caching (cannot guarantee valid cache item)
        repItems.CacheMinutes = 0;

        // Create and register delayed search
        string delayedSearchScript =
            @"var timer = null ;             

            function SetupSearch()
            {                         
                var txtSearch = $cmsj('#" + txtSearch.ClientID + @"');
                if (txtSearch) {
                    txtSearch.keyup(StartTimeout);
                }
            }
          
            function StartTimeout()
            {                                
                clearTimeout(timer);                
                timer = setTimeout('OnTimeout()', 700);                
            }

            function OnTimeout()
            {                                         
                " + ControlsHelper.GetPostBackEventReference(btnUpdate, ClientID + "_search") + @"                        
            }  
            
            SetupSearch();
            ";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "DelayedSearch", ScriptHelper.GetScript(delayedSearchScript));
        btnUpdate.Click += btnUpdate_Click;

        // Create and register select item script
        if (String.IsNullOrEmpty(JavascriptFunction))
        {
            string selectItemScript = (!String.IsNullOrEmpty(CustomSelectItemFunction)) ? CustomSelectItemFunction :
                                                                                                                       "function " + selectItemFunction + @"(value, sender)
            {           
                // Remove old selection    
                selectedFlatItem = $cmsj('div .FlatSelectedItem');  
                if (selectedFlatItem != null)
                {
                    selectedFlatItem.removeClass('FlatSelectedItem');
                    selectedFlatItem.addClass('FlatItem');
                }
                
                // Add new selection
                selectedFlatItem = $cmsj(sender);
                selectedFlatItem.removeClass('FlatItem');
                selectedFlatItem.addClass('FlatSelectedItem');
                
                selectedValue = value;
                selectedSkipDialog = (sender.getAttribute('data-skipdialog') == 1);
                selectedItemName = selectedFlatItem.find('div .SelectorFlatText').text().trim();
                $cmsj('#" + SelectedItemFieldId + "').val(value);\n" +
                                                                                                                       "var callBackArgument = value;\n"
                                                                                                                       + Page.ClientScript.GetCallbackEventReference(this, "callBackArgument", callBackHandlerFunction, callBackHandlerFunction) + "}";

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), selectItemFunction, ScriptHelper.GetScript(selectItemScript));
        }

        // Create and register callback handler script 
        string callBackHandlerScript = (!String.IsNullOrEmpty(CustomCallBackHandlerFunction)) ? CustomCallBackHandlerFunction :
                                                                                                                                  "function " + callBackHandlerFunction + @"(content, context) 
            {   
                var pnlDescription = $cmsj('div .selector-flat-description'); 
                if (pnlDescription != null) {
                    if (content != '') {
                        pnlDescription.addClass('selector-flat-description-background');
                    }
                    else {
                        pnlDescription.removeClass('selector-flat-description-background');
                    }
                    pnlDescription.html(content);
                }
            } 
            ";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), callBackHandlerFunction, ScriptHelper.GetScript(callBackHandlerScript));

        // If select function is defined, hook double click on it
        if (!String.IsNullOrEmpty(SelectFunction))
        {
            // Create and register script for double click
            string doubleclickScript = (!String.IsNullOrEmpty(CustomDoubleClickFunction)) ? CustomDoubleClickFunction :
                                                                                                                          @"// Set selected item in tree" +
                                                                                                                          "\n$cmsj(\"" + ItemsCSSSelector + "\").each(function() {" +
                                                                                                                          @"var jThis = $cmsj(this);
                jThis.bind('dblclick', function(){ "
                                                                                                                          + SelectFunction + @"(jThis.attr('id').substring(3), this.getAttribute('data-skipdialog')==1) ;    
                });
            });            
            ";

            ScriptHelper.RegisterStartupScript(pnlUpdate, typeof(string), "DoubleClick", ScriptHelper.GetScript(doubleclickScript));
        }

        // Render reload script only if user has not set page size manually
        if (ManualPageSize == null)
        {
            if (!RequestHelper.IsPostBack())
            {
                // Add a reload script to the page which will update the page size (items count) according to the window size.
                RegisterRefreshPageSizeScript(false);
            }
        }
    }


    /// <summary>
    /// Add a reload script to the page which will update the page size (items count) according to the window size.
    /// </summary>
    /// <param name="forceResize">Indicates whether to invoke resizing of the page before calculating the items count</param>
    public void RegisterRefreshPageSizeScript(bool forceResize)
    {
        // This method needs the "FlatResize.js" script to be registered
        // Add a reload script to the page which will update the page size (items count) according to the window size
        StringBuilder script = new StringBuilder();
        script.Append("$cmsj(function() {");

        // Invoke resizing of the page before calculating the items count
        if (forceResize)
        {
            script.Append(@"
                // Get items
                getItems(true);
                // Initial resize
                resizeareainternal();
                // get the total count of items which can fit into the page
                uniFlatItemsCount = getItemsCount();");
        }

        script.Append(@"
            var hdnItemsCount = document.getElementById('" + hdnItemsCount.ClientID + @"');
            if (hdnItemsCount != null) { hdnItemsCount.value = uniFlatItemsCount; } " +
                      ControlsHelper.GetPostBackEventReference(btnUpdate, string.Empty) + ";" +
                      "})");

        ScriptHelper.RegisterStartupScript(this, typeof(string), "GetItemsCount", ScriptHelper.GetScript(script.ToString()));
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        pgrItems.PagedControl = repItems;
    }


    /// <summary>
    /// On PreRender.
    /// </summary>    
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            return;
        }
        else if (!string.IsNullOrEmpty(ErrorText))
        {
            pnlLabel.Visible = true;
            lblError.Visible = true;
            return;
        }

        // Search condition
        string searchText = SearchText;
        if (!RequestHelper.IsPostBack())
        {
            searchText = QueryHelper.GetString("searchtext", SearchText);
        }

        txtSearch.Text = searchText;

        // Add search condition
        if (!string.IsNullOrEmpty(searchText))
        {
            // Avoid SQL injection
            searchText = SqlHelper.EscapeQuotes(searchText);

            string[] columns = SearchColumn.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string columnsWhere = string.Empty;

            foreach (string column in columns)
            {
                columnsWhere = SqlHelper.AddWhereCondition(columnsWhere, column + " LIKE N'%" + SqlHelper.EscapeLikeText(searchText) + "%'", "OR");
            }

            repItems.WhereCondition = SqlHelper.AddWhereCondition(repItems.WhereCondition, columnsWhere);
        }

        // Handle TOP N for first load
        if (repItems.TopN == 0)
        {
            int currentPage = pgrItems.CurrentPage;

            // Minimum is first group
            int currentGroup = currentPage / pgrItems.GroupSize + ((currentPage % pgrItems.GroupSize > 0) ? 1 : 0);
            int topN = currentGroup * pgrItems.GroupSize * pgrItems.DefaultPageSize + pgrItems.DefaultPageSize;
            repItems.TopN = topN;
        }
        
        if (!String.IsNullOrEmpty(SelectedItem))
        {
            var clearSelectedItemScript = String.Empty;

            if (!RememberSelectedItem)
            {
                // Clear selected item value if remembering is disabled (category change, paging, ...)
                SelectedItem = string.Empty;
                clearSelectedItemScript = @"
selectedFlatItem = null;
selectedValue = null;
selectedItemName = null;";
            }
            
            // Get description
            string description = RaiseOnItemSelected(SelectedItem);

            // Create startup script which optionally clears selected item and updates description if selected item is not removed
            var startupScript = clearSelectedItemScript + "\r\n" + callBackHandlerFunction + "('" + description + "');";

            // Register startup script
            ScriptHelper.RegisterStartupScript(this, typeof(string), ClientID + "_StartupScript", ScriptHelper.GetScript(startupScript));
        }

        // Different no records messages based on search
        lblNoRecords.Text = GetString(String.IsNullOrEmpty(SearchText) ? NoRecordsMessage : NoRecordsSearchMessage);

        repItems.ReloadData(true);

        // Show no results found
        if (!repItems.HasData())
        {
            pnlLabel.Visible = true;
            lblNoRecords.Visible = true;
        }

        // Set focus on the search text box when required
        if (!searchExecuted && UseStartUpFocus)
        {
            RegisterFocusScript();
        }
    }


    /// <summary>
    /// Render event.
    /// </summary>
    protected override void Render(HtmlTextWriter writer)
    {
        // Register event argument value to pass event validation
        Page.ClientScript.RegisterForEventValidation(btnUpdate.UniqueID, ClientID + "_search");

        base.Render(writer);
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// Button search click.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        SearchText = txtSearch.Text;
        RaiseOnSearch();
    }

    bool firstItem = true;

    /// <summary>
    /// Repeater item databound.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event argumets</param>
    protected void repItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.DataItem != null)
        {
            DataRow row = ((DataRowView)e.Item.DataItem).Row;

            if (PreSelectFirstItem && firstItem)
            {
                SelectedItem = Convert.ToString(row[ValueColumn]);
                //hdnSelectedItem.EnableViewState = true;
            }
            string value = ValidationHelper.GetString(row[ValueColumn], "");
            string cssClass = ((value == SelectedItem)) ? "FlatSelectedItem" : "FlatItem";

            firstItem = false;

            string skipPropertiesDialogAttribute = string.Empty;

            // Add the attribute indicating that the item (web part, widget...) should be inserted without the property dialog
            if (!string.IsNullOrEmpty(SkipPropertiesDialogColumn) && ValidationHelper.GetBoolean(row[SkipPropertiesDialogColumn], false))
            {
                skipPropertiesDialogAttribute = "data-skipdialog=\"1\"";
            }

            // Add javascript function
            string link = selectItemFunction + "(" + ScriptHelper.GetString(value) + ", this )";
            
            // Add postback event reference
            if (UsePostback)
            {
                if (!string.IsNullOrEmpty(link))
                {
                    link += ";";
                }

                link += ControlsHelper.GetPostBackEventReference(this, HTMLHelper.HTMLEncode(value));
            }

            // Add envelope
            e.Item.Controls.AddAt(0, new LiteralControl("<div id=\"fi_" + value + "\" class=\"" + cssClass + "\" onclick=\"" + link + ";\"" + skipPropertiesDialogAttribute + " >"));
            e.Item.Controls.Add(new LiteralControl("</div>"));
        }
    }


    /// <summary>
    /// Updates content update panel.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Event argumets</param>
    private void btnUpdate_Click(object sender, EventArgs e)
    {
        SearchText = txtSearch.Text;
        RaiseOnSearch();
        searchExecuted = true;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    public void ReloadData()
    {
        if (!StopProcessing)
        {
            repItems.ReloadData(true);
        }
    }


    /// <summary>
    /// Returns url to flat item image.
    /// </summary>
    /// <param name="metafileGuid">Meta file guid. If value is empty not available image is used</param>    
    public string GetFlatImageUrl(object metafileGuid)
    {
        if (String.IsNullOrEmpty(ValidationHelper.GetString(metafileGuid, "")))
        {
            return HTMLHelper.HTMLEncode(ResolveUrl(NotAvailableImageUrl));
        }
        else
        {
            return HTMLHelper.HTMLEncode(ResolveUrl("~/CMSPages/GetMetaFile.aspx") + "?maxsidesize=" + ImageMaxSideSize + "&fileguid=" + metafileGuid);
        }
    }


    /// <summary>
    /// Clears searched text.
    /// </summary>
    public override void ClearSearchText()
    {
        SearchText = String.Empty;
        txtSearch.Text = String.Empty;
    }


    /// <summary>
    /// Reset pager to the first page.
    /// </summary>
    public override void ResetPager()
    {
        pgrItems.CurrentPage = 1;
    }


    /// <summary>
    /// Clears search condition and resets pager to first page.
    /// </summary>
    public override void ResetToDefault()
    {
        ClearSearchText();
        ResetPager();
        SelectedItem = "";
        if (ControlsHelper.IsInUpdatePanel(this))
        {
            ControlsHelper.UpdateCurrentPanel(this);
        }
    }


    private void RegisterFocusScript()
    {
        string txtSearchFocus =
            @"function Focus()
            {                
                var textbox = document.getElementById('" + txtSearch.ClientID + @"') ;            
                if (textbox != null)
                {
                    textbox.focus();                    
                }
            }            
            setTimeout('Focus()', 100);";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "TextboxFocus", ScriptHelper.GetScript(txtSearchFocus));
    }

    #endregion


    #region IPostBackEventHandler Members

    public void RaisePostBackEvent(string eventArgument)
    {
        SelectedItem = eventArgument;
        RaiseOnItemSelected(eventArgument);
    }

    #endregion


    #region ICallbackEventHandler Members

    private string callbackArgument = string.Empty;

    public string GetCallbackResult()
    {
        SelectedItem = callbackArgument;
        return RaiseOnItemSelected(SelectedItem);
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        callbackArgument = eventArgument;
    }

    #endregion
}
