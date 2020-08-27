using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.DataEngine.CollectionExtensions;

using System.Linq;

using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_FilesLog : FilesLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            DataView dv;
            var providerSet = !String.IsNullOrEmpty(ProviderName);

            lock (dt)
            {
                if (providerSet || WriteOnly)
                {
                    var readOnlyOperationSet = FileDebugOperation.ReadOnlyOperations.ToHashSetCollection(StringComparer.InvariantCultureIgnoreCase);

                    var results = from row in dt.AsEnumerable()
                                  where (!providerSet || row.Field<string>("ProviderName").Equals(ProviderName, StringComparison.InvariantCultureIgnoreCase)) 
                                        && (!WriteOnly || !readOnlyOperationSet.Contains(row.Field<string>("FileOperation")))
                                  select row;

                    dv = results.AsDataView();
                }
                else
                {
                    dv = new DataView(dt);
                }
            }

            if (!DataHelper.DataSourceIsEmpty(dv))
            {
                Visible = true;

                gridStates.SetHeaders("", "FilesLog.Operation", "FilesLog.FilePath", "FilesLog.OperationType", "General.Context");

                // Hide the operation type column if only specific operation type is selected
                if (providerSet)
                {
                    gridStates.Columns[3].Visible = false;
                }

                HeaderText = GetString("FilesLog.Info");

                // Bind the data
                BindGrid(gridStates, dv);
            }
        }
    }
}