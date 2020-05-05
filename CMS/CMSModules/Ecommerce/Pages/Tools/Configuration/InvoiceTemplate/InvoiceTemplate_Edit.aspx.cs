using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("InvoiceTemplate.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_InvoiceTemplate_InvoiceTemplate_Edit : CMSEcommerceConfigurationPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        GlobalObjectsKeyName = ECommerceSettings.USE_GLOBAL_INVOICE;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI element
        var elementName = IsMultiStoreConfiguration ? "Tools.Ecommerce.Invoice" : "Configuration.Invoice";
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, elementName);

        var actions = CurrentMaster.HeaderActions;

        // Save action
        actions.ActionsList.Add(new SaveAction
        {
            CommandName = "lnksave_click"
        });

        // Show "Copy from global" link when not configuring global invoice.
        if (ConfiguredSiteID != 0)
        {
            actions.ActionsList.Add(new HeaderAction
            {
                Text = GetString("com.InvoiceFromGlobal"),
                OnClientClick = "return ConfirmCopyFromGlobal();",
                CommandName = "copyFromGlobal",
                ButtonStyle = ButtonStyle.Default
            });

            // Register javascript to confirm generate 
            string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmInvoiceFromGlobal")) + ");}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromGlobal", ScriptHelper.GetScript(script));
        }

        actions.ActionPerformed += HeaderActions_ActionPerformed;

        if (!RequestHelper.IsPostBack())
        {
            // Read the invoice
            invoiceTemplate.Text = ECommerceSettings.InvoiceTemplate(ConfiguredSiteID);
        }

        // Show "using global settings" info message only if showing global store settings
        if ((ConfiguredSiteID == 0) && (SiteID != 0))
        {
            ShowInformation(GetString("com.UsingGlobalInvoice"));
        }
    }


    /// <summary>
    /// Save button action.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "lnksave_click":
                CheckConfigurationModification();

                // Save template
                SettingsKeyInfoProvider.SetValue(ECommerceSettings.INVOICE_TEMPLATE, ConfiguredSiteID, invoiceTemplate.Text.Trim());

                ShowChangesSaved();

                break;

            case "copyfromglobal":
                // Read global invoice
                invoiceTemplate.Text = ECommerceSettings.InvoiceTemplate(null);

                break;
        }
    }
}
