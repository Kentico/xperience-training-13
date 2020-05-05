using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

using IOExceptions = System.IO;

public partial class CMSModules_ImportExport_Controls_ImportWizard : CMSUserControl
{
    #region "Constants"

    /// <summary>
    /// Short link to help page regarding disk permissions.
    /// </summary>
    private const string HELP_TOPIC_DISKPERMISSIONS_LINK = "disk_permission_problems";

    #endregion


    #region "Variables"

    private static readonly Hashtable mManagers = new Hashtable();
    private static readonly object mLock = new object();

    private SiteImportSettings mImportSettings;
    private AsyncProcessData mProcessData;

    #endregion


    #region "Properties"

    /// <summary>
    /// Process messages
    /// </summary>
    public AsyncProcessData ProcessData
    {
        get
        {
            return mProcessData ?? (mProcessData = AsyncProcessData.GetDataForProcess(ProcessGUID));
        }
    }


    /// <summary>
    /// Redirection URL after finish button click.
    /// </summary>
    public string FinishUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Import manager.
    /// </summary>
    public ImportManager ImportManager
    {
        get
        {
            string key = "imManagers_" + ProcessGUID;
            if (mManagers[key] == null)
            {
                // On Azure, the restart cannot be detected via Instace GUIDs since with more instances, each instace has a different one.
                if (!StorageHelper.IsExternalStorage(null))
                {
                    // Detect restart of the application
                    if (ApplicationInstanceGUID != SystemHelper.ApplicationInstanceGUID)
                    {
                        // Lock section to avoid multiple log same error
                        lock (mLock)
                        {
                            LogStatusEnum progressLog = ImportSettings.GetProgressState();
                            if (progressLog == LogStatusEnum.Info)
                            {
                                ImportSettings.LogProgressState(LogStatusEnum.UnexpectedFinish, GetString("SiteImport.ApplicationRestarted"));
                            }
                        }
                    }
                }

                ImportManager im = new ImportManager(ImportSettings);
                mManagers[key] = im;
            }
            return (ImportManager)mManagers[key];
        }
        set
        {
            string key = "imManagers_" + ProcessGUID;
            mManagers[key] = value;
        }
    }


