using System;
using System.Security.Principal;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.UIControls;

public partial class CMSModules_System_Macros_System_Macros : SystemMacroPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        InitForm();
    }


    protected void ctlAsyncLog_OnCancel(object sender, EventArgs args)
    {
        LogInformationEvent("CANCELLED");

        pnlAsyncLog.Visible = false;

        ShowConfirmation(GetString("general.actioncanceled"));
    }


    protected void ctlAsyncLog_OnFinished(object sender, EventArgs args)
    {
        LogInformationEvent("FINISHED");

        pnlAsyncLog.Visible = false;

        ShowConfirmation(GetString("general.actionfinished"));
    }


    /// <summary>
    /// Runs the specified action asynchronously.
    /// </summary>
    /// <param name="action">Action</param>
    private void RunAsync(AsyncAction action)
    {
        LogInformationEvent("STARTED");

        // Run async action
        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Inits the "Refresh security parameters" form.
    /// </summary>
    private void InitForm()
    {
        ctlAsyncLog.TitleText = GetString("macros.refreshsecurityparams.title");

        // Init old salt text box
        if (chkRefreshAll.Checked)
        {
            txtOldSalt.Enabled = false;
            txtOldSalt.Text = GetString("macros.refreshsecurityparams.refreshalldescription");
        }
        else
        {
            txtOldSalt.Enabled = true;
        }

        // Init new salt text box
        if (chkUseCurrentSalt.Checked)
        {
            txtNewSalt.Enabled = false;

            var customSalt = Service.Resolve<IAppSettingsService>()[ValidationHelper.APP_SETTINGS_HASH_STRING_SALT];

            var resString = string.IsNullOrEmpty(customSalt) ? "macros.refreshsecurityparams.currentsaltisconnectionstring" : "macros.refreshsecurityparams.currentsaltiscustomvalue";

            txtNewSalt.Text = GetString(resString);
        }
        else
        {
            txtNewSalt.Enabled = true;
        }

        // Init submit button
        btnRefreshSecurityParams.Text = GetString("macros.refreshsecurityparams");
        btnRefreshSecurityParams.Click += (sender, args) =>
        {
            var oldSaltInput = txtOldSalt.Text.Trim();
            var newSaltInput = txtNewSalt.Text.Trim();

            if (!chkRefreshAll.Checked && string.IsNullOrEmpty(oldSaltInput))
            {
                ShowError(GetString("macros.refreshsecurityparams.oldsaltempty"));
                return;
            }

            if (!chkUseCurrentSalt.Checked && string.IsNullOrEmpty(newSaltInput))
            {
                ShowError(GetString("macros.refreshsecurityparams.newsaltempty"));
                return;
            }

            pnlAsyncLog.Visible = true;
            var objectTypes = Functions.GetObjectTypesWithMacros();

            RunAsync(delegate
            {
                LicenseCheckDisabler.ExecuteWithoutLicenseCheck(() => RefreshSecurityParams(objectTypes, oldSaltInput, newSaltInput, chkRefreshAll.Checked, chkUseCurrentSalt.Checked, AddLog));
            });
        };
    }


    protected void chkRefreshAll_CheckedChanged(object sender, EventArgs e)
    {
        // Clear the textbox after enabling it
        if (!chkRefreshAll.Checked)
        {
            txtOldSalt.Text = null;
        }
    }


    protected void chkUseCurrentSalt_CheckedChanged(object sender, EventArgs args)
    {
        // Clear the textbox after enabling it
        if (!chkUseCurrentSalt.Checked)
        {
            txtNewSalt.Text = null;
        }
    }


    private void AddLog(string logText)
    {
        ctlAsyncLog.AddLog(logText);
    }
}
