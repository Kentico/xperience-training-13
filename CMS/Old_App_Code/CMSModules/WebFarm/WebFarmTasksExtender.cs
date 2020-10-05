using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WebFarmSync;
using CMS.WebFarmSync.Internal;

[assembly: RegisterCustomClass("WebFarmTasksExtender", typeof(WebFarmTasksExtender))]

public class WebFarmTasksExtender : ControlExtender<UniGrid>
{
    #region "Variables"

    private readonly string mAllServers = UniSelector.US_ALL_RECORDS.ToString();
    private string mSelectedServer;
    private WebFarmServerInfo mSelectedServerInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Selected web farm server name.
    /// </summary>
    private string SelectedServerName
    {
        get
        {
            if (string.IsNullOrEmpty(mSelectedServer))
            {
                mSelectedServer = ValidationHelper.GetString(Control.UIContext.Data["WebFarmServerName"], mAllServers);
            }

            return mSelectedServer;
        }
    }


    /// <summary>
    /// Selected web farm server info object.
    /// </summary>
    private WebFarmServerInfo SelectedServerInfo
    {
        get
        {
            if (SelectedServerName == mAllServers)
            {
                return null;
            }

            // Get selected server info if server is not loaded yet
            if (mSelectedServerInfo == null)
            {
                mSelectedServerInfo = WebFarmServerInfo.Provider.Get(SelectedServerName);
            }

            // Selected server not found - clear cache and try again (server can be created on other instance)
            if (mSelectedServerInfo == null)
            {
                WebFarmContext.Clear(false);
                mSelectedServerInfo = WebFarmServerInfo.Provider.Get(SelectedServerName);
            }

            return mSelectedServerInfo;
        }
    }


    /// <summary>
    /// Indicates if any particular server was selected.
    /// </summary>
    private bool ServerSelected
    {
        get
        {
            return (SelectedServerInfo != null);
        }
    }

    #endregion


    /// <summary>
    /// Custom control initialization. 
    /// </summary>
    public override void OnInit()
    {
        Control.GridView.RowDataBound += Control_GridViewRowDataBound;
        Control.OnAction += Control_OnAction;
        Control.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        Control.PreRender += Control_OnPreRender;
        Control.GridView.AddCssClass("web-farm");

        // Add header actions
        InitHeaderActions();

        if (!WebFarmLicenseHelper.LicenseIsValid)
        {
            Control.ShowError(ResHelper.GetString("webfarm.unsufficientdomainlicense"));
        }
    }


    private void Control_OnPreRender(object sender, EventArgs eventArgs)
    {
        Control.GridView.Columns[1].Visible = !ServerSelected;
    }


    /// <summary>
    /// Initialize Header actions
    /// </summary>
    private void InitHeaderActions()
    {
        // Prepare header actions
        var clearAction = new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            Text = ResHelper.GetString("WebFarmTasks_List.EmptyButton"),
            CommandName = "clear"
        };
        Control.HeaderActions.AddAction(clearAction);
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    private void Control_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            int taskId = ValidationHelper.GetInteger(actionArgument, 0);

            // Delete object from database
            if (ServerSelected)
            {
                // Delete task binding to server
                WebFarmTaskInfoProvider.DeleteServerTask(SelectedServerInfo.ServerID, taskId);
            }
            else
            {
                // Delete task object
                WebFarmTaskInfo.Provider.Get(taskId)?.Delete();
            }

            Control.ReloadData();
        }
    }



    private void Control_GridViewRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string error = ValidationHelper.GetString(((DataRowView)(e.Row.DataItem)).Row["ErrorMessage"], string.Empty);
            if (!String.IsNullOrEmpty(error))
            {
                e.Row.CssClass = "error";
            }
        }
    }


    /// <summary>
    /// Performs header actions.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "clear":
                EmptyTasks();
                break;
        }
    }


    /// <summary>
    /// Clear task list.
    /// </summary>
    private void EmptyTasks()
    {
        if (!ServerSelected)
        {
            // Delete all task objects
            WebFarmTaskInfoProvider.DeleteServerTasks();
        }
        else
        {
            // Delete bindings to specified server
            WebFarmTaskInfoProvider.DeleteServerTasks(SelectedServerInfo.ServerID);
        }

        Control.ReloadData();
    }
}
