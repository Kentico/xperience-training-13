using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.UIControls;


// Edited object
[EditedObject(ReportSubscriptionInfo.OBJECT_TYPE, "subscriptionID")]

// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "Report_Header.Subscriptions", "list.aspx?reportID={?reportID?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "reportsubscription.new", NewObject = true)]
public partial class CMSModules_Reporting_Tools_Subscription_Edit : CMSReportingPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.HeaderActions.AddAction(new SaveAction());
        CurrentMaster.HeaderActions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);
    }


    void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                editElem.Save();
                break;
        }
    }
}