    /// <summary>
    /// Wizard height.
    /// </summary>
    public int PanelHeight
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["PanelHeight"], 400);
        }
        set
        {
            ViewState["PanelHeight"] = value;
        }
    }


    /// <summary>
    /// Application instance GUID.
    /// </summary>
    public Guid ApplicationInstanceGUID
    {
        get
        {
            if (ViewState["ApplicationInstanceGUID"] == null)
            {
                ViewState["ApplicationInstanceGUID"] = SystemHelper.ApplicationInstanceGUID;
            }

            return ValidationHelper.GetGuid(ViewState["ApplicationInstanceGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Import process GUID.
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
            return "Import_" + ProcessGUID + "_Settings";
        }
    }


    /// <summary>
    /// Import settings stored in viewstate.
    /// </summary>
    public SiteImportSettings ImportSettings
    {
        get
        {
            if (mImportSettings == null)
            {
                SiteImportSettings settings = (SiteImportSettings)AbstractImportExportSettings.GetFromPersistentStorage(PersistentSettingsKey);
                if (settings == null)
                {
                    if (wzdImport.ActiveStepIndex == 0)
                    {
                        settings = GetNewSettings();
                        PersistentStorageHelper.SetValue(PersistentSettingsKey, settings);
                    }
                    else
                    {
                        throw new Exception("[ImportWizard.ImportSettings]: Import settings has been lost.");
                    }
                }
                mImportSettings = settings;
            }
            return mImportSettings;
        }
        set
        {
            PersistentStorageHelper.SetValue(PersistentSettingsKey, value);
        }
    }

    #endregion


    #region "Finish step wizard buttons"

    /// <summary>
    /// Finish button.
    /// </summary>
    public LocalizedButton FinishButton
    {
        get
        {
            return wzdImport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepFinishButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Previous button.
    /// </summary>
    public LocalizedButton PreviousButton
    {
        get
        {
            return wzdImport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepPreviousButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton
    {
        get
        {
            return wzdImport.FindControl("StartNavigationTemplateContainerID").FindControl("StepNextButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Cancel button.
    /// </summary>
    public LocalizedButton CancelButton
    {
        get
        {
            return wzdImport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepCancelButton") as LocalizedButton;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Handle Import settings
        if (!RequestHelper.IsCallback() && !RequestHelper.IsPostBack())
        {
            // Initialize import settings
            ImportSettings = GetNewSettings();
        }

        if (wzdImport.ActiveStepIndex < 3)
        {
            stpConfigImport.Settings = ImportSettings;
            stpSiteDetails.Settings = ImportSettings;
            stpImport.Settings = ImportSettings;
        }

        if (!RequestHelper.IsCallback())
        {
            if (!VirtualPathHelper.UsingVirtualPathProvider)
            {
                SetWarningLabel(GetString("ImportSite.VirtualPathProviderNotRunning"));
            }

            ctrlAsyncUnzip.OnFinished += CtrlAsyncUnzipOnFinished;
            ctrlAsyncUnzip.OnError += CtrlAsyncUnzipOnError;

            ctlAsyncImport.OnCancel += ctlAsyncImport_OnCancel;

            bool notTempPermissions = false;

            if (wzdImport.ActiveStepIndex < 3)
            {
                // Ensure directory
                try
                {
                    DirectoryHelper.EnsureDiskPath(ImportSettings.TemporaryFilesPath + "\\temp.file", ImportSettings.WebsitePath);
                }
                catch (IOExceptions.IOException ex)
                {
                    pnlWrapper.Visible = false;
                    SetAlertLabel(lblErrorBlank, ex.Message);
                    return;
                }

                // Check permissions
                notTempPermissions = !DirectoryHelper.CheckPermissions(ImportSettings.TemporaryFilesPath, true, true, false, false);
            }

            if (notTempPermissions)
            {
                pnlWrapper.Visible = false;
                pnlPermissions.Visible = true;
                SetAlertLabel(lblErrorBlank, String.Format(GetString("ImportSite.ErrorPermissions"), ImportSettings.TemporaryFilesPath, WindowsIdentity.GetCurrent().Name));
                lnkPermissions.Target = "_blank";
                lnkPermissions.Text = GetString("Install.ErrorPermissions");
                lnkPermissions.NavigateUrl = DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_DISKPERMISSIONS_LINK);
            }
            else
            {
                if (!RequestHelper.IsPostBack())
                {
                    // Delete temporary files
                    try
                    {
                        // Delete only folder structure if there is not special folder
                        bool onlyFolderStructure = !Directory.Exists(DirectoryHelper.CombinePath(ImportSettings.TemporaryFilesPath, ImportExportHelper.FILES_FOLDER));
                        ImportProvider.DeleteTemporaryFiles(ImportSettings, onlyFolderStructure);
                    }
                    catch (Exception ex)
                    {
                        pnlWrapper.Visible = false;
                        SetAlertLabel(lblErrorBlank, GetString("ImportSite.ErrorDeletionTemporaryFiles") + " " + ex.Message);
                        return;
                    }
                }

                // Javascript functions
                string script = String.Format(
@"
function Finished(sender) {{
    var errorElement = document.getElementById('{2}');

    var errorText = sender.getErrors();
    if (errorText != '') {{ 
        errorElement.innerHTML = errorText;
        document.getElementById('{4}').style.removeProperty('display');
    }}

    var warningElement = document.getElementById('{3}');
    
    var warningText = sender.getWarnings();
    if (warningText != '') {{ 
        warningElement.innerHTML = warningText;
        document.getElementById('{5}').style.removeProperty('display');
    }}    

    var actDiv = document.getElementById('actDiv');
    if (actDiv != null) {{ 
        actDiv.style.display = 'none'; 
    }}

    BTN_Disable('{0}');
    BTN_Enable('{1}');

    if ((errorText == null) || (errorText == '')) {{ 
        BTN_Enable('{6}');
    }}
}}
",
                    CancelButton.ClientID,
                    FinishButton.ClientID,
                    lblError.LabelClientID,
                    lblWarning.LabelClientID,
                    pnlError.ClientID,
                    pnlWarning.ClientID,
                    NextButton.ClientID
                );

                // Register the script to perform get flags for showing buttons retrieval callback
                ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Finished", ScriptHelper.GetScript(script));

                // Add cancel button attribute
                CancelButton.Attributes.Add("onclick", ctlAsyncImport.GetCancelScript(true) + "return false;");

                wzdImport.NextButtonClick += wzdImport_NextButtonClick;
                wzdImport.PreviousButtonClick += wzdImport_PreviousButtonClick;
                wzdImport.FinishButtonClick += wzdImport_FinishButtonClick;

                if (!RequestHelper.IsPostBack())
                {
                    stpConfigImport.InitControl();
                }
            }
        }
    }


    private void ctlAsyncImport_OnCancel(object sender, EventArgs e)
    {
        wzdImport_FinishButtonClick(sender, null);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsCallback())
        {
            InitializeHeader();

            // Button click script
            const string afterScript =
@"
var imClicked = false;

function NextStepAction() {
    if (!imClicked) {
        imClicked = true;
        return true;
    }
    return false;
}
";

            ltlScriptAfter.Text += ScriptHelper.GetScript(afterScript);

            // Ensure default button
            EnsureDefaultButton();

            ShowObjectTypeCycleWarning();

            // Display warning if importing package with unsupported version
            if (wzdImport.ActiveStepIndex == 1 || wzdImport.ActiveStepIndex == 2)
            {
                if (ImportSettings.IsUnsupportedVersion())
                {
                    SetWarningLabel(String.Format(GetString("siteimport.olderversionimportwarning"),ImportSettings.Version));
                }
            }

            InitAlertLabels();
        }
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);

        // Save the settings
        if (wzdImport.ActiveStep.StepType != WizardStepType.Finish)
        {
            ImportSettings.SavePersistent();
        }
    }

    #endregion


    #region "Button handling"

    protected void wzdImport_FinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        if (String.IsNullOrEmpty(FinishUrl))
        {
            FinishUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Sites", false);
        }

        URLHelper.Redirect(UrlResolver.ResolveUrl(FinishUrl));
    }


    protected void wzdImport_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
        if (wzdImport.ActiveStepIndex == 1 || e.NextStepIndex == 0)
        {
            wzdImport.ActiveStepIndex = 0;
            stpConfigImport.Settings.TemporaryFilesCreated = false;
        }
        else
        {
            wzdImport.ActiveStepIndex = e.NextStepIndex;
        }
    }


    protected void wzdImport_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        switch (e.CurrentStepIndex)
        {
            case 0:
                // Apply settings
                if (!stpConfigImport.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }

                // Update settings
                ImportSettings = stpConfigImport.Settings;

                ltlScriptAfter.Text = ScriptHelper.GetScript(
                    "var actDiv = document.getElementById('actDiv'); \n" +
                    "if (actDiv != null) { actDiv.style.display='block'; } \n" +
                    "var buttonsDiv = document.getElementById('buttonsDiv'); if (buttonsDiv != null) { buttonsDiv.disabled=true; } \n" +
                    "BTN_Disable('" + NextButton.ClientID + "'); \n" +
                    "StartUnzipTimer();"
                    );

                // Create temporary files asynchronously
                ctrlAsyncUnzip.RunAsync(CreateTemporaryFiles, WindowsIdentity.GetCurrent());

                e.Cancel = true;
                break;

            case 1:
                // Apply settings
                if (!stpSiteDetails.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }

                // Update settings
                ImportSettings = stpSiteDetails.Settings;
                //stpImport.SelectedNodeValue = CMSObjectHelper.GROUP_OBJECTS;
                stpImport.ReloadData(true);

                wzdImport.ActiveStepIndex++;
                break;

            case 2:
                // Apply settings
                if (!stpImport.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }

                ImportSettings = stpImport.Settings;
                ImportSettings.DefaultProcessObjectType = ProcessObjectEnum.Selected;

                if (!StartImport(ImportSettings))
                {
                    e.Cancel = true;

                    return;
                }

                wzdImport.ActiveStepIndex++;
                break;
        }
    }

    #endregion


    #region "Async control events"

    protected void CtrlAsyncUnzipOnError(object sender, EventArgs e)
    {
        if (((AsyncControl)sender).Worker.LastException != null)
        {
            // Show error message
            SetErrorLabel(((AsyncControl)sender).Worker.LastException.Message);
        }
        else
        {
            // Show general error message
            SetErrorLabel(String.Format(GetString("logon.erroroccurred"), GetString("general.seeeventlog")));
        }

        // Stop the timer
        ltlScript.Text += ScriptHelper.GetScript("StopUnzipTimer();");
    }


    protected void CtrlAsyncUnzipOnFinished(object sender, EventArgs e)
    {
        // Stop the timer
        const string script = "StopUnzipTimer();";

        var settings = ImportSettings;

        // Check if a new module is being imported
        if (settings.IsInstallableModule)
        {
            // Start the import process immediately
            settings.DefaultProcessObjectType = ProcessObjectEnum.All;
            settings.CopyFiles = false;
            settings.CopyCodeFiles = false;
            if (!StartImport(settings))
            {
                return;
            }

            // Skip step for site and objects selection
            wzdImport.ActiveStepIndex++;

            // Skip step for object selection
            wzdImport.ActiveStepIndex++;

        } // Decide if importing site
        else if (settings.SiteIsIncluded)
        {
            // Single site import and no site exists
            if (ValidationHelper.GetBoolean(settings.GetInfo(ImportExportHelper.INFO_SINGLE_OBJECT), false) && (SiteInfoProvider.GetSitesCount() == 0))
            {
                SetErrorLabel(GetString("SiteImport.SingleSiteObjectNoSite"));
                return;
            }

            // Init control
            stpSiteDetails.ReloadData();
        }
        else
        {
            // Skip step for site selection
            wzdImport.ActiveStepIndex++;
            stpImport.ReloadData(true);
        }

        // Move to the next step
        wzdImport.ActiveStepIndex++;

        ltlScriptAfter.Text += ScriptHelper.GetScript(script);
    }

    #endregion


    #region "Other methods"

    protected void InitializeHeader()
    {
        // Make some step count corrections
        if ((wzdImport.ActiveStepIndex == 0) || ImportSettings.SiteIsIncluded)
        {
            ucHeader.Title = string.Format(GetString("ImportPanel.Title"), wzdImport.ActiveStepIndex + 1, wzdImport.WizardSteps.Count);
        }
        else
        {
            ucHeader.Title = string.Format(GetString("ImportPanel.Title"), wzdImport.ActiveStepIndex, wzdImport.WizardSteps.Count - 1);
        }

        switch (wzdImport.ActiveStepIndex)
        {
            case 0:
                ucHeader.Header = GetString("ImportPanel.ObjectsSettingsHeader");
                ucHeader.Description = GetString("ImportPanel.ObjectsSelectionSetting");
                break;

            case 1:
                ucHeader.Header = GetString("ImportPanel.ObjectsSiteDetailsHeader");
                if (ImportSettings.SiteIsIncluded && ValidationHelper.GetBoolean(ImportSettings.GetInfo(ImportExportHelper.INFO_SINGLE_OBJECT), false))
                {
                    ucHeader.Description = GetString("ImportPanel.SiteObjectImport");
                }
                else
                {
                    ucHeader.Description = GetString("ImportPanel.ObjectsSiteDetailsDescription");
                }
                break;

            case 2:
                ucHeader.Header = GetString("ImportPanel.ObjectsSelectionHeader");
                ucHeader.Description = GetString("ImportPanel.ObjectsSelectionDescription");
                break;

            case 3:
                ucHeader.Header = GetString("ImportPanel.ObjectsProgressHeader");
                ucHeader.Description = GetString("ImportPanel.ObjectsProgressDescription");
                break;
        }
    }


    // Create temporary files and preselect objects
    private void CreateTemporaryFiles(object parameter)
    {
        var settngs = ImportSettings;

        ImportProvider.CreateTemporaryFiles(settngs);

        settngs.LoadDefaultSelection();
        settngs.SavePersistent();
    }


    /// <summary>
    /// Starts import process with given settings. Returns true if import was started successfully, false otherwise (error label is set in this case).
    /// </summary>
    /// <param name="settings">Import settings</param>
    /// <returns>Returns true if import was started successfully, false otherwise.</returns>
    private bool StartImport(SiteImportSettings settings)
    {
        if (!string.IsNullOrEmpty(ImportExportControl.CheckLicenses(settings)))
        {
            // License is missing, let's try to import some from package.
            EnsureLicenseFromPackage(settings);
        }

        // Check licences
        string error = ImportExportControl.CheckLicenses(settings);
        if (!string.IsNullOrEmpty(error))
        {
            SetErrorLabel(error);

            return false;
        }

        // Start asynchronnous Import
        if (settings.SiteIsIncluded)
        {
            settings.EventLogSource = String.Format(settings.GetAPIString("ImportSite.EventLogSiteSource", "Import '{0}' site"), ResHelper.LocalizeString(settings.SiteDisplayName));
        }

        var manager = ImportManager;

        settings.LogContext = ctlAsyncImport.CurrentLog;
        manager.Settings = settings;

        ctlAsyncImport.RunAsync(manager.Import, WindowsIdentity.GetCurrent());

        return true;
    }


    /// <summary>
    /// Imports the license keys from package defined by <paramref name="settings"/>.
    /// </summary>
    private static void EnsureLicenseFromPackage(SiteImportSettings settings)
    {
        // Clone import settings so they aren't changed by license import process
        SiteImportSettings settingsCopy;

        using (var stream = new IOExceptions.MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, settings);

            stream.Position = 0;
            settingsCopy = (SiteImportSettings)formatter.Deserialize(stream);
        }

        ImportProvider.ImportObjectType(settingsCopy, LicenseKeyInfo.OBJECT_TYPE, false, new TranslationHelper(), ProcessObjectEnum.Selected, new List<int>());
    }


    private void EnsureDefaultButton()
    {
        if (wzdImport.ActiveStep != null)
        {
            switch (wzdImport.ActiveStep.StepType)
            {
                case WizardStepType.Start:
                    Page.Form.DefaultButton =
                        wzdImport.FindControl("StartNavigationTemplateContainerID").FindControl("StepNextButton").
                            UniqueID;
                    break;

                case WizardStepType.Step:
                    Page.Form.DefaultButton =
                        wzdImport.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton").
                            UniqueID;
                    break;

                case WizardStepType.Finish:
                    Page.Form.DefaultButton =
                        wzdImport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepFinishButton").
                            UniqueID;
                    break;
            }
        }
    }


    /// <summary>
    /// Creates new settings object for Import.
    /// </summary>
    private SiteImportSettings GetNewSettings()
    {
        SiteImportSettings result = new SiteImportSettings(MembershipContext.AuthenticatedUser);

        result.WebsitePath = Server.MapPath("~/");
        result.PersistentSettingsKey = PersistentSettingsKey;

        return result;
    }


    /// <summary>
    /// Initializes (hides) alert labels
    /// </summary>
    private void InitAlertLabels()
    {
        // Do not use Visible property to hide this elements. They are used in JS.
        pnlError.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, String.IsNullOrEmpty(lblError.Text) ? "none" : "block");
        pnlWarning.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, String.IsNullOrEmpty(lblWarning.Text) ? "none" : "block");
    }


    /// <summary>
    /// Displays a warning about potential problems caused by object type dependency cycles.
    /// </summary>
    private void ShowObjectTypeCycleWarning()
    {
        // Show only on the second step
        if ((wzdImport.ActiveStepIndex == 2) && ImportExportHelper.ObjectTypeCycles.Any())
        {
            SetAlertLabel(lblWarningCycles, String.Format(GetString("importexport.objecttypecycles"), String.Join("<br />", ImportExportHelper.ObjectTypeCycles.Select(HTMLHelper.HTMLEncode))));
        }
        else
        {
            pnlWarningCycles.Visible = false;
        }
    }


    /// <summary>
    /// Displays text in given alert label
    /// </summary>
    /// <param name="label">Alert label</param>
    /// <param name="text">Text to display</param>
    private void SetAlertLabel(Label label, string text)
    {
        label.Text = text;
    }


    /// <summary>
    /// Displays error alert label with given text
    /// </summary>
    /// <param name="text">Text to display</param>
    private void SetErrorLabel(string text)
    {
        SetAlertLabel(lblError, text);
    }


    /// <summary>
    /// Displays warning alert label with given text
    /// </summary>
    /// <param name="text">Text to display</param>
    private void SetWarningLabel(string text)
    {
        SetAlertLabel(lblWarning, text);
    }

    #endregion
}