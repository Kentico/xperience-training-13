using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Import___objects__ : ImportExportControl
{
    #region "Properties"

    /// <summary>
    /// Import settings.
    /// </summary>
    public SiteImportSettings ImportSettings
    {
        get
        {
            return (Settings as SiteImportSettings);
        }

        set
        {
            Settings = value;
        }
    }

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            return;
        }

        if (ImportSettings != null)
        {
            if (CheckVersion())
            {
                pnlWarning.Visible = true;
                lblWarning.Text = GetString("ImportObjects.WarningVersion");
            }
            else if (CheckHotfixVersion())
            {
                pnlWarning.Visible = true;
                lblWarning.Text = GetString("ImportObjects.WarningHotfixVersion");
            }

            pnlMacroResigning.Visible = !ImportSettings.IsNewSite;

            // If importing a new site which is not from template, preselect Current user as default macro re-sign user
            if (!ImportSettings.ExistingSite && !ImportSettings.IsNewSite)
            {
                userSelectorMacroResigningUser.Value = MembershipContext.AuthenticatedUser.UserID;
            }
        }

        // Confirmation for select all
        lnkSelectAll.OnClientClick = $"return confirm({ScriptHelper.GetString(ResHelper.GetString("importobjects.selectallconfirm"))});";

        chkCopyFiles.Attributes.Add("onclick", "CheckChange();");

        userSelectorMacroResigningUser.UniSelector.TextBoxSelect.AddCssClass("input-width-60");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            return;
        }

        // Javascript
        string script = $@"
