using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;
using SubMenuItem = CMS.UIControls.UniMenuConfig.SubItem;

public partial class CMSAdminControls_UI_UniMenu_UniMenuButtons : CMSUserControl
{
    #region "Variables"

    protected int mSelectedIndex = -1;

    private int mMaximumItems = 1;

    private bool mHorizontalLayout = true;

    private List<MenuItem> mButtons;

    private StringBuilder mStartupScript;

    private bool mAllowToggleScriptRegistered;

    private Table mTabGroup;

    private string mElemsIds = "";

    private string defaultSelectedClientID;

    /// <summary>
    /// Table wrapper for buttons group
    /// </summary>
    public Table TabGroup
    {
        get
        {
            return mTabGroup ?? (mTabGroup = new Table());
        }
    }

    private Dictionary<string, string> mJsModuleData;

    #endregion


    #region "Constants"

    public const string SelectedSuffix = " Selected";

    private const string BUTTON_PANEL_SHORTID = "pb";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Determines whether buttons are selectable.
    /// </summary>
    public bool AllowSelection
    {
        get;
        set;
    }


    /// <summary>
    /// Selection Group Name.
    /// If two buttons are in the same group,  they cannot be
    /// selected both at the same time. But if they are in
    /// different groups, they can be selected both at the same time.
    /// </summary>
    public string SelectionGroupName
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether buttons are draggable.
    /// </summary>
    public bool AllowDraggable
    {
        get;
        set;
    }


