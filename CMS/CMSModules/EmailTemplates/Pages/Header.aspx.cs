using System;

using CMS.EmailEngine;
using CMS.Helpers;
using CMS.UIControls;


[EditedObject(EmailTemplateInfo.OBJECT_TYPE, "templateid")]
[Title("EmailTemplate_Edit.Title")]
[Tabs("content")]
public partial class CMSModules_EmailTemplates_Pages_Header : CMSEmailTemplatesPage
{
    private bool isDialog = false;


    protected override void OnPreInit(EventArgs e)
    {
        isDialog = QueryHelper.GetBoolean("editonlycode", false);
        if (isDialog)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (isDialog)
        {
            string templateName = QueryHelper.GetString("name", String.Empty);
            EmailTemplateInfo emailTemplate = EmailTemplateProvider.GetEmailTemplate(templateName, SiteID);
            if (emailTemplate != null)
            {
                EditedObject = emailTemplate;
            }

            RegisterEscScript();
            RegisterModalPageScripts();
        }

        SetTab(0, GetString("general.general"), "Tab_General.aspx" + RequestContext.CurrentQueryString, String.Empty);
    }
}