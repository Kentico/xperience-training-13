using System;
using System.Data;

using CMS.Base;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;


public partial class CMSModules_System_Debug_System_LogFiles : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        gridFiles.OnAction += gridFiles_OnAction;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();

        dt.Columns.Add("Name");
        dt.Columns.Add("Size");
        ds.Tables.Add(dt);

        DirectoryInfo dir = DirectoryInfo.New(DebugHelper.LogFolder);
        FileInfo[] files = dir.GetFiles("*.log");

        // Fill the datatable with data
        foreach (FileInfo file in files)
        {
            DataRow dr = dt.NewRow();
            dr["Name"] = file.Name;
            dr["Size"] = DataHelper.GetSizeString(file.Length);
            dt.Rows.Add(dr);
        }

        // Bind the data to a grid
        gridFiles.DataSource = ds;
        gridFiles.ReloadData();
    }


    protected void gridFiles_OnAction(string actionName, object actionArgument)
    {
        if (actionName.ToLowerCSafe() == "delete")
        {
            File.Delete(DebugHelper.LogFolder + "\\" + actionArgument);
        }
    }
}