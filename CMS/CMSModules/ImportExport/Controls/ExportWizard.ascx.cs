using System;
using System.Collections;
using System.Linq;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Core;
using CMS.EventLog;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.UIControls;

using IOExceptions = System.IO;

public partial class CMSModules_ImportExport_Controls_ExportWizard : CMSUserControl
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
    /// Export manager.
    /// </summary>
    public ExportManager ExportManager
    {
        get
        {
            string key = "exManagers_" + ProcessGUID;
            if (mManagers[key] == null)
            {
                // Restart of the application
                if (ApplicationInstanceGUID != SystemHelper.ApplicationInstanceGUID)
                {
                    // Lock section to avoid multiple log same error
                    lock (mLock)
                    {
                        LogStatusEnum progressLog = ExportSettings.GetProgressState();
                        if (progressLog == LogStatusEnum.Info)
                        {
                            ExportSettings.LogProgressState(LogStatusEnum.UnexpectedFinish, GetString("SiteExport.ApplicationRestarted"));
                        }
                    }
                }

                ExportManager em = new ExportManager(ExportSettings);
                mManagers[key] = em;
            }

            return (ExportManager)mManagers[key];
        }
        set
        {
            string key = "exManagers_" + ProcessGUID;
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
    /// Site ID.
    /// </summary>
    public int SiteId
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["SiteId"], 0);
        }
        set
        {
            ViewState["SiteId"] = value;
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
            return "Export_" + ProcessGUID + "_Settings";
        }
    }


    /// <summary>
    /// Export settings stored in viewstate.
    /// </summary>
    public SiteExportSettings ExportSettings
    {
        get
        {
            SiteExportSettings settings = (SiteExportSettings)AbstractImportExportSettings.GetFromPersistentStorage(PersistentSettingsKey);
            if (settings == null)
            {
                if (wzdExport.ActiveStepIndex == 0)
                {
                    settings = GetNewSettings();
                }
                else
                {
                    throw new Exception("[ExportWizard.ExportSettings]: Export settings has been lost.");
                }
            }
            return settings;
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
            return wzdExport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepFinishButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Previous button.
    /// </summary>
    public LocalizedButton PreviousButton
    {
        get
        {
            return wzdExport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepPreviousButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton
    {
        get
        {
            return wzdExport.FindControl("StartNavigationTemplateContainerID").FindControl("StepNextButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Cancel button.
    /// </summary>
    public LocalizedButton CancelButton
    {
        get
        {
            return wzdExport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepCancelButton") as LocalizedButton;
        }
    }

    #endregion


    #region "Events handling"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Handle export settings
        if (!RequestHelper.IsCallback() && !RequestHelper.IsPostBack())
        {
            ExportSettings = GetNewSettings();
        }

        if (!RequestHelper.IsCallback())
        {
            // Display BETA warning
            lblBeta.Visible = CMSVersion.IsBetaVersion();
            lblBeta.Text = string.Format(GetString("export.BETAwarning"), CMSVersion.GetFriendlySystemVersion(false));

            bool notTargetPermissions = false;
            bool notTempPermissions = false;

            ctrlAsyncSelection.OnFinished += CtrlAsyncSelectionOnFinished;
            ctrlAsyncSelection.OnError += CtrlAsyncSelectionOnError;

            ctlAsyncExport.OnCancel += ctlAsyncExport_OnCancel;

            // Init steps
            if (wzdExport.ActiveStepIndex < 2)
            {
                configExport.Settings = ExportSettings;
                if (!RequestHelper.IsPostBack())
                {
                    configExport.SiteId = SiteId;
                }

                pnlExport.Settings = ExportSettings;

                // Ensure directories and check permissions
                try
                {
                    DirectoryHelper.EnsureDiskPath(ExportSettings.TargetPath + "\\temp.file", ExportSettings.WebsitePath);
                    notTargetPermissions = !DirectoryHelper.CheckPermissions(ExportSettings.TargetPath, true, true, false, false);
                }
                catch (UnauthorizedAccessException)
                {
                    notTargetPermissions = true;
                }
                catch (IOExceptions.IOException ex)
                {
                    pnlWrapper.Visible = false;
                    SetAlertLabel(lblErrorBlank, ex.Message);
                    return;
                }
                try
                {
                    DirectoryHelper.EnsureDiskPath(ExportSettings.TemporaryFilesPath + "\\temp.file", ExportSettings.WebsitePath);
                    notTempPermissions = !DirectoryHelper.CheckPermissions(ExportSettings.TemporaryFilesPath, true, true, false, false);
                }
                catch (UnauthorizedAccessException)
                {
                    notTempPermissions = true;
                }
                catch (IOExceptions.IOException ex)
                {
                    pnlWrapper.Visible = false;
                    SetAlertLabel(lblErrorBlank, ex.Message);
                    return;
                }
            }

            if (notTargetPermissions || notTempPermissions)
            {
                string folder = (notTargetPermissions) ? ExportSettings.TargetPath : ExportSettings.TemporaryFilesPath;
                pnlWrapper.Visible = false;
                pnlPermissions.Visible = true;
                SetAlertLabel(lblErrorBlank, String.Format(GetString("ExportSite.ErrorPermissions"), folder, WindowsIdentity.GetCurrent().Name));
                lnkPermissions.Target = "_blank";
                lnkPermissions.Text = GetString("Install.ErrorPermissions");
                lnkPermissions.NavigateUrl = DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_DISKPERMISSIONS_LINK);
            }
            else
            {
                // Try to delete temporary files from previous export
                if (!RequestHelper.IsPostBack())
                {
                    try
                    {
                        ExportProvider.DeleteTemporaryFiles(ExportSettings, false);
                    }
                    catch (Exception ex)
                    {
                        pnlWrapper.Visible = false;
                        SetAlertLabel(lblErrorBlank, GetString("ImportSite.ErrorDeletionTemporaryFiles") + " " + ex.Message);
                        return;
                    }
                }

                ControlsHelper.EnsureScriptManager(Page).EnablePageMethods = true;

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
}}
",
                    CancelButton.ClientID,
                    FinishButton.ClientID,
                    lblError.LabelClientID,
                    lblWarning.LabelClientID,
                    pnlError.ClientID,
                    pnlWarning.ClientID
                );

                // Register the script to perform get flags for showing buttons retrieval callback
                ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Finished", ScriptHelper.GetScript(script));

                // Add cancel button attribute
                CancelButton.Attributes.Add("onclick", ctlAsyncExport.GetCancelScript(true) + "return false;");

                wzdExport.NextButtonClick += wzdExport_NextButtonClick;
                wzdExport.PreviousButtonClick += wzdExport_PreviousButtonClick;
                wzdExport.FinishButtonClick += wzdExport_FinishButtonClick;

                if (!RequestHelper.IsPostBack())
                {
                    configExport.InitControl();
                }
            }
        }
    }


    private void ctlAsyncExport_OnCancel(object sender, EventArgs e)
    {
        wzdExport_FinishButtonClick(sender, null);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        InitializeHeader();

        // Button click script
        const string afterScript =
@"var exClicked = false;
function exNextStepAction()
{
   if(!exClicked)
   {
     exClicked = true;
     return true;
   }
   return false;
}";

        ltlScriptAfter.Text += ScriptHelper.GetScript(afterScript);

        EnsureDefaultButton();

        ShowObjectTypeCycleWarning();

        InitAlertLabels();
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);

        // Save the settings
        if (wzdExport.ActiveStep.StepType != WizardStepType.Finish)
        {
            ExportSettings.SavePersistent();
        }
    }


    private void wzdExport_FinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        if (String.IsNullOrEmpty(FinishUrl))
        {
            FinishUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Sites", false);
        }

        URLHelper.Redirect(UrlResolver.ResolveUrl(FinishUrl));
    }


    private void wzdExport_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
        wzdExport.ActiveStepIndex = e.NextStepIndex;
    }


    private void wzdExport_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        switch (e.CurrentStepIndex)
        {
            case 0:
                // Apply settings
                if (!configExport.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }

                // Update settings
                ExportSettings = configExport.Settings;

                if (!configExport.ExportHistory)
                {
                    ltlScriptAfter.Text = ScriptHelper.GetScript(
@"var actDiv = document.getElementById('actDiv');
if (actDiv != null) { actDiv.style.display='block'; }
var buttonsDiv = document.getElementById('buttonsDiv');
if (buttonsDiv != null) { buttonsDiv.disabled=true; }
BTN_Disable('" + NextButton.ClientID + @"');
StartSelectionTimer();");

                    // Select objects asynchronously
                    ctrlAsyncSelection.RunAsync(SelectObjects, WindowsIdentity.GetCurrent());
                    e.Cancel = true;
                }
                else
                {
                    pnlExport.Settings = ExportSettings;
                    pnlExport.ReloadData();

                    wzdExport.ActiveStepIndex = e.NextStepIndex;
                }
                break;

            case 1:
                // Apply settings
                if (!pnlExport.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }
                ExportSettings = pnlExport.Settings;

                // Delete temporary files
                try
                {
                    ExportProvider.DeleteTemporaryFiles(ExportSettings, true);
                }
                catch (Exception ex)
                {
                    SetErrorLabel(ex.Message);
                    e.Cancel = true;
                    return;
                }

                try
                {
                    // Save export history
                    ExportHistoryInfo history = new ExportHistoryInfo
                    {
                        ExportDateTime = DateTime.Now,
                        ExportFileName = ExportSettings.TargetFileName,
                        ExportSettings = ExportSettings.GetXML(),
                        ExportSiteID = ExportSettings.SiteId,
                        ExportUserID = MembershipContext.AuthenticatedUser.UserID
                    };

                    ExportHistoryInfo.Provider.Set(history);
                }
                catch (Exception ex)
                {
                    SetErrorLabel(ex.Message);
                    pnlError.ToolTip = EventLogProvider.GetExceptionLogMessage(ex);
                    e.Cancel = true;
                    return;
                }

                if (ExportSettings.SiteId > 0)
                {
                    ExportSettings.EventLogSource = String.Format(ExportSettings.GetAPIString("ExportSite.EventLogSiteSource", "Export '{0}' site"), ResHelper.LocalizeString(ExportSettings.SiteInfo.DisplayName));
                }

                // Start asynchronous export
                var manager = ExportManager;

                ExportSettings.LogContext = ctlAsyncExport.CurrentLog;
                manager.Settings = ExportSettings;

                ctlAsyncExport.RunAsync(manager.Export, WindowsIdentity.GetCurrent());

                wzdExport.ActiveStepIndex = e.NextStepIndex;

                break;
        }
    }


    protected void CtrlAsyncSelectionOnError(object sender, EventArgs e)
    {
        SetErrorLabel(((AsyncControl)sender).Worker.LastException.Message);

        ltlScript.Text += ScriptHelper.GetScript("StopSelectionTimer();");
    }


    protected void CtrlAsyncSelectionOnFinished(object sender, EventArgs e)
    {
        pnlExport.Settings = ExportSettings;
        pnlExport.ReloadData();

        wzdExport.ActiveStepIndex = 1;

        ltlScriptAfter.Text += ScriptHelper.GetScript("StopSelectionTimer();");
    }


    // Preselect objects
    private void SelectObjects(object parameter)
    {
        var settings = ExportSettings;

        settings.LoadDefaultSelection();
        settings.SavePersistent();
    }

    #endregion


    #region "Private methods"

    private void InitializeHeader()
    {
        ucHeader.Title = string.Format(GetString("ExportPanel.Title"), wzdExport.ActiveStepIndex + 1, wzdExport.WizardSteps.Count);

        switch (wzdExport.ActiveStepIndex)
        {
            case 0:
                ucHeader.Header = GetString("ExportPanel.ObjectsSettingsHeader");
                ucHeader.Description = GetString("ExportPanel.ObjectsSelectionSetting");
                break;

            case 1:
                ucHeader.Header = GetString("ExportPanel.ObjectsSelectionHeader");
                ucHeader.Description = GetString("ExportPanel.ObjectsSelectionDescription");
                break;

            case 2:
                ucHeader.Header = GetString("ExportPanel.ObjectsProgressHeader");
                ucHeader.Description = GetString("ExportPanel.ObjectsProgressDescription");
                break;
        }
    }


    private void EnsureDefaultButton()
    {
        switch (wzdExport.ActiveStep.StepType)
        {
            case WizardStepType.Start:
                Page.Form.DefaultButton = NextButton.UniqueID;
                break;
            case WizardStepType.Step:
                Page.Form.DefaultButton = wzdExport.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton").UniqueID;
                break;
            case WizardStepType.Finish:
                Page.Form.DefaultButton = FinishButton.UniqueID;
                break;
        }
    }


    /// <summary>
    /// Creates new settings object for export.
    /// </summary>
    private SiteExportSettings GetNewSettings()
    {
        SiteExportSettings result = new SiteExportSettings(MembershipContext.AuthenticatedUser)
        {
            WebsitePath = Server.MapPath("~/"),
            TargetPath = ImportExportHelper.GetSiteUtilsFolder() + "Export",
            PersistentSettingsKey = PersistentSettingsKey
        };

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
        if ((wzdExport.ActiveStepIndex == 1) && ImportExportHelper.ObjectTypeCycles.Any())
        {
            SetAlertLabel(lblWarning, String.Format(GetString("importexport.objecttypecycles"), String.Join("<br />", ImportExportHelper.ObjectTypeCycles.Select(HTMLHelper.HTMLEncode))));
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

    #endregion
}