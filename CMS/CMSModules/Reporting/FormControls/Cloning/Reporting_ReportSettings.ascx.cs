using System;
using System.Collections;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Reporting;
using CMS.UIControls;


public partial class CMSModules_Reporting_FormControls_Cloning_Reporting_ReportSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Returns close script which should be run when cloning is sucessfully done.
    /// </summary>
    public override string CloseScript
    {
        get
        {
            string script = String.Empty;
            string refreshLink = UIContextHelper.GetElementUrl(ModuleName.REPORTING, "reporting.reporting", false);
            refreshLink = URLHelper.AddParameterToUrl(refreshLink, "objectid", "{0}");

            if (QueryHelper.Contains("reloadall"))
            {
                script = "wopener.location = '" + refreshLink + "';";
            }
            else
            {
                script += "wopener.parent.parent.location.href ='" + refreshLink + "';";
            }
            script += "CloseDialog();";

            return script;
        }
    }
    

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return SavedReportInfo.OBJECT_TYPE + ";" + ReportSubscriptionInfo.OBJECT_TYPE;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblCategory.ToolTip = GetString("clonning.settings.report.category.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            ReportInfo ri = InfoToClone as ReportInfo;
            if (ri != null)
            {
                categorySelector.Value = ri.ReportCategoryID.ToString();
            }
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[ReportInfo.OBJECT_TYPE + ".categoryid"] = categorySelector.Value;
        result[ReportInfo.OBJECT_TYPE + ".savedreports"] = chkCloneSavedReports.Checked;
        result[ReportInfo.OBJECT_TYPE + ".subscriptions"] = chkSubscriptions.Checked;
        return result;
    }

    #endregion
}