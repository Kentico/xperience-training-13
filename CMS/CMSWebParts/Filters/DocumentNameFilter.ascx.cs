using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Filters_DocumentNameFilter : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the filter button text.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ButtonText"), string.Empty);
        }
        set
        {
            SetValue("ButtonText", value);
            filterDocuments.ButtonText = value;
        }
    }


    /// <summary>
    /// Gets or sets the button css class.
    /// </summary>
    public string ButtonCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ButtonCssClass"), string.Empty);
        }
        set
        {
            SetValue("ButtonCssClass", value);
            filterDocuments.ButtonCssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string LabelText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LabelText"), string.Empty);
        }
        set
        {
            SetValue("LabelText", value);
            filterDocuments.LabelText = value;
        }
    }


    /// <summary>
    /// Gets or sets the label css class.
    /// </summary>
    public string LabelCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LabelCssClass"), string.Empty);
        }
        set
        {
            SetValue("LabelCssClass", value);
            filterDocuments.LabelCssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the textbox css class.
    /// </summary>
    public string TextBoxCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TextBoxCssClass"), string.Empty);
        }
        set
        {
            SetValue("TextBoxCssClass", value);
            filterDocuments.TextBoxCssClass = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register control ID in filter collection
        filterDocuments.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
    }


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
    public void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            filterDocuments.ButtonText = ButtonText;
            filterDocuments.ButtonCssClass = ButtonCssClass;
            filterDocuments.LabelText = LabelText;
            filterDocuments.LabelCssClass = LabelCssClass;
            filterDocuments.TextBoxCssClass = TextBoxCssClass;
        }
    }

    #endregion
}