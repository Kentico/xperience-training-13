using System;
using System.Text;

using CMS.Base.Web.UI;
using CMS.Helpers;


public partial class CMSInlineControls_ImageControl : InlineUserControl
{
    #region "Properties"

    /// <summary>
    /// URL of the image media.
    /// </summary>
    public string URL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("URL"), null);
        }
        set
        {
            SetValue("URL", value);
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to append size parameters to URL ot not.
    /// </summary>
    public bool SizeToURL
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SizeToURL"), true);
        }
        set
        {
            SetValue("SizeToURL", value);
        }
    }


    /// <summary>
    /// Image extension.
    /// </summary>
    public string Extension
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Extension"), null);
        }
        set
        {
            SetValue("Extension", value);
        }
    }


    /// <summary>
    /// Image alternative text.
    /// </summary>
    public string Alt
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Alt"), null);
        }
        set
        {
            SetValue("Alt", value);
        }
    }


    /// <summary>
    /// Image width.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), -1);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Image height.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), -1);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Image border width.
    /// </summary>
    public int BorderWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("BorderWidth"), -1);
        }
        set
        {
            SetValue("BorderWidth", value);
        }
    }


    /// <summary>
    /// Image border color.
    /// </summary>
    public string BorderColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BorderColor"), null);
        }
        set
        {
            SetValue("BorderColor", value);
        }
    }

    /// <summary>
    /// Image horizontal space.
    /// </summary>
    public int HSpace
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("HSpace"), -1);
        }
        set
        {
            SetValue("HSpace", value);
        }
    }


    /// <summary>
    /// Image vertical space.
    /// </summary>
    public int VSpace
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("VSpace"), -1);
        }
        set
        {
            SetValue("VSpace", value);
        }
    }


    /// <summary>
    /// Image align.
    /// </summary>
    public string Align
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Align"), null);
        }
        set
        {
            SetValue("Align", value);
        }
    }


    /// <summary>
    /// Image ID.
    /// </summary>
    public string ImageID
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
    /// Image tooltip text.
    /// </summary>
    public string Tooltip
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Tooltip"), null);
        }
        set
        {
            SetValue("Tooltip", value);
        }
    }


    /// <summary>
    /// Image css class.
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
    /// Image link destination.
    /// </summary>
    public string Link
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Link"), null);
        }
        set
        {
            SetValue("Link", value);
        }
    }


    /// <summary>
    /// Image link target (_blank/_self/_parent/_top)
    /// </summary>
    public string Target
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Target"), null);
        }
        set
        {
            SetValue("Target", value);
        }
    }


    /// <summary>
    /// Image behavior.
    /// </summary>
    public string Behavior
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Behavior"), null);
        }
        set
        {
            SetValue("Behavior", value);
        }
    }


    /// <summary>
    /// Control parameter.
    /// </summary>
    public override string Parameter
    {
        get
        {
            return URL;
        }
        set
        {
            URL = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_PreRender(object sender, EventArgs e)
    {
        ImageParameters imgParams = new ImageParameters();
        if (!String.IsNullOrEmpty(URL))
        {
            imgParams.Url = ResolveUrl(URL);
        }
        imgParams.Align = Align;
        imgParams.Alt = Alt;
        imgParams.Behavior = Behavior;
        imgParams.BorderColor = BorderColor;
        imgParams.BorderWidth = BorderWidth;
        imgParams.Class = Class;
        imgParams.Extension = Extension;
        imgParams.Height = Height;
        imgParams.HSpace = HSpace;
        imgParams.Id = (String.IsNullOrEmpty(ImageID) ? Guid.NewGuid().ToString() : ImageID);
        imgParams.Link = Link;
        imgParams.SizeToURL = SizeToURL;
        imgParams.Style = Style;
        imgParams.Target = Target;
        imgParams.Tooltip = Tooltip;
        imgParams.VSpace = VSpace;
        imgParams.Width = Width;

        ltlImage.Text = MediaHelper.GetImage(imgParams);
    }

    #endregion
}