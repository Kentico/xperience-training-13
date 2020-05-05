using System;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;


[assembly: RegisterCustomClass("CSSStylesheetNewControlExtender", typeof(CSSStylesheetNewControlExtender))]

/// <summary>
/// CSS Style sheets edit control extender
/// </summary>
public class CSSStylesheetNewControlExtender : ControlExtender<UIForm>
{
    #region "Variables"

    private FormEngineUserControl mStylesheetText;
    private ExtendedTextArea mEditor;
    private HiddenField hidCompiledCss;
    public const string BLANKOUTPUT = "__blank__";

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns style sheet text form control.
    /// </summary>
    private FormEngineUserControl CssStylesheetCode
    {
        get
        {
            if (mStylesheetText == null)
            {
                mStylesheetText = Control.FieldControls["StylesheetText"];
            }
            return mStylesheetText;
        }
    }


    /// <summary>
    /// Returns inner text area control of style sheet text form control.
    /// </summary>
    private ExtendedTextArea Editor
    {
        get
        {
            if ((mEditor == null) && (CssStylesheetCode != null))
            {
                System.Web.UI.Control macroEditor = CssStylesheetCode.FindControl("ucEditor");
                if (macroEditor != null)
                {
                    mEditor = (ExtendedTextArea)macroEditor.FindControl("txtCode");
                }
            }
            return mEditor;
        }
    }


