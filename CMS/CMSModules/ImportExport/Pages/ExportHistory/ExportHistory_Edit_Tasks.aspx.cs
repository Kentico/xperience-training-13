using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Pages_ExportHistory_ExportHistory_Edit_Tasks : CMSImportExportPage
{
    #region "Page & control events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set unigrid properties
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnDataReload += UniGrid_OnDataReload;
        UniGrid.Columns = "TaskID, TaskSiteID, TaskTitle, TaskTime, TaskType";
        UniGrid.OrderBy = "TaskTime DESC";
        UniGrid.ShowActionsMenu = true;

        ExportTaskInfo eti = new ExportTaskInfo();
        UniGrid.AllColumns = SqlHelper.JoinColumnList(eti.ColumnNames);

        // Set master page properties
        PageTitle.TitleText = GetString("ExportHistory.Tasks");
        CurrentMaster.DisplaySiteSelectorPanel = true;
        CurrentMaster.DisplayControlsPanel = true;

        // Initialize javascripts
        ScriptHelper.RegisterDialogScript(this);
        btnDeleteAll.Attributes.Add("onclick", "return confirm(" + ScriptHelper.GetString(GetString("tasks.confirmdeleteall")) + ");");

        // Load sites list
        LoadSites();
    }


    protected DataSet UniGrid_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        // Where condition
        string where;

        int siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);

        // Site dropdownlist
        if (siteId != 0)
        {
            where = "TaskSiteID = " + siteId;
        }
        else
        {
            where = "TaskSiteID IS NULL";
        }

        where = SqlHelper.AddWhereCondition(where, completeWhere);

        // Get the data
        DataSet resultSet = ExportTaskInfoProvider.SelectTaskList(siteId, string.Empty, where, currentOrder, currentTopN, columns, currentOffset, currentPageSize, ref totalRecords);
        // Set visibility of delete button
        btnDeleteAll.Enabled = (totalRecords > 0);
        return resultSet;
    }


    protected void lnkDeleteAll_Click(object sender, EventArgs e)
    {
        int siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        ExportTaskInfoProvider.DeleteExportTasks(siteId);
        UniGrid.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        pnlUpdate.Update();
    }

    #endregion


    #region "UniGrid actions"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            // delete ExportHistoryTaskInfo object from database
            ExportTaskInfoProvider.DeleteExportTaskInfo(Convert.ToInt32(actionArgument));
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Load list of sites.
    /// </summary>
    private void LoadSites()
    {
        int siteId = QueryHelper.GetInteger("siteid", 0);

        // Set site selector
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.AllowAll = false;
        siteSelector.UniSelector.SpecialFields.Add(new SpecialField { Text = GetString("ExportConfiguration.NoSite"), Value = "0" });
        siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

        if (!RequestHelper.IsPostBack())
        {
            if (siteId != 0)
            {
                siteSelector.Value = siteId;
                siteSelector.Enabled = false;
            }
            else
            {
                siteSelector.Value = "0";
            }
        }
    }

    #endregion
}