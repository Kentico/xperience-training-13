using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSFormControls_Basic_NumericUpDown : FormEngineUserControl
{
    #region "Variables"

    private Dictionary<string, string> mValues;
    private string mInnerValue;

    #endregion


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
            btnDown.Enabled = value;
            btnUp.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            if ((mValues != null) && (mValues.Count > 0))
            {
                string key = textbox.Text;
                if (mValues.ContainsKey(key))
                {
                    return mValues[key];
                }
            }

            return textbox.Text;
        }
        set
        {
            if ((value != null) || ((FieldInfo != null) && FieldInfo.AllowEmpty))
            {                
                // Convert the value to a proper type
                value = ConvertInputValue(value);

                mInnerValue = ValidationHelper.GetString(value, String.Empty);

                LoadValues();

                if ((mValues != null) && (mValues.Count > 0))
                {
                    foreach (string key in mValues.Keys)
                    {
                        if (mValues[key] == mInnerValue)
                        {
                            textbox.Text = key;
                            break;
                        }
                    }
                }
                else
                {
                    textbox.Text = mInnerValue;
                }
            }
        }
    }

    #endregion


    #region "Custom properties"

    /// <summary>
    /// Step used for simple numeric incrementing and decrementing. The default value is 1.
    /// </summary>
    public double Step
    {
        get
        {
            return ValidationHelper.GetDouble(GetValue("Step"), 1);
        }
        set
        {
            SetValue("Step", value);
        }
    }


    /// <summary>
    /// Minimal value of the selector.
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
    /// Maximal value of the selector.
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


    /// <summary>
    /// Combined size of the TextBox and Up/Down buttons (min value 25). This property is not used if you provide custom buttons.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 112);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// URL of the image to display as the Up button.
    /// </summary>
    public string UpButtonImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UpButtonImageUrl"), null);
        }
        set
        {
            SetValue("UpButtonImageUrl", value);
        }
    }


    /// <summary>
    /// URL of the image to display as the Down button.
    /// </summary>
    public string DownButtonImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DownButtonImageUrl"), null);
        }
        set
        {
            SetValue("DownButtonImageUrl", value);
        }
    }


    /// <summary>
    /// The alt text to show when the mouse is over the  Up button.
    /// </summary>
    public string UpButtonImageAlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UpButtonImageAlternateText"), ResHelper.GetString("general.up"));
        }
        set
        {
            SetValue("UpButtonImageAlternateText", value);
        }
    }


    /// <summary>
    /// The alt text to show when the mouse is over the  Down button.
    /// </summary>
    public string DownButtonImageAlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DownButtonImageAlternateText"), ResHelper.GetString("general.down"));
        }
        set
        {
            SetValue("DownButtonImageAlternateText", value);
        }
    }

    #endregion


    #region "Methods"

    private void LoadValues()
    {
        if (mValues == null)
        {
            string options = GetResolvedValue<string>("options", null);
            string query = ValidationHelper.GetString(GetValue("query"), null);
            ListItemCollection items = new ListItemCollection();
            mValues = new Dictionary<string, string>();

            try
            {
                new SpecialFieldsDefinition(resolver: ContextResolver)
                {
                    FieldInfo = FieldInfo,
                    AllowDuplicates = true
                }
                .LoadFromText(options)
                .LoadFromQuery(query)
                .FillItems(items);

                foreach (ListItem item in items)
                {
                    mValues.Add(item.Text, item.Value);
                }
            }
            catch (Exception ex)
            {
                FormControlError ctrlError = new FormControlError();
                ctrlError.FormControlName = "NumericUpDown";
                ctrlError.InnerException = ex;
                Controls.Add(ctrlError);
                pnlContainer.Visible = false;
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQueryUI(Page);

        btnDown.ScreenReaderDescription = GetString("spinner.decrement");
        btnUp.ScreenReaderDescription = GetString("spinner.increment");

        textbox.Width = Width;

        LoadValues();

        // Initialize up button
        if (!string.IsNullOrEmpty(UpButtonImageUrl))
        {
            btnImgUp.Visible = true;
            btnUp.Visible = false;
            btnImgUp.ImageUrl = UpButtonImageUrl;
            btnImgUp.AlternateText = ContextResolver.ResolveMacros(UpButtonImageAlternateText);
            btnImgUp.ImageAlign = ImageAlign.Middle;
        }

        // Initialize down button
        if (!string.IsNullOrEmpty(DownButtonImageUrl))
        {
            btnImgDown.Visible = true;
            btnDown.Visible = false;
            btnImgDown.ImageUrl = DownButtonImageUrl;
            btnImgDown.AlternateText = ContextResolver.ResolveMacros(DownButtonImageAlternateText);
            btnImgDown.ImageAlign = ImageAlign.Middle;
        }

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
        CheckFieldEmptiness = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Enabled && Visible)
        {
            string upElementId = btnImgUp.Visible ? btnImgUp.ClientID : btnUp.ClientID;
            string downElementId = btnImgDown.Visible ? btnImgDown.ClientID : btnDown.ClientID;

            ScriptHelper.RegisterModule(this, "CMS/NumericUpDown", new
            {
                controlId = pnlContainer.ClientID,
                textBoxId = textbox.ClientID,
                textBoxUniqueId = textbox.UniqueID,
                upElementId = upElementId,
                downElementId = downElementId,
                minimum = Minimum,
                maximum = Maximum,
                step = Step,
                raisePostBackOnChange = HasDependingFields,
            });
        }
    }

    #endregion
}