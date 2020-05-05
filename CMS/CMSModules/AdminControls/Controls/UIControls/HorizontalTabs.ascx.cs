using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DocumentEngine.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_HorizontalTabs : CMSAbstractUIWebpart
{
    #region "Variables"

    private UITabs mTabControl;
    private CMSFormControls_Sites_SiteSelector mSiteSelector;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether site selector should be displayed.
    /// </summary>
    public bool DisplaySiteSelector
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplaySiteSelector"), false);
        }
        set
        {
            SetValue("DisplaySiteSelector", value);
        }
    }


    /// <summary>
    /// Tab selected by default.
    /// </summary>
    public String DefaultTabName
    {
        get
        {
            return GetStringContextValue("DefaultTabName");
        }
        set
        {
            SetValue("DefaultTabName", value);
        }
    }


    /// <summary>
    /// Gets site selector control nested in UILayoutPane template.
    /// </summary>
    public CMSFormControls_Sites_SiteSelector SiteSelector
    {
        get
        {
            return mSiteSelector ?? (mSiteSelector = paneTabs.FindControl("siteSelector") as CMSFormControls_Sites_SiteSelector);
        }
    }


    /// <summary>
    /// If true, drop down contains '(global)'
    /// </summary>
    public bool AllowGlobal
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowGlobal"), false);
        }
        set
        {
            SetValue("AllowGlobal", value);
        }
    }


    /// <summary>
    /// If true, drop down contains '(empty)'
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEmpty"), false);
        }
        set
        {
            SetValue("AllowEmpty", value);
        }
    }


    /// <summary>
    /// If true, drop down contains '(all)'
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowAll"), false);
        }
        set
        {
            SetValue("AllowAll", value);
        }
    }


    /// <summary>
    /// Returns true if the control processing should be stopped
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            layoutElem.StopProcessing = value;
            layoutElem.Visible = !value;
        }
    }


    /// <summary>
    /// Additional query parameters in tabs url.
    /// </summary>
    public String AdditionalTabQuery
    {
        get
        {
            return GetStringContextValue("AdditionalTabQuery");
        }
    }


    /// <summary>
    /// Indicates whether the selected tabs is remembered when the page is next loaded
    /// </summary>
    public bool RememberSelectedTab
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RememberSelectedTab"), false);
        }
        set
        {
            SetValue("RememberSelectedTab", value);
            TabControl.RememberSelectedTab = value;
        }
    }


    /// <summary>
    /// If true, the sub tabs are allowed in these tabs
    /// </summary>
    public bool AllowSubTabs
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowSubTabs"), true);
        }
        set
        {
            SetValue("AllowSubTabs", value);
            TabControl.AllowSubTabs = value;
        }
    }


    /// <summary>
    /// Tab control
    /// </summary>
    private UITabs TabControl
    {
        get
        {
            return mTabControl ?? (mTabControl = paneTabs.FindControl("tabsElem") as UITabs);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        if (StopProcessing)
        {
            // No actions if processing is stopped
        }
        else
        {
            SetHeaderControls(paneTabs);

            SiteSelector.DropDownSingleSelect.AutoPostBack = true;
            PlaceHolder panel = paneTabs.FindControl("pnlSiteContainer") as PlaceHolder;
            if (panel != null)
            {
                if (DisplaySiteSelector)
                {
                    SiteSelector.AllowAll = AllowAll;
                    SiteSelector.AllowEmpty = AllowEmpty;
                    SiteSelector.AllowGlobal = AllowGlobal;

                    if (!RequestHelper.IsPostBack())
                    {
                        int siteId = ValidationHelper.GetInteger(UIContext["SiteID"], 0);
                        if (siteId != 0)
                        {
                            SiteSelector.Value = siteId;
                        }

                        // Reload for first time selection
                        SiteSelector.Reload(false);
                    }

                    // Register event
                    UIContext.OnGetValue += Current_OnGetValue;
                }
                else
                {
                    panel.Visible = false;
                    SiteSelector.StopProcessing = true;
                }
            }
        }

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // No actions if processing is stopped
        }
        else
        {
            ManagePaneTitle(paneTabs, false);

            // Show dialog footer only when used in a dialog
            layoutElem.DisplayFooter = DisplayFooter;

            var isDesign = PortalContext.ViewMode.IsDesign(true);
            if (isDesign)
            {
                paneContent.RenderAs = HtmlTextWriterTag.Div;
                paneContent.Size = "0";
            }

            if (IsDialog)
            {
                paneTabs.PaneClass = "DialogsPageHeader";
            }

            var tabs = TabControl;
            if (tabs != null)
            {
                tabs.DefaultTabName = DefaultTabName;
                tabs.ElementName = ElementName;
                tabs.ModuleName = ResourceName;

                tabs.TabControlLayout = TabControlLayoutEnum.Horizontal;

                tabs.RememberSelectedTab = RememberSelectedTab && !isDesign;
                tabs.AllowSubTabs = AllowSubTabs;

                // Set selected tab based on tab index
                int tabIndex = ValidationHelper.GetInteger(UIContext["TabIndex"], 0);
                if (tabIndex != 0)
                {
                    tabs.SelectedTab = tabIndex;
                    tabs.SelectFirstItemByDefault = false;
                }

                tabs.OnTabCreated += tabControl_OnTabCreated;
                RegisterBreadcrumbsScript();
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            // No actions if processing is stopped
        }
        else
        {
            if (RequestHelper.IsPostBack())
            {
                TabControl.DoTabSelection();
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Other events"

    private void Current_OnGetValue(object sender, UIContextEventArgs e)
    {
        if (e.ColumnName.EqualsCSafe("siteid", true))
        {
            int siteId = SiteSelector.SiteID;
            if (SiteSelector.Visible && (siteId != 0))
            {
                e.Result = siteId;
            }
        }
    }


    protected void tabsElem_OnTabsLoaded(List<UITabItem> tabs)
    {
        // Dynamically add versions tab
        ManageVersionTab(UIContext, tabs);

        // Show information message in case there is no child element to display in the tab control
        if (TabControl.TabItems.Count == 0)
        {
            CMSPage.RedirectToInformation(GetString("uielement.nochildfound"));
        }

        // Manage tab index based on tab name
        String tabName = UIContext["TabName"].ToString("");
        if (tabName != string.Empty)
        {
            UITabItem ti = tabs.Find(x => tabName.EqualsCSafe(x.TabName, true));
            if (ti != null)
            {
                TabControl.SelectedTab = ti.Index;
                TabControl.SelectFirstItemByDefault = false;
            }
        }
    }


    /// <summary>
    /// Appends additional query parameters.
    /// </summary>
    protected void tabControl_OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        var tab = e.Tab;

        if (!String.IsNullOrEmpty(AdditionalTabQuery))
        {
            tab.RedirectUrl = URLHelper.AppendQuery(tab.RedirectUrl, AdditionalTabQuery);
        }
    }

    #endregion
}