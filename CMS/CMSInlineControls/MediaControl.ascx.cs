using System;
using System.Web;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSInlineControls_MediaControl : InlineUserControl
{
    #region "Properties"

    /// <summary>
    /// Url of media file.
    /// </summary>
    public string Url
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Url"), null);
        }
        set
        {
            SetValue("Url", value);
        }
    }


    /// <summary>
    /// Type of media file.
    /// </summary>
    public string Type
    {
        get
        {
            string type = ValidationHelper.GetString(GetValue("Type"), null);
            if (type == null)
            {
                type = ValidationHelper.GetString(GetValue("Ext"), null);
            }
            if (type == null)
            {
                type = URLHelper.GetUrlParameter(Url, "ext");
            }
            return type;
        }
        set
        {
            SetValue("Type", value);
        }
    }


    /// <summary>
    /// Width of media.
    /// </summary>
    public int Width
    {
        get
        {
            int width = ValidationHelper.GetInteger(GetValue("Width"), -1);
            if (width == -1)
            {
                width = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(Url, "width"), -1);
            }
            return width;
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Height of media.
    /// </summary>
    public int Height
    {
        get
        {
            int height = ValidationHelper.GetInteger(GetValue("Height"), -1);
            if (height == -1)
            {
                height = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(Url, "height"), -1);
            }
            return height;
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Auto play media.
    /// </summary>
    public bool AutoPlay
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoPlay"), false);
        }
        set
        {
            SetValue("AutoPlay", value);
        }
    }


    /// <summary>
    /// Loop media.
    /// </summary>
    public bool Loop
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Loop"), false);
        }
        set
        {
            SetValue("Loop", value);
        }
    }


    /// <summary>
    /// Show media player controls.
    /// </summary>
    public bool AVControls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Controls"), true);
        }
        set
        {
            SetValue("Controls", value);
        }
    }


    /// <summary>
    /// Automatically active media player.
    /// </summary>
    public bool AutoActive
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoActive"), false);
        }
        set
        {
            SetValue("AutoActive", value);
        }
    }


    /// <summary>
    /// Image ID attribute value.
    /// </summary>
    public string Id
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Id"), null);
        }
        set
        {
            SetValue("Id", value);
        }
    }


    /// <summary>
    /// Title of image.
    /// </summary>
    public string Title
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Title"), null);
        }
        set
        {
            SetValue("Title", value);
        }
    }


    /// <summary>
    /// Image css style class.
    /// </summary>
    public string Class
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Class"), null);
        }
        set
        {
            SetValue("Class", value);
        }
    }


    /// <summary>
    /// Image inline style.
    /// </summary>
    public string Style
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Style"), null);
        }
        set
        {
            SetValue("Style", value);
        }
    }


    /// <summary>
    /// Control parameter.
    /// </summary>
    public override string Parameter
    {
        get
        {
            return Url;
        }
        set
        {
            Url = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (ImageHelper.IsImage(Type))
        {
            CreateImage();
        }
        else
        {
            CreateMedia();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates the media (audio / video) object
    /// </summary>
    private void CreateMedia()
    {
        AudioVideoParameters avParams = new AudioVideoParameters();
        if (Url != null)
        {
            avParams.SiteName = SiteContext.CurrentSiteName;
            avParams.Url = URLHelper.GetAbsoluteUrl(Url);
            avParams.Extension = Type;
            avParams.Width = Width;
            avParams.Height = Height;
            avParams.AutoPlay = AutoPlay;
            avParams.Loop = Loop;
            avParams.Controls = AVControls;
        }

        ltlMedia.Text = MediaHelper.GetAudioVideo(avParams);
    }


    /// <summary>
    /// Creates the image object
    /// </summary>
    private void CreateImage()
    {
        ImageParameters imgParams = new ImageParameters();
        if (Url != null)
        {
            imgParams.Url = URLHelper.GetAbsoluteUrl(Url);
            imgParams.Extension = Type;
            imgParams.Width = Width;
            imgParams.Height = Height;
            imgParams.Id = HttpUtility.UrlDecode(Id);
            imgParams.Tooltip = HttpUtility.UrlDecode(Title);
            imgParams.Class = HttpUtility.UrlDecode(Class);
            imgParams.Style = HttpUtility.UrlDecode(Style);
        }
        ltlMedia.Text = MediaHelper.GetImage(imgParams);
    }

    #endregion
}