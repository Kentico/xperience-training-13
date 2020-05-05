using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;


public partial class CMSFormControls_MediaSelector : FormEngineUserControl, IDialogControl
{
    #region "Properties"

    /// <summary>
    /// Indicates if the Clear button should be displayed.
    /// </summary>
    public bool ShowClearButton
    {
        get
        {
            return selectMediaElement.ShowClearButton;
        }
        set
        {
            selectMediaElement.ShowClearButton = value;
        }
    }


    /// <summary>
    /// Indicates if the image preview be displayed.
    /// </summary>
    public bool ShowPreview
    {
        get
        {
            return selectMediaElement.ShowPreview;
        }
        set
        {
            selectMediaElement.ShowPreview = value;
        }
    }


    /// <summary>
    /// Indicates if the path textbox should be displayed.
    /// </summary>
    public bool ShowTextBox
    {
        get
        {
            return selectMediaElement.ShowTextBox;
        }
        set
        {
            selectMediaElement.ShowTextBox = value;
        }
    }


    /// <summary>
    /// Selector value: URL of the Media.
    /// </summary>
    public override object Value
    {
        get
        {
            return selectMediaElement.Value;
        }
        set
        {
            if (value != null)
            {
                selectMediaElement.Value = value.ToString();
            }
            else
            {
                selectMediaElement.Value = String.Empty;
            }
        }
    }


    ///<summary>
    /// Width of the image preview.
    ///</summary>
    public int ImageWidth
    {
        get
        {
            return selectMediaElement.ImageWidth;
        }
        set
        {
            selectMediaElement.ImageWidth = value;
        }
    }


    /// <summary>
    /// Height of the image preview.
    /// </summary>
    public int ImageHeight
    {
        get
        {
            return selectMediaElement.ImageHeight;
        }
        set
        {
            selectMediaElement.ImageHeight = value;
        }
    }


    /// <summary>
    /// Image max side size.
    /// </summary>
    public int ImageMaxSideSize
    {
        get
        {
            return selectMediaElement.ImageMaxSideSize;
        }
        set
        {
            selectMediaElement.ImageMaxSideSize = value;
        }
    }


    /// <summary>
    /// CSS class of the image preview.
    /// </summary>
    public string ImageCssClass
    {
        get
        {
            return selectMediaElement.ImageCssClass;
        }
        set
        {
            selectMediaElement.ImageCssClass = value;
        }
    }


    /// <summary>
    /// CSS style of the image preview.
    /// </summary>
    public string ImageStyle
    {
        get
        {
            return selectMediaElement.ImageStyle;
        }
        set
        {
            selectMediaElement.ImageStyle = value;
        }
    }


    /// <summary>
    /// Enable open in full size behavior.
    /// </summary>
    public bool EnableOpenInFull
    {
        get
        {
            return selectMediaElement.EnableOpenInFull;
        }
        set
        {
            selectMediaElement.EnableOpenInFull = value;
        }
    }


    /// <summary>
    /// Interface culture of the control.
    /// </summary>
    public string Culture
    {
        get
        {
            return selectMediaElement.Culture;
        }
        set
        {
            selectMediaElement.Culture = value;
        }
    }


    /// <summary>
    /// Enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return selectMediaElement.Enabled;
        }
        set
        {
            selectMediaElement.Enabled = value;
        }
    }


    /// <summary>
    /// Indicates if control is used in live site mode.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return selectMediaElement.IsLiveSite;
        }
        set
        {
            selectMediaElement.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Configuration of the dialog for inserting Images.
    /// </summary>
    public DialogConfiguration DialogConfig
    {
        get
        {
            return selectMediaElement.DialogConfig;
        }
        set
        {
            selectMediaElement.DialogConfig = value;
        }
    }

    #endregion
}