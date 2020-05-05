using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_Pager_UIPager : UIPager
{
    #region "Properties"

    /// <summary>
    /// Indicates if pager was already loaded.
    /// </summary>
    private bool PagerLoaded
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["PagerLoaded"], false);
        }
        set
        {
            ViewState["PagerLoaded"] = value;
        }
    }


    /// <summary>
    /// UniPager control.
    /// </summary>
    public override UniPager UniPager
    {
        get
        {
            return pagerElem;
        }
    }


    /// <summary>
    /// PageSize dropdown control.
    /// </summary>
    public override DropDownList PageSizeDropdown
    {
        get
        {
            return drpPageSize;
        }
    }


    /// <summary>
    /// Default page size at first load.
    /// </summary>
    public override int DefaultPageSize
    {
        get
        {
            return base.DefaultPageSize;
        }
        set
        {
            base.DefaultPageSize = value;
            SetupControls(true);
        }
    }


    /// <summary>
    /// Gets or sets current page size.
    /// </summary>
    public override int CurrentPageSize
    {
        get
        {
            if (PagerMode == UniPagerMode.Querystring)
            {
                return QueryHelper.GetInteger(PAGE_SIZE_QUERYSTRING_KEY, base.CurrentPageSize);
            }

            if (PageSizeDropdown.Visible)
            {
                return ValidationHelper.GetInteger(PageSizeDropdown.SelectedValue, base.CurrentPageSize);
            }

            return base.CurrentPageSize;
        }
        set
        {
            base.CurrentPageSize = value;
            SetupControls(true);
        }
    }


    /// <summary>
    /// Page size values separates with comma. 
    /// Macro ##ALL## indicates that all the results will be displayed at one page.
    /// </summary>
    public override string PageSizeOptions
    {
        get
        {
            return base.PageSizeOptions;
        }
        set
        {
            base.PageSizeOptions = value;
            SetupControls(true);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        SetupControls();

        pagerElem.OnBeforeTemplateLoading += PagerElemOnBeforeTemplateLoading;

        base.OnInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        SetupControls();

        drpPageSize.SelectedIndexChanged += drpPageSize_SelectedIndexChanged;

        base.OnLoad(e);
    }


    void PagerElemOnBeforeTemplateLoading(object sender, EventArgs e)
    {
        UniPager pager = (UniPager)sender;
        pager.DirectPageControlID = (pager.PageCount > 20) ? "txtPage" : "drpPage";
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        pagerElem.Visible = DisplayPager;
        plcPageSize.Visible = (DisplayPager && ShowPageSize && (drpPageSize.Items.Count > 1) && (UniPager.DataSourceItemsCount > 0));

        // Handle pager only if visible
        if (pagerElem.Visible)
        {
            if (UniPager.PageCount > UniPager.GroupSize)
            {
                LocalizedLabel lblPage = ControlsHelper.GetChildControl(UniPager, typeof(LocalizedLabel), "lblPage") as LocalizedLabel;
                using (Control drpPage = ControlsHelper.GetChildControl(UniPager, typeof(DropDownList), "drpPage"))
                {
                    using (Control txtPage = ControlsHelper.GetChildControl(UniPager, typeof(TextBox), "txtPage"))
                    {
                        if ((lblPage != null) && (drpPage != null) && (txtPage != null))
                        {
                            if (UniPager.PageCount > 20)
                            {
                                drpPage.Visible = false;
                                // Set labels associated control for US Section 508 validation
                                lblPage.AssociatedControlClientID = txtPage.ClientID;
                            }
                            else
                            {
                                txtPage.Visible = false;
                                // Set labels associated control for US Section 508 validation
                                lblPage.AssociatedControlClientID = drpPage.ClientID;
                            }
                        }
                    }
                }
            }
            else
            {
                // Remove direct page control if only one group of pages is  shown
                using (Control plcDirectPage = ControlsHelper.GetChildControl(UniPager, typeof(PlaceHolder), "plcDirectPage"))
                {
                    if (plcDirectPage != null)
                    {
                        plcDirectPage.Controls.Clear();
                    }
                }
            }
        }

        // Hide entire control if pager and page size drodown is hidden
        if (!plcPageSize.Visible && !pagerElem.Visible)
        {
            Visible = false;
        }

        PagerLoaded = true;
    }


    protected void drpPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        UniPager.CurrentPage = 1;
        UniPager.PageSize = ValidationHelper.GetInteger(drpPageSize.SelectedValue, -1);

        if (PagerMode == UniPagerMode.Querystring)
        {
            // Remove query string paging key to get to page 1
            string url = URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, UniPager.QueryStringKey);
            url = URLHelper.UpdateParameterInUrl(url, PAGE_SIZE_QUERYSTRING_KEY, UniPager.PageSize.ToString());
            URLHelper.Redirect(url);
        }
        else if (UniPager.PagedControl != null)
        {
            UniPager.PagedControl.ReBind();
        }
    }

    #endregion


    #region "Private methods"

    private void SetupControls(bool forceReload = false)
    {
        SetPageSize(forceReload);

        Visible = true;
        UniPager.PageSize = ValidationHelper.GetInteger(drpPageSize.SelectedValue, -1);
        UniPager.DirectPageControlID = DirectPageControlID;
    }


    /// <summary>
    /// Sets page size dropdown list according to PageSize property.
    /// </summary>
    private void SetPageSize(bool forceReload)
    {
        if ((drpPageSize.Items.Count == 0) || forceReload)
        {
            string currentPagesize = CurrentPageSize.ToString();

            if (!PagerLoaded && (PagerMode != UniPagerMode.Querystring))
            {
                currentPagesize = DefaultPageSize.ToString();
            }

            drpPageSize.Items.Clear();

            PageSizeOptionsData pageSizeOptionsData;

            if (!TryParsePageSizeOptions(PageSizeOptions, out pageSizeOptionsData))
            {
                Service.Resolve<IEventLogService>().LogError("UIPager", "ParseCustomOptions", "Could not parse custom page size options: '" + PageSizeOptions + "'. Correct format is values separated by comma.");
                TryParsePageSizeOptions(DEFAULT_PAGE_SIZE_OPTIONS, out pageSizeOptionsData);
            }

            // Add default page size if not present
            if ((DefaultPageSize > 0) && !pageSizeOptionsData.Options.Contains(DefaultPageSize))
            {
                pageSizeOptionsData.Options.Add(DefaultPageSize);
            }

            // Sort list of page sizes
            pageSizeOptionsData.Options.Sort();

            FillPageSizeDropdown(pageSizeOptionsData, currentPagesize);
        }
    }


    private void FillPageSizeDropdown(PageSizeOptionsData pageSizeOptionsData, string currentPagesize)
    {
        ListItem item;
        foreach (int size in pageSizeOptionsData.Options)
        {
            item = new ListItem(size.ToString());
            if (item.Value == currentPagesize)
            {
                item.Selected = true;
            }
            drpPageSize.Items.Add(item);
        }

        // Add 'Select ALL' macro at the end of list
        if (pageSizeOptionsData.ContainsAll)
        {
            item = new ListItem(GetString("general.selectall"), "-1");
            if (currentPagesize == "-1")
            {
                item.Selected = true;
            }
            drpPageSize.Items.Add(item);
        }
    }

    #endregion
}