using System;
using System.Collections.Generic;
using System.Web;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Search;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_SmartSearch_Controls_SearchDialog : CMSUserControl, ISearchFilterable
{
    #region "Variables"

    private string mSearchForLabel = ResHelper.GetString("srch.dialog.searchfor");
    private string mSearchButton = ResHelper.GetString("general.search");
    private string mSearchModeLabel = ResHelper.GetString("srch.dialog.mode");
    private SearchModeEnum mSearchMode = EnumHelper.GetDefaultValue<SearchModeEnum>();
    private bool mShowSearchMode = true;
    private bool mShowOnlySearchButton = false;
    private string mFilterID = "";
    private string mResultWebpartID = "";
    private IPagesActivityLogger mPagesActivityLogger = Service.Resolve<IPagesActivityLogger>();

    // Filter support
    private List<string> mFilterUrlParameters = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// The text to show when the control has no value.
    /// </summary>
    public string WatermarkText
    {
        get
        {
            return txtSearchFor.WatermarkText;
        }
        set
        {
            txtSearchFor.WatermarkText = value;
        }
    }


    /// <summary>
    /// The CSS class to apply to the TextBox when it has no value (e.g. the watermark text is shown).
    /// </summary>
    public string WatermarkCssClass
    {
        get
        {
            return txtSearchFor.WatermarkCssClass;
        }
        set
        {
            txtSearchFor.WatermarkCssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the Filter URL parameters
    /// </summary>
    public List<string> FilterUrlParameters
    {
        get
        {
            if (mFilterUrlParameters == null)
            {
                mFilterUrlParameters = new List<string>();
            }

            return mFilterUrlParameters;
        }
        set
        {
            mFilterUrlParameters = value;
        }
    }


    /// <summary>
    /// Gets or sets the label search for text.
    /// </summary>
    public string SearchForLabel
    {
        get
        {
            return mSearchForLabel;
        }
        set
        {
            mSearchForLabel = value;
            lblSearchFor.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether search mode settings should be displayed.
    /// </summary>
    public bool ShowSearchMode
    {
        get
        {
            return mShowSearchMode;
        }
        set
        {
            mShowSearchMode = value;
            plcSearchMode.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only search button should be displayed.
    /// </summary>
    public bool ShowOnlySearchButton
    {
        get
        {
            return mShowOnlySearchButton;
        }
        set
        {
            mShowOnlySearchButton = value;
            plcSearchOptions.Visible = !value;
        }
    }


    /// <summary>
    /// Gets or sets the search button text.
    /// </summary>
    public string SearchButton
    {
        get
        {
            return mSearchButton;
        }
        set
        {
            mSearchButton = value;
            btnSearch.Text = value;
        }
    }


    /// <summary>
    ///  Gets or sets the search mode.
    /// </summary>
    public SearchModeEnum SearchMode
    {
        get
        {
            return mSearchMode;
        }
        set
        {
            mSearchMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the search mode label text.
    /// </summary>
    public string SearchModeLabel
    {
        get
        {
            return mSearchModeLabel;
        }
        set
        {
            mSearchModeLabel = value;
            lblSearchMode.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the search filter id.
    /// </summary>
    public string FilterID
    {
        get
        {
            return mFilterID;
        }
        set
        {
            mFilterID = value;
        }
    }


    /// <summary>
    /// Gets or sets the search mode label text.
    /// </summary>
    public string ResultWebpartID
    {
        get
        {
            return mResultWebpartID;
        }
        set
        {
            mResultWebpartID = value;
        }
    }


    /// <summary>
    /// Gets or sets filter condition - not implemented.
    /// </summary>
    public string FilterSearchCondition
    {
        get
        {
            return null;
        }
        set
        {
            ;
        }
    }

    /// <summary>
    /// Gets or sets filter search sort - not implemented.
    /// </summary>
    public string FilterSearchSort
    {
        get
        {
            return null;
        }
        set
        {
            ;
        }
    }

    #endregion


    #region "Page and controls events"

    /// <summary>
    /// Page load.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }
        else
        {
            // Set up drop down list
            if (ShowSearchMode)
            {
                if (!RequestHelper.IsPostBack())
                {
                    // Fill dropdownlist option with values
                    ControlsHelper.FillListControlWithEnum<SearchModeEnum>(drpSearchMode, "srch.dialog", useStringRepresentation: true);
                    drpSearchMode.SelectedValue = QueryHelper.GetString("searchmode", SearchMode.ToStringRepresentation());
                }
            }

            // Set up search text  
            if (!RequestHelper.IsPostBack() && (!ShowOnlySearchButton))
            {
                txtSearchFor.Text = QueryHelper.GetString("searchtext", "");
            }
        }
    }


    /// <summary>
    /// Fires at btn search click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string url = RequestContext.CurrentURL;

        // Remove pager query string
        url = URLHelper.RemoveParameterFromUrl(url, "page");

        // Update search text parameter
        url = URLHelper.UpdateParameterInUrl(url, "searchtext", HttpUtility.UrlEncode(txtSearchFor.Text));

        // Update search mode parameter
        url = URLHelper.RemoveParameterFromUrl(url, "searchmode");
        if (ShowSearchMode)
        {
            url = URLHelper.AddParameterToUrl(url, "searchmode", drpSearchMode.SelectedValue);
        }
        else
        {
            url = URLHelper.AddParameterToUrl(url, "searchmode", SearchMode.ToStringRepresentation());
        }

        // Add filter params to url
        foreach (string urlParam in FilterUrlParameters)
        {
            string[] urlParams = urlParam.Split('=');
            url = URLHelper.UpdateParameterInUrl(url, urlParams[0], urlParams[1]);
        }

        // Log "internal search" activity
        mPagesActivityLogger.LogInternalSearch(txtSearchFor.Text, DocumentContext.CurrentDocument);

        // Redirect
        URLHelper.Redirect(url);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Applies filter.
    /// </summary>
    /// <param name="searchCondition">Search condition</param>
    /// <param name="searchSort">Search sort</param>
    /// <param name="filterPostback">If true filter caused the postback which means that filter condition has been changed.</param>
    public void ApplyFilter(string searchCondition, string searchSort, bool filterPostback)
    {
        // Call Result webpart id
        ISearchFilterable resultWebpart = (ISearchFilterable)CMSControlsHelper.GetFilter(ResultWebpartID);
        if (resultWebpart != null)
        {
            resultWebpart.ApplyFilter(searchCondition, searchSort, filterPostback);
        }
    }


    /// <summary>
    /// Adds filter option to url.
    /// </summary>
    /// <param name="searchWebpartID">Webpart id</param>
    /// <param name="options">Options</param>
    public void AddFilterOptionsToUrl(string searchWebpartID, string options)
    {
        FilterUrlParameters.Add(searchWebpartID + "=" + options);
    }


    /// <summary>
    /// Loads data.
    /// </summary>
    public void LoadData()
    {
        // Register control for filter
        CMSControlsHelper.SetFilter(FilterID, this);
    }

    #endregion
}