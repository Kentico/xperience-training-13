using System;
using System.Configuration;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Basic_SelectConnectionString : FormEngineUserControl
{
    #region "Variables"

    private String mDefaultConnectionString = String.Empty;
    private bool mDisplayInherit = false;

    #endregion


    #region "Properties

    /// <summary>
    /// Item's default connection string
    /// </summary>
    public String DefaultConnectionString
    {
        get
        {
            if (String.IsNullOrEmpty(mDefaultConnectionString))
            {
                mDefaultConnectionString = (Scope == PredefinedObjectType.REPORT) ? ModuleCommands.GetDefaultReportConnectionString() : ConnectionHelper.DEFAULT_CONNECTIONSTRING_NAME;
            }

            return mDefaultConnectionString;
        }
        set
        {
            mDefaultConnectionString = value;
        }
    }


    /// <summary>
    /// If true, inherit button is displayed
    /// </summary>
    public bool DisplayInherit
    {
        get
        {
            return mDisplayInherit;
        }
        set
        {
            mDisplayInherit = value;
            pnlInherit.Visible = value;
        }
    }


    /// <summary>
    /// Scope of control's usage
    /// </summary>
    public String Scope
    {
        get;
        set;
    }


    /// <summary>
    /// Connection string selected value
    /// </summary>
    public override object Value
    {
        get
        {
            if ((chkInherit.Checked) && DisplayInherit)
            {
                return String.Empty;
            }

            return drpConnString.SelectedValue;
        }
        set
        {
            CreateList();
            if ((ValidationHelper.GetString(value, String.Empty) == String.Empty))
            {
                drpConnString.SelectedValue = DefaultConnectionString;
                chkInherit.Checked = true;
            }
            else
            {
                drpConnString.SelectedValue = ValidationHelper.GetString(value, DefaultConnectionString);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates list of items
    /// </summary>
    private void CreateList()
    {
        drpConnString.Items.Clear();
        drpConnString.Items.Add(new ListItem(GetString("general.defaultchoice"), ConnectionHelper.DEFAULT_CONNECTIONSTRING_NAME));

        // Add basic conn. string when in development mode
        if (SystemContext.DevelopmentMode)
        {
            drpConnString.Items.Add(new ListItem("CMSOMConnectionString", "CMSOMConnectionString"));
            drpConnString.Items.Add(new ListItem("CMSReadOnlyConnectionString", "CMSReadOnlyConnectionString"));
        }

        // Add all connection strings in web config
        foreach (ConnectionStringSettings sett in ConfigurationManager.ConnectionStrings)
        {
            // If key is not already in dropdown or is not default connection string -- add it
            if (!drpConnString.Items.Contains(new ListItem(sett.Name, sett.Name)) && (sett.Name != ConnectionHelper.DEFAULT_CONNECTIONSTRING_NAME) && IsConnectionStringValid(sett.ConnectionString))
            {
                drpConnString.Items.Add(sett.Name);
            }
        }
    }


    /// <summary>
    /// Test if connection string is to be shown in selector
    /// </summary>
    /// <param name="conn">Connection string text</param>
    protected bool IsConnectionStringValid(String conn)
    {
        return !conn.StartsWithCSafe("ldap", true);
    }


    protected override void OnLoad(EventArgs e)
    {
        // If list was not already created (create it now)
        if (!RequestHelper.IsPostBack() && (drpConnString.Items.Count == 0))
        {
            CreateList();
        }

        chkInherit.Attributes["onclick"] = "var drp = document.getElementById('" + drpConnString.ClientID + "');if (this.checked){drp.value ='" + DefaultConnectionString + "'};drp.disabled = (this.checked);";

        if (DisplayInherit)
        {
            drpConnString.Enabled = !chkInherit.Checked;

            // As disabled dropdown returns no value, reset default value 
            if (chkInherit.Checked)
            {
                drpConnString.SelectedValue = DefaultConnectionString;
            }
        }

        base.OnLoad(e);
    }

    #endregion
}