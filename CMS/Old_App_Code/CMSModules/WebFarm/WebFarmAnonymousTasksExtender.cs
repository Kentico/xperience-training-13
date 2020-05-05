using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WebFarmSync;

[assembly: RegisterCustomClass("WebFarmAnonymousTasksExtender", typeof(WebFarmAnonymousTasksExtender))]

/// <summary>
/// Web farm anonymous tasks unigrid extender.
/// </summary>
public class WebFarmAnonymousTasksExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// Custom control initialization. 
    /// </summary>
    public override void OnInit()
    {
        Control.GridView.DataBound += Control_GridViewDataBound;
        Control.GridView.RowDataBound += Control_GridViewRowDataBound;
        Control.OnAction += Control_OnAction;
        Control.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        Control.ZeroRowsText = ResHelper.GetString("WebFarmTasks_List.ZeroRows");
        Control.GridView.AddCssClass("web-farm");

        // Add header actions
        InitHeaderActions();
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
            // Delete task object
            WebFarmTaskInfoProvider.DeleteWebFarmTaskInfo(Convert.ToInt32(actionArgument));

            Control.ReloadData();
        }
    }


    /// <summary>
    /// Unigrid - data bind.
    /// </summary>
    private void Control_GridViewDataBound(object sender, EventArgs e)
    {
        Control.HeaderActions.Visible = !DataHelper.DataSourceIsEmpty(Control.GridView.DataSource);
    }


    /// <summary>
    /// Unigrid - row data bind.
    /// </summary>
    protected void Control_GridViewRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string code = ValidationHelper.GetString(((DataRowView)(e.Row.DataItem)).Row["TaskErrorMessage"], string.Empty);
            if (!string.IsNullOrEmpty(code))
            {
                e.Row.CssClass = "error"; 
            }
        }
    }


    /// <summary>
    /// Initialize Header actions
    /// </summary>
    private void InitHeaderActions()
    {
        var clearTasks = new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            Text = ResHelper.GetString("WebFarmTasks_List.EmptyButton"),
            CommandName = "empty"
        };

        Control.AddHeaderAction(clearTasks);
    }


    /// <summary>
    /// Header actions - perform action.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "empty":
                // Delete anonymous tasks
                WebFarmTaskInfoProvider.DeleteAnonymousTasks();
                Control.ReloadData();
                break;
        }
    }
}
