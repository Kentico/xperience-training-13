using System;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;


public partial class CMSFormControls_Macros_MacroSyntaxHighlighter : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return txtMacro.Editor.Enabled;
        }
        set
        {
            txtMacro.Editor.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureMacroSettings();

            return txtMacro.Text;
        }
        set
        {
            EnsureMacroSettings();

            txtMacro.Text = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Gets or sets macro resolver of the macro editor.
    /// </summary>
    public MacroResolver Resolver
    {
        get
        {
            return txtMacro.Resolver;
        }
        set
        {
            txtMacro.Resolver = value;
        }
    }


    /// <summary>
    /// If true, auto completion is shown above the editor, otherwise it is below (default position is below).
    /// </summary>
    public bool ShowAutoCompletionAbove
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAutoCompletionAbove"), false);
        }
        set
        {
            SetValue("ShowAutoCompletionAbove", value);
            txtMacro.ShowAutoCompletionAbove = value;
        }
    }


    /// <summary>
    /// If true, toolbar of the editor is displayed
    /// </summary>
    public bool ShowToolbar
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowToolbar"), false);
        }
        set
        {
            SetValue("ShowToolbar", value);
            txtMacro.Editor.ShowToolbar = value;
        }
    }


    /// <summary>
    /// If true, the control signs the output macro
    /// </summary>
    public bool SignMacro
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SignMacro"), false);
        }
        set
        {
            SetValue("SignMacro", value);
        }
    }


    /// <summary>
    /// Gets ClientID of the textbox
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtMacro.ClientID;
        }
    }

    #endregion


    /// <summary>
    /// Ensures the macro settings of the underlying editor
    /// </summary>
    private void EnsureMacroSettings()
    {
        var ed = txtMacro.Editor;
        ed.ProcessMacroSecurity = SignMacro;
        ed.ValueIsMacro = true;
    }


    /// <summary>
    /// Page init
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        var ed = txtMacro.Editor;
        ed.UseSmallFonts = true;
        ed.Height = new Unit("80px");
        ed.ShowLineNumbers = false;

        EnsureMacroSettings();
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set control style and css class
        if (!string.IsNullOrEmpty(ControlStyle))
        {
            txtMacro.Editor.Attributes.Add("style", ControlStyle);
        }
        if (!string.IsNullOrEmpty(CssClass))
        {
            txtMacro.Editor.CssClass = CssClass;
        }
        if (Form != null)
        {
            txtMacro.ResolverName = Form.ResolverName;
        }

        txtMacro.ShowAutoCompletionAbove = ShowAutoCompletionAbove;
        txtMacro.MixedMode = false;

        var ed = txtMacro.Editor;
        ed.ShowToolbar = ShowToolbar;
    }
}