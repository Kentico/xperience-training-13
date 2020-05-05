using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Macros_MacroEditor : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the value of this form control.
    /// </summary>
    /// <value>Text content of this editor</value>
    [Browsable(false)]
    public override object Value
    {
        get
        {
            return ucEditor.Text;
        }
        set
        {
            ucEditor.Text = (string)value;
        }
    }


    /// <summary>
    /// Gets or sets whether this form control is enabled.
    /// </summary>
    /// <value>True, if form control is enabled, otherwise false</value>
    [Browsable(true)]
    [Description("Determines whether this form control is enabled")]
    [Category("Form Control")]
    [DefaultValue(true)]
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = ucEditor.Editor.Enabled = value;
        }
    }


    /// <summary>
    /// Gets the editor control.
    /// </summary>
    public ExtendedTextArea Editor
    {
        get
        {
            return ucEditor.Editor;
        }
    }


    /// <summary>
    /// Gets or sets the left offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int LeftOffset
    {
        get
        {
            return ucEditor.LeftOffset;
        }
        set
        {
            ucEditor.LeftOffset = value;
        }
    }


    /// <summary>
    /// Gets or sets the top offset of the autocomplete control (to position it correctly).
    /// </summary>
    public int TopOffset
    {
        get
        {
            return ucEditor.TopOffset;
        }
        set
        {
            ucEditor.TopOffset = value;
        }
    }


    public override bool EnableViewState
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableViewState"), true);
        }
        set
        {
            SetValue("EnableViewState", value);
        }
    }


    public string Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Height"), "");
        }
        set
        {
            SetValue("Height", value);
        }
    }


    public string Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Width"), "");
        }
        set
        {
            SetValue("Width", value);
        }
    }


    public bool AutoSize
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoSize"), false);
        }
        set
        {
            SetValue("AutoSize", value);
        }
    }


    public LanguageEnum Language
    {
        get
        {
            return (LanguageEnum)ValidationHelper.GetInteger(GetValue("Language"), 0);
        }
        set
        {
            SetValue("Language", (int)value);
            Editor.Language = value;
        }
    }


    public bool SingleLineMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SingleLineMode"), false);
        }
        set
        {
            SetValue("SingleLineMode", value);
        }
    }


    public override string ResolverName
    {
        get
        {
            string resolverName = ValidationHelper.GetString(GetValue("ResolverName"), base.ResolverName);
            if (Form != null && Form.ContextResolver != null)
            {
                resolverName = Form.ContextResolver.ResolveMacros(resolverName);
            }

            return resolverName;
        }
        set
        {
            SetValue("ResolverName", value);
        }
    }


    /// <summary>
    /// If macro editor support pasting images, it must register "paste script".
    /// </summary>
    public bool SupportPasteImages
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SupportPasteImages"), false);
        }
        set
        {
            SetValue("SupportPasteImages", value);
        }
    }


    /// <summary>
    /// If true, the whole text is considered as a K# expression and IntelliSense is available everywhere. If false, K# expressions are considered only within {% %} environment and IntelliSense rises only there.
    /// </summary>
    public bool SingleMacroMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SingleMacroMode"), false);
        }
        set
        {
            SetValue("SingleMacroMode", value);
        }
    }


    /// <summary>
    /// Gets or sets if the bookmarks panel is shown in the code editor.
    /// </summary>
    /// <value>True, if the bookmarks should be visible, otherwise false. Default is true.</value>        
    public bool ShowBookmarks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowBookmarks"), false);
        }
        set
        {
            Editor.ShowBookmarks = value;
            SetValue("ShowBookmarks", value);
        }
    }


    /// <summary>
    /// Gets or sets the regular expression which is used to detect bookmarks.
    /// </summary>
    public string RegularExpression
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RegularExpression"), String.Empty);
        }
        set
        {
            Editor.RegularExpression = value;
            SetValue("RegularExpression", value);
        }
    }


    /// <summary>
    /// Determines whether the position member is enabled.
    /// </summary>
    public bool EnablePositionMember
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePositionMember"), false);
        }
        set
        {
            Editor.EnablePositionMember = value;
            SetValue("EnablePositionMember", value);
        }
    }


    /// <summary>
    /// Determines whether whether the bookmarks are enabled.
    /// </summary>
    public bool EnableSections
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableSections"), false);
        }
        set
        {
            Editor.EnableSections = value;
            SetValue("EnableSections", value);
        }
    }


    /// <summary>
    /// Determines if the line numbers are displayed.
    /// </summary>
    public bool ShowLineNumbers
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowLineNumbers"), false);
        }
        set
        {
            Editor.ShowLineNumbers = value;
            SetValue("ShowLineNumbers", value);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!String.IsNullOrEmpty(Width))
        {
            ucEditor.Width = new Unit(Width);
        }

        if (!String.IsNullOrEmpty(Height))
        {
            ucEditor.Height = new Unit(Height);
        }

        Editor.EditorMode = EditorModeEnum.Advanced;
        Editor.AutoSize = AutoSize;
        Editor.Language = Language;
        Editor.ShowBookmarks = ShowBookmarks;
        Editor.EnableSections = EnableSections;
        Editor.EnablePositionMember = EnablePositionMember;
        Editor.RegularExpression = RegularExpression;
        Editor.ShowLineNumbers = ShowLineNumbers;

        ucEditor.SingleLineMode = SingleLineMode;
        ucEditor.MixedMode = !SingleMacroMode;
        ucEditor.ResolverName = ResolverName;
        ucEditor.EnableViewState = EnableViewState;

        // Initialize insert macro button
        btnInsertMacro.OnClientClick = ucEditor.Editor.EditorID + ".createInsertMacroWindow(); return false;";
        btnInsertMacro.ToolTip = GetString("macroeditor.insertmacro");
        if (SingleLineMode)
        {
            plcInsertMacro.Visible = true;
        }

        if (SupportPasteImages)
        {
            RegisterPasteScript();
        }
    }

    #endregion


    #region "Custom methods"

    /// <summary>
    /// Registers script for pasting images into macro editor.
    /// </summary>
    protected void RegisterPasteScript()
    {
        string script =
String.Format(@"
function PasteImage(imageurl) {{
        if (imageurl.substr(0, 1) === '/') {{
            var imageurl = '~' + imageurl;
        }}
        var imageHtml = '<img src=""' + imageurl + '"" alt="""" />';
        if (InsertMacroExtended != null) {{
            InsertMacroExtended(imageHtml, (typeof({0}) != 'undefined' ? {0} : null), '{1}');
        }}
    }}", Editor.EditorID, Editor.ClientID);

        ScriptHelper.RegisterScriptFile(Page, "Macros/MacroSelector.js");
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "PasteImage_" + ClientID, script, true);
    }

    #endregion
}