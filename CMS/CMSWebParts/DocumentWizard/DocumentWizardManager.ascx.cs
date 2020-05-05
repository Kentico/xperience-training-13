using System;
using System.Collections.Generic;
using System.Data;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_DocumentWizard_DocumentWizardManager : CMSAbstractWebPart, IDocumentWizardManager
{
    #region "Variables"

    private List<DocumentWizardStep> steps;
    private StepEventArgs mStepEventArgs;
    private int? mLastConfirmedStepIndex;
    private int? mLastVisitedStepIndex;
    public CMSDataProperties dataProperties = new CMSDataProperties();

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether next/previous step is allowed by view mode
    /// </summary>
    private bool AllowedByViewMode
    {
        get
        {
            return (PortalContext.ViewMode == ViewModeEnum.Preview) || (PortalContext.ViewMode == ViewModeEnum.LiveSite);
        }
    }


    /// <summary>
    /// Determines which document types to load. The document types are specified by a list of code names separated by semicolons (;). The * wildcard can be used as a substitute for a random sequence of characters. For example, Product.* would include document types such as Product.Camera, Product.CellPhone, Product.Computer etc. If left empty, the web part retrieves all document types by default. Please note that in this case (empty value), only the common data columns from document view are available. When writing transformations, WHERE conditions etc., bear in mind that the specific fields of individual document types are not included..
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ClassNames"), dataProperties.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            dataProperties.ClassNames = value;
        }
    }


    /// <summary>
    /// Sets the Order By condition
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("OrderBy"), "NodeLevel, NodeOrder, NodeName");
        }
        set
        {
            SetValue("OrderBy", value);
            dataProperties.OrderBy = value;
        }
    }



    /// <summary>
    /// Sets the WHERE condition that limits which documents should have their attachments searched..
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), dataProperties.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            dataProperties.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), dataProperties.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            dataProperties.SelectTopN = value;
        }
    }


    /// <summary>
    /// Specifies the path of the documents to be selected. If left empty, the path is set to all child documents under a page on which the web part is placed. Use '.' for the current document selection..
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), PagePlaceholder.PageInfo.NodeAliasPath + "/%");
        }
        set
        {
            SetValue("Path", value);
            dataProperties.Path = Path;
        }
    }


    /// <summary>
    /// Specifies whether the default language version of a document is to be used as a replacement if a given document is not translated into the currently selected language. If the 'Use site settings' option is selected, the value is taken from 'Site Manager -> Settings -> Content -> Combine with default culture'..
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), dataProperties.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            dataProperties.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Indicates which culture version of the specified documents should be used..
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CultureCode"), dataProperties.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            dataProperties.CultureCode = value;
        }
    }


    /// <summary>
    /// Specifies whether only published documents are to be loaded..
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), dataProperties.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            dataProperties.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Lists the database columns, separated by commas (,), to be loaded along with the given objects. If left empty, all columns are loaded. Specifying a list without unnecessary columns can significantly improve performance..
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), dataProperties.SelectedColumns);
        }
        set
        {
            SetValue("Columns", value);
            dataProperties.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Indicates if the retrieved data should be filtered to contain documents only once, even if they are linked multiple times..
    /// </summary>
    public bool FilterOutDuplicates
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterOutDuplicates"), dataProperties.FilterOutDuplicates);
        }
        set
        {
            SetValue("FilterOutDuplicates", value);
            dataProperties.FilterOutDuplicates = value;
        }
    }


    /// <summary>
    /// Specifies whether the current user's permissions are to be checked for the content of the web part. If enabled, only documents for which the user has the 'Read' permission are loaded..
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), dataProperties.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            dataProperties.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Sets the name of the cache key used for the content of the web part. If not specified, this name is generated automatically based on the site, document path, Web part control ID and current user. A cache key can be shared between multiple web parts with the same content on different pages in order to avoid keeping redundant data in the memory..
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            dataProperties.CacheItemName = value;
        }
    }


    /// <summary>
    /// Sets the number of minutes for which the content of the web part should remain cached before its latest version is reloaded from the database. If left empty, the value entered into the Site Manager -> Settings -> System -> Performance -> Cache content (minutes) setting will be used instead. If set to 0, caching will be disabled for the web part..
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            dataProperties.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Contains a list of cache keys on which the partial cache of the web part depends. When the specified cache items change, the partial cache of the web part is deleted. Each line may contain a single item only. If the 'Use default cache dependencies' box is checked, the default dependencies will be used, which include all possible object changes that could affect the specific web part..
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, dataProperties.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            dataProperties.CacheDependencies = value;
        }
    }

    #endregion


    #region "IPageWizardManager properties"

    /// <summary>
    /// Resets the wizard step indexes(LastConfirmedStep, LastVisitedStep)
    /// </summary>
    public void ResetWizard()
    {
        LastVisitedStepIndex = -1;
        LastConfirmedStepIndex = -1;
    }


    /// <summary>
    /// Restrict step order.
    /// </summary>
    public bool RestrictStepOrder
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RestrictStepOrder"), true);
        }
        set
        {
            SetValue("RestrictStepOrder", value);
        }
    }


    /// <summary>
    /// Final step URL.
    /// </summary>
    public string FinalStepNextUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FinalStepNextUrl"), "");
        }
        set
        {
            SetValue("FinalStepNextUrl", value);
        }
    }


    /// <summary>
    /// Gets the key name for session storage
    /// </summary>
    private string KeyName
    {
        get
        {
            return "PageWizardManager_" + PagePlaceholder.PageInfo.NodeAliasPath;
        }
    }


    /// <summary>
    /// Gets the last visited key name for session storage
    /// </summary>
    private string LastVisitedKeyName
    {
        get
        {
            return KeyName + "_LastVisited";
        }
    }


    /// <summary>
    /// Gets or sets the last visited step index
    /// </summary>
    public int LastVisitedStepIndex
    {
        get
        {
            if (mLastVisitedStepIndex == null)
            {
                mLastVisitedStepIndex = ValidationHelper.GetInteger(SessionHelper.GetValue(LastVisitedKeyName), -1);
            }
            return mLastVisitedStepIndex.Value;
        }
        set
        {
            if (value < 0)
            {
                mLastVisitedStepIndex = null;
                SessionHelper.Remove(LastVisitedKeyName);
            }
            else
            {
                mLastVisitedStepIndex = value;
                SessionHelper.SetValue(LastVisitedKeyName, value);
            }
        }
    }


    /// <summary>
    /// Gets or sets the value of the last confirmed step index
    /// </summary>
    public int LastConfirmedStepIndex
    {
        get
        {
            if (mLastConfirmedStepIndex == null)
            {
                mLastConfirmedStepIndex = ValidationHelper.GetInteger(SessionHelper.GetValue(KeyName), -1);
            }
            return mLastConfirmedStepIndex.Value;
        }
        set
        {
            if (value < 0)
            {
                mLastConfirmedStepIndex = null;
                SessionHelper.Remove(KeyName);
                LastVisitedStepIndex = -1;
            }
            else
            {
                mLastConfirmedStepIndex = value;
                SessionHelper.SetValue(KeyName, value);
            }
        }
    }


    /// <summary>
    /// Gets the current StepEventArgs
    /// </summary>
    public StepEventArgs StepEventArgs
    {
        get
        {
            if (mStepEventArgs == null)
            {
                mStepEventArgs = new StepEventArgs(Steps.Count, CurrentStep.StepIndex);
            }
            return mStepEventArgs;
        }
    }


    /// <summary>
    /// Gets the list of steps
    /// </summary>
    public List<DocumentWizardStep> Steps
    {
        get
        {
            if (steps == null)
            {
                steps = new List<DocumentWizardStep>();
            }
            return steps;
        }
    }


    /// <summary>
    /// First step
    /// </summary>
    public DocumentWizardStep FirstStep
    {
        get;
        private set;
    }


    /// <summary>
    /// Current step
    /// </summary>
    public DocumentWizardStep CurrentStep
    {
        get;
        private set;
    }


    /// <summary>
    /// Last step
    /// </summary> 
    public DocumentWizardStep LastStep
    {
        get;
        private set;
    }

    #endregion


    #region "Page methods"

    protected override void OnLoad(EventArgs e)
    {
        if (!StopProcessing)
        {
            // Set current final step URL
            StepEventArgs.FinalStepUrl = FinalStepNextUrl;

            // Register for next/previous
            if (AllowedByViewMode)
            {
                ComponentEvents.RequestEvents.RegisterForComponentEvent<EventArgs>("PageWizardManager", ComponentEvents.PREVIOUS, null, ProcessPreviousStep);
                ComponentEvents.RequestEvents.RegisterForComponentEvent<EventArgs>("PageWizardManager", ComponentEvents.NEXT, null, ProcessNextStep);
            }

            // Raise Load_Step and Step_Loaded
            ComponentEvents.RequestEvents.RaiseComponentEvent(this, StepEventArgs, "PageWizardManager", ComponentEvents.LOAD_STEP);
            ComponentEvents.RequestEvents.RaiseComponentEvent(this, StepEventArgs, "PageWizardManager", ComponentEvents.STEP_LOADED);

            // Check skip status
            if (AllowedByViewMode && (StepEventArgs.Skip))
            {
                // Ensure skip for first/last step
                bool forceNext = (CurrentStep == FirstStep);
                bool forcePrevious = (CurrentStep == LastStep);

                // With dependence on wizard flow set skip action
                if (!forceNext && (forcePrevious || (LastVisitedStepIndex > CurrentStep.StepIndex)))
                {   // Previous
                    ProcessPreviousStep(this, StepEventArgs);
                }
                else
                {
                    bool raiseEvents = ValidationHelper.GetBoolean(StepEventArgs["RaiseEvents"], false);
                    // Next
                    ProcessNextStep(StepEventArgs, raiseEvents);
                }
            }

            // Keep the last visited step index
            LastVisitedStepIndex = CurrentStep.StepIndex;
        }

        base.OnLoad(e);
    }


    protected override void OnInit(EventArgs e)
    {
        // Register manager
        DocumentWizardManager = this;

        base.OnInit(e);

        // Load steps
        if (!StopProcessing)
        {
            StopProcessing = !LoadSteps();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Previous step
    /// </summary>
    private void ProcessPreviousStep(object sender, EventArgs e)
    {
        if (CurrentStep.StepIndex > 0)
        {
            string url = Steps[CurrentStep.StepIndex - 1].StepUrl;
            URLHelper.Redirect(UrlResolver.ResolveUrl(url));
        }
    }


    /// <summary>
    /// Next step
    /// </summary>
    private void ProcessNextStep(object sender, EventArgs e)
    {
        StepEventArgs sea = e as StepEventArgs;
        if (sea == null)
        {
            sea = StepEventArgs;
        }

        ProcessNextStep(sea, true);
    }


    /// <summary>
    /// Next step
    /// </summary>
    private void ProcessNextStep(StepEventArgs e, bool raiseEvents)
    {
        if (raiseEvents)
        {
            ComponentEvents.RequestEvents.RaiseEvent(this, e, ComponentEvents.VALIDATE_STEP);

            // Cancel if not validated
            if (e.CancelEvent)
            {
                ComponentEvents.RequestEvents.RaiseEvent(this, e, ComponentEvents.CANCEL_STEP);
                return;
            }

            // Fire the event to finish current step
            ComponentEvents.RequestEvents.RaiseEvent(this, e, ComponentEvents.FINISH_STEP);

            // Cancel if not validated
            if (e.CancelEvent)
            {
                ComponentEvents.RequestEvents.RaiseEvent(this, e, ComponentEvents.CANCEL_STEP);
                return;
            }

            // Fire the event to finished state
            ComponentEvents.RequestEvents.RaiseEvent(this, e, ComponentEvents.STEP_FINISHED);

            // Cancel if not validated
            if (e.CancelEvent)
            {
                ComponentEvents.RequestEvents.RaiseEvent(this, e, ComponentEvents.CANCEL_STEP);
                return;
            }
        }

        if (RestrictStepOrder)
        {
            // Keep last confirmed step index
            LastConfirmedStepIndex = CurrentStep.StepIndex;
        }

        // Use step custom URL if set
        string url = e["NextStepUrl"] as string;

        // Use default step URL
        if (String.IsNullOrEmpty(url))
        {
            // Next step
            if (!e.IsLastStep)
            {
                url = Steps[CurrentStep.StepIndex + 1].StepUrl;
            }
            // URL after final step
            else
            {
                url = String.IsNullOrEmpty(e.FinalStepUrl) ? FinalStepNextUrl : e.FinalStepUrl;

                // Reset last confirmed step
                LastConfirmedStepIndex = -1;
            }
        }

        // Redirect to the URL
        if (!String.IsNullOrEmpty(url))
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl(url));
        }
    }


    /// <summary>
    /// Load collection of steps
    /// </summary>
    /// <returns>Returns false if steps weren't loaded</returns>
    private bool LoadSteps()
    {
        // Set default values to the data control
        dataProperties.Path = Path;
        dataProperties.ClassNames = ClassNames;

        dataProperties.OrderBy = OrderBy;
        dataProperties.WhereCondition = WhereCondition;

        dataProperties.CultureCode = CultureCode;
        dataProperties.CombineWithDefaultCulture = CombineWithDefaultCulture;

        dataProperties.SelectedColumns = Columns;
        dataProperties.MaxRelativeLevel = 1;
        dataProperties.SelectTopN = SelectTopN;
        dataProperties.FilterOutDuplicates = FilterOutDuplicates;
        dataProperties.SelectOnlyPublished = SelectOnlyPublished;

        dataProperties.CheckPermissions = CheckPermissions;

        dataProperties.CacheMinutes = CacheMinutes;
        dataProperties.CacheItemName = CacheItemName;
        dataProperties.CacheDependencies = CacheDependencies;

        // Resolve path
        string path = ContextResolver.ResolvePath(Path, true);
        string currentPath = DocumentContext.CurrentAliasPath;

        // Do not process if is out of context
        if (!currentPath.StartsWith(path.TrimEnd('%'), StringComparison.InvariantCultureIgnoreCase))
        {
            StopProcessing = true;
            return false;
        }

        object data = null;

        // Load data
        dataProperties.LoadData(ref data, false);

        // Create collection of steps
        if (!DataHelper.DataSourceIsEmpty(data))
        {
            int index = 0;
            DataView ds = data as DataView;

            foreach (DataRowView dr in ds)
            {
                DocumentWizardStep step = new DocumentWizardStep();
                step.StepIndex = index;
                step.StepData = dr;

                if (FirstStep == null)
                {
                    FirstStep = step;
                }

                if ((CurrentStep == null) && string.Equals(currentPath, Convert.ToString(dr["NodeAliasPath"]), StringComparison.InvariantCultureIgnoreCase))
                {
                    CurrentStep = step;
                }

                Steps.Add(step);
                LastStep = step;

                index++;
            }
        }

        // Do not process if is out of context
        if (CurrentStep == null)
        {
            StopProcessing = true;
            return false;
        }

        // validate current step index
        if (PortalContext.ViewMode == ViewModeEnum.LiveSite)
        {
            if (RestrictStepOrder)
            {
                // For not existing confirmed step current step must be first step
                if ((LastConfirmedStepIndex == -1) && (CurrentStep.StepIndex > 0))
                {
                    string url = FirstStep.StepUrl;
                    URLHelper.Redirect(UrlResolver.ResolveUrl(url));
                }
                // Current step index cannot be higher than last confirmed step index + 1
                else if ((CurrentStep.StepIndex - 1) > LastConfirmedStepIndex)
                {
                    string url = Steps[LastConfirmedStepIndex].StepUrl;
                    URLHelper.Redirect(UrlResolver.ResolveUrl(url));
                }
            }
        }

        return true;
    }

    #endregion

}
