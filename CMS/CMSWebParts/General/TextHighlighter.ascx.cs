using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_General_TextHighlighter : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the class name of span which wrap the highlighted words.
    /// </summary>
    public string CSSClassName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CSSClassName"), string.Empty);
        }
        set
        {
            SetValue("CSSClassName", value);
        }
    }


    /// <summary>
    /// Gets or sets the text or regular expression which should be highlighted.
    /// </summary>
    public string TextToHighlight
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TextToHighlight"), string.Empty);
        }
        set
        {
            SetValue("TextToHighlight", value);
        }
    }


    /// <summary>
    /// Indicates whether the text to highlight is regular expression or not.
    /// </summary>
    public bool TextIsRegularExpression
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("TextIsRegularExpression"), false);
        }
        set
        {
            SetValue("TextIsRegularExpression", value);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (PortalContext.ViewMode.IsOneOf(ViewModeEnum.LiveSite, ViewModeEnum.Preview))
            {
                ScriptHelper.RegisterJQueryHighLighter(Page);
                ScriptHelper.RegisterStartupScript(Page, typeof(String), "highlighter" + ClientID, ScriptHelper.GetScript("$cmsj(function(){$cmsj('body').highlight(" + ScriptHelper.GetString(TextToHighlight) + ", " + ScriptHelper.GetString(CSSClassName) + ", " + ScriptHelper.GetString(TextIsRegularExpression.ToString()) + ")})"));
            }
        }
    }

    #endregion
}
