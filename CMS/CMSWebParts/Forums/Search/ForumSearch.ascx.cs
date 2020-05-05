using System;
using System.Web;

using CMS.Core;
using CMS.DocumentEngine;
using CMS.Forums;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.WebAnalytics;

public partial class CMSWebParts_Forums_Search_ForumSearch : CMSAbstractWebPart
{
    #region Private fields

    private IPagesActivityLogger mPagesActivityLogger = Service.Resolve<IPagesActivityLogger>();

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the path to the advance search page path.
    /// </summary>
    public string AdvancedSearchPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AdvancedSearchPath"), "");
        }
        set
        {
            SetValue("AdvancedSearchPath", value);
        }
    }


    /// <summary>
    /// Gets or sets the url where is the search result web part.
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
    /// Gets or sets the value that indicates whether search should be performed only in current context.
    /// </summary>
    public bool SearchInCurrentContext
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SearchInCurrentContext"), false);
        }
        set
        {
            SetValue("SearchInCurrentContext", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        // WAI validation
        lblSearch.ResourceString = "ForumSearch.SearchWord";
        lblSearch.Attributes.Add("style", "display: none;");

        btnGo.Text = GetString("ForumSearch.Go");
        if (!String.IsNullOrEmpty(AdvancedSearchPath))
        {
            lnkAdvanceSearch.Visible = true;
            lnkAdvanceSearch.Text = GetString("ForumSearch.AdvanceSearch");
            if (!RequestHelper.IsPostBack())
            {
                txtSearch.Text = QueryHelper.GetString("searchtext", txtSearch.Text);
            }
        }
    }


    /// <summary>
    /// OnGo search click.
    /// </summary>
    protected void btnGo_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(txtSearch.Text))
        {
            string contextQuery = String.Empty;

            if (SearchInCurrentContext)
            {
                if ((ForumContext.CurrentForum != null) && (ForumContext.CurrentThread != null) && (ForumContext.CurrentThread.PostForumID == ForumContext.CurrentForum.ForumID))
                {
                    contextQuery = "&searchforums=" + ForumContext.CurrentForum.ForumID + "&searchthread=" + ForumContext.CurrentThread.PostId;
                }
                else if (ForumContext.CurrentForum != null)
                {
                    contextQuery = "&searchforums=" + ForumContext.CurrentForum.ForumID;
                }
            }

            // Log "internal search" activity
            mPagesActivityLogger.LogInternalSearch(txtSearch.Text, DocumentContext.CurrentDocument);

            if (!String.IsNullOrEmpty(RedirectUrl.Trim()))
            {
                URLHelper.Redirect(ResolveUrl(RedirectUrl) + "?searchtext=" + HttpUtility.UrlEncode(txtSearch.Text) + contextQuery);
            }
            else //Redirect back to current page
            {
                string url = URLHelper.RemoveQuery(RequestContext.CurrentURL);
                url = URLHelper.UpdateParameterInUrl(url, "searchtext", HttpUtility.UrlEncode(txtSearch.Text));
                url = URLHelper.RemoveParameterFromUrl(url, "searchforums");
                url = URLHelper.RemoveParameterFromUrl(url, "searchthread");
                url += contextQuery;

                URLHelper.Redirect(url);
            }
        }
    }
}