using System;
using System.Text.RegularExpressions;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.UIControls;


[UIElement(ModuleName.REPORTING, "EditSavedReports")]
public partial class CMSModules_Reporting_Tools_SavedReports_SavedReport_View : CMSReportingPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var savedReportId = QueryHelper.GetInteger("reportId", 0);

        if ((!RequestHelper.IsPostBack()) && (QueryHelper.GetInteger("View", 0) == 1))
        {
            ltlScript.Text += ScriptHelper.GetScript("parent.SelTab(3,'','');");
        }

        string queryParams = String.Format("?reportid={0}", savedReportId);

        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("Report_View.Print"),
            OnClientClick = string.Format("myModalDialog('SavedReport_Print.aspx{0}&hash={1}', 'SavedReportPrint', 650, 700); return false;", queryParams, QueryHelper.GetHash(queryParams))
        });

        // Creates SavedReport obj
        SavedReportInfo sri = SavedReportInfoProvider.GetSavedReportInfo(savedReportId);
        if (sri != null)
        {
            ReportInfo ri = ReportInfoProvider.GetReportInfo(sri.SavedReportReportID);
            if (ri != null)
            {
                // Initialize pagetitle breadcrumbs
                PageBreadcrumbs.Items.Add(new BreadcrumbItem()
                {
                    Text = ri.ReportDisplayName,
                    RedirectUrl = URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl(ModuleName.REPORTING, "SavedReports", false, ri.ReportID), "reportid", ri.ReportID.ToString())
                });

                PageBreadcrumbs.Items.Add(new BreadcrumbItem()
                {
                    Text = sri.SavedReportTitle
                });
            }

            // Backward compatibility
            // Check whether url is relative and check whether url to get report graph page is correct
            string savedReportHtml = ReportInfoProvider.GetReportGraphUrlRegExp.Replace(sri.SavedReportHTML, RepairURL);
            ltlHtml.Text = HTMLHelper.ResolveUrls(savedReportHtml, ResolveUrl("~/"));
        }

        ScriptHelper.RegisterPrintDialogScript(this);
    }


    /// <summary>
    /// Repair URL if is with application path or if is not relative.
    /// </summary>
    /// <param name="m">Match</param>
    public string RepairURL(Match m)
    {
        return m.Groups[1].Value + "~/CMSModules/Reporting/CMSPages/GetReportGraph.aspx";
    }
}