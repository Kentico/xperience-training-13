using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Text_statictext : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the text to be displayed.
    /// </summary>
    public string Text
    {
        get
        {
            return HTMLHelper.ResolveUrls(ValidationHelper.GetString(GetValue("Text"), ltlText.Text), null);
        }
        set
        {
            SetValue("Text", value);
            ltlText.Text = EncodeText ? HTMLHelper.HTMLEncode(value) : value;
            ltlText.EnableViewState = (ResolveDynamicControls && ControlsHelper.ResolveDynamicControls(this));
        }
    }


    /// <summary>
    /// Enables or disables resolving of inline controls.
    /// </summary>
    public bool ResolveDynamicControls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ResolveDynamicControls"), true);
        }
        set
        {
            SetValue("ResolveDynamicControls", value);
        }
    }


    /// <summary>
    /// Enables or disables HTML encoding of text.
    /// </summary>
    public bool EncodeText
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EncodeText"), false);
        }
        set
        {
            SetValue("EncodeText", value);
        }
    }


    /// <summary>
    /// Specifies as which HTML tag the content text will be rendered.
    /// </summary>
    public string Tag
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Tag"), "");
        }
        set
        {
            this.SetValue("Tag", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            string text = EncodeText ? HTMLHelper.HTMLEncode(Text) : Text;

            // Create the tag around
            string tag = this.Tag;
            if (!String.IsNullOrEmpty(tag))
            {
                text = String.Format("<{0}>{1}</{0}>", tag, text);
            }
            
            // Encode the text
            ltlText.Text = text;

            // Resolve controls
            ltlText.EnableViewState = (ResolveDynamicControls && ControlsHelper.ResolveDynamicControls(this));
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }
}