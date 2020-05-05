using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Preview_PreviewNavigationButtons : CMSPreviewControl
{
    #region "Properties"

    /// <summary>
    /// If true, preview controls values are loaded from cache (even when postback)
    /// </summary>
    public bool SetControls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SetControls"), false);
        }
    }


    /// <summary>
    /// Indicates whether show panel separator (used in vertical mode)
    /// </summary>
    public bool ShowPanelSeparator
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPanelSeparator"), false);
        }
    }


    /// <summary>
    /// Gets or sets the key under which the preview state is stored in the cookies
    /// </summary>
    public String CookiesPreviewStateName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CookiesPreviewStateName"), String.Empty);
        }
        set
        {
            SetValue("CookiesPreviewStateName", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);

        ucButtons.PreviewURLSuffix = PreviewURLSuffix;
        ucButtons.SetControls = SetControls;
        ucButtons.PreviewObjectName = PreviewObjectName;
        ucButtons.DefaultAliasPath = DefaultAliasPath;
        ucButtons.DialogMode = DialogMode;
        ucButtons.PreviewInitialized = PreviewInitialized;
        ucButtons.IgnoreSessionValues = IgnoreSessionValues;
        ucButtons.DefaultPreviewPath = DefaultPreviewPath;

        int previewValue = GetPreviewStateFromCookies(CookiesPreviewStateName);

        if (previewValue == 2)
        {
            pnlMain.CssClass = pnlMain.CssClass.Replace("Vertical", "Horizontal");
        }

        buttons.Actions.Add(new CMSButtonGroupAction { Name = "close", UseIconButton = true, OnClientClick = "performToolbarAction('split');return false;", IconCssClass = "icon-l-list-titles", ToolTip = GetString("splitmode.closelayout") });
        buttons.Actions.Add(new CMSButtonGroupAction { Name = "vertical", UseIconButton = true, OnClientClick = "performToolbarAction('vertical');return false;", IconCssClass = "icon-l-cols-2", ToolTip = GetString("splitmode.verticallayout") });
        buttons.Actions.Add(new CMSButtonGroupAction { Name = "horizontal", UseIconButton = true, OnClientClick = "performToolbarAction('horizontal');return false;", IconCssClass = "icon-l-rows-2", ToolTip = GetString("splitmode.horizontallayout") });

        switch (previewValue)
        {
            case 1:
                buttons.SelectedActionName = "vertical";
                break;

            case 2:
                buttons.SelectedActionName = "horizontal";
                break;

            default:
                buttons.SelectedActionName = "close";
                break;
        }

        pnlHeaderSeparator.Visible = ShowPanelSeparator;
    }

    #endregion
}
