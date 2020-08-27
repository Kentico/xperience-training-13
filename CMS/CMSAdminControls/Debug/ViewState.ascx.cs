using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_ViewState : ViewStateLog
{
    protected void Page_Load(object sender, EventArgs e)
    {
        EnableViewState = false;
        Visible = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        try
        {
            if (Log != null)
            {
                // Get the log table
                DataTable dt = Log.LogTable;
                lock (dt)
                {
                    if (!DataHelper.DataSourceIsEmpty(dt))
                    {
                        var dv = new DataView(dt);

                        Visible = true;

                        gridStates.SetHeaders("", "ViewStateLog.ID", "ViewStateLog.IsDirty", "ViewStateLog.ViewState", "ViewStateLog.Size");

                        HeaderText = GetString("ViewStateLog.Info");

                        // Bind to the grid
                        if (DisplayOnlyDirty)
                        {
                            dv.RowFilter = "HasDirty = 1";
                        }

                        MaxSize = DataHelper.GetMaximumValue<int>(dv, "ViewStateSize");

                        // Bind the data
                        BindGrid(gridStates, dv);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            HeaderText = "Unable to acquire ViewState from the controls collection: " + ex.Message;
            Visible = true;

            Service.Resolve<IEventLogService>().LogException("Debug", "GETVIEWSTATE", ex);
        }
    }
}