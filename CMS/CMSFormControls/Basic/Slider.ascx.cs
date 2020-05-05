using System;

using CMS.Helpers;

using System.Text;

using AjaxControlToolkit;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.MacroEngine;


public partial class CMSFormControls_Basic_Slider : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return textbox.Enabled;
        }
        set
        {
            textbox.Enabled = value;
            exSlider.EnableKeyboard = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return textbox.Text;
        }
        set
        {
            // Load default value on insert
            Double dblVal = ValidationHelper.GetDoubleSystem(value, Double.NaN);
            textbox.Text = !Double.IsNaN(dblVal) ? dblVal.ToString() : string.Empty;
        }
    }

    #endregion


    #region "Slider properties"

    /// <summary>
    /// Number of discrete values inside the slider's range.
    /// </summary>
    public int Steps
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Steps"), 0);
        }
        set
        {
            SetValue("Steps", value);
        }
    }


    /// <summary>
    /// Number of decimal digits for the value.
    /// </summary>
    public int Decimals
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Decimals"), 0);
        }
        set
        {
            SetValue("Decimals", value);
        }
    }


    /// <summary>
    /// Show/hide slider label.
    /// </summary>
    public bool ShowLabel
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowLabel"), false);
        }
        set
        {
            SetValue("ShowLabel", value);
        }
    }


    /// <summary>
    /// CSS class for the label.
    /// </summary>
    public string LabelCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LabelCssClass"), null);
        }
        set
        {
            SetValue("LabelCssClass", value);
        }
    }


    /// <summary>
    /// CSS class for the slider's rail.
    /// </summary>
    public string RailCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RailCssClass"), Orientation == SliderOrientation.Horizontal ? "slider-horizontal-rail" : "slider-vertical-rail");
        }
        set
        {
            SetValue("RailCssClass", value);
        }
    }


    /// <summary>
    /// CSS class for the slider's handle.
    /// </summary>
    public string HandleCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HandleCssClass"), Orientation == SliderOrientation.Horizontal ? "slider-horizontal-handle" : "slider-vertical-handle");
        }
        set
        {
            SetValue("HandleCssClass", value);
        }
    }


    /// <summary>
    /// URL of the image to display as the slider's handle.
    /// </summary>
    public string HandleImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HandleImageUrl"), GetImageUrl("Design/Controls/VariantSlider/slider.png"));
        }
        set
        {
            SetValue("HandleImageUrl", value);
        }
    }


    /// <summary>
    /// Width/height of a horizontal/vertical slider when the default layout is used.
    /// </summary>
    public int Length
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Length"), 0);
        }
        set
        {
            SetValue("Length", value);
        }
    }


    /// <summary>
    /// Text to display in a tooltip when the handle is hovered. The {0} placeholder in the text is replaced with the current value of the slider.
    /// </summary>
    public string TooltipText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TooltipText"), null);
        }
        set
        {
            SetValue("TooltipText", value);
        }
    }


    /// <summary>
    /// Orientation of the slider (horizontal/vertical)
    /// </summary>
    public SliderOrientation Orientation
    {
        get;
        set;
    }


    /// <summary>
    /// Minimal value of the slider.
    /// </summary>
    public double Minimum
    {
        get
        {
            return ValidationHelper.GetDouble(GetValue("Minimum"), 0);
        }
        set
        {
            SetValue("Minimum", value);
        }
    }


    /// <summary>
    /// Maximal value of the slider.
    /// </summary>
    public double Maximum
    {
        get
        {
            return ValidationHelper.GetDouble(GetValue("Maximum"), 0);
        }
        set
        {
            SetValue("Maximum", value);
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Initialize properties
        ControlsHelper.EnsureScriptManager(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the orientation
        object orientObj = GetValue("Orientation");
        if (orientObj == null)
        {
            exSlider.Orientation = Orientation;
        }
        else
        {
            exSlider.Orientation = Orientation = ValidationHelper.GetBoolean(orientObj, false) ? SliderOrientation.Vertical : SliderOrientation.Horizontal;
        }
        
        exSlider.Minimum = Minimum;
        exSlider.Maximum = Maximum;
        exSlider.Steps = Steps;
        exSlider.Decimals = Decimals;
        exSlider.Orientation = Orientation;
        exSlider.HandleCssClass = HandleCssClass;
        exSlider.HandleImageUrl = HandleImageUrl;
        exSlider.Length = Length;
        exSlider.RailCssClass = RailCssClass;
        exSlider.TooltipText = MacroResolver.Resolve(TooltipText);

        // Initialize label
        lblValue.CssClass = LabelCssClass;
        lblValue.Visible = ShowLabel;

        // Apply CSS styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            pnlContainer.CssClass = CssClass;
            CssClass = null;
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            pnlContainer.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckRegularExpression = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!Enabled)
        {
            DisableSlider();
        }
    }


    /// <summary>
    /// Makes slider extender read only by removing javascript event handlers.
    /// </summary>
    private void DisableSlider()
    {
        ScriptHelper.RegisterJQuery(Page);

        // Remove all javascript handlers from slider so it is read-only.
        StringBuilder script = new StringBuilder();

        // Bind function to AJAX life cycle event
        // Slider extender elements are not created earlier.
        script.Append(@"
Sys.Application.add_load(function (){
    var slider = $cmsj('#", exSlider.ClientID, @"_railElement')[0];
    $clearHandlers(slider);
    $clearHandlers(slider.children[0]);
});");

        ScriptHelper.RegisterStartupScript(this, typeof(string), ClientID + "_disableSlider", ScriptHelper.GetScript(script.ToString()));
    }

    #endregion
}