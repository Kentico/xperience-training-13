using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_General_javascript : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the inline JavaScript code.
    /// </summary>
    public string InlineScript
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InlineScript"), string.Empty);
        }
        set
        {
            SetValue("InlineScript", value);
        }
    }


    /// <summary>
    /// Gets or sets the inline JavaScript code page location.
    /// </summary>
    public string InlineScriptPageLocation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InlineScriptPageLocation"), string.Empty);
        }
        set
        {
            SetValue("InlineScriptPageLocation", value);
        }
    }


    /// <summary>
    /// Indicates whether the script tags are generated or not.
    /// </summary>
    public bool GenerateScriptTags
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("GenerateScriptTags"), true);
        }
        set
        {
            SetValue("GenerateScriptTags", value);
        }
    }


    /// <summary>
    /// Gets or sets the linked file url.
    /// </summary>
    public string LinkedFile
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkedFile"), string.Empty);
        }
        set
        {
            SetValue("LinkedFile", value);
        }
    }


    /// <summary>
    /// Gets or sets the linked file url page location.
    /// </summary>
    public string LinkedFilePageLocation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkedFilePageLocation"), string.Empty);
        }
        set
        {
            SetValue("LinkedFilePageLocation", value);
        }
    }

    #endregion


    #region "Methods"


    /// <summary>
    /// Registers the control scripts
    /// </summary>
    protected void RegisterScripts()
    {
        // Include javascript only in live site or preview mode
        ViewModeEnum viewMode = PortalContext.ViewMode;
        if (viewMode != ViewModeEnum.Design)
        {
            RegisterLinkedFiles();
            RegisterInlineScript();
        }
    }


    /// <summary>
    /// Registers the inline script
    /// </summary>
    private void RegisterInlineScript()
    {
        // Render the inline script
        if (InlineScript.Trim() != string.Empty)
        {
            string inlineScript = InlineScript;

            // Check if script tags must be generated
            if (GenerateScriptTags && (InlineScriptPageLocation.ToLowerCSafe() != "submit"))
            {
                inlineScript = ScriptHelper.GetScript(InlineScript);
            }

            // Switch for script position on the page
            switch (InlineScriptPageLocation.ToLowerCSafe())
            {
                case "header":
                    Page.Header.Controls.Add(new LiteralControl(inlineScript));
                    break;

                case "beginning":
                    ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), ClientID + "inlinescript", inlineScript);
                    break;

                case "startup":
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), ClientID + "inlinescript", inlineScript);
                    break;

                case "submit":
                    ScriptHelper.RegisterOnSubmitStatement(Page, typeof(string), ClientID + "inlinescript", inlineScript);
                    break;

                default:
                    ltlInlineScript.Text = inlineScript;
                    break;
            }
        }
    }


    /// <summary>
    /// Registers the linked files
    /// </summary>
    private void RegisterLinkedFiles()
    {
        string linkedFile = LinkedFile;

        if (String.IsNullOrWhiteSpace(linkedFile))
        {
            return;
        }

        string scriptTag = ScriptHelper.GetScriptTag(linkedFile);
        if (String.IsNullOrEmpty(scriptTag))
        {
            return;
        }

        switch (LinkedFilePageLocation.ToLowerCSafe())
        {
            case "beginning":
                ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), ClientID + "script", scriptTag);
                break;

            case "startup":
                ScriptHelper.RegisterStartupScript(Page, typeof(string), ClientID + "script", scriptTag);
                break;

            default:
                // Default location is page header
                string key = ScriptHelper.SCRIPTFILE_PREFIX_KEY + linkedFile;
                if (ScriptHelper.RequestScriptRegistration(key))
                { 
                    Page.Header.Controls.Add(new LiteralControl(scriptTag));
                }
                break;
        }
    }
    

    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            RegisterScripts();
        }
    }

    #endregion
}