var im_g_parent = document.getElementById('{chkCopyFiles.ClientID}');
var im_g_childIDs = ['{chkCopyGlobalFiles.ClientID}', '{chkCopySiteFiles.ClientID}', '{chkCopyCodeFiles.ClientID}','{chkCopyAssemblies.ClientID}'];
var im_g_childIDNames = ['gl', 'site', 'code', 'asbl'];
var im_g_isPrecompiled = {(SystemContext.IsPrecompiledWebsite ? "true" : "false")};
InitCheckboxes();
";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "ImportObjects" + ClientID, script, true);
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_DELETE_SITE, chkDeleteSite.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_ADD_SITE_BINDINGS, chkBindings.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_RUN_SITE, chkRunSite.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_UPDATE_SITE_DEFINITION, chkUpdateSite.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_SKIP_OBJECT_ON_TRANSLATION_ERROR, chkSkipOrfans.Checked);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_TASKS, chkImportTasks.Checked);

        ImportSettings.CopyFiles = chkCopyFiles.Checked;
        ImportSettings.CopyCodeFiles = ImportSettings.CopyFiles && chkCopyCodeFiles.Checked;

        // Copy files property is stronger
        bool copyGlobal = chkCopyFiles.Checked && chkCopyGlobalFiles.Checked;
        bool copyAssemblies = chkCopyFiles.Checked && chkCopyAssemblies.Checked;
        bool copySite = chkCopyFiles.Checked && chkCopySiteFiles.Checked;

        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_GLOBAL_FOLDERS, copyGlobal);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_ASSEMBLIES, copyAssemblies);
        ImportSettings.SetSettings(ImportExportHelper.SETTINGS_SITE_FOLDERS, copySite);

        ImportSettings.LogSynchronization = chkLogSync.Checked;
        ImportSettings.LogIntegration = chkLogInt.Checked;
        ImportSettings.RebuildSearchIndex = chkRebuildIndexes.Checked;

        var userId = (string) userSelectorMacroResigningUser.Value;
        if (!String.IsNullOrEmpty(userId))
        {
            var user = UserInfo.Provider.Get(Int32.Parse(userId));
            ImportSettings.RefreshMacroSecurity = (user != null);
            ImportSettings.MacroSecurityUser = user;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        if (ImportSettings != null)
        {
            bool singleObject = ValidationHelper.GetBoolean(ImportSettings.GetInfo(ImportExportHelper.INFO_SINGLE_OBJECT), false);

            chkCopyFiles.Checked = ImportSettings.CopyFiles;
            chkCopyCodeFiles.Checked = ImportSettings.CopyCodeFiles;
            chkCopyGlobalFiles.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_GLOBAL_FOLDERS), true);
            chkCopyAssemblies.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_ASSEMBLIES), false);
            chkCopySiteFiles.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_SITE_FOLDERS), true);

            if (SystemContext.IsPrecompiledWebsite)
            {
                // No code files or assemblies can be copied in precompiled website
                chkCopyAssemblies.Checked = chkCopyCodeFiles.Checked = false;
                chkCopyAssemblies.Enabled = chkCopyCodeFiles.Enabled = false;
                chkCopyAssemblies.ToolTip = chkCopyCodeFiles.ToolTip = GetString("importobjects.copyfiles.disabled");
            }

            chkBindings.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_ADD_SITE_BINDINGS), true);
            chkDeleteSite.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_DELETE_SITE), false);
            chkRunSite.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_RUN_SITE), !singleObject);

            bool? updateSiteDefinition = ImportSettings.GetSettings(ImportExportHelper.SETTINGS_UPDATE_SITE_DEFINITION);

            // Disable checkbox if value is already explicitly set to false
            if (updateSiteDefinition == false)
            {
                chkUpdateSite.Enabled = false;
                chkUpdateSite.Checked = false;
            }
            else
            {
                chkUpdateSite.Checked = ValidationHelper.GetBoolean(updateSiteDefinition, !singleObject);
            }      

            chkSkipOrfans.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_SKIP_OBJECT_ON_TRANSLATION_ERROR), false);
            chkImportTasks.Checked = ValidationHelper.GetBoolean(ImportSettings.GetSettings(ImportExportHelper.SETTINGS_TASKS), true);
            chkLogSync.Checked = ImportSettings.LogSynchronization;
            chkLogInt.Checked = ImportSettings.LogIntegration;
            chkRebuildIndexes.Checked = ImportSettings.SiteIsIncluded;
            chkRebuildIndexes.Visible = ImportSettings.SiteIsIncluded;

            Visible = true;

            if (ImportSettings.TemporaryFilesCreated)
            {
                if (ImportSettings.SiteIsIncluded && !singleObject)
                {
                    plcSite.Visible = true;

                    if (ImportSettings.ExistingSite)
                    {
                        plcExistingSite.Visible = true;
                        chkUpdateSite.Text = GetString("ImportObjects.UpdateSite");
                    }
                    plcSiteFiles.Visible = true;
                    chkBindings.Text = GetString("ImportObjects.Bindings");
                    chkRunSite.Text = GetString("ImportObjects.RunSite");
                    chkDeleteSite.Text = GetString("ImportObjects.DeleteSite");
                }
                else
                {
                    plcSite.Visible = false;
                }
            }
        }
        else
        {
            Visible = false;
        }
    }


    protected void lnkSelectAll_Click(object sender, EventArgs e)
    {
        ImportTypeEnum importType = ImportSettings.ImportType;

        ImportSettings.ImportType = ImportTypeEnum.AllForced;
        ImportSettings.LoadDefaultSelection(false);

        SaveSettings();

        ImportSettings.ImportType = importType;

        lblInfo.Text = GetString("ImportObjects.AllSelected");
    }


    protected void lnkSelectNone_Click(object sender, EventArgs e)
    {
        ImportTypeEnum importType = ImportSettings.ImportType;

        ImportSettings.ImportType = ImportTypeEnum.None;
        ImportSettings.LoadDefaultSelection(false);

        SaveSettings();

        ImportSettings.ImportType = importType;

        lblInfo.Text = GetString("ImportObjects.NoneSelected");
    }


    protected void lnkSelectDefault_Click(object sender, EventArgs e)
    {
        ImportSettings.LoadDefaultSelection(false);

        SaveSettings();

        lblInfo.Text = GetString("ImportObjects.DefaultSelected");
    }


    protected void lnkSelectNew_Click(object sender, EventArgs e)
    {
        ImportTypeEnum importType = ImportSettings.ImportType;

        ImportSettings.ImportType = ImportTypeEnum.New;
        ImportSettings.LoadDefaultSelection(false);

        SaveSettings();

        ImportSettings.ImportType = importType;

        lblInfo.Text = GetString("ImportObjects.NewSelected");
    }
}
