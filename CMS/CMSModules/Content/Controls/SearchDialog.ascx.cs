using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Search;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Content_Controls_SearchDialog : CMSUserControl
{
    #region "Variables"

    private string currentSiteName;
    private int? mSiteCulturesCount;

    public const string SQL = "##SQL##";
    public const string ANY = "##ANY##";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets search webpart ID.
    /// </summary>
    public string SearchControlID
    {
        get;
        set;
    }


    /// <summary>
    /// Site cultures count.
    /// </summary>
    private int SiteCulturesCount
    {
        get
        {
            if (mSiteCulturesCount == null)
            {
                DataSet dsCultures = CultureSiteInfoProvider.GetSiteCultures(currentSiteName);
                mSiteCulturesCount = !DataHelper.DataSourceIsEmpty(dsCultures) ? dsCultures.Tables[0].Rows.Count : 0;
            }
            return mSiteCulturesCount.Value;
        }
    }


    /// <summary>
    /// Gets selected index.
    /// </summary>
    public string SelectedIndex
    {
        get
        {
            return drpIndexes.SelectedValue;
        }
    }


    /// <summary>
    /// Gets WHERE condition for SQL search.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return GetWhere();
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        currentSiteName = SiteContext.CurrentSiteName;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        rfvText.ErrorMessage = GetString("general.requiresvalue");

        drpIndexes.AutoPostBack = true;
        drpIndexes.SelectedIndexChanged += drpIndexes_SelectedIndexChanged;
        btnSearch.Click += btnSearch_Click;

        // Init cultures
        cultureElem.AllowDefault = false;
        cultureElem.UpdatePanel.RenderMode = UpdatePanelRenderMode.Inline;

        if (!RequestHelper.IsPostBack())
        {
            LoadDropDowns();
            SetControls();
        }

        SetCulture();
    }


    /// <summary>
    /// OnPreRender.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Hide column with languages if only one culture is assigned to the site
        if (SiteCulturesCount <= 1)
        {
            plcLang.Visible = false;
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Sets WHERE condition.
    /// </summary>
    private string GetWhere()
    {
        var where = new WhereCondition();
        var oper = EnumStringRepresentationExtensions.ToEnum<QueryOperator>(QueryHelper.GetString("searchlanguage", null));
        var val = QueryHelper.GetString("searchculture", "##ANY##");
        if (String.IsNullOrEmpty(val))
        {
            val = "##ANY##";
        }

        if (val != "##ANY##")
        {
            // Create base query
            var tree = new TreeProvider();
            var query = tree.SelectNodes()
                            .All()
                            .Column("NodeID");

            switch (val)
            {
                case "##ALL##":
                    {
                        query.GroupBy("NodeID").Having(string.Format("(COUNT(NodeID) {0} {1})", oper.ToStringRepresentation(), SiteCulturesCount));

                        where.WhereIn("NodeID", query);
                    }
                    break;

                default:
                    {
                        query.WhereEquals("DocumentCulture", val);

                        if (oper == QueryOperator.NotEquals)
                        {
                            where.WhereNotIn("NodeID", query);
                        }
                        else
                        {
                            where.WhereIn("NodeID", query);
                        }
                    }
                    break;
            }
        }
        else if (oper == QueryOperator.NotEquals)
        {
            where.NoResults();
        }

        return where.ToString(true);
    }


    /// <summary>
    /// Sets controls from query string parameters.
    /// </summary>
    private void SetControls()
    {
        drpLanguage.SelectedValue = QueryHelper.GetString("searchlanguage", "=");
        cultureElem.Value = QueryHelper.GetString("searchculture", ANY);
        cultureElem.ReloadData();
        drpIndexes.SelectedValue = QueryHelper.GetString("searchindex", SQL);
        chkOnlyPublished.Checked = QueryHelper.GetBoolean("searchpublished", true);
        if (drpIndexes.SelectedValue != SQL)
        {
            plcPublished.Visible = false;
        }
        drpSearchMode.SelectedValue = QueryHelper.GetString("searchmode", "");
        txtSearchFor.Text = QueryHelper.GetString("searchtext", null);
    }


    private void SetCulture()
    {
        string selectedCulture = cultureElem.Value.ToString();
        string selected = drpIndexes.SelectedValue;
        if (String.IsNullOrEmpty(selected))
        {
            selected = SQL;
        }

        cultureElem.UniSelector.SpecialFields.Add(new SpecialField
        {
            Text = GetString("transman.anyculture"),
            Value = ANY
        });

        if (selected != SQL)
        {
            drpLanguage.Visible = false;
            pnlCultureElem.CssClass = "filter-form-value-cell-wide";
        }
        else
        {
            cultureElem.UniSelector.SpecialFields.Add(new SpecialField
            {
                Text = GetString("transman.allcultures"),
                Value = "##ALL##"
            });
            drpLanguage.Visible = true;
            pnlCultureElem.CssClass = "filter-form-value-cell";
        }

        cultureElem.Value = selectedCulture;
        cultureElem.ReloadData();
    }


    /// <summary>
    /// Loads drop-down lists.
    /// </summary>
    private void LoadDropDowns()
    {
        // Init operands
        if (drpLanguage.Items.Count == 0)
        {
            drpLanguage.Items.Add(new ListItem(GetString("transman.translatedto"), "="));
            drpLanguage.Items.Add(new ListItem(GetString("transman.nottranslatedto"), "<>"));
        }

        // Get site indexes
        var indexes = SearchIndexSiteInfoProvider.GetSiteIndexes(SiteContext.CurrentSiteID)
            .Where("IndexType", QueryOperator.Equals, TreeNode.OBJECT_TYPE)
            .Where("IndexProvider", QueryOperator.Equals, SearchIndexInfo.LUCENE_SEARCH_PROVIDER);

        foreach (SearchIndexInfo index in indexes)
        {
            drpIndexes.Items.Add(new ListItem(index.IndexDisplayName, index.IndexName));
        }

        drpIndexes.Items.Insert(0, new ListItem(GetString("search.sqlsearch"), SQL));

        // Init Search for drop down list
        ControlsHelper.FillListControlWithEnum<SearchModeEnum>(drpSearchMode, "srch.dialog", useStringRepresentation: true);
        drpSearchMode.SelectedValue = QueryHelper.GetString("searchmode", EnumHelper.GetDefaultValue<SearchModeEnum>().ToStringRepresentation());
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Index changed.
    /// </summary>
    private void drpIndexes_SelectedIndexChanged(object sender, EventArgs e)
    {
        plcPublished.Visible = (drpIndexes.SelectedValue == SQL);
        SetCulture();
    }


    /// <summary>
    /// Search button click.
    /// </summary>
    private void btnSearch_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(txtSearchFor.Text.Trim()))
        {
            string url = RequestContext.CurrentURL;

            // Remove pager query string
            url = URLHelper.RemoveParameterFromUrl(url, "pagesearchresults");
            url = URLHelper.RemoveParameterFromUrl(url, "pagesize");

            // Update search text parameter
            url = URLHelper.UpdateParameterInUrl(url, "searchtext", HttpUtility.UrlEncode(txtSearchFor.Text));

            // Update search mode parameter
            url = URLHelper.UpdateParameterInUrl(url, "searchmode", HttpUtility.UrlEncode(drpSearchMode.SelectedValue));

            // Update search for published items
            if (chkOnlyPublished.Visible)
            {
                url = URLHelper.UpdateParameterInUrl(url, "searchpublished", chkOnlyPublished.Checked.ToString());
            }

            // Update selected search index
            url = URLHelper.UpdateParameterInUrl(url, "searchindex", HttpUtility.UrlEncode(drpIndexes.SelectedValue));

            // Update selected language
            if (plcLang.Visible)
            {
                url = URLHelper.UpdateParameterInUrl(url, "searchlanguage", HttpUtility.UrlEncode(drpLanguage.SelectedValue));
                url = URLHelper.UpdateParameterInUrl(url, "searchculture", HttpUtility.UrlEncode(ValidationHelper.GetString(cultureElem.Value, null)));
            }

            // Redirect
            URLHelper.Redirect(url);
        }
    }

    #endregion
}