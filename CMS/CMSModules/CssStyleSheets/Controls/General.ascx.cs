using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_CssStyleSheets_Controls_General : CMSPreviewControl
{
    #region "Variables"

    protected bool startWithFullScreen = true;
    private CssStylesheetInfo si = null;
    private int previewState = 0;
    private FormEngineUserControl mStylesheetText = null;
    private ExtendedTextArea mEditor = null;
    private bool languageChangeProceeded = false;
    private bool codePreviewDisplayed = false;
    private bool plainCssOnly = true;
    private bool isDialog;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessagePlaceholder;
        }
    }


    /// <summary>
    /// Returns stylesheet text form control.
    /// </summary>
    private FormEngineUserControl CssStylesheetCode
    {
        get
        {
            if (mStylesheetText == null)
            {
                // Get selected style sheet language.
                FormEngineUserControl langControl = EditForm.FieldControls["StylesheetDynamicLanguage"];

                if (langControl != null)
                {
                    string lang = langControl.Value as string;

                    // According to the style sheet language return the proper form control.
                    if (String.IsNullOrEmpty(lang) || lang.EqualsCSafe(CssStylesheetInfo.PLAIN_CSS, true))
                    {
                        mStylesheetText = EditForm.FieldControls["StylesheetText"];
                    }
                    else
                    {
                        mStylesheetText = EditForm.FieldControls["StylesheetDynamicCode"];
                    }
                }
            }
            return mStylesheetText;
        }
    }


    /// <summary>
    /// Returns inner text area control of stylesheet text form control.
    /// </summary>
    private ExtendedTextArea Editor
    {
        get
        {
            if ((mEditor == null) && (CssStylesheetCode != null))
            {
                Control macroEditor = CssStylesheetCode.FindControl("ucEditor");
                if (macroEditor != null)
                {
                    mEditor = (ExtendedTextArea)macroEditor.FindControl("txtCode");
                }
            }
            return mEditor;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the control.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        isDialog = (QueryHelper.GetBoolean("dialog", false) || QueryHelper.GetBoolean("isindialog", false));

        Page.LoadComplete += Page_LoadComplete;
        EditForm.PreRender += EditForm_PreRender;
        editMenuElem.ObjectManager.OnAfterAction += ObjectManager_OnAfterAction;
        editMenuElem.ObjectEditMenu.OnGetClientActionScript += ObjectEditMenu_OnGetClientActionScript;

        hidCompiledCss.Value = CSSStylesheetNewControlExtender.BLANKOUTPUT;
    }


    /// <summary>
    /// Event handler called on getting client script that is supposed to be executed on action.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void ObjectEditMenu_OnGetClientActionScript(object sender, EditMenuEventArgs e)
    {
        if ((e.ActionName == ComponentEvents.SAVE) || (e.ActionName == ComponentEvents.CHECKIN))
        {
            e.ClientActionScript = "if (typeof CompileCss != 'undefined') {if (window.Loader) { window.Loader.show(); }  CompileCss();} ";
        }
    }


    /// <summary>
    /// Handles OnAfterAction event of the ObjectManager.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (e.ActionName == ComponentEvents.UNDO_CHECKOUT)
        {
            // Refresh the page after undo-checkout is performed
            URLHelper.Redirect(RequestContext.CurrentURL);
        }

        if (e.ActionName == ComponentEvents.SAVE)
        {
            if ((Editor != null) && Editor.ShowBookmarks)
            {
                // Refresh bookmarks 
                Editor.ExtractBookmarks();
                String scriptBookmark = String.Format("RefreshBookmarks({0}, {1});", Editor.EditorID, Editor.GetPreformattedBookmarks());
                ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "RefreshBookmarks", ScriptHelper.GetScript(scriptBookmark));
            }

            // Refresh preview after save
            RegisterRefreshScript();
        }

        if (isDialog)
        {
            switch (e.ActionName)
            {
                case ComponentEvents.SAVE:
                case ComponentEvents.CHECKOUT:
                case ComponentEvents.UNDO_CHECKOUT:
                case ComponentEvents.CHECKIN:
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "wopenerRefresh", ScriptHelper.GetScript("if (wopener && wopener.refresh) { wopener.refresh(); }"));
                    break;
            }
        }
    }


    /// <summary>
    /// Handles PreRender event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void EditForm_PreRender(object sender, EventArgs e)
    {
        // Disable text area of the code preview
        if (codePreviewDisplayed)
        {
            // If UseCheckinCheckout is enabled there needs to run EnableByLockState before disabling code preview 
            if (SynchronizationHelper.UseCheckinCheckout)
            {
                EditForm.EnableByLockState();
                EditForm.EnabledByLockState = false;
            }

            EditingFormControl efc = EditForm.FieldEditingControls["StylesheetText"];
            if (efc != null)
            {
                efc.Enabled = false;
            }
        }
    }


    /// <summary>
    /// Handle OnAfterValidate event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void EditForm_OnAfterValidate(object sender, EventArgs e)
    {
        // Validate the dynamic code
        if (!languageChangeProceeded && !EditForm.IsInsertMode)
        {
            // Save button doesn't change style sheet language, reset field value to the data base value
            EditForm.FieldControls["StylesheetDynamicLanguage"].Value = EditForm.GetDataValue("StylesheetDynamicLanguage");
            string lang = ValidationHelper.GetString(EditForm.GetDataValue("StylesheetDynamicLanguage"), CssStylesheetInfo.PLAIN_CSS);
            
            if (!lang.EqualsCSafe(CssStylesheetInfo.PLAIN_CSS))
            {
                string output = String.Empty;
                string dynamicCode = EditForm.GetFieldValue("StylesheetDynamicCode") as string;

                string error = CSSStylesheetNewControlExtender.ProcessCss(dynamicCode, lang, out output, hidCompiledCss);

                if (!String.IsNullOrEmpty(error))
                {
                    EditForm.AddError(error);
                    EditForm.StopProcessing = true;
                }
                // Keep the current value for better performance
                else
                {
                    CssPreprocessor.CurrentCompiledValue = output;
                }
            }
        }

        RegisterHideProgress();
    }


    /// <summary>
    /// Handles OnBeforeDataLoad event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void EditForm_OnBeforeDataLoad(object sender, EventArgs e)
    {
        if (!EditForm.IsInsertMode)
        {
            // Ensure that dynamic language exists
            string lang = ValidationHelper.GetString(EditForm.GetDataValue("StylesheetDynamicLanguage"), String.Empty);
            CssPreprocessor prep = CssStylesheetInfoProvider.GetCssPreprocessor(lang);
            EditForm.Data["StylesheetDynamicLanguage"] = (prep != null) ? lang : CssStylesheetInfo.PLAIN_CSS;
        }

        // Set the flag in order to bypass additional logic in the case when no CSS preprocessor is registered
        if (CssStylesheetInfoProvider.CssPreprocessors.Count > 0)
        {
            plainCssOnly = false;
        }
    }


    /// <summary>
    /// UI Form Before Save event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Ignore the change of stylesheet language in insert mode or if the flag is set
        if (!languageChangeProceeded && !EditForm.IsInsertMode && !plainCssOnly)
        {
            CssStylesheetInfo cssInfo = EditForm.EditedObject as CssStylesheetInfo;
            if (cssInfo != null)
            {
                cssInfo.StylesheetDynamicLanguage = ValidationHelper.GetString(EditForm.EditedObject.GetOriginalValue("StylesheetDynamicLanguage"), CssStylesheetInfo.PLAIN_CSS);
            }
        }
    }


    /// <summary>
    /// Page 'Load' event handler.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Bind the 'Changed' event handler for StylesheetDynamicLanguage.
        FormEngineUserControl langControl = EditForm.FieldControls["StylesheetDynamicLanguage"];
        if (langControl != null)
        {
            langControl.Changed += langControl_Changed;
        }

        // Show 'Changes were saved' message according to the query parameter.
        if (!RequestHelper.IsPostBack() && (QueryHelper.GetInteger("saved", 0) == 1))
        {
            EditForm.ShowChangesSaved();
        }
    }


    /// <summary>
    /// 'Changed' event handling for StylesheetDynamicLanguage field.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void langControl_Changed(object sender, EventArgs e)
    {
        // Go on only if form validation succeeded
        if (EditForm.StopProcessing)
        {
            return;
        }

        string originalLanguage = ValidationHelper.GetString(EditForm.GetDataValue("StylesheetDynamicLanguage"), String.Empty);
        string newLanguage = ValidationHelper.GetString(EditForm.GetFieldValue("StylesheetDynamicLanguage"), String.Empty);

        ChangeStylesheetLanguage(originalLanguage, newLanguage);

        if (!EditForm.IsInsertMode)
        {
            // Save data and refresh the page if the form is not in insert mode
            string redirectUrl = null;

            redirectUrl = URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "saved", "1");

            /* Determine whether the StylesheetDynamicCode field control is visible in order to ensure correct value of the field. If it is not visible then BasicForm ignores the value of the field.
             * In that case it is necessary to set value of the field manually. */
            FormEngineUserControl ctrl = EditForm.FieldControls["StylesheetDynamicCode"];
            if ((ctrl != null) && !ctrl.Visible)
            {
                CssStylesheetInfo cssInfo = EditForm.EditedObject as CssStylesheetInfo;
                if (cssInfo != null)
                {
                    cssInfo.StylesheetDynamicCode = EditForm.GetFieldValue("StylesheetDynamicCode") as string;
                }
            }

            /* Determine whether the StylesheetText field control is visible in order to ensure correct value of the field. If it is not visible then BasicForm ignores the value of the field.
             * In that case it is necessary to set value of the field manually. */
            ctrl = EditForm.FieldControls["StylesheetText"];
            if ((ctrl != null) && !ctrl.Visible)
            {
                CssStylesheetInfo cssInfo = EditForm.EditedObject as CssStylesheetInfo;
                if (cssInfo != null)
                {
                    cssInfo.StylesheetText = EditForm.GetFieldValue("StylesheetText") as string;
                }
            }

            EditForm.SaveData(redirectUrl);
        }
    }


    /// <summary>
    /// Page LoadComplete event handler.
    /// </summary>
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        HandleCssCodePreview();

        // Setup the editor syntax highlighting according to the selected language
        string lang = EditForm.GetDataValue("StylesheetDynamicLanguage") as string;
        string previewMode = EditForm.GetFieldValue("StylesheetCodePreview") as string;

        if (String.IsNullOrEmpty(lang) || lang.EqualsCSafe(CssStylesheetInfo.PLAIN_CSS, true) || (!String.IsNullOrEmpty(previewMode) && previewMode.EqualsCSafe("preview", true)))
        {
            Editor.Language = LanguageCode.GetLanguageEnumFromString("css");
        }
        else
        {
            LanguageEnum langEnum = LanguageCode.GetLanguageEnumFromString(lang);
            Editor.Language = (langEnum != default(LanguageEnum)) ? langEnum : LanguageEnum.CSS;
        }
    }


    /// <summary>
    /// Displays and hides plain CSS code preview.
    /// </summary>
    private void HandleCssCodePreview()
    {
        if (RequestHelper.IsPostBack() && !plainCssOnly)
        {
            // Don't process if the CSS preview control is not visible
            FormEngineUserControl previewControl = EditForm.FieldControls["StylesheetCodePreview"];
            if ((previewControl != null) && !previewControl.Visible)
            {
                return;
            }

            // Get the edited stylesheet
            CssStylesheetInfo cssInfo = EditForm.EditedObject as CssStylesheetInfo;
            if (cssInfo == null)
            {
                return;
            }

            string lang = ValidationHelper.GetString(cssInfo.StylesheetDynamicLanguage, CssStylesheetInfo.PLAIN_CSS);
            string previewMode = ValidationHelper.GetString(EditForm.GetFieldValue("StylesheetCodePreview"), String.Empty);

            // Display preview
            if (!String.IsNullOrEmpty(previewMode) && previewMode.EqualsCSafe("preview", true) && !lang.EqualsCSafe(CssStylesheetInfo.PLAIN_CSS, true))
            {
                // Get the stylesheet dynamic code
                string code = ValidationHelper.GetString(EditForm.GetFieldValue("StylesheetDynamicCode"), String.Empty);
                string output = String.Empty;

                // Run preprocessor
                string parserError = CSSStylesheetNewControlExtender.ProcessCss(code, lang, out output, hidCompiledCss);

                if (String.IsNullOrEmpty(parserError))
                {
                    // Set the editor value and disable editing
                    FormEngineUserControl plainCssEditor = EditForm.FieldControls["StylesheetText"];
                    if (plainCssEditor != null)
                    {
                        plainCssEditor.Value = output;
                        codePreviewDisplayed = true;
                    }

                    // Hide dynamic code editor
                    FormEngineUserControl dynamicCodeEditor = EditForm.FieldControls["StylesheetDynamicCode"];
                    if (dynamicCodeEditor != null)
                    {
                        dynamicCodeEditor.CssClass = "Hidden";
                    }
                }
                else
                {
                    // Handle the error that occurred during parsing
                    EditForm.ShowError(parserError);
                    FormEngineUserControl previewField = EditForm.FieldControls["StylesheetCodePreview"];
                    if (previewField != null)
                    {
                        previewField.Value = lang;
                    }
                }

                return;
            }

            // Hide preview
            if (!String.IsNullOrEmpty(previewMode) && !previewMode.EqualsCSafe("preview", true))
            {
                // Ensure the visibility of the field if the preview is not active
                FormEngineUserControl dynamicCodeEditor = EditForm.FieldControls["StylesheetDynamicCode"];
                if (dynamicCodeEditor != null)
                {
                    dynamicCodeEditor.CssClass = "";
                }

                // Enable for editing despite the editor is hidden
                FormEngineUserControl plainCssEditor = EditForm.FieldControls["StylesheetText"];
                if (plainCssEditor != null)
                {
                    plainCssEditor.Enabled = true;
                }
            }
        }
    }


    /// <summary>
    /// Called when [after data load].
    /// </summary>
    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Get preview position
        previewState = GetPreviewStateFromCookies(CSSSTYLESHEET);
    }


    /// <summary>
    /// Register script for hiding progress bar
    /// </summary>
    private void RegisterHideProgress()
    {
        ScriptHelper.RegisterClientScriptBlock(pnlUpdate, pnlUpdate.GetType(), "HideProgress", ScriptHelper.GetScript("if (window.Loader) { window.Loader.hide(); }"));
    }


    /// <summary>
    /// Detects stylesheet language change and converts CSS code according to the former and the latter language.
    /// </summary>
    /// <param name="originalLanguage">The previous language</param>
    /// <param name="newLanguage">The new language</param>
    private void ChangeStylesheetLanguage(string originalLanguage, string newLanguage)
    {
        // Check whether the stylesheet language has actually changed.
        if (!String.IsNullOrEmpty(newLanguage) && !String.IsNullOrEmpty(originalLanguage) && !originalLanguage.EqualsCSafe(newLanguage, true))
        {
            string code = String.Empty;
            string output = String.Empty;
            string errorMessage = String.Empty;

            if (newLanguage.EqualsCSafe(CssStylesheetInfo.PLAIN_CSS, true))
            {
                // Changed from any CSS preprocessor language to plain CSS.
                code = ValidationHelper.GetString(EditForm.GetFieldValue("StylesheetDynamicCode"), String.Empty);
                // Try to parse the code against the original preprocessor language.
                errorMessage = CSSStylesheetNewControlExtender.ProcessCss(code, originalLanguage, out output, hidCompiledCss);
            }
            else if (originalLanguage.EqualsCSafe(CssStylesheetInfo.PLAIN_CSS, true))
            {
                // Changed from Plain CSS to some CSS preprocessor language.
                code = ValidationHelper.GetString(EditForm.GetFieldValue("StylesheetText"), String.Empty);
                // Try to parse the code against the new preprocessor language.
                errorMessage = CSSStylesheetNewControlExtender.ProcessCss(code, newLanguage, out output, hidCompiledCss);
            }
            else
            {
                // Changed from CSS preprocessor language to another one.
                code = ValidationHelper.GetString(EditForm.GetFieldValue("StylesheetDynamicCode"), String.Empty);
                // Try to parse the code against the original preprocessor language.
                errorMessage = CSSStylesheetNewControlExtender.ProcessCss(code, originalLanguage, out output, hidCompiledCss);
            }

            if (String.IsNullOrEmpty(errorMessage))
            {
                // Success -> Set correct values to form controls.
                if (newLanguage.EqualsCSafe(CssStylesheetInfo.PLAIN_CSS, true))
                {
                    // output -> StylesheetText
                    EditForm.FieldControls["StylesheetText"].Value = output;
                }
                else if (originalLanguage.EqualsCSafe(CssStylesheetInfo.PLAIN_CSS, true))
                {
                    // Copy the original code to StylesheetDynamicCode field.
                    EditForm.FieldControls["StylesheetDynamicCode"].Value = code;
                }
                else
                {
                    // Copy the original code to StylesheetDynamicCode field.
                    EditForm.FieldControls["StylesheetDynamicCode"].Value = output;
                }

                // Prevent to additional change processing.
                languageChangeProceeded = true;
            }
            else
            {
                // Failure -> Show error message
                EditForm.AddError(errorMessage);
                // Prevent further processing
                EditForm.StopProcessing = true;
                // Set drop-down list to its former value
                FormEngineUserControl langControl = EditForm.FieldControls["StylesheetDynamicLanguage"];
                if (langControl != null)
                {
                    langControl.Value = originalLanguage;
                    CssStylesheetInfo cssInfo = EditForm.EditedObject as CssStylesheetInfo;
                    if (cssInfo != null)
                    {
                        cssInfo.StylesheetDynamicLanguage = originalLanguage;
                    }
                }
            }
        }
    }


    /// <summary>
    /// OnLoad event.
    /// </summary>
    /// <param name="e">Event argument</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        si = EditForm.EditedObject as CssStylesheetInfo;

        previewState = GetPreviewStateFromCookies(CSSSTYLESHEET);

        // Add preview action
        HeaderAction preview = new HeaderAction
        {
            Text = GetString("general.preview"),
            OnClientClick = "performToolbarAction('split');return false;",
            Visible = (previewState == 0),
            Tooltip = GetString("preview.tooltip"),
            Index = 1
        };

        editMenuElem.ObjectEditMenu.AddExtraAction(preview);
        editMenuElem.ObjectEditMenu.PreviewMode = true;
        editMenuElem.MenuPanel.CssClass = "PreviewMenu";

