using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Syndication_FeedLink : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// URL of the feed.
    /// </summary>
    public string FeedURL
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("FeedURL"), null), string.Empty);
        }
        set
        {
            SetValue("FeedURL", value);
        }
    }


    /// <summary>
    /// URL title of the feed.
    /// </summary>
    public string FeedTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FeedTitle"), string.Empty);
        }
        set
        {
            SetValue("FeedTitle", value);
        }
    }


    /// <summary>
    /// Text for the feed link.
    /// </summary>
    public string LinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkText"), string.Empty);
        }
        set
        {
            SetValue("LinkText", value);
        }
    }


    /// <summary>
    /// Icon which will be displayed in the feed link.
    /// </summary>
    public string LinkIcon
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkIcon"), string.Empty);
        }
        set
        {
            SetValue("LinkIcon", value);
        }
    }


    /// <summary>
    /// Indicates if the feed is automatically discovered by the browser.
    /// </summary>
    public bool EnableAutodiscovery
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableAutodiscovery"), true);
        }
        set
        {
            SetValue("EnableAutodiscovery", value);
        }
    }


    /// <summary>
    /// Type of content for autodiscovery functionality.
    /// </summary>
    public string ContentType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ContentType"), string.Empty);
        }
        set
        {
            SetValue("ContentType", value);
        }
    }


    /// <summary>
    /// Gets or sets the path of the target document.
    /// </summary>
    public string Path
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Path"), string.Empty);
        }
        set
        {
            SetValue("Path", value);
        }
    }

    #endregion


    #region "Overidden methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            string absoluteUrl = ResolveUrl(FeedURL);

            if (EnableAutodiscovery && (ContentType.ToLowerCSafe() != "application/xml"))
            {
                // Add link to page header                
                string link = HTMLHelper.GetLink(absoluteUrl, ContentType, "alternate", null, FeedTitle);
                LiteralControl ltlMetadata = new LiteralControl(link);
                ltlMetadata.EnableViewState = false;
                Page.Header.Controls.Add(ltlMetadata);
            }

            Controls.Clear();

            if (!string.IsNullOrEmpty(LinkIcon))
            {
                HyperLink lnkFeedImg = new HyperLink();
                lnkFeedImg.ID = "lnkFeedImg";
                lnkFeedImg.NavigateUrl = absoluteUrl;
                lnkFeedImg.EnableViewState = false;
                lnkFeedImg.CssClass = "FeedLink";

                Image imgFeed = new Image();
                imgFeed.ID = "imgFeed";
                imgFeed.ImageUrl = UIHelper.GetImageUrl(this.Page, LinkIcon);
                imgFeed.AlternateText = FeedTitle;
                imgFeed.EnableViewState = false;
                imgFeed.CssClass = "FeedIcon";

                lnkFeedImg.Controls.Add(imgFeed);
                Controls.Add(lnkFeedImg);
            }

            if (!string.IsNullOrEmpty(LinkText))
            {
                HyperLink lnkFeedText = new HyperLink();
                lnkFeedText.ID = "lnkFeedText";
                lnkFeedText.NavigateUrl = absoluteUrl;
                lnkFeedText.EnableViewState = false;
                lnkFeedText.CssClass = "FeedLink";

                Label ltlFeed = new Label();
                ltlFeed.ID = "ltlFeed";
                ltlFeed.EnableViewState = false;
                ltlFeed.Text = HTMLHelper.HTMLEncode(LinkText);
                ltlFeed.CssClass = "FeedCaption";

                lnkFeedText.Controls.Add(ltlFeed);
                Controls.Add(lnkFeedText);
            }
        }
    }

    #endregion
}
