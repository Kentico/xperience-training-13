using System;
using System.Collections.Generic;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Basic_BBEditorControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return bbEditor.Enabled;
        }
        set
        {
            bbEditor.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return bbEditor.TextArea.Text;
        }
        set
        {
            bbEditor.TextArea.Text = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Indicates if control is placed on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return bbEditor.IsLiveSite;
        }
        set
        {
            bbEditor.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize editor
        bbEditor.TextArea.Columns = ValidationHelper.GetInteger(GetValue("cols"), 40);
        bbEditor.TextArea.Rows = ValidationHelper.GetInteger(GetValue("rows"), 5);
        bbEditor.ShowURL = ValidationHelper.GetBoolean(GetValue("showurl"), true);
        bbEditor.ShowQuote = ValidationHelper.GetBoolean(GetValue("showquote"), true);
        bbEditor.ShowImage = ValidationHelper.GetBoolean(GetValue("showimage"), true);
        bbEditor.ShowBold = ValidationHelper.GetBoolean(GetValue("showbold"), true);
        bbEditor.ShowItalic = ValidationHelper.GetBoolean(GetValue("showitalic"), true);
        bbEditor.ShowUnderline = ValidationHelper.GetBoolean(GetValue("showunderline"), true);
        bbEditor.ShowStrike = ValidationHelper.GetBoolean(GetValue("showstrike"), true);
        bbEditor.ShowColor = ValidationHelper.GetBoolean(GetValue("showcolor"), true);
        bbEditor.ShowCode = ValidationHelper.GetBoolean(GetValue("showcode"), true);
        bbEditor.UsePromptDialog = ValidationHelper.GetBoolean(GetValue("usepromptdialog"), true);
        bbEditor.ShowAdvancedImage = ValidationHelper.GetBoolean(GetValue("showadvancedimage"), false);
        bbEditor.ShowAdvancedURL = ValidationHelper.GetBoolean(GetValue("showadvancedurl"), false);
        int size = ValidationHelper.GetInteger(GetValue("size"), 0);
        if (size > 0)
        {
            bbEditor.TextArea.MaxLength = size;
        }

        bbEditor.IsLiveSite = IsLiveSite;

        if (!String.IsNullOrEmpty(CssClass))
        {
            bbEditor.AddCssClass(CssClass);
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(bbEditor.CssClass))
        {
            bbEditor.AddCssClass("BBEditorField");
        }
        if (!string.IsNullOrEmpty(ControlStyle))
        {
            bbEditor.TextArea.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        DialogConfiguration config = GetDialogConfiguration();
        bbEditor.ImageDialogConfig = config;
        bbEditor.URLDialogConfig = config.Clone();

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        int maxControlSize = 0;

        // Check min/max text length
        if (GetValue("size") != null)
        {
            maxControlSize = ValidationHelper.GetInteger(GetValue("size"), 0);
        }

        // Validate control
        string error = null;
        bool valid = CheckLength(0, maxControlSize, bbEditor.TextArea.Text.Length, ref error);
        ValidationError = error;
        return valid;
    }


    /// <summary>
    /// Returns the arraylist of the field IDs (Client IDs of the inner controls) that should be spell checked.
    /// </summary>
    public override List<string> GetSpellCheckFields()
    {
        List<string> result = new List<string>();
        result.Add(bbEditor.TextArea.ClientID);
        return result;
    }

    #endregion
}