#pragma warning disable CS0618 // Type or member is obsolete
        bool hide = !(BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
#pragma warning restore CS0618 // Type or member is obsolete
        if (hide)
        {
            pnlContainer.CssClass += " Hidden ";
        }
    }


    /// <summary>
    /// OnPreRender event of the control.
    /// </summary>
    /// <param name="e">Event argument</param>
    protected override void OnPreRender(EventArgs e)
    {
        FormEngineUserControl plainCssPreviewControl = EditForm.FieldControls["StylesheetCodePreview"];
        CMSRadioButtonList radios = ControlsHelper.GetChildControl<CMSRadioButtonList>(plainCssPreviewControl);

        // Bind click event handler to radio buttons in order to compile CSS client-side after Plain CSS preview button is clicked
        if (radios != null)
        {
            string radioScript = @"
function CompileCssForPreview(obj) {
    if (obj.value == 'preview') {
        CompileCss();
    }
}
";

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CompileCssForPreview", radioScript, true);
            foreach (ListItem li in radios.Items)
            {
                li.Attributes.Add("onclick", "CompileCssForPreview(this)");
            }
        }

        // Register script for client-side compilation
        RegisterClientSideCompilationScript();

        startWithFullScreen = ((previewState != 0) && editMenuElem.ObjectManager.IsObjectChecked());
        RegisterInitScripts(pnlContainer.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);

        if (Editor != null)
        {
            Editor.ShowBookmarks = true;
            Editor.EnablePositionMember = true;
            Editor.EnableSections = true;
            Editor.AutoSize = true;
            Editor.ParentElementID = ParentClientID;

            // Initialize selected line
            string script = @"
$cmsj(document).ready(function () {
    setTimeout(function() {
        var cmCSS = document.getElementById('" + Editor.EditorID + @"');
        if(cmCSS != null) {
            cmCSS.setCursor(" + (QueryHelper.GetInteger("line", 0) - 1) + @");
        }
    }, 50);
});";
            // Register client script
            ScriptHelper.RegisterStartupScript(EditForm, typeof(string), "JumpToLine", script, true);
        }

        // Correct offset for displaying preview
        if ((previewState != 0) && (CssStylesheetCode != null))
        {
            CssStylesheetCode.SetValue("TopOffset", 40);
        }

        // Register loader script
        ScriptHelper.RegisterLoader(Page);

        base.OnPreRender(e);
    }


    /// <summary>
    /// Adds JavaScript code required for client-side compilation to the page.
    /// </summary>
    private void RegisterClientSideCompilationScript()
    {
        // Get dynamic language database value
        string originalLang = ValidationHelper.GetString(EditForm.GetDataValue("StylesheetDynamicLanguage"), CssStylesheetInfo.PLAIN_CSS);

        // Get dynamic language selected in the form
        string newLang = ValidationHelper.GetString(EditForm.GetFieldValue("StylesheetDynamicLanguage"), CssStylesheetInfo.PLAIN_CSS);

        CssPreprocessor prep = null;

        // Indicate post-back and that dynamic language has changed
        if (RequestHelper.IsPostBack() && !originalLang.EqualsCSafe(newLang, true))
        {
            /*
             * If the original language is plain CSS then load preprocessor according to new language. If the original language is dynamic then load 
             * appropriate preprocessor and ignore selected new language.
             */
            prep = CssStylesheetInfoProvider.GetCssPreprocessor(originalLang) ?? CssStylesheetInfoProvider.GetCssPreprocessor(newLang);
        }
        else
        {
            prep = CssStylesheetInfoProvider.GetCssPreprocessor(newLang);
        }

        // Register client scripts for appropriate preprocessor
        if (prep != null)
        {
            if (prep.RegisterClientCompilationScripts != null)
            {
                prep.RegisterClientCompilationScripts();
            }
        }

        if (Editor != null)
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "CompileScript", CSSStylesheetNewControlExtender.GetClientCompilationScript(Editor.ClientID, hidCompiledCss.ClientID), true);
        }
    }

    #endregion
}