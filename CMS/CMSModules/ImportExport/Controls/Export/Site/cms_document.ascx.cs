using System;
using System.Collections.Generic;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Export_Site_cms_document : ImportExportControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        chkDocuments.Text = GetString("ExportDocuments.ExportDocuments");
        chkDocumentsHistory.Text = GetString("ExportDocuments.ExportDocumentsHistory");
        chkRelationships.Text = GetString("ExportDocuments.ExportRelationships");
        chkACLs.Text = GetString("ExportDocuments.ExportACLs");

        // Javascript
        string script = "var ed_parent = document.getElementById('" + chkDocuments.ClientID + "'); \n" +
                        "var childIDs = ['" + chkDocumentsHistory.ClientID + "', '" + chkRelationships.ClientID + "', '" + chkACLs.ClientID + "']; \n" +
                        "InitCheckboxes(); \n";

        ltlScript.Text = ScriptHelper.GetScript(script);

        chkDocuments.Attributes.Add("onclick", "CheckChange();");
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        if (chkDocuments.Checked)
        {
            Settings.SetObjectsProcessType(ProcessObjectEnum.All, TreeNode.OBJECT_TYPE, true);
            Settings.SetSelectedObjects(new List<string> { "/" }, TreeNode.OBJECT_TYPE, true);
        }
        else
        {
            Settings.SetObjectsProcessType(ProcessObjectEnum.None, TreeNode.OBJECT_TYPE, true);
            Settings.SetSelectedObjects(null, TreeNode.OBJECT_TYPE, true);
        }

        Settings.SetSettings(ImportExportHelper.SETTINGS_DOC_HISTORY, chkDocumentsHistory.Checked);

        Settings.SetSettings(ImportExportHelper.SETTINGS_DOC_RELATIONSHIPS, chkRelationships.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_DOC_ACLS, chkACLs.Checked);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        chkDocuments.Checked = (Settings.GetObjectsProcessType(TreeNode.OBJECT_TYPE, true) != ProcessObjectEnum.None);
        chkDocumentsHistory.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_HISTORY), false);

        chkRelationships.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_RELATIONSHIPS), false);
        chkACLs.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_ACLS), false);
    }
}
