using System;
using System.Linq;
using System.Threading;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Search;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_General : CMSAdminEditControl, IPostBackEventHandler
{
    // Code name of the editation alternative form for Azure Search index
    private const string AZURE_INDEX_EDIT_FORM = "EditAzureSearchIndexForm";
    private const string REBUILD = "rebuild";
    private const string OPTIMIZE = "optimize";
    private const string REBUILD_REQUIRED = "rebuildrequired";

    private const string INDEX_NAME = "indexname";
    private const string INDEX_ANALYZER_TYPE = "indexanalyzertype";
    private const string INDEX_STOP_WORDS_FILE = "indexstopwordsfile";
    private const string INDEX_CUSTOM_ANALYZER_ASSEMBLY = "indexcustomanalyzerassemblyname";
    private const string INDEX_CUSTOM_ANALYZER_CLASS = "indexcustomanalyzerclassname";

    private SearchIndexInfo mSearchIndex = null;


    /// <summary>
    /// Occurs after the async search index task is started, e.g. index rebuild, index optimization.
    /// </summary>
    public event EventHandler<EventArgs> AsyncIndexTaskStarted;


    /// <summary>
    /// Gets the edited search index.
    /// </summary>
    public SearchIndexInfo SearchIndex
    {
        get
        {
            return mSearchIndex ?? (mSearchIndex = UIContext.EditedObject as SearchIndexInfo);
        }
    }


    #region "Init"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        InitFormEvents();

        if (IsSearchIndexAzureBased())
        {
            form.AlternativeFormName = AZURE_INDEX_EDIT_FORM;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Check the index path length
        var indexPath = Path.Combine(SystemContext.WebApplicationPhysicalPath, SearchHelper.SearchPath);
        if (indexPath.Length > SearchHelper.MAX_INDEX_PATH)
        {
            ShowWarning(GetString("srch.pathtoolong"), null, null);
            StopProcessing = true;
            return;
        }

        ucDisabledModule.InfoText = GetString("srch.searchdisabledinfo");

        if (form.Mode == FormModeEnum.Update)
        {
            InitAsyncIndexTaskHeaderActions();
        }
    }


    /// <summary>
    /// Initializes the header actions for async index tasks.
    /// </summary>
    private void InitAsyncIndexTaskHeaderActions()
    {
        // Add rebuild action
        AddHeaderAction(new HeaderAction()
        {
            Text = GetString("srch.index.rebuild"),
            OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("srch.index.confirmrebuild")) + ");",
            CommandName = REBUILD
        });

        ComponentEvents.RequestEvents.RegisterForEvent(REBUILD, (sender, args) => { Rebuild(); });

        // Add optimize action if the search index is not Azure based
        if (!IsSearchIndexAzureBased())
        {
            AddHeaderAction(new HeaderAction()
            {
                Text = GetString("srch.index.optimize"),
                OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("srch.index.confirmoptimize")) + ");",
                CommandName = OPTIMIZE
            });

            ComponentEvents.RequestEvents.RegisterForEvent(OPTIMIZE, (sender, args) => { Optimize(); });
        }
    }

    #endregion


    #region "Form"

    /// <summary>
    /// Initializes the edit form events.
    /// </summary>
    private void InitFormEvents()
    {
        form.OnItemValidation += (object control, ref string errorMessage) =>
        {
            var formControl = control as FormEngineUserControl;
            if (formControl != null)
            {
                errorMessage = ValidateField(formControl);
            }
        };

        var analyzerTypeChanged = false;

        form.OnBeforeSave += (sender, args) =>
        {
            // Check if the analyzer type changed
            analyzerTypeChanged = SearchIndex.ChangedColumns()
                .Select(c => c.ToLowerInvariant())
                .Intersect(new[] { INDEX_ANALYZER_TYPE, INDEX_STOP_WORDS_FILE, INDEX_CUSTOM_ANALYZER_ASSEMBLY, INDEX_CUSTOM_ANALYZER_CLASS })
                .Any();

            // Set other values
            SearchIndex.IndexIsCommunityGroup = false;
        };

        form.OnAfterSave += (sender, args) =>
        {
            RaiseOnSaved();

            if (analyzerTypeChanged)
            {
                // Notify that the index requires a rebuild
                var anchor = string.Format(@"<a href=""javascript:{0}"">{1}</a>", Page.ClientScript.GetPostBackEventReference(this, REBUILD_REQUIRED), GetString("general.clickhere"));
                var message = string.Format(GetString("srch.indexrequiresrebuild"), anchor);

                ShowInformation(message);
            }
        };
    }


    /// <summary>
    /// Performs extra validation for the value of the specified form control.
    /// </summary>
    /// <param name="formControl">Form control</param>
    /// <returns>Returns an error message if the field value is invalid, otherwise returns null.</returns>
    private string ValidateField(FormEngineUserControl formControl)
    {
        switch (formControl.FieldInfo.Name.ToLowerCSafe())
        {
            case INDEX_NAME:
                {
                    var value = ValidationHelper.GetString(formControl.Value, null);

                    // Possible length of path - already taken, +1 because in MAX_INDEX_PATH is count code name of length 1
                    var indexPath = Path.Combine(SystemContext.WebApplicationPhysicalPath, SearchHelper.SearchPath);
                    var maxLength = SearchHelper.MAX_INDEX_PATH - indexPath.Length + 1;

                    if (value.Length > maxLength)
                    {
                        return GetString("srch.codenameexceeded");
                    }

                    break;
                }
        }

        return null;
    }

    #endregion


    #region "Async index tasks"

    /// <summary>
    /// Raises the AsyncIndexTaskStarted event.
    /// </summary>
    private void RaiseAsyncIndexTaskStarted()
    {
        if (AsyncIndexTaskStarted != null)
        {
            Thread.Sleep(100);
            AsyncIndexTaskStarted(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Starts the search index rebuild task.
    /// </summary>
    private void Rebuild()
    {
        // Extra check for document index
        if ((SearchIndex.IndexType.Equals(TreeNode.OBJECT_TYPE, StringComparison.OrdinalIgnoreCase) || (SearchIndex.IndexType == SearchHelper.DOCUMENTS_CRAWLER_INDEX)))
        {
            // Check if there is at least one site assigned
            if (!SearchIndexSiteInfoProvider.SearchIndexHasAnySite(SearchIndex.IndexID))
            {
                ShowError(GetString("index.nosite"));
                return;
            }

            // Check if there is any culture assigned
            if (!SearchIndexCultureInfoProvider.SearchIndexHasAnyCulture(SearchIndex.IndexID))
            {
                ShowError(GetString("index.noculture"));
                return;
            }
        }

        if (!SearchHelper.CreateRebuildTask(SearchIndex.IndexID))
        {
            ShowError(GetString("index.nocontent"));
            return;
        }

        RaiseAsyncIndexTaskStarted();
        ShowInformation(GetString("srch.index.rebuildstarted"));
    }


    /// <summary>
    /// Starts the search index optimization task.
    /// </summary>
    private void Optimize()
    {
        SearchTaskInfoProvider.CreateTask(SearchTaskTypeEnum.Optimize, null, null, SearchIndex.IndexName, SearchIndex.IndexID);

        RaiseAsyncIndexTaskStarted();
        ShowInformation(GetString("srch.index.optimizestarted"));
    }

    #endregion


    #region "IPostBackEventHandler members"

    /// <summary>
    /// Handles the post back event.
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == REBUILD_REQUIRED)
        {
            Rebuild();
        }
    }

    #endregion


    /// <summary>
    /// Returns true if the search index is Azure based.
    /// </summary>
    private bool IsSearchIndexAzureBased()
    {
        return SearchIndex.IndexProvider.Equals(SearchIndexInfo.AZURE_SEARCH_PROVIDER, StringComparison.OrdinalIgnoreCase);
    }
}