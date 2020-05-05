using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_ExportGridTasks : ImportExportGridTask, IUniPageable
{
    #region "Variables"

    protected string codeNameColumnName = "";
    protected string displayNameColumnName = "";
    protected int pagerForceNumberOfResults = -1;

    private SiteExportSettings mSettings;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Export settings.
    /// </summary>
    public SiteExportSettings Settings
    {
        get
        {
            return (mSettings ?? (mSettings = new SiteExportSettings(MembershipContext.CurrentUserProfile)));
        }
        set
        {
            mSettings = value;
        }
    }


    /// <summary>
    /// Gets current page size from pager.
    /// </summary>
    protected int CurrentPageSize
    {
        get
        {
            return pagerElem.CurrentPageSize;
        }
    }


    /// <summary>
    /// Gets current offset.
    /// </summary>
    protected int CurrentOffset
    {
        get
        {
            return CurrentPageSize * (pagerElem.CurrentPage - 1);
        }
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
        DateTime originalTS = Settings.TimeStamp;

        Settings.TimeStamp = DateTimeHelper.ZERO_TIME;
        DefaultSelectionParameters parameters = new DefaultSelectionParameters()
        {
            ObjectType = ObjectType,
            SiteObjects = SiteObject,
            ExportType = ExportTypeEnum.All,
            LoadObjects = false,
            ClearProgressLog = true
        };
        Settings.LoadDefaultSelection(parameters);
        Settings.TimeStamp = originalTS;

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
                taskCheckBoxField.HeaderText = GetString("General.Export");

                TemplateField titleField = (TemplateField)gvTasks.Columns[1];
                titleField.HeaderText = GetString("Export.TaskTitle");

                BoundField typeField = (BoundField)gvTasks.Columns[2];
                typeField.HeaderText = GetString("general.type");

                BoundField timeField = (BoundField)gvTasks.Columns[3];
                timeField.HeaderText = GetString("Export.TaskTime");

                // Load tasks
                int siteId = (SiteObject ? Settings.SiteId : 0);

                DataSet ds = ExportTaskInfoProvider.SelectTaskList(siteId, ObjectType, null, "TaskTime DESC", 0, null, CurrentOffset, CurrentPageSize, ref pagerForceNumberOfResults);

                // Set correct ID for direct page control
                pagerElem.DirectPageControlID = ((float)pagerForceNumberOfResults / pagerElem.CurrentPageSize > 20.0f) ? "txtPage" : "drpPage";

                // Call page binding event
                if (OnPageBinding != null)
                {
                    OnPageBinding(this, null);
                }

                if (!DataHelper.DataSourceIsEmpty(ds) && (ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_TASKS), true)))
                {
                    plcTasks.Visible = true;
                    gvTasks.DataSource = ds;
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
            return pagerForceNumberOfResults;
        }
        set
        {
            pagerForceNumberOfResults = value;
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


    #region "Private methods"

    /// <summary>
    /// On RowDataBound add CMSCheckbox to the row.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Events</param>
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
}