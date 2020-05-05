using System;
using System.Security.Principal;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSInstall_Controls_WizardSteps_UserServer : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Server name.
    /// </summary>
    public string ServerName
    {
        get
        {
            return txtServerName.Text.Trim();
        }
        set
        {
            txtServerName.Text = value;
        }
    }


    /// <summary>
    /// Database password.
    /// </summary>
    public string DBPassword
    {
        get
        {
            return txtDBPassword.Text.Trim();
        }
    }


    /// <summary>
    /// User name.
    /// </summary>
    public string DBUsername
    {
        get
        {
            return txtDBUsername.Text.Trim();
        }
    }


    /// <summary>
    /// Windows authentication checked
    /// </summary>
    public bool WindowsAuthenticationChecked
    {
        get
        {
            return radWindowsAuthentication.Checked;
        }
    }


    /// <summary>
    /// Indicates if is DB separation.
    /// </summary>
    public bool IsDBSeparation
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {

        lblSQLServer.Text = IsDBSeparation ? GetString("separationDB.server") : ResHelper.GetFileString("Install.lblSQLServer");

        txtDBPassword.Enabled = radSQLAuthentication.Checked;
        txtDBUsername.Enabled = radSQLAuthentication.Checked;

        if (ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSWWAGInstallation"], false))
        {
            radWindowsAuthentication.Text = ResHelper.GetFileString("Install.WAGradWindowsAuthentication");
            radWindowsAuthentication.Enabled = false;
        }
        else
        {
            radWindowsAuthentication.Text = ResHelper.GetFileString("Install.radWindowsAuthentication") + "<br /><span>" + String.Format(ResHelper.GetFileString("Install.Account"), WindowsIdentity.GetCurrent().Name) + "</span>";
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        txtDBPassword.Enabled = radSQLAuthentication.Checked;
        txtDBUsername.Enabled = radSQLAuthentication.Checked;

        if (IsDBSeparation)
        {
            plcRadSQL.Visible = false;
            plcWinAuth.Visible = false;

            if (SqlInstallationHelper.DatabaseIsSeparated())
            {
                DisplaySeparationError(GetString("separationDB.separationerror"));
            }
        }
    }


    /// <summary>
    /// Validates control, returns error if failed.
    /// </summary>
    public bool ValidateForSeparation()
    {
        // Check if separation is not already completed
        if (SqlInstallationHelper.DatabaseIsSeparated())
        {
            DisplaySeparationError(GetString("separationDB.separationerror"));
            return false;
        }

        // Check the server name
        if (String.IsNullOrEmpty(txtServerName.Text))
        {
            DisplaySeparationError(ResHelper.GetFileString("Install.ErrorServerEmpty"));
            return false;
        }

        if (radSQLAuthentication.Checked)
        {
            // Check the user name and password
            if (String.IsNullOrEmpty(txtDBUsername.Text) || String.IsNullOrEmpty(txtDBPassword.Text))
            {
                DisplaySeparationError(ResHelper.GetFileString("install.errorusernamepasswordempty"));
                return false;
            }
        }
        
        // Check if it is possible to connect to the database
        string res = TestNewConnection();
        if (string.IsNullOrEmpty(res))
        {
            return true;
        }

        DisplaySeparationError(res);
        return false;
    }


    /// <summary>
    /// Displays separation error.
    /// </summary>
    private void DisplaySeparationError(string error)
    {
        plcSeparationError.Visible = true;
        lblError.Text = HTMLHelper.HTMLEncode(error);
    }


    /// <summary>
    /// Test new connection.
    /// </summary>
    private string TestNewConnection()
    {
        SQLServerAuthenticationModeEnum authenticationType = radSQLAuthentication.Checked 
                                                                    ? SQLServerAuthenticationModeEnum.SQLServerAuthentication 
                                                                    : SQLServerAuthenticationModeEnum.WindowsAuthentication;

        string res = ConnectionHelper.TestConnection(authenticationType, txtServerName.Text.Trim(), String.Empty, txtDBUsername.Text.Trim(), txtDBPassword.Text.Trim());
        return res;
    }
}