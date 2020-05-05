using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[Title("customtable.list.Title")]
[Action(0, "customtable.list.NewCustomTable", "CustomTable_New.aspx")]
[UIElement(ModuleName.CUSTOMTABLES, "Development.CustomTables")]
public partial class CMSModules_CustomTables_CustomTable_List : CMSCustomTablesPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize grid
        uniGrid.OnAction += uniGrid_OnAction;
        uniGrid.ZeroRowsText = GetString("general.nodatafound");
        uniGrid.EditActionUrl = GetEditUrl();
    }

    #endregion


    #region "Grid events"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl("CustomTable_Edit.aspx?customtableid=" + actionArgument));
        }
        else if (actionName == "delete")
        {
            int classId = ValidationHelper.GetInteger(actionArgument, 0);
            var dci = DataClassInfoProvider.GetDataClassInfo(classId);
            if (dci == null)
            {
                return;
            }

            try
            {
                // Delete the class
                DataClassInfoProvider.DeleteDataClassInfo(classId);
                CustomTableItemProvider.ClearLicensesCount(true);
            }
            catch (CheckDependenciesException)
            {
                var description = uniGrid.GetCheckDependenciesDescription(dci);
                ShowError(GetString("unigrid.deletedisabledwithoutenable"), description);
            }
            catch (Exception ex)
            {
                LogAndShowError("Custom table", "Delete", ex);
            }
        }
    }

    #endregion

    
    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.CustomTables", "EditCustomTable");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, false), "objectid={0}");
        }

        return String.Empty;
    }

    #endregion
}