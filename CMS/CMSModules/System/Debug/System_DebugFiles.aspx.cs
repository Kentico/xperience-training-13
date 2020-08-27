using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;


public partial class CMSModules_System_Debug_System_DebugFiles : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("Debug.ClearLog");

        ReloadData();
    }


    protected void ReloadData()
    {
        drpOperationType.Visible = true;
        lblOperationType.Visible = true;

        if (!RequestHelper.IsPostBack())
        {
            var items = drpOperationType.Items;
            items.Add(new ListItem(GetString("general.selectall"), ""));
            items.Add(new ListItem(GetString("FilesLog.TypeFileSystem"), IOProviderName.FileSystem));
            items.Add(new ListItem(GetString("FilesLog.TypeZip"), IOProviderName.Zip));
            items.Add(new ListItem(GetString("FilesLog.TypeAzureBlob"), IOProviderName.Azure));
            items.Add(new ListItem(GetString("FilesLog.TypeAmazonS3"), IOProviderName.Amazon));
        }

        if (FileDebug.Settings.Enabled)
        {
            plcLogs.Controls.Clear();

            var logs = FileDebug.Settings.LastLogs;

            RequestLog lastLog = null;

            for (int i = logs.Count - 1; i >= 0; i--)
            {
                try
                {
                    // Get the log
                    RequestLog log = logs[i];
                    if (log != null)
                    {
                        // Load the table
                        DataTable dt = log.LogTable;
                        if (!DataHelper.DataSourceIsEmpty(dt))
                        {
                            var providerName = drpOperationType.SelectedValue;

                            DataView dv;

                            // Apply operation type filter
                            lock (dt)
                            {
                                dv = new DataView(dt);
                                if (!String.IsNullOrEmpty(providerName))
                                {
                                    dv.RowFilter = "ProviderName = '" + providerName + "'";
                                }
                            }

                            bool display = !DataHelper.DataSourceIsEmpty(dv);
                            if (display)
                            {
                                // Load the control
                                FilesLog logCtrl = (FilesLog)LoadLogControl(log, "~/CMSAdminControls/Debug/FilesLog.ascx", i);

                                logCtrl.PreviousLog = lastLog;
                                logCtrl.ShowCompleteContext = chkCompleteContext.Checked;
                                logCtrl.ProviderName = providerName;
                                logCtrl.WriteOnly = chkWriteOnly.Checked;

                                // Add to the output
                                plcLogs.Controls.Add(logCtrl);

                                lastLog = log;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        FileDebug.Settings.LastLogs.Clear();
        ReloadData();
    }
}