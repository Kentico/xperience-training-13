using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_WebPartZoneProperties : CMSModalDesignPage
{
    #region "Variables"

    private Control currentControl = null;
    private bool isVariantTab = false;
    private string selectedTab = String.Empty;
    protected Guid instanceGuid = QueryHelper.GetGuid("instanceguid", Guid.Empty);
    protected int templateId = QueryHelper.GetInteger("templateid", 0);
    protected int variantId = QueryHelper.GetInteger("variantid", 0);
    protected int zoneVariantId = QueryHelper.GetInteger("zonevariantid", 0);
    protected VariantModeEnum variantMode = VariantModeFunctions.GetVariantModeEnum(QueryHelper.GetString("variantmode", string.Empty));

    #endregion


    #region "Page methods"

    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Disable check save changes script
        DocumentManager.RegisterSaveChangesScript = false;

        // When displaying an existing variant of a web part, get the variant mode for its original web part
        if ((variantId > 0) || (zoneVariantId > 0))
        {
            PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
            if ((pti != null) && ((pti.TemplateInstance != null)))
            {
                // Get the original webpart and retrieve its variant mode
                WebPartInstance webpartInstance = pti.TemplateInstance.GetWebPart(instanceGuid, zoneVariantId, 0);
                if (webpartInstance != null)
                {
                    variantMode = webpartInstance.VariantMode;
                }
            }
        }
    }


    /// <summary>
    /// Create child controls.
    /// </summary>
    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        // Default control path 
        string controlPath = "~/CMSModules/PortalEngine/Controls/WebParts/WebPartZoneProperties.ascx";
        string ctrlId = "wpzp";

        selectedTab = ValidationHelper.GetString(Request["hdnSelectedTab"], String.Empty);

        // Set personalized control path if selected
        switch (selectedTab.ToLowerCSafe())
        {
            case "webpartzoneproperties.variant":
                controlPath = "~/CMSModules/OnlineMarketing/Controls/WebParts/WebPartZonePersonalized.ascx";
                ctrlId = "pers";
                isVariantTab = true;
                break;
        }

        // Load selected control
        currentControl = LoadUserControl(controlPath);
        currentControl.ID = ctrlId;
        // Add to control collection
        plcDynamicContent.Controls.Add(currentControl);
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Script that switches tabs
        string postBackReference = ControlsHelper.GetPostBackEventReference(pnlUpdate, "");
        postBackReference = postBackReference.Replace("''", "name");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "TabSelect", ScriptHelper.GetScript("function TabSelect(name){ StoreSelectedTab(name); " + postBackReference + "}"));

        // Title
        PageTitle.TitleText = GetString("webpartzone.propertiesheader");
        // Tabs header css class
        CurrentMaster.PanelHeader.CssClass = "WebpartTabsPageHeader";

        // UI Strings
        btnOk.Text = GetString("general.saveandclose");
        chkRefresh.Text = GetString("webpartzone.refresh");

        // Ensure correct tab selection
        ScriptHelper.RegisterStartupScript(this, typeof(string), "SelectedTab", ScriptHelper.GetScript("StoreSelectedTab('" + selectedTab + "');"));
    }


    /// <summary>
    /// Saves the webpart zone properties and closes the window.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        string script = string.Empty;
        bool saved = false;

        // Save webpart properties
        if (currentControl != null)
        {
            if (!isVariantTab)
            {
                CMSModules_PortalEngine_Controls_WebParts_WebPartZoneProperties webPartZonePropertiesElem = currentControl as CMSModules_PortalEngine_Controls_WebParts_WebPartZoneProperties;
                if ((webPartZonePropertiesElem != null) && webPartZonePropertiesElem.Save())
                {
                    saved = true;
                    if (chkRefresh.Checked)
                    {
                        if ((variantId == 0) && (webPartZonePropertiesElem.WebPartZoneInstance != null))
                        {
                            // Display the new variant by default
                            script = "UpdateVariantPosition('Variant_Zone_" + webPartZonePropertiesElem.WebPartZoneInstance.ZoneID + "', -1); ";
                        }

                        script += "RefreshPage();";
                    }
                }
            }
            else
            {
                IWebPartZoneProperties webPartZoneVariantElem = currentControl as IWebPartZoneProperties;
                if (webPartZoneVariantElem != null)
                {
                    saved = webPartZoneVariantElem.Save();
                }
            }
        }

        // Close the window
        if (saved)
        {
            script += "CloseDialog();";
            ScriptHelper.RegisterStartupScript(this, typeof(string), "closeZoneProperties", script, true);
        }
    }

    #endregion
}
