using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_QueryEdit : CMSUserControl
{
    #region "Variables"

    private bool mShowHelp = true;
    private string mQueryName;
    private CMSMasterPage mCurrentMaster;
    private QueryInfo mQuery;
    private bool? mQueriesCanBeEdited;
    private ResourceInfo mModule;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of the current query.
    /// </summary>
    public int QueryID
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the class current query belongs to.
    /// </summary>
    public int ClassID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether the help should be displayed in the bottom of the page.
    /// </summary>
    public bool ShowHelp
    {
        get
        {
            return mShowHelp;
        }
        set
        {
            mShowHelp = value;
        }
    }


    /// <summary>
    /// Page that should be refreshed after button click.
    /// </summary>
    public string RefreshPageURL
    {
        get;
        set;
    }


    /// <summary>
    /// If true, control is used in site manager.
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return filter.IsSiteManager;
        }
        set
        {
            filter.IsSiteManager = value;
        }
    }


    /// <summary>
    /// Gets or sets whether the control is in dialog mode.
    /// </summary>
    public bool DialogMode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets whether the control is in edit mode.
    /// </summary>
    public bool EditMode
    {
        get;
        set;
    }


    /// <summary>
    /// Query to edit.
    /// </summary>
    private QueryInfo Query
    {
        get
        {
            return mQuery;
        }
        set
        {
            mQuery = value;

            // Assigning null to EditedObject throws exception
            if (value != null)
            {
                UIContext.EditedObject = value;
            }
        }
    }


    /// <summary>
    /// Name of the class query belongs to.
    /// </summary>
    private string ClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Name of the actual query.
    /// </summary>
    private string QueryName
    {
        get
        {
            return mQueryName;
        }
        set
        {
            mQueryName = value;
        }
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return pnlContainer.Enabled;
        }
        set
        {
            pnlContainer.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets module identifier.
    /// </summary>
    public int ModuleID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the module to which the query belongs.
    /// </summary>
    private ResourceInfo Module
    {
        get
        {
            return mModule ?? (mModule = ResourceInfo.Provider.Get(ModuleID));
        }
    }


    /// <summary>
    /// Gets whether queries in the current UIContext can be deleted, cloned, created or modified.
    /// </summary>
    private bool QueriesCanBeEdited
    {
        get
        {
            if (!mQueriesCanBeEdited.HasValue)
            {
                DataClassInfo classInfo = GetClass();
                bool result = (classInfo == null) || classInfo.ClassIsDocumentType || classInfo.ClassIsCustomTable || classInfo.ClassShowAsSystemTable;
                if (!result)
                {
                    ModuleID = classInfo.ClassResourceID;
                    result = ((Module != null) && Module.IsEditable);
                }
                mQueriesCanBeEdited = result;
            }
            return mQueriesCanBeEdited.Value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Register save
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, (s, args) => Save(false));

        mCurrentMaster = Page.Master as CMSMasterPage;

        if (mCurrentMaster == null)
        {
            throw new Exception("Page using this control must have CMSMasterPage master page.");
        }

        txtQueryText.FullScreenParentElementID = "divContent";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the control
        SetupControl();

        if (!RequestHelper.IsPostBack())
        {
            // Shows that the query was created or updated successfully
            if (QueryHelper.GetInteger("saved", 0) == 1)
            {
                ShowChangesSaved();
            }
        }
    }


    /// <summary>
    /// Generate default query.
    /// </summary>
    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        rblQueryType.SelectedIndex = 0;
        try
        {
            txtQueryText.Text = SqlGenerator.GetSqlQuery(ClassName, SqlGenerator.GetQueryType(QueryName), null);
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }


    public bool Save(bool closeOnSave)
    {
        // Validate query name for emptiness and code name format
        string result = new Validator()
            .NotEmpty(txtQueryName.Text, GetString("queryedit.erroremptyqueryname"))
            .IsIdentifier(txtQueryName.Text, GetString("queryedit.querynameformat"))
            .Result;

        // If the entries were successfully validated
        if (result != string.Empty)
        {
            ShowError(result);
            return false;
        }

        bool isSaved;

        if (filter.Visible)
        {
            ClassID = filter.ClassId;
            ClassName = DataClassInfoProvider.GetClassName(filter.ClassId);
        }

        // Gets full query name in the form - "ClassName.QueryName"
        string fullQueryName = !string.IsNullOrEmpty(ClassName) ? string.Concat(ClassName, ".", txtQueryName.Text) : txtQueryName.Text;

        // Finds whether new or edited query is unique in the document type
        var newQuery = QueryInfoProvider.GetQueryInfoFromDB(fullQueryName);
        if (newQuery != null)
        {
            // Check if it is the same query as one being edited
            if (newQuery.QueryID != QueryID)
            {
                // The new query tries to use the same code name
                ShowError(GetString("queryedit.errorexistingqueryid"));
                return false;
            }

            Query = newQuery;
            isSaved = SaveQuery();
        }
        else
        {
            // Query with specified name doesn't exist yet            
            isSaved = SaveQuery();
        }

        // If the query was successfully saved
        RedirectOnSave(isSaved, closeOnSave);

        if (isSaved)
        {
            ShowChangesSaved();
        }

        return true;
    }


    /// <summary>
    /// Gets the class to which belongs currently edited query or the query that's being created.
    /// </summary>
    private DataClassInfo GetClass()
    {
        DataClassInfo classInfo = null;

        if ((Query == null) && (QueryID > 0))
        {
            Query = QueryInfo.Provider.Get(QueryID);
        }
        if (Query != null)
        {
            if (Query.ClassID > 0)
            {
                classInfo = DataClassInfoProvider.GetDataClassInfo(Query.ClassID);
            }
            if ((classInfo == null) && ClassID > 0)
            {
                classInfo = DataClassInfoProvider.GetDataClassInfo(ClassID);
            }
            if ((classInfo == null) && !String.IsNullOrEmpty(ClassName))
            {
                classInfo = DataClassInfoProvider.GetDataClassInfo(ClassName);
            }
        }

        return classInfo;
    }


    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControl()
    {
        mQueryName = GetString("queryedit.newquery");

        DataClassInfo queryClass = null;

        // If the existing query is being edited
        if (QueryID > 0)
        {
            // Get information on existing query
            Query = QueryInfo.Provider.Get(QueryID);
            if (Query != null)
            {
                queryClass = DataClassInfoProvider.GetDataClassInfo(Query.ClassID);
                ClassName = queryClass.ClassName;
                QueryName = Query.QueryName;

                if (!RequestHelper.IsPostBack())
                {
                    // Fills form with the existing query information
                    LoadValues();
                    txtQueryText.Focus();
                }
            }
        }
        else if (ClassID > 0)
        {
            queryClass = DataClassInfoProvider.GetDataClassInfo(ClassID);
            if (queryClass == null)
            {
                // For situation when object is deleted in another tab
                RedirectToInformation("editedobject.notexists");
            }

            // New query is being created
            Query = new QueryInfo();
            ClassName = queryClass.ClassName;

            if (!RequestHelper.IsPostBack())
            {
                ucSelectString.Value = queryClass.ClassConnectionString;
            }

            txtQueryName.Focus();
        }

        plcConnectionString.Visible = SystemContext.DevelopmentMode;

        // Ensure generate button and custom checkbox for default queries
        if (SqlGenerator.IsSystemQuery(QueryName))
        {
            btnGenerate.Visible = true;
        }

        // Initialize the validator
        RequiredFieldValidatorQueryName.ErrorMessage = GetString("queryedit.erroremptyqueryname");

        if (ShowHelp)
        {
            DisplayHelperTable();
        }

        // Filter available only when creating new query in dialog mode
        plcDocTypeFilter.Visible = filter.Visible = DialogMode && !EditMode;

        // Set filter preselection 
        if (plcDocTypeFilter.Visible)
        {
            filter.SelectedValue = QueryHelper.GetString("selectedvalue", null);
            filter.FilterMode = QueryInfo.OBJECT_TYPE;
        }

        // Hide header actions when creating new query in dialog mode
        if ((DialogMode && !EditMode) || !Visible)
        {
            mCurrentMaster.HeaderActions.ActionsList.Clear();
        }

        txtQueryName.Enabled = !DialogMode || !EditMode;

        // Set dialog's content panel CSS class
        if (DialogMode)
        {
            Panel pnlContent = EditMode ? mCurrentMaster.PanelContent : pnlContainer;
            pnlContent.CssClass = "PageContent";
        }


        if (!QueriesCanBeEdited)
        {
            // Disable customization for module not in development and not customizable class
            pnlCustomization.StopProcessing = true;
            HeaderAction save = HeaderActions.ActionsList.FirstOrDefault(a => a is SaveAction);
            if (save != null)
            {
                save.Enabled = false;
                ShowInformation(GetString("cms.query.customization.disabled"));
            }
        }
        else
        {
            if ((Module != null) && !Module.IsEditable && (QueryID > 0) && !Query.QueryIsLocked)
            {
                // Enable customization system tables in modules not in development (this includes different header-actions)
                pnlCustomization.HeaderActions = HeaderActions;
                pnlCustomization.MessagesPlaceHolder = MessagesPlaceHolder;
            }
            else
            {            
                // Normal processing for new queries and queries in document types etc.
                pnlCustomization.StopProcessing = true;
            }
        }
    }


    /// <summary>
    /// Displays helper table with transformation examples.
    /// </summary>
    private void DisplayHelperTable()
    {
        // Add header row
        TableHeaderRow headerRow = new TableHeaderRow
        {
            TableSection = TableRowSection.TableHeader,
            CssClass = "unigrid-head"
        };
        headerRow.Cells.Add(new TableHeaderCell
        {
            Text = GetString("queryedit.helpcaption")
        });
        headerRow.Cells.Add(new TableHeaderCell
        {
            Text = GetString("queryedit.helpdescription"),
            CssClass = "main-column-100"
        });
        tblHelp.Rows.Add(headerRow);

        // Add macro rows
        string[,] rows =
			{
				{ "##WHERE##", GetString("queryedit.help_where") },
				{ "##TOPN##", GetString("queryedit.help_topn") },
				{ "##ORDERBY##", GetString("queryedit.help_orderby") },
				{ "##COLUMNS##", GetString("queryedit.help_columns") }
			};

        for (int i = 0; i <= rows.GetUpperBound(0); i++)
        {
            TableRow macroRow = new TableRow();
            macroRow.Cells.Add(new TableCell
            {
                Text = rows[i, 0]
            });
            macroRow.Cells.Add(new TableCell
            {
                Text = rows[i, 1]
            });
            tblHelp.Rows.Add(macroRow);
        }

        // Make help table visible on click only
        tblHelp.Attributes["style"] = "display: none;";
    }


    /// <summary>
    /// Loads values from query.
    /// </summary>
    private void LoadValues()
    {
        txtQueryName.Text = QueryName;
        rblQueryType.SelectedValue = Query.QueryType.ToString();
        chbTransaction.Checked = Query.QueryRequiresTransaction;
        txtQueryText.Text = Query.QueryText;
        ucSelectString.Value = Query.QueryConnectionString;
    }


    /// <summary>
    /// Saves new or edited query of the given name and returns to the query list.
    /// </summary>
    /// <returns>True if query was successfully saved</returns>
    private bool SaveQuery()
    {
        // The query ID was specified, edit existing
        bool existing = (QueryID > 0);

        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
        if (dci == null)
        {
            ShowError(GetString("editedobject.notexists"));
            return false;
        }

        if (!QueriesCanBeEdited)
        {
            ShowError(GetString("cms.query.customization.createdisabled"));
            return false;
        }

        QueryInfo newQuery = existing ? QueryInfo.Provider.Get(QueryID) : new QueryInfo();

        // Sets query object's properties
        newQuery.QueryName = txtQueryName.Text;
        newQuery.ClassID = dci.ClassID;

        // Check the query type
        newQuery.QueryType = rblQueryType.SelectedValue == "SQLQuery" ? QueryTypeEnum.SQLQuery : QueryTypeEnum.StoredProcedure;
        newQuery.QueryRequiresTransaction = chbTransaction.Checked;

        // Only for new query
        if (!existing)
        {
            // Queries created in system development mode under any module ARE NOT custom. 
            // All other queries including those created under module that is currently in development ARE custom (they are exported with the module).
            bool custom = (Module == null) || !SystemContext.DevelopmentMode;

            newQuery.QueryIsCustom = custom;
            newQuery.QueryIsLocked = custom;
        }

        newQuery.QueryText = txtQueryText.Text.TrimEnd('\r', '\n');
        newQuery.QueryConnectionString = ValidationHelper.GetString(ucSelectString.Value, String.Empty);

        // Insert new / update existing query
        QueryInfo.Provider.Set(newQuery);

        Query = newQuery;

        return true;
    }


    private void RedirectOnSave(bool isSaved, bool closeOnSave)
    {
        if (RefreshPageURL == null || !isSaved)
        {
            return;
        }

        if (DialogMode)
        {
            // Check for selector ID
            string selector = QueryHelper.GetControlClientId("selectorid", string.Empty);
            if (!string.IsNullOrEmpty(selector))
            {
                // Add selector refresh
                StringBuilder script = new StringBuilder();
                if (!EditMode)
                {
                    ScriptHelper.RegisterWOpenerScript(Page);
                    script.AppendFormat(@"
						if (wopener) {{                        
							wopener.US_SelectNewValue_{0}('{1}');                        
						}}", selector, Query.QueryFullName);
                }

                script.AppendFormat(@"
						window.name = '{2}';
						window.open('{0}?name={1}&saved=1&editonlycode=true&selectorid={2}&tabmode={3}', window.name);
						", ScriptHelper.ResolveUrl(RefreshPageURL), Query.QueryFullName, selector, QueryHelper.GetInteger("tabmode", 0));

                if (closeOnSave)
                {
                    script.AppendLine("CloseDialog();");
                }

                ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", script.ToString(), true);
            }
        }
        else
        {
            string redirectUrl = URLHelper.AddParameterToUrl(RefreshPageURL, "saved", "1");
            redirectUrl = URLHelper.AddParameterToUrl(redirectUrl, "parentobjectid", ClassID.ToString());
            redirectUrl = URLHelper.AddParameterToUrl(redirectUrl, "objectid", Query.QueryID.ToString());
            redirectUrl = URLHelper.AddParameterToUrl(redirectUrl, "moduleid", ModuleID.ToString());
            URLHelper.Redirect(UrlResolver.ResolveUrl(redirectUrl));
        }
    }

    #endregion
}
