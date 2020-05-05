using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_ImportGridTasks : ImportExportGridTask, IUniPageable
{
    #region "Variables"

    // Indicates if object selection is enabled
    protected bool selectionEnabled = true;

    protected string codeNameColumnName = "";
    protected string displayNameColumnName = "";
    
    protected ArrayList mExistingObjects;
    protected SiteImportSettings mSettings;
    
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
                DataSet ds = ImportProvider.GetExistingObjects(Settings, ObjectType, SiteObject, true);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    // Get info object
                    GeneralizedInfo infoObj = ModuleManager.GetReadOnlyObject(ObjectType);
                    if (infoObj == null)
                    {
                        throw new Exception("[ImportGridView]: Object type '" + ObjectType + "' not found.");
                    }

                    int colIndex = ds.Tables[0].Columns.IndexOf(infoObj.CodeNameColumn);
                    if (colIndex >= 0)
                    {
                        // For each object get codename
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string codeName = ValidationHelper.GetString(dr[colIndex], null);

                            if (codeName != null)
                            {
                                // Initialize array list
                                if (mExistingObjects == null)
                                {
                                    mExistingObjects = new ArrayList();
                                }

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
                string[] available = hdnAvailableTasks.Value.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in available)
                {
                    int taskId = ValidationHelper.GetInteger(item, 0);
                    string name = Request.Form.AllKeys.FirstOrDefault(x => x?.EndsWith(GetCheckBoxName(taskId), StringComparison.Ordinal) ?? false) ?? string.Empty;

                    if (Request.Form[name] == null)
                    {
                        // Unchecked
                        Settings.DeselectTask(ObjectType, taskId, SiteObject);
                    }
                    else
                    {
                        // Checked
                        Settings.SelectTask(ObjectType, taskId, SiteObject);
                    }
                }
            }
        }
    }

    
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        
        // Render the available task IDs
        hdnAvailableTasks.Value = AvailableItems.ToString();
    }


    protected void btnAll_Click(object sender, EventArgs e)
    {
        // Load all selection
        DefaultSelectionParameters parameters = new DefaultSelectionParameters()
        {
            ObjectType = ObjectType,
            SiteObjects = SiteObject,
            ImportType = ImportTypeEnum.AllNonConflicting,
            LoadObjects = false,
            ClearProgressLog = true
        };
        Settings.LoadDefaultSelection(parameters);

        RaiseButtonPressed(sender, e);
    }


    protected void btnNone_Click(object sender, EventArgs e)
    {
        // Load none selection
        DefaultSelectionParameters parameters = new DefaultSelectionParameters()
        {
            ObjectType = ObjectType,
            SiteObjects = SiteObject,
            LoadObjects = false,
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
            selectionEnabled = ((ObjectType != LicenseKeyInfo.OBJECT_TYPE) || !Settings.IsOlderVersion);

            pnlTaskLinks.Visible = selectionEnabled;

            // Get object info
            GeneralizedInfo info = ModuleManager.GetReadOnlyObject(ObjectType);
            if (info != null)
            {
                gvTasks.RowDataBound += gvTasks_RowDataBound;
                plcGrid.Visible = true;
                codeNameColumnName = info.CodeNameColumn;
                displayNameColumnName = info.DisplayNameColumn;

                // Task fields
                TemplateField taskCheckBoxField = (TemplateField)gvTasks.Columns[0];
                taskCheckBoxField.HeaderText = GetString("General.Process");

                BoundField titleField = (BoundField)gvTasks.Columns[1];
                titleField.HeaderText = GetString("Export.TaskTitle");

                BoundField typeField = (BoundField)gvTasks.Columns[2];
                typeField.HeaderText = GetString("general.type");

                BoundField timeField = (BoundField)gvTasks.Columns[3];
                timeField.HeaderText = GetString("Export.TaskTime");

                // Load tasks
                DataSet ds = DataSource;
                if (!DataHelper.DataSourceIsEmpty(ds) && !DataHelper.DataSourceIsEmpty(ds.Tables["Export_Task"]) && (ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_TASKS), true)))
                {
                    plcTasks.Visible = true;
                    gvTasks.DataSource = ds.Tables["Export_Task"];

                    // Set correct ID for direct page control
                    pagerElem.DirectPageControlID = ((float)ds.Tables["Export_Task"].Rows.Count / pagerElem.CurrentPageSize > 20.0f) ? "txtPage" : "drpPage";

                    // Call page binding event
                    if (OnPageBinding != null)
                    {
                        OnPageBinding(this, null);
                    }

                    gvTasks.DataBind();
                }
                else
                {
                    plcTasks.Visible = false;
                }
            }
            else
            {
                plcGrid.Visible = false;
            }

            // Disable license selection
            bool enable = !((ObjectType == LicenseKeyInfo.OBJECT_TYPE) && Settings.IsOlderVersion);
            gvTasks.Enabled = enable;
            pnlTaskLinks.Enabled = enable;
        }
        else
        {
            pnlGrid.Visible = false;
        }
    }

    
    /// <summary>
    /// Ensure tasks preselection.
    /// </summary>
    /// <param name="taskId">Task ID</param>
    public override bool IsChecked(object taskId)
    {
        int id = ValidationHelper.GetInteger(taskId, 0);
        if (Settings.IsTaskSelected(ObjectType, id, SiteObject))
        {
            return true;
        }
        return false;
    }

    #endregion


    #region "Private methods"

    private void gvTasks_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var row = (DataRowView)e.Row.DataItem;
            string codeName = ValidationHelper.GetString(row[TASK_ID], "");

            AddAvailableItem(codeName);
            e.Row.Cells[0].Controls.Add(GetCheckBox(codeName));
        }
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
            return gvTasks.DataSource;
        }
        set
        {
            gvTasks.DataSource = value;
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