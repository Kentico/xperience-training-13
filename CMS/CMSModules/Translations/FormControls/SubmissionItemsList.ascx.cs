using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_Translations_FormControls_SubmissionItemsList : FormEngineUserControl
{
    #region "Variables"

    private int mSubmissionID;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of the submission from which to display the items.
    /// </summary>
    public override object Value
    {
        get
        {
            return SubmissionID;
        }
        set
        {
            SubmissionID = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// ID of the submission from which to display the items.
    /// </summary>
    public int SubmissionID
    {
        get
        {
            return mSubmissionID;
        }
        set
        {
            mSubmissionID = value;
            gridElem.WhereCondition = "SubmissionItemSubmissionID = " + value;
        }
    }


    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


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
            gridElem.StopProcessing = value;
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
            gridElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.WhereCondition = "SubmissionItemSubmissionID = " + SubmissionID;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

        btnImportFromZip.OnClientClick = "ShowUploadDialog(" + SubmissionID + ", 0);";
        btnExportToZip.OnClientClick = "window.open('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Translations/CMSPages/DownloadTranslation.aspx?submissionid=" + SubmissionID) + "'); return false;";


        string script = "function ShowUploadDialog(submissionId, submissionItemTd) { modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Translations/Pages/UploadTranslation.aspx") + "?itemid=' + submissionItemTd + '&submissionid=' + submissionId, 'Upload translation', 500, 225); }";
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "ShowUploadDialog", script, true);
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe() == "downloadxliff")
        {
            CMSGridActionButton btn = (CMSGridActionButton)sender;
            btn.OnClientClick = "window.open('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Translations/CMSPages/DownloadTranslation.aspx?itemid=" + ((DataRowView)((GridViewRow)parameter).DataItem)[0]) + "'); return false;";
            return btn;
        }
        return parameter;
    }

    #endregion
}