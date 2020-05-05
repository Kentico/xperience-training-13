using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement("CMS.CustomTables", "CustomTables")]
public partial class CMSModules_CustomTables_Tools_CustomTable_List : CMSCustomTablesToolsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize master page
        PageTitle.TitleText = GetString("customtable.list.Title");
        // Initialize unigrid
        uniGrid.OnAction += uniGrid_OnAction;
        uniGrid.ZeroRowsText = GetString("customtable.notable");
        uniGrid.OnDataReload += uniGrid_OnDataReload;
        uniGrid.WhereCondition = "ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = " + SiteContext.CurrentSite.SiteID + ")";
    }

    #endregion


    #region "Unigrid events"

    /// <summary>
    /// Data reloading event handler.
    /// </summary>
    /// <param name="completeWhere">Complete where condition</param>
    /// <param name="currentOrder">Current order by clause</param>
    /// <param name="currentTopN">Current top N value</param>
    /// <param name="columns">Currently selected columns</param>
    /// <param name="currentOffset">Current page offset</param>
    /// <param name="currentPageSize">Current size of page</param>
    /// <param name="totalRecords">Returns number of returned records</param>
    protected DataSet uniGrid_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        // Get all custom tables which may current user read
        return CustomTableHelper.GetFilteredTablesByPermission(completeWhere, currentOrder, currentTopN, columns);
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName != "edit")
        {
            return;
        }

        int classId = ValidationHelper.GetInteger(actionArgument, 0);
        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(classId);
        if (dci == null)
        {
            return;
        }

        URLHelper.Redirect(UrlResolver.ResolveUrl("CustomTable_Data_List.aspx?objectid=" + classId));
    }

    #endregion
}