using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("abuse.properties")]
[Breadcrumb(0, "abuse.reports", "~/CMSModules/AbuseReport/AbuseReport_List.aspx", null)]
[UIElement(ModuleName.ABUSEREPORT, "AbuseReport")]
public partial class CMSModules_AbuseReport_AbuseReport_Edit : CMSAbuseReportPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Ensure modal dialog opening
        ScriptHelper.RegisterDialogScript(this);

        int reportID = QueryHelper.GetInteger("reportid", 0);
        ucAbuseEdit.ReportID = reportID;

        AbuseReportInfo ar = AbuseReportInfoProvider.GetAbuseReportInfo(reportID);
                
        // Set edited object
        EditedObject = ar;

        if (ar != null)
        {
            // Set breadcrumb
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = ar.ReportTitle
            });

            // Ensure that object belongs to current site or user is global admin 
            if(!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && (ar.ReportSiteID != SiteContext.CurrentSiteID)) 
            {
                RedirectToInformation(GetString("general.notassigned"));
            }
        }

        ucAbuseEdit.OnCheckPermissions += ucAbuseEdit_OnCheckPermissions;
    }


    /// <summary>
    /// Check permission.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="sender">Sender</param>
    private void ucAbuseEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.AbuseReport", permissionType))
        {
            sender.StopProcessing = true;
            RedirectToAccessDenied("CMS.AbuseReport", permissionType);
        }
    }

    #endregion
}
