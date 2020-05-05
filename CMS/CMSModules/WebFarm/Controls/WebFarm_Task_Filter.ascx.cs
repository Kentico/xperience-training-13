using System;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;

public partial class CMSModules_WebFarm_Controls_WebFarm_Task_Filter : CMSAbstractBaseFilterControl
{
    #region "Properties"

    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            drpTaskStatus.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), "all"));
            drpTaskStatus.Items.Add(new ListItem(ResHelper.GetString("webfarm.notprocessed"), "notprocessed"));
            drpTaskStatus.Items.Add(new ListItem(ResHelper.GetString("general.failed"), "failed"));
        }
    }


    /// <summary>
    /// Generates where condition.
    /// </summary>
    protected string GenerateWhereCondition()
    {
        string where = String.Empty;
        string status = drpTaskStatus.SelectedValue;

        switch (status)
        {
            case "notprocessed":
                where = SqlHelper.AddWhereCondition(where, "ErrorMessage IS NULL");
                break;

            case "failed":
                where = SqlHelper.AddWhereCondition(where, "ErrorMessage IS NOT NULL");
                break;
        }
        return where;
    }


    /// <summary>
    /// Resets filter to default state.
    /// </summary>
    public override void ResetFilter()
    {
        if (drpTaskStatus.Items.Count > 0)
        {
            drpTaskStatus.SelectedIndex = 0;
        }
    }

    #endregion
}