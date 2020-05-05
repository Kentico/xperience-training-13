using System;
using System.Collections;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AbuseReport_CMSPages_ReportAbuse : CMSLiveModalPage
{
    #region "Variables"

    private Hashtable parameters;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string identifier = QueryHelper.GetString("params", null);
        parameters = (Hashtable)WindowHelper.GetItem(identifier);

        if (parameters != null)
        {
            // Initialize reporting control
            reportElem.ConfirmationText = HTMLHelper.HTMLEncode(ValidationHelper.GetString(parameters["confirmationtext"], string.Empty));
            reportElem.ReportTitle = HTMLHelper.HTMLEncode(ValidationHelper.GetString(parameters["reporttitle"], string.Empty));
            reportElem.ReportObjectID = ValidationHelper.GetInteger(parameters["reportobjectid"], 0);
            reportElem.ReportObjectType = ValidationHelper.GetString(parameters["reportobjecttype"], string.Empty);
            reportElem.ReportURL = ValidationHelper.GetString(parameters["reporturl"], string.Empty);
            reportElem.DisplayButtons = false;
            reportElem.BodyPanel.CssClass = "DialogAbuseBody";
            reportElem.ReportButton = btnReport;

            // Initialize buttons
            btnReport.Click += btnReport_Click;

            // Set title
            string reportDialogTitle = HTMLHelper.HTMLEncode(ValidationHelper.GetString(parameters["reportdialogtitle"], string.Empty));
            PageTitle.TitleText = reportDialogTitle;
            PageTitle.AlternateText = reportDialogTitle;
        }
        else
        {
            reportElem.Visible = false;
        }
    }

    #endregion


    protected void btnReport_Click(object sender, EventArgs e)
    {
        reportElem.PerformAction();
    }
}