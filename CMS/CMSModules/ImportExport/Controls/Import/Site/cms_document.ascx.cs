using System;
using System.Collections.Generic;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Import_Site_cms_document : ImportExportControl
{
    /// <summary>
    /// True if import into existing site.
    /// </summary>
    protected bool ExistingSite
    {
        get
        {
            if (Settings != null)
            {
                return ((SiteImportSettings)Settings).ExistingSite;
            }
            return true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        chkDocuments.Text = GetString("ImportDocuments.ImportDocuments");
        chkDocumentsHistory.Text = GetString("ImportDocuments.ImportDocumentsHistory") + "<br />";
        chkRelationships.Text = GetString("ImportDocuments.ImportRelationships") + "<br />";
        chkACLs.Text = GetString("ImportDocuments.ImportACLs") + "<br />";

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
        chkDocumentsHistory.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_HISTORY), true);

        chkRelationships.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_RELATIONSHIPS), true);
        chkACLs.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_DOC_ACLS), true);

        // Visibility
        SiteImportSettings settings = (SiteImportSettings)Settings;

        chkACLs.Visible = settings.IsIncluded(AclInfo.OBJECT_TYPE, false);
        chkDocumentsHistory.Visible = settings.IsIncluded(VersionHistoryInfo.OBJECT_TYPE, false);
        chkRelationships.Visible = settings.IsIncluded(RelationshipInfo.OBJECT_TYPE, false);
        pnlDocumentData.Visible = chkDocumentsHistory.Visible || chkACLs.Visible || chkRelationships.Visible;
    }
}
