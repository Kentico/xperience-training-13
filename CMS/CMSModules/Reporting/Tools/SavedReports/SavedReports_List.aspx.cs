using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.UIControls;


[UIElement("CMS.Reporting", "SavedReports")]
public partial class CMSModules_Reporting_Tools_SavedReports_SavedReports_List : CMSReportingPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        UniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        UniGrid.WhereCondition = createWhereCondition();
    }


    /// <summary>
    /// Creates where condition for unigrid.
    /// </summary>
    private string createWhereCondition()
    {
        string condition = "";

        int reportId = QueryHelper.GetInteger("reportId", 0);

        if (reportId != 0)
        {
            condition += "SavedReportReportId = " + reportId;
        }

        return condition;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl("SavedReport_View.aspx?reportId=" + Convert.ToString(actionArgument)));
        }
        else if (actionName == "delete")
        {
            // Check 'Modify' permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "Modify"))
            {
                RedirectToAccessDenied("cms.reporting", "Modify");
            }
            // delete ReportInfo object from database
            SavedReportInfoProvider.DeleteSavedReportInfo(Convert.ToInt32(actionArgument));
        }
    }
}