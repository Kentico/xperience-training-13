using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.UIControls;

[EditedObject(EmailOAuthCredentialsInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.CMS, "EditEmailOAuthCredentials.General")]
public partial class CMSModules_EmailEngine_Pages_EmailOAuthCredentials_Edit : GlobalAdminPage
{
    private EmailOAuthCredentialsInfo EditedCredentials
    {
        get
        {
            return (EmailOAuthCredentialsInfo)EditedObject;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        assemblyElem.HasDependingFields = true;

        btnSetupSave.Text = GetString("general.save");
        rfvDisplayName.ErrorMessage = GetString("EmailOAuthCredentials_Edit.ErrorEmptyDisplayName");
        rfvClientID.ErrorMessage = GetString("EmailOAuthCredentials_Edit.ErrorEmptyClientID");
        rfvClientSecret.ErrorMessage = GetString("EmailOAuthCredentials_Edit.ErrorEmptyClientSecret");
        rfvTenantID.ErrorMessage = GetString("EmailOAuthCredentials_Edit.ErrorEmptyTenantID");
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!RequestHelper.IsPostBack())
        {
            ReloadData();
        }

        if (!RequestContext.IsSSL)
        {
            ShowWarning(GetString("EmailOAuthCredentials_Edit.SSLWarning"));
        }

        plcTenantID.Visible = IsTenantIDVisible();

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);
    }


    private bool IsTenantIDVisible()
    {
        return assemblyElem.IsValid() && (ClassHelper.GetClass(assemblyElem.AssemblyName, assemblyElem.ClassName) as IEmailOAuthProvider)?.RequiresTenantID == true;
    }


    /// <summary>
    /// Load data of editing task.
    /// </summary>
    protected void ReloadData()
    {
        if (EditedCredentials != null)
        {
            txtDisplayName.Text = EditedCredentials.EmailOAuthCredentialsDisplayName;
            txtClientID.Text = EditedCredentials.EmailOAuthCredentialsClientID;
            txtClientSecret.Text = EditedCredentials.EmailOAuthCredentialsClientSecret;
            txtTenantID.Text = EditedCredentials.EmailOAuthCredentialsTenantID;

            assemblyElem.AssemblyName = EditedCredentials.EmailOAuthCredentialsProviderAssemblyName;
            assemblyElem.ClassName = EditedCredentials.EmailOAuthCredentialsProviderClass;
        }
    }



    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        btnGetToken.Enabled = assemblyElem.IsValid()
                                && !String.IsNullOrEmpty(EditedCredentials.EmailOAuthCredentialsClientID)
                                && !String.IsNullOrEmpty(EditedCredentials.EmailOAuthCredentialsClientSecret)
                                && (!plcTenantID.Visible || !String.IsNullOrEmpty(EditedCredentials.EmailOAuthCredentialsTenantID));

        ConfigureTokenControl();
    }


    private void ConfigureTokenControl()
    {
        var isTokenConfigured = !String.IsNullOrEmpty(EditedCredentials.EmailOAuthCredentialsAccessToken);

        var iconTag = isTokenConfigured
                            ? UIHelper.GetAccessibleIconTag("icon-check-circle", additionalClass: "color-green-100")
                            : UIHelper.GetAccessibleIconTag("icon-exclamation-triangle", additionalClass: "color-orange-80");

        var tokenStatus = isTokenConfigured ? GetString("EmailOAuthCredentials_Edit.TokenConfigured") : GetString("EmailOAuthCredentials_Edit.TokenNotConfigured");

        ltlTokenStatus.Text = $"<div class=\"form-control-text\">{iconTag}<span class=\"TokenStatus\">{tokenStatus}</span></div>";
    }


    protected void ButtonSetupSave_Click(object sender, EventArgs e)
    {
        if (!assemblyElem.IsValid())
        {
            ShowError(assemblyElem.ValidationError);
            return;
        }


        EditedCredentials.EmailOAuthCredentialsDisplayName = txtDisplayName.Text;
        EditedCredentials.EmailOAuthCredentialsProviderAssemblyName = assemblyElem.AssemblyName;
        EditedCredentials.EmailOAuthCredentialsProviderClass = assemblyElem.ClassName;
        EditedCredentials.EmailOAuthCredentialsTenantID = txtTenantID.Text;
        EditedCredentials.EmailOAuthCredentialsClientID = txtClientID.Text;
        EditedCredentials.EmailOAuthCredentialsClientSecret = txtClientSecret.Text;

        EmailOAuthCredentialsInfo.Provider.Set(EditedCredentials);

        ShowConfirmation(GetString("EmailOAuthCredentials_Edit.SettingsSaved"));
    }


    protected void ButtonGetToken_Click(object sender, EventArgs e)
    {
        var dialogPath = (ClassHelper.GetClass(assemblyElem.AssemblyName, assemblyElem.ClassName) as IEmailOAuthProvider)?.CustomAuthorizationDialogPath;

        if (String.IsNullOrEmpty(dialogPath))
        {
            dialogPath = "~/CMSModules/EmailEngine/Pages/OAuth2AccessTokenDialog.aspx";
        }

        var dialogURL = URLHelper.GetAbsoluteUrl(dialogPath);
        dialogURL = URLHelper.AddParameterToUrl(dialogURL, "credentials", EditedCredentials.EmailOAuthCredentialsGuid.ToString());


        // Open client dialog script
        var openDialogScript = String.Format($"modalDialog('{dialogURL}', 'OAuth2AccessTokenDialog', 600, 600, null, null, null, true);");

        ScriptHelper.RegisterStartupScript(this, GetType(), "OAuth2AccessTokenOpenModal" + ClientID, openDialogScript, true);
    }
}