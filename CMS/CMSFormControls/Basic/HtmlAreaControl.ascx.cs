using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.CKEditor.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Internal;
using CMS.DocumentEngine.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSFormControls_Basic_HtmlAreaControl : FormEngineUserControl, IControlWithSentimentAnalysisComponent
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return editor.Enabled;
        }
        set
        {
            editor.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return editor.ResolvedValue;
        }
        set
        {
            editor.ResolvedValue = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Returns value prepared for validation.
    /// </summary>
    public override object ValueForValidation
    {
        get
        {
            var plainText = editor.ResolvedValue;
            if (!String.IsNullOrEmpty(plainText))
            {
                plainText = Regex.Replace(plainText, @"(>)(\r|\n)*(<)", "><");
                plainText = Regex.Replace(plainText, "(<[^>]*>)([^<]*)", "$2");
                // Just substitute spec.chars with one letter
                plainText = Regex.Replace(plainText, "(&#x?[0-9]{2,4};|&quot;|&amp;|&nbsp;|&lt;|&gt;|&euro;|&copy;|&reg;|&permil;|&Dagger;|&dagger;|&lsaquo;|&rsaquo;|&bdquo;|&rdquo;|&ldquo;|&sbquo;|&rsquo;|&lsquo;|&mdash;|&ndash;|&rlm;|&lrm;|&zwj;|&zwnj;|&thinsp;|&emsp;|&ensp;|&tilde;|&circ;|&Yuml;|&scaron;|&Scaron;)", "@");
            }
            return plainText;
        }
    }


    /// <summary>
    /// Toolbar used in editor.
    /// </summary>
    public string ToolbarSet
    {
        get
        {
            return editor.ToolbarSet;
        }
        set
        {
            editor.ToolbarSet = value;
        }
    }


    /// <summary>
    /// Gets current editor.
    /// </summary>
    public CMSHtmlEditor CurrentEditor
    {
        get
        {
            return editor;
        }
    }


    /// <summary>
    /// Shows or hides Add stamp button.
    /// </summary>
    public bool ShowAddStampButton
    {
        get
        {
            return GetValue("ShowAddStampButton", false);
        }
        set
        {
            SetValue("ShowAddStampButton", value);
        }
    }


    /// <summary>
    /// Returns Html value for stamp.
    /// </summary>
    protected string StampValue
    {
        get
        {
            string stamp = SettingsKeyInfoProvider.GetValue("CMSCMStamp", SiteContext.CurrentSiteName);
            string addStampValue = MacroResolver.Resolve(stamp).Replace("'", @"\'");

            return "<div>" + addStampValue + "</div>";
        }
    }


    /// <summary>
    /// List of plugins that must not be loaded.
    /// This is a tool setting which makes it easier to avoid loading plugins defined in the CKEDITOR.config.plugins setting,
    /// without having to touch it and potentially breaking it.
    /// </summary>
    public ICollection<string> RemovePlugins
    {
        get
        {
            return editor.RemovePlugins;
        }
    }


    /// <summary>
    /// Whether to enable the resizing feature.
    /// If disabled the resize handler will not be visible.
    ///
    /// Default value: true
    /// </summary>
    public bool ResizeEnabled
    {
        get
        {
            return editor.ResizeEnabled;
        }
        set
        {
            editor.ResizeEnabled = value;
        }
    }


    /// <summary>
    /// Height of the editor area.
    /// </summary>
    public Unit Height
    {
        get
        {
            object height = GetValue("height");
            if (height == null)
            {
                return editor.Height;
            }

            UnitType heightUnitType = GetUnitType(GetValue("heightunittype"));
            return new Unit(ValidationHelper.GetInteger(height, 300), heightUnitType);
        }
        set
        {
            editor.Height = value;
        }
    }


    /// <summary>
    /// Width of the editor area.
    /// </summary>
    public Unit Width
    {
        get
        {
            object width = GetValue("width");
            if (width == null && (editor.Width.Value != 0.0))
            {
                return editor.Width;
            }

            UnitType widthUnitType = GetUnitType(GetValue("widthunittype"));
            return new Unit(ValidationHelper.GetInteger(width, 700), widthUnitType);
        }
        set
        {
            editor.Width = value;
        }
    }


    /// <summary>
    /// Indicates whether the sentiment analysis component should be rendered.
    /// </summary>
    public bool RenderSentimentAnalysisComponent { get; set; } = false;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set control properties
        editor.AutoDetectLanguage = false;
        editor.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        if (Form != null)
        {
            // Set current node
            var node = Form.EditedObject as CMS.DocumentEngine.TreeNode;
            if (node != null)
            {
                editor.Node = node;
            }
        }

        // Get editor area toolbar
        editor.ToolbarSet = DataHelper.GetNotEmpty(GetValue("toolbarset"), (Form != null) ? Form.HtmlAreaToolbar : String.Empty);
        editor.ToolbarLocation = DataHelper.GetNotEmpty(GetValue("toolbarlocation"), (Form != null) ? Form.HtmlAreaToolbarLocation : String.Empty);

        // Set form dimensions
        editor.Width = Width;
        editor.Height = Height;

        // Get editor area starting path
        String startingPath = ValidationHelper.GetString(GetValue("startingpath"), String.Empty);
        editor.StartingPath = startingPath;

        // Set current context resolver
        editor.ResolverName = ResolverName;

        // Get editor area css file
        string cssStylesheetUrl = ValidationHelper.GetString(GetValue("cssstylesheeturl"), String.Empty);

        if (SiteContext.CurrentSite != null)
        {
            // Set editor area css
            if (!string.IsNullOrEmpty(cssStylesheetUrl))
            {
                var presentationUrl = new PresentationUrlRetriever().RetrieveForAdministration(SiteContext.CurrentSiteName);                
                editor.EditorAreaCSS = URLHelper.CombinePath(cssStylesheetUrl, '/', presentationUrl, null);
            }
            else
            {
                editor.EditorAreaCSS = PortalHelper.GetHtmlEditorAreaCss(SiteContext.CurrentSiteName);
            }
        }

        // Set direction
        editor.ContentsLangDirection = CultureHelper.IsPreferredCultureRTL() ? LanguageDirection.RightToLeft : LanguageDirection.LeftToRight;

        // Get dialog configuration
        DialogConfiguration mediaConfig = GetDialogConfiguration();
        if (mediaConfig != null)
        {
            // Override starting path from main configuration
            if (!String.IsNullOrEmpty(startingPath))
            {
                mediaConfig.ContentStartingPath = startingPath;
                mediaConfig.LibStartingPath = startingPath;
            }

            // Set configuration for 'Insert image or media' dialog
            editor.MediaDialogConfig = mediaConfig;
            // Set configuration for 'Insert link' dialog
            editor.LinkDialogConfig = mediaConfig.Clone();
            // Set configuration for 'Quickly insert image' dialog
            editor.QuickInsertConfig = mediaConfig.Clone();
        }

        // Set CSS settings
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            editor.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!String.IsNullOrEmpty(CssClass))
        {
            editor.CssClass = CssClass;
        }

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;

        if (ShowAddStampButton)
        {
            //Add stamp button
            RegisterAndShowStampButton();
        }
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        if (RenderSentimentAnalysisComponent)
        {
            var initFunction = @"
var ckeditor = CKEDITOR.instances['" + editor.ClientID + @"'];
ckeditor.on('instanceReady',function() {
    var ckeditorElement = CKEDITOR.instances['" + editor.ClientID + @"'].container.$;  
    sentimentAnalysisComponent.relatedField = ckeditorElement;        
    sentimentAnalysisComponent.dataset.sentimentAnalysisForSelector = '#" + editor.ClientID + @" + div';
});
sentimentAnalysisComponent.getText = function() {
    var text = CKEDITOR.instances['" + editor.ClientID + @"'].getData();
    // Encapsulate content from CKEditor to DIV element since it can directly contain text
    return $cmsj('<div/>').html(text).text();
};
ckeditor.on('change', function() { 
    sentimentAnalysisComponent.onTextChanged(); 
});
";

            var sentimentAnalysisButton = new SentimentAnalysisButton(initFunction);
            Controls.Add(sentimentAnalysisButton);
        }
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

        if (maxControlSize > 0)
        {

            string error = null;
            bool valid = CheckLength(0, maxControlSize, ValidationHelper.GetString(ValueForValidation, string.Empty).Length, ref error);
            ValidationError = error;
            return valid;
        }
        return true;
    }


    /// <summary>
    /// Returns the list of the field IDs (Client IDs of the inner controls) that should be spell checked.
    /// </summary>
    public override List<string> GetSpellCheckFields()
    {
        List<string> result = new List<string>();
        result.Add(editor.ClientID);
        return result;
    }


    private UnitType GetUnitType(object unitTypeValue)
    {
        if (unitTypeValue != null)
        {
            switch (unitTypeValue.ToString().ToLowerInvariant())
            {
                case "px":
                    return UnitType.Pixel;

                case "em":
                    return UnitType.Em;

                case "percentage":
                    return UnitType.Percentage;
            }
        }
        return UnitType.Pixel;
    }


    /// <summary>
    /// Register and show stamp button.
    /// </summary>
    private void RegisterAndShowStampButton()
    {
        string insertHTMLScript = ScriptHelper.GetScript(
@"
function InsertHTML(htmlString, ckClientID) 
{
    // Get the editor instance that we want to interact with.
    var oEditor = oEditor = window.CKEDITOR.instances[ckClientID];
    // Check the active editing mode.
    if (oEditor != null) 
    {
        // Check the active editing mode.
        if (oEditor.mode == 'wysiwyg') 
        {
            // Insert the desired HTML.
            oEditor.focus();
            oEditor.insertHtml(htmlString);
        }
    }
    return false;
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "InsertHTML", insertHTMLScript);

        pnlStamp.Visible = true;
    }

    #endregion
}

