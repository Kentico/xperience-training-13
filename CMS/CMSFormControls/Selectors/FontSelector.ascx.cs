using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Selectors_FontSelector : FormEngineUserControl, ICallbackEventHandler
{
    #region "Variables"

    private string mDefaultFont = "Arial";
    private string mDefaultStyle = "Regular";
    private int mDefaultSize = 11;
    private bool mDisplayClearButton = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtFontType.Text;
        }
        set
        {
            txtFontType.Text = (string)value;
        }
    }


    /// <summary>
    /// Default font family.
    /// </summary>
    public string DefaultFont
    {
        get
        {
            return mDefaultFont;
        }
        set
        {
            mDefaultFont = value;
        }
    }


    /// <summary>
    /// Default font style.
    /// </summary>
    public string DefaultStyle
    {
        get
        {
            return mDefaultStyle;
        }
        set
        {
            mDefaultStyle = value;
        }
    }


    /// <summary>
    /// Default font size.
    /// </summary>
    public int DefaultSize
    {
        get
        {
            return mDefaultSize;
        }
        set
        {
            mDefaultSize = value;
        }
    }


    /// <summary>
    /// If true display button for clear font.
    /// </summary>
    public bool DisplayClearButton
    {
        get
        {
            return mDisplayClearButton;
        }
        set
        {
            mDisplayClearButton = value;
        }
    }


    /// <summary>
    /// ClientId of font type text box.
    /// </summary>
    public string FontTypeTextBoxClientId
    {
        get
        {
            return txtFontType.ClientID;
        }
    }

    #endregion


    #region "Control events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (hfValue.Value != String.Empty)
        {
            txtFontType.Text = hfValue.Value;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        btnChangeFontType.OnClientClick = Page.ClientScript.GetCallbackEventReference(this, "document.getElementById('" + txtFontType.ClientID + "').value", "selectFont", null) + ";return false;";
        
        btnClearFont.Visible = DisplayClearButton;

        RegisterScripts();
    }


    protected void btnClearFont_Click(object sender, EventArgs e)
    {
        // Clear value in selector
        txtFontType.Text = String.Empty;
        hfValue.Value = String.Empty;
    }

    #endregion


    #region "Methods"

    private void RegisterScripts()
    {
        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Create script for open dialog, get parameters and refresh
        string script = @" 
function selectFont(queryParams) {
    modalDialog('" + ResolveUrl("~/CMSFormControls/Selectors/FontSelectorDialog.aspx") + @"' + queryParams, 'FontSelector', 500, 470);
}
function setParameters(val,hf,tb) {
    document.getElementById(hf).value = val;
    document.getElementById(tb).value = val;
}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(String), "FontSelectorScripts", script, true);
    }


    /// <summary>
    /// Sets "onchange" javascript function to control's input.
    /// </summary>
    /// <param name="fnction"></param>
    public void SetOnChangeAttribute(string fnction)
    {
        txtFontType.Attributes["onchange"] = fnction;
    }

    #endregion


    #region "Callback handlers"

    private string callBackArg;


    public string GetCallbackResult()
    {
        // Add font parameters for selector dialog
        string value = (String.IsNullOrEmpty(callBackArg)) ? String.Format("{0};{1};{2};;", DefaultFont, DefaultStyle, DefaultSize) : callBackArg;
        WindowHelper.Add(hfValue.ClientID, value);

        return String.Format("?hiddenId={0}&fontTypeId={1}", hfValue.ClientID, txtFontType.ClientID);
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        callBackArg = eventArgument;
    }

    #endregion
}