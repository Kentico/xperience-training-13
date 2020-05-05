using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Protection;
using CMS.UIControls;


public partial class CMSModules_AbuseReport_Controls_AbuseReportListAndEdit : CMSAdminControl
{
    #region "Properties"

    /// <summary>
    /// Where condition for grid.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ucAbuseReportList.WhereCondition;
        }
        set
        {
            ucAbuseReportList.WhereCondition = value;
        }
    }


    /// <summary>
    /// Items per page for grid.
    /// </summary>
    public string ItemsPerPage
    {
        get
        {
            return ucAbuseReportList.ItemsPerPage;
        }
        set
        {
            ucAbuseReportList.ItemsPerPage = value;
        }
    }


    /// <summary>
    /// Site name filter.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ucAbuseReportList.SiteName;
        }
        set
        {
            ucAbuseReportList.SiteName = value;
        }
    }


    /// <summary>
    /// Status of abuse report.
    /// </summary>
    public int Status
    {
        get
        {
            return ucAbuseReportList.Status;
        }
        set
        {
            ucAbuseReportList.Status = value;
        }
    }


    /// <summary>
    /// Order by for uni grid.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ucAbuseReportList.OrderBy;
        }
        set
        {
            ucAbuseReportList.OrderBy = value;
        }
    }


    /// <summary>
    /// Returns true if the control processing should be stopped.
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
            ucAbuseEdit.StopProcessing = value;
            ucAbuseReportList.StopProcessing = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        ucAbuseReportList.UseEditOnPostback = true;

        if (!String.IsNullOrEmpty(hfEditReport.Value))
        {
            ucAbuseEdit.ReportID = ValidationHelper.GetInteger(hfEditReport.Value, 0);
        }

        // Edit control must have stop processing true if list (first edit will be executed in prerender) to handle ItemEdit properly
        if (ucAbuseEdit.ReportID == 0)
        {
            ucAbuseEdit.StopProcessing = true;
        }

        ucAbuseReportList.OnCheckPermissions += new CheckPermissionsEventHandler(ucAbuseReportList_OnCheckPermissions);
        ucAbuseEdit.OnCheckPermissions += new CheckPermissionsEventHandler(ucAbuseReportList_OnCheckPermissions);
        lnkBackHidden.Click += lnkBackHidden_Click;
    }


    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        hfEditReport.Value = String.Empty;
        ucAbuseEdit.Visible = false;
        ucAbuseReportList.Visible = true;
        pnlHeader.Visible = false;
    }


    /// <summary>
    /// Check permissions.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="sender">Sender</param>
    private void ucAbuseReportList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if ((ucAbuseReportList.EditReportID != 0) || (!String.IsNullOrEmpty(hfEditReport.Value)))
        {
            ucAbuseEdit.Visible = true;
            ucAbuseReportList.Visible = false;
            ucAbuseReportList.StopProcessing = true;
            ucAbuseEdit.StopProcessing = false;
            pnlHeader.Visible = true;

            ucAbuseEdit.ReportID = ucAbuseReportList.EditReportID;

            bool editForceLoad = false;
            if (ucAbuseEdit.ReportID != 0)
            {
                hfEditReport.Value = ucAbuseEdit.ReportID.ToString();
                editForceLoad = true;
            }

            ucAbuseEdit.ReloadData(editForceLoad);
            SetupBreadcrumbs();
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Initializes and updates breadcrumbs items.
    /// </summary>
    private void SetupBreadcrumbs()
    {
        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = GetString("abuse.reports"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        int reportID = ValidationHelper.GetInteger(ucAbuseEdit.ReportID, 0);

        if (!String.IsNullOrEmpty(hfEditReport.Value))
        {
            reportID = ValidationHelper.GetInteger(hfEditReport.Value, 0);
        }

        AbuseReportInfo ari = AbuseReportInfoProvider.GetAbuseReportInfo(reportID);
        if (ari != null)
        {
            ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = ari.ReportTitle,
            });
        }
    }
}