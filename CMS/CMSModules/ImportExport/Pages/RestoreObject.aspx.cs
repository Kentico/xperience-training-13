using System;
using System.Collections.Generic;
using System.Security.Principal;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Pages_RestoreObject : CMSModalPage
{
    #region "Variables"

    protected string codeName = null;
    protected string exportObjectDisplayName = null;

    protected string targetFolder = null;

    protected bool allowDependent = false;
    protected bool siteObject = false;
    protected int siteId = 0;
    protected int objectId = 0;
    protected string objectType = string.Empty;

    protected GeneralizedInfo infoObj = null;
    protected GeneralizedInfo exportObj = null;

    protected bool backup = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Export process GUID.
    /// </summary>
    public Guid ProcessGUID
    {
        get
        {
            if (ViewState["ProcessGUID"] == null)
            {
                ViewState["ProcessGUID"] = Guid.NewGuid();
            }

            return ValidationHelper.GetGuid(ViewState["ProcessGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Persistent settings key.
    /// </summary>
    public string PersistentSettingsKey
    {
        get
        {
            return "RestoreObject_" + ProcessGUID + "_Settings";
        }
    }


    /// <summary>
    /// Import settings stored in viewstate.
    /// </summary>
    public SiteImportSettings ImportSettings
    {
        get
        {
            SiteImportSettings settings = (SiteImportSettings)PersistentStorageHelper.GetValue(PersistentSettingsKey);
            if (settings == null)
            {
                throw new Exception("[ImportObject.ImportSettings]: Import settings has been lost.");
            }
            return settings;
        }
        set
        {
            PersistentStorageHelper.SetValue(PersistentSettingsKey, value);
        }
    }

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        if (!DebugHelper.DebugImportExport)
        {
            DisableDebugging();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        var currentSiteName = SiteContext.CurrentSiteName;

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.globalpermissions", "RestoreObjects", currentSiteName))
        {
            RedirectToAccessDenied("cms.globalpermissions", "RestoreObjects");
        }

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Async control events binding
        ucAsyncControl.OnFinished += ucAsyncControl_OnFinished;
        ucAsyncControl.OnError += ucAsyncControl_OnError;

        if (!RequestHelper.IsCallback())
        {
            try
            {
                // Delete temporary files
                ExportProvider.DeleteTemporaryFiles();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }

            SetTitle(GetString("RestoreObject.Title"));

            // Get data from parameters
            siteId = QueryHelper.GetInteger("siteId", 0);
            objectId = QueryHelper.GetInteger("objectId", 0);
            objectType = QueryHelper.GetString("objectType", "");

            // Get the object
            infoObj = ModuleManager.GetReadOnlyObject(objectType);
            if (infoObj == null)
            {
                lblIntro.Text = GetString("ExportObject.ObjectTypeNotFound");
                lblIntro.CssClass = "ErrorLabel";
                return;
            }

            // Get exported object
            exportObj = infoObj.GetObject(objectId);
            if (exportObj == null)
            {
                lblIntro.Text = GetString("ExportObject.ObjectNotFound");
                lblIntro.CssClass = "ErrorLabel";
                btnRestore.Visible = false;
                return;
            }

            if (!CheckSpecialSecurityCases(exportObj))
            {
                return;
            }

            if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                if (!exportObj.CheckPermissions(PermissionsEnum.Modify, CurrentSiteName, CurrentUser))
                {
                    RedirectToAccessDenied(exportObj.TypeInfo.ModuleName, PermissionsEnum.Modify.ToString());
                }
            }
            
            // Store display name
            exportObjectDisplayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(exportObj.ObjectDisplayName));
            codeName = exportObj.ObjectCodeName;

            lblIntro.Text = string.Format(GetString("RestoreObject.Intro"), exportObjectDisplayName);

            targetFolder = ImportExportHelper.GetObjectBackupFolder(exportObj);

            btnRestore.Click += btnRestore_Click;

            if (!RequestHelper.IsPostBack())
            {
                lblIntro.Visible = true;

                // Load the available backups
                if (lstImports.Items.Count == 0)
                {
                    RefreshPackageList();
                }
            }
        }
    }


    private bool CheckSpecialSecurityCases(GeneralizedInfo restoredObject)
    {
        var userInfo = restoredObject.MainObject as UserInfo;
        if (userInfo != null)
        {
            // User is able to perform rollback of users with the same or lower privilege level
            if (userInfo.SiteIndependentPrivilegeLevel > CurrentUser.SiteIndependentPrivilegeLevel)
            {
                lblIntro.Text = GetString("RestoreObject.Users.HigherPrivilegeLevel");
                lblIntro.CssClass = "ErrorLabel";
                btnRestore.Visible = false;
                return false;
            }
        }

        return true;
    }


    private void RefreshPackageList()
    {
        lstImports.Items.Clear();

        List<string> files = null;

        // Get import packages
        try
        {
            files = ImportProvider.GetImportFilesList(targetFolder);
        }
        catch
        {
            // Show error and log exception
            lblError.Text = GetString("importconfiguration.securityerror").Replace("{0}", ImportExportHelper.EXPORT_BACKUP_PATH);
        }

        if ((files != null) && (files.Count != 0))
        {
            lstImports.Enabled = true;
            lstImports.DataSource = files;
            lstImports.DataBind();

            lstImports.SelectedIndex = 0;
        }
        else
        {
            lstImports.Enabled = false;
        }

        // Select first item
        if (lstImports.Items.Count > 0)
        {
            lstImports.SelectedIndex = 0;
            btnDelete.OnClientClick = "if (!confirm(" + ScriptHelper.GetLocalizedString("importconfiguration.deleteconf") + ")) { return false;}";
        }
        else
        {
            btnDelete.OnClientClick = "";
        }
    }


    /// <summary>
    /// Gets the path to the selected file
    /// </summary>
    private string GetSelectedFilePath()
    {
        if (String.IsNullOrEmpty(lstImports.SelectedValue))
        {
            return null;
        }

        return Server.MapPath(targetFolder + "/" + lstImports.SelectedValue);
    }


    private void btnRestore_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(lstImports.SelectedValue))
        {
            siteObject = (exportObj.ObjectSiteID > 0);

            // Prepare the settings
            ImportSettings = new SiteImportSettings(MembershipContext.AuthenticatedUser);

            ImportSettings.UseAutomaticSiteForTranslation = true;
            ImportSettings.LogSynchronization = true;
            ImportSettings.WebsitePath = Server.MapPath("~/");
            ImportSettings.SourceFilePath = GetSelectedFilePath();

            if (siteObject)
            {
                ImportSettings.SiteId = exportObj.ObjectSiteID;
                ImportSettings.ExistingSite = true;

                // Do not update site definition when restoring a single object
                ImportSettings.SetSettings(ImportExportHelper.SETTINGS_UPDATE_SITE_DEFINITION, false);
            }

            // Set the filename
            lblProgress.Text = string.Format(GetString("RestoreObject.RestoreProgress"), exportObjectDisplayName);

            pnlDetails.Visible = false;
            btnRestore.Enabled = false;
            pnlProgress.Visible = true;

            try
            {
                // Export the data
                ScriptHelper.RegisterStartupScript(this, typeof(string), "StartTimer", "StartTimer();", true);
                ucAsyncControl.RunAsync(RestoreSingleObject, WindowsIdentity.GetCurrent());
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }


    private void DisplayError(Exception ex)
    {
        pnlProgress.Visible = false;
        btnRestore.Enabled = false;
        pnlDetails.Visible = false;

        string displayName = null;
        if (exportObj != null)
        {
            displayName = exportObj.ObjectDisplayName;
        }

        pnlContent.Visible = true;
        lblResult.Text = string.Format(GetString("RestoreObject.Error"), HTMLHelper.HTMLEncode(displayName));
        lblResult.CssClass = "ErrorLabel";

        // Log to the event log
        Service.Resolve<IEventLogService>().LogException("Import", "RestoreObject", ex);
    }


    /// <summary>
    /// Restores the single object.
    /// </summary>
    private void RestoreSingleObject(object parameter)
    {
        // Restore the entire content of the package
        ImportProvider.CreateTemporaryFiles(ImportSettings);

        ImportSettings.LoadDefaultSelection();

        ImportProvider.ImportObjectsData(ImportSettings);
    }


    private void ucAsyncControl_OnError(object sender, EventArgs e)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), "StopTimer", "StopTimer();", true);
        Exception ex = ((AsyncControl)sender).Worker.LastException;

        DisplayError(ex);
    }


    private void ucAsyncControl_OnFinished(object sender, EventArgs e)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), "StopTimer", "StopTimer();", true);

        // Reload unigrid
        var unigridID = QueryHelper.GetText("ug", String.Empty);
        if (!String.IsNullOrEmpty(unigridID))
        {
            var reloadScript = @"
if(wopener && wopener.CMS) {
    var ug = wopener.CMS." + unigridID + @";
    if(ug && ug.reload) {
        ug.reload();
    }
}
";
            ScriptHelper.RegisterStartupScript(this, typeof(string), "ReloadUnigrid", reloadScript, true);
        }

        pnlProgress.Visible = false;
        btnRestore.Visible = false;
        pnlContent.Visible = true;

        lblResult.CssClass = "ContentLabel";
        lblResult.Text = string.Format(GetString("RestoreObject.RestoreFinished"), exportObjectDisplayName);
    }


    protected override void OnPreRender(EventArgs e)
    {
        lblResult.Visible = (lblResult.Text != "");
        base.OnPreRender(e);
    }


    /// <summary>
    /// Refresh button click handler.
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(lstImports.SelectedValue))
        {
            try
            {
                string path = GetSelectedFilePath();
                File.Delete(path);
            }
            catch (Exception ex)
            {
                ScriptHelper.RegisterStartupScript(Page, typeof(string), "ErrorMessage", ScriptHelper.GetAlertScript(ex.Message));
            }
            finally
            {
                RefreshPackageList();
            }
        }
    }
}
