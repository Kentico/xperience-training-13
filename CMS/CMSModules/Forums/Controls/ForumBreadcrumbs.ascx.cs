using System;
using System.Text;

using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;

public partial class CMSModules_Forums_Controls_ForumBreadcrumbs : ForumViewer
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether forum group should be displayed in breadcrumbs.
    /// </summary>
    public bool DisplayGroup
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DisplayGroup"], true);
        }
        set
        {
            ViewState["DisplayGroup"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether threads names should be displayed in breadcrumbs.
    /// </summary>
    public bool DisplayThreads
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DisplayThreads"], true);
        }
        set
        {
            ViewState["DisplayThreads"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether thread name should be displayed in breadcrumbs.
    /// </summary>
    public bool DisplayThread
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DisplayThread"], true);
        }
        set
        {
            ViewState["DisplayThread"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the breadcrumbs separator.
    /// </summary>
    public string BreadcrumbSeparator
    {
        get
        {
            return ValidationHelper.GetString(ViewState["BreadcrumbsSeparator"], "&nbsp;&gt;&nbsp;");
        }
        set
        {
            ViewState["BreadcrumbsSeparator"] = HTMLHelper.HTMLEncode(value);
        }
    }


    /// <summary>
    /// Gets or sets the breadcrumbs prefix which should be displayed before breadcrumbs items.
    /// </summary>
    public string BreadcrumbPrefix
    {
        get
        {
            return ValidationHelper.GetString(ViewState["BreadcrumbPrefix"], "");
        }
        set
        {
            ViewState["BreadcrumbPrefix"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether current item should be rendered as link.
    /// </summary>
    public bool UseLinkForCurrentItem
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["UseLinkForCurrentItem"], false);
        }
        set
        {
            ViewState["UseLinkForCurrentItem"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether breadcrumbs should be hidden on forum group page
    /// This option hides only forum breadcrumbs, breadcrumbs prefix is allways visible if is defined
    /// </summary>
    public bool HideBreadcrumbsOnForumGroupPage
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["HideBreadcrumbsOnForumGroupPage"], true);
        }
        set
        {
            ViewState["HideBreadcrumbsOnForumGroupPage"] = value;
        }
    }

    #endregion


    /// <summary>
    /// Generates breadcrumbs.
    /// </summary>
    public virtual string GenerateBreadcrumbs()
    {
        // Create string builder object
        StringBuilder result = new StringBuilder();

        bool separator = false;

        // Check whether breadcrumbs prefix is defined, if so generate it
        if (!String.IsNullOrEmpty(BreadcrumbPrefix))
        {
            result.Append(BreadcrumbPrefix);
            separator = true;
        }

        // Check hide breadcrumbs on forum page 
        if ((ForumContext.CurrentForum == null) && (HideBreadcrumbsOnForumGroupPage))
        {
            return result.ToString();
        }

        // Check whether group exists and if should be displayed
        if ((DisplayGroup) && (ForumContext.CurrentGroup != null))
        {
            if (separator)
            {
                result.Append(BreadcrumbSeparator);
            }
            string grUrl = URLCreator("", "", false, null, ForumActionType.ForumGroup);
            grUrl = URLHelper.RemoveParameterFromUrl(grUrl, "forumId");
            grUrl = URLHelper.RemoveParameterFromUrl(grUrl, "threadId");
            grUrl = URLHelper.RemoveParameterFromUrl(grUrl, "mode");
            grUrl = URLHelper.RemoveParameterFromUrl(grUrl, "postid");
            grUrl = URLHelper.RemoveParameterFromUrl(grUrl, "replyto");
            grUrl = URLHelper.RemoveParameterFromUrl(grUrl, "subscribeto");
            grUrl = URLHelper.RemoveParameterFromUrl(grUrl, "moveto");
            grUrl = URLHelper.RemoveParameterFromUrl(grUrl, "fpage");
            result.Append(CreateItem(grUrl, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ForumContext.CurrentGroup.GroupDisplayName)), (ForumContext.CurrentForum != null) ? true : UseLinkForCurrentItem));
            separator = true;
        }
        else
        {
            //separator = false;
        }

        // Check whether forum exists and if should be displayed
        if ((DisplayThreads) && (ForumContext.CurrentGroup != null || ForumContext.CurrentThread != null) && (ForumContext.CurrentForum != null) && (ForumContext.CurrentForum.ForumID > 0))
        {
            if (separator)
            {
                result.Append(BreadcrumbSeparator);
            }

            string frUrl = URLCreator("forumid", ForumContext.CurrentForum.ForumID.ToString(), false, ForumContext.CurrentForum, ForumActionType.Forum);

            frUrl = URLHelper.RemoveParameterFromUrl(frUrl, "threadId");
            frUrl = URLHelper.RemoveParameterFromUrl(frUrl, "mode");
            frUrl = URLHelper.RemoveParameterFromUrl(frUrl, "postid");
            frUrl = URLHelper.RemoveParameterFromUrl(frUrl, "replyto");
            frUrl = URLHelper.RemoveParameterFromUrl(frUrl, "subscribeto");
            frUrl = URLHelper.RemoveParameterFromUrl(frUrl, "moveto");
            frUrl = URLHelper.RemoveParameterFromUrl(frUrl, "tpage");

            result.Append(CreateItem(frUrl, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ForumContext.CurrentForum.ForumDisplayName)), (ForumContext.CurrentThread != null) ? true : UseLinkForCurrentItem));
            separator = true;
        }
        else
        {
            //separator = false;
        }

        // Check whether thread exists and if should be displayed
        if ((DisplayThread) && (ForumContext.CurrentThread != null))
        {
            result.Append(BreadcrumbSeparator);
            result.Append(CreateItem(URLCreator("threadid", ForumContext.CurrentThread.PostId.ToString(), false, ForumContext.CurrentThread), HTMLHelper.HTMLEncode(ForumContext.CurrentThread.PostSubject), UseLinkForCurrentItem));
        }

        return result.ToString();
    }


    /// <summary>
    /// Creates link or clear text.
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="text">Link text</param>
    /// <param name="useLink">Set if current item is left</param>
    protected string CreateItem(string url, string text, bool useLink)
    {
        StringBuilder sb = new StringBuilder();

        if (useLink)
        {
            sb.Append("<a href=\"");
            sb.Append(HTMLHelper.EncodeForHtmlAttribute(url));
            sb.Append("\">");
        }
        else
        {
            sb.Append("<span>");
        }

        sb.Append(HTMLHelper.HTMLEncode(text));

        if (useLink)
        {
            sb.Append("</a>");
        }
        else
        {
            sb.Append("</span>");
        }

        return sb.ToString();
    }


    /// <summary>
    /// OnPreRender, call Generate breadcrumbs.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        CopyValuesFromParent(this);

        litBreadcrumbs.Text = GenerateBreadcrumbs();
        base.OnPreRender(e);
    }
}