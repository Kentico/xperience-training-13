using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.ImportExport;


public partial class CMSAdminControls_UI_UniGrid_Controls_UniGridMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string menuId = ContextMenu.MenuID;
        string parentElemId = ContextMenu.ParentElementClientID;

        // Get menu settings
        string[] settings = ContextMenu.Parameter.Split('|');

        bool allowExport = ValidationHelper.GetBoolean(settings[0], false);
        bool allowReset = ValidationHelper.GetBoolean(settings[1], false);
        bool allowShowFilter = SystemContext.DevelopmentMode && ValidationHelper.GetBoolean(settings[2], false);

        string parameterScript = "GetContextMenuParameter('" + menuId + "')";

        string actionPattern = "window.CMS.UG_Export_" + parentElemId + ".ugExport('{0}', " + parameterScript + ");";

        string jsGrid = "window.CMS.UG_" + parentElemId;

        var disableProgressScript = ScriptHelper.GetDisableProgressScript();
        
        // Initialize menu
        if (allowExport)
        {
            iExcel.Attributes.Add("onclick", disableProgressScript + String.Format(actionPattern, DataExportFormatEnum.XLSX));
            iCSV.Attributes.Add("onclick", disableProgressScript + String.Format(actionPattern, DataExportFormatEnum.CSV));
            iXML.Attributes.Add("onclick", disableProgressScript + String.Format(actionPattern, DataExportFormatEnum.XML));
            iAdvanced.Attributes.Add("onclick", string.Format(actionPattern, "advancedexport"));

            sm1.Visible = allowReset || allowShowFilter;
        }
        else
        {
            plcExport.Visible = false;
        }

        iReset.Visible = allowReset;
        iReset.Attributes.Add("onclick", disableProgressScript + jsGrid + ".reset();");

        iFilter.Visible = allowShowFilter;
        iFilter.Attributes.Add("onclick", disableProgressScript + jsGrid + ".showFilter();");
    }
}
