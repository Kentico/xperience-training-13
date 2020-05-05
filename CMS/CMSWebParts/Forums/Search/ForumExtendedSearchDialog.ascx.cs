using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.WebAnalytics;

public partial class CMSWebParts_Forums_Search_ForumExtendedSearchDialog : CMSAbstractWebPart
{
    #region Private fields

    private IPagesActivityLogger mPagesActivityLogger = Service.Resolve<IPagesActivityLogger>();
    
    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether user can select forums to search.
    /// </summary>
    public bool EnableForumSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableForumSelection"), true);
        }
        set
        {
            SetValue("EnableForumSelection", value);
        }
    }


    /// <summary>
    /// Gets or sets the URL where is the search result web part.
    /// </summary>
    public string RedirectUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectUrl"), "");
        }
        set
        {
            SetValue("RedirectUrl", value);
        }
    }


    /// <summary>
    /// Gets or sets forum groups which are displayed in forum selection.
    /// </summary>
    public string ForumGroups
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ForumGroups"), "");
        }
        set
        {
            SetValue("ForumGroups", value);
        }
    }


    /// <summary>
    /// Indicates whether the web part should be hidden for result page.
    /// </summary>
    public bool HideForResult
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideForResult"), false);
        }
        set
        {
            SetValue("HideForResult", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the forums for which the user has no permissions
    /// are visible in the list of forums in forum group.
    /// </summary>
    public bool HideForumForUnauthorized
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideForumForUnauthorized"), false);
        }
        set
        {
            SetValue("HideForumForUnauthorized", value);
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!RequestHelper.IsPostBack())
        {
            // Fill the search-in options
            drpSearchIn.Items.Clear();
            drpSearchIn.Items.Add(new ListItem(GetString("ForumExtSearch.SubjAndText.Title"), "subjecttext"));
            drpSearchIn.Items.Add(new ListItem(GetString("ForumExtSearch.Subject.Title"), "subject"));
            drpSearchIn.Items.Add(new ListItem(GetString("ForumExtSearch.Text.Title"), "text"));
            drpSearchIn.SelectedIndex = 0;

            // Fill the order-by options
            drpSearchOrderBy.Items.Clear();
            drpSearchOrderBy.Items.Add(new ListItem(GetString("ForumExtSearch.PostTime.Title"), "posttime"));
            drpSearchOrderBy.Items.Add(new ListItem(GetString("general.subject"), "subject"));
            drpSearchOrderBy.Items.Add(new ListItem(GetString("ForumExtSearch.Author.Title"), "author"));
            drpSearchOrderBy.SelectedIndex = 0;

            // Initialize order buttons
            rblSearchOrder.Items.Clear();
            rblSearchOrder.Items.Add(new ListItem(GetString("ForumExtSearch.Ascending.Title"), "ascending"));
            rblSearchOrder.Items.Add(new ListItem(GetString("ForumExtSearch.Descending.Title"), "descending"));
            rblSearchOrder.SelectedIndex = 0;

            // Try to pre-select filter items
            PreSelectFilter();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (Visible && !StopProcessing)
        {
            // Register tooltips script file
            ScriptHelper.RegisterTooltip(Page);
        }
        base.OnPreRender(e);
    }


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        InitControl();
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        InitControl();
    }


    #region "Private methods"

    /// <summary>
    /// Initializes control.
    /// </summary>
    private void InitControl()
    {
        // Check if the web part should be hidden for the search result page
        if (QueryHelper.Contains("searchtext") || QueryHelper.Contains("searchusername"))
        {
            if (HideForResult)
            {
                Visible = false;
            }
        }

        if (!StopProcessing && Visible)
        {
            // Initialize control
            SetupControl();
        }
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        imgTextHint.ImageUrl = GetImageUrl("Design/Forums/hint.gif");
        imgTextHint.AlternateText = "Hint";
        ScriptHelper.AppendTooltip(imgTextHint, GetString("ForumSearch.SearchTextHint"), "help");

        btnSearch.Text = GetString("general.search");

        if (EnableForumSelection)
        {
            plcForums.Visible = true;
            string selected = ";" + QueryHelper.GetString("searchforums", "") + ";";
            bool allForums = QueryHelper.GetBoolean("allforums", false);

            ForumPostsDataSource fpd = new ForumPostsDataSource();
            fpd.CacheMinutes = 0;
            fpd.SelectedColumns = "GroupID, GroupDisplayName, ForumID, ForumDisplayName,GroupOrder, ForumOrder, ForumName ";
            fpd.Distinct = true;
            fpd.SelectOnlyApproved = false;
            fpd.SiteName = SiteContext.CurrentSiteName;

            string where = "(GroupGroupID IS NULL) AND (GroupName != 'adhocforumgroup') AND (ForumOpen=1)";

            // Get only selected forum groups
            if (!String.IsNullOrEmpty(ForumGroups))
            {
                string groups = "";
                foreach (string group in ForumGroups.Split(';'))
                {
                    groups += " '" + SqlHelper.GetSafeQueryString(group) + "',";
                }

                // Add new part to where condition
                where += " AND (GroupName IN (" + groups.TrimEnd(',') + "))";
            }

            if (HideForumForUnauthorized)
            {
                where = ForumInfoProvider.CombineSecurityWhereCondition(where, 0);
            }
            fpd.WhereCondition = where;

            fpd.OrderBy = "GroupOrder, ForumOrder ASC, ForumName ASC";

            DataSet ds = fpd.DataSource as DataSet;
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                int oldGroup = -1;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (oldGroup != ValidationHelper.GetInteger(dr["GroupID"], 0))
                    {
                        ListItem li = new ListItem(ResHelper.LocalizeString(Convert.ToString(dr["GroupDisplayName"])), "");
                        li.Attributes.Add("disabled", "disabled");
                        if (!listForums.Items.Contains(li))
                        {
                            listForums.Items.Add(li);
                        }
                        oldGroup = ValidationHelper.GetInteger(dr["GroupID"], 0);
                    }

                    string forumId = Convert.ToString(dr["ForumID"]);
                    ListItem lif = new ListItem(" \xA0\xA0\xA0\xA0 " + ResHelper.LocalizeString(Convert.ToString(dr["ForumDisplayName"])), forumId);
                    if ((selected.Contains(";" + forumId + ";")) && (!allForums))
                    {
                        lif.Selected = true;
                    }

                    // On postback on ASPX 
                    if (!listForums.Items.Contains(lif))
                    {
                        listForums.Items.Add(lif);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Validates search dialog entries and decides whether the search query should be generated.
    /// </summary>    
    private string ValidateSearchDialog()
    {
        // Check if minimum searching criteria were matched
        return string.IsNullOrEmpty(txtSearchText.Text) && string.IsNullOrEmpty(txtUserName.Text)
                   ? GetString("ForumExtSearch.Search.NothingToSearch")
                   : string.Empty;
    }


    /// <summary>
    /// Loads the settings from the querystring when search result page is the same where this control resides.
    /// </summary>
    private void PreSelectFilter()
    {
        // Get info from the query string
        string searchtext = QueryHelper.GetString("searchtext", "");
        string searchusername = QueryHelper.GetString("searchusername", "");
        string searchin = QueryHelper.GetString("searchin", "");
        string searchorderby = QueryHelper.GetString("searchorderby", "");
        string searchorder = QueryHelper.GetString("searchorder", "");

        // Load the selection
        txtSearchText.Text = (searchtext != "") ? searchtext : "";
        txtUserName.Text = (searchusername != "") ? searchusername : "";

        try
        {
            drpSearchIn.SelectedValue = searchin;
        }
        catch
        {
        }

        try
        {
            drpSearchOrderBy.SelectedValue = searchorderby;
        }
        catch
        {
        }

        try
        {
            rblSearchOrder.SelectedValue = searchorder;
        }
        catch
        {
        }
    }

    #endregion


    #region "Event handlers"

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string errMsg = ValidateSearchDialog();

        // Search dialog filled properly
        if (errMsg == string.Empty)
        {
            // Generate query string according search dialog selections
            string queryString = String.Empty;
            queryString += (!string.IsNullOrEmpty(txtSearchText.Text)) ? "searchtext=" + HttpUtility.UrlEncode(txtSearchText.Text) + "&" : "";
            queryString += (!string.IsNullOrEmpty(txtUserName.Text)) ? "searchusername=" + HttpUtility.UrlEncode(txtUserName.Text) + "&" : "";
            queryString += (!string.IsNullOrEmpty(drpSearchIn.SelectedValue)) ? "searchin=" + drpSearchIn.SelectedValue + "&" : "";
            queryString += (!string.IsNullOrEmpty(drpSearchOrderBy.SelectedValue)) ? "searchorderby=" + drpSearchOrderBy.SelectedValue + "&" : "";
            queryString += (!string.IsNullOrEmpty(rblSearchOrder.SelectedValue)) ? "searchorder=" + rblSearchOrder.SelectedValue + "&" : "";
            queryString = queryString.TrimEnd('&');

            if (EnableForumSelection)
            {
                string forQuery = "";
                bool allForums = false;

                // If no forum selected
                if (listForums.SelectedIndex < 0)
                {
                    allForums = true;
                    queryString += "&allforums=" + allForums;
                }

                foreach (ListItem li in listForums.Items)
                {
                    if ((li.Selected != allForums) && (li.Value != ""))
                    {
                        if (forQuery != "")
                        {
                            forQuery += ";";
                        }
                        forQuery += li.Value;
                    }
                }

                if (forQuery != "")
                {
                    queryString += "&searchforums=" + forQuery;
                }
            }

            // Log "internal search" activity
            mPagesActivityLogger.LogInternalSearch(txtSearchText.Text, DocumentContext.CurrentDocument);

            // Redirect to the search result page
            if (!string.IsNullOrEmpty(RedirectUrl))
            {
                if (RedirectUrl.IndexOfCSafe("?") < 0)
                {
                    queryString = "?" + queryString;
                }

                URLHelper.Redirect(ResolveUrl(RedirectUrl) + queryString);
            }
            else
            {
                string url = RequestContext.CurrentURL;

                // Get rid of previous query string parameters
                if (url.IndexOfCSafe("?") > -1)
                {
                    url = URLHelper.RemoveParameterFromUrl(url, "searchtext");
                    url = URLHelper.RemoveParameterFromUrl(url, "searchusername");
                    url = URLHelper.RemoveParameterFromUrl(url, "searchin");
                    url = URLHelper.RemoveParameterFromUrl(url, "searchorderby");
                    url = URLHelper.RemoveParameterFromUrl(url, "searchorder");
                    url = URLHelper.RemoveParameterFromUrl(url, "searchforums");
                    url = URLHelper.RemoveParameterFromUrl(url, "forumid");
                    url = URLHelper.RemoveParameterFromUrl(url, "threadid");
                    url = URLHelper.RemoveParameterFromUrl(url, "thread");
                    url = URLHelper.RemoveParameterFromUrl(url, "postid");
                    url = URLHelper.RemoveParameterFromUrl(url, "mode");
                    url = URLHelper.RemoveParameterFromUrl(url, "replyto");
                    url = URLHelper.RemoveParameterFromUrl(url, "subscribeto");
                    url = URLHelper.RemoveParameterFromUrl(url, "page");
                    url = URLHelper.RemoveParameterFromUrl(url, "allforums");
                }

                // Append query string
                url = URLHelper.AppendQuery(url, queryString);

                //Redirect back to the current page
                URLHelper.Redirect(url);
            }
        }
        else
        {
            // Display error info to the user
            lblInfo.Text = errMsg;
            lblInfo.Visible = true;
        }
    }

    #endregion


    #endregion
}
