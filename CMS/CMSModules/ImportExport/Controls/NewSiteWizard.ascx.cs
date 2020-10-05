using System;
using System.Collections;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Internal;
using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_ImportExport_Controls_NewSiteWizard : CMSUserControl
{
    #region "Variables"

    private const string BLANK_SITE_PATH = @"~\App_Data\Templates\BlankSite";
    private const string STOP_SELECTION_TIMER_SCRIPT = "StopSelectionTimer();";

    private static readonly Hashtable mManagers = new Hashtable();

    private string mFinishUrl;
    private bool? mSiteIsRunning;
    private bool mImportCanceled;
    private SiteImportSettings mImportSettings;

    #endregion


    #region "Properties"

    /// <summary>
    /// Redirection URL after finish button click.
    /// </summary>
    private string FinishUrl => mFinishUrl ?? (mFinishUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Sites", false));


    /// <summary>
    /// Import manager.
    /// </summary>
    private ImportManager ImportManager
    {
        get
        {
            var key = $"imManagers_{ProcessGUID}";
            if (mManagers[key] == null)
            {
                // On Azure, the restart cannot be detected via Instance GUIDs since with more instances, each instance has a different one.
                if (!StorageHelper.IsExternalStorage(null))
                {
                    // Detect restart of the application
                    if (ApplicationInstanceGUID != SystemHelper.ApplicationInstanceGUID)
                    {
                        LogStatusEnum progressLog = ImportSettings.GetProgressState();
                        if (progressLog == LogStatusEnum.Info)
                        {
                            ImportSettings.LogProgressState(LogStatusEnum.UnexpectedFinish, GetString("SiteImport.ApplicationRestarted"));
                        }
                    }
                }

                ImportManager im = new ImportManager(ImportSettings);
                im.ThrowExceptionOnError = true;
                mManagers[key] = im;
            }
            return (ImportManager)mManagers[key];
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
    private Guid ApplicationInstanceGUID
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
    private Guid ProcessGUID
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
    private string PersistentSettingsKey => $"NewSiteWizard_{ProcessGUID}_Settings";


    /// <summary>
    /// Import settings stored in viewstate.
    /// </summary>
    private SiteImportSettings ImportSettings
    {
        get
        {
            if (mImportSettings == null)
            {
                SiteImportSettings settings = (SiteImportSettings)PersistentStorageHelper.GetValue(PersistentSettingsKey);
                mImportSettings = settings ?? throw new Exception("[ImportWizard.ImportSettings]: Import settings has been lost.");
            }
            return mImportSettings;
        }
        set
        {
            mImportSettings = value;
            PersistentStorageHelper.SetValue(PersistentSettingsKey, value);
        }
    }


    /// <summary>
    /// Indicates if imported site is running.
    /// </summary>
    private bool SiteIsRunning => mSiteIsRunning ?? (mSiteIsRunning = ImportManager.Settings.SiteInfo.Status == SiteStatusEnum.Running).Value;


    /// <summary>
    /// Site name.
    /// </summary>
    private string SiteName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SiteName"], String.Empty);
        }
        set
        {
            ViewState["SiteName"] = value;
        }
    }


    /// <summary>
    /// Site domain.
    /// </summary>
    private string Domain
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Domain"], String.Empty);
        }
        set
        {
            ViewState["Domain"] = value;
        }
    }


    /// <summary>
    /// Site culture.
    /// </summary>
    private string Culture
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Culture"], String.Empty);
        }
        set
        {
            ViewState["Culture"] = value;
        }
    }

    #endregion


    #region "Finish step wizard buttons"

    /// <summary>
    /// Finish button.
    /// </summary>
    public LocalizedButton FinishButton => wzdImport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepFinishButton") as LocalizedButton;


    /// <summary>
    /// Previous button.
    /// </summary>
    public LocalizedButton PreviousButton => wzdImport.FindControl("StepNavigationTemplateContainerID").FindControl("StepPreviousButton") as LocalizedButton;


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton => wzdImport.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton") as LocalizedButton;


    /// <summary>
    /// Cancel button.
    /// </summary>
    public LocalizedButton CancelButton => wzdImport.FindControl("StepNavigationTemplateContainerID").FindControl("StepCancelButton") as LocalizedButton;

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        if (!RequestHelper.IsPostBack())
        {
            // Initialize import settings
            ImportSettings = new SiteImportSettings(MembershipContext.AuthenticatedUser)
            {
                WebsitePath = Server.MapPath("~/"),
                PersistentSettingsKey = PersistentSettingsKey
            };

            if (wzdImport.ActiveStep.StepType == WizardStepType.Start)
            {
                EnsureTemplateSelection();
            }
        }

        if (RequestHelper.IsCallback())
        {
            // Stop processing when callback
            siteDetails.StopProcessing = true;
        }
        else
        {
            var previousButton = PreviousButton;
            var nextButton = NextButton;

            previousButton.Enabled = true;
            previousButton.Visible = wzdImport.ActiveStepIndex <= 2;
            nextButton.Enabled = true;

            // Bind async controls events
            ctrlAsyncSelection.OnFinished += ctrlAsyncSelection_OnFinished;
            ctrlAsyncSelection.OnError += ctrlAsyncSelection_OnError;

            ctlAsyncImport.OnCancel += ctlAsyncImport_OnCancel;
            ctlAsyncImport.OnFinished += ctlAsyncImport_OnFinished;

            if (wzdImport.ActiveStepIndex < 2)
            {
                siteDetails.Settings = ImportSettings;
                pnlImport.Settings = ImportSettings;
            }

            // Javascript functions
            var script = $@"
function Finished(sender) {{
    var errorElement = document.getElementById('{lblError.LabelClientID}');

    var errorText = sender.getErrors();
    if (errorText != '') {{
        errorElement.innerHTML = errorText;
        document.getElementById('{pnlError.ClientID}').style.removeProperty('display');
    }}

    var warningElement = document.getElementById('{lblWarning.LabelClientID}');

    var warningText = sender.getWarnings();
    if (warningText != '') {{
        warningElement.innerHTML = warningText;
        document.getElementById('{pnlWarning.ClientID}').style.removeProperty('display');
    }}

    var actDiv = document.getElementById('actDiv');
    if (actDiv != null) {{
        actDiv.style.display = 'none';
    }}

    BTN_Disable('{CancelButton.ClientID}');
    BTN_Enable('{FinishButton.ClientID}');

    if ((errorText == null) || (errorText == '')) {{
        BTN_Enable('{nextButton.ClientID}');
    }}
}}";

            // Register the script to perform get flags for showing buttons retrieval callback
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Finished", ScriptHelper.GetScript(script));

            // Add cancel button attribute
            CancelButton.Attributes.Add("onclick", ctlAsyncImport.GetCancelScript(true) + "return false;");

            wzdImport.NextButtonClick += wzdImport_NextButtonClick;
            wzdImport.PreviousButtonClick += wzdImport_PreviousButtonClick;
            wzdImport.FinishButtonClick += wzdImport_FinishButtonClick;
        }
    }


    private void ctlAsyncImport_OnCancel(object sender, EventArgs e)
    {
        FinishAndRedirect();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Initialize header
        InitializeHeader();

        // Button click script
        const string afterScript = @"
var imClicked = false;
function NextStepAction() {
   if(!imClicked) {
        imClicked = true;
        return true;
   }
   return false;
}";

        ltlScriptAfter.Text += ScriptHelper.GetScript(afterScript);

        // Ensure default button
        EnsureDefaultButton();

        InitAlertLabels();
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);

        // Save the settings
        if (!mImportCanceled && wzdImport.ActiveStepIndex <= 2)
        {
            ImportSettings.SavePersistent();
        }
    }

    #endregion


    #region "Button handling"

    protected void wzdImport_FinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        if (!mImportCanceled)
        {
            FinishAndRedirect();
        }
    }


    protected void wzdImport_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
        wzdImport.ActiveStepIndex--;
    }


    protected void wzdImport_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        var settings = ImportSettings;
        ClearErrorLabel();

        switch (e.CurrentStepIndex)
        {
            // Site details
            case 0:
                {
                    if (!siteDetails.ApplySettings())
                    {
                        e.Cancel = true;
                        return;
                    }

                    Culture = siteDetails.Culture;

                    pnlImport.ReloadData(true);
                    wzdImport.ActiveStepIndex++;
                }
                break;

            // Objects selection
            case 1:
                if (!pnlImport.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }

                // Check licenses
                var error = ImportExportControl.CheckLicenses(settings);
                if (!String.IsNullOrEmpty(error))
                {
                    SetErrorLabel(error);

                    e.Cancel = true;
                    return;
                }

                ImportSettings = pnlImport.Settings;

                PreviousButton.Enabled = false;
                NextButton.Enabled = false;

                SiteName = settings.SiteName;
                Domain = settings.SiteDomain;

                // Start asynchronous Import
                settings.SetSettings(ImportExportHelper.SETTINGS_DELETE_TEMPORARY_FILES, false);
                settings.DefaultProcessObjectType = ProcessObjectEnum.Selected;

                var manager = ImportManager;

                settings.LogContext = ctlAsyncImport.CurrentLog;
                manager.Settings = settings;

                // Import site asynchronously
                ctlAsyncImport.RunAsync(ImportManager.Import, WindowsIdentity.GetCurrent());

                wzdImport.ActiveStepIndex++;
                break;

            // Import progress
            case 2:
                PreviousButton.Visible = false;

                CultureHelper.SetPreferredCulture(Culture);

                if (ImportManager.Settings.IsWarning())
                {
                    try
                    {
                        // Convert default culture and change root's GUID
                        ImportPostProcess();
                    }
                    catch (Exception ex)
                    {
                        Service.Resolve<IEventLogService>().LogException("NewSiteWizard", "FINISH", ex);
                        SetErrorLabel(ex.Message);
                        e.Cancel = true;

                        NextButton.Enabled = false;
                        CancelButton.Enabled = false;
                        mImportCanceled = true;
                        return;
                    }
                }
                break;

            case 3:
                break;

            // Other steps
            default:
                wzdImport.ActiveStepIndex = e.NextStepIndex;
                break;
        }
    }

    #endregion


    #region "Async control events"

    protected void ctrlAsyncSelection_OnError(object sender, EventArgs e)
    {
        SetErrorLabel(((AsyncControl)sender).Worker.LastException.Message);

        // Stop the timer
        ltlScript.Text += ScriptHelper.GetScript(STOP_SELECTION_TIMER_SCRIPT);
    }


    protected void ctrlAsyncSelection_OnFinished(object sender, EventArgs e)
    {
        // Init control
        siteDetails.SiteName = String.Empty;
        siteDetails.SiteDisplayName = String.Empty;
        siteDetails.DomainName = RequestContext.FullDomain;
        siteDetails.DisplayCulture = true;
        siteDetails.ReloadData();

        ltlScriptAfter.Text += ScriptHelper.GetScript(STOP_SELECTION_TIMER_SCRIPT);
    }


    protected void ctlAsyncImport_OnFinished(object sender, EventArgs e)
    {
        try
        {
            ImportPostProcess();
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("NewSiteWizard", "FINISH", ex);
        }
        finally
        {
            if (ImportManager.Settings.ProcessCanceled)
            {
                NextButton.Enabled = CancelButton.Enabled = false;
                mImportCanceled = true;
            }
            else if (!ImportManager.Settings.IsWarning() && !ImportManager.Settings.IsError())
            {
                PreviousButton.Visible = false;
                CultureHelper.SetPreferredCulture(Culture);

                finishSite.Domain = Domain;
                finishSite.SiteIsRunning = SiteIsRunning;

                wzdImport.ActiveStepIndex++;
            }

            // Stop the timer
            ltlScriptAfter.Text += ScriptHelper.GetScript(STOP_SELECTION_TIMER_SCRIPT);
        }
    }

    #endregion


    #region "Other methods"

    protected void InitializeHeader()
    {
        switch (wzdImport.ActiveStepIndex)
        {
            case 0:
                ucHeader.Header = GetString("NewSite_SiteDetails.StepTitle");
                ucHeader.Description = GetString("NewSite_SiteDetails.StepDesc");
                break;

            case 1:
                ucHeader.Header = GetString("ImportPanel.ObjectsSelectionHeader");
                ucHeader.Description = GetString("ImportPanel.ObjectsSelectionDescription");
                break;

            case 2:
                ucHeader.Header = GetString("ImportPanel.ObjectsProgressHeader");
                ucHeader.Description = GetString("ImportPanel.ObjectsProgressDescription");
                break;

            case 3:
                ucHeader.Header = GetString("NewSite_Finish.StepTitle");
                ucHeader.Description = GetString("NewSite_Finish.StepDesc");
                break;
        }

        ucHeader.Title = String.Format(GetString("NewSite_Step"), wzdImport.ActiveStepIndex + 1, 4);
    }


    private void EnsureTemplateSelection()
    {
        try
        {
            var webPathMapper = Service.Resolve<IWebPathMapper>();
            var path = webPathMapper.MapPath(BLANK_SITE_PATH);

            if (!File.Exists(Path.Combine(path, "template.zip")))
            {
                siteDetails.StopProcessing = true;
                pnlWrapper.Visible = false;

                ShowError(GetString("NewSite.NoWebTemplate"));

                return;
            }

            path = Path.Combine(path, ZipStorageProvider.GetZipFileName("template.zip"));

            ImportSettings.SourceFilePath = path;
            ImportSettings.TemporaryFilesPath = path;
            ImportSettings.TemporaryFilesCreated = true;
            ImportSettings.RefreshMacroSecurity = true;
            ImportSettings.IsNewSite = true;

            ImportProvider.CreateTemporaryFiles(ImportSettings);

            // Import all, but only add new data
            ImportSettings.ImportType = ImportTypeEnum.AllNonConflicting;
            ImportSettings.ImportOnlyNewObjects = true;
            ImportSettings.CopyFiles = false;

            // Allow bulk inserts for faster import, web templates must be consistent enough to allow this without collisions
            ImportSettings.AllowBulkInsert = true;

            ltlScriptAfter.Text = ScriptHelper.GetScript($@"
var actDiv = document.getElementById('actDiv');
if (actDiv != null) {{
    actDiv.style.display='block';
}}
var buttonsDiv = document.getElementById('buttonsDiv');
if (buttonsDiv != null) {{
    buttonsDiv.disabled=true;
}}
BTN_Disable('{NextButton.ClientID}');
StartSelectionTimer();");

            // Preselect objects asynchronously
            ctrlAsyncSelection.RunAsync(SelectObjects, WindowsIdentity.GetCurrent());
        }
        catch (Exception ex)
        {
            SetErrorLabel(ex.Message);
        }
    }


    private void SelectObjects(object parameter)
    {
        var settings = ImportSettings;

        settings.LoadDefaultSelection();
        settings.SavePersistent();
    }


    private void EnsureDefaultButton()
    {
        if (wzdImport.ActiveStep != null)
        {
            switch (wzdImport.ActiveStep.StepType)
            {
                case WizardStepType.Start:
                    Page.Form.DefaultButton = wzdImport.FindControl("StartNavigationTemplateContainerID").FindControl("StepNextButton").UniqueID;
                    break;

                case WizardStepType.Step:
                    Page.Form.DefaultButton = wzdImport.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton").UniqueID;
                    break;

                case WizardStepType.Finish:
                    Page.Form.DefaultButton = wzdImport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepFinishButton").UniqueID;
                    break;
            }
        }
    }


    private void InitAlertLabels()
    {
        // Do not use Visible property to hide this elements. They are used in JS.
        pnlError.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, String.IsNullOrEmpty(lblError.Text) ? "none" : "block");
        pnlWarning.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, String.IsNullOrEmpty(lblWarning.Text) ? "none" : "block");
    }


    private void SetErrorLabel(string text)
    {
        lblError.Text = text;
    }


    private void ClearErrorLabel()
    {
        lblError.Text = String.Empty;
    }


    private void ImportPostProcess()
    {
        // Convert default culture
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        tree.ChangeSiteDefaultCulture(SiteName, Culture, "en-US");

        // Change root GUID
        TreeNode root = DocumentHelper.GetDocument(SiteName, "/", Culture, false, SystemDocumentTypes.Root, null, null, 1, false, null, tree);
        if (root != null)
        {
            root.NodeGUID = Guid.NewGuid();
            DocumentHelper.UpdateDocument(root, tree);
        }
    }


    private void FinishAndRedirect()
    {
        URLHelper.Redirect(UrlResolver.ResolveUrl(FinishUrl));
    }

    #endregion
}
