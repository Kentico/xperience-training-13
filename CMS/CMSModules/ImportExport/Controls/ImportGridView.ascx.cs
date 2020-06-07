using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_ImportGridView : ImportExportGridView, IUniPageable
{
    #region "Variables"

    // Indicates if object selection is enabled
    protected bool selectionEnabled = true;
    protected string codeNameColumnName = "";
    protected string displayNameColumnName = "";
    protected ArrayList mExistingObjects;

    private SiteImportSettings mSettings;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Import settings.
    /// </summary>
    public SiteImportSettings Settings
    {
        get
        {
            return mSettings ?? (mSettings = new SiteImportSettings(MembershipContext.CurrentUserProfile));
        }
        set
        {
            mSettings = value;
        }
    }


    /// <summary>
    /// Existing objects in the database.
    /// </summary>
    public ArrayList ExistingObjects
    {
        get
        {
            if (mExistingObjects == null)
            {
                mExistingObjects = new ArrayList();

                // Get the existing objects from database
                DataSet ds = ImportProvider.GetExistingObjects(Settings, ObjectType, SiteObject, true);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    // Get info object
                    GeneralizedInfo infoObj = ModuleManager.GetReadOnlyObject(ObjectType);
                    if (infoObj == null)
                    {
                        throw new Exception("[ImportGridView]: Object type '" + ObjectType + "' not found.");
                    }

                    int colIndex = ds.Tables[0].Columns.IndexOf(codeNameColumnName);
                    if (colIndex >= 0)
                    {
                        // For each object get codename
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string codeName = ValidationHelper.GetString(dr[colIndex], null);

                            if (codeName != null)
                            {
                                mExistingObjects.Add(codeName.ToLowerCSafe());
                            }
                        }
                    }
                }
            }

            return mExistingObjects;
        }
    }


    /// <summary>
    /// Data source.
    /// </summary>
    public DataSet DataSource
    {
        get;
        set;
    }


    /// <summary>
    /// Pager control.
    /// </summary>
    public UIPager PagerControl
    {
        get
        {
            return pagerElem;
        }
    }

    #endregion


    #region "Protected methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        pagerElem.PagedControl = this;

        if (RequestHelper.IsPostBack())
        {
            if (Settings != null)
            {
                // Process the results of the available tasks
                string[] available = hdnAvailableItems.Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string codeName in available)
                {
                    string name = Request.Form.AllKeys.FirstOrDefault(x => x?.EndsWith(GetCheckBoxName(codeName), StringComparison.Ordinal) ?? false) ?? string.Empty;
                    if (Request.Form[name] == null)
                    {
                        // Unchecked
                        Settings.Deselect(ObjectType, codeName, SiteObject);
                    }
                    else
                    {
                        // Checked
                        Settings.Select(ObjectType, codeName, SiteObject);
                    }
                }
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        hdnAvailableItems.Value = AvailableItems.ToString();
    }

    /// <summary>
    /// Check if object is in conflict in database.
    /// </summary>
    /// <param name="codeName">Code name</param>
    protected bool IsInConflict(object codeName)
    {
        string name = ValidationHelper.GetString(codeName, "").ToLowerCSafe();
        if ((ExistingObjects != null) && (ExistingObjects.Contains(name)))
        {
            return true;
        }
        return false;
    }


    protected string GetName(object codeNameObj, object displayNameObj)
    {
        string codeName = ValidationHelper.GetString(codeNameObj, "");
        string displayName = ValidationHelper.GetString(displayNameObj, "");

        if (string.IsNullOrEmpty(displayName))
        {
            return codeName;
        }
        return ResHelper.LocalizeString(displayName);
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        ResetFilter();
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        FilterCurrentObjectType = ObjectType;
    }


    protected void btnAll_Click(object sender, EventArgs e)
    {
        // Load all selection
        DefaultSelectionParameters parameters = new DefaultSelectionParameters()
        {
            ObjectType = ObjectType,
            SiteObjects = SiteObject,
            ImportType = ImportTypeEnum.AllNonConflicting,
            LoadTasks = false,
            ClearProgressLog = true,
            FilterCurrentWhereCondition = FilterCurrentWhereCondition
        };
        Settings.LoadDefaultSelection(parameters);

        RaiseButtonPressed(sender, e);
    }


    protected void btnNone_Click(object sender, EventArgs e)
    {
        ResetFilter();

        // Load none selection
        DefaultSelectionParameters parameters = new DefaultSelectionParameters()
        {
            ObjectType = ObjectType,
            SiteObjects = SiteObject,
            LoadTasks = false,
            ClearProgressLog = true
        };
        Settings.LoadDefaultSelection(parameters);

        RaiseButtonPressed(sender, e);
    }


    protected void btnDefault_Click(object sender, EventArgs e)
    {
        ResetFilter();

        // Load default selection
        ImportTypeEnum importType = ImportTypeEnum.Default;
        if (Settings.IsWebTemplate)
        {
            if (SiteInfoProvider.GetSitesCount() == 0)
            {
                // No site exists, overwrite all
                importType = ImportTypeEnum.AllNonConflicting;
            }
            else
            {
                // Some site exists, only new objects
                importType = ImportTypeEnum.New;
            }
        }

        DefaultSelectionParameters parameters = new DefaultSelectionParameters()
        {
            ObjectType = base.ObjectType,
            SiteObjects = base.SiteObject,
            ImportType = importType,
            LoadTasks = false,
            ClearProgressLog = true
        };
        Settings.LoadDefaultSelection(parameters);

        RaiseButtonPressed(sender, e);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Bind the data.
    /// </summary>
    public void Bind()
    {
        if (!string.IsNullOrEmpty(ObjectType))
        {
            pnlGrid.Visible = true;
            pnlSearch.Visible = true;
            selectionEnabled = ((ObjectType != LicenseKeyInfo.OBJECT_TYPE) || !Settings.IsOlderVersion);
            pnlLinks.Visible = selectionEnabled;

            // Get object info
            GeneralizedInfo info = ModuleManager.GetReadOnlyObject(ObjectType);
            if (info != null)
            {
                gvObjects.RowDataBound += gvObjects_RowDataBound;
                plcGrid.Visible = true;
                codeNameColumnName = info.CodeNameColumn;
                displayNameColumnName = info.DisplayNameColumn;

                // Set filter field
                textFilter.Field = string.IsNullOrEmpty(displayNameColumnName) ? codeNameColumnName : displayNameColumnName;

                // Hide filter if the object doesn't have needed columns
                if (string.IsNullOrEmpty(textFilter.Field))
                {
                    pnlSearch.Visible = false;
                }

                string filterWhereCondition = "";

                // Reset filter if current node has changed 
                if (FilterCurrentObjectType != ObjectType)
                {
                    ResetFilter();
                }
                else
                {
                    // Important, do not remove this check
                    if (!string.IsNullOrEmpty(textFilter.Text))
                    {
                        // Get filter condition
                        filterWhereCondition = textFilter.GetWhereCondition();
                    }
                }

                // Get data source
                DataSet ds = DataSource;

                DataTable table = null;
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    // Get the table
                    table = ObjectHelper.GetTable(ds, info);

                    // Set correct ID for direct page control
                    pagerElem.DirectPageControlID = ((float)table.Rows.Count / pagerElem.CurrentPageSize > 20.0f) ? "txtPage" : "drpPage";

                    // Filter and sort data
                    if (!DataHelper.DataSourceIsEmpty(table))
                    {
                        if (!string.IsNullOrEmpty(filterWhereCondition))
                        {
                            // Make filter condition compatible with DataTables
                            var query = filterWhereCondition;
                            query = query.Replace("[", "");
                            query = query.Replace("]", "");
                            query = query.Replace("N'%", "'%");
                            query = query.Replace("N'", "'");

                            table.DefaultView.RowFilter = query;

                            //Save filter condition to ViewState
                            FilterCurrentWhereCondition = query;
                        }

                        string orderBy = GetOrderByExpression(info);
                        table.DefaultView.Sort = orderBy;

                        // Update correct ID for direct page control
                        pagerElem.DirectPageControlID = ((float)table.Rows.Count / pagerElem.CurrentPageSize > 20.0f) ? "txtPage" : "drpPage";

                        if (ValidationHelper.GetString(table.Rows[0][codeNameColumnName], null) == null)
                        {
                            codeNameColumnName = info.TypeInfo.GUIDColumn;
                        }
                    }
                }

                // Prepare checkBox column
                TemplateField checkBoxField = (TemplateField)gvObjects.Columns[0];
                checkBoxField.HeaderText = GetString("General.Import");

                // Prepare name field
                TemplateField nameField = (TemplateField)gvObjects.Columns[1];
                nameField.HeaderText = GetString("general.displayname");

                if (!DataHelper.DataSourceIsEmpty(table) && table != null && table.DefaultView.Count > 0)
                {
                    plcObjects.Visible = true;
                    lblNoData.Visible = false;
                    gvObjects.DataSource = table;

                    // Call page binding event
                    if (OnPageBinding != null)
                    {
                        OnPageBinding(this, null);
                    }

                    PagedDataSource pagedDS = gvObjects.DataSource as PagedDataSource;
                    if (pagedDS != null)
                    {
                        if (pagedDS.PageSize <= 0)
                        {
                            gvObjects.DataSource = table;
                        }
                    }

                    gvObjects.DataBind();
                }
                else
                {
                    plcObjects.Visible = false;
                    lblNoData.Visible = true;
                    lblNoData.Text = String.Format(GetString("ImportGridView.NoData"), GetString("objecttype." + ObjectType.Replace(".", "_").Replace("#", "_")));
                }
            }
            else
            {
                plcGrid.Visible = false;
            }

            // Disable license selection
            bool enable = !((ObjectType == LicenseKeyInfo.OBJECT_TYPE) && Settings.IsOlderVersion);
            gvObjects.Enabled = enable;
            pnlLinks.Enabled = enable;
            lblInfo.Text = enable ? GetString("ImportGridView.Info") : GetString("ImportGridView.Disabled");
        }
        else
        {
            pnlGrid.Visible = false;
            gvObjects.DataSource = null;
            gvObjects.DataBind();
        }
    }


    /// <summary>
    /// Is checkBox checked.
    /// </summary>
    /// <param name="codeName">Code name</param>
    public override bool IsChecked(object codeName)
    {
        string name = ValidationHelper.GetString(codeName, "");
        if (Settings.IsSelected(ObjectType, name, SiteObject))
        {
            return true;
        }
        return false;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// On RowDataBound add CMSCheckbox to the row.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void gvObjects_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var row = (DataRowView)e.Row.DataItem;
            string codeName = ValidationHelper.GetString(row[codeNameColumnName], "");

            CMSCheckBox checkBox = GetCheckBox(codeName);
            if (!e.Row.Cells[0].Controls.OfType<CMSCheckBox>().Any(ccb => ccb.ID == checkBox.ID))
            {
                e.Row.Cells[0].Controls.Add(checkBox);
            }
            AddAvailableItem(codeName);

            if (IsInConflict(codeName))
            {
                // Adjust warning message for existing object - it will be either overwritten, or just a binding will be added (if allowed in import settings)
                string warningMessage = Settings.ImportOnlyNewObjects ? "importgridview.import.warningNewObjects" : "importgridview.import.warning";

                var icon = new CMSIcon
                {
                    ID = "warning-icon",
                    CssClass = "RightAlignAlign icon-exclamation-triangle color-orange-80 warning-icon",
                    ToolTip = GetString(warningMessage),
                };

                if (!e.Row.Cells[1].Controls.OfType<CMSIcon>().Any(ci => ci.ID == icon.ID))
                {
                    e.Row.Cells[1].Controls.Add(icon);
                }
            }
        }
    }


    // Get orderby expression
    private string GetOrderByExpression(GeneralizedInfo info)
    {
        if (info.DisplayNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
        {
            return info.DisplayNameColumn;
        }

        return codeNameColumnName;
    }


    // Resets the filter and ViewState property;
    private void ResetFilter()
    {
        textFilter.Value = null;
        FilterCurrentWhereCondition = "";
    }

    #endregion


    #region "IUniPageable Members"

    /// <summary>
    /// Pager data item.
    /// </summary>
    public object PagerDataItem
    {
        get
        {
            return gvObjects.DataSource;
        }
        set
        {
            gvObjects.DataSource = value;
        }
    }


    /// <summary>
    /// Pager control.
    /// </summary>
    public UniPager UniPagerControl
    {
        get;
        set;
    }


    public int PagerForceNumberOfResults
    {
        get
        {
            return -1;
        }
        set
        {
        }
    }

    /// <summary>
    /// Occurs when the control bind data.
    /// </summary>
    public event EventHandler<EventArgs> OnPageBinding;


    /// <summary>
    /// Occurs when the pager change the page and current mode is postback => reload data
    /// </summary>
    public event EventHandler<EventArgs> OnPageChanged;


    /// <summary>
    /// Evokes control databind.
    /// </summary>
    public virtual void ReBind()
    {
        if (OnPageChanged != null)
        {
            OnPageChanged(this, null);
        }
    }

    #endregion
}