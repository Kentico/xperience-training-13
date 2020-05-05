using System;

using CMS.PortalEngine;

using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_ObjectTreeMenu : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Indicates whether use max node limit stored in settings.
    /// </summary>
    public bool UseMaxNodeLimit
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseMaxNodeLimit"), true);
        }
        set
        {
            SetValue("UseMaxNodeLimit", value);
        }
    }
    

    /// <summary>
    /// Maximum tree nodes shown under parent node - this value can be ignored if UseMaxNodeLimit set to false.
    /// </summary>
    public int MaxTreeNodes
    {
        get
        {
            return  GetIntContextValue("MaxTreeNodes", -1);
        }
        set
        {
            SetValue("MaxTreeNodes", value);
        }
    }


    /// <summary>
    /// Indicates whether tree displays clone button
    /// </summary>
    public bool DisplayCloneButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayCloneButton"), false);
        }
        set
        {
            SetValue("DisplayCloneButton", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        paneTree.Values.Add(new UILayoutValue("MaxTreeNodes", MaxTreeNodes));
        paneTree.Values.Add(new UILayoutValue("UseMaxNodeLimit", UseMaxNodeLimit));
        paneTree.Values.Add(new UILayoutValue("DisplayCloneButton", DisplayCloneButton));

        paneContentTMain.Src = ResolveUrl("~/CMSPages/Blank.aspx");

        base.OnContentLoaded();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (PortalContext.ViewMode.IsDesign(true))
        {
            paneContentTMain.RenderAs = HtmlTextWriterTag.Div;
            paneContentTMain.Size = "0";
        }

        // Handle title
        ManagePaneTitle(paneTitle, true);

        // Show dialog footer only when used in a dialog
        layoutElem.DisplayFooter = DisplayFooter;

        ScriptHelper.HideVerticalTabs(Page);
    }

    #endregion
}
