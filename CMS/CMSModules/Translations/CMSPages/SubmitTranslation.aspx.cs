using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.TranslationServices;
using CMS.UIControls;


[Title("translationservice.uploadtranslation")]
public partial class CMSModules_Translations_CMSPages_SubmitTranslation : CMSLiveModalPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check the license
        if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.TranslationServices);
        }

        if (!QueryHelper.ValidateHash("hash", null, new HashSettings("")))
        {
            ShowError(GetString("general.badhashtext"));
            uploadElem.StopProcessing = true;
            uploadElem.Visible = false;
            return;
        }

        int submissionID = QueryHelper.GetInteger("submissionid", 0);

        TranslationSubmissionInfo submissionInfo = TranslationSubmissionInfoProvider.GetTranslationSubmissionInfo(submissionID);
        if (submissionInfo != null)
        {
            EditedObject = submissionInfo;
            bool allowUpload = true;

            // Show information about submission status
            switch (submissionInfo.SubmissionStatus)
            {
                case TranslationStatusEnum.TranslationCanceled:
                    ShowInformation(String.Format(GetString("translationservice.submit.submissioncanceled"), submissionInfo.SubmissionName));
                    allowUpload = false;
                    break;

                case TranslationStatusEnum.TranslationCompleted:
                    ShowInformation(String.Format(GetString("translationservice.submissioncompleted"), submissionInfo.SubmissionName));
                    allowUpload = false;
                    break;

                case TranslationStatusEnum.ResubmittingSubmission:
                case TranslationStatusEnum.ProcessingSubmission:
                    ShowInformation(String.Format(GetString("translationservice.submissionactive"), submissionInfo.SubmissionName));
                    allowUpload = false;
                    break;
            }

            if (!allowUpload)
            {
                // Disable uploader
                uploadElem.Visible = false;
                uploadElem.StopProcessing = true;
                btnUpload.Visible = false;
            }
            else
            {
                // Initialize uploader
                uploadElem.SubmissionID = QueryHelper.GetInteger("submissionid", 0);
                uploadElem.SubmissionItemID = QueryHelper.GetInteger("itemid", 0);
                btnUpload.Click += btnUpload_Click;
            }
        }
        else
        {
            ShowInformation(GetString("translationservice.submit.submissiondeleted"));
            // Disable uploader
            uploadElem.Visible = false;
            uploadElem.StopProcessing = true;
            btnUpload.Visible = false;
        }
    }


    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (uploadElem.UploadTranslation())
        {
            ShowInformation(GetString("translationservice.translationsuploaded"));

            uploadElem.Visible = false;
            btnUpload.Visible = false;
        }
    }

    #endregion
}