using System;

using CMS.Base.Web.UI;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


[EditedObject(EmailTemplateInfo.OBJECT_TYPE, "templateid")]
public partial class CMSModules_EmailTemplates_Pages_Tab_General : CMSEmailTemplatesPage
{
    #region "Variables"

    private bool isDialog;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        isDialog = QueryHelper.GetBoolean("editonlycode", false);
        if (isDialog)
        {
            // Check hash
            var settings = new HashSettings("")
            {
                Redirect = false
            };

            if (!QueryHelper.ValidateHash("hash", "saved;name;templateid;selectorid;tabmode;siteid;selectedsiteid", settings, true))
            {
                URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
            }

            string templateName = QueryHelper.GetString("name", String.Empty);
            EmailTemplateInfo templateInfo = EmailTemplateProvider.GetEmailTemplate(templateName, SiteID);
            if (templateInfo != null)
            {
                EditedObject = templateInfo;
            }

            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
        }

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (isDialog)
        {
            RegisterEscScript();
            RegisterModalPageScripts();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        InitActions();
    }

    #endregion


    #region "Custom methods"

    protected void InitActions()
    {
        EmailTemplateInfo emailTemplate = (EmailTemplateInfo)EditedObject;

        if ((emailTemplate != null) && (emailTemplate.TemplateID > 0))
        {
            ObjectEditMenu menu = (ObjectEditMenu)ControlsHelper.GetChildControl(Page, typeof(ObjectEditMenu));
            if (menu != null)
            {
                EmailTemplateEditExtender.RegisterEditPageHeaderActions(Page, menu, emailTemplate);
            }
        }
    }

    #endregion
}
