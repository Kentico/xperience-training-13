using System;
using System.Collections.Generic;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_ImportConfiguration : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessages;
        }
    }


    /// <summary>
    /// Import settings.
    /// </summary>
    public SiteImportSettings Settings
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Initialize button for adding packages
            newImportPackage.DisplayInline = true;
            newImportPackage.AllowedExtensions = "zip";
            newImportPackage.ParentElemID = ClientID;
            newImportPackage.ForceLoad = true;
            newImportPackage.ShowProgress = false;
            newImportPackage.CheckPermissions = true;
            newImportPackage.SourceType = MediaSourceEnum.PhysicalFile;
            newImportPackage.TargetFolderPath = ImportExportHelper.GetSiteUtilsFolder() + "Import";
            newImportPackage.Text = GetString("importconfiguration.uploadpackage");
            newImportPackage.InnerElementClass = "NewAttachment";
            newImportPackage.InnerLoadingElementClass = "NewAttachmentLoading";
            newImportPackage.DisplayInline = true;
            newImportPackage.AfterSaveJavascript = "Refresh" + ClientID;

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Refresh" + ClientID, ScriptHelper.GetScript("function Refresh" + ClientID + "(){" + Page.ClientScript.GetPostBackEventReference(btnRefresh, null).Replace("'", "\"") + "}"));

            if (!RequestHelper.IsCallback() && !RequestHelper.IsPostBack())
            {
                radAll.Text = GetString("ImportConfiguration.All");
                radNew.Text = GetString("ImportConfiguration.Date");

                // Load imports list
                RefreshPackageList(null);
            }
        }
    }


    /// <summary>
    /// Refresh button click handler.
    /// </summary>
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        RefreshPackageList(sender);
    }


    /// <summary>
    /// Refresh package list.
    /// </summary>
    /// <param name="sender">Sender object</param>
    private void RefreshPackageList(object sender)
    {
        // Reload the list
        lstImports.Items.Clear();
        LoadImports();

        // Update panel
        pnlUpdate.Update();

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
    /// Refresh button click handler.
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(lstImports.SelectedValue))
        {
            try
            {
                File.Delete(ImportExportHelper.GetSiteUtilsFolder() + "Import\\" + lstImports.SelectedValue);
            }
            catch (Exception ex)
            {
                ScriptHelper.RegisterStartupScript(Page, typeof(string), "ErrorMessage", ScriptHelper.GetAlertScript(ex.Message));
            }
            finally
            {
                RefreshPackageList(sender);
            }
        }
    }


    // Load imports lsit
    private void LoadImports()
    {
        if (lstImports.Items.Count == 0)
        {
            List<string> files = null;

            // Get import packages
            try
            {
                files = ImportProvider.GetImportFilesList();
            }
            catch
            {
                // Show error and log exception
                string path = ImportExportHelper.GetSiteUtilsFolderRelativePath();
                if (path == null)
                {
                    path = ImportExportHelper.GetSiteUtilsFolder();
                }

                ShowError(GetString("importconfiguration.securityerror").Replace("{0}", path + "Import"));
            }

            if ((files != null) && (files.Count != 0))
            {
                lstImports.Enabled = true;
                lstImports.DataSource = files;
                lstImports.DataBind();
            }
            else
            {
                lstImports.Enabled = false;
            }
        }

        if (!RequestHelper.IsPostBack())
        {
            try
            {
                lstImports.SelectedIndex = 0;
            }
            catch
            {
            }
        }
    }


    /// <summary>
    /// Initialize control.
    /// </summary>
    public void InitControl()
    {
        // Could be used in the future:)
    }


    /// <summary>
    /// Apply control settings.
    /// </summary>
    public bool ApplySettings()
    {
        string result = null;

        // File is not selected, inform the user
        if (lstImports.SelectedItem == null)
        {
            result = GetString("ImportConfiguration.FileError");
        }

        if (string.IsNullOrEmpty(result))
        {
            try
            {
                // Set current user information
                Settings.CurrentUser = MembershipContext.AuthenticatedUser;

                // Set source filename
                Settings.SourceFilePath = ImportExportHelper.GetSiteUtilsFolder() + "Import\\" + lstImports.SelectedValue;

                // Init default values
                Settings.ImportType = radAll.Checked ? ImportTypeEnum.AllNonConflicting : ImportTypeEnum.New;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return false;
            }
        }
        else
        {
            ShowError(result);
            return false;
        }

        return true;
    }

}
