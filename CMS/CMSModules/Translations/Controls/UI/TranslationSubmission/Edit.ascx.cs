using System;

using CMS.DataEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.TranslationServices;
using CMS.UIControls;


public partial class CMSModules_Translations_Controls_UI_TranslationSubmission_Edit : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl => EditForm;


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            EditForm.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        UIFormControl.SubmitButton.Visible = false;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        SetupUnsupportedFeaturesVisibility();
    }

    #endregion

    private void SetupUnsupportedFeaturesVisibility()
    {
        var submission = UIFormControl.EditedObject as TranslationSubmissionInfo;
        if (submission == null)
        {
            return;
        }

        var service = TranslationServiceInfo.Provider.Get(submission.SubmissionServiceID);
        if (service == null)
        {
            return;
        }

        if (!service.TranslationServiceSupportsPriority)
        {
            UIFormControl.FieldsToHide.Add("SubmissionPriority");
        }

        if (!service.TranslationServiceSupportsDeadline)
        {
            UIFormControl.FieldsToHide.Add("SubmissionDeadline");
        }

        if (!service.TranslationServiceSupportsInstructions)
        {
            UIFormControl.FieldsToHide.Add("SubmissionInstructions");
        }

        if (!SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSAllowAttachmentTranslation"))
        {
            UIFormControl.FieldsToHide.Add("SubmissionTranslateAttachments");
        }

        UIFormControl.FieldControls["SubmissionStatus"].Enabled = service.TranslationServiceSupportsStatusUpdate;
    }
}

