using System;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_ContentNavigationPanel : CMSUserControl
{
    #region "Properties"

    // Gets node ID
    private int NodeID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("NodeID"), 0);
        }
    }


    /// <summary>
    /// Gets culture code
    /// </summary>
    private string Culture
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Culture"), string.Empty);
        }
    }


    /// <summary>
    /// Gets expended node ID
    /// </summary>
    private int ExpandNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ExpandNodeID"), 0);
        }
    }


    public string SelectedMode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedMode"), null);
        }
        set
        {
            SetValue("SelectedMode", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        // Set pane's properties
        menu.Values.AddRange(new[] { new UILayoutValue("SelectedMode", SelectedMode) });
        tree.Values.AddRange(new[] { new UILayoutValue("NodeID", NodeID), new UILayoutValue("ExpandNodeID", ExpandNodeID), new UILayoutValue("Culture", Culture) });

        // Store selected culture in UIContext
        UIContext[UIContextDataItemName.SELECTEDCULTURE] = Culture;

        // Show pane only for multilingual site
        language.Visible = SiteContext.CurrentSite.HasMultipleCultures;
    }

    #endregion
}
