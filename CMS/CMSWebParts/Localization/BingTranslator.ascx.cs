using System.Text;
using System.Web;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_Localization_BingTranslator : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Webpart no script text.
    /// </summary>
    public string NoScriptText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NoScriptText"), "");
        }
        set
        {
            SetValue("NoScriptText", value);
        }
    }


    /// <summary>
    /// Translation mode.
    /// </summary>
    public string TranslationMode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TranslationMode"), "manual");
        }
        set
        {
            SetValue("TranslationMode", value);
        }
    }


    /// <summary>
    /// Indicates, whether the web part will be invisible or not.
    /// </summary>
    public bool DisplayTranslationDialog
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayTranslationDialog"), true);
        }
        set
        {
            SetValue("DisplayTranslationDialog", value);
        }
    }


    /// <summary>
    /// Web part border color.
    /// </summary>
    public string BorderColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BorderColor"), "black");
        }
        set
        {
            SetValue("BorderColor", value);
        }
    }


    /// <summary>
    /// Web part background color.
    /// </summary>
    public string BackgroundColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BackgroundColor"), "gray");
        }
        set
        {
            SetValue("BackgroundColor", value);
        }
    }


    /// <summary>
    /// Web part width.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 200);
        }
        set
        {
            SetValue("Width", value);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Loads the web part content.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Sets up the control.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (PortalContext.ViewMode.IsOneOf(ViewModeEnum.LiveSite, ViewModeEnum.Preview))
            {
                string culture = CultureHelper.GetShortCultureCode(DocumentContext.CurrentDocumentCulture.CultureCode);

                string src = "http://www.microsofttranslator.com/Ajax/V2/Widget.aspx";
                string query = "";
                string display = "";
                string borderColor = "";
                string backgroundColor = "";
                int width = 200;

                if (BorderColor != "")
                {
                    borderColor = "border-color: " + BorderColor + "; ";
                }

                if (BackgroundColor != "")
                {
                    backgroundColor = "background-color: " + BackgroundColor + "; ";
                }

                if (Width > 0)
                {
                    width = Width;
                }

                // If widget is not visible, translation mode auto is set
                if (!DisplayTranslationDialog && ((TranslationMode == "auto") || (TranslationMode == "notify")))
                {
                    query = URLHelper.AddUrlParameter(query, "mode", TranslationMode);
                    query = URLHelper.AddUrlParameter(query, "widget", "none");
                    display = "display: none;";
                }
                else
                {
                    query = URLHelper.AddUrlParameter(query, "mode", TranslationMode);
                }
                query = URLHelper.AddUrlParameter(query, "from", culture);
                query = URLHelper.AddUrlParameter(query, "layout", "ts");

                // Add query parameters to the url
                src = ScriptHelper.GetString(URLHelper.AppendQuery(src, query), false);

                string url = HttpUtility.UrlEncode(URLHelper.GetFullApplicationUrl());
                ScriptHelper.RegisterStartupScript(Page, typeof(string), "BingTranslatorScript", ScriptHelper.GetScript("setTimeout(function() { var s = document.createElement(\"script\"); s.type = \"text/javascript\"; s.charset = \"UTF-8\"; s.src = \"" + src + "\"; var p = document.getElementsByTagName('head')[0] || document.documentElement; p.insertBefore(s, p.firstChild); }, 0);"));
                StringBuilder builder = new StringBuilder();

                builder.Append("<div id=\"MicrosoftTranslatorWidget\" style=\"width: " + width + "px; min-height: 83px; " + borderColor + backgroundColor + display + "\">");
                builder.Append("<noscript><a href=\"http://www.microsofttranslator.com/bv.aspx?a=" + url + "\">");
                if (NoScriptText != "")
                {
                    builder.Append(NoScriptText);
                }
                else
                {
                    builder.Append(ResHelper.GetString("BingTranslator.TranslateThisPage"));
                }
                builder.Append("</a><br />Using technology <a href=\"http://www.microsofttranslator.com\">Microsoft® Translator</a></noscript></div>");

                ltlBingTranslator.Text = builder.ToString();
            }
        }
    }

    #endregion
}