    /// <summary>
    /// Gets value of the Stylesheet dynamic language form field.
    /// </summary>
    private string LanguageFieldValue
    {
        get
        {
            return ValidationHelper.GetString(Control.GetFieldValue("StylesheetDynamicLanguage"), CssStylesheetInfo.PLAIN_CSS);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the extender.
    /// </summary>
    public override void OnInit()
    {
        Control.OnAfterDataLoad += OnAfterDataLoad;
        Control.Page.LoadComplete += Page_LoadComplete;
        Control.OnBeforeSave += Control_OnBeforeSave;
        Control.PreRender += Control_PreRender;

        hidCompiledCss = new HiddenField();
        hidCompiledCss.ID = "hidCompiledCss";
        hidCompiledCss.Value = BLANKOUTPUT;
        Control.Controls.Add(hidCompiledCss);
    }


    /// <summary>
    /// UI Form Before Save event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Control_OnBeforeSave(object sender, EventArgs e)
    {
        // Ensure proper values of code fields while creating a new stylesheet
        CssStylesheetInfo cssInfo = Control.EditedObject as CssStylesheetInfo;
        if(cssInfo == null)
        {
            return;
        }

        if (cssInfo.IsPlainCss())
        {
            return;
        }

        cssInfo.StylesheetDynamicCode = cssInfo.StylesheetText;

        // Check whether stylesheet dynamic code is valid
        string output;
        string error = ProcessCss(cssInfo.StylesheetDynamicCode, cssInfo.StylesheetDynamicLanguage, out output, hidCompiledCss);
        if (String.IsNullOrEmpty(error))
        {
            return;
        }

        Control.ShowError(error);

        // Reset stylesheet language
        cssInfo.StylesheetDynamicLanguage = CssStylesheetInfo.PLAIN_CSS;
        Control.StopProcessing = true;
    }


    /// <summary>
    /// Page LoadComplete event handler.
    /// </summary>
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        if (Editor != null)
        {
            // Setup the editor syntax highlighting language
            Editor.Language = LanguageCode.GetLanguageEnumFromString("css");
        }

        BaseEditMenu editMenu = ControlsHelper.GetParentControl(Control.ObjectManager.HeaderActions, typeof(BaseEditMenu)) as BaseEditMenu;
        if (editMenu != null)
        {   
            editMenu.OnGetClientActionScript += editMenu_OnGetClientActionScript;
        }
    }


    /// <summary>
    /// Add custom JavaScript code executed when Save button is clicked.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event arguments</param>
    private void editMenu_OnGetClientActionScript(object sender, EditMenuEventArgs e)
    {
        if (e.ActionName == ComponentEvents.SAVE)
        {
            e.ClientActionScript = "if (typeof CompileCss != 'undefined') { CompileCss(); } ";
        }
    }


    /// <summary>
    /// Called when [after data load].
    /// </summary>
    private void OnAfterDataLoad(object sender, EventArgs e)
    {
        // Initialize selected line
        if (!RequestHelper.IsPostBack() && (Editor != null))
        {
            string script = @"
$cmsj(document).ready(function () {
    setTimeout(function() {
        var cmCSS = " + Editor.EditorID + @";
        if(cmCSS != null) {
            cmCSS.setCursor(" + (QueryHelper.GetInteger("line", 0) - 1) + @");
        }
    }, 50);
});";
            // Register client script
            ScriptHelper.RegisterStartupScript(Control, typeof(string), "JumpToLine", script, true);
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    private void Control_PreRender(object sender, EventArgs e)
    {
        RegisterClientSideCompilationScript();
    }


    /// <summary>
    /// Ensures that style sheet code compiled on client-side is stored in context variable.
    /// </summary>
    private static void EnsureClientSideCompiledCss(HiddenField hidden)
    {
        if (RequestHelper.IsPostBack() && String.IsNullOrEmpty(CssPreprocessor.CurrentCompiledValue))
        {
            CssPreprocessor.CurrentCompiledValue = ValidationHelper.GetString(hidden.Value, String.Empty);
        }
    }


    /// <summary>
    /// This method encapsulates both client- and server- side CSS processing. 
    /// </summary>
    /// <param name="input">Stylesheet code in dynamic language syntax</param>
    /// <param name="language">CSS dynamic language</param>
    /// <param name="output">Plain CSS output</param>
    /// <param name="hiddenField">Hidden field that holds the result of client-side compilation</param>
    public static string ProcessCss(string input, string language, out string output, HiddenField hiddenField)
    {
        string errorMessage = String.Empty;
        CssPreprocessor prep = CssStylesheetInfoProvider.GetCssPreprocessor(language);

        EnsureClientSideCompiledCss(hiddenField);

        if (String.IsNullOrEmpty(CssPreprocessor.CurrentCompiledValue) || ((prep != null) && !prep.UsesClientSideCompilation))
        {
            // Parse the style sheet code server-side
            errorMessage = CssStylesheetInfoProvider.TryParseCss(input, language, out output);
            if (String.IsNullOrEmpty(errorMessage))
            {
                // Store last result
                CssPreprocessor.CurrentCompiledValue = output;
            }
        }
        else
        {
            // Use client-side parsed code
            output = CssPreprocessor.CurrentCompiledValue;

            // Check if client-side process result contains special sequence indicating that parser produced no output (this may not be an error)
            if (output.EqualsCSafe(BLANKOUTPUT))
            {
                output = String.Empty;
            }
            else
            {
                if ((prep != null) && (prep.GetErrorDescription != null))
                {
                    // Handle error
                    errorMessage = prep.GetErrorDescription(output);
                    if (!String.IsNullOrEmpty(errorMessage))
                    {
                        output = String.Empty;
                    }
                }
            }
        }

        return errorMessage;
    }


    /// <summary>
    /// Adds JavaScript code required for client-side compilation to the page.
    /// </summary>
    private void RegisterClientSideCompilationScript()
    {
        CssPreprocessor prep = CssStylesheetInfoProvider.GetCssPreprocessor(LanguageFieldValue);
        if (prep != null)
        {
            // Register client scripts
            if (prep.RegisterClientCompilationScripts != null)
            {
                prep.RegisterClientCompilationScripts();
            }
        }
        else
        {
            ScriptHelper.RegisterStartupScript(Control.Page, typeof(string), "CleanUpScript", "function GetCss() {}", true);
        }

        if (Editor != null)
        {
            ScriptHelper.RegisterStartupScript(Control.Page, typeof(string), "CompileScript", GetClientCompilationScript(Editor.ClientID, hidCompiledCss.ClientID), true);
        }
    }


    /// <summary>
    /// Gets client script that ensures invoking style sheet compilation.
    /// </summary>
    /// <param name="editorClientId">Code editor client ID</param>
    /// <param name="hiddenFieldClientId">ID of hidden field which is used for pass compilation result to the server</param>
    public static string GetClientCompilationScript(string editorClientId, string hiddenFieldClientId)
    {
        string script = @"
var CompileCss = function() {
    var editorElement = document.getElementById('" + editorClientId + @"');
    var hidElement = document.getElementById('" + hiddenFieldClientId + @"');
    
    if (!editorElement || !hidElement) return;
    
    // Compile code 
    if (typeof GetCss != 'undefined') {
        var output = GetCss(editorElement.value);
        if (output && (output.length > 0))
        {
            hidElement.value = output;
        } else {
            hidElement.value = '" + BLANKOUTPUT + @"';
        }
    }
}
";
        return script;
    }

    #endregion
}