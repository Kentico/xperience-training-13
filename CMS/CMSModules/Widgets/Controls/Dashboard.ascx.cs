using System.Globalization;
using System.Threading;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.SiteProvider;

public partial class CMSModules_Widgets_Controls_Dashboard : CMSUserControl
{
    #region "Variables"

    private bool mHighlightDropableAreas = true;

    #endregion


    #region "Properties"
    
    /// <summary>
    /// Droppable areas are highlighted when widget dragged.
    /// </summary>
    public bool HighlightDropableAreas
    {
        get
        {
            return mHighlightDropableAreas;
        }
        set
        {
            mHighlightDropableAreas = value;
        }
    }


    /// <summary>
    /// If true zone border can be activated (+add widget button).
    /// </summary>
    public bool ActivateZoneBorder
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Ensures site dashboard initialization.
    /// </summary>
    public void SetupSiteDashboard()
    {
        PortalContext.DashboardSiteName = SiteContext.CurrentSiteName;
        SetupDashboard();
    }


    /// <summary>
    /// Ensures dashboard initialization.
    /// </summary>
    public void SetupDashboard()
    {
        // Register placeholder for context menu
        DashboardPage page = Page as DashboardPage;
        if (page != null)
        {
            page.ContextMenuContainer = plcCtx;
            page.ManagersContainer = plcManagers;
            page.ScriptManagerControl = manScript;
        }

        // Default settings for drag and drop for dashboard
        manPortal.HighlightDropableAreas = HighlightDropableAreas;
        manPortal.ActivateZoneBorder = ActivateZoneBorder;
        
        // Set culture
        CultureInfo ci = CultureHelper.PreferredUICultureInfo;
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;

        manPortal.SetMainPagePlaceholder(plc);
    }
    
    #endregion
}