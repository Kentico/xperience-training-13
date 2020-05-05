using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Media_YouTubeVideo : CMSAbstractWebPart
{
    #region "Private fiels"

    private bool mHide;
    private static int? mSocialMediaCookieLevel;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether to hide content of the WebPart
    /// </summary>
    public bool HideContent
    {
        get
        {
            return mHide;
        }
        set
        {
            mHide = value;
            ltlPlaceholder.Visible = !value;
            ltlScript.Visible = !value;
        }
    }



    /// <summary>
    ///  Gets or sets the URL of YouTube video to be displayed.
    /// </summary>
    public string VideoURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("VideoURL"), "");
        }
        set
        {
            SetValue("VideoURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the video width.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 425);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Gets or sets the video height.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), 355);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether video start immediately after webpart load.
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
    /// Gets or sets the value that indicates whether player should display related videos after playback stops.
    /// </summary>
    public bool Rel
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Rel"), false);
        }
        set
        {
            SetValue("Rel", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether video should support full screen playback.
    /// </summary>
    public bool FullScreen
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FullScreen"), false);
        }
        set
        {
            SetValue("FullScreen", value);
        }
    }


    /// <summary>
    /// Gets Social media cookie level.
    /// </summary>
    private static int SocialMediaCookieLevel
    {
        get
        {
            if (!mSocialMediaCookieLevel.HasValue)
            {
                mSocialMediaCookieLevel = ValidationHelper.GetInteger(Service.Resolve<IAppSettingsService>()["CMSSocialMediaCookieLevel"], CookieLevel.All);
            }
            return mSocialMediaCookieLevel.Value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Pre-render
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        EnsureCookieLaw();
    }


    /// <summary>
    /// Reload Data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        EnsureCookieLaw();
    }


    private void EnsureCookieLaw()
    {
        var cookieLevelProvider = Service.Resolve<ICurrentCookieLevelProvider>();

        if (cookieLevelProvider.GetCurrentCookieLevel() < SocialMediaCookieLevel)
        {
            HideContent = true;
            StopProcessing = true;
        }
        else
        {
            HideContent = false;
            StopProcessing = false;
            SetupControl();
        }
    }


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
            string videoUrl = VideoURL.Trim();

            if (!ValidationHelper.IsURL(videoUrl))
            {
                Service.Resolve<IEventLogService>().LogError("YouTubeVideo", "INVALIDVIDEOURL", String.Format("YouTube video web part couldn't load the video because the given URL is not valid: {0}", HTMLHelper.HTMLEncode(videoUrl)));
                return;
            }

            // If no wmode is set, append 'transparent' wmode. Fix IE issue with widget (widgets buttons are beyond YouTube video)
            if (String.IsNullOrEmpty(URLHelper.GetQueryValue(videoUrl, "wmode")))
            {
                videoUrl = URLHelper.UpdateParameterInUrl(videoUrl, "wmode", "transparent");
            }

            YouTubeVideoParameters ytParams = new YouTubeVideoParameters
            {
                Url = videoUrl,
                FullScreen = FullScreen,
                AutoPlay = AutoPlay,
                RelatedVideos = Rel,
                Width = Width,
                Height = Height,
            };

            ltlPlaceholder.Text = MediaHelper.GetYouTubeVideo(ytParams);
        }
    }

    #endregion
}