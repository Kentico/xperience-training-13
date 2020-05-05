using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.TranslationServices.Web.UI;


public partial class CMSModules_Translations_Pages_UploadTranslation : CMSTranslationServiceModalPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        uploadElem.SubmissionID = QueryHelper.GetInteger("submissionid", 0);
        uploadElem.SubmissionItemID = QueryHelper.GetInteger("itemid", 0);
        uploadElem.CheckTranslationsPermissions = true;
        PageTitle.TitleText = GetString("translationservice.uploadtranslation");

        btnUpload.Click += btnUpload_Click;
    }


    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (uploadElem.UploadTranslation())
        {
            ltlScript.Text = ScriptHelper.GetScript("if (wopener && wopener.ShowUploadSuccess) { wopener.ShowUploadSuccess(); }; CloseDialog();");
        }
    }

    #endregion
}
