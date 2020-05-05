using System;
using System.Collections.Generic;

using CMS.Base;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;

public partial class CMSModules_MediaLibrary_Controls_Filters_MediaLibrarySort : CMSAbstractDataFilterControl
{
    #region "Variables"

    private readonly List<string> columns = new List<string> { "filename", "filecreatedwhen", "filesize" };
    private int mFilterMethod;

    #endregion


    #region "Properties"

    /// <summary>
    /// Sort by value.
    /// </summary>
    public string SortBy
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SortBy"], String.Empty);
        }
        set
        {
            ViewState["SortBy"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the file id querystring parameter.
    /// </summary>
    public string FileIDQueryStringKey
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the sort querystring parameter.
    /// </summary>
    public string SortQueryStringKey
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the filter method.
    /// </summary>
    public int FilterMethod
    {
        get
        {
            return mFilterMethod;
        }
        set
        {
            mFilterMethod = value;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        SetupControls();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
            return;
        }

        if (QueryHelper.GetInteger(FileIDQueryStringKey, 0) > 0)
        {
            StopProcessing = true;
            Visible = false;
        }
        else
        {
            if (FilterMethod != 0)
            {
                OrderBy = SortBy;
            }
            else
            {
                string[] orderBy = QueryHelper.GetString(SortQueryStringKey, "").Split(';');
                if ((orderBy.Length == 2) && columns.Contains(orderBy[0].ToLowerCSafe()) && ((orderBy[1].ToLowerCSafe() == "asc") || (orderBy[1].ToLowerCSafe() == "desc")))
                {
                    OrderBy = String.Format("{0} {1}", orderBy[0], orderBy[1]);
                }
            }
            RaiseOnFilterChanged();
        }
    }


    #region "Links handlers"

    protected void lnkName_Click(object sender, EventArgs e)
    {
        if (FilterMethod == 1)
        {
            if ((SortBy != null) && (SortBy.EndsWithCSafe("ASC")))
            {
                SortBy = "FileName DESC";
            }
            else
            {
                SortBy = "FileName ASC";
            }
            OrderBy = SortBy;
            RaiseOnFilterChanged();
        }
        else
        {
            if (!String.IsNullOrEmpty(SortQueryStringKey))
            {
                string sort = QueryHelper.GetString(SortQueryStringKey, String.Empty);
                if (sort.StartsWithCSafe("FileName"))
                {
                    if (sort.EndsWithCSafe("ASC"))
                    {
                        RedirectToUpdatedUrl("FileName;DESC");
                    }
                    else
                    {
                        RedirectToUpdatedUrl("FileName;ASC");
                    }
                }
                else
                {
                    RedirectToUpdatedUrl("FileName;ASC");
                }
            }
        }
    }


    protected void lnkDate_Click(object sender, EventArgs e)
    {
        if (FilterMethod == 1)
        {
            if ((SortBy != null) && (SortBy.EndsWithCSafe("ASC")))
            {
                SortBy = "FileCreatedWhen DESC";
            }
            else
            {
                SortBy = "FileCreatedWhen ASC";
            }
            OrderBy = SortBy;
            RaiseOnFilterChanged();
        }
        else
        {
            if (!String.IsNullOrEmpty(SortQueryStringKey))
            {
                string sort = QueryHelper.GetString(SortQueryStringKey, String.Empty);
                if (sort.StartsWithCSafe("FileCreatedWhen"))
                {
                    if (sort.EndsWithCSafe("ASC"))
                    {
                        RedirectToUpdatedUrl("FileCreatedWhen;DESC");
                    }
                    else
                    {
                        RedirectToUpdatedUrl("FileCreatedWhen;ASC");
                    }
                }
                else
                {
                    RedirectToUpdatedUrl("FileCreatedWhen;ASC");
                }
            }
        }
    }


    protected void lnkSize_Click(object sender, EventArgs e)
    {
        if (FilterMethod == 1)
        {
            if ((SortBy != null) && (SortBy.EndsWithCSafe("ASC")))
            {
                SortBy = "FileSize DESC";
            }
            else
            {
                SortBy = "FileSize ASC";
            }
            OrderBy = SortBy;
            RaiseOnFilterChanged();
        }
        else
        {
            if (!String.IsNullOrEmpty(SortQueryStringKey))
            {
                string sort = QueryHelper.GetString(SortQueryStringKey, String.Empty);
                if (sort.StartsWithCSafe("FileSize"))
                {
                    if (sort.EndsWithCSafe("ASC"))
                    {
                        RedirectToUpdatedUrl("FileSize;DESC");
                    }
                    else
                    {
                        RedirectToUpdatedUrl("FileSize;ASC");
                    }
                }
                else
                {
                    RedirectToUpdatedUrl("FileSize;ASC");
                }
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Setup controls.
    /// </summary>
    private void SetupControls()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            lnkDate.Text = ResHelper.GetString("media.library.sort.date");
            lnkName.Text = ResHelper.GetString("media.library.sort.name");
            lnkSize.Text = ResHelper.GetString("media.library.sort.size");
        }
    }


    /// <summary>
    /// Redirect to updated url.
    /// </summary>
    /// <param name="value">Value</param>
    private void RedirectToUpdatedUrl(string value)
    {
        URLHelper.Redirect(URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, SortQueryStringKey, value));
    }

    #endregion
}