using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_VerticalTabs : CMSAbstractUIWebpart
{
    #region "Variables"

    private UITabs tabControl;
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
    /// Indicates whether this instance represents a UI application.
    /// </summary>
    public bool IsApplication
    {
        get
        {
            return (UIContext.UIElement != null) && UIContext.UIElement.IsApplication;
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
    /// Indicates whether display title in children tabs
    /// </summary>
    public bool DisplayTitleInTabs
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayTitleInTabs"), false);
        }
        set
        {
            SetValue("DisplayTitleInTabs", value);
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
            tabControl.RememberSelectedTab = value;
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
            tabControl.AllowSubTabs = value;
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

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        if (StopProcessing)
        {
            // No actions if processing is stopped
        }
        else
        {
            SiteSelector.DropDownSingleSelect.AutoPostBack = true;

            var panel = paneTabs.FindControl("pnlSiteContainer") as PlaceHolder;
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
            SetHeaderControls(paneTitle);

            var isDesign = PortalContext.ViewMode.IsDesign(true);
            if (isDesign)
            {
                paneContent.RenderAs = HtmlTextWriterTag.Div;
            }

            // Handle title
            ManagePaneTitle(paneTitle, true);

            // Show dialog footer only when used in a dialog
            layoutElem.DisplayFooter = DisplayFooter;

            tabControl = paneTabs.FindControl("tabsElem") as UITabs;

            // Ensure correct property
            UIContext["DisplayTitleInTabs"] = GetValue("DisplayTitleInTabs");

            if (tabControl != null)
            {
                tabControl.ElementName = ElementName;
                tabControl.ModuleName = ResourceName;
                tabControl.DefaultTabName = DefaultTabName;

                tabControl.TabControlLayout = TabControlLayoutEnum.Vertical;

                tabControl.RememberSelectedTab = RememberSelectedTab && !isDesign;
                tabControl.AllowSubTabs = AllowSubTabs;

                // Set selected tab based on tab index
                int tabIndex = ValidationHelper.GetInteger(UIContext["TabIndex"], 0);
                if (tabIndex != 0)
                {
                    tabControl.SelectedTab = tabIndex;
                    tabControl.SelectFirstItemByDefault = false;
                }

                tabControl.OnTabCreated += tabControl_OnTabCreated;

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
            ScriptHelper.RegisterJQuery(Page);
            RequestContext.ClientApplication.Add("isVerticalTabs", true);
            RequestContext.ClientApplication.Add("isApplication", IsApplication);

            bool allowScrolling = !PortalContext.ViewMode.IsDesign(true);

            ScriptHelper.RegisterModule(this, "CMS/VerticalTabs", new
            {
                id = layoutElem.ClientID,
                scrollable = allowScrolling
            });

            if (allowScrolling)
            {
                CssRegistration.RegisterCssLink(Page, "~/CMSScripts/jquery/jquery-jscrollpane.css");
            }

            if (RequestHelper.IsPostBack())
            {
                tabControl.DoTabSelection();
            }
        }

        base.OnPreRender(e);
    }


    protected void tabsElem_OnTabsLoaded(List<UITabItem> tabs)
    {
        ManageVersionTab(UIContext, tabs);

        // Show information message in case there is no child element to display in the tab control
        if (tabControl.TabItems.Count == 0)
        {
            CMSPage.RedirectToInformation(GetString("uielement.nochildfound"));
        }

        // Manage tab index based on tab name
        String tabName = UIContext["TabName"].ToString("");
        if (tabName != string.Empty)
        {
            var ti = tabs.Find(x => tabName.EqualsCSafe(x.TabName, true));
            if (ti != null)
            {
                tabControl.SelectedTab = ti.Index;
                tabControl.SelectFirstItemByDefault = false;
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

    #endregion
}