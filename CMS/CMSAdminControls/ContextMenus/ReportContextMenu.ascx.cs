using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.ImportExport;


public partial class CMSAdminControls_ContextMenus_ReportContextMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool enableExport = ContextMenu.Parameter.Contains("enableexport");
        bool enableSubscription = ContextMenu.Parameter.Contains("enablesubscription");
        string[] arr = ContextMenu.Parameter.Trim('\'').Split(';');
        if (arr.Length > 0)
        {
            switch (arr[0].ToLowerCSafe())
            {
                case "graph":
                    lblSubscription.ResourceString = "reportsubscription.graph";
                    break;

                case "table":
                    lblSubscription.ResourceString = "reportsubscription.table";
                    break;

                case "value":
                    lblSubscription.ResourceString = "reportsubscription.value";
                    break;

                case "htmlgraph":
                    lblSubscription.ResourceString = "reportsubscription.htmlgraph";
                    break;
            }
        }

        string menuId = ContextMenu.MenuID;

        string parentElemId = ContextMenu.ParentElementClientID;
        pnlExport.Visible = enableExport;
        pnlSubscription.Visible = enableSubscription;

        string parameterScript = "GetContextMenuParameter('" + menuId + "')";
        string closeScript = "CM_Close('" + menuId + "', 0);";
        string actionPattern = "Report_ContextMenu_" + parentElemId + "('{0}', " + parameterScript + ");HideContextMenu('" + menuId + "', true);";

        // Initialize menu
        lblExcel.Text = ResHelper.GetString("export.exporttoexcel");
        pnlExcel.Attributes.Add("onclick", string.Format(actionPattern, DataExportFormatEnum.XLSX) + closeScript);

        lblCSV.Text = ResHelper.GetString("export.exporttocsv");
        pnlCSV.Attributes.Add("onclick", string.Format(actionPattern, DataExportFormatEnum.CSV) + closeScript);

        lblXML.Text = ResHelper.GetString("export.exporttoxml");
        pnlXML.Attributes.Add("onclick", string.Format(actionPattern, DataExportFormatEnum.XML) + closeScript);

        pnlSubscription.Attributes.Add("onclick", string.Format(actionPattern, "subscription") + closeScript);
    }
}