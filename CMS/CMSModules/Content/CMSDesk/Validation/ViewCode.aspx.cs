using System;
using System.Web;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


[Title("validation.viewcodetooltip")]
public partial class CMSModules_Content_CMSDesk_Validation_ViewCode : CMSPage, IPostBackEventHandler
{
    #region "Properties"

    /// <summary>
    /// Overriding message placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash"))
        {
            RedirectToAccessDenied(GetString("dialogs.badhashtitle"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string url = HttpUtility.UrlDecode(QueryHelper.GetString("url", null));
        string format = QueryHelper.GetString("format", null);

        string message = HttpUtility.UrlDecode(QueryHelper.GetString("message", null));
        if (!String.IsNullOrEmpty(message))
        {
            ShowError(message);
        }

        txtCodeText.HighlightMacros = false;
        txtCodeText.AllowFullscreen = false;

        if (!String.IsNullOrEmpty(format))
        {
            txtCodeText.Language = LanguageCode.GetLanguageEnumFromString(format);
            SetTitle(GetString("validation.css.viewcodetooltip"));
        }

        InitializeScripts(url);
    }


    /// <summary>
    /// Initializes the validation scripts
    /// </summary>
    /// <param name="url">URL of the page</param>
    private void InitializeScripts(string url)
    {
        ScriptHelper.RegisterScriptFile(Page, "Validation.js");
        ScriptHelper.RegisterJQuery(Page);

        // Disable minification on URL
        if (txtCodeText.Language == LanguageEnum.CSS)
        {
            url = DocumentValidationHelper.DisableMinificationOnUrl(url);
        }

        RegisterModalPageScripts();

        string script = @"
function ResizeCodeArea() {
    var height = $cmsj(""#divContent"").height();
    $cmsj(""#" + txtCodeText.ClientID + @""").parent().css(""height"", height - 20 + ""px"");
    $cmsj("".js-code-mirror-scroll"").css(""height"", height - 52 + ""px"");
}

$cmsj(window).resize(function(){ResizeCodeArea()});
$cmsj(document).ready(function(){setTimeout(""ResizeCodeArea()"",300);" + ((!RequestHelper.IsPostBack() && !String.IsNullOrEmpty(url)) ? "LoadHTMLToElement('" + hdnHTML.ClientID + "','" + url + "');" + ControlsHelper.GetPostBackEventReference(this, null) + ";" : "") + @"});$cmsj(""#divContent"").css(""overflow"", ""hidden"");
";

        ScriptManager managaer = ScriptManager.GetCurrent(Page);
        managaer.RegisterAsyncPostBackControl(this);

        // Register script for resizing and scroll bar remove
        ScriptHelper.RegisterStartupScript(this, typeof(string), "AreaResizeAndScrollBarRemover", ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Initializes the default editor position
    /// </summary>
    private void InitPosition()
    {
        // Set the initial position
        int line = QueryHelper.GetInteger("line", 0);
        int ch = QueryHelper.GetInteger("ch", 0);

        if ((line > 0) || (ch > 0))
        {
            txtCodeText.SetPosition(true, line - 1, ch - 1);
            pnlCode.CssClass = "ValidationCode";
        }
    }

    #endregion


    #region "IPostBackEventHandler Members"

    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
        txtCodeText.Text = DocumentValidator.RemoveVirtualContextDataFromUrls(ValidationHelper.Base64Decode(hdnHTML.Value), CultureCode);
        if (txtCodeText.Language == LanguageEnum.CSS)
        {
            txtCodeText.Text = txtCodeText.Text.Trim(new[] { '\r', '\n' });
        }

        InitPosition();

        hdnHTML.Value = "";
    }

    #endregion
}
