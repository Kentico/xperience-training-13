using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Activities;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Activities_Controls_UI_ActivityType_List : CMSAdminListControl
{
    #region "Properties"

    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnAction += gridElem_OnAction;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (gridElem.RowsCount > 0)
        {
            int i = 0;
            DataView view = (DataView)gridElem.GridView.DataSource;
            foreach (DataRow row in view.Table.Rows)
            {
                // Hide object menu to system activity types (only custom activity types may be exported)
                if (!DataHelper.GetBoolValue(row, "ActivityTypeIsCustom"))
                {
                    if ((gridElem.GridView.Rows[i].Cells.Count > 0) && (gridElem.GridView.Rows[i].Cells[0].Controls.Count > 2)
                        && (gridElem.GridView.Rows[i].Cells[0].Controls[2] is ContextMenuContainer))
                    {
                        gridElem.GridView.Rows[i].Cells[0].Controls[2].Visible = false;
                    }
                }

                i++;
            }
        }
    }


    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                int activityTypeID = ValidationHelper.GetInteger(actionArgument, 0);
                var activityType = ActivityTypeInfo.Provider.Get(activityTypeID);
                if (!TypeCanBeDeleted(activityType))
                {
                    RedirectToInformation("general.modifynotallowed");

                    return;
                }

                ActivityTypeInfo.Provider.Delete(activityType);

                break;
        }
    }


    private bool TypeCanBeDeleted(ActivityTypeInfo activityType)
    {
        return (activityType != null) && activityType.ActivityTypeIsCustom && CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "delete":
                // Disable "delete" button for system objects
                bool iscustom = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["ActivityTypeIsCustom"], false);
                var button = (CMSGridActionButton)sender;
                if (!iscustom)
                {
                    button.Enabled = false;
                }

                break;
            case "activitytypename":
                var activityTypeRow = parameter as DataRowView;
                if (activityTypeRow == null)
                {
                    return string.Empty;
                }

                // Create tag with activity type name and color
                var activityTypeInfo = new ActivityTypeInfo(activityTypeRow.Row);
                return new Tag
                {
                    Text = activityTypeInfo.ActivityTypeDisplayName,
                    Color = activityTypeInfo.ActivityTypeColor
                };
        }
        return sender;
    }

    #endregion
}