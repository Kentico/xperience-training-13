using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_RecycleBin_Pages_RecycleBin_Objects : GlobalAdminPage
{
    #region "Constants"

    private const string GLOBAL_OBJECTS = "##global##";

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Check the license
        if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.ObjectVersioning);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Set site selector
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.OnlyRunningSites = false;
        siteSelector.AllowAll = false;
        CurrentMaster.DisplaySiteSelectorPanel = true;
        
        // Add special fields
        siteSelector.UniSelector.SpecialFields.Add(new SpecialField {Text = GetString("RecycleBin.AllSitesAndGlobal"), Value = "0"});
        siteSelector.UniSelector.SpecialFields.Add(new SpecialField {Text = GetString("General.GlobalObjects"), Value = "-1"});
        
        siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

        if (!RequestHelper.IsPostBack())
        {
            siteSelector.Value = 0;
        }

        // Set site name to recycle bin control
        int siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        SiteInfo si = SiteInfo.Provider.Get(siteId);
        if (si != null)
        {
            recycleBin.SiteName = si.SiteName;
            UIContext["SiteID"] = siteId;
        }
        else if (siteId == -1)
        {
            recycleBin.SiteName = GLOBAL_OBJECTS;
        }

        // Set delayed reload if site was changed
        Control pbCtrl = ControlsHelper.GetPostBackControl(this);
        if ((pbCtrl != null) && (pbCtrl == siteSelector.DropDownSingleSelect))
        {
            recycleBin.DelayedLoading = true;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Hide site selector if there are no sites
        if (!siteSelector.UniSelector.HasData)
        {
            pnlSiteSelector.Visible = false;            
        }
        base.OnPreRender(e);
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        recycleBin.ReloadData(true);
    }

    #endregion
}