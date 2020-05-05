using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Media_MediaSelectionControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mediaSelector.Enabled;
        }
        set
        {
            mediaSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return mediaSelector.Value;
        }
        set
        {
            mediaSelector.Value = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Indicates if control is placed on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return mediaSelector.IsLiveSite;
        }
        set
        {
            mediaSelector.IsLiveSite = value;
        }
    }

    /// <summary>
    /// Indicates if the image preview should be displayed.
    /// </summary>
    public bool ShowPreview
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPreview"), true);
        }
        set
        {
            SetValue("ShowPreview", value);
            mediaSelector.ShowPreview = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
    }


    /// <summary>
    /// Setups selection control.
    /// </summary>
    private void SetupControl()
    {
        // Setup control
        mediaSelector.ImagePathTextBox.AddCssClass("EditingFormMediaPathTextBox");
        mediaSelector.PreviewControl.AddCssClass("EditingFormMediaPathPreview");
        mediaSelector.SelectImageButton.AddCssClass("EditingFormMediaPathButton");
        mediaSelector.ClearPathButton.AddCssClass("EditingFormMediaPathClearButton");
        mediaSelector.AutoPostback = HasDependingFields;

        if (!String.IsNullOrEmpty(ControlStyle))
        {
            mediaSelector.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!String.IsNullOrEmpty(CssClass))
        {
            mediaSelector.CssClass = CssClass;
            CssClass = null;
        }

        // Get dialog configuration
        DialogConfiguration mediaConfig = GetDialogConfiguration();
        if (mediaConfig != null)
        {
            mediaConfig.SelectableContent = SelectableContentEnum.AllFiles;
            mediaConfig.OutputFormat = OutputFormatEnum.URL;
            mediaConfig.HideWeb = true;
            mediaSelector.DialogConfig = mediaConfig;
            mediaSelector.UseCustomDialogConfig = true;
            mediaSelector.ShowPreview = ShowPreview;
        }
    }

    #endregion
}