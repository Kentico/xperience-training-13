using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSInstall_Controls_WizardSteps_CollationDialog : CMSUserControl
{
    /// <summary>
    /// Flag indicates if existing database is on SQL Azure server.
    /// </summary>
    public bool IsSqlAzure
    {
        get;
        set;
    }


    /// <summary>
    /// Collation used on existing database.
    /// </summary>
    public string Collation
    {
        get;
        set;
    }


    /// <summary>
    /// Gets value that indicates whether user wants to change collation or not.
    /// </summary>
    public bool ChangeCollationRequested
    {
        get
        {
            return !IsSqlAzure && !rbLeaveCollation.Checked && rbChangeCollationCI.Checked;
        }
    }


    /// <summary>
    /// Initialize control based on properties.
    /// </summary>
    public void InitControls()
    {
        if (!IsSqlAzure)
        {
            // For regular database allow user to change collation through wizard
            lblCollation.Text = String.Format(ResHelper.GetFileString("install.databasecollation"), DatabaseHelper.DEFAULT_DB_COLLATION);
            rbLeaveCollation.Text = String.Format(ResHelper.GetFileString("install.leavecollation"), Collation);
            rbChangeCollationCI.Text = String.Format(ResHelper.GetFileString("install.changecollation"), DatabaseHelper.DEFAULT_DB_COLLATION);
        }
        else
        {
            // For SQL Azure database show inform message only
            lblCollation.Text = String.Format(ResHelper.GetFileString("install.databasecollationazure"), Collation, DatabaseHelper.DEFAULT_DB_COLLATION);
            rbLeaveCollation.Visible = false;
            rbChangeCollationCI.Visible = false;
        }
    }
}