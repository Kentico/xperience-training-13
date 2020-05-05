using System;
using System.Collections;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;
using CMS.UIControls.UniMenuConfig;


public partial class CMSAdminControls_UI_UIProfiles_UIToolbar : UIToolbar
{
    #region "Variables"

    protected string mPreselectedItem;
    protected string mElementName;
    protected string mModuleName;
    protected string mTargetFrameset;

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, target frame is not in parrent frames but iframe
    /// </summary>
    public bool UseIFrame
    {
        get
        {
            return uniMenu.UseIFrame;
        }
        set
        {
            uniMenu.UseIFrame = value;
        }
    }

    /// <summary>
    /// Gets or sets the value that indicates whether scroll panel should be used
    /// </summary>
    public bool DisableScrollPanel
    {
        get
        {
            return uniMenu.DisableScrollPanel;
        }
        set
        {
            uniMenu.DisableScrollPanel = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether specific CSS classes should be generated for each group element.
    /// </summary>
    public bool GenerateElementCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether Edit icon in dev. mode should be displayed
    /// </summary>
    public bool DisableEditIcon
    {
        get;
        set;
    }


    /// <summary>
    /// Returns the UIElementInfo representing the first button of first group displayed.
    /// </summary>
    public UIElementInfo FirstUIElement
    {
        get
        {
            return uniMenu.FirstUIElement;
        }
    }


    /// <summary>
    /// Returns the UIElementInfo representing the explicitly highlighted UI element.
    /// </summary>
    public UIElementInfo HighlightedUIElement
    {
        get
        {
            return uniMenu.HighlightedUIElement;
        }
    }


    /// <summary>
    /// Returns the UIElementInfo representing the selected (either explicitly highlighted or first) UI element.
    /// </summary>
    public UIElementInfo SelectedUIElement
    {
        get
        {
            return uniMenu.SelectedUIElement;
        }
    }


    /// <summary>
    /// Indicates whether at least one group with at least one button is rendered in the menu.
    /// </summary>
    public bool MenuEmpty
    {
        get
        {
            return uniMenu.MenuEmpty;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first item should be highlighted.
    /// </summary>
    public bool HighlightFirstItem
    {
        get
        {
            return uniMenu.HighlightFirstItem;
        }
        set
        {
            uniMenu.HighlightFirstItem = value;
        }
    }


    /// <summary>
    /// Indicates whether to remember item which was last selected and select it again.
    /// </summary>
    public bool RememberSelectedItem
    {
        get
        {
            return uniMenu.RememberSelectedItem;
        }
        set
        {
            uniMenu.RememberSelectedItem = value;
        }
    }


    /// <summary>
    /// Code name of the UI element.
    /// </summary>
    public string ElementName
    {
        get
        {
            return mElementName;
        }
        set
        {
            mElementName = value;
        }
    }


    /// <summary>
    /// Code name of the module.
    /// </summary>
    public string ModuleName
    {
        get
        {
            return mModuleName;
        }
        set
        {
            mModuleName = value;
            uniMenu.ModuleName = value;
        }
    }


    /// <summary>
    /// Target frameset in which the links are opened.
    /// </summary>
    public string TargetFrameset
    {
        get
        {
            return mTargetFrameset;
        }
        set
        {
            mTargetFrameset = value;
            uniMenu.TargetFrameset = value;
        }
    }


    /// <summary>
    /// Query parameter name for the preselection of the item.
    /// </summary>
    public string QueryParameterName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether current UI culture is RTL.
    /// </summary>
    public bool IsRTL
    {
        get
        {
            return uniMenu.IsRTL;
        }
        set
        {
            uniMenu.IsRTL = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        string localizationCulture = PortalHelper.GetUILocalizationCulture();
        uniMenu.ResourceCulture = localizationCulture;

        // Handle the pre-selection
        mPreselectedItem = QueryHelper.GetString(QueryParameterName, "");
        if (mPreselectedItem.StartsWith("cms.", StringComparison.OrdinalIgnoreCase))
        {
            mPreselectedItem = mPreselectedItem.Substring(4);
        }

        uniMenu.HighlightItem = mPreselectedItem;

        // If element name is not set, use root module element
        string elemName = ElementName;
        if (String.IsNullOrEmpty(elemName))
        {
            elemName = ModuleName.Replace(".", "");
        }

        // Get the UI elements
        DataSet ds = UIElementInfoProvider.GetChildUIElements(ModuleName, elemName);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            FilterElements(ds);

            // Prepare the list of elements
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string url = ValidationHelper.GetString(dr["ElementTargetURL"], "");
                UIElementTypeEnum type = ValidationHelper.GetString(dr["ElementType"], "").ToEnum<UIElementTypeEnum>();

                Group group = new Group();
                if (url.EndsWith("ascx", StringComparison.OrdinalIgnoreCase) && (type == UIElementTypeEnum.UserControl))
                {
                    group.ControlPath = url;
                }
                else
                {
                    group.UIElementID = ValidationHelper.GetInteger(dr["ElementID"], 0);
                }

                group.CssClass = "ContentMenuGroup";

                if (GenerateElementCssClass)
                {
                    string name = ValidationHelper.GetString(dr["ElementName"], String.Empty).Replace(".", String.Empty);
                    group.CssClass += " ContentMenuGroup" + name;
                    group.SeparatorCssClass = "UniMenuSeparator" + name;
                }

                group.Caption = ResHelper.LocalizeString(ValidationHelper.GetString(dr["ElementCaption"], ""), localizationCulture);
                if (group.Caption == String.Empty)
                {
                    group.Caption = ResHelper.LocalizeString(ValidationHelper.GetString(dr["ElementDisplayName"], ""), localizationCulture);
                }
                uniMenu.Groups.Add(group);
            }

            // Raise groups created event
            RaiseOnGroupsCreated(this, uniMenu.Groups);

            // Button created & filtered event handler
            uniMenu.OnButtonCreating += uniMenu_OnButtonCreating;
            uniMenu.OnButtonCreated += uniMenu_OnButtonCreated;
            uniMenu.OnButtonFiltered += uniMenu_OnButtonFiltered;
        }

        // Add editing icon in development mode
        if (SystemContext.DevelopmentMode && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && !DisableEditIcon)
        {
            var link = PortalUIHelper.GetResourceUIElementLink(ModuleName, ElementName);
            if (!String.IsNullOrEmpty(link))
            {
                ltlAfter.Text += $"<div class=\"UIElementsLink\" >{link}</div>";
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Select first item 
        if (!String.IsNullOrEmpty(uniMenu.StartingPage) || (HighlightFirstItem && !String.IsNullOrEmpty(TargetFrameset) && (FirstUIElement != null)))
        {
            String url = String.IsNullOrEmpty(uniMenu.StartingPage) ? UIContextHelper.GetElementUrl(FirstUIElement, UIContext) : uniMenu.StartingPage;

            // Ensure hash code if required
            url = MacroResolver.Resolve(URLHelper.EnsureHashToQueryParameters(url));

            String target = UseIFrame ? String.Format("frames['{0}']", TargetFrameset) : String.Format("parent.frames['{0}']", TargetFrameset);
            String script = String.Format("{0}.location.href = '{1}';", target, ResolveUrl(url));
            ScriptHelper.RegisterStartupScript(Page, typeof(String), "FirstItemSelection", ScriptHelper.GetScript(script));
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Filters the dataset with UI Elements according to UI Profile of current user by default and according to custom event (if defined).
    /// </summary>
    protected void FilterElements(DataSet dsElements)
    {
        // For all tables in dataset
        foreach (DataTable dt in dsElements.Tables)
        {
            ArrayList deleteRows = new ArrayList();

            // Find rows to filter out
            foreach (DataRow dr in dt.Rows)
            {
                UIElementInfo uiElement = new UIElementInfo(dr);
                bool allowed = MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement(ModuleName, uiElement.ElementName);
                allowed = allowed && RaiseOnGroupFiltered(this, new UniMenuArgs { UIElement = uiElement });

                if (!allowed || !UIContextHelper.CheckElementAvailabilityInUI(uiElement))
                {
                    deleteRows.Add(dr);
                }
            }

            // Delete the filtered rows
            foreach (DataRow dr in deleteRows)
            {
                dt.Rows.Remove(dr);
            }
        }
    }


    protected void uniMenu_OnButtonCreating(object sender, UniMenuArgs e)
    {
        RaiseOnButtonCreating(sender, e);
    }


    protected void uniMenu_OnButtonCreated(object sender, UniMenuArgs e)
    {
        RaiseOnButtonCreated(sender, e);
    }


    protected bool uniMenu_OnButtonFiltered(object sender, UniMenuArgs e)
    {
        return RaiseOnButtonFiltered(sender, e);
    }

    #endregion
}