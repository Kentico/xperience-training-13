using CMS.Base.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.Helpers;

public partial class CMSWebParts_Text_Link : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Link text
    /// </summary>
    public string LinkText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("LinkText"), btnElem.LinkText);
        }
        set
        {
            this.SetValue("LinkText", value);
            btnElem.LinkText = value;
        }
    }


    /// <summary>
    /// Link CSS class
    /// </summary>
    public string LinkCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("LinkCssClass"), btnElem.CssClass);
        }
        set
        {
            this.SetValue("LinkCssClass", value);
            btnElem.CssClass = value;
        }
    }


    /// <summary>
    /// Show as button
    /// </summary>
    public bool ShowAsButton
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ShowAsButton"), btnElem.ShowAsButton);
        }
        set
        {
            this.SetValue("ShowAsButton", value);
            btnElem.ShowAsButton = value;
        }
    }


    /// <summary>
    /// Image URL
    /// </summary>
    public string ImageUrl
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ImageURL"), btnElem.ImageUrl);
        }
        set
        {
            this.SetValue("ImageURL", value);
            btnElem.ImageUrl = value;
        }
    }


    /// <summary>
    /// Image alternate text
    /// </summary>
    public string ImageAltText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ImageAltText"), btnElem.ImageAltText);
        }
        set
        {
            this.SetValue("ImageAltText", value);
            btnElem.ImageAltText = value;
        }
    }


    /// <summary>
    /// Image CSS class
    /// </summary>
    public string ImageCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ImageCssClass"), btnElem.ImageCssClass);
        }
        set
        {
            this.SetValue("ImageCssClass", value);
            btnElem.ImageCssClass = value;
        }
    }


    /// <summary>
    /// Link URL
    /// </summary>
    public string LinkUrl
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("LinkURL"), btnElem.LinkUrl);
        }
        set
        {
            this.SetValue("LinkURL", value);
            btnElem.LinkUrl = value;
        }
    }


    /// <summary>
    /// Link target
    /// </summary>
    public string LinkTarget
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("LinkTarget"), btnElem.LinkTarget);
        }
        set
        {
            this.SetValue("LinkTarget", value);
            btnElem.LinkTarget = value;
        }
    }


    /// <summary>
    /// Raise event
    /// </summary>
    public string LinkEvent
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("LinkEvent"), btnElem.LinkEvent);
        }
        set
        {
            this.SetValue("LinkEvent", value);
            btnElem.LinkEvent = value;
        }
    }


    /// <summary>
    /// Link javascript
    /// </summary>
    public string LinkJavascript
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("LinkJavascript"), btnElem.OnClientClick);
        }
        set
        {
            this.SetValue("LinkJavascript", value);
            btnElem.OnClientClick = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            btnElem.LinkText = LinkText; 
            btnElem.CssClass = LinkCssClass;
            btnElem.ShowAsButton = ShowAsButton;
            btnElem.ImageUrl = ImageUrl;
            btnElem.ImageAltText = ImageAltText;
            btnElem.ImageCssClass = ImageCssClass;
            btnElem.LinkUrl = LinkUrl;
            btnElem.LinkTarget = LinkTarget;
            btnElem.LinkEvent = LinkEvent;

            string linkJavascript = string.Empty;

            if (ShowAsButton)
            {
                // Ensure that the link will be opened in the specified target
                linkJavascript = "; window.open(" + ScriptHelper.GetString(UrlResolver.ResolveUrl(LinkUrl)) + ", " + ScriptHelper.GetString(LinkTarget) + "); return false;";
            }

            btnElem.OnClientClick = LinkJavascript + linkJavascript;
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}



