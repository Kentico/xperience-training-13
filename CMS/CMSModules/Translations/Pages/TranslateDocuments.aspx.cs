using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataEngine.CollectionExtensions;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.TranslationServices;
using CMS.TranslationServices.Web.UI;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Translations_Pages_TranslateDocuments : CMSTranslationServiceModalPage
{
    #region "Private variables"

    private HashSet<string> targetCultures;

    private bool isSelect;
    private readonly List<int> nodeIds = new List<int>();
    private string[] nodeIdsArr;
    private int cancelNodeId;
    private CurrentUserInfo currentUser;

    private Hashtable mParameters;

    private bool allowTranslate = true;


    #endregion


    #region "Properties"

    /// <summary>
    /// Returns messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ctlAsyncLog.ProcessData.Error;
        }
        set
        {
            ctlAsyncLog.ProcessData.Error = value;
        }
    }


    /// <summary>
    /// Indicates whether action is multiple.
    /// </summary>
    private bool AllLevels
    {
        get
        {
            return QueryHelper.GetBoolean("alllevels", false);
        }
    }


    /// <summary>
    /// Gets selected class ID.
    /// </summary>
    private int ClassID
    {
        get
        {
            return QueryHelper.GetInteger("classid", 0);
        }
    }


    /// <summary>
    /// Where condition used for multiple actions.
    /// </summary>
    private string WhereCondition
    {
        get
        {
            string where = string.Empty;
            if (Parameters != null)
            {
                where = ValidationHelper.GetString(Parameters["where"], string.Empty);
            }
            return where;
        }
    }


    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Set up dialog mode
        IsDialog = QueryHelper.GetBoolean("dialog", false);

        base.OnPreInit(e);

        if (IsDialog)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
            if (Master == null)
            {
                return;
            }

            // Get footer placeholder
            var footer = (ContentPlaceHolder)Master.FindControl("plcFooter");
            if (footer == null)
            {
                return;
            }

            // Add button
            var button = new LocalizedButton
            {
                ButtonStyle = ButtonStyle.Primary,
                ResourceString = "general.translate",
                EnableViewState = false
            };
            button.Click += btnTranslate_Click;
            footer.Controls.Add(button);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script files
        ScriptHelper.RegisterCMS(this);
        ScriptHelper.RegisterScriptFile(this, "~/CMSModules/Content/CMSDesk/Operation.js");

        if (!QueryHelper.ValidateHash("hash"))
        {
            pnlContent.Visible = false;
            ShowError(GetString("dialogs.badhashtext"));
            return;
        }

        // Setup page title text and image
        PageTitle.TitleText = GetString("Content.TranslateTitle");
        EnsureDocumentBreadcrumbs(PageBreadcrumbs, action: PageTitle.TitleText);

        if (IsDialog)
        {
            RegisterModalPageScripts();
            RegisterEscScript();

            plcInfo.Visible = false;

            pnlButtons.Visible = false;
        }

        if (!TranslationServiceHelper.IsTranslationAllowed(CurrentSiteName))
        {
            pnlContent.Visible = false;
            ShowError(GetString("translations.translationnotallowed"));
            return;
        }

        // Initialize current user
        currentUser = MembershipContext.AuthenticatedUser;

        // Initialize events
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        isSelect = QueryHelper.GetBoolean("select", false);
        if (isSelect)
        {
            pnlDocList.Visible = false;
            pnlDocSelector.Visible = true;
            translationElem.DisplayMachineServices = false;
        }

        var currentCulture = LocalizationContext.CurrentCulture.CultureCode;
        var displayTargetLanguage = !IsDialog || isSelect;
        translationElem.DisplayTargetlanguage = displayTargetLanguage;

        // Get target culture(s)
        targetCultures = displayTargetLanguage ? translationElem.TargetLanguages : new HashSet<string>(new[] { QueryHelper.GetString("targetculture", currentCulture) });

        // Set the target settings
        var settings = new TranslationSettings();
        settings.TargetLanguages.AddRangeToSet(targetCultures);

        var useCurrentAsDefault = QueryHelper.GetBoolean("currentastargetdefault", false);
        if (!currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && currentUser.UserHasAllowedCultures && !currentUser.IsCultureAllowed(currentCulture, CurrentSiteName))
        {
            // Do not use current culture as default if user has no permissions to edit it
            useCurrentAsDefault = false;
        }

        translationElem.UseCurrentCultureAsDefaultTarget = useCurrentAsDefault;

        // Do not include default culture if it is current one
        string defaultCulture = CultureHelper.GetDefaultCultureCode(CurrentSiteName);
        if (useCurrentAsDefault && !string.Equals(currentCulture, defaultCulture, StringComparison.InvariantCultureIgnoreCase) && !RequestHelper.IsPostBack())
        {
            settings.TargetLanguages.Add(currentCulture);
        }

        translationElem.TranslationSettings = settings;
        allowTranslate = true;

        if (RequestHelper.IsCallback())
        {
            return;
        }

        // If not in select mode, load all the document IDs and check permissions
        // In select mode, documents are checked when the button is clicked
        if (!isSelect)
        {
            DataSet allDocs = null;
            TreeProvider tree = new TreeProvider();

            // Current Node ID to translate
            string parentAliasPath = string.Empty;
            if (Parameters != null)
            {
                parentAliasPath = ValidationHelper.GetString(Parameters["parentaliaspath"], string.Empty);
                nodeIdsArr = ValidationHelper.GetString(Parameters["nodeids"], string.Empty).Trim('|').Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (string.IsNullOrEmpty(parentAliasPath))
            {
                if (nodeIdsArr == null)
                {
                    // One document translation is requested
                    string nodeIdQuery = QueryHelper.GetString("nodeid", "");
                    if (nodeIdQuery != "")
                    {
                        // Mode of single node translation
                        pnlList.Visible = false;
                        chkSkipTranslated.Checked = false;

                        translationElem.NodeID = ValidationHelper.GetInteger(nodeIdQuery, 0);

                        nodeIdsArr = new[] { nodeIdQuery };
                    }
                    else
                    {
                        nodeIdsArr = new string[] { };
                    }
                }

                foreach (string nodeId in nodeIdsArr)
                {
                    int id = ValidationHelper.GetInteger(nodeId, 0);
                    if (id != 0)
                    {
                        nodeIds.Add(id);
                    }
                }
            }
            else
            {
                // Exclude root of the website from multiple translation requested by document listing bulk action
                var where = new WhereCondition(WhereCondition)
                    .WhereNotEquals("ClassName", SystemDocumentTypes.Root);

                allDocs = tree.SelectNodes(CurrentSiteName, parentAliasPath.TrimEnd(new[] { '/' }) + "/%",
                    TreeProvider.ALL_CULTURES, true, ClassID > 0 ? DataClassInfoProvider.GetClassName(ClassID) : TreeProvider.ALL_CLASSNAMES, where.ToString(true),
                    "DocumentName", AllLevels ? TreeProvider.ALL_LEVELS : 1, false, 0,
                    DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS + ",DocumentName,NodeParentID,NodeSiteID,NodeAliasPath");

                if (!DataHelper.DataSourceIsEmpty(allDocs))
                {
                    foreach (DataTable table in allDocs.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            nodeIds.Add(ValidationHelper.GetInteger(row["NodeID"], 0));
                        }
                    }
                }
            }

            if (nodeIds.Count > 0)
            {
                var where = new WhereCondition().WhereIn("NodeID", nodeIds).ToString(true);
                DataSet ds = allDocs ?? tree.SelectNodes(CurrentSiteName, "/%", TreeProvider.ALL_CULTURES, true, null, where, "DocumentName", TreeProvider.ALL_LEVELS, false);

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    string docList = null;

                    cancelNodeId = string.IsNullOrEmpty(parentAliasPath)
                        ? DataHelper.GetIntValue(ds.Tables[0].Rows[0], "NodeParentID")
                        : TreePathUtils.GetNodeIdByAliasPath(CurrentSiteName, parentAliasPath);

                    foreach (DataTable table in ds.Tables)
                    {
                        foreach (DataRow dr in table.Rows)
                        {
                            bool isLink = (dr["NodeLinkedNodeID"] != DBNull.Value);
                            string name = (string)dr["DocumentName"];
                            docList += HTMLHelper.HTMLEncode(name);
                            if (isLink)
                            {
                                docList += DocumentUIHelper.GetDocumentMarkImage(Page, DocumentMarkEnum.Link);
                            }
                            docList += "<br />";
                            lblDocuments.Text = docList;

                            // Set visibility of checkboxes
                            TreeNode node = TreeNode.New(ValidationHelper.GetString(dr["ClassName"], string.Empty), dr);

                            if (!TranslationServiceHelper.IsAuthorizedToTranslateDocument(node, currentUser, targetCultures))
                            {
                                allowTranslate = false;

                                plcMessages.AddError(String.Format(GetString("cmsdesk.notauthorizedtotranslatedocument"), HTMLHelper.HTMLEncode(node.NodeAliasPath)));
                            }
                        }
                    }

                    if (!allowTranslate && !RequestHelper.IsPostBack())
                    {
                        // Hide UI only when security check is performed within first load, if postback used user may loose some setting
                        HideUI();
                    }
                }

                // Display check box for separate submissions for each document if there is more than one document
                translationElem.DisplaySeparateSubmissionOption = (nodeIds.Count > 1);
            }
            else
            {
                // Hide everything
                pnlContent.Visible = false;
            }
        }

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this);

        ctlAsyncLog.TitleText = GetString("contentrequest.starttranslate");
        // Set visibility of panels
        pnlContent.Visible = true;
        pnlLog.Visible = false;
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!translationElem.SourceCultureAvailable)
        {
            pnlSettings.Visible = false;
            btnOk.Visible = false;
            plcMessages.AddError(GetString("translations.nosourceculture"));
        }

        if (translationElem.DisplayedServicesCount == 1)
        {
            headTranslate.ResourceString = null;
            headTranslate.Text = HTMLHelper.HTMLEncode(translationElem.DisplayedServiceName);
        }

        // Overwrite cancelNodeId variable if sub-levels are visible
        if (AllLevels && Parameters.ContainsKey("refreshnodeid"))
        {
            cancelNodeId = ValidationHelper.GetInteger(Parameters["refreshnodeid"], 0);
        }

        string refreshCurrent;

        if (IsDialog)
        {
            btnNo.Visible = false;
            refreshCurrent = "function RefreshCurrent(){ CloseDialog(); RefreshTree(" + cancelNodeId + "," + cancelNodeId + "); }";
        }
        else
        {
            btnNo.OnClientClick = "SelectNode(" + cancelNodeId + "); return false";
            refreshCurrent = "function RefreshCurrent(){ RefreshTree(" + cancelNodeId + "," + cancelNodeId + "); }";
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "refreshCurrent", ScriptHelper.GetScript(refreshCurrent));

        base.OnPreRender(e);
    }

    #endregion


    #region "Button actions"

    protected void btnTranslate_Click(object sender, EventArgs e)
    {
        // Check if data are correct
        var error = translationElem.ValidateData();
        if (!String.IsNullOrEmpty(error) || !allowTranslate)
        {
            plcMessages.AddError(error);
            return;
        }

        if (isSelect)
        {
            // If in select mode, prepare node IDs now
            TreeProvider tree = new TreeProvider();
            DataSet ds = tree.SelectNodes(CurrentSiteName, pathElem.Value.ToString(), TreeProvider.ALL_CULTURES, true, TreeProvider.ALL_CLASSNAMES, null,
                null, TreeProvider.ALL_LEVELS, false, 0, DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS + ",DocumentName,NodeParentID,NodeSiteID,NodeAliasPath");

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        TreeNode node = TreeNode.New(ValidationHelper.GetString(dr["ClassName"], string.Empty), dr);

                        if (TranslationServiceHelper.IsAuthorizedToTranslateDocument(node, currentUser, translationElem.TargetLanguages))
                        {
                            nodeIds.Add(ValidationHelper.GetInteger(dr["NodeID"], 0));
                        }
                        else
                        {
                            HideUI();
                            plcMessages.AddError(String.Format(GetString("cmsdesk.notauthorizedtotranslatedocument"), HTMLHelper.HTMLEncode(node.NodeAliasPath)));
                            return;
                        }
                    }
                }
            }
            else
            {
                ShowError(GetString("translationservice.nodocumentstotranslate"));
                return;
            }

            targetCultures = translationElem.TargetLanguages;
        }

        pnlLog.Visible = true;
        pnlContent.Visible = false;

        CurrentError = string.Empty;

        var parameters = new AsyncParameters
        {
            IsDialog = IsDialog,
            AllLevels = AllLevels,
            UICulture = CultureHelper.PreferredUICultureCode
        };

        ctlAsyncLog.RunAsync(p => Translate(parameters), WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// Translates document(s).
    /// </summary>
    private void Translate(object parameter)
    {
        var parameters = parameter as AsyncParameters;
        if ((parameters == null) || nodeIds.Count < 1)
        {
            return;
        }

        AbstractMachineTranslationService machineService = null;
        AbstractHumanTranslationService humanService = null;
        TranslationSubmissionInfo submission = null;
        string submissionFileName = "";
        int charCount = 0;
        int wordCount = 0;
        int refreshId = 0;
        int itemCount = 0;
        int pageCount = 0;
        bool oneSubmission = translationElem.CreateSeparateSubmission;
        bool success = false;
        bool separateSubmissionCreated = false;

        TreeProvider tree = new TreeProvider();
        tree.AllowAsyncActions = false;

        try
        {
            // Begin log
            AddLog(GetString("contentrequest.starttranslate", parameters.UICulture));

            // Prepare translation settings
            var settings = PrepareTranslationSettings();

            // Check selected service
            var service = TranslationServiceInfo.Provider.Get(translationElem.SelectedService);
            if (service == null)
            {
                return;
            }

            if (service.TranslationServiceIsMachine)
            {
                machineService = AbstractMachineTranslationService.GetTranslationService(service, CurrentSiteName);
            }
            else
            {
                humanService = AbstractHumanTranslationService.GetTranslationService(service, CurrentSiteName);
            }


            bool langSupported = (humanService == null) || CheckLanguageSupport(humanService, settings);
            if (!langSupported)
            {
                return;
            }

            if ((machineService != null) || (humanService != null))
            {
                var data = tree.SelectNodes()
                               .CombineWithDefaultCulture(false)
                               .Published(false)
                               .Culture(settings.SourceLanguage)
                               .WhereIn("NodeID", nodeIds)
                               .OnSite(CurrentSiteName)
                               .OrderBy("NodeLevel, NodeAliasPath")
                               .Column("NodeID")
                               .FilterDuplicates();

                if (!DataHelper.DataSourceIsEmpty(data))
                {
                    var processedNodes = new List<int>();

                    // Translate the documents
                    foreach (DataRow dr in data.Tables[0].Rows)
                    {
                        int nodeId = ValidationHelper.GetInteger(dr["NodeID"], 0);

                        // Get document in source language
                        var node = DocumentHelper.GetDocument(nodeId, settings.SourceLanguage, false, tree);
                        if (node == null)
                        {
                            // Document doesn't exist in source culture, skip it
                            continue;
                        }

                        var targetLanguages = GetTargetLanguages(settings.TargetLanguages, node).ToList();
                        if (!targetLanguages.Any())
                        {
                            continue;
                        }

                        if ((submission == null) && (humanService != null))
                        {
                            // Create new submission if not exists for human translation service
                            submission = TranslationServiceHelper.CreateSubmissionInfo(settings, service, MembershipContext.AuthenticatedUser.UserID, SiteInfoProvider.GetSiteID(CurrentSiteName), node.GetDocumentName());
                        }

                        // Handle duplicities
                        if (processedNodes.Contains(nodeId))
                        {
                            continue;
                        }

                        processedNodes.Add(nodeId);
                        bool targetLanguageVersionCreated = false;
                        bool logged = false;
                        string encodedPath = HTMLHelper.HTMLEncode(node.NodeAliasPath);

                        foreach (var targetLanguage in targetLanguages)
                        {
                            // Log only once per document
                            if (!logged)
                            {
                                AddLog(String.Format(GetString("content.translating"), encodedPath, settings.SourceLanguage));
                                logged = true;
                            }

                            itemCount++;
                            targetLanguageVersionCreated = true;

                            if (humanService != null)
                            {
                                if (String.IsNullOrEmpty(submissionFileName))
                                {
                                    submissionFileName = node.NodeAlias;
                                }

                                var targetNode = TranslationServiceHelper.CreateTargetCultureNode(node, targetLanguage, true, false, !settings.TranslateAttachments);

                                TranslationSubmissionItemInfo submissionItem;
                                using (new CMSActionContext { TouchParent = false })
                                {
                                    // Do not touch parent because all updated information are saved after last item
                                    submissionItem = TranslationServiceHelper.CreateSubmissionItemInfo(settings, submission, node, targetNode.DocumentID, targetLanguage);
                                }

                                charCount += submissionItem.SubmissionItemCharCount;
                                wordCount += submissionItem.SubmissionItemWordCount;
                            }
                            else
                            {
                                // Prepare local settings to translate per one target language
                                var localSettings = settings.Clone();
                                localSettings.TargetLanguages.Clear();
                                localSettings.TargetLanguages.Add(targetLanguage);

                                // Translate page via machine translator
                                TranslationServiceHelper.Translate(machineService, localSettings, node);
                            }
                        }

                        // Each page has own submission if human service is used
                        if (!oneSubmission && (humanService != null))
                        {
                            if (itemCount > 0)
                            {
                                SubmitSubmissionToService(itemCount, submission, charCount, wordCount, submissionFileName, humanService, true);

                                // Reset counters
                                itemCount = 0;
                                charCount = 0;
                                wordCount = 0;

                                // Reset submission file name
                                submissionFileName = null;

                                // At least one submission was created
                                separateSubmissionCreated = true;
                            }
                            else
                            {
                                // No documents were submitted to translation delete empty submission
                                TranslationSubmissionInfo.Provider.Delete(submission);
                            }

                            // Reset submission to create new for next page
                            submission = null;
                        }

                        if (targetLanguageVersionCreated)
                        {
                            // Check if at least one target language version was created
                            pageCount++;
                        }

                        // Store parent ID to refresh UI
                        refreshId = node.NodeParentID;
                    }

                    success = true;
                }
                else
                {
                    AddError(GetString("TranslateDocument.NoSourceDocuments", parameters.UICulture));
                }
            }
            else
            {
                AddError(GetString("TranslateDocument.TranslationServiceNotFound", parameters.UICulture));
            }
        }
        catch (ThreadAbortException ex)
        {
            if (CMSThread.Stopped(ex))
            {
                // When canceled
                AddError(GetString("TranslateDocument.TranslationCanceled", parameters.UICulture));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex, parameters.UICulture);
            }
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex, parameters.UICulture);
        }
        finally
        {
            var showAllAlreadyTranslatedError = false;
            if (itemCount > 0)
            {
                // All pages are submitted via only one submission or using machine service
                if ((humanService != null) && (submission != null))
                {
                    // Set submission name if more pages are translated
                    if (pageCount > 1)
                    {
                        submission.SubmissionName += " " + String.Format(GetString("translationservices.submissionnamesuffix"), pageCount - 1);
                        // Do not localize the file name
                        submissionFileName += String.Format(" (and {0} more)", pageCount - 1);
                    }

                    SubmitSubmissionToService(itemCount, submission, charCount, wordCount, submissionFileName, humanService, success);
                }
            }
            else if (oneSubmission)
            {
                TranslationSubmissionInfo.Provider.Delete(submission);

                // Log error only if the translation was successfully processed
                if (success)
                {
                    showAllAlreadyTranslatedError = true;
                }
            }
            else if (!separateSubmissionCreated)
            {
                // Separate submissions were used and no one was created 
                showAllAlreadyTranslatedError = true;
            }

            if (showAllAlreadyTranslatedError)
            {
                AddError(GetString("TranslateDocument.DocumentsAlreadyTranslated", parameters.UICulture));
            }

            if (parameters.IsDialog)
            {
                ctlAsyncLog.Parameter = "wopener.location.replace(wopener.location); CloseDialog(); if (wopener.RefreshTree) { wopener.RefreshTree(null, null);}";
            }
            else
            {
                if (String.IsNullOrEmpty(CurrentError))
                {
                    // Overwrite refreshId variable if sub-levels are visible
                    if (parameters.AllLevels && Parameters.ContainsKey("refreshnodeid"))
                    {
                        refreshId = ValidationHelper.GetInteger(Parameters["refreshnodeid"], 0);
                    }

                    // Refresh tree
                    ctlAsyncLog.Parameter = "RefreshTree(" + refreshId + ", " + refreshId + "); \n" + "SelectNode(" + refreshId + ");";
                }
                else
                {
                    ctlAsyncLog.Parameter = "RefreshTree(null, null);";
                }
            }
        }
    }


    private IEnumerable<string> GetTargetLanguages(IEnumerable<string> targetLanguages, TreeNode node)
    {
        return chkSkipTranslated.Checked ? targetLanguages.Except(node.GetTranslatedCultures()) : targetLanguages;
    }


    private bool CheckLanguageSupport(AbstractHumanTranslationService humanService, TranslationSettings settings)
    {
        var sourceLanguage = settings.SourceLanguage;
        if (!humanService.CheckSourceLanguageAvailability(sourceLanguage))
        {
            AddError(String.Format(GetString("translationservice.sourcelanguagenotsupported"), sourceLanguage));
            return false;
        }

        var unavailableLanguages = humanService.CheckTargetLanguagesAvailability(settings.TargetLanguages);
        if (unavailableLanguages.Count > 0)
        {
            AddError(String.Format(GetString("translationservice.targetlanguagenotsupported"), String.Join(", ", unavailableLanguages)));
            return false;
        }

        return true;
    }


    /// <summary>
    /// Prepares translation settings.
    /// </summary>
    private TranslationSettings PrepareTranslationSettings()
    {
        var settings = new TranslationSettings
        {
            SourceLanguage = translationElem.FromLanguage,
            Instructions = translationElem.Instructions,
            Priority = translationElem.Priority,
            TranslateAttachments = translationElem.ProcessBinary,
            TranslationDeadline = translationElem.Deadline,
            TranslationServiceName = translationElem.SelectedService
        };
        settings.TargetLanguages.AddRangeToSet(targetCultures);

        return settings;
    }


    /// <summary>
    /// Submits submission to translation service.
    /// </summary>
    /// <param name="itemCount">Number of items</param>
    /// <param name="submission">Submission info object</param>
    /// <param name="charCount">Number of characters</param>
    /// <param name="wordCount">Number of words</param>
    /// <param name="submissionFileName">Submission file name</param>
    /// <param name="humanService">Translation service info</param>
    /// <param name="contactService">Indicates if service should be contacted</param>
    private void SubmitSubmissionToService(int itemCount, TranslationSubmissionInfo submission, int charCount, int wordCount, string submissionFileName, AbstractHumanTranslationService humanService, bool contactService)
    {
        submission.SubmissionCharCount = charCount;
        submission.SubmissionWordCount = wordCount;
        submission.SubmissionItemCount = itemCount;
        submission.SubmissionParameter = submissionFileName;

        AddLog(GetString("content.submitingtranslation"));

        if (contactService)
        {
            string error = humanService.CreateSubmission(submission);
            if (!String.IsNullOrEmpty(error))
            {
                AddError(GetString("ContentRequest.TranslationFailed") + " \"" + error + "\"");
            }
        }

        // Update submission after submitting to service to save ticket
        submission.Update();
    }

    #endregion


    #region "Help methods"

    private void HideUI()
    {
        pnlButtons.Visible = false;
        pnlSettings.Visible = false;
        pnlList.Visible = false;
        plcInfo.Visible = false;
    }


    private void LogExceptionToEventLog(Exception ex, string uiCulture)
    {
        var logData = new EventLogData(EventTypeEnum.Error, "Content", "TRANSLATEDOC")
        {
            EventDescription = EventLogProvider.GetExceptionLogMessage(ex),
            EventUrl = RequestContext.RawURL,
            UserID = currentUser.UserID,
            UserName = currentUser.UserName,
            IPAddress = RequestContext.UserHostAddress,
            SiteID = SiteInfoProvider.GetSiteID(CurrentSiteName)
        };
        
        Service.Resolve<IEventLogService>().LogEvent(logData);

        AddError(GetString("ContentRequest.TranslationFailed", uiCulture) + ex.Message);
    }


    public override void AddScript(string script)
    {
        ltlScript.Text += ScriptHelper.GetScript(script);
    }

    #endregion


    #region "Handling async thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        ctlAsyncLog.Parameter = null;
        string canceled = GetString("TranslateDocument.TranslationCanceled");
        AddLog(canceled);
        ShowConfirmation(canceled);
        ltlScript.Text += ScriptHelper.GetScript("var __pendingCallbacks = new Array();RefreshCurrent();");
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
    }


    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        if (ctlAsyncLog.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsyncLog.Stop();
        }
        ctlAsyncLog.Parameter = null;
        ShowError(CurrentError);
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        ShowError(CurrentError);

        if (!String.IsNullOrEmpty(CurrentError))
        {
            ctlAsyncLog.Parameter = null;
            ShowError(CurrentError);
        }

        if (ctlAsyncLog.Parameter != null)
        {
            AddScript(ctlAsyncLog.Parameter);
        }
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    /// <summary>
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = (error + "<br />" + CurrentError);
    }

    #endregion


    private class AsyncParameters
    {
        public bool IsDialog
        {
            get;
            set;
        }


        public bool AllLevels
        {
            get;
            set;
        }


        public string UICulture
        {
            get;
            set;
        }
    }
}
