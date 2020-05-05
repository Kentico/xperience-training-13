using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


/// <summary>
/// This form control is used in layout UI to edit styles for specified layout.
/// </summary>
public partial class CMSFormControls_Layouts_CSSStylesEditor : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the CSS code.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtLayoutCSS.Text;
        }
        set
        {
            txtLayoutCSS.Text = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Returns ExtendedArea object for code editing.
    /// </summary>
    public ExtendedTextArea Editor
    {
        get
        {
            return txtLayoutCSS;
        }
    }


    /// <summary>
    /// Returns button for adding CSS styles.
    /// </summary>
    public LocalizedButton Button
    {
        get
        {
            return btnStyles;
        }
    }


    /// <summary>
    /// If true, styles for TwoColumn UIForm mode are set.
    /// </summary>
    public bool TwoColumnMode
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Enabled)
        {
            string script =
    @"function ShowCSS_" + ClientID + @"() {
    document.getElementById('" + pnlEditCSS.ClientID + @"').style.display = 'block'; 
    document.getElementById('" + pnlLink.ClientID + @"').style.display = 'none'; 
}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ShowEditCSS_" + ClientID, script, true);

            btnStyles.OnClientClick = "ShowCSS_" + ClientID + "(); return false;";
        }

        if (!TwoColumnMode)
        {
            pnlWrapper.CssClass = "LayoutCSSControl";
        }

        plcCssLink.Visible = string.IsNullOrEmpty(txtLayoutCSS.Text);
        if (plcCssLink.Visible)
        {
            pnlEditCSS.Attributes.CssStyle.Add("display", "none");
        }
        else
        {
            pnlEditCSS.Attributes.CssStyle.Remove("display");
        }
    }

    #endregion
}