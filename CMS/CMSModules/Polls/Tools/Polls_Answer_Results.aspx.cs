using System;
using System.Linq;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.Modules;
using CMS.OnlineForms;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
public partial class CMSModules_Polls_Tools_Polls_Answer_Results : CMSModalPage
{
    #region "Variables"

    private CMSAdminControl bizFormData = null;

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Check license
        if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.Polls);
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Polls", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Polls");
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Form", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Form");
        }

        var user = MembershipContext.AuthenticatedUser;

        // Check permissions for CMS Desk -> Tools -> Polls
        if (!user.IsAuthorizedPerUIElement("CMS.Polls", "Polls"))
        {
            RedirectToUIElementAccessDenied("CMS", "Polls");
        }

        // Check permissions for site polls
        if (!user.IsAuthorizedPerResource("CMS.Polls", CMSAdminControl.PERMISSION_READ))
        {
            RedirectToAccessDenied("CMS.Polls", "Read");
        }

        // Check permissions for forms
        if (!user.IsAuthorizedPerResource("CMS.Form", "ReadData"))
        {
            RedirectToAccessDenied("CMS.Form", "ReadData");
        }
        
        // Load BizForm selector if BizForms module is available
        if (ModuleManager.IsModuleLoaded(ModuleName.BIZFORM) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleName.BIZFORM, SiteContext.CurrentSiteName))
        {
            bizFormData = this.LoadUserControl("~/CMSModules/BizForms/Controls/BizFormEditData.ascx") as CMSAdminControl;
            bizFormData.ShortID = "bizFormData";
            bizFormData.SetValue("ShowNewRecordButton", false);
            plcBizForm.Controls.Add(bizFormData);
            bizFormData.Visible = true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("polls_answer_records.title");
    }

    #endregion
}