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
        chkEventAttendees.Text = GetString("ImportDocuments.ImportEventAttendees") + "<br />";
        chkBlogComments.Text = GetString("ImportDocuments.ImportBlogComments") + "<br />";
        chkUserPersonalizations.Text = GetString("CMSImport_UserPersonalizations.ImportUserPersonalizations") + "<br />";

        // Javascript
        string script = "var ed_parent = document.getElementById('" + chkDocuments.ClientID + "'); \n" +
                        "var childIDs = ['" + chkDocumentsHistory.ClientID + "', '" + chkRelationships.ClientID + "', '" + chkBlogComments.ClientID + "', '" + chkEventAttendees.ClientID + "', '" + chkACLs.ClientID + "', '" + chkUserPersonalizations.ClientID + "']; \n" +
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
        Settings.SetSettings(ImportExportHelper.SETTINGS_EVENT_ATTENDEES, chkEventAttendees.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_BLOG_COMMENTS, chkBlogComments.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_USER_PERSONALIZATIONS, chkUserPersonalizations.Checked);
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
        chkEventAttendees.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_EVENT_ATTENDEES), true);
        chkBlogComments.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_BLOG_COMMENTS), true);
        chkUserPersonalizations.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_USER_PERSONALIZATIONS), !ExistingSite);

        // Visibility
        SiteImportSettings settings = (SiteImportSettings)Settings;

        chkACLs.Visible = settings.IsIncluded(AclInfo.OBJECT_TYPE, false);
        chkDocumentsHistory.Visible = settings.IsIncluded(VersionHistoryInfo.OBJECT_TYPE, false);
        chkRelationships.Visible = settings.IsIncluded(RelationshipInfo.OBJECT_TYPE, false);
        //this.chkUserPersonalizations.Visible = settings.IsIncluded(PortalObjectType.PERSONALIZATION, false);
        pnlDocumentData.Visible = chkDocumentsHistory.Visible || chkACLs.Visible || chkRelationships.Visible || chkUserPersonalizations.Visible;

        chkBlogComments.Visible = settings.IsIncluded(PredefinedObjectType.BLOGCOMMENT, false);
        chkEventAttendees.Visible = settings.IsIncluded(PredefinedObjectType.EVENTATTENDEE, false);
        pnlModules.Visible = chkBlogComments.Visible || chkEventAttendees.Visible;
    }
}
