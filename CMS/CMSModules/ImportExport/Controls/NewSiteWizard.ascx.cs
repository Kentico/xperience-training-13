using System;
using System.Collections;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Core;
using CMS.DataEngine;
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

    private static readonly Hashtable mManagers = new Hashtable();

    private bool? mSiteIsRunning;
    private bool mImportCanceled;
    private SiteImportSettings mImportSettings;

    #endregion


    #region "Properties"

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
            return "NewSiteWizard_" + ProcessGUID + "_Settings";
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
                SiteImportSettings settings = (SiteImportSettings)PersistentStorageHelper.GetValue(PersistentSettingsKey);
                if (settings == null)
                {
                    throw new Exception("[ImportWizard.ImportSettings]: Import settings has been lost.");
                }
                mImportSettings = settings;
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
    public bool SiteIsRunning
    {
        get
        {
            if (mSiteIsRunning == null)
            {
                mSiteIsRunning = (ImportManager.Settings.SiteInfo.Status == SiteStatusEnum.Running);
            }
            return mSiteIsRunning.Value;
        }
    }


    /// <summary>
    /// Web template ID.
    /// </summary>
    public int WebTemplateID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["WebTemplateID"], 0);
        }
        set
        {
            ViewState["WebTemplateID"] = value;
        }
    }


    /// <summary>
    /// Site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SiteName"], "");
        }
        set
        {
            ViewState["SiteName"] = value;
        }
    }


    /// <summary>
    /// Site domain.
    /// </summary>
    public string Domain
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Domain"], "");
        }
        set
        {
            ViewState["Domain"] = value;
        }
    }


    /// <summary>
    /// Site culture.
    /// </summary>
    public string Culture
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Culture"], "");
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
            return wzdImport.FindControl("StepNavigationTemplateContainerID").FindControl("StepPreviousButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton
    {
        get
        {
            if (wzdImport.ActiveStepIndex == 0)
            {
                return wzdImport.FindControl("StartNavigationTemplateContainerID").FindControl("StepNextButton") as LocalizedButton;
            }
            return wzdImport.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Cancel button.
    /// </summary>
    public LocalizedButton CancelButton
    {
        get
        {
            return wzdImport.FindControl("StepNavigationTemplateContainerID").FindControl("StepCancelButton") as LocalizedButton;
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
            // Check if any template is present on the disk
            if (!WebTemplateInfoProvider.IsAnyTemplatePresent())
            {
                selectTemplate.StopProcessing = true;
                pnlWrapper.Visible = false;
                ShowError(GetString("NewSite.NoWebTemplate"));
            }

            // Initialize import settings
            ImportSettings = new SiteImportSettings(MembershipContext.AuthenticatedUser);
            ImportSettings.WebsitePath = Server.MapPath("~/");
            ImportSettings.PersistentSettingsKey = PersistentSettingsKey;
        }

        if (RequestHelper.IsCallback())
        {
            // Stop processing when callback
            selectTemplate.StopProcessing = true;
        }
        else
        {
            var previousButton = PreviousButton;
            var nextButton = NextButton;
            selectTemplate.StopProcessing = (!CausedPostback(previousButton) || (wzdImport.ActiveStepIndex != 2)) && (wzdImport.ActiveStepIndex != 1);

            previousButton.Enabled = true;
            previousButton.Visible = (wzdImport.ActiveStepIndex <= 4);
            nextButton.Enabled = true;

            // Bind async controls events
            ctrlAsyncSelection.OnFinished += ctrlAsyncSelection_OnFinished;
            ctrlAsyncSelection.OnError += ctrlAsyncSelection_OnError;

            ctlAsyncImport.OnCancel += ctlAsyncImport_OnCancel;
            ctlAsyncImport.OnFinished += ctlAsyncImport_OnFinished;

            if (wzdImport.ActiveStepIndex < 4)
            {
                siteDetails.Settings = ImportSettings;
                pnlImport.Settings = ImportSettings;
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
                nextButton.ClientID
            );

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
        wzdImport_FinishButtonClick(sender, null);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Initialize header
        InitializeHeader();

        // Button click script
        const string afterScript = "var imClicked = false; \n" +
                                   "function NextStepAction() \n" +
                                   "{ \n" +
                                   "   if(!imClicked) \n" +
                                   "   { \n" +
                                   "     imClicked = true; \n" +
                                   "     return true; \n" +
                                   "   } \n" +
                                   "   return false; \n" +
                                   "} \n";

        ltlScriptAfter.Text += ScriptHelper.GetScript(afterScript);

        // Ensure default button
        EnsureDefaultButton();

        InitAlertLabels();
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);

        // Save the settings
        if (!mImportCanceled && (wzdImport.ActiveStepIndex <= 4))
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
            if (String.IsNullOrEmpty(FinishUrl))
            {
                FinishUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Sites", false);
            }

            URLHelper.Redirect(UrlResolver.ResolveUrl(FinishUrl));
        }
    }


    protected void wzdImport_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
        switch (e.CurrentStepIndex)
        {
            // Site details
            case 2:
                if (!siteType.SelectTemplate)
                {
                    wzdImport.ActiveStepIndex--;
                }
                wzdImport.ActiveStepIndex--;
                break;

            // Progress
            case 4:
                URLHelper.Redirect(RequestContext.URL.AbsoluteUri);
                break;

            default:
                wzdImport.ActiveStepIndex--;
                break;
        }
    }


    protected void wzdImport_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        var settings = ImportSettings;
        ClearErrorLabel();
        switch (e.CurrentStepIndex)
        {
            // Import type
            case 0:
                {
                    if (!siteType.SelectTemplate)
                    {
                        try
                        {
                            // Get blank web template                    
                            WebTemplateInfo wi = WebTemplateInfoProvider.GetWebTemplateInfo("BlankSite");
                            if (wi == null)
                            {
                                e.Cancel = true;
                                return;
                            }

                            WebTemplateID = wi.WebTemplateId;

                            string path = Server.MapPath(wi.WebTemplateFileName);
                            if (File.Exists(path + "\\template.zip"))
                            {
                                // Template from zip file
                                path += "\\" + ZipStorageProvider.GetZipFileName("template.zip");

                                settings.TemporaryFilesPath = path;
                                settings.SourceFilePath = path;
                                settings.TemporaryFilesCreated = true;
                            }
                            else
                            {
                                // Init the settings
                                settings.TemporaryFilesCreated = false;
                                settings.SourceFilePath = Server.MapPath(wi.WebTemplateFileName);
                            }

                            settings.RefreshMacroSecurity = true;
                            settings.IsNewSite = true;

                            if (!File.Exists(settings.SourceFilePath))
                            {
                                try
                                {
                                    ImportProvider.CreateTemporaryFiles(settings);
                                }
                                catch (Exception ex)
                                {
                                    SetErrorLabel(ex.Message);
                                    e.Cancel = true;
                                    return;
                                }
                            }

                            // Import all, but only add new data
                            settings.ImportType = ImportTypeEnum.AllNonConflicting;
                            settings.ImportOnlyNewObjects = true;
                            settings.CopyFiles = false;

                            // Allow bulk inserts for faster import, web templates must be consistent enough to allow this without collisions
                            settings.AllowBulkInsert = true;

                            ltlScriptAfter.Text = ScriptHelper.GetScript(
                                "var actDiv = document.getElementById('actDiv'); \n" +
                                "if (actDiv != null) { actDiv.style.display='block'; } \n" +
                                "var buttonsDiv = document.getElementById('buttonsDiv'); if (buttonsDiv != null) { buttonsDiv.disabled=true; } \n" +
                                "BTN_Disable('" + NextButton.ClientID + "'); \n" +
                                "StartSelectionTimer();"
                                );

                            // Preselect objects asynchronously
                            ctrlAsyncSelection.Parameter = "N";
                            ctrlAsyncSelection.RunAsync(SelectObjects, WindowsIdentity.GetCurrent());

                            e.Cancel = true;
                        }
                        catch (Exception ex)
                        {
                            SetErrorLabel(ex.Message);
                            e.Cancel = true;
                            return;
                        }
                    }
                    else
                    {
                        siteDetails.SiteName = null;
                        siteDetails.SiteDisplayName = null;
                        selectTemplate.ReloadData();
                    }

                    wzdImport.ActiveStepIndex++;
                }
                break;

            // Template selection
            case 1:
                {
                    if (!selectTemplate.ApplySettings())
                    {
                        e.Cancel = true;
                        return;
                    }

                    // Init the settings
                    WebTemplateInfo wi = WebTemplateInfoProvider.GetWebTemplateInfo(selectTemplate.WebTemplateId);
                    if (wi == null)
                    {
                        throw new Exception("Web template not found.");
                    }

                    settings.IsWebTemplate = true;

                    string path = Server.MapPath(wi.WebTemplateFileName);
                    if (File.Exists(path + "\\template.zip"))
                    {
                        // Template from zip file
                        path += "\\" + ZipStorageProvider.GetZipFileName("template.zip");

                        settings.TemporaryFilesPath = path;
                        settings.SourceFilePath = path;
                        settings.TemporaryFilesCreated = true;
                    }
                    else
                    {
                        // Template from folder
                        settings.TemporaryFilesCreated = false;
                        settings.SourceFilePath = path;
                        try
                        {
                            ImportProvider.CreateTemporaryFiles(settings);
                        }
                        catch (Exception ex)
                        {
                            SetErrorLabel(ex.Message);
                            e.Cancel = true;
                            return;
                        }
                    }

                    settings.RefreshMacroSecurity = true;
                    settings.IsNewSite = true;

                    // Import all, but only add new data
                    settings.ImportType = ImportTypeEnum.AllNonConflicting;
                    settings.ImportOnlyNewObjects = true;
                    settings.CopyFiles = false;

                    // Allow bulk inserts for faster import, web templates must be consistent enough to allow this without collisions
                    settings.AllowBulkInsert = true;

                    ltlScriptAfter.Text = ScriptHelper.GetScript(
                        "var actDiv = document.getElementById('actDiv');\n" +
                        "if (actDiv != null) { actDiv.style.display='block'; }\n" +
                        "var buttonsDiv = document.getElementById('buttonsDiv');\n" +
                        "if (buttonsDiv != null) { buttonsDiv.disabled=true; }\n" +
                        "BTN_Disable('" + NextButton.ClientID + "');\n" +
                        "BTN_Disable('" + PreviousButton.ClientID + "');\n" +
                        "StartSelectionTimer();"
                        );

                    // Preselect objects asynchronously
                    ctrlAsyncSelection.Parameter = "T";
                    ctrlAsyncSelection.RunAsync(SelectObjects, WindowsIdentity.GetCurrent());

                    e.Cancel = true;
                }
                break;

            // Site details
            case 2:
                if (!siteDetails.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }

                // Update settings
                ImportSettings = siteDetails.Settings;

                if (!siteType.SelectTemplate && (ImportSettings.SiteName == InfoHelper.CODENAME_AUTOMATIC))
                {
                    ImportSettings.SiteName = ValidationHelper.GetCodeName(settings.SiteDisplayName);
                }

                Culture = siteDetails.Culture;

                pnlImport.ReloadData(true);
                wzdImport.ActiveStepIndex++;
                break;

            // Objects selection
            case 3:
                if (!pnlImport.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }

                // Check licenses
                string error = ImportExportControl.CheckLicenses(settings);
                if (!string.IsNullOrEmpty(error))
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
            case 4:
                PreviousButton.Visible = false;

                CultureHelper.SetPreferredCulture(Culture);
                if (siteType.SelectTemplate)
                {
                    // Done
                    finishSite.Domain = Domain;
                    finishSite.SiteIsRunning = SiteIsRunning;
                    wzdImport.ActiveStepIndex = 6;
                }
                else
                {
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
                }
                break;

            case 5:
                finishSite.Domain = Domain;
                finishSite.SiteIsRunning = SiteIsRunning;
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
        ltlScript.Text += ScriptHelper.GetScript("StopSelectionTimer();");
    }


    protected void ctrlAsyncSelection_OnFinished(object sender, EventArgs e)
    {
        string param = ValidationHelper.GetString(ctrlAsyncSelection.Parameter, "");
        if (param == "N")
        {
            // Stop the timer
            const string script = "StopSelectionTimer();";

            // Init control
            siteDetails.SiteName = "";
            siteDetails.SiteDisplayName = "";
            siteDetails.DomainName = RequestContext.FullDomain;
            siteDetails.DisplayCulture = true;
            siteDetails.ReloadData();

            wzdImport.ActiveStepIndex += 2;

            ltlScriptAfter.Text += ScriptHelper.GetScript(script);
        }
        else if (param == "T")
        {
            // Init control
            siteDetails.DomainName = RequestContext.FullDomain;
            siteDetails.DisplayCulture = false;
            siteDetails.ReloadData();

            wzdImport.ActiveStepIndex++;
        }
    }


    protected void ctlAsyncImport_OnFinished(object sender, EventArgs e)
    {
        try
        {
            if (!siteType.SelectTemplate)
            {
                ImportPostProcess();
            }
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
            else
            {
                if (!ImportManager.Settings.IsWarning() && !ImportManager.Settings.IsError())
                {
                    PreviousButton.Visible = false;
                    CultureHelper.SetPreferredCulture(Culture);
                    if (siteType.SelectTemplate)
                    {
                        // Done
                        finishSite.Domain = Domain;
                        wzdImport.ActiveStepIndex = 6;
                    }
                    else
                    {
                        wzdImport.ActiveStepIndex += 1;
                    }
                }
            }

            // Stop the timer
            ltlScriptAfter.Text += ScriptHelper.GetScript("StopSelectionTimer();");
        }
    }

    #endregion


    #region "Other methods"

    private bool CausedPostback(params Control[] controls)
    {
        foreach (Control control in controls)
        {
            string uniqueID = control.UniqueID;
            bool toReturn = (Request.Form[uniqueID] != null) || ((Request.Form[Page.postEventSourceID] != null) && Request.Form[Page.postEventSourceID].Equals(uniqueID, StringComparison.OrdinalIgnoreCase)) || ((Request.Form[uniqueID + ".x"] != null) && (Request.Form[uniqueID + ".y"] != null));
            if (toReturn)
            {
                return true;
            }
        }
        return false;
    }


    protected void InitializeHeader()
    {
        int stepIndex = wzdImport.ActiveStepIndex + 1;

        switch (wzdImport.ActiveStepIndex)
        {
            case 0:
                ucHeader.Header = GetString("NewSite_ChooseSite.StepTitle");
                ucHeader.Description = GetString("NewSite_ChooseSite.StepDesc");
                break;

            case 1:
                ucHeader.Header = GetString("NewSite_ChooseWebTemplate.StepTitle");
                ucHeader.Description = GetString("NewSite_ChooseWebTemplate.StepDesc");
                break;

            case 2:
                stepIndex = siteType.SelectTemplate ? 3 : 2;
                ucHeader.Header = GetString("NewSite_SiteDetails.StepTitle");
                ucHeader.Description = GetString("NewSite_SiteDetails.StepDesc");
                break;

            case 3:
                stepIndex = siteType.SelectTemplate ? 4 : 3;
                ucHeader.Header = GetString("ImportPanel.ObjectsSelectionHeader");
                ucHeader.Description = GetString("ImportPanel.ObjectsSelectionDescription");
                break;

            case 4:
                stepIndex = siteType.SelectTemplate ? 5 : 4;
                ucHeader.Header = GetString("ImportPanel.ObjectsProgressHeader");
                ucHeader.Description = GetString("ImportPanel.ObjectsProgressDescription");
                break;

            case 5:
                stepIndex = 5;
                ucHeader.Header = GetString("NewSite_ChooseMasterTemplate.StepTitle");
                ucHeader.Description = GetString("NewSite_ChooseMasterTemplate.StepDesc");
                break;

            case 6:
                stepIndex = 6;
                ucHeader.Header = GetString("NewSite_Finish.StepTitle");
                ucHeader.Description = GetString("NewSite_Finish.StepDesc");
                break;
        }

        ucHeader.Title = string.Format(GetString("NewSite_Step"), stepIndex, 6);
    }


    // Preselect objects
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
    /// Displays error alert label with given text
    /// </summary>
    /// <param name="text">Text to display</param>
    private void SetErrorLabel(string text)
    {
        lblError.Text = text;
    }


    /// <summary>
    /// Clears error alert label
    /// </summary>
    private void ClearErrorLabel()
    {
        lblError.Text = "";
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

    #endregion
}