    /// <summary>
    /// If selection is enabled, determines which button is implicitly selected.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            return mSelectedIndex;
        }
        set
        {
            mSelectedIndex = value;
        }
    }


    /// <summary>
    /// Indicates if function 'CheckChanges' should be called before action.
    /// </summary>
    public bool CheckChanges
    {
        get;
        set;
    }


    /// <summary>
    /// List of buttons
    /// </summary>
    public List<MenuItem> Buttons
    {
        get
        {
            return mButtons ?? (mButtons = new List<MenuItem>());
        }
        set
        {
            mButtons = value;
        }
    }


    /// <summary>
    /// Dictionary containing button's arguments for JavaScript module
    /// </summary>
    public Dictionary<string, string> JsModuleData
    {
        get
        {
            return mJsModuleData ?? (mJsModuleData = new Dictionary<string, string>());
        }
    }


    /// <summary>
    /// Description.
    /// </summary>
    public List<Panel> InnerControls
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether to repeat items horizontally. Value is true by default. Otherwise items will be rendered vertically.
    /// </summary>
    public bool HorizontalLayout
    {
        get
        {
            return mHorizontalLayout;
        }
        set
        {
            mHorizontalLayout = value;
        }
    }


    /// <summary>
    /// Specifies number of menu items to be rendered in single row or column (depending on RepeatHorizontal property).
    /// </summary>
    public int MaximumItems
    {
        get
        {
            return mMaximumItems;
        }
        set
        {
            mMaximumItems = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            if (Buttons != null)
            {
                ReloadButtons();
            }
            else
            {
                Controls.Add(GetError(GetString("unimenubuttons.wrongdimensions")));
            }
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Load buttons collection.
    /// </summary>
    public void ReloadButtons()
    {
        mStartupScript = new StringBuilder();

        AddMainTable();

        if (mStartupScript.Length > 0)
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "UniMenuStartupScript_" + ClientID, ScriptHelper.GetScript(mStartupScript.ToString()));
        }
    }


    /// <summary>
    /// Method for registering JS scripts for toggling buttons.
    /// </summary>
    private void EnsureAllowToggleScript()
    {
        if (mAllowToggleScriptRegistered)
        {
            return;
        }

        // Toggle script
        StringBuilder toggleScript = new StringBuilder();
        toggleScript.AppendLine("function ToggleButton(elem)");
        toggleScript.AppendLine("{");
        toggleScript.AppendLine("    var selected = '" + SelectedSuffix.Trim() + "';");
        toggleScript.AppendLine("    var jElem =$cmsj(elem);");
        // Get first parent table
        toggleScript.AppendLine("    jElem.toggleClass(selected);");
        toggleScript.AppendLine("}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "toggleScript", ScriptHelper.GetScript(toggleScript.ToString()));
        mAllowToggleScriptRegistered = true;
    }


    /// <summary>
    /// Method for formatting draggable handler definition.
    /// </summary>
    /// <param name="html">html element of helper</param>
    /// <returns>clone if default</returns>
    private string GetDraggableHandler(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return "clone";
        }
        return "function(){ return $cmsj(\"" + html + "\"); }";
    }


    /// <summary>
    /// Method for adding button to controls.
    /// </summary>
    /// <param name="outerPanel">Panel to be added to</param>
    /// <param name="identifier">Index of button</param>
    private void AddButtonTo(CMSPanel outerPanel, int identifier)
    {
        MenuItem button = Buttons[identifier];
        string caption = button.Text;
        string tooltip = button.Tooltip;
        string cssClass = button.CssClass;
        string onClick = button.OnClientClick;
        string redirectUrl = button.RedirectUrl;
        string imagePath = button.ImagePath;
        string iconClass = button.IconClass;
        string alt = button.ImageAltText;
        int minWidth = button.MinimalWidth;
        ImageAlign imageAlign = GetImageAlign(button.ImageAlign);

        // Generate button image
        WebControl image = null;
        if (!string.IsNullOrEmpty(imagePath))
        {
            var buttonImage = new Image
            {
                ID = "img" + identifier,
                CssClass = "MenuButtonImage",
                EnableViewState = false,
                AlternateText = alt ?? caption,
                ImageAlign = imageAlign,
                ImageUrl = ResolveUrl(imagePath)
            };
            image = buttonImage;
        }
        else if (!string.IsNullOrEmpty(iconClass))
        {
            var icon = new CMSIcon
            {
                ID = "ico" + identifier,
                EnableViewState = false,
                ToolTip = tooltip,
                CssClass = "cms-icon-80 " + iconClass
            };
            image = icon;
        }

        // Generate button text
        Literal captionLiteral = new Literal();
        captionLiteral.ID = "ltlCaption" + identifier;
        captionLiteral.EnableViewState = false;
        string separator = (imageAlign == ImageAlign.Top) ? "<br />" : "\n";
        captionLiteral.Text = String.Format("{0}<span class=\"MenuButtonText\">{1}</span>", separator, caption);

        CMSPanel pnlSubItems = null;
        if (button.SubItems.Count > 0)
        {
            // Ensure jQuery tool is loaded
            ScriptHelper.RegisterJQueryTools(Page);

            // Register support script
            ScriptHelper.RegisterScriptFile(Page, "~/CMSAdminControls/UI/UniMenu/UniMenu.js");

            // Append sub-menu toggle script
            onClick = "CMSUniMenu.ToogleSubMenu(this);" + onClick;

            // Append arrow down image for indication sub menu
            captionLiteral.Text += @"<i aria-hidden=""true"" class=""icon-caret-right-down cms-icon-30""></i>";

            // Create submenu
            pnlSubItems = CreateSubMenu(button, cssClass, identifier);
        }

        // Generate button link
        HyperLink buttonLink = new HyperLink();
        buttonLink.ID = "btn" + identifier;
        buttonLink.EnableViewState = false;

        if (image != null)
        {
            buttonLink.Controls.Add(image);
        }
        buttonLink.Controls.Add(captionLiteral);

        if (!string.IsNullOrEmpty(redirectUrl))
        {
            buttonLink.NavigateUrl = ResolveUrl(redirectUrl);
        }

        // Generate left border
        CMSPanel pnlLeft = new CMSPanel();
        pnlLeft.ID = "pnlLeft" + identifier;
        pnlLeft.ShortID = "pl" + identifier;

        pnlLeft.EnableViewState = false;
        pnlLeft.CssClass = "Left" + cssClass;

        // Generate middle part of button
        CMSPanel pnlMiddle = new CMSPanel();
        pnlMiddle.ID = "pnlMiddle" + identifier;
        pnlMiddle.ShortID = "pm" + identifier;

        pnlMiddle.EnableViewState = false;
        pnlMiddle.CssClass = "Middle" + cssClass;
        pnlMiddle.Controls.Add(buttonLink);
        if (minWidth > 0)
        {
            pnlMiddle.Style.Add("min-width", minWidth + "px");

            // IE7 issue with min-width
            CMSPanel pnlMiddleTmp = new CMSPanel();
            pnlMiddleTmp.EnableViewState = false;
            pnlMiddleTmp.Style.Add("width", minWidth + "px");
            pnlMiddle.Controls.Add(pnlMiddleTmp);
        }

        // Add sub items if exists
        if (pnlSubItems != null)
        {
            pnlMiddle.Controls.Add(pnlSubItems);
        }

        // Generate right border
        CMSPanel pnlRight = new CMSPanel();
        pnlRight.ID = "pnlRight" + identifier;
        pnlRight.ShortID = "pr" + identifier;

        pnlRight.EnableViewState = false;
        pnlRight.CssClass = "Right" + cssClass;

        // Generate whole button
        CMSPanel pnlButton = new CMSPanel();
        pnlButton.ID = "pnlButton" + identifier;
        pnlButton.ShortID = BUTTON_PANEL_SHORTID + identifier;

        // Propagate attributes to panel for JavaScript use
        foreach (var entry in button.Attributes)
        {
            pnlButton.Attributes.Add(entry.Key, entry.Value);
        }

        pnlButton.EnableViewState = false;

        if (button.AllowToggle && button.IsToggled)
        {
            cssClass += SelectedSuffix;
        }

        pnlButton.CssClass = cssClass;

        if (button.AllowToggle)
        {
            pnlButton.CssClass += " Toggle";
            EnsureAllowToggleScript();
        }

        //Generate button table (IE7 issue)
        Table tabButton = new Table();
        TableRow tabRow = new TableRow();
        TableCell tabCellLeft = new TableCell();
        TableCell tabCellMiddle = new TableCell();
        TableCell tabCellRight = new TableCell();

        tabButton.CellPadding = 0;
        tabButton.CellSpacing = 0;

        tabButton.Rows.Add(tabRow);
        tabRow.Cells.Add(tabCellLeft);
        tabRow.Cells.Add(tabCellMiddle);
        tabRow.Cells.Add(tabCellRight);

        // Add inner controls
        tabCellLeft.Controls.Add(pnlLeft);
        tabCellMiddle.Controls.Add(pnlMiddle);
        tabCellRight.Controls.Add(pnlRight);

        pnlButton.Controls.Add(tabButton);

        pnlButton.ToolTip = tooltip ?? caption;

        outerPanel.Controls.Add(pnlButton);

        if (AllowDraggable)
        {
            mStartupScript.Append(String.Format("$cmsj( '#{0}' ).draggable({{ helper:{1}, scope:'{2}' }});", pnlButton.ClientID, GetDraggableHandler(button.DraggableTemplateHandler), button.DraggableScope));
        }

        if (!AllowSelection && button.AllowToggle)
        {
            onClick = "ToggleButton(this);" + onClick;
        }

        pnlButton.Attributes["onclick"] = CheckChanges ? "if (CheckChanges()) {" + onClick + "}" : onClick;

        // In case of horizontal layout
        if (HorizontalLayout)
        {
            // Stack buttons underneath
            pnlButton.Style.Add("clear", "both");
        }
        else
        {
            // Stack buttons side-by-side
            pnlButton.Style.Add("float", "left");
        }

        if (button.IsSelectable)
        {
            // Collect panel client IDs for JavaScript
            mElemsIds += "#" + pnlButton.ClientID + ",";
            if (AllowSelection && (identifier == SelectedIndex))
            {
                // Button should be selected by default, remember its ClientID for JavaScript
                defaultSelectedClientID = "#" + pnlButton.ClientID;
            }
        }
    }


    /// <summary>
    /// Method for adding submenu.
    /// </summary>
    /// <param name="button">Menu item button</param>
    /// <param name="cssClass">Button css class</param>
    /// <param name="identifier">Button identifier</param>
    private CMSPanel CreateSubMenu(MenuItem button, string cssClass, int identifier)
    {
        // Generate sub items container
        var pnlSubItems = new CMSPanel();
        pnlSubItems.ID = "pnlSubItems" + identifier;
        pnlSubItems.ShortID = "ps" + identifier;
        pnlSubItems.CssClass = "SubMenuItems ContextMenu";

        // Forward scroll button
        Panel pnlForward = new Panel
        {
            ID = "pnlForward",
            CssClass = "ForwardScroller"
        };

        // Backward scroll button
        Panel pnlBackward = new Panel
        {
            ID = "pnlBackward",
            CssClass = "BackwardScroller"
        };

        // Scrollable area
        ScrollPanel pnlMenu = new ScrollPanel
        {
            ID = "pnlCultureList",
            ShortID = "pcl" + identifier,
            CssClass = "ContextMenuContainer",
            ScrollAreaCssClass = "PortalContextMenu UniMenuContextMenu",
            Layout = RepeatDirection.Vertical,
            InnerItemClass = "Item",
            ForwardScrollerControlID = pnlForward.ID,
            BackwardScrollerControlID = pnlBackward.ID,
            ScrollStep = 200
        };

        pnlSubItems.Controls.Add(pnlForward);
        pnlSubItems.Controls.Add(pnlBackward);
        pnlSubItems.Controls.Add(pnlMenu);

        // Add menu items
        int subIdentifier = 0;
        foreach (SubMenuItem item in button.SubItems)
        {
            AddSubMenuItem(pnlMenu, item, cssClass, subIdentifier);
            subIdentifier++;
        }

        return pnlSubItems;
    }


    /// <summary>
    /// Method for adding sub menu item into menu.
    /// </summary>
    /// <param name="pnlMenu">Menu container control</param>
    /// <param name="item">Sub menu item definition</param>
    /// <param name="cssClass">Button css class</param>
    /// <param name="subIdentifier">Button identifier</param>
    private void AddSubMenuItem(CMSPanel pnlMenu, SubMenuItem item, string cssClass, int subIdentifier)
    {
        if (String.IsNullOrEmpty(item.ControlPath))
        {
            // Generate sub item
            CMSPanel pnlItem = new CMSPanel();
            pnlItem.ID = "pnlSubItem" + subIdentifier;
            pnlItem.ShortID = "pi" + subIdentifier;
            pnlItem.CssClass = "Item ItemPadding";
            pnlItem.ToolTip = item.Tooltip;

            if (!String.IsNullOrEmpty(item.OnClientClick))
            {
                pnlItem.Attributes["onclick"] = item.OnClientClick.Replace("##BUTTON##", "$cmsj(this).parents('." + cssClass + "').get(0)");
            }

            if (!String.IsNullOrEmpty(item.ImagePath))
            {
                string altText = String.IsNullOrEmpty(item.ImageAltText) ? item.Text : item.ImageAltText;

                // Generate sub items text
                CMSPanel pnlIcon = new CMSPanel();
                pnlIcon.ID = "pnlIcon";
                pnlIcon.ShortID = "pii" + subIdentifier;
                pnlIcon.CssClass = "Icon";
                Literal subItemImage = new Literal();
                subItemImage.ID = "ltlSubItemImage" + subIdentifier;
                subItemImage.EnableViewState = false;
                subItemImage.Text = String.Format("<img src=\"{0}\" alt=\"{1}\" />&nbsp;",
                        ResolveUrl(item.ImagePath),
                        GetString(altText));

                pnlIcon.Controls.Add(subItemImage);
                pnlItem.Controls.Add(pnlIcon);
            }

            // Add custom HTML attributes
            foreach (var htmlAttribute in item.HtmlAttributes)
            {
                pnlItem.Attributes[htmlAttribute.Key] = HTMLHelper.HTMLEncode(htmlAttribute.Value);
            }

            // Generate sub items text
            CMSPanel pnlText = new CMSPanel();
            pnlText.ID = "pnlText";
            pnlText.ShortID = "pt" + subIdentifier;
            pnlText.CssClass = "Name";
            Label subItemText = new Label();
            subItemText.ID = "ltlSubItem" + subIdentifier;
            subItemText.EnableViewState = false;
            subItemText.Text = GetString(item.Text);

            pnlText.Controls.Add(subItemText);
            pnlItem.Controls.Add(pnlText);

            if (!String.IsNullOrEmpty(item.RightImagePath))
            {
                string altText = String.IsNullOrEmpty(item.RightImageAltText) ? item.Text : item.RightImageAltText;

                // Generate sub item right icon
                Literal subRightItemImage = new Literal();
                subRightItemImage.ID = "ltlSubRightItemImage" + subIdentifier;
                subRightItemImage.EnableViewState = false;
                subRightItemImage.Text = String.Format("<img class=\"RightIcon\" src=\"{0}\" alt=\"{1}\" />&nbsp;",
                        ResolveUrl(item.RightImagePath),
                        GetString(altText));

                CMSPanel pnlRightIcon = CreateRightIconPanel(subIdentifier);
                pnlRightIcon.Controls.Add(subRightItemImage);
                pnlItem.Controls.Add(pnlRightIcon);
            }
            else if (!string.IsNullOrEmpty(item.RightImageIconClass))
            {
                string altText = String.IsNullOrEmpty(item.RightImageAltText) ? item.Text : item.RightImageAltText;

                var icon = new CMSIcon
                {
                    ID = "ico" + subIdentifier,
                    EnableViewState = false,
                    ToolTip = GetString(altText),
                    CssClass = "cms-icon-80 " + item.RightImageIconClass
                };

                CMSPanel pnlRightIcon = CreateRightIconPanel(subIdentifier);
                pnlRightIcon.Controls.Add(icon);
                pnlItem.Controls.Add(pnlRightIcon);
            }

            pnlMenu.Controls.Add(pnlItem);
        }
        else
        {
            // Load menu item control
            Control ctrl = LoadUserControl(item.ControlPath);
            if (ctrl != null)
            {
                pnlMenu.Controls.Add(ctrl);
            }
        }
    }


    /// <summary>
    /// Geterates CMSPanel for right icon
    /// </summary>
    /// <param name="identifierId">Identifier id</param>
    private CMSPanel CreateRightIconPanel(int identifierId)
    {
        CMSPanel pnlRightIcon = new CMSPanel();
        pnlRightIcon.ID = "pnlRightIcon";
        pnlRightIcon.ShortID = "pri" + identifierId;
        pnlRightIcon.CssClass = "Icon IconRight";
        return pnlRightIcon;
    }


    /// <summary>
    /// Method for adding panel to group cell.
    /// </summary>
    /// <param name="tabGroupCell">Table cell to be added to</param>
    /// <param name="panelIndex">Index of panel to be added</param>
    private void AddPanelTo(TableCell tabGroupCell, int panelIndex)
    {
        int startIndex = (panelIndex * MaximumItems);

        // Create new instance of panel
        CMSPanel outerPanel = new CMSPanel();
        outerPanel.EnableViewState = false;
        outerPanel.ID = "pnlOuter" + panelIndex;
        outerPanel.ShortID = "po" + panelIndex;

        // In case of horizontal layout
        if (HorizontalLayout)
        {
            // Stack panels side-by-side
            outerPanel.Style.Add("float", CultureHelper.IsUICultureRTL() ? "right" : "left");
        }
        else
        {
            // Stack panels underneath
            outerPanel.Style.Add("clear", "both");
        }

        // Add panel to collection of panels
        tabGroupCell.Controls.Add(outerPanel);

        // Add buttons to panel
        for (int buttonIndex = startIndex; buttonIndex < startIndex + MaximumItems; buttonIndex++)
        {
            if (Buttons.Count > buttonIndex)
            {
                AddButtonTo(outerPanel, buttonIndex);
            }
        }
    }


    /// <summary>
    /// Method leading the menu construction.
    /// </summary>
    private void AddMainTable()
    {
        int panelCount = Buttons.Count / MaximumItems;
        if ((Buttons.Count % MaximumItems) > 0)
        {
            panelCount++;
        }

        TabGroup.CellPadding = 0;
        TabGroup.CellSpacing = 0;
        TableRow tabGroupRow = null;

        Controls.Clear();
        Controls.Add(TabGroup);

        // Add all panels to control
        for (int i = 0; i < panelCount; i++)
        {
            if (!HorizontalLayout || i == 0)
            {
                tabGroupRow = new TableRow();
                TabGroup.Rows.Add(tabGroupRow);
            }

            TableCell tabGroupCell = new TableCell();
            tabGroupCell.VerticalAlign = VerticalAlign.Top;
            if (tabGroupRow != null)
            {
                tabGroupRow.Cells.Add(tabGroupCell);
            }

            AddPanelTo(tabGroupCell, i);
        }

        // Add params for JavaScript module
        JsModuleData.Add("elemsSelector", mElemsIds);
        if (!String.IsNullOrEmpty(defaultSelectedClientID))
        {
            JsModuleData.Add("defaultSelection", defaultSelectedClientID);
        }
    }


    /// <summary>
    /// Corrects image align for RTL cultures.
    /// </summary>
    /// <param name="align">Value to parse</param>
    /// <returns>Align of an image</returns>
    private static ImageAlign GetImageAlign(ImageAlign align)
    {
        // RTL switch
        if (CultureHelper.IsUICultureRTL())
        {
            if (align == ImageAlign.Left)
            {
                align = ImageAlign.Right;
            }
            else if (align == ImageAlign.Right)
            {
                align = ImageAlign.Left;
            }
        }

        return align;
    }


    /// <summary>
    /// Generates error label.
    /// </summary>
    /// <param name="message">Error message to display</param>
    /// <returns>Label with error message</returns>
    protected Label GetError(string message)
    {
        // If error occurs skip this group
        Label errorLabel = new Label();
        errorLabel.ID = "lblError";
        errorLabel.EnableViewState = false;
        errorLabel.Text = message;
        errorLabel.CssClass = "ErrorLabel";
        return errorLabel;
    }

    #endregion
}
