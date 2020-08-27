using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_ValuesTable : ValuesTable
{
    #region "Variables"

    protected int index = 0;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        EnableViewState = false;
        Visible = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        if (Table != null)
        {
            // Get the log table
            DataTable dt = Table;
            if (!DataHelper.DataSourceIsEmpty(dt))
            {
                Visible = true;

                // Set the column names
                foreach (DataColumn dc in dt.Columns)
                {
                    BoundField col = new BoundField();
                    col.DataField = dc.ColumnName;
                    col.HeaderText = GetString(ResourcePrefix + dc.ColumnName);

                    col.HeaderStyle.CopyFrom(col.ItemStyle);

                    gridValues.Columns.Add(col);
                }

                // Bind the data
                gridValues.DataSource = dt;
                gridValues.DataBind();

                if (!String.IsNullOrEmpty(Title))
                {
                    ltlInfo.Text = "<div style=\"padding: 5px 2px 2px 2px;\"><strong>" + Title + "</strong></div>";
                }
            }
        }
    }


    /// <summary>
    /// Gets the item index.
    /// </summary>
    protected int GetIndex()
    {
        return ++index;
    }
}