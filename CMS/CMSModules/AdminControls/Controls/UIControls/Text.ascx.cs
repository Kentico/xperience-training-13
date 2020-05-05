using System;

using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_Text : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the text to be displayed.
    /// </summary>
    public string Text
    {
        get
        {
            return GetStringContextValue("Text", null);
        }
        set
        {
            SetValue("Text", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        // Init dynamically changed properties (when defined custom properties for the UI element)
        Text = Text;

        // Encode the text
        ltlText.Text = Text;

        // Hide control for empty text
        Visible = !String.IsNullOrEmpty(Text);
    }


    /// <summary>
    /// Prerender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        ReloadData();
        base.OnPreRender(e);
    }

    #endregion
}
