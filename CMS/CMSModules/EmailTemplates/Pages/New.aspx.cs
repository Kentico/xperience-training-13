using System;

using CMS.EmailEngine;
using CMS.Helpers;
using CMS.UIControls;


[Title("emailtemplate_edit.dialog.titlenew")]
public partial class CMSModules_EmailTemplates_Pages_New : CMSEmailTemplatesPage
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

            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
        }

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.SiteId = SiteID;
        editElem.SelectedSiteId = SelectedSiteID;

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

            var master = CurrentMaster as ICMSModalMasterPage;
            if (master != null)
            {
                // Register the Save and close button as the form submit button
                HeaderActions.Visible = false;
                editElem.EditForm.SubmitButton.Visible = false;
                master.Save += (s, ea) => editElem.EditForm.SaveData(null);
                master.ShowSaveAndCloseButton();
                master.SetSaveResourceString("general.create");
            }
        }
    }

    #endregion
}