using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_RequestLog : RequestProcessLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        if (Log != null)
        {
            // Log request values
            RequestDebug.LogRequestValues(true, true, true);

            var dt = GetLogData();
            if (dt != null)
            {
                Visible = true;

                // Ensure value collections
                if (Log.ValueCollections != null)
                {
                    tblResC.Title = GetString("RequestLog.ResponseCookies");
                    tblResC.Table = Log.ValueCollections.Tables["ResponseCookies"];

                    tblReqC.Title = GetString("RequestLog.RequestCookies");
                    tblReqC.Table = Log.ValueCollections.Tables["RequestCookies"];

                    tblVal.Title = GetString("RequestLog.Values");
                    tblVal.Table = Log.ValueCollections.Tables["Values"];
                }

                // Ensure header texts
                gridCache.SetHeaders("", "RequestLog.Operation", "RequestLog.Parameter", "RequestLog.FromStart", "RequestLog.Duration");

                HeaderText = GetString("RequestLog.Info");

                // Bind the data
                BindGrid(gridCache, dt);
            }
        }
    }
}