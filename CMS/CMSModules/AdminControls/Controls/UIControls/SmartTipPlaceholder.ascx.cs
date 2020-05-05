using System;

using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_SmartTipPlaceholder : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the identifier of the smart tip used for storing the collapsed state. If multiple smart tips with the same
    /// identifier are created, closing one will result in closing all of them.
    /// </summary>
    public string SmartTipCollapsedStateIdentifier
    {
        get
        {
            return GetStringContextValue("SmartTipCollapsedStateIdentifier", String.Empty);
        }
        set
        {
            SetValue("SmartTipCollapsedStateIdentifier", value);
        }
    }


    /// <summary>
    /// Gets or sets the collapsed header of the smart tip. Use plain text.
    /// </summary>
    public string SmartTipCollapsedHeader
    {
        get
        {
            return GetStringContextValue("SmartTipCollapsedHeader", String.Empty);
        }
        set
        {
            SetValue("SmartTipCollapsedHeader", value);
        }
    }


    /// <summary>
    /// Gets or sets the expanded header of the smart tip. Use plain text.
    /// </summary>
    public string SmartTipExpandedHeader
    {
        get
        {
            return GetStringContextValue("SmartTipExpandedHeader", String.Empty);
        }
        set
        {
            SetValue("SmartTipExpandedHeader", value);
        }
    }


    /// <summary>
    /// Gets or sets the text content of the displayed smart tip either as simple text or HTML.
    /// </summary>
    public string SmartTipText
    {
        get
        {
            return GetStringContextValue("SmartTipText", String.Empty);
        }
        set
        {
            SetValue("SmartTipText", value);
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        smrtp.Visible = false;

        String smartTip = SmartTipText;

        if (smartTip != String.Empty)
        {
            smrtp.CollapsedStateIdentifier = SmartTipCollapsedStateIdentifier;
            smrtp.CollapsedHeader = SmartTipCollapsedHeader;
            smrtp.ExpandedHeader = SmartTipExpandedHeader;
            smrtp.Content = smartTip;
            smrtp.Visible = true;
        }
    }

    #endregion
}
