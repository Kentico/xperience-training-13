using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Reporting;
using CMS.UIControls;


[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditProcess;EditProcessReport")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Report : CMSAutomationPage
{
    #region "Variables"

    private bool mSaving;
    int processId = QueryHelper.GetInteger("processid", 0);

    #endregion


    #region "Event handlers"
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AuthorizedForContacts)
        {
            // User has no permissions
            RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Read");
        }

        // Set report parameters
        ReportInfo report = ReportInfoProvider.GetReportInfo("Number_of_contacts_in_steps");
        if (report != null)
        {
            ucReport.ReportName = report.ReportName;
            reportHeader.ReportName = report.ReportName;
            
            ucReport.DisplayFilter = false;
            ucReport.LoadFormParameters = false;

            string parameterString = String.Format("AutomationProcessID;{0};", processId);
            DataRow parameters = ReportHelper.GetReportParameters(report, parameterString, null, CultureHelper.EnglishCulture);
            ucReport.ReportParameters = parameters;
            reportHeader.ReportParameters = parameters;

            reportHeader.ActionPerformed += reportHeader_ActionPerformed;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Create refresh action
        reportHeader.HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("general.refresh"),
            RedirectUrl = "Tab_Report.aspx?processid=" + processId
        });
    }


    private void reportHeader_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                // Check 'SaveReports' permission
                if (!CurrentUser.IsAuthorizedPerResource("cms.reporting", "SaveReports"))
                {
                    RedirectToAccessDenied("cms.reporting", "SaveReports");
                }

                mSaving = true;

                if (ucReport.SaveReport() > 0)
                {
                    ShowConfirmation(String.Format(GetString("reporting.reportsavedto"), ucReport.ReportDisplayName + " - " + DateTime.Now));
                }
                else
                {
                    ShowError(GetString("reporting.savingreportfailed"));
                }

                mSaving = false;
                break;
        }
    }


    /// <summary>
    /// Verify rendering in server form only when not saving report.
    /// </summary>
    /// <param name="control">Control</param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!mSaving)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }

    #endregion
}