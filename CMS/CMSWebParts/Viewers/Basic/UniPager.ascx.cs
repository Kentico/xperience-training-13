using System;

using CMS.Base;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Basic_UniPager : CMSAbstractWebPart
{
    #region "Public properties"


    #region "Pager properties"

    /// <summary>
    /// Gets or sets the value that indicates whether scroll position should be cleared after post back paging
    /// </summary>
    public bool ResetScrollPositionOnPostBack
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ResetScrollPositionOnPostBack"), pagerElem.ResetScrollPositionOnPostBack);
        }
        set
        {
            SetValue("ResetScrollPositionOnPostBack", value);
            pagerElem.ResetScrollPositionOnPostBack = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of web part which should be paged. Use this property only for web parts.
    /// </summary>
    public string TargetControlName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TargetControlName"), pagerElem.FilterTypePageControl);
        }
        set
        {
            SetValue("TargetControlName", value);
            pagerElem.FilterTypePageControl = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of records to display on a page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), pagerElem.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            pagerElem.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of pages displayed for current page range.
    /// </summary>
    public int GroupSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("GroupSize"), pagerElem.GroupSize);
        }
        set
        {
            SetValue("GroupSize", value);
            pagerElem.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager mode ('querystring' or 'postback').
    /// </summary>
    public string PagingMode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagingMode"), "querystring");
        }
        set
        {
            if (value != null)
            {
                SetValue("PagingMode", value);
                switch (value.ToLowerCSafe())
                {
                    case "postback":
                        pagerElem.PagerMode = UniPagerMode.PostBack;
                        break;
                    default:
                        pagerElem.PagerMode = UniPagerMode.Querystring;
                        break;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets the querysting parameter.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("QueryStringKey"), pagerElem.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            pagerElem.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayFirstLastAutomatically
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFirstLastAutomatically"), pagerElem.DisplayFirstLastAutomatically);
        }
        set
        {
            SetValue("DisplayFirstLastAutomatically", value);
            pagerElem.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayPreviousNextAutomatically
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayPreviousNextAutomatically"), pagerElem.DisplayPreviousNextAutomatically);
        }
        set
        {
            SetValue("DisplayPreviousNextAutomatically", value);
            pagerElem.DisplayPreviousNextAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether pager should be hidden for single page.
    /// </summary>
    public bool HidePagerForSinglePage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HidePagerForSinglePage"), pagerElem.HidePagerForSinglePage);
        }
        set
        {
            SetValue("HidePagerForSinglePage", value);
            pagerElem.HidePagerForSinglePage = value;
        }
    }

    #endregion


    #region "Template properties"

    /// <summary>
    /// Gets or sets the pages template.
    /// </summary>
    public string PagesTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Pages"), "");
        }
        set
        {
            SetValue("Pages", value);
        }
    }


    /// <summary>
    /// Gets or sets the current page template.
    /// </summary>
    public string CurrentPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CurrentPage"), "");
        }
        set
        {
            SetValue("CurrentPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the separator template.
    /// </summary>
    public string SeparatorTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageSeparator"), "");
        }
        set
        {
            SetValue("PageSeparator", value);
        }
    }


    /// <summary>
    /// Gets or sets the first page template.
    /// </summary>
    public string FirstPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FirstPage"), "");
        }
        set
        {
            SetValue("FirstPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the last page template.
    /// </summary>
    public string LastPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LastPage"), "");
        }
        set
        {
            SetValue("LastPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the previous page template.
    /// </summary>
    public string PreviousPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousPage"), "");
        }
        set
        {
            SetValue("PreviousPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the next page template.
    /// </summary>
    public string NextPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextPage"), "");
        }
        set
        {
            SetValue("NextPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the previous group template.
    /// </summary>
    public string PreviousGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousGroup"), "");
        }
        set
        {
            SetValue("PreviousGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets the next group template.
    /// </summary>
    public string NextGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextGroup"), "");
        }
        set
        {
            SetValue("NextGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets the layout template.
    /// </summary>
    public string LayoutTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerLayout"), "");
        }
        set
        {
            SetValue("PagerLayout", value);
        }
    }


    /// <summary>
    /// Gets or sets the direct page template.
    /// </summary>
    public string DirectPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DirectPage"), "");
        }
        set
        {
            SetValue("DirectPage", value);
        }
    }

    #endregion


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
    /// Initializes the control properties.
    /// </summary>
    public void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // If target control is not specified do nothing
            if (!String.IsNullOrEmpty(TargetControlName))
            {
                #region "Pager properties"

                // Set pager properties
                pagerElem.FilterTypePageControl = TargetControlName;
                pagerElem.PageSize = PageSize;
                pagerElem.GroupSize = GroupSize;
                pagerElem.QueryStringKey = QueryStringKey;
                pagerElem.DisplayFirstLastAutomatically = DisplayFirstLastAutomatically;
                pagerElem.DisplayPreviousNextAutomatically = DisplayPreviousNextAutomatically;
                pagerElem.HidePagerForSinglePage = HidePagerForSinglePage;
                pagerElem.ResetScrollPositionOnPostBack = ResetScrollPositionOnPostBack;

                // Set pager mode
                switch (PagingMode.ToLowerCSafe())
                {
                    case "postback":
                        pagerElem.PagerMode = UniPagerMode.PostBack;
                        break;
                    default:
                        pagerElem.PagerMode = UniPagerMode.Querystring;
                        break;
                }

                #endregion


                #region "Pager templates"

                // Pages
                if (!String.IsNullOrEmpty(PagesTemplate))
                {
                    pagerElem.PageNumbersTemplate = TransformationHelper.LoadTransformation(pagerElem, PagesTemplate);
                }

                // Current page
                if (!String.IsNullOrEmpty(CurrentPageTemplate))
                {
                    pagerElem.CurrentPageTemplate = TransformationHelper.LoadTransformation(pagerElem, CurrentPageTemplate);
                }

                // Separator
                if (!String.IsNullOrEmpty(SeparatorTemplate))
                {
                    pagerElem.PageNumbersSeparatorTemplate = TransformationHelper.LoadTransformation(pagerElem, SeparatorTemplate);
                }

                // First page
                if (!String.IsNullOrEmpty(FirstPageTemplate))
                {
                    pagerElem.FirstPageTemplate = TransformationHelper.LoadTransformation(pagerElem, FirstPageTemplate);
                }

                // Last page
                if (!String.IsNullOrEmpty(LastPageTemplate))
                {
                    pagerElem.LastPageTemplate = TransformationHelper.LoadTransformation(pagerElem, LastPageTemplate);
                }

                // Previous page
                if (!String.IsNullOrEmpty(PreviousPageTemplate))
                {
                    pagerElem.PreviousPageTemplate = TransformationHelper.LoadTransformation(pagerElem, PreviousPageTemplate);
                }

                // Next page
                if (!String.IsNullOrEmpty(NextPageTemplate))
                {
                    pagerElem.NextPageTemplate = TransformationHelper.LoadTransformation(pagerElem, NextPageTemplate);
                }

                // Previous group
                if (!String.IsNullOrEmpty(PreviousGroupTemplate))
                {
                    pagerElem.PreviousGroupTemplate = TransformationHelper.LoadTransformation(pagerElem, PreviousGroupTemplate);
                }

                // Next group
                if (!String.IsNullOrEmpty(NextGroupTemplate))
                {
                    pagerElem.NextGroupTemplate = TransformationHelper.LoadTransformation(pagerElem, NextGroupTemplate);
                }

                // Direct page
                if (!String.IsNullOrEmpty(DirectPageTemplate))
                {
                    pagerElem.DirectPageTemplate = TransformationHelper.LoadTransformation(pagerElem, DirectPageTemplate);
                }

                // Layout
                if (!String.IsNullOrEmpty(LayoutTemplate))
                {
                    pagerElem.LayoutTemplate = TransformationHelper.LoadTransformation(pagerElem, LayoutTemplate);
                }

                #endregion
            }
        }
    }


    /// <summary>
    /// OnPreRender - check visibility.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Add event handler
        Page.SaveStateComplete += new EventHandler(Page_SaveStateComplete);
    }


    /// <summary>
    /// Checks whether current control should be visible.
    /// </summary>
    private void Page_SaveStateComplete(object sender, EventArgs e)
    {
        // Check visibility
        if ((HidePagerForSinglePage && pagerElem.PageCount < 2) || (pagerElem.DataSourceItemsCount == 0))
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        SetupControl();
        pagerElem.ReloadData(true);

        base.ReloadData();
    }

    #endregion
}