using System;
using System.Collections;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSAdminControls_UI_ObjectParameters : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// FormInfo for specific control.
    /// </summary>
    public FormInfo FormInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Parameters data.
    /// </summary>
    public ObjectParameters Parameters
    {
        get;
        set;
    }


    /// <summary>
    /// Basic form.
    /// </summary>
    public BasicForm BasicForm
    {
        get
        {
            return form;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Checks if form is loaded with any controls and returns appropriate value.
    /// </summary>
    public bool CheckVisibility()
    {
        Visible = form.IsAnyFieldVisible();
        return Visible;
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void ReloadData(bool forceReloadCategories)
    {
        if (FormInfo != null)
        {
            form.SubmitButton.Visible = false;
            form.SiteName = SiteContext.CurrentSiteName;
            form.FormInformation = FormInfo;
            form.Data = GetData();
            form.MacroTable = GetMacroTable();
            form.ForceReloadCategories = forceReloadCategories;
            form.EnsureMessagesPlaceholder(MessagesPlaceHolder);
        }
        else
        {
            form.DataRow = null;
            form.FormInformation = null;
        }
        form.ReloadData();
    }


    /// <summary>
    /// Saves basic form data and updates Parameters property.
    /// </summary>
    /// <param name="showChangesSaved">Indicates whether info message "Changes were saved" should be displayed</param>
    public bool SaveData(bool showChangesSaved)
    {
        if (form.Visible)
        {
            bool saved = form.SaveData(null, showChangesSaved);
            if (saved)
            {
                // After successful save switch from insert mode
                form.Mode = FormModeEnum.Update;

                if (Parameters == null)
                {
                    Parameters = new ObjectParameters();
                }

                // Save data
                foreach (DataColumn column in form.DataRow.Table.Columns)
                {
                    string colName = column.ColumnName;
                    // Save macro value
                    if ((form.MacroTable != null) && (form.MacroTable[colName.ToLowerInvariant()]) != null)
                    {
                        Parameters[colName] = form.MacroTable[colName.ToLowerInvariant()];
                    }
                    else
                    {
                        Parameters[colName] = form.DataRow.Table.Rows[0][colName];
                    }
                }

                Parameters.MacroTable = form.MacroTable;
            }
            return saved;
        }

        return true;
    }


    /// <summary>
    /// Validates the data, returns true if succeeded.
    /// </summary>
    public bool ValidateData()
    {
        return !form.Visible || form.ValidateData();
    }


    /// <summary>
    /// Loads DataRow for BasicForm with data from Parameters property.
    /// </summary>
    private DataRowContainer GetData()
    {
        DataRowContainer result = new DataRowContainer(FormInfo.GetDataRow());

        if (Parameters != null)
        {
            foreach (DataColumn column in result.DataRow.Table.Columns)
            {
                string columnName = column.ColumnName;
                if (!String.IsNullOrEmpty(Convert.ToString(Parameters[columnName])) && ValidationHelper.IsType(column.DataType, Parameters[columnName]))
                {
                    result[columnName] = Parameters[columnName];
                }
            }
        }
        return result;
    }


    /// <summary>
    /// Loads macro table
    /// </summary>
    private Hashtable GetMacroTable()
    {
        if (Parameters != null)
        {
            return Parameters.MacroTable;
        }

        return null;
    }

    #endregion
}