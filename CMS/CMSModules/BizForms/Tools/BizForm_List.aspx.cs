using System;

using CMS.Base;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.OnlineForms.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement("CMS.Form", "Form")]
public partial class CMSModules_BizForms_Tools_BizForm_List : CMSBizFormPage
{
    private CurrentUserInfo currentUser;


    protected void Page_Load(object sender, EventArgs e)
    {
        currentUser = MembershipContext.AuthenticatedUser;

        if (currentUser == null)
        {
            return;
        }

        // Check 'ReadForm' permission
        if (!currentUser.IsAuthorizedPerResource("cms.form", "ReadForm"))
        {
            RedirectToAccessDenied("cms.form", "ReadForm");
        }

        UniGridBizForms.OnAction += UniGridBizForms_OnAction;
        UniGridBizForms.HideControlForZeroRows = false;
        UniGridBizForms.ZeroRowsText = GetString("general.nodatafound");
        UniGridBizForms.WhereCondition = GetUniGridWhereCondition();

        PageTitle.TitleText = GetString("BizFormList.TitleText");

        InitHeaderActions();
    }


    private string GetUniGridWhereCondition()
    {
        // Global administrators can see all forms. 
        if (currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            return "FormSiteID = " + SiteContext.CurrentSiteID;
        }

        var bizFormsAvailableForUser = BizFormInfo.Provider.Get()
                                                          .Column("CMS_Form.FormID")
                                                          .OnSite(SiteContext.CurrentSiteID)
                                                          .Distinct()
                                                          .Source(s => s.LeftJoin<BizFormRoleInfo>("FormID", "FormID"))
                                                          .Where(new WhereCondition()
                                                              .WhereIn("RoleID", UserRoleInfo.Provider.Get().Column("RoleID").WhereEquals("UserID", currentUser.UserID))
                                                              .Or()
                                                              .WhereNull("FormAccess")
                                                              .Or()
                                                              .WhereEquals("FormAccess", (int)FormAccessEnum.AllBizFormUsers));

        return new WhereCondition().WhereIn("CMS_Form.FormID", bizFormsAvailableForUser).ToString(true);
    }


    private void InitHeaderActions()
    {
        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("BizFormList.lnkNewBizForm"),
            Tooltip = GetString("BizFormList.lnkNewBizForm"),
            RedirectUrl = "BizForm_New.aspx",
            ResourceName = "cms.form",
            Permission = "CreateForm"
        });
    }


    protected void UniGridBizForms_OnAction(string actionName, object actionArgument)
    {
        if (DataHelper.GetNotEmpty(actionName, String.Empty) == "edit")
        {
            URLHelper.Redirect(UIContextHelper.GetElementUrl("CMS.Form", "Forms.Properties", false, Convert.ToInt32(actionArgument)));
        }
        if (DataHelper.GetNotEmpty(actionName, "") == "delete")
        {
            // Check 'DeleteFormAndData' permission
            if (!currentUser.IsAuthorizedPerResource("cms.form", "DeleteFormAndData"))
            {
                RedirectToAccessDenied("cms.form", "DeleteFormAndData");
            }

            BizFormInfo.Provider.Get(ValidationHelper.GetInteger(actionArgument, 0))?.Delete();
        }
    }
}