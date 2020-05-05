using System;
using System.Collections.Generic;
using System.Data;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Reporting;
using CMS.UIControls;


public partial class CMSModules_Reporting_Dialogs_ReportParametersSelector : CMSModalPage
{
    /// <summary>
    /// OnInit.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        ScriptHelper.RegisterWOpenerScript(Page);
        PageTitle.TitleText = GetString("rep.webparts.reportparameterstitle");
        // Load data for this form
        int reportID = QueryHelper.GetInteger("ReportID", 0);
        hdnGuid.Value = QueryHelper.GetString("GUID", String.Empty);
        ReportInfo ri = ReportInfoProvider.GetReportInfo(reportID);
        if (ri == null)
        {
            return;
        }

        FormInfo fi = new FormInfo(ri.ReportParameters);
        bfParameters.FormInformation = fi;
        bfParameters.SubmitButton.Visible = false;
        bfParameters.Mode = FormModeEnum.Update;

        // Get dataset from cache
        DataSet ds = (DataSet)WindowHelper.GetItem(hdnGuid.Value);
        DataRow dr = fi.GetDataRow(false);
        fi.LoadDefaultValues(dr, true);

        if (ds == null)
        {
            if (dr.ItemArray.Length > 0)
            {
                // Set up grid
                bfParameters.DataRow = RequestHelper.IsPostBack() ? fi.GetDataRow(false) : dr;
            }
        }


        // Set data set given from cache
        if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
        {
            //Merge with default data from report
            MergeDefaultValuesWithData(dr, ds.Tables[0].Rows[0]);
            //Set row to basic form
            bfParameters.DataRow = dr;
        }

        // Test if there is any item visible in report parameters
        bool itemVisible = false;
        List<IDataDefinitionItem> items = fi.ItemsList;
        foreach (IDataDefinitionItem item in items)
        {
            FormFieldInfo ffi = item as FormFieldInfo;
            if (ffi != null && ffi.Visible)
            {
                itemVisible = true;
                break;
            }
        }

        if (!itemVisible)
        {
            ShowInformation(GetString("rep.parameters.noparameters"));
        }

        base.OnInit(e);
    }


    /// <summary>
    /// Merge default values with data row. If column not exist in data row default value is loaded.
    /// </summary>
    /// <param name="defaultRow">Default data row</param>
    /// <param name="dataRow">Changed data row</param>
    protected void MergeDefaultValuesWithData(DataRow defaultRow, DataRow dataRow)
    {
        // Search all columns in defualt report parameters
        foreach (DataColumn col in dataRow.Table.Columns)
        {
            string sourceColumnName = col.ColumnName;
            if (defaultRow.Table.Columns.Contains(sourceColumnName))
            {
                // Use try-catch block, to prevent error in conversion among different data types (string->int).         
                try
                {
                    defaultRow[sourceColumnName] = dataRow[sourceColumnName];
                }
                catch (Exception)
                {
                }
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Save += btnOK_Click;
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        // If data are valid
        if (bfParameters.ValidateData())
        {
            // Fill BasicForm with user data
            bfParameters.SaveData(null);
            if (bfParameters.DataRow != null)
            {
                int reportID = QueryHelper.GetInteger("ReportID", 0);
                ReportInfo ri = ReportInfoProvider.GetReportInfo(reportID);
                if (ri == null)
                {
                    return;
                }

                FormInfo fi = new FormInfo(ri.ReportParameters);
                DataRow defaultRow = fi.GetDataRow(false);
                fi.LoadDefaultValues(defaultRow, true);

                // Load default parameters to items ,where displayed in edit form not checked
                List<IDataDefinitionItem> items = fi.ItemsList;
                foreach (IDataDefinitionItem item in items)
                {
                    FormFieldInfo ffi = item as FormFieldInfo;
                    if ((ffi != null) && (!ffi.Visible))
                    {
                        bfParameters.DataRow[ffi.Name] = defaultRow[ffi.Name];
                    }
                }

                WindowHelper.Add(hdnGuid.Value, bfParameters.DataRow.Table.DataSet);
            }
            // Refreshh opener update panel script
            ltlScript.Text = ScriptHelper.GetScript("wopener.refresh (); CloseDialog()");
        }
    }
}
