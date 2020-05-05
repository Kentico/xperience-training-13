using System;
using System.Web;

using CMS.Helpers;
using CMS.TranslationServices;
using CMS.TranslationServices.Web.UI;


public partial class CMSModules_Translations_CMSPages_DownloadTranslation : CMSTranslationServicePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int submissionId = QueryHelper.GetInteger("submissionid", 0);
        int itemId = QueryHelper.GetInteger("itemid", 0);
        if (submissionId > 0)
        {
            TranslationSubmissionInfo info = TranslationSubmissionInfoProvider.GetTranslationSubmissionInfo(submissionId);
            if (info != null)
            {
                TranslationServiceHelper.DownloadXLIFFinZIP(info, HttpContext.Current.Response);
            }
        }
        else if (itemId > 0)
        {
            TranslationSubmissionItemInfo info = TranslationSubmissionItemInfoProvider.GetTranslationSubmissionItemInfo(itemId);
            if (info != null)
            {
                TranslationServiceHelper.DownloadXLIFF(info, Response);
            }
        }
    }
}