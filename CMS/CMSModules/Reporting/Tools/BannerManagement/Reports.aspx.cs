using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.BannerManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Reporting;

public partial class CMSModules_Reporting_Tools_BannerManagement_Reports : CMSBannerManagementEditPage
{
    #region "Private fields"

    private bool mParamsLoaded;
    private bool mIsSaved;

    #endregion


    #region "Properties"

    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!mIsSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        MessagesPlaceHolder = plcMess;

        string objectType = string.Empty;
        string elementName = string.Empty;

        switch (QueryHelper.GetString("parameterName", string.Empty).ToLowerInvariant())
        {
            // It is banner
            case "bannerid":
                objectType = BannerInfo.OBJECT_TYPE;
                elementName = "Report_1";
                break;

            // It is banner category
            case "bannercategoryid":
                objectType = BannerCategoryInfo.OBJECT_TYPE;
                elementName = "Report";
                break;

            default:
                RedirectToInformation(GetString("bannermanagement.error.internal"));
                break;
        }


        // Check UI personalization
        var uiElement = new UIElementAttribute(ResourceName, elementName);
        uiElement.Check(this);

        // Check Reporting permissions
        CheckReportingAvailability();

        // Get the ID
        int id = QueryHelper.GetInteger("parameterValue", 0);

        SetEditedObject(ProviderHelper.GetInfoById(objectType, id), string.Empty);
    }


    /// <summary>
    /// Checks permissions for Reporting module.
    /// </summary>
    private void CheckReportingAvailability()
    {
        if (!ModuleEntryManager.IsModuleLoaded(ModuleName.REPORTING))
        {
            RedirectToUINotAvailable();
        }

        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Reporting", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Reporting");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Set disabled module info
        ucDisabledModule.ParentPanel = pnlDisabled;

        reportHeader.ActionPerformed += reportHeader_ActionPerformed;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        LoadAttributes();

        ucDisplayReport.ReloadData(false);
    }

    #endregion


    #region "Private methods"

    private void reportHeader_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                LoadAttributes();

                mIsSaved = true;

                // Check 'SaveReports' permission
                if (!CurrentUser.IsAuthorizedPerResource("cms.reporting", "SaveReports"))
                {
                    RedirectToAccessDenied("cms.reporting", "SaveReports");
                }

                // Save report
                if (ucDisplayReport.SaveReport() > 0)
                {
                    ShowConfirmation(String.Format(GetString("Ecommerce_Report.ReportSavedTo"), ucDisplayReport.ReportName + " - " + DateTime.Now.ToString()));
                }

                mIsSaved = false;
                break;
        }
    }


    private void LoadAttributes()
    {
        if (mParamsLoaded)
        {
            return;
        }

        if (RequestHelper.IsPostBack() && !IsValidInterval())
        {
            ShowError(GetString("analt.invalidinterval"));
            return;
        }

        string reportCodeName = QueryHelper.GetString("reportCodeName", String.Empty);


        if (!reportCodeName.Contains(";"))
        {
            ValidateReportCategory(reportCodeName);
            ucDisplayReport.ReportName = reportCodeName;
            ucGraphTypePeriod.GraphTypeVisible = false;
        }
        else
        {
            var reportName = ucGraphTypePeriod.GetReportName(reportCodeName);
            ValidateReportCategory(reportName);
            ucDisplayReport.ReportName = reportName;
        }

        ucGraphTypePeriod.ProcessChartSelectors(false);

        // Prepare report parameters
        DataTable dtp = new DataTable();

        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add(QueryHelper.GetString("parameterName", String.Empty), typeof(int));

        object[] parameters = new object[3];

        parameters[0] = ucGraphTypePeriod.From;
        parameters[1] = ucGraphTypePeriod.To;
        parameters[2] = QueryHelper.GetInteger("parameterValue", 0);

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();

        ucDisplayReport.LoadFormParameters = false;
        ucDisplayReport.DisplayFilter = false;
        ucDisplayReport.ReportParameters = dtp.Rows[0];
        ucDisplayReport.UseExternalReload = true;
        ucDisplayReport.UseProgressIndicator = true;

        reportHeader.ReportName = ucDisplayReport.ReportName;
        reportHeader.ReportParameters = ucDisplayReport.ReportParameters;
        reportHeader.SelectedInterval = ucGraphTypePeriod.SelectedInterval;

        mParamsLoaded = true;
    }


    private void ValidateReportCategory(string reportCodeName)
    {
        if (!IsBannerManagementReport(reportCodeName))
        {
            RedirectToAccessDenied(GetString("accessdenied.notallowedtoread"));
        }
    }


    private bool IsBannerManagementReport(string reportCodeName)
    {
        var report = ReportInfoProvider.GetReportInfo(reportCodeName);
        if (report != null)
        {
            var category = ReportCategoryInfoProvider.GetReportCategoryInfo(report.ReportCategoryID);
            if (category != null)
            {
                if (!category.CategoryPath.StartsWith("/BannerManagement", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Returns true if selected interval is valid.
    /// </summary>
    private bool IsValidInterval()
    {
        var from = ucGraphTypePeriod.From;
        var to = ucGraphTypePeriod.To;

        if ((from == DateTimeHelper.ZERO_TIME) || (to == DateTimeHelper.ZERO_TIME))
        {
            return false;
        }

        return from <= to;
    }

    #endregion
}