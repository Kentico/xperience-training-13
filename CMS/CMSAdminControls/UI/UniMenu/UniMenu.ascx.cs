using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;
using CMS.UIControls.UniMenuConfig;

public partial class CMSAdminControls_UI_UniMenu_UniMenu : CMSUserControl
{
    #region "Variables"

    private bool? mIsRTL = null;
    private int identifier = 0;
    private bool mShowErrors = true;
    private bool mMenuEmpty = true;

    private UIElementInfo mFirstUIElement = null;
    private UIElementInfo mHighlightedUIElement = null;

    private Panel firstPanel = null;
    private Panel preselectedPanel = null;
    private List<Group> mGroups = null;
    private bool mHorizontalLayout = true;
    private bool mUseIFrame = false;
    private string mLocalizationCulture;

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, target frame is not in parrent frames but iframe
    /// </summary>
    public bool UseIFrame
    {
        get
        {
            return mUseIFrame;
        }
        set
        {
            mUseIFrame = value;
        }
    }


    /// <summary>
    /// Gets the current panel control
    /// </summary>
    protected Panel CurrentPanelControl
    {
        get
        {
            if (DisableScrollPanel)
            {
                return pnlControls;
            }

            return pnlScrollControls;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether scroll panel should be used
    /// </summary>
    public bool DisableScrollPanel
    {
        get;
        set;
    }


    /// <summary>
    /// Width of the menu. Applies only for vertical layout.
    /// </summary>
    public int Width
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
    /// Returns the UIElementInfo representing the first button of first group displayed.
    /// </summary>
    public UIElementInfo FirstUIElement
    {
        get
        {
            return mFirstUIElement;
        }
    }


    /// <summary>
    /// Returns the UIElementInfo representing the explicitly highlighted UI element.
    /// </summary>
    public UIElementInfo HighlightedUIElement
    {
        get
        {
            return mHighlightedUIElement;
        }
    }


    /// <summary>
    /// Returns the UIElementInfo representing the selected (either explicitly highlighted or first) UI element.
    /// </summary>
    public UIElementInfo SelectedUIElement
    {
        get
        {
            return HighlightedUIElement ?? FirstUIElement;
        }
    }


    /// <summary>
    /// Indicates whether at least one group with at least one button is rendered in the menu.
    /// </summary>
    public bool MenuEmpty
    {
        get
        {
            return mMenuEmpty;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first item should be highligted.
    /// </summary>
    public bool HighlightFirstItem
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates which item should be selected (has higher priority than HighlightFirstItem).
    /// </summary>
    public string HighlightItem
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether to remember item which was last selected and select it again.
    /// </summary>
    public bool RememberSelectedItem
    {
        get;
        set;
    }


    /// <summary>
    /// Target frameset in which the links generated from UI Elements are opened.
    /// </summary>
    public string TargetFrameset
    {
        get;
        set;
    }


    /// <summary>
    /// Starting page for first load. Used when this page has different URL from first item.
    /// </summary>
    public String StartingPage
    {
        get;
        set;
    }


    /// <summary>
    /// List of groups to display.
    /// </summary>
    public List<Group> Groups
    {
        get
        {
            if (mGroups == null)
            {
                mGroups = new List<Group>();
            }

            return mGroups;
        }
        set
        {
            mGroups = value;
        }
    }


    /// <summary>
    /// Description.
    /// </summary>
    public List<CMSUserControl> InnerControls
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether to display errors in the control.
    /// </summary>
    public bool ShowErrors
    {
        get
        {
            return mShowErrors;
        }
        set
        {
            mShowErrors = value;
        }
    }


    /// <summary>
    /// Code name of the module.
    /// </summary>
    public string ModuleName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if current UI culture is RTL.
    /// </summary>
    public bool IsRTL
    {
        get
        {
            if (mIsRTL == null)
            {
                mIsRTL = CultureHelper.IsUICultureRTL();
            }
            return (mIsRTL == true ? true : false);
        }
        set
        {
            mIsRTL = value;
        }
    }


    /// <summary>
    /// Indicates if empty group captions are allowed
    /// </summary>
    public bool AllowEmptyCaptions
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the culture (culture code) which is used for localizing resource strings.
    /// </summary>
    public string ResourceCulture
    {
        get
        {
            if (String.IsNullOrEmpty(mLocalizationCulture))
            {
                mLocalizationCulture = Thread.CurrentThread.CurrentUICulture.ToString();
            }

            return mLocalizationCulture;
        }
        set
        {
            mLocalizationCulture = value;
        }
    }

    #endregion


    #region "Custom events"

    /// <summary>
    /// Button filtered delegate.
    /// </summary>
    public delegate bool ButtonFilterEventHandler(object sender, UniMenuArgs e);


    /// <summary>
    /// Button created delegate.
    /// </summary>
    public delegate void ButtonCreatedEventHandler(object sender, UniMenuArgs e);


    /// <summary>
    /// Button filtered event handler.
    /// </summary>
    public event ButtonFilterEventHandler OnButtonFiltered;


    /// <summary>
    /// Button creating event handler.
    /// </summary>
    public event ButtonCreatedEventHandler OnButtonCreating;


    /// <summary>
    /// Button created event handler.
    /// </summary>
    public event ButtonCreatedEventHandler OnButtonCreated;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Groups.Count > 0)
        {
            pnlScrollControls.IsRTL = IsRTL;
            pnlScrollControls.InnerItemClass = "MiddleBigButton";

            if (RememberSelectedItem)
            {
                RegisterRememberSelectedItemScript();
                RegisterSelectItemScript("SelectButton(this);");
            }
            RegisterSelectItemScript("");

            InnerControls = new List<CMSUserControl>(Groups.Count);

            // Process the groups
            for (identifier = 0; identifier < Groups.Count; identifier++)
            {
                Group group = Groups[identifier];

                if (!string.IsNullOrEmpty(group.ControlPath) && (group.Control == null))
                {
                    group.Control = TryToCreateGroupControlBy(group.ControlPath, identifier);
                }
                if (group.Control != null)
                {
                    int index = identifier;
                    if (InnerControls.Count < index)
                    {
                        index = InnerControls.Count;
                    }

                    InnerControls.Insert(index, group.Control);
                }

                CMSPanel groupPanel = CreateGroupPanelWith(group, identifier);

                if (groupPanel != null)
                {
                    mMenuEmpty = false;
                    CurrentPanelControl.Controls.Add(groupPanel);

                    // Insert separator after group
                    if (Groups.Count > 1)
                    {
                        CurrentPanelControl.Controls.Add(GetGroupSeparator(group));
                    }
                }
            }

            // Handle the pre-selection
            if (preselectedPanel != null)
            {
                // Add the selected class to the preselected button
                preselectedPanel.CssClass += " Selected";
            }
            else if ((firstPanel != null) && HighlightFirstItem)
            {
                // Add the selected class to the first button
                firstPanel.CssClass += " Selected";
            }

            if (!HorizontalLayout)
            {
                CurrentPanelControl.CssClass = "Vertical";
                if (Width > 0)
                {
                    CurrentPanelControl.Attributes.Add("style", "width:" + Width + "px");
                }
            }
        }

        if (DisableScrollPanel)
        {
            pnlScrollControls.Visible = false;
            pnlLeftScroller.Visible = false;
            pnlRightScroller.Visible = false;
        }
        else
        {
            pnlControls.Visible = false;
        }
    }


    /// <summary>
    /// Register scripts
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSAdminControls/UI/UniMenu/UniMenu.js");

        base.OnPreRender(e);
    }


    /// <summary>
    /// Creates group control.
    /// </summary>
    /// <param name="path">Path to control to be created</param>
    /// <param name="identifier">Identifier of control</param>
    private CMSUserControl CreateGroupControl(string path, int identifier)
    {
        CMSUserControl contentControl;
        contentControl = (CMSUserControl)Page.LoadUserControl(path);
        contentControl.ID = "ctrlContent" + identifier;
        contentControl.ShortID = "c" + identifier;
        return contentControl;
    }


    /// <summary>
    /// Encapsulates operation of creating group control in try catch construction and returns result.
    /// </summary>
    /// <param name="path">Path of control to be created</param>
    /// <param name="identifier">Identifier of control</param>
    private CMSUserControl TryToCreateGroupControlBy(string path, int identifier)
    {
        try
        {
            return CreateGroupControl(path, identifier);
        }
        catch
        {
            Controls.Add(GetError(GetString("unimenu.errorloadingcontrol", ResourceCulture)));
            return null;
        }
    }


    /// <summary>
    /// Method for registering JS function for selecting button.
    /// </summary>
    private void RegisterRememberSelectedItemScript()
    {
        var script = String.Format(
@"
function SelectButton(elem) {{
    var selected = 'Selected';
    var jElem =$cmsj(elem);
    $cmsj('#{0}').find('.' + selected).removeClass(selected);
    jElem.addClass(selected);
}}
"
            , CurrentPanelControl.ClientID);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UIToolbarSelectionScript", ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Method for registering JS function for selecting item.
    /// </summary>
    /// <param name="buttonSelection">Function call for selecting button</param>
    private void RegisterSelectItemScript(string buttonSelection)
    {
        StringBuilder remoteSelectionScript = new StringBuilder();
        remoteSelectionScript.AppendLine("var SelectedItemID = null;");
        remoteSelectionScript.AppendLine("function SelectItem(elementID, elementUrl, forceSelection)");
        remoteSelectionScript.AppendLine("{");
        remoteSelectionScript.AppendLine("  if(forceSelection === undefined) forceSelection = true;");
        remoteSelectionScript.AppendLine("  if(SelectedItemID == elementID && !forceSelection) { return; }");
        remoteSelectionScript.AppendLine("    SelectedItemID = elementID;");
        remoteSelectionScript.AppendLine("    var selected = 'Selected';");
        remoteSelectionScript.AppendFormat("    $cmsj(\"#{0} .\" + selected).removeClass(selected);\n", CurrentPanelControl.ClientID);
        remoteSelectionScript.AppendFormat("    $cmsj(\"#{0} div[name='\" + elementID + \"']\").addClass(selected);\n", CurrentPanelControl.ClientID);
        remoteSelectionScript.AppendLine("    if(elementUrl != null && elementUrl != '') {");
        if (!String.IsNullOrEmpty(TargetFrameset))
        {
            // Create target based on iframe usage
            String target = UseIFrame ? String.Format("frames['{0}']", TargetFrameset) : String.Format("parent.frames['{0}']", TargetFrameset);
            remoteSelectionScript.AppendLine(String.Format("{0}{1}.location.href = elementUrl;", buttonSelection, target));
        }
        else
        {
            remoteSelectionScript.AppendLine(buttonSelection + "self.location.href = elementUrl;");
        }
        remoteSelectionScript.AppendLine("    }");
        remoteSelectionScript.AppendLine("}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UIToolbarRemoteSelectionScript", ScriptHelper.GetScript(remoteSelectionScript.ToString()));
    }


    /// <summary>
    /// Creates group panel from group.
    /// </summary>
    /// <param name="group">Definition of group</param>
    /// <param name="identifier">Identifier of panel</param>
    private CMSPanel CreateGroupPanelWith(Group group, int identifier)
    {
        Panel innerPanel = GetContent(group.Control, group.UIElementID, group.Caption);
        CMSPanel groupPanel = null;

        if (innerPanel != null)
        {
            groupPanel = new CMSPanel()
            {
                ID = "pnlGroup" + identifier,
                ShortID = "g" + identifier
            };

            groupPanel.Controls.Add(GetLeftBorder());
            groupPanel.Controls.Add(innerPanel);
            groupPanel.Controls.Add(GetRightBorder());
            groupPanel.CssClass = group.CssClass;
        }

        return groupPanel;
    }


    /// <summary>
    /// Generates div with left border.
    /// </summary>
    /// <returns>Panel with left border</returns>
    private Panel GetLeftBorder()
    {
        // Create panel and set up
        return new CMSPanel()
        {
            ID = "lb" + identifier,
            EnableViewState = false,
            CssClass = "UniMenuLeftBorder"
        };
    }


    /// <summary>
    /// Generates div with right border.
    /// </summary>
    /// <returns>Panel with right border</returns>
    private Panel GetRightBorder()
    {
        // Create panel and set up
        return new CMSPanel()
        {
            ID = "pnlRightBorder" + identifier,
            ShortID = "rb" + identifier,
            EnableViewState = false,
            CssClass = "UniMenuRightBorder"
        };
    }


    /// <summary>
    /// Generates separator between groups.
    /// </summary>
    /// <param name="group">Group object</param>
    /// <returns>Panel separating groups</returns>
    private Panel GetGroupSeparator(Group group)
    {
        string css = String.IsNullOrEmpty(group.SeparatorCssClass) ? "UniMenuSeparator" : "UniMenuSeparator " + group.SeparatorCssClass;
        // Create panel and set up
        return new CMSPanel()
        {
            ID = "pnlGroupSeparator" + identifier,
            ShortID = "gs" + identifier,
            EnableViewState = false,
            CssClass = css
        };
    }


    /// <summary>
    /// Generates div with right border.
    /// </summary>
    /// <param name="captionText">Caption of group</param>
    /// <returns>Panel with right border</returns>
    private Panel GetCaption(string captionText)
    {
        // Create literal with caption
        Literal caption = new Literal()
        {
            ID = "ltlCaption" + identifier,
            EnableViewState = false,
            Text = captionText
        };

        // Create panel and add caption literal
        CMSPanel captionPanel = new CMSPanel()
        {
            ID = "pnlCaption" + identifier,
            ShortID = "cp" + identifier,
            EnableViewState = false,
            CssClass = "UniMenuDescription"
        };
        captionPanel.Controls.Add(caption);

        return captionPanel;
    }


    /// <summary>
    /// Generates panel with buttons loaded from given UI Element.
    /// </summary>
    /// <param name="uiElementId">ID of the UI Element</param>
    private Panel GetButtons(int uiElementId)
    {
        const int bigButtonMinimalWidth = 40;
        const int smallButtonMinimalWidth = 66;

        // Load the buttons manually from UI Element
        IEnumerable<UIElementInfo> elements = UIElementInfoProvider.GetChildUIElements(uiElementId);

        // When no child found
        if (!elements.Any())
        {
            // Try to use group element as button
            elements = UIElementInfo.Provider.Get()
                .WhereEquals("ElementID", uiElementId);

            var groupElement = elements.FirstOrDefault();
            if (groupElement != null)
            {
                // Use group element as button only if it has URL specified
                if (String.IsNullOrEmpty(groupElement.ElementTargetURL))
                {
                    elements = Enumerable.Empty<UIElementInfo>();
                }
            }
        }

        // Filter the dataset according to UI Profile
        var filteredElements = FilterElements(elements).ToList();

        int small = 0;
        int count = filteredElements.Count;

        // No buttons, render nothing
        if (count == 0)
        {
            return null;
        }

        // Prepare the panel
        var pnlButtons = new Panel();
        pnlButtons.CssClass = "ActionButtons";

        // Prepare the table
        Table tabGroup = new Table();
        TableRow tabGroupRow = new TableRow();

        tabGroup.CellPadding = 0;
        tabGroup.CellSpacing = 0;
        tabGroup.EnableViewState = false;
        tabGroupRow.EnableViewState = false;
        tabGroup.Rows.Add(tabGroupRow);

        List<Panel> panels = new List<Panel>();

        for (int i = 0; i < count; i++)
        {
            // Get current and next button
            UIElementInfo uiElement = filteredElements[i];

            UIElementInfo sel = UIContextHelper.CheckSelectedElement(uiElement, UIContext);
            if (sel != null)
            {
                String selectionSuffix = ValidationHelper.GetString(UIContext["selectionSuffix"], String.Empty);
                StartingPage = UIContextHelper.GetElementUrl(sel, UIContext) + selectionSuffix;
                HighlightItem = uiElement.ElementName;
            }

            // Raise button creating event
            OnButtonCreating?.Invoke(this, new UniMenuArgs { UIElement = uiElement });

            UIElementInfo uiElementNext = null;
            if (i < count - 1)
            {
                uiElementNext = filteredElements[i + 1];
            }

            // Set the first button
            if (mFirstUIElement == null)
            {
                mFirstUIElement = uiElement;
            }

            // Get the sizes of current and next button. Button is large when it is the only in the group
            bool isSmall = (uiElement.ElementSize == UIElementSizeEnum.Regular) && (count > 1);
            bool isResized = (uiElement.ElementSize == UIElementSizeEnum.Regular) && (!isSmall);
            bool isNextSmall = (uiElementNext != null) && (uiElementNext.ElementSize == UIElementSizeEnum.Regular);

            // Set the CSS class according to the button size
            string cssClass = (isSmall ? "SmallButton" : "BigButton");
            string elementName = uiElement.ElementName;

            // Display only caption - do not substitute with Display name when empty
            string elementCaption = ResHelper.LocalizeString(uiElement.ElementCaption, ResourceCulture);

            // Create main button panel
            CMSPanel pnlButton = new CMSPanel
            {
                ID = "pnlButton" + elementName,
                ShortID = "b" + elementName
            };

            pnlButton.Attributes.Add("name", elementName);
            pnlButton.CssClass = cssClass;

            // Remember the first button
            if (firstPanel == null)
            {
                firstPanel = pnlButton;
            }

            // Remember the selected button
            if ((preselectedPanel == null) && elementName.EqualsCSafe(HighlightItem, true))
            {
                preselectedPanel = pnlButton;

                // Set the selected button
                if (mHighlightedUIElement == null)
                {
                    mHighlightedUIElement = uiElement;
                }
            }

            // URL or behavior
            string url = uiElement.ElementTargetURL;

            if (!string.IsNullOrEmpty(url) && url.StartsWithCSafe("javascript:", true))
            {
                pnlButton.Attributes["onclick"] = url.Substring("javascript:".Length);
            }
            else
            {
                url = UIContextHelper.GetElementUrl(uiElement, UIContext);

                if (url != String.Empty)
                {
                    string buttonSelection = (RememberSelectedItem ? "SelectButton(this);" : "");

                    // Ensure hash code if required
                    url = MacroResolver.Resolve(URLHelper.EnsureHashToQueryParameters(url));

                    if (!String.IsNullOrEmpty(TargetFrameset))
                    {
                        if (uiElement.ElementType == UIElementTypeEnum.PageTemplate)
                        {
                            url = URLHelper.UpdateParameterInUrl(url, "displaytitle", "false");
                        }

                        String target = UseIFrame ? String.Format("frames['{0}']", TargetFrameset) : String.Format("parent.frames['{0}']", TargetFrameset);
                        pnlButton.Attributes["onclick"] = String.Format("{0}{1}.location.href = '{2}';", buttonSelection, target, UrlResolver.ResolveUrl(url));
                    }
                    else
                    {
                        pnlButton.Attributes["onclick"] = String.Format("{0}self.location.href = '{1}';", buttonSelection, UrlResolver.ResolveUrl(url));
                    }
                }
            }

            // Tooltip
            if (!String.IsNullOrEmpty(uiElement.ElementDescription))
            {
                pnlButton.ToolTip = ResHelper.LocalizeString(uiElement.ElementDescription, ResourceCulture);
            }
            else
            {
                pnlButton.ToolTip = elementCaption;
            }
            pnlButton.EnableViewState = false;

            // Ensure correct grouping of small buttons
            if (isSmall && (small == 0))
            {
                small++;

                Panel pnlSmallGroup = new Panel
                {
                    ID = "pnlGroupSmall" + uiElement.ElementName
                };
                if (IsRTL)
                {
                    pnlSmallGroup.Style.Add("float", "right");
                    pnlSmallGroup.Style.Add("text-align", "right");
                }
                else
                {
                    pnlSmallGroup.Style.Add("float", "left");
                    pnlSmallGroup.Style.Add("text-align", "left");
                }

                pnlSmallGroup.EnableViewState = false;
                pnlSmallGroup.Controls.Add(pnlButton);
                panels.Add(pnlSmallGroup);
            }

            // Generate button link
            HyperLink buttonLink = new HyperLink
            {
                ID = "lnkButton" + uiElement.ElementName,
                EnableViewState = false
            };

            // Generate button image
            Image buttonImage = new Image
            {
                ID = "imgButton" + uiElement.ElementName,
                ImageAlign = (isSmall ? ImageAlign.AbsMiddle : ImageAlign.Top),
                AlternateText = elementCaption,
                EnableViewState = false
            };

            // Use icon path
            if (!string.IsNullOrEmpty(uiElement.ElementIconPath))
            {
                string iconPath = GetImagePath(uiElement.ElementIconPath);

                // Check if element size was changed
                if (isResized)
                {
                    // Try to get larger icon
                    string largeIconPath = iconPath.Replace("list.png", "module.png");
                    if (FileHelper.FileExists(largeIconPath))
                    {
                        iconPath = largeIconPath;
                    }
                }

                buttonImage.ImageUrl = GetImageUrl(iconPath);
                buttonLink.Controls.Add(buttonImage);
            }
            // Use Icon class
            else if (!string.IsNullOrEmpty(uiElement.ElementIconClass))
            {
                var icon = new CMSIcon
                {
                    ID = string.Format("ico_{0}_{1}", identifier, i),
                    EnableViewState = false,
                    ToolTip = pnlButton.ToolTip,
                    CssClass = "cms-icon-80 " + uiElement.ElementIconClass
                };
                buttonLink.Controls.Add(icon);
            }
            // Load default module icon if ElementIconPath is not specified
            else
            {
                buttonImage.ImageUrl = GetImageUrl("CMSModules/module.png");
                buttonLink.Controls.Add(buttonImage);
            }

            // Generate caption text
            Literal captionLiteral = new Literal
            {
                ID = "ltlCaption" + uiElement.ElementName,
                Text = (isSmall ? "\n" : "<br />") + elementCaption,
                EnableViewState = false
            };
            buttonLink.Controls.Add(captionLiteral);

            //Generate button table (IE7 issue)
            Table tabButton = new Table();
            TableRow tabRow = new TableRow();
            TableCell tabCellLeft = new TableCell();
            TableCell tabCellMiddle = new TableCell();
            TableCell tabCellRight = new TableCell();

            tabButton.CellPadding = 0;
            tabButton.CellSpacing = 0;

            tabButton.EnableViewState = false;
            tabRow.EnableViewState = false;
            tabCellLeft.EnableViewState = false;
            tabCellMiddle.EnableViewState = false;
            tabCellRight.EnableViewState = false;

            tabButton.Rows.Add(tabRow);
            tabRow.Cells.Add(tabCellLeft);
            tabRow.Cells.Add(tabCellMiddle);
            tabRow.Cells.Add(tabCellRight);

            // Generate left border
            Panel pnlLeft = new Panel
            {
                ID = "pnlLeft" + uiElement.ElementName,
                CssClass = "Left" + cssClass,
                EnableViewState = false
            };

            // Generate middle part of button
            Panel pnlMiddle = new Panel
            {
                ID = "pnlMiddle" + uiElement.ElementName,
                CssClass = "Middle" + cssClass
            };
            pnlMiddle.Controls.Add(buttonLink);
            Panel pnlMiddleTmp = new Panel
            {
                EnableViewState = false
            };
            if (isSmall)
            {
                pnlMiddle.Style.Add("min-width", smallButtonMinimalWidth + "px");
                // IE7 issue with min-width
                pnlMiddleTmp.Style.Add("width", smallButtonMinimalWidth + "px");
                pnlMiddle.Controls.Add(pnlMiddleTmp);
            }
            else
            {
                pnlMiddle.Style.Add("min-width", bigButtonMinimalWidth + "px");
                // IE7 issue with min-width
                pnlMiddleTmp.Style.Add("width", bigButtonMinimalWidth + "px");
                pnlMiddle.Controls.Add(pnlMiddleTmp);
            }
            pnlMiddle.EnableViewState = false;

            // Generate right border
            Panel pnlRight = new Panel
            {
                ID = "pnlRight" + uiElement.ElementName,
                CssClass = "Right" + cssClass,
                EnableViewState = false
            };

            // Add inner controls
            tabCellLeft.Controls.Add(pnlLeft);
            tabCellMiddle.Controls.Add(pnlMiddle);
            tabCellRight.Controls.Add(pnlRight);

            pnlButton.Controls.Add(tabButton);

            // If there were two small buttons in a row end the grouping div
            if ((small == 2) || (isSmall && !isNextSmall))
            {
                small = 0;

                // Add the button to the small buttons grouping panel
                panels[panels.Count - 1].Controls.Add(pnlButton);
            }
            else
            {
                if (small == 0)
                {
                    // Add the generated button into collection
                    panels.Add(pnlButton);
                }
            }
            if (small == 1)
            {
                small++;
            }

            // Raise button created event
            OnButtonCreated?.Invoke(this, new UniMenuArgs { UIElement = uiElement, TargetUrl = url, ButtonControl = pnlButton, ImageControl = buttonImage });
        }

        // Add all panels to control
        foreach (Panel panel in panels)
        {
            TableCell tabGroupCell = new TableCell
            {
                VerticalAlign = VerticalAlign.Top,
                EnableViewState = false
            };

            tabGroupCell.Controls.Add(panel);
            tabGroupRow.Cells.Add(tabGroupCell);
        }

        pnlButtons.Controls.Add(tabGroup);

        return pnlButtons;
    }


    /// <summary>
    /// Generates panel with content and caption.
    /// </summary>
    /// <param name="control">Control to add to the content</param>
    /// <param name="captionText">Caption of group</param>
    /// <param name="uiElementId">ID of the UI Element, if the content should be loaded from UI Elements</param>
    private Panel GetContent(Control control, int uiElementId, string captionText)
    {
        Panel content = null;
        if (string.IsNullOrEmpty(captionText) && !AllowEmptyCaptions)
        {
            if (ShowErrors)
            {
                Controls.Add(GetError(GetString("unimenu.captionempty", ResourceCulture)));
            }
        }
        else if (control == null && uiElementId == 0)
        {
            if (ShowErrors)
            {
                Controls.Add(GetError(GetString("unimenu.pathempty", ResourceCulture)));
            }
        }
        else
        {
            // Create panel and set up
            content = new Panel
            {
                ID = "pnlContent" + identifier,
                CssClass = "UniMenuContent"
            };

            // Add caption
            if (!HorizontalLayout && !string.IsNullOrEmpty(captionText))
            {
                content.Controls.Add(GetCaption(captionText));
            }

            // Add inner content control
            if (control != null)
            {
                content.Controls.Add(control);
            }
            else if (uiElementId > 0)
            {
                Panel innerPanel = GetButtons(uiElementId);
                if (innerPanel == null)
                {
                    return null;
                }
                else
                {
                    content.Controls.Add(innerPanel);
                }
            }

            // Add caption
            if (HorizontalLayout && !string.IsNullOrEmpty(captionText))
            {
                content.Controls.Add(GetCaption(captionText));
            }
        }

        return content;
    }


    /// <summary>
    /// Generates error label.
    /// </summary>
    /// <param name="message">Error message to display</param>
    /// <returns>Label with error message</returns>
    private Label GetError(string message)
    {
        // If error occurs skip this group
        return new Label
        {
            ID = "lblError" + identifier,
            EnableViewState = false,
            Text = message,
            CssClass = "ErrorLabel"
        };
    }


    /// <summary>
    /// Filters the dataset with UI Elements according to UI Profile of current user by default and according to custom event (if defined).
    /// </summary>
    private IEnumerable<UIElementInfo> FilterElements(IEnumerable<UIElementInfo> elements)
    {
        foreach (var uiElement in elements)
        {
            bool allowed = MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement(uiElement.ElementResourceID, uiElement.ElementName);

            if (OnButtonFiltered != null)
            {
                allowed = allowed && OnButtonFiltered(this, new UniMenuArgs { UIElement = uiElement });
            }

            if (allowed && UIContextHelper.CheckElementAvailabilityInUI(uiElement))
            {
                yield return uiElement;
            }
        }
    }

    #endregion
}