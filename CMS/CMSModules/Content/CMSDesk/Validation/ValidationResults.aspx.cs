using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_Validation_ValidationResults : CMSPage
{
    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash"))
        {
            RedirectToAccessDenied(GetString("dialogs.badhashtitle"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int documentId = QueryHelper.GetInteger("docid", 0);
        string titlePart = null;
        if (documentId > 0)
        {
            TreeNode doc = DocumentHelper.GetDocument(documentId, null);
            if (doc != null)
            {
                titlePart = HTMLHelper.HTMLEncode(doc.GetDocumentName());
            }
        }

        SetTitle(String.Format(GetString("validation.validationdialogresults"), titlePart));

        string key = QueryHelper.GetString("datakey", null);
        if (!String.IsNullOrEmpty(key))
        {
            viewDataSet.AdditionalContent = "";
            DataSet ds = WindowHelper.GetItem(key) as DataSet;
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                viewDataSet.DataSet = ds;
            }
        }

        ScriptHelper.RegisterDialogScript(Page);
        RegisterModalPageScripts();
    }

    #endregion
}
