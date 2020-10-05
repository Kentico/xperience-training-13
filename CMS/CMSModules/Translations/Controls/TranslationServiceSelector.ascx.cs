using System;
using System.Collections.Generic;
using System.Data;

using CMS.Base;
using CMS.Helpers;

using System.Linq;
using System.Text;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.TranslationServices;
using CMS.UIControls;


public partial class CMSModules_Translations_Controls_TranslationServiceSelector : CMSUserControl
{
    #region "Variables"

    private readonly string defaultCulture = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
    private readonly string currentCulture = LocalizationContext.CurrentCulture.CultureCode;

    private bool mAnyServiceAvailable;
    private bool mDisplayMachineServices = true;
    private string mMachineServiceSuffix;
    private TreeProvider mTreeProvider;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets TreeProvider to use.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider());
        }
        set
        {
            mTreeProvider = value;
        }
    }


    /// <summary>
    /// Gets number of services displayed.
    /// </summary>
    public int DisplayedServicesCount
    {
        get;
        private set;
    }


    /// <summary>
    /// If true, the name of the service is displayed when only one is available.
    /// </summary>
    public bool DisplayOnlyServiceName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if option for separate submission creation for each document should be displayed.
    /// </summary>
    public bool DisplaySeparateSubmissionOption
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if separate submission for each document should be created.
    /// </summary>
    public bool CreateSeparateSubmission
    {
        get
        {
            return chkSeparateSubmissions.Checked;
        }
        set
        {
            chkSeparateSubmissions.Checked = value;
        }
    }


    /// <summary>
    /// Gets name of the service displayed (if only one is displayed, if more services are displayed, null is returned)
    /// </summary>
    public string DisplayedServiceName
    {
        get;
        private set;
    }


    /// <summary>
    /// Gets or sets NodeID which to translates
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets TranslationSettings object which will be used to process/submit translation.
    /// </summary>
    public TranslationSettings TranslationSettings
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if the translation should process binary fields.
    /// </summary>
    public bool ProcessBinary
    {
        get
        {
            return chkProcessBinary.Checked;
        }
        set
        {
            chkProcessBinary.Checked = value;
        }
    }


    /// <summary>
    /// Gets or sets the instructions of the submission.
    /// </summary>
    public string Instructions
    {
        get
        {
            return txtInstruction.Text;
        }
        set
        {
            txtInstruction.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets source language of translation.
    /// </summary>
    public string FromLanguage
    {
        get
        {
            return selectCultureElem.Value.ToString();
        }
        set
        {
            selectCultureElem.Value = value;
        }
    }


    /// <summary>
    /// Gets target languages of translation.
    /// </summary>
    public HashSet<string> TargetLanguages
    {
        get
        {
            // Get target culture
            string targetCulture;
            if (DisplayTargetlanguage)
            {
                targetCulture = selectTargetCultureElem.Value.ToString();
            }
            else
            {
                // Use target language from settings or current culture if target not set
                targetCulture = TranslationSettings.TargetLanguages.FirstOrDefault() ?? currentCulture;
            }

            return new HashSet<string>(targetCulture.Split(';').Where(s => !String.IsNullOrEmpty(s) && !s.EqualsCSafe(FromLanguage)), StringComparer.OrdinalIgnoreCase);
        }
    }


    /// <summary>
    /// Gets or sets the priority of the submission.
    /// </summary>
    public int Priority
    {
        get
        {
            return ValidationHelper.GetInteger(drpPriority.Value, 1);
        }
        set
        {
            drpPriority.Value = value.ToString();
        }
    }


    /// <summary>
    /// Gets or sets the deadline of the submission.
    /// </summary>
    public DateTime Deadline
    {
        get
        {
            return dtDeadline.SelectedDateTime;
        }
        set
        {
            dtDeadline.SelectedDateTime = value;
        }
    }


    /// <summary>
    /// Gets or sets the selected service name.
    /// </summary>
    public string SelectedService
    {
        get
        {
            return hdnSelectedName.Value;
        }
        set
        {
            hdnSelectedName.Value = value;
        }
    }


    /// <summary>
    /// Determines whether to display machine translation services.
    /// </summary>
    public bool DisplayMachineServices
    {
        get
        {
            return mDisplayMachineServices;
        }
        set
        {
            mDisplayMachineServices = value;
        }
    }


    /// <summary>
    /// Determines whether to display target language selection.
    /// </summary>
    public bool DisplayTargetlanguage
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if current culture should be used as a default selection for target culture
    /// </summary>
    public bool UseCurrentCultureAsDefaultTarget
    {
        get;
        set;
    }


    /// <summary>
    /// String which will be addad as a suffix of each machine translation service.
    /// </summary>
    public string MachineServiceSuffix
    {
        get
        {
            return mMachineServiceSuffix ?? GetString("translationservice.machineservicesuffix");
        }
        set
        {
            mMachineServiceSuffix = value;
        }
    }


    /// <summary>
    /// Indicates if there is any enabled service available in the selector.
    /// </summary>
    public bool AnyServiceAvailable
    {
        get
        {
            return mAnyServiceAvailable;
        }
    }


    /// <summary>
    /// Returns true if there is at least one source culture which can be used for translation.
    /// </summary>
    public bool SourceCultureAvailable
    {
        get
        {
            return selectCultureElem.UniSelector.HasData;
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        if (!RequestHelper.IsPostBack())
        {
            // Preselect default values
            dtDeadline.SelectedDateTime = DateTime.Now.AddDays(7);
            drpPriority.Value = 1;

            HandlePreselectedCultures();
        }

        // Set visibility of the target language selector
        plcTargetLang.Visible = DisplayTargetlanguage;
        if (DisplayTargetlanguage)
        {
            selectTargetCultureElem.UniSelector.OnSelectionChanged += TargetLanguageSelector_OnSelectionChanged;

            SetupTargetLanguageDropDownWhereCondition();
        }

        if (TranslationSettings != null)
        {
            SetupSourceLanguageDropDownWhereCondition(TranslationSettings.TargetLanguages);
        }

        SetupDisplayNameFormat();

        if (RequestHelper.IsCallback())
        {
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.Append(@"
function SelectService(serviceName, displaySeparateSubmission, supportsInstructions, supportsPriority, supportsAttachments, supportsDeadline) {
    var nameElem = document.getElementById('", hdnSelectedName.ClientID, @"');
    if (nameElem != null) {
        nameElem.value = serviceName;
 
        document.getElementById('pnlSeparateSubmissions').style.display = (displaySeparateSubmission ? '' : 'none');
        document.getElementById('pnlInstructions').style.display = (supportsInstructions ? '' : 'none');
        document.getElementById('pnlPriority').style.display = (supportsPriority ? '' : 'none');
        document.getElementById('pnlDeadline').style.display = (supportsDeadline ? '' : 'none');
        document.getElementById('pnlProcessBinary').style.display = (supportsAttachments ? '' : 'none');

        var selectButton = document.getElementById('rad' + serviceName);
        if (selectButton != null) {
            selectButton.checked = 'checked';
        }
    }
}");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "TranslationServiceSelector", sb.ToString(), true);

        ReloadData();
    }


    private void SetupDisplayNameFormat()
    {
        string format = "{% CultureName %}{% if (CultureCode == \"" + defaultCulture + "\") { \" \" +\"" + GetString("general.defaultchoice") + "\" } %}";
        selectCultureElem.DropDownCultures.AutoPostBack = DisplayTargetlanguage;
        selectCultureElem.UniSelector.DisplayNameFormat = format;
        selectTargetCultureElem.UniSelector.DisplayNameFormat = format;
    }


    private void HandlePreselectedCultures()
    {
        // Preselect current culture if different from default one, otherwise preselect default one
        if (UseCurrentCultureAsDefaultTarget)
        {
            if (!currentCulture.EqualsCSafe(defaultCulture, true))
            {
                // Preselect cultures
                selectTargetCultureElem.Value = currentCulture;
                selectCultureElem.Value = defaultCulture;

                TranslationSettings.TargetLanguages.Add(currentCulture);
            }
            else
            {
                selectCultureElem.Value = defaultCulture;
            }

            selectTargetCultureElem.Reload(true);
        }
        else
        {
            selectCultureElem.Value = defaultCulture;
        }
    }


    /// <summary>
    /// Setups source language drop down list control.
    /// </summary>
    /// <param name="targetCultures">List of target cultures which should not be in te source selector</param>
    private void SetupSourceLanguageDropDownWhereCondition(HashSet<string> targetCultures)
    {
        var condition = new WhereCondition();
        if (targetCultures.Count > 0)
        {
            condition.WhereNotIn("CultureCode", targetCultures.ToList());
        }

        if (NodeID > 0)
        {
            // Get source culture list from original node if current node is linked
            var node = TreeProvider.SelectSingleNode(NodeID, TreeProvider.ALL_CULTURES, true, false);
            var sourceNodeID = node.OriginalNodeID;

            condition.WhereIn("CultureCode", new IDQuery("cms.document", "DocumentCulture").WhereEquals("DocumentNodeID", sourceNodeID));
        }

        selectCultureElem.AdditionalWhereCondition = condition.ToString(true);
    }


    private void TargetLanguageSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Update where condition
        SetupSourceLanguageDropDownWhereCondition(TargetLanguages);

        selectCultureElem.ReloadData();
        selectCultureElem.Reload(true);
    }


    /// <summary>
    /// Setups target language selector control.
    /// </summary>
    private void SetupTargetLanguageDropDownWhereCondition()
    {
        WhereCondition condition = new WhereCondition();

        var culturesAreEqual = currentCulture.EqualsCSafe(defaultCulture, true);
        var selectedSourceCulture = selectCultureElem.DropDownCultures.SelectedValue;

        string notTargetLanguage = null;

        if (!String.IsNullOrEmpty(selectedSourceCulture))
        {
            // Use source language if selected
            notTargetLanguage = selectedSourceCulture;
        }
        else if (!culturesAreEqual || !UseCurrentCultureAsDefaultTarget)
        {
            // Use default culture if source and target languages are equal
            notTargetLanguage = defaultCulture;
        }

        if (!String.IsNullOrEmpty(selectedSourceCulture))
        {
            condition.WhereNotEquals("CultureCode", notTargetLanguage);
        }

        if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && CurrentUser.UserHasAllowedCultures)
        {
            condition.WhereIn("CultureID", new IDQuery(UserCultureInfo.OBJECT_TYPE, "CultureID").WhereEquals("UserID", CurrentUser.UserID).WhereEquals("SiteID", CurrentSite.SiteID));
        }

        selectTargetCultureElem.AdditionalWhereCondition = condition.ToString(true);
    }


    /// <summary>
    /// Selects correct value.
    /// </summary>
    private void ReloadData()
    {
        string where = "TranslationServiceEnabled = 1";
        if (!DisplayMachineServices)
        {
            where += " AND TranslationServiceIsMachine = 0";
        }

        // Get services
        var data = TranslationServiceInfoProvider.GetTranslationServices(where, "TranslationServiceIsMachine DESC, TranslationServiceDisplayName ASC", 0, "TranslationServiceDisplayName, TranslationServiceName, TranslationServiceIsMachine, TranslationServiceSupportsPriority, TranslationServiceSupportsInstructions, TranslationServiceSupportsDeadline");
        if (DataHelper.DataSourceIsEmpty(data))
        {
            return;
        }

        bool allowBinary = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSAllowAttachmentTranslation");
        string selected = SelectedService;
        string lastDisplayName = null;

        int i = 0;

        string machSelectScript = null;
        ltlServices.Text += "<div class=\"radio-list-vertical\">";

        foreach (DataRow dr in data.Tables[0].Rows)
        {
            string codeName = ValidationHelper.GetString(dr["TranslationServiceName"], "");

            // Check availability
            if (!TranslationServiceHelper.IsServiceAvailable(codeName, SiteContext.CurrentSiteName))
            {
                continue;
            }

            if (string.IsNullOrEmpty(selected) && (i == 0))
            {
                selected = codeName;
            }

            bool isMachine = ValidationHelper.GetBoolean(dr["TranslationServiceIsMachine"], false);
            string displayName = ValidationHelper.GetString(dr["TranslationServiceDisplayName"], "");
            bool supportsInstructions = ValidationHelper.GetBoolean(dr["TranslationServiceSupportsInstructions"], false);
            bool supportsPriority = ValidationHelper.GetBoolean(dr["TranslationServiceSupportsPriority"], false);
            bool supportsDeadline = ValidationHelper.GetBoolean(dr["TranslationServiceSupportsDeadline"], false);

            if (isMachine && !string.IsNullOrEmpty(MachineServiceSuffix))
            {
                displayName += MachineServiceSuffix;
            }

            string selectScript = string.Format("SelectService({0}, {1}, {2}, {3}, {4}, {5})", ScriptHelper.GetString(codeName), (!isMachine && DisplaySeparateSubmissionOption ? "true" : "false"), (supportsInstructions ? "true" : "false"), (supportsPriority ? "true" : "false"), (!isMachine && allowBinary ? "true" : "false"), (supportsDeadline ? "true" : "false"));

            bool isSelected = string.Equals(selected, codeName, StringComparison.CurrentCultureIgnoreCase);
            if (isSelected)
            {
                SelectedService = selected;
                if (string.IsNullOrEmpty(machSelectScript))
                {
                    machSelectScript = selectScript;
                }
            }

            string radioBtn = "<div class=\"radio\"><input id=\"rad" + codeName + "\" " + (isSelected ? "checked=\"checked\"" : "") + " type=\"radio\" name=\"services\" value=\"" + codeName + "\" onclick=\"" + selectScript + "\" />";
            radioBtn += "<label for=\"rad" + codeName + "\">" + HTMLHelper.HTMLEncode(displayName) + "</label></div>";
            lastDisplayName = displayName;

            mAnyServiceAvailable = true;

            ltlServices.Text += radioBtn;
            i++;
        }

        ltlServices.Text += "</div>";

        // If only one service is available, display it in a different way
        if (i == 1)
        {
            DisplayedServiceName = lastDisplayName;
            ltlServices.Text = DisplayOnlyServiceName ? "<strong>" + HTMLHelper.HTMLEncode(lastDisplayName) + "</strong>" : String.Empty;
        }

        if (!String.IsNullOrEmpty(machSelectScript))
        {
            // Register selection script for first service
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "TranslationServiceSelectorSelection", machSelectScript, true);
        }

        DisplayedServicesCount = i;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Validates the data. Returns error msg if something is not ok.
    /// </summary>
    public string ValidateData()
    {
        if (dtDeadline.SelectedDateTime < DateTime.Now)
        {
            // DateTime from the past is not allowed
            return GetString("translationservice.invaliddeadline");
        }

        // Validate maximal length (not supported by the control itself)
        var instructionsLength = txtInstruction.Text.Length;
        var maximumLength = txtInstruction.MaxLength;
        if (instructionsLength > maximumLength)
        {
            return string.Format(GetString("translationservice.instructionstoolong"), maximumLength, instructionsLength);
        }

        if ((TargetLanguages.Count == 0) || String.IsNullOrEmpty(TargetLanguages.FirstOrDefault()))
        {
            return GetString("translationservice.invalidtargetlanguage");
        }

        return null;
    }


    /// <summary>
    /// Submits the node for translation. Does not check the permissions, you need to check it before calling this method.
    /// </summary>
    public string SubmitToTranslation()
    {
        string err = ValidateData();
        if (!string.IsNullOrEmpty(err))
        {
            return err;
        }

        var settings = TranslationSettings ?? new TranslationSettings();
        if (settings.TargetLanguages.Count == 0)
        {
            settings.TargetLanguages.Add(currentCulture);
        }

        settings.SourceLanguage = FromLanguage;
        settings.Instructions = Instructions;
        settings.Priority = Priority;
        settings.TranslateAttachments = ProcessBinary;
        settings.TranslationDeadline = Deadline;
        settings.TranslationServiceName = SelectedService;

        var node = TreeProvider.GetOriginalNode(DocumentHelper.GetDocument(NodeID, settings.SourceLanguage, true, TreeProvider));

        TranslationSubmissionInfo submissionInfo;
        return TranslationServiceHelper.SubmitToTranslation(settings, node, out submissionInfo);
    }

    #endregion
}
