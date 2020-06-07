using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_CheckoutProcess : CMSEcommerceStoreSettingsPage
{
    private SiteInfo configuredSite = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        configuredSite = SiteInfo.Provider.Get(ConfiguredSiteID);

        // Check UI element
        var elementName = IsMultiStoreConfiguration ? "Tools.Ecommerce.ChackoutSettings" : "Configuration.Settings.CheckoutProcess";
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, elementName);

        // Set up header
        InitHeaderActions();

        // Modify page content css class
        Panel pnl = CurrentMaster.PanelBody.FindControl("pnlContent") as Panel;
        if (pnl != null)
        {
            pnl.CssClass = "";
        }

        ucCheckoutProcess.OnModeChanged += ucCheckoutProcess_OnModeChanged;

        // Register javascript to confirm generate default checkout process
        string script = "function ConfirmGlobalProcess() {return confirm(" + ScriptHelper.GetString(GetString("CheckoutProcess.ConfirmGlobalProcess")) + ");}";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmGlobalProcess", ScriptHelper.GetScript(script));

        ucCheckoutProcess.OnCheckoutProcessDefinitionUpdate += ucCheckoutProcess_OnCheckoutProcessDefinitionUpdate;

        // Load data
        if (!RequestHelper.IsPostBack())
        {
            if (configuredSite != null)
            {
                ucCheckoutProcess.Value = ECommerceSettings.CheckoutProcess(configuredSite.SiteName);
            }
            else
            {
                ucCheckoutProcess.Value = ECommerceSettings.CheckoutProcess(null);
            }

            ucCheckoutProcess.EnableDefaultCheckoutProcessTypes = true;
        }
    }


    /// <summary>
    /// Handles checkout process controls mode change event.
    /// </summary>
    /// <param name="isListingMode">Listing mode flag</param>
    private void ucCheckoutProcess_OnModeChanged(bool isListingMode)
    {
        CurrentMaster.HeaderActionsPlaceHolder.Visible = isListingMode;
    }


    /// <summary>
    /// Gets string array representing header actions.
    /// </summary>
    /// <returns>Array of strings</returns>
    private void InitHeaderActions()
    {
        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("CheckoutProcess.NewStep"),
            CommandName = "newStep"
        });

        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("CheckoutProcess.DefaultProcess"),
            OnClientClick = "return ConfirmDefaultProcess();",
            CommandName = "defaultProcess",
            ButtonStyle = ButtonStyle.Default
        });

        if (ConfiguredSiteID != 0)
        {
            CurrentMaster.HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("CheckoutProcess.FromGlobalProcess"),
                OnClientClick = "return ConfirmGlobalProcess();",
                CommandName = "fromGlobalProcess",
                ButtonStyle = ButtonStyle.Default
            });
        }

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Handles actions performed on the master header.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerInvariant())
        {
            case "newstep":
                ucCheckoutProcess.NewStep();
                break;

            case "defaultprocess":
                ucCheckoutProcess.GenerateDefaultProcess();
                break;

            case "fromglobalprocess":
                // Generate only for non-global process
                if (ConfiguredSiteID != 0)
                {
                    ucCheckoutProcess.GenerateFromGlobalProcess();
                }
                break;
        }
    }


    private void ucCheckoutProcess_OnCheckoutProcessDefinitionUpdate(string action)
    {
        // Check 'EcommerceModify' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ECOMMERCE, EcommercePermissions.CONFIGURATION_MODIFY))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, EcommercePermissions.CONFIGURATION_MODIFY);
        }

        switch (action.ToLowerInvariant())
        {
            case "update":
            case "delete":
            case "moveup":
            case "movedown":
                // Update checkout process xml definition in database
                SaveProcess();
                break;

            case "defaultprocess":
                // Set default checkout process

                ucCheckoutProcess.Value = ShoppingCart.DEFAULT_CHECKOUT_PROCESS;
                ucCheckoutProcess.ReloadData();
                SaveProcess();
                break;

            case "fromglobalprocess":
                // Set default checkout process
                if (configuredSite != null)
                {
                    ucCheckoutProcess.Value = ECommerceSettings.CheckoutProcess(null);
                    ucCheckoutProcess.ReloadData();
                    SaveProcess();
                }
                break;
        }
    }


    private void SaveProcess()
    {
        string siteName = null;
        if (configuredSite != null)
        {
            siteName = configuredSite.SiteName;
        }
        SettingsKeyInfoProvider.SetValue(ECommerceSettings.CHECKOUT_PROCESS, siteName, ucCheckoutProcess.Value.ToString());

        ucCheckoutProcess.ShowConfirmation(GetString("General.ChangesSaved"));
    }
}
