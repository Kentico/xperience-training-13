using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_QueryLog : QueryLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            Visible = true;

            // Setup header texts
            gridQueries.SetHeaders("", "QueryLog.QueryText", "General.Context", "QueryLog.QueryDuration");

            HeaderText = GetString("QueryLog.Info");

            // Override maximum size with parameters if larger
            int paramSize = DataHelper.GetMaximumValue<int>(dt, "QueryParametersSize");
            if (paramSize > MaxSize)
            {
                MaxSize = paramSize;
            }

            // Bind the data
            BindGrid(gridQueries, dt);
        }
    }